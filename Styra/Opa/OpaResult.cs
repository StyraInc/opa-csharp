using System.Collections.Generic;
using Newtonsoft.Json;
using Styra.Opa.OpenApi.Models.Components;

namespace Styra.Opa;

public class OpaResult
{
    /// <summary>
    /// If decision logging is enabled, this field contains a string that uniquely identifies the decision. The identifier will be included in the decision log event for this decision. Callers can use the identifier for correlation purposes.
    /// </summary>
    [JsonProperty("decision_id")]
    public string? DecisionId { get; set; }

    /// <summary>
    /// If query metrics are enabled, this field contains query performance metrics collected during the parse, compile, and evaluation steps.
    /// </summary>
    [JsonProperty("metrics")]
    public Dictionary<string, object>? Metrics { get; set; }

    /// <summary>
    /// Provenance information can be requested on individual API calls and are returned inline with the API response. To obtain provenance information on an API call, specify the `provenance=true` query parameter when executing the API call.
    /// </summary>
    [JsonProperty("provenance")]
    public Provenance? Provenance { get; set; }

    /// <summary>
    /// The base or virtual document referred to by the URL path. If the path is undefined, this key will be omitted.
    /// </summary>
    [JsonProperty("result")]
    public Result? Result { get; set; }

    /// <summary>
    /// The HTTP status code for the request. Limited to "200" or "500".
    /// </summary>
    [JsonProperty("http_status_code")]
    public string? HttpStatusCode { get; set; }

    public OpaResult() { }

    public OpaResult(ResponsesSuccessfulPolicyResponse resp)
    {
        DecisionId = resp.DecisionId;
        Metrics = resp.Metrics;
        Provenance = resp.Provenance;
        Result = resp.Result;
        HttpStatusCode = resp.HttpStatusCode;
    }

    public OpaResult(SuccessfulPolicyResponse resp)
    {
        DecisionId = resp.DecisionId;
        Metrics = resp.Metrics;
        Provenance = resp.Provenance;
        Result = resp.Result;
    }

    public static explicit operator OpaResult(ResponsesSuccessfulPolicyResponse e) => new OpaResult(e);
    public static explicit operator OpaResult(SuccessfulPolicyResponse e) => new OpaResult(e);

    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }
}