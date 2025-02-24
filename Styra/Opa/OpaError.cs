using Newtonsoft.Json;

namespace Styra.Opa;

public class OpaError
{
    /// <summary>
    /// The short-form category of error, such as "internal_error", "invalid_policy_or_data", etc.
    /// </summary>
    [JsonProperty("code")]
    public string Code { get; set; } = default!;

    /// <summary>
    /// If decision logging is enabled, this field contains a string that uniquely identifies the decision. The identifier will be included in the decision log event for this decision. Callers can use the identifier for correlation purposes.
    /// </summary>
    [JsonProperty("decision_id")]
    public string? DecisionId { get; set; }

    /// <summary>
    /// The long-form error message from the OPA instance, describing what went wrong.
    /// </summary>
    [JsonProperty("message")]
    public string Message { get; set; } = default!;

    /// <summary>
    /// The HTTP status code for the request. Limited to "200" or "500".
    /// </summary>
    [JsonProperty("http_status_code")]
    public string? HttpStatusCode { get; set; }

    public OpaError()
    {

    }

    public OpaError(Styra.Opa.OpenApi.Models.Components.ServerErrorWithStatusCode err)
    {
        Code = err.Code;
        DecisionId = err.DecisionId;
        Message = err.Message;
        HttpStatusCode = err.HttpStatusCode;
    }

    public OpaError(Styra.Opa.OpenApi.Models.Components.ServerError err)
    {
        Code = err.Code;
        DecisionId = err.DecisionId;
        Message = err.Message;
    }

    public OpaError(Styra.Opa.OpenApi.Models.Errors.ServerError err)
    {
        Code = err.Code;
        DecisionId = err.DecisionId;
        Message = err.Message;
    }

    public static explicit operator OpaError(OpenApi.Models.Components.ServerErrorWithStatusCode e) => new(e);
    public static explicit operator OpaError(OpenApi.Models.Errors.ServerError e) => new(e);

    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }
}
