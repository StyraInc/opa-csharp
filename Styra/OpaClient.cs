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
    public bool check(string path)
    {
        return query<bool>(path);
    }

    /// <summary>
    /// Simple allow/deny-style check against a rule, using the provided input mapping.
    /// </summary>
    /// <param name="input">The input Dictionary OPA will use for evaluating the rule.</param>
    /// <param name="path">The rule to evaluate. (Example: "app/rbac")</param>
    /// <returns>Result, as a boolean, or nil in the case of a query failure.</returns>
    /// <remarks>The closest idiomatic type mapping to a JSON Object type for .NET is a Dictionary, so we use that here.</remarks>
    public bool check(Dictionary<string, object> input, string path)
    {
        return query<bool>(input, path);
    }

    /// <summary>
    /// Simple allow/deny-style check against a rule, using the provided input string.
    /// </summary>
    /// <param name="input">The input string OPA will use for evaluating the rule.</param>
    /// <param name="path">The rule to evaluate. (Example: "app/rbac")</param>
    /// <returns>Result, as a boolean, or nil in the case of a query failure.</returns>
    public bool check(string input, string path)
    {
        return query<bool>(input, path);
    }

    /// <summary>
    /// Simple allow/deny-style check against a rule, using the provided input boolean value.
    /// </summary>
    /// <param name="input">The input boolean value OPA will use for evaluating the rule.</param>
    /// <param name="path">The rule to evaluate. (Example: "app/rbac")</param>
    /// <returns>Result, as a boolean, or nil in the case of a query failure.</returns>
    public bool check(bool input, string path)
    {
        return query<bool>(input, path);
    }

    /// <summary>
    /// Simple allow/deny-style check against a rule, using the provided floating-point number.
    /// </summary>
    /// <param name="input">The input floating-point number OPA will use for evaluating the rule.</param>
    /// <param name="path">The rule to evaluate. (Example: "app/rbac")</param>
    /// <returns>Result, as a boolean, or nil in the case of a query failure.</returns>
    public bool check(double input, string path)
    {
        return query<bool>(input, path);
    }

    /// <summary>
    /// Simple allow/deny-style check against a rule, using the provided input list.
    /// </summary>
    /// <param name="input">The input list OPA will use for evaluating the rule.</param>
    /// <param name="path">The rule to evaluate. (Example: "app/rbac")</param>
    /// <returns>Result, as a boolean, or nil in the case of a query failure.</returns>
    /// <remarks>The closest idiomatic type mapping to a JSON Array type for .NET is a List, so we use that here.</remarks>
    public bool check(List<object> input, string path)
    {
        return query<bool>(input, path);
    }

    /// <summary>
    /// Evaluate a rule, then coerce the result to type T.
    /// </summary>
    /// <param name="path">The rule to evaluate. (Example: "app/rbac")</param>
    /// <returns>Result, as a boolean, or nil in the case of a query failure.</returns>
    public T? query<T>(string path)
    {
        return queryMachinery<T>(Input.CreateNull(), path);
    }

    /// <summary>
    /// Evaluate a rule, using the provided input map, then coerce the result to type T.
    /// </summary>
    /// <param name="input">The input Dictionary OPA will use for evaluating the rule.</param>
    /// <param name="path">The rule to evaluate. (Example: "app/rbac")</param>
    /// <returns>Result, as a boolean, or nil in the case of a query failure.</returns>
    public T? query<T>(Dictionary<string, object> input, string path)
    {
        return queryMachinery<T>(Input.CreateMapOfany(input), path);
    }

    /// <summary>
    /// Evaluate a rule, using the provided input string, then coerce the result to type T.
    /// </summary>
    /// <param name="input">The input string OPA will use for evaluating the rule.</param>
    /// <param name="path">The rule to evaluate. (Example: "app/rbac")</param>
    /// <returns>Result, as a boolean, or nil in the case of a query failure.</returns>
    public T? query<T>(string input, string path)
    {
        return queryMachinery<T>(Input.CreateStr(input), path);
    }

    /// <summary>
    /// Evaluate a rule, using the provided input boolean value, then coerce the result to type T.
    /// </summary>
    /// <param name="input">The input boolean value OPA will use for evaluating the rule.</param>
    /// <param name="path">The rule to evaluate. (Example: "app/rbac")</param>
    /// <returns>Result, as a boolean, or nil in the case of a query failure.</returns>
    public T? query<T>(bool input, string path)
    {
        return queryMachinery<T>(Input.CreateBoolean(input), path);
    }

    /// <summary>
    /// Evaluate a rule, using the provided input floating-point number, then coerce the result to type T.
    /// </summary>
    /// <param name="input">The input floating-point number OPA will use for evaluating the rule.</param>
    /// <param name="path">The rule to evaluate. (Example: "app/rbac")</param>
    /// <returns>Result, as a boolean, or nil in the case of a query failure.</returns>
    public T? query<T>(double input, string path)
    {
        return queryMachinery<T>(Input.CreateNumber(input), path);
    }

    /// <summary>
    /// Evaluate a rule, using the provided input boolean value, then coerce the result to type T.
    /// </summary>
    /// <param name="input">The input boolean value OPA will use for evaluating the rule.</param>
    /// <param name="path">The rule to evaluate. (Example: "app/rbac")</param>
    /// <returns>Result, as a boolean, or nil in the case of a query failure.</returns>
    /// <remarks>The closest idiomatic type mapping to a JSON Array type for .NET is a List, so we use that here.</remarks>
    public T? query<T>(List<object> input, string path)
    {
        return queryMachinery<T>(Input.CreateArrayOfany(input), path);
    }

    /// <exclude />
    private T? queryMachinery<T>(Input input, string path)
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
            res = Task.Run(() => opa.ExecutePolicyWithInputAsync(req)).GetAwaiter().GetResult();
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
