using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json;
using Styra.Opa.OpenApi;
using Styra.Opa.OpenApi.Models.Components;
using Styra.Opa.OpenApi.Models.Errors;
using Styra.Opa.OpenApi.Models.Requests;

namespace Styra.Opa;

/// <summary>
/// OpaClient provides high-level convenience APIs for interacting with an OPA server.
/// It is generally recommended to use this class for most common OPA integrations.
/// </summary>
public class OpaClient
{
    private readonly OpaApiClient opa;

    // Default values to use when creating the SDK instance.
    private static readonly string sdkServerUrl = "http://localhost:8181";
    private readonly string _serverUrl;

    // Internal: Records whether or not to go to fallback mode immediately for
    // batched queries. It is switched over to false as soon as it gets a 404
    // from an OPA server.
    private bool opaSupportsBatchQueryAPI = true;

    // Values to use when generating requests.
    private readonly bool requestPretty = false;
    private readonly bool requestProvenance = false;
    private readonly Explain requestExplain = Explain.Notes;
    private readonly bool requestMetrics = false;
    private readonly bool requestInstrument = false;
    private readonly bool requestStrictBuiltinErrors = false;

    private readonly ILogger _logger;

    private readonly JsonSerializerSettings? _jsonSerializerSettings;

    /// <summary>
    /// Constructs an OpaClient, connecting to a specified server address if provided.
    /// </summary>
    /// <param name="serverUrl">The URL for connecting to the OPA server instance. (default: "http://localhost:8181")</param>
    /// <param name="logger">The ILogger instance to use for this OpaClient. (default: NullLogger)</param>
    /// <param name="jsonSerializerSettings">The Newtonsoft.Json.JsonSerializerSettings to use as the default for serializing inputs for OPA. (default: none)</param>
    public OpaClient(string? serverUrl = null, ILogger<OpaClient>? logger = null, JsonSerializerSettings? jsonSerializerSettings = null)
    {
        opa = new OpaApiClient(serverIndex: 0, serverUrl: serverUrl ?? sdkServerUrl);
        _serverUrl = serverUrl?.TrimEnd('/') ?? sdkServerUrl;
        _logger = logger ?? new NullLogger<OpaClient>();
        _jsonSerializerSettings = jsonSerializerSettings;
    }

    /// <summary>
    /// Simple allow/deny-style check against a rule, using the provided object,
    /// This will round-trip an object through Newtonsoft.JsonConvert, in order
    /// to generate the input object for the eventual OPA API call.
    /// </summary>
    /// <param name="input">The input C# object OPA will use for evaluating the rule.</param>
    /// <param name="path">The rule to evaluate. (Example: "app/rbac")</param>
    /// <param name="jsonSerializerSettings">The Newtonsoft.Json.JsonSerializerSettings object to use for round-tripping the input through JSON serdes. (default: global serializer settings, if any)</param>
    /// <returns>Result, as a boolean</returns>
    public async Task<bool> Check(string path, object? input, JsonSerializerSettings? jsonSerializerSettings = null)
    {
        if (input is null)
        {
            return await Evaluate<bool>(path, input);
        }
        // Round-trip through JSON conversion, such that it becomes an Input.
        var jsonInput = JsonConvert.SerializeObject(input, jsonSerializerSettings ?? _jsonSerializerSettings);
        var roundTrippedInput = JsonConvert.DeserializeObject<Input>(jsonInput, jsonSerializerSettings ?? _jsonSerializerSettings) ?? throw new OpaException(string.Format("could not convert object type to a valid OPA input"));
        return await Evaluate<bool>(path, roundTrippedInput);
    }

