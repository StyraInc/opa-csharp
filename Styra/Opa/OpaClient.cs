using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json;
using Styra.Opa.OpenApi.Models.Components;
using Styra.Opa.OpenApi;
using Styra.Opa.OpenApi.Models.Requests;
using Styra.Opa.OpenApi.Models.Errors;

namespace Styra.Opa;

/// <summary>
/// OpaClient provides high-level convenience APIs for interacting with an OPA server.
/// It is generally recommended to use this class for most common OPA integrations.
/// </summary>
public class OpaClient
{
    private OpaApiClient opa;

    // Default values to use when creating the SDK instance.
    private string sdkServerUrl = "http://localhost:8181";

    // Internal: Records whether or not to go to fallback mode immediately for
    // batched queries. It is switched over to false as soon as it gets a 404
    // from an OPA server.
    private bool opaSupportsBatchQueryAPI = true;

    // Values to use when generating requests.
    private bool requestPretty = false;
    private bool requestProvenance = false;
    private Explain requestExplain = Explain.Notes;
    private bool requestMetrics = false;
    private bool requestInstrument = false;
    private bool requestStrictBuiltinErrors = false;

    private readonly ILogger _logger;

    /// <summary>
    /// Constructs a default OpaClient, connecting to a specified server address.
    /// </summary>
    public OpaClient()
    {
        opa = new OpaApiClient(serverIndex: 0, serverUrl: sdkServerUrl);
        _logger = new NullLogger<OpaClient>();
    }

    /// <summary>
    /// Constructs an OpaClient using the provided server URL.
    /// </summary>
    /// <param name="serverUrl">The URL for connecting to the OPA server instance.</param>
    public OpaClient(string serverUrl)
    {
        opa = new OpaApiClient(serverIndex: 0, serverUrl: serverUrl);
        _logger = new NullLogger<OpaClient>();
    }

    /// <summary>
    /// Constructs a default OpaClient, connecting to a specified server address.
    /// </summary>
    /// <param name="logger">The ILogger instance to use for this OpaClient.</param>
    public OpaClient(ILogger<OpaClient> logger)
    {
        opa = new OpaApiClient(serverIndex: 0, serverUrl: sdkServerUrl);
        _logger = logger;
    }

    /// <summary>
    /// Constructs a default OpaClient, connecting to a specified server address.
    /// </summary>
    /// <param name="serverUrl">The URL for connecting to the OPA server instance.</param>
    /// <param name="logger">The ILogger instance to use for this OpaClient.</param>
    public OpaClient(string serverUrl, ILogger<OpaClient> logger)
    {
        opa = new OpaApiClient(serverIndex: 0, serverUrl: serverUrl);
        _logger = logger;
    }

    /// <summary>
    /// Simple allow/deny-style check against a rule, using the provided input mapping.
    /// </summary>
    /// <param name="path">The rule to evaluate. (Example: "app/rbac")</param>
    /// <returns>Result, as a boolean</returns>
    public async Task<bool> check(string path)
    {
        return await evaluate<bool>(path);
    }

    /// <summary>
    /// Simple allow/deny-style check against a rule, using the provided input boolean value.
    /// </summary>
    /// <param name="input">The input boolean value OPA will use for evaluating the rule.</param>
    /// <param name="path">The rule to evaluate. (Example: "app/rbac")</param>
    /// <returns>Result, as a boolean</returns>
    public async Task<bool> check(string path, bool input)
    {
        return await evaluate<bool>(path, input);
    }

    /// <summary>
    /// Simple allow/deny-style check against a rule, using the provided floating-point number.
    /// </summary>
    /// <param name="input">The input floating-point number OPA will use for evaluating the rule.</param>
    /// <param name="path">The rule to evaluate. (Example: "app/rbac")</param>
    /// <returns>Result, as a boolean</returns>
    public async Task<bool> check(string path, double input)
    {
        return await evaluate<bool>(path, input);
    }

    /// <summary>
    /// Simple allow/deny-style check against a rule, using the provided input string.
    /// </summary>
    /// <param name="input">The input string OPA will use for evaluating the rule.</param>
    /// <param name="path">The rule to evaluate. (Example: "app/rbac")</param>
    /// <returns>Result, as a boolean</returns>
    public async Task<bool> check(string path, string input)
    {
        return await evaluate<bool>(path, input);
    }

    /// <summary>
    /// Simple allow/deny-style check against a rule, using the provided input list.
    /// </summary>
    /// <param name="input">The input List value OPA will use for evaluating the rule.</param>
    /// <param name="path">The rule to evaluate. (Example: "app/rbac")</param>
    /// <returns>Result, as a boolean</returns>
    /// <remarks>The closest idiomatic type mapping to a JSON Array type for .NET is a List, so we use that here.</remarks>
    public async Task<bool> check(string path, List<object> input)
    {
        return await evaluate<bool>(path, input);
    }

