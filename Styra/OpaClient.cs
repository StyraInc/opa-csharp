namespace Styra;

using OpenApi;
using OpenApi.Models.Requests;
using OpenApi.Models.Components;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using System;
using System.IO;
using Newtonsoft.Json;

/// <summary>
/// OpaClient provides high-level convenience APIs for interacting with an OPA server.
/// It is generally recommended to use this class for most common OPA integrations.
/// </summary>
public class OpaClient
{
    private OpaApiClient opa;

    // Default values to use when creating the SDK instance.
    private string sdkServerUrl = "http://localhost:8181";

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
    /// <returns>Result, as a boolean, or nil in the case of a query failure.</returns>
    public async Task<bool> check(string path)
    {
        return await evaluate<bool>(path);
    }

    /// <summary>
    /// Simple allow/deny-style check against a rule, using the provided input mapping.
    /// </summary>
    /// <param name="input">The input Dictionary value OPA will use for evaluating the rule.</param>
    /// <param name="path">The rule to evaluate. (Example: "app/rbac")</param>
    /// <returns>Result, as a boolean, or nil in the case of a query failure.</returns>
    /// <remarks>The closest idiomatic type mapping to a JSON Object type for .NET is a Dictionary, so we use that here.</remarks>
    public async Task<bool> check(string path, Dictionary<string, object> input)
    {
        return await evaluate<bool>(path, input);
    }

    /// <summary>
    /// Simple allow/deny-style check against a rule, using the provided input string.
    /// </summary>
    /// <param name="input">The input string OPA will use for evaluating the rule.</param>
    /// <param name="path">The rule to evaluate. (Example: "app/rbac")</param>
    /// <returns>Result, as a boolean, or nil in the case of a query failure.</returns>
    public async Task<bool> check(string path, string input)
    {
        return await evaluate<bool>(path, input);
    }

    /// <summary>
    /// Simple allow/deny-style check against a rule, using the provided input boolean value.
    /// </summary>
    /// <param name="input">The input boolean value OPA will use for evaluating the rule.</param>
    /// <param name="path">The rule to evaluate. (Example: "app/rbac")</param>
    /// <returns>Result, as a boolean, or nil in the case of a query failure.</returns>
    public async Task<bool> check(string path, bool input)
    {
        return await evaluate<bool>(path, input);
    }

    /// <summary>
    /// Simple allow/deny-style check against a rule, using the provided floating-point number.
    /// </summary>
    /// <param name="input">The input floating-point number OPA will use for evaluating the rule.</param>
    /// <param name="path">The rule to evaluate. (Example: "app/rbac")</param>
    /// <returns>Result, as a boolean, or nil in the case of a query failure.</returns>
    public async Task<bool> check(string path, double input)
    {
        return await evaluate<bool>(path, input);
    }

    /// <summary>
    /// Simple allow/deny-style check against a rule, using the provided input list.
    /// </summary>
    /// <param name="input">The input List value OPA will use for evaluating the rule.</param>
    /// <param name="path">The rule to evaluate. (Example: "app/rbac")</param>
    /// <returns>Result, as a boolean, or nil in the case of a query failure.</returns>
    /// <remarks>The closest idiomatic type mapping to a JSON Array type for .NET is a List, so we use that here.</remarks>
    public async Task<bool> check(string path, List<object> input)
    {
        return await evaluate<bool>(path, input);
    }

    /// <summary>
    /// Evaluate a rule, then coerce the result to type T.
    /// </summary>
    /// <param name="path">The rule to evaluate. (Example: "app/rbac")</param>
    /// <returns>Result, as an instance of T, or null in the case of a query failure.</returns>
    public async Task<T?> evaluate<T>(string path)
    {
        return await queryMachinery<T>(path, Input.CreateNull());
    }

    /// <summary>
    /// Evaluate a rule, using the provided input map, then coerce the result to type T.
    /// </summary>
    /// <param name="input">The input Dictionary OPA will use for evaluating the rule.</param>
    /// <param name="path">The rule to evaluate. (Example: "app/rbac")</param>
    /// <returns>Result, as an instance of T, or null in the case of a query failure.</returns>
    public async Task<T?> evaluate<T>(string path, Dictionary<string, object> input)
    {
        return await queryMachinery<T>(path, Input.CreateMapOfany(input));
    }

    /// <summary>
    /// Evaluate a rule, using the provided input string, then coerce the result to type T.
    /// </summary>
    /// <param name="input">The input string OPA will use for evaluating the rule.</param>
    /// <param name="path">The rule to evaluate. (Example: "app/rbac")</param>
    /// <returns>Result, as an instance of T, or null in the case of a query failure.</returns>
    public async Task<T?> evaluate<T>(string path, string input)
    {
        return await queryMachinery<T>(path, Input.CreateStr(input));
    }

    /// <summary>
    /// Evaluate a rule, using the provided input boolean value, then coerce the result to type T.
    /// </summary>
    /// <param name="input">The input boolean value OPA will use for evaluating the rule.</param>
    /// <param name="path">The rule to evaluate. (Example: "app/rbac")</param>
    /// <returns>Result, as an instance of T, or null in the case of a query failure.</returns>
    public async Task<T?> evaluate<T>(string path, bool input)
    {
        return await queryMachinery<T>(path, Input.CreateBoolean(input));
    }

    /// <summary>
    /// Evaluate a rule, using the provided input floating-point number, then coerce the result to type T.
    /// </summary>
    /// <param name="input">The input floating-point number OPA will use for evaluating the rule.</param>
    /// <param name="path">The rule to evaluate. (Example: "app/rbac")</param>
    /// <returns>Result, as an instance of T, or null in the case of a query failure.</returns>
    public async Task<T?> evaluate<T>(string path, double input)
    {
        return await queryMachinery<T>(path, Input.CreateNumber(input));
    }

    /// <summary>
    /// Evaluate a rule, using the provided input boolean value, then coerce the result to type T.
    /// </summary>
    /// <param name="input">The input List value OPA will use for evaluating the rule.</param>
    /// <param name="path">The rule to evaluate. (Example: "app/rbac")</param>
    /// <returns>Result, as an instance of T, or null in the case of a query failure.</returns>
    /// <remarks>The closest idiomatic type mapping to a JSON Array type for .NET is a List, so we use that here.</remarks>
    public async Task<T?> evaluate<T>(string path, List<object> input)
    {
        return await queryMachinery<T>(path, Input.CreateArrayOfany(input));
    }

    /// <exclude />
    private async Task<T?> queryMachinery<T>(string path, Input input)
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

        ExecutePolicyWithInputResponse res;
        try
        {
            res = await opa.ExecutePolicyWithInputAsync(req);
        }
        catch (Exception e)
        {
            string msg = string.Format("executing policy at '{0}' with failed due to exception '{1}'", path, e);
            throw new OpaException(msg, e);
        }

        // We return the default null value for type T if Result is null.
        var result = res.SuccessfulPolicyEvaluation?.Result;
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
            case "arrayOfany":
                if (result.ArrayOfany is T defArray) { return defArray; }
                break;
            case "mapOfany":
                if (result.MapOfany is T defObject) { return defObject; }
                break;
            case null:
            default:
                break;
        }
        // If we could not find a type present in the result that T derives
        // from or is, we return the appropriate null type for T.
        return default;
    }
}