    /// <summary>
    /// Evaluate a policy, using the provided object, then coerce the result to
    /// type T. This will round-trip an object through Newtonsoft.JsonConvert,
    /// in order to generate the input object for the eventual OPA API call.
    /// </summary>
    /// <param name="input">The input C# object OPA will use for evaluating the rule.</param>
    /// <param name="path">The rule to evaluate. (Example: "app/rbac")</param>
    /// <param name="jsonSerializerSettings">The Newtonsoft.Json.JsonSerializerSettings object to use for round-tripping the input through JSON serdes. (default: global serializer settings, if any)</param>
    /// <returns>Result, as an instance of T</returns>
    public async Task<T> Evaluate<T>(string path, object? input, JsonSerializerSettings? jsonSerializerSettings = null)
    {
        if (input is null)
        {
            return await QueryMachinery<T>(path, Input.CreateNull());
        }
        // Round-trip through JSON conversion, such that it becomes an Input.
        var jsonInput = JsonConvert.SerializeObject(input, jsonSerializerSettings ?? _jsonSerializerSettings);
        var roundTrippedInput = JsonConvert.DeserializeObject<Input>(jsonInput, jsonSerializerSettings ?? _jsonSerializerSettings) ?? throw new OpaException(string.Format("could not convert object type to a valid OPA input"));
        return await QueryMachinery<T>(path, roundTrippedInput);
    }

    /// <summary>
    /// Evaluate the server's default policy, using the provided object, then
    /// coerce the result to type T. This will round-trip an object through
    /// Newtonsoft.JsonConvert, in order to generate the input object for the
    /// eventual OPA API call.
    /// </summary>
    /// <param name="input">The input C# object OPA will use for evaluating the rule.</param>
    /// <param name="jsonSerializerSettings">The Newtonsoft.Json.JsonSerializerSettings object to use for round-tripping the input through JSON serdes. (default: global serializer settings, if any)</param>
    /// <returns>Result, as an instance of T</returns>
    public async Task<T> EvaluateDefault<T>(object? input, JsonSerializerSettings? jsonSerializerSettings = null)
    {
        if (input is null)
        {
            return await QueryMachineryDefault<T>(Input.CreateNull());
        }
        // Round-trip through JSON conversion, such that it becomes an Input.
        var jsonInput = JsonConvert.SerializeObject(input, jsonSerializerSettings ?? _jsonSerializerSettings);
        var roundTrippedInput = JsonConvert.DeserializeObject<Input>(jsonInput, jsonSerializerSettings ?? _jsonSerializerSettings) ?? throw new OpaException(string.Format("could not convert object type to a valid OPA input"));
        return await QueryMachineryDefault<T>(roundTrippedInput);
    }

    /// <exclude />
    private async Task<T> QueryMachinery<T>(string path, Input input)
    {
        ExecutePolicyWithInputResponse res;
        try
        {
            res = await EvalPolicySingle(path, input);
        }
        catch (Exception e)
        {
            LogMessages.LogQueryError(_logger, path, e.Message);
            var msg = string.Format("executing policy at '{0}' with failed due to exception '{1}'", path, e);
            throw new OpaException(msg, e);
        }

        var result = res.SuccessfulPolicyResponse?.Result;
        if (result is null)
        {
            LogMessages.LogQueryNullResult(_logger, path);
            var msg = string.Format("executing policy at '{0}' succeeded, but OPA did not reply with a result", path);
            throw new OpaException(msg);
        }
        return ConvertResult<T>(result);
    }

    /// <exclude />
    private async Task<T> QueryMachineryDefault<T>(Input input)
    {
        ExecuteDefaultPolicyWithInputResponse res;
        try
        {
            res = await opa.ExecuteDefaultPolicyWithInputAsync(input, requestPretty);
        }
        catch (Exception e)
        {
            LogMessages.LogDefaultQueryError(_logger, e.Message);
            var msg = string.Format("executing server default policy failed due to exception '{0}'", e);
            throw new OpaException(msg, e);
        }

        var result = res.Result;
        if (result is null)
        {
            LogMessages.LogDefaultQueryNullResult(_logger);
            var msg = string.Format("executing server default policy succeeded, but OPA did not reply with a result");
            throw new OpaException(msg);
        }
        return ConvertResult<T>(result);
    }