    /// <summary>
    /// Simple allow/deny-style check against a rule, using the provided input mapping.
    /// </summary>
    /// <param name="input">The input Dictionary value OPA will use for evaluating the rule.</param>
    /// <param name="path">The rule to evaluate. (Example: "app/rbac")</param>
    /// <returns>Result, as a boolean</returns>
    /// <remarks>The closest idiomatic type mapping to a JSON Object type for .NET is a Dictionary, so we use that here.</remarks>
    public async Task<bool> check(string path, Dictionary<string, object> input)
    {
        return await evaluate<bool>(path, input);
    }

    /// <summary>
    /// Evaluate a policy, then coerce the result to type T.
    /// </summary>
    /// <param name="path">The rule to evaluate. (Example: "app/rbac")</param>
    /// <returns>Result, as an instance of T</returns>
    public async Task<T> evaluate<T>(string path)
    {
        return await queryMachinery<T>(path, Input.CreateNull());
    }

    /// <summary>
    /// Evaluate a policy, using the provided input boolean value, then coerce the result to type T.
    /// </summary>
    /// <param name="input">The input boolean value OPA will use for evaluating the rule.</param>
    /// <param name="path">The rule to evaluate. (Example: "app/rbac")</param>
    /// <returns>Result, as an instance of T</returns>
    public async Task<T> evaluate<T>(string path, bool input)
    {
        return await queryMachinery<T>(path, Input.CreateBoolean(input));
    }

    /// <summary>
    /// Evaluate a policy, using the provided input floating-point number, then coerce the result to type T.
    /// </summary>
    /// <param name="input">The input floating-point number OPA will use for evaluating the rule.</param>
    /// <param name="path">The rule to evaluate. (Example: "app/rbac")</param>
    /// <returns>Result, as an instance of T</returns>
    public async Task<T> evaluate<T>(string path, double input)
    {
        return await queryMachinery<T>(path, Input.CreateNumber(input));
    }

    /// <summary>
    /// Evaluate a policy, using the provided input string, then coerce the result to type T.
    /// </summary>
    /// <param name="input">The input string OPA will use for evaluating the rule.</param>
    /// <param name="path">The rule to evaluate. (Example: "app/rbac")</param>
    /// <returns>Result, as an instance of T</returns>
    public async Task<T> evaluate<T>(string path, string input)
    {
        return await queryMachinery<T>(path, Input.CreateStr(input));
    }

    /// <summary>
    /// Evaluate a policy, using the provided input boolean value, then coerce the result to type T.
    /// </summary>
    /// <param name="input">The input List value OPA will use for evaluating the rule.</param>
    /// <param name="path">The rule to evaluate. (Example: "app/rbac")</param>
    /// <returns>Result, as an instance of T</returns>
    /// <remarks>The closest idiomatic type mapping to a JSON Array type for .NET is a List, so we use that here.</remarks>
    public async Task<T> evaluate<T>(string path, List<object> input)
    {
        return await queryMachinery<T>(path, Input.CreateArrayOfAny(input));
    }

    /// <summary>
    /// Evaluate a policy, using the provided input map, then coerce the result to type T.
    /// </summary>
    /// <param name="input">The input Dictionary OPA will use for evaluating the rule.</param>
    /// <param name="path">The rule to evaluate. (Example: "app/rbac")</param>
    /// <returns>Result, as an instance of T</returns>
    public async Task<T> evaluate<T>(string path, Dictionary<string, object> input)
    {
        return await queryMachinery<T>(path, Input.CreateMapOfAny(input));
    }

    /// <summary>
    /// Evaluate a policy, using the provided object, then coerce the result to
    /// type T. This will round-trip an object through Newtonsoft.JsonConvert,
    /// in order to generate the input object for the eventual OPA API call.
    /// </summary>
    /// <param name="input">The input C# object OPA will use for evaluating the rule.</param>
    /// <param name="path">The rule to evaluate. (Example: "app/rbac")</param>
    /// <returns>Result, as an instance of T</returns>
    public async Task<T> evaluate<T>(string path, object input)
    {
        // Round-trip through JSON conversion, such that it becomes an Input.
        var jsonInput = JsonConvert.SerializeObject(input);
        var roundTrippedInput = JsonConvert.DeserializeObject<Input>(jsonInput) ?? throw new OpaException(string.Format("could not convert object type to a valid OPA input"));

        return await queryMachinery<T>(path, roundTrippedInput);
    }

