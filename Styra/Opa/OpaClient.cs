namespace Styra.Opa;

using OpenApi;
using OpenApi.Models.Requests;
using OpenApi.Models.Components;
using OpenApi.Models.Errors;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using System;
using System.IO;
using Newtonsoft.Json;

public class OpaResult : SuccessfulPolicyResponse { }

public class OpaError
{
    [JsonProperty("code")]
    public string Code { get; set; } = default!;

    [JsonProperty("decision_id")]
    public string? DecisionId { get; set; }

    [JsonProperty("message")]
    public string Message { get; set; } = default!;

    [JsonProperty("http_status_code")]
    public string? HttpStatusCode { get; set; }

    public OpaError()
    {

    }
    public OpaError(Styra.Opa.OpenApi.Models.Components.ServerError err)
    {
        Code = err.Code;
        DecisionId = err.DecisionId;
        Message = err.Message;
        HttpStatusCode = err.HttpStatusCode;
    }
    public OpaError(Styra.Opa.OpenApi.Models.Errors.ServerError err)
    {
        Code = err.Code;
        DecisionId = err.DecisionId;
        Message = err.Message;
        HttpStatusCode = "500";
    }
}

public class OpaBatchResults : Dictionary<string, OpaResult> { }

public class OpaBatchErrors : Dictionary<string, OpaError> { }


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
    private bool policyRequestPretty = false;
    private bool policyRequestProvenance = false;
    private Explain policyRequestExplain = Explain.Notes;
    private bool policyRequestMetrics = false;
    private bool policyRequestInstrument = false;
    private bool policyRequestStrictBuiltinErrors = false;

    /// <summary>
    /// Constructs a default OpaClient, connecting to a server on the default host and port.
    /// </summary>
    public OpaClient()
    {
        opa = new OpaApiClient(serverIndex: 0, serverUrl: sdkServerUrl);
    }

    /// <summary>
    /// Constructs an OpaClient using the provided server URL.
    /// </summary>
    /// <param name="serverUrl">The URL for connecting to the OPA server instance.</param>
    public OpaClient(string serverUrl)
    {
        opa = new OpaApiClient(serverIndex: 0, serverUrl: serverUrl);
    }

    /// <summary>
    /// Simple allow/deny-style check against a rule, using the provided input mapping.
    /// </summary>
    /// <param name="path">The rule to evaluate. (Example: "app/rbac")</param>
    /// <returns>Result, as a boolean, or null in the case of a query failure.</returns>
    public async Task<bool> check(string path)
    {
        return await evaluate<bool>(path);
    }

    /// <summary>
    /// Simple allow/deny-style check against a rule, using the provided input boolean value.
    /// </summary>
    /// <param name="input">The input boolean value OPA will use for evaluating the rule.</param>
    /// <param name="path">The rule to evaluate. (Example: "app/rbac")</param>
    /// <returns>Result, as a boolean, or null in the case of a query failure.</returns>
    public async Task<bool> check(string path, bool input)
    {
        return await evaluate<bool>(path, input);
    }

    /// <summary>
    /// Simple allow/deny-style check against a rule, using the provided floating-point number.
    /// </summary>
    /// <param name="input">The input floating-point number OPA will use for evaluating the rule.</param>
    /// <param name="path">The rule to evaluate. (Example: "app/rbac")</param>
    /// <returns>Result, as a boolean, or null in the case of a query failure.</returns>
    public async Task<bool> check(string path, double input)
    {
        return await evaluate<bool>(path, input);
    }

    /// <summary>
    /// Simple allow/deny-style check against a rule, using the provided input string.
    /// </summary>
    /// <param name="input">The input string OPA will use for evaluating the rule.</param>
    /// <param name="path">The rule to evaluate. (Example: "app/rbac")</param>
    /// <returns>Result, as a boolean, or null in the case of a query failure.</returns>
    public async Task<bool> check(string path, string input)
    {
        return await evaluate<bool>(path, input);
    }

    /// <summary>
    /// Simple allow/deny-style check against a rule, using the provided input list.
    /// </summary>
    /// <param name="input">The input List value OPA will use for evaluating the rule.</param>
    /// <param name="path">The rule to evaluate. (Example: "app/rbac")</param>
    /// <returns>Result, as a boolean, or null in the case of a query failure.</returns>
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
    /// <returns>Result, as a boolean, or null in the case of a query failure.</returns>
    /// <remarks>The closest idiomatic type mapping to a JSON Object type for .NET is a Dictionary, so we use that here.</remarks>
    public async Task<bool> check(string path, Dictionary<string, object> input)
    {
        return await evaluate<bool>(path, input);
    }

    /// <summary>
    /// Evaluate a policy, then coerce the result to type T.
    /// </summary>
    /// <param name="path">The rule to evaluate. (Example: "app/rbac")</param>
    /// <returns>Result, as an instance of T, or null in the case of a query failure.</returns>
    public async Task<T?> evaluate<T>(string path)
    {
        return await queryMachinery<T>(path, Input.CreateNull());
    }

    /// <summary>
    /// Evaluate a policy, using the provided input boolean value, then coerce the result to type T.
    /// </summary>
    /// <param name="input">The input boolean value OPA will use for evaluating the rule.</param>
    /// <param name="path">The rule to evaluate. (Example: "app/rbac")</param>
    /// <returns>Result, as an instance of T, or null in the case of a query failure.</returns>
    public async Task<T?> evaluate<T>(string path, bool input)
    {
        return await queryMachinery<T>(path, Input.CreateBoolean(input));
    }

    /// <summary>
    /// Evaluate a policy, using the provided input floating-point number, then coerce the result to type T.
    /// </summary>
    /// <param name="input">The input floating-point number OPA will use for evaluating the rule.</param>
    /// <param name="path">The rule to evaluate. (Example: "app/rbac")</param>
    /// <returns>Result, as an instance of T, or null in the case of a query failure.</returns>
    public async Task<T?> evaluate<T>(string path, double input)
    {
        return await queryMachinery<T>(path, Input.CreateNumber(input));
    }

    /// <summary>
    /// Evaluate a policy, using the provided input string, then coerce the result to type T.
    /// </summary>
    /// <param name="input">The input string OPA will use for evaluating the rule.</param>
    /// <param name="path">The rule to evaluate. (Example: "app/rbac")</param>
    /// <returns>Result, as an instance of T, or null in the case of a query failure.</returns>
    public async Task<T?> evaluate<T>(string path, string input)
    {
        return await queryMachinery<T>(path, Input.CreateStr(input));
    }

    /// <summary>
    /// Evaluate a policy, using the provided input boolean value, then coerce the result to type T.
    /// </summary>
    /// <param name="input">The input List value OPA will use for evaluating the rule.</param>
    /// <param name="path">The rule to evaluate. (Example: "app/rbac")</param>
    /// <returns>Result, as an instance of T, or null in the case of a query failure.</returns>
    /// <remarks>The closest idiomatic type mapping to a JSON Array type for .NET is a List, so we use that here.</remarks>
    public async Task<T?> evaluate<T>(string path, List<object> input)
    {
        return await queryMachinery<T>(path, Input.CreateArrayOfAny(input));
    }

    /// <summary>
    /// Evaluate a policy, using the provided input map, then coerce the result to type T.
    /// </summary>
    /// <param name="input">The input Dictionary OPA will use for evaluating the rule.</param>
    /// <param name="path">The rule to evaluate. (Example: "app/rbac")</param>
    /// <returns>Result, as an instance of T, or null in the case of a query failure.</returns>
    public async Task<T?> evaluate<T>(string path, Dictionary<string, object> input)
    {
        return await queryMachinery<T>(path, Input.CreateMapOfAny(input));
    }

    /// <exclude />
    private async Task<T?> queryMachinery<T>(string path, Input input)
    {
        ExecutePolicyWithInputResponse res;
        try
        {
            res = await evalPolicySingle(path, input);
        }
        catch (Exception e)
        {
            string msg = string.Format("executing policy at '{0}' with failed due to exception '{1}'", path, e);
            throw new OpaException(msg, e);
        }

        // We return the default null value for type T if Result is null.
        var result = res.SuccessfulPolicyResponse?.Result;
        if (result is null)
        {
            return default;
        }

        // We do the type-switch here, so that high-level clients don't have to.
        // Because the Result members are nullable types, we use type testing
        // with pattern matching to extract a non-null instance of the value if
        // it exists.
        switch (result.Type.ToString())
        {
            case "boolean":
                if (result.Boolean is T defBoolean) { return defBoolean; }
                break;
            case "number":
                if (result.Number is T defNumber) { return defNumber; }
                break;
            case "str":
                if (result.Str is T defStr) { return defStr; }
                break;
            case "arrayOfAny":
                if (result.ArrayOfAny is T defArray) { return defArray; }
                break;
            case "mapOfAny":
                if (result.MapOfAny is T defObject) { return defObject; }
                break;
            case null:
            default:
                break;
        }
        // If we could not find a type present in the result that T derives
        // from or is, we return the appropriate null type for T.
        return default;
    }

    /// <summary>
    /// Evaluate the server's default policy, then coerce the result to type T.
    /// </summary>
    /// <returns>Result, as an instance of T, or null in the case of a query failure.</returns>
    public async Task<T?> evaluateDefault<T>()
    {
        return await queryMachineryDefault<T>(Input.CreateNull());
    }

    /// <summary>
    /// Evaluate the server's default policy, using the provided input boolean value, then coerce the result to type T.
    /// </summary>
    /// <param name="input">The input boolean value OPA will use for evaluating the rule.</param>
    /// <returns>Result, as an instance of T, or null in the case of a query failure.</returns>
    public async Task<T?> evaluateDefault<T>(bool input)
    {
        return await queryMachineryDefault<T>(Input.CreateBoolean(input));
    }

    /// <summary>
    /// Evaluate the server's default policy, using the provided input floating-point number, then coerce the result to type T.
    /// </summary>
    /// <param name="input">The input floating-point number OPA will use for evaluating the rule.</param>
    /// <returns>Result, as an instance of T, or null in the case of a query failure.</returns>
    public async Task<T?> evaluateDefault<T>(double input)
    {
        return await queryMachineryDefault<T>(Input.CreateNumber(input));
    }

    /// <summary>
    /// Evaluate the server's default policy, using the provided input string, then coerce the result to type T.
    /// </summary>
    /// <param name="input">The input string OPA will use for evaluating the rule.</param>
    /// <returns>Result, as an instance of T, or null in the case of a query failure.</returns>
    public async Task<T?> evaluateDefault<T>(string input)
    {
        return await queryMachineryDefault<T>(Input.CreateStr(input));
    }

    /// <summary>
    /// Evaluate the server's default policy, using the provided input boolean value, then coerce the result to type T.
    /// </summary>
    /// <param name="input">The input List value OPA will use for evaluating the rule.</param>
    /// <returns>Result, as an instance of T, or null in the case of a query failure.</returns>
    /// <remarks>The closest idiomatic type mapping to a JSON Array type for .NET is a List, so we use that here.</remarks>
    public async Task<T?> evaluateDefault<T>(List<object> input)
    {
        return await queryMachineryDefault<T>(Input.CreateArrayOfAny(input));
    }

    /// <summary>
    /// Evaluate the server's default policy, using the provided input map, then coerce the result to type T.
    /// </summary>
    /// <param name="input">The input Dictionary OPA will use for evaluating the rule.</param>
    /// <returns>Result, as an instance of T, or null in the case of a query failure.</returns>
    public async Task<T?> evaluateDefault<T>(Dictionary<string, object> input)
    {
        return await queryMachineryDefault<T>(Input.CreateMapOfAny(input));
    }

    /// <exclude />
    private async Task<T?> queryMachineryDefault<T>(Input input)
    {
        ExecuteDefaultPolicyWithInputResponse res;
        try
        {
            res = await opa.ExecuteDefaultPolicyWithInputAsync(input, policyRequestPretty);
        }
        catch (Exception e)
        {
            string msg = string.Format("executing server default policy failed due to exception '{0}'", e);
            throw new OpaException(msg, e);
        }

        // We return the default null value for type T if Result is null.
        var result = res.Result;
        if (result is null)
        {
            return default;
        }

        // We do the type-switch here, so that high-level clients don't have to.
        // Because the Result members are nullable types, we use type testing
        // with pattern matching to extract a non-null instance of the value if
        // it exists.
        switch (result.Type.ToString())
        {
            case "boolean":
                if (result.Boolean is T defBoolean) { return defBoolean; }
                break;
            case "number":
                if (result.Number is T defNumber) { return defNumber; }
                break;
            case "str":
                if (result.Str is T defStr) { return defStr; }
                break;
            case "arrayOfAny":
                if (result.ArrayOfAny is T defArray) { return defArray; }
                break;
            case "mapOfAny":
                if (result.MapOfAny is T defObject) { return defObject; }
                break;
            case null:
            default:
                break;
        }
        // If we could not find a type present in the result that T derives
        // from or is, we return the appropriate null type for T.
        return default;
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
    public async Task<(OpaBatchResults, OpaBatchErrors)> evaluateBatch(string path, Dictionary<string, Input> inputs)
    {
        return await queryMachineryBatch(path, inputs);
    }

    /// <exclude />
    private async Task<(OpaBatchResults, OpaBatchErrors)> queryMachineryBatch(string path, Dictionary<string, Input> inputs)
    {
        OpaBatchResults successResults = new();
        OpaBatchErrors failureResults = new();

        // Attempt using the /v1/batch/data endpoint. If we ever receive a 404, then it's a vanilla OPA instance, and we should skip straight to fallback mode.
        if (opaSupportsBatchQueryAPI)
        {
            ExecuteBatchPolicyWithInputRequest req = new ExecuteBatchPolicyWithInputRequest()
            {
                Path = path,
                RequestBody = new ExecuteBatchPolicyWithInputRequestBody()
                {
                    Inputs = inputs,
                },
                Pretty = policyRequestPretty,
                Provenance = policyRequestProvenance,
                Explain = policyRequestExplain,
                Metrics = policyRequestMetrics,
                Instrument = policyRequestInstrument,
                StrictBuiltinErrors = policyRequestStrictBuiltinErrors,
            };

            // Launch query. The all-errors case is handled in the exception handler block.
            // The other two possibilities (mixed and all-success) are handled further down.
            ExecuteBatchPolicyWithInputResponse res;
            try
            {
                res = await opa.ExecuteBatchPolicyWithInputAsync(req);
            }
            catch (Exception ex)
            {
                if (ex is ClientError ce)
                {
                    throw ce; // Rethrow for the caller to deal with. Request was malformed.
                }
                else if (ex is BatchServerError bse)
                {
                    foreach (var (key, value) in bse.Responses!) // Should not be null here.
                    {
                        failureResults.Add(key, new OpaError(value));
                    }
                    return (successResults, failureResults);
                }
                else
                {
                    throw; // Something unexpected blew up. Rethrow.
                }
            }

            // All-success case.
            if (res.StatusCode == 200)
            {
                foreach (var (key, value) in res.BatchSuccessfulPolicyEvaluation?.Responses!) // Should not be null here.
                {
                    successResults.Add(key, value as OpaResult);
                }
                return (successResults, failureResults);
            }

            // Mixed results case.
            if (res.StatusCode == 207)
            {
                Dictionary<string, Responses> mixedResponses = res.BatchMixedResults?.Responses!; // Should not be null here.
                foreach (var (key, value) in mixedResponses)
                {
                    switch (value.Type.ToString())
                    {
                        case "200":
                            successResults.Add(key, new OpaResult()
                            {
                                DecisionId = value.ResponsesSuccessfulPolicyResponse?.DecisionId,
                                Metrics = value.ResponsesSuccessfulPolicyResponse?.Metrics,
                                Provenance = value.ResponsesSuccessfulPolicyResponse?.Provenance,
                                Result = value.ResponsesSuccessfulPolicyResponse?.Result,
                            });
                            break;
                        case "500":
                            failureResults.Add(key, new OpaError(value.ServerError!)); // Should not be null.
                            break;
                    }
                }
                return (successResults, failureResults);
            }
            // TODO: Throw exception if we reach the end of this block without a successful return.
        }

        // Fall back to sequential queries against the OPA instance.
        if (!opaSupportsBatchQueryAPI)
        {
            foreach (var (key, value) in inputs)
            {
                ExecutePolicyWithInputResponse res;
                try
                {
                    res = await evalPolicySingle(path, value);
                }
                catch (Exception e)
                {
                    string msg = string.Format("executing policy at '{0}' with failed due to exception '{1}'", path, e);
                    throw new OpaException(msg, e);
                }

                // We return the default null value for type T if Result is null.
                var result = res.SuccessfulPolicyResponse?.Result;
                if (result is null)
                {
                    return default;
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
        ExecutePolicyWithInputRequest req = new ExecutePolicyWithInputRequest()
        {
            Path = path,
            RequestBody = new ExecutePolicyWithInputRequestBody()
            {
                Input = input
            },
            Pretty = policyRequestPretty,
            Provenance = policyRequestProvenance,
            Explain = policyRequestExplain,
            Metrics = policyRequestMetrics,
            Instrument = policyRequestInstrument,
            StrictBuiltinErrors = policyRequestStrictBuiltinErrors,
        };

        return await opa.ExecutePolicyWithInputAsync(req);
    }
}