    /// <summary>
    /// Evaluate a policy, using the provided map of query inputs. Results will
    /// be returned in an identically-structured pair of maps, one for
    /// successful evals, and one for errors. In the event that the OPA server
    /// does not support the /v1/batch/data endpoint, this method will fall back
    /// to performing sequential queries against the OPA server.
    /// </summary>
    /// <param name="path">The rule to evaluate. (Example: "app/rbac")</param>
    /// <param name="inputs">The input Dictionary OPA will use for evaluating the rule. The keys are arbitrary ID strings, the values are the input values intended for each query.</param>
    /// <returns>A pair of mappings, between string keys, and SuccessfulPolicyResponses, or ServerErrors.</returns>
    public async Task<(OpaBatchResults, OpaBatchErrors)> EvaluateBatch(string path, Dictionary<string, Dictionary<string, object>> inputs)
    {
        return await QueryMachineryBatch(path, inputs);
    }

    /// <exclude />
    private async Task<(OpaBatchResults, OpaBatchErrors)> QueryMachineryBatch(string path, Dictionary<string, Dictionary<string, object>> inputs)
    {
        OpaBatchResults successResults;
        OpaBatchErrors failureResults;

        // Attempt using the /v1/batch/data endpoint. If we ever receive a 404, then it's a vanilla OPA instance, and we should skip straight to fallback mode.
        if (opaSupportsBatchQueryAPI)
        {
            var req = new ExecuteBatchPolicyWithInputRequest()
            {
                Path = path,
                RequestBody = new ExecuteBatchPolicyWithInputRequestBody()
                {
                    Inputs = inputs.ToOpaBatchInputRaw(),
                },
                Pretty = requestPretty,
                Provenance = requestProvenance,
                Explain = requestExplain,
                Metrics = requestMetrics,
                Instrument = requestInstrument,
                StrictBuiltinErrors = requestStrictBuiltinErrors,
            };

            // Launch query. The all-errors case is handled in the exception handler block.
            ExecuteBatchPolicyWithInputResponse res;
            try
            {
                res = await opa.ExecuteBatchPolicyWithInputAsync(req);
                switch (res.StatusCode)
                {
                    // All-success case.
                    case 200:
                        successResults = res.BatchSuccessfulPolicyEvaluation!.Responses!.ToOpaBatchResults(); // Should not be null here.
                        failureResults = [];
                        return (successResults, failureResults);
                    // Mixed results case.
                    case 207:
                        var mixedResponses = res.BatchMixedResults?.Responses!; // Should not be null here.
                        var numSuccess = mixedResponses.Values.Where(v => v.Type == ResponsesType.TwoHundred).Count();
                        var numErr = mixedResponses.Values.Where(v => v.Type == ResponsesType.FiveHundred).Count();
                        successResults = new(numSuccess);
                        failureResults = new(numErr);
                        foreach (var (key, value) in mixedResponses)
                        {
                            switch (value.Type.ToString())
                            {
                                case "200":
                                    successResults.Add(key, (OpaResult)value.SuccessfulPolicyResponseWithStatusCode!);
                                    break;
                                case "500":
                                    failureResults.Add(key, (OpaError)value.ServerErrorWithStatusCode!); // Should not be null.
                                    break;
                            }
                        }

                        return (successResults, failureResults);
                    default:
                        // TODO: Throw exception if we reach the end of this block without a successful return.
                        // This *should* never happen. It means we didn't return from the batch or fallback handler blocks earlier.
                        throw new Exception("Impossible error");
                }
            }
            catch (ClientError)
            {
                throw; // Rethrow for the caller to deal with. Request was malformed.
            }
            catch (BatchServerError bse)
            {
                failureResults = bse.Responses!.ToOpaBatchErrors(); // Should not be null here.
                successResults = [];
                return (successResults, failureResults);
            }
            catch (SDKException se) when (se.StatusCode == 404)
            {
                // We know we've got an issue now.
                opaSupportsBatchQueryAPI = false;
                LogMessages.LogBatchQueryFallback(_logger);
                // Fall-through to the "unsupported" case.
            }
        }
        // Implicitly rethrow all other exceptions.

        // Fall back to sequential queries against the OPA instance.
        if (!opaSupportsBatchQueryAPI)
        {
            successResults = [];
            failureResults = [];
            foreach (var (key, value) in inputs)
            {
                try
                {
                    var res = await EvalPolicySingle(path, Input.CreateMapOfAny(value));
                    successResults.Add(key, (OpaResult)res.SuccessfulPolicyResponse!);
                }
                catch (ClientError)
                {
                    throw; // Rethrow for the caller to deal with. Request was malformed.
                }
                catch (Styra.Opa.OpenApi.Models.Errors.ServerError se)
                {
                    failureResults.Add(key, (OpaError)se);
                }
                // Implicitly rethrow all other exceptions.
            }

            // If we have the mixed case, add the HttpStatusCode fields.
            if (successResults.Count > 0 && failureResults.Count > 0)
            {
                // Modifying the dictionary element while iterating is a language feature since 2020, apparently.
                // Ref: https://github.com/dotnet/runtime/pull/34667
                foreach (var key in successResults.Keys)
                {
                    successResults[key].HttpStatusCode = "200";
                }
                foreach (var key in failureResults.Keys)
                {
                    failureResults[key].HttpStatusCode = "500";
                }
            }

            return (successResults, failureResults);
        }

        // This *should* never happen. It means we didn't return from the batch or fallback handler blocks earlier.
        throw new Exception("Impossible error");
    }