    /// <exclude />
    private async Task<T> queryMachinery<T>(string path, Input input)
    {
        ExecutePolicyWithInputResponse res;
        try
        {
            res = await evalPolicySingle(path, input);
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
        return convertResult<T>(result);
    }

    /// <summary>
    /// Evaluate the server's default policy, then coerce the result to type T.
    /// </summary>
    /// <returns>Result, as an instance of T</returns>
    public async Task<T> evaluateDefault<T>()
    {
        return await queryMachineryDefault<T>(Input.CreateNull());
    }

    /// <summary>
    /// Evaluate the server's default policy, using the provided input boolean value, then coerce the result to type T.
    /// </summary>
    /// <param name="input">The input boolean value OPA will use for evaluating the rule.</param>
    /// <returns>Result, as an instance of T</returns>
    public async Task<T> evaluateDefault<T>(bool input)
    {
        return await queryMachineryDefault<T>(Input.CreateBoolean(input));
    }

    /// <summary>
    /// Evaluate the server's default policy, using the provided input floating-point number, then coerce the result to type T.
    /// </summary>
    /// <param name="input">The input floating-point number OPA will use for evaluating the rule.</param>
    /// <returns>Result, as an instance of T</returns>
    public async Task<T> evaluateDefault<T>(double input)
    {
        return await queryMachineryDefault<T>(Input.CreateNumber(input));
    }

    /// <summary>
    /// Evaluate the server's default policy, using the provided input string, then coerce the result to type T.
    /// </summary>
    /// <param name="input">The input string OPA will use for evaluating the rule.</param>
    /// <returns>Result, as an instance of T</returns>
    public async Task<T> evaluateDefault<T>(string input)
    {
        return await queryMachineryDefault<T>(Input.CreateStr(input));
    }

    /// <summary>
    /// Evaluate the server's default policy, using the provided input boolean value, then coerce the result to type T.
    /// </summary>
    /// <param name="input">The input List value OPA will use for evaluating the rule.</param>
    /// <returns>Result, as an instance of T</returns>
    /// <remarks>The closest idiomatic type mapping to a JSON Array type for .NET is a List, so we use that here.</remarks>
    public async Task<T> evaluateDefault<T>(List<object> input)
    {
        return await queryMachineryDefault<T>(Input.CreateArrayOfAny(input));
    }

    /// <summary>
    /// Evaluate the server's default policy, using the provided input map, then coerce the result to type T.
    /// </summary>
    /// <param name="input">The input Dictionary OPA will use for evaluating the rule.</param>
    /// <returns>Result, as an instance of T</returns>
    public async Task<T> evaluateDefault<T>(Dictionary<string, object> input)
    {
        return await queryMachineryDefault<T>(Input.CreateMapOfAny(input));
    }

    /// <summary>
    /// Evaluate the server's default policy, using the provided object, then
    /// coerce the result to type T. This will round-trip an object through
    /// Newtonsoft.JsonConvert, in order to generate the input object for the
    /// eventual OPA API call.
    /// </summary>
    /// <param name="input">The input C# object OPA will use for evaluating the rule.</param>
    /// <returns>Result, as an instance of T</returns>
    public async Task<T> evaluateDefault<T>(object input)
    {
        // Round-trip through JSON conversion, such that it becomes an Input.
        var jsonInput = JsonConvert.SerializeObject(input);
        var roundTrippedInput = JsonConvert.DeserializeObject<Input>(jsonInput) ?? throw new OpaException(string.Format("could not convert object type to a valid OPA input"));

        return await queryMachineryDefault<T>(roundTrippedInput);
    }

    /// <exclude />
    private async Task<T> queryMachineryDefault<T>(Input input)
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
        return convertResult<T>(result);
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
    public async Task<(OpaBatchResults, OpaBatchErrors)> evaluateBatch(string path, Dictionary<string, Dictionary<string, object>> inputs)
    {
        return await queryMachineryBatch(path, inputs);
    }

    /// <exclude />
    private async Task<(OpaBatchResults, OpaBatchErrors)> queryMachineryBatch(string path, Dictionary<string, Dictionary<string, object>> inputs)
    {
        OpaBatchResults successResults = new();
        OpaBatchErrors failureResults = new();

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
                        return (successResults, failureResults);
                    // Mixed results case.
                    case 207:
                        var mixedResponses = res.BatchMixedResults?.Responses!; // Should not be null here.
                        foreach (var (key, value) in mixedResponses)
                        {
                            switch (value.Type.ToString())
                            {
                                case "200":
                                    successResults.Add(key, (OpaResult)value.ResponsesSuccessfulPolicyResponse!);
                                    break;
                                case "500":
                                    failureResults.Add(key, (OpaError)value.ServerError!); // Should not be null.
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
            catch (ClientError ce)
            {
                throw ce; // Rethrow for the caller to deal with. Request was malformed.
            }
            catch (BatchServerError bse)
            {
                failureResults = bse.Responses!.ToOpaBatchErrors(); // Should not be null here.
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
            foreach (var (key, value) in inputs)
            {
                try
                {
                    var res = await evalPolicySingle(path, Input.CreateMapOfAny(value));
                    successResults.Add(key, (OpaResult)res.SuccessfulPolicyResponse!);
                }
                catch (ClientError ce)
                {
                    throw ce; // Rethrow for the caller to deal with. Request was malformed.
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

    /// <exclude />
    private async Task<ExecutePolicyWithInputResponse> evalPolicySingle(string path, Input input)
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

    /// <exclude />
    // Designed to respect the nullability of the incoming generic type when possible.
    private static T convertResult<T>(Result resultValue)
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
