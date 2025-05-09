//------------------------------------------------------------------------------
// <auto-generated>
// This code was generated by Speakeasy (https://speakeasy.com). DO NOT EDIT.
//
// Changes to this file may cause incorrect behavior and will be lost when
// the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
#nullable enable
namespace Styra.Opa.OpenApi.Models.Components
{
    using Newtonsoft.Json;
    using Styra.Opa.OpenApi.Models.Components;
    using Styra.Opa.OpenApi.Utils;
    using System.Collections.Generic;
    
    public class SuccessfulPolicyResponseWithStatusCode
    {

        [JsonProperty("http_status_code")]
        public string HttpStatusCode { get; set; } = default!;

        /// <summary>
        /// The base or virtual document referred to by the URL path. If the path is undefined, this key will be omitted.
        /// </summary>
        [JsonProperty("result")]
        public Result? Result { get; set; }

        /// <summary>
        /// If query metrics are enabled, this field contains query performance metrics collected during the parse, compile, and evaluation steps.
        /// </summary>
        [JsonProperty("metrics")]
        public Dictionary<string, object>? Metrics { get; set; }

        /// <summary>
        /// If decision logging is enabled, this field contains a string that uniquely identifies the decision. The identifier will be included in the decision log event for this decision. Callers can use the identifier for correlation purposes.
        /// </summary>
        [JsonProperty("decision_id")]
        public string? DecisionId { get; set; }

        /// <summary>
        /// Provenance information can be requested on individual API calls and are returned inline with the API response. To obtain provenance information on an API call, specify the `provenance=true` query parameter when executing the API call.
        /// </summary>
        [JsonProperty("provenance")]
        public Provenance? Provenance { get; set; }
    }
}