    /// <summary>
    /// Evaluate a policy, using the provided map of query inputs. Results will
    /// be returned in an identically-structured pair of maps, one for
    /// successful evals, and one for errors. In the event that the OPA server
    /// does not support the /v1/batch/data endpoint, this method will fall back
    /// to performing sequential queries against the OPA server.
    /// </summary>
    /// <param name="path">The rule to evaluate. (Example: "app/rbac")</param>
    /// <param name="inputs">The input Dictionary OPA will use for evaluating the rule. The keys are arbitrary ID strings, the values are the input values intended for each query.</param>
    /// <returns>A pair of mappings, between string keys, and generic type T, or ServerErrors.</returns>
    public async Task<(OpaBatchResultGeneric<T>, OpaBatchErrors)> EvaluateBatch<T>(string path, Dictionary<string, Dictionary<string, object>> inputs)
    {
        return await QueryMachineryBatch<T>(path, inputs);
    }

    /// <exclude />
    private async Task<(OpaBatchResultGeneric<T>, OpaBatchErrors)> QueryMachineryBatch<T>(string path, Dictionary<string, Dictionary<string, object>> inputs)
    {
        OpaBatchResultGeneric<T> successResults;
        OpaBatchErrors failureResults;

        // Attempt using the /v1/batch/data endpoint. If we ever receive a 404, then it's a vanilla OPA instance, and we should skip straight to fallback mode.
        if (opaSupportsBatchQueryAPI)
        {
            var req = new ExecuteBatchPolicyWithInputRequest()
            {
                Path = path,
                RequestBody = new ExecuteBatchPolicyWithInputRequestBody()
                {
                    Inputs = inputs.ToOpaBatchInputRaw(),
                },
                Pretty = requestPretty,
                Provenance = requestProvenance,
                Explain = requestExplain,
                Metrics = requestMetrics,
                Instrument = requestInstrument,
                StrictBuiltinErrors = requestStrictBuiltinErrors,
            };

            // Launch query. The all-errors case is handled in the exception handler block.
            ExecuteBatchPolicyWithInputResponse res;
            try
            {
                res = await opa.ExecuteBatchPolicyWithInputAsync(req);
                switch (res.StatusCode)
                {
                    // All-success case.
                    case 200:
                        successResults = res.BatchSuccessfulPolicyEvaluation!.Responses!.ToOpaBatchResults<T>(); // Should not be null here.
                        failureResults = [];
                        return (successResults, failureResults);
                    // Mixed results case.
                    case 207:
                        var mixedResponses = res.BatchMixedResults?.Responses!; // Should not be null here.
                        var numSuccess = mixedResponses.Values.Where(v => v.Type == ResponsesType.TwoHundred).Count();
                        var numErr = mixedResponses.Values.Where(v => v.Type == ResponsesType.FiveHundred).Count();
                        successResults = new(numSuccess);
                        failureResults = new(numErr);
                        foreach (var (key, value) in mixedResponses)
                        {
                            switch (value.Type.ToString())
                            {
                                case "200":
                                    successResults.Add(key, ConvertResult<T>(value.SuccessfulPolicyResponseWithStatusCode!.Result!));
                                    break;
                                case "500":
                                    failureResults.Add(key, (OpaError)value.ServerErrorWithStatusCode!); // Should not be null.
                                    break;
                            }
                        }

                        return (successResults, failureResults);
                    default:
                        // TODO: Throw exception if we reach the end of this block without a successful return.
                        // This *should* never happen. It means we didn't return from the batch or fallback handler blocks earlier.
                        throw new Exception("Impossible error");
                }
            }
            catch (ClientError)
            {
                throw; // Rethrow for the caller to deal with. Request was malformed.
            }
            catch (BatchServerError bse)
            {
                failureResults = bse.Responses!.ToOpaBatchErrors(); // Should not be null here.
                successResults = [];
                return (successResults, failureResults);
            }
            catch (SDKException se) when (se.StatusCode == 404)
            {
                // We know we've got an issue now.
                opaSupportsBatchQueryAPI = false;
                LogMessages.LogBatchQueryFallback(_logger);
                // Fall-through to the "unsupported" case.
            }
        }
        // Implicitly rethrow all other exceptions.

        // Fall back to sequential queries against the OPA instance.
        if (!opaSupportsBatchQueryAPI)
        {
            successResults = [];
            failureResults = [];
            foreach (var (key, value) in inputs)
            {
                try
                {
                    var res = await EvalPolicySingle(path, Input.CreateMapOfAny(value));
                    successResults.Add(key, ConvertResult<T>(res.SuccessfulPolicyResponse!.Result!));
                }
                catch (ClientError)
                {
                    throw; // Rethrow for the caller to deal with. Request was malformed.
                }
                catch (Styra.Opa.OpenApi.Models.Errors.ServerError se)
                {
                    failureResults.Add(key, (OpaError)se);
                }
                // Implicitly rethrow all other exceptions.
            }

            // If we have the mixed case, add the HttpStatusCode fields.
            if (successResults.Count > 0 && failureResults.Count > 0)
            {
                // Modifying the dictionary element while iterating is a language feature since 2020, apparently.
                // Ref: https://github.com/dotnet/runtime/pull/34667
                foreach (var key in failureResults.Keys)
                {
                    failureResults[key].HttpStatusCode = "500";
                }
            }

            return (successResults, failureResults);
        }

        // This *should* never happen. It means we didn't return from the batch or fallback handler blocks earlier.
        throw new Exception("Impossible error");
    }

