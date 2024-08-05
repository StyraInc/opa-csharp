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
    
    public class ServerError
    {

        [JsonProperty("code")]
        public string Code { get; set; } = default!;

        [JsonProperty("message")]
        public string Message { get; set; } = default!;

        [JsonProperty("errors")]
        public List<Models.Components.Errors>? Errors { get; set; }

        [JsonProperty("decision_id")]
        public string? DecisionId { get; set; }

        [JsonProperty("http_status_code")]
        public string? HttpStatusCode { get; set; }
    }
}