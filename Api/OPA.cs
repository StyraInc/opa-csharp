namespace Api;

using Api.SDK;
using Api.SDK.Models.Requests;
using Api.SDK.Models.Components;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using System;
using System.IO;
using Newtonsoft.Json;

public class OPA
{
    private Opa opa;

    // Default values to use when creating the SDK instance.
    private string sdkServerURL = "http://localhost:8181";

    // Values to use when generating requests.
    private bool policyRequestPretty = false;
    private bool policyRequestProvenance = false;
    private Explain policyRequestExplain = Explain.Notes;
    private bool policyRequestMetrics = false;
    private bool policyRequestInstrument = false;
    private bool policyRequestStrictBuiltinErrors = false;

    public OPA()
    {
        this.opa = new Opa(serverIndex: 0, serverUrl: sdkServerURL);
    }

    public OPA(string serverURL)
    {
        this.opa = new Opa(serverIndex: 0, serverUrl: serverURL);
    }

    public bool? check(Dictionary<string, object> input, string path)
    {
        return query<bool>(input, path);
    }

    public bool? check(string input, string path)
    {
        return query<bool>(input, path);
    }

    public bool? check(bool input, string path)
    {
        return query<bool>(input, path);
    }

    public bool? check(double input, string path)
    {
        return query<bool>(input, path);
    }

    public bool? check(List<object> input, string path)
    {
        return query<bool>(input, path);
    }

    public T? query<T>(Dictionary<string, object> input, string path)
    {
        return queryMachinery<T>(Input.CreateMapOfany(input), path);
    }

    public T? query<T>(string input, string path)
    {
        return queryMachinery<T>(Input.CreateStr(input), path);
    }

    public T? query<T>(bool input, string path)
    {
        return queryMachinery<T>(Input.CreateBoolean(input), path);
    }

    public T? query<T>(double input, string path)
    {
        return queryMachinery<T>(Input.CreateNumber(input), path);
    }

    public T? query<T>(List<object> input, string path)
    {
        return queryMachinery<T>(Input.CreateArrayOfany(input), path);
    }

    private T? queryMachinery<T>(Input input, string path)
    {
        ExecutePolicyWithInputRequest req = new ExecutePolicyWithInputRequest()
        {
            Path = "app/rbac",
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
            throw new OPAException(msg, e);
        }

        T? result = default;
        var resultString = res.SuccessfulPolicyEvaluation?.Result?.ToString();
        if (resultString != null)
        {
            JsonSerializer serializer = new JsonSerializer();
            JsonTextReader reader = new JsonTextReader(new StringReader(resultString));
            result = serializer.Deserialize<T>(reader);
        }

        return result;
    }
}