    /// <exclude />
    // Used for the fallback version of QueryMachineryBatch.
    private async Task<ExecutePolicyWithInputResponse> EvalPolicySingle(string path, Input input)
    {
        var req = new ExecutePolicyWithInputRequest()
        {
            Path = path,
            RequestBody = new ExecutePolicyWithInputRequestBody()
            {
                Input = input
            },
            Pretty = requestPretty,
            Provenance = requestProvenance,
            Explain = requestExplain,
            Metrics = requestMetrics,
            Instrument = requestInstrument,
            StrictBuiltinErrors = requestStrictBuiltinErrors,
        };

        return await opa.ExecutePolicyWithInputAsync(req);
    }

    /// <summary>
    /// Uses the Enterprise OPA Compile API to partially evaluate a data
    /// filter policy. Results are returned as a tuple of the form
    /// (data filters (UCAST nodes or SQL), column masking rules (if present)).
    /// </summary>
    /// <param name="path">The rule to use for generating data filters. (Example: "app/rbac")</param>
    /// <param name="input">The input C# object OPA will use for evaluating the data filter policy.</param>
    /// <param name="unknowns">The unknowns to use in partial evaluation of the data filter policy.</param>
    /// <param name="tableMappings">The mappings between tables and columns that should be used for generating the data filters.</param>
    /// <param name="jsonSerializerSettings">The Newtonsoft.Json.JsonSerializerSettings object to use for round-tripping the input through JSON serdes. (default: global serializer settings, if any)</param>
    /// <returns>A ValueTuple of data filters (UCAST nodes or SQL) and column masking rules (if present).</returns>
    /// <exception cref="OpaException"></exception>
    public async Task<(Filters.Filters, Filters.ColumnMasks?)> GetFilters(string path, object? input, List<string>? unknowns = null, Filters.TargetSQLTableMappings? tableMappings = null, JsonSerializerSettings? jsonSerializerSettings = null)
    {
        if (input is null)
        {
            return await CompileMachinery(path, Input.CreateNull(), unknowns, tableMappings);
        }
        // Round-trip through JSON conversion, such that it becomes an Input.
        var jsonInput = JsonConvert.SerializeObject(input, jsonSerializerSettings ?? _jsonSerializerSettings);
        var roundTrippedInput = JsonConvert.DeserializeObject<Input>(jsonInput, jsonSerializerSettings ?? _jsonSerializerSettings) ?? throw new OpaException(string.Format("could not convert object type to a valid OPA input"));

        return await CompileMachinery(path, roundTrippedInput, unknowns, tableMappings);
    }

