//------------------------------------------------------------------------------
// <auto-generated>
// This code was generated by Speakeasy (https://speakeasy.com). DO NOT EDIT.
//
// Changes to this file may cause incorrect behavior and will be lost when
// the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
#nullable enable
namespace Styra.Opa.OpenApi.Models.Errors
{
    using Newtonsoft.Json;
    using Styra.Opa.OpenApi.Models.Errors;
    using Styra.Opa.OpenApi.Utils;
    using System.Collections.Generic;
    using System;
    
    public class BatchServerError : Exception
    {

        [JsonProperty("batch_decision_id")]
        public string? BatchDecisionId { get; set; }

        [JsonProperty("responses")]
        public Dictionary<string, ServerError>? Responses { get; set; }
    }
}