    /// <exclude />
    // Note(philip): This method allows us to hide the implementation of the
    // `/v1/compile/{path}` query, and will be swapped out for a call into the
    // Speakeasy-generated SDK once upstream bugfixes land.
    private async Task<(Filters.Filters, Filters.ColumnMasks?)> CompileMachinery(string path, Input input, List<string>? unknowns = null, Filters.TargetSQLTableMappings? tableMappings = null)
    {
        // Build URL manually, emulating the query parameter wrangling Speakeasy would normally do for us.
        var compileURL = $"{_serverUrl}/v1/compile/{path}";
        var urlParams = new Dictionary<string, string>
        {
            { "pretty", requestPretty.ToString() },
            { "provenance", requestProvenance.ToString() },
            { "explain", requestExplain.ToString() },
            { "metrics", requestMetrics.ToString() },
            { "instrument", requestInstrument.ToString() },
            { "strict-builtin-errors" ,requestStrictBuiltinErrors.ToString() },
        };
        string queryString = "?" + string.Join("&", urlParams.Select(p => $"{HttpUtility.UrlEncode(p.Key)}={HttpUtility.UrlEncode(p.Value)}"));
        if (queryString != "?") { compileURL += queryString; }
        _logger.LogDebug(string.Format("{0}", compileURL));

        // TODO: Decide if we want to do a polymorphism about this, a discriminated union type about it, or something else entirely.
        // The problem is that we need to make sure we deserialize into something sensible, so we will at a minimum need a response type, even if it's ugly. Ugh.

        try
        {
            using var client = new HttpClient();
            // Set custom Accept header
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/vnd.styra.ucast.linq+json"));

            // Serialize request object to JSON
            var jsonContent = JsonConvert.SerializeObject(input);
            _logger.LogDebug(string.Format("{0}", jsonContent)); // DEBUG
            var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

            // Send the POST request asynchronously
            var response = await client.PostAsync($"{_serverUrl}/v1/compile/{path}", content);

            // Read response content
            var responseContent = await response.Content.ReadAsStringAsync();
            _logger.LogDebug(string.Format("{0}", responseContent)); // DEBUG

            // Handle different status codes
            if (response.IsSuccessStatusCode) // 200 OK
            {
                // Deserialize the successful response
                var result = JsonConvert.DeserializeObject<CompileResultUCAST>(responseContent);
                if (result is null)
                {
                    LogMessages.LogQueryNullResult(_logger, path);
                    var msg = string.Format("executing policy at '{0}' succeeded, but OPA did not reply with a result", path);
                    throw new OpaException(msg);
                }
                // Handle null result cases.
                if (result.Result is null)
                {
                    LogMessages.LogQueryNullResult(_logger, path);
                    var msg = string.Format("executing policy at '{0}' succeeded, but OPA did not reply with valid data filters", path);
                    throw new OpaException(msg);
                }
                var query = JsonConvert.DeserializeObject<Filters.Filters>(JsonConvert.SerializeObject(result.Result.Query));
                if (query is null)
                {
                    LogMessages.LogQueryNullResult(_logger, path);
                    var msg = string.Format("executing policy at '{0}' succeeded, but UCAST data filter was malformed", path);
                    throw new OpaException(msg);
                }
                var masks = JsonConvert.DeserializeObject<Filters.ColumnMasks>(JsonConvert.SerializeObject(result.Result.Masks));
                return (query, masks);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest) // 400
            {
                throw new Exception($"Bad request: {responseContent}");
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError) // 500
            {
                throw new Exception($"Server error: {responseContent}");
            }
            else
            {
                throw new Exception($"Unexpected status code: {response.StatusCode}, Response: {responseContent}");
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Error during HTTP request: {ex.Message}", ex);
        }
    }

    /// <exclude />
    // Designed to respect the nullability of the incoming generic type when possible.
    protected internal static T ConvertResult<T>(Result resultValue)
    {
        // We check to see if T maps to any of the core JSON types.
        // We do the type-switch here, so that high-level clients don't have to.
        // Because the Result members are nullable types, we use type testing
        // with pattern matching to extract a non-null instance of the value if
        // it exists.
        switch (resultValue.Type.ToString())
        {
            case "boolean":
                if (resultValue.Boolean is T defBoolean) { return defBoolean; }
                // If not a perfect match, we return null.
                return IsNullable(typeof(T)) ? default! : throw new OpaException(string.Format("Could not convert bool result to type {0}", typeof(T).FullName));
            case "number":
                if (resultValue.Number is T defNumber) { return defNumber; }
                // If not a perfect match, we return null.
                return IsNullable(typeof(T)) ? default! : throw new OpaException(string.Format("Could not convert number result to type {0}", typeof(T).FullName));
            case "str":
                if (resultValue.Str is T defStr) { return defStr; }
                // If not a perfect match, we return null.
                return IsNullable(typeof(T)) ? default! : throw new OpaException(string.Format("Could not convert string result to type {0}", typeof(T).FullName));
            case "arrayOfAny":
                if (resultValue.ArrayOfAny is T defArray) { return defArray; }
                break; // Fall through to the JSON round-trip path.
            case "mapOfAny":
                if (resultValue.MapOfAny is T defObject) { return defObject; }
                break; // Fall through to the JSON round-trip path.
            case null:
                return IsNullable(typeof(T)) ? default! : throw new OpaException(string.Format("Could not convert null result to type {0}", typeof(T).FullName));
            default:
                break;
        }

        // At this point, T must be a C# object type, and we'll attempt to
        // deserialize to it.
        try
        {
            var temp = JsonConvert.SerializeObject(resultValue);
            var converted = JsonConvert.DeserializeObject<T>(temp);

            if (converted is null)
            {
                return IsNullable(typeof(T)) ? converted! : throw new OpaException(string.Format("Could not convert result array/object to type {0}", typeof(T).FullName));
            }

            // Not null, successful conversion.
            return converted;
        }
        catch (Exception e)
        {
            throw new OpaException(string.Format("Exception occurred while converting result array/object to type {0}", typeof(T).FullName), e);
        }
    }

    /// <exclude />
    private static bool IsNullable(Type type) => Nullable.GetUnderlyingType(type) != null;
}
