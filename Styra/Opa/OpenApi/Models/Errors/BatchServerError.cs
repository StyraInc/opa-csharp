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
    using Styra.Opa.OpenApi.Models.Components;
    using Styra.Opa.OpenApi.Utils;
    using System;
    using System.Collections.Generic;
    
    /// <summary>
    /// Server Error. All requests returned a 500 error.<br/>
    /// 
    /// <remarks>
    /// 
    /// </remarks>
    /// </summary>
    public class BatchServerError : Exception
    {

        [JsonProperty("batch_decision_id")]
        public string? BatchDecisionId { get; set; }

        [JsonProperty("responses")]
        public Dictionary<string, Models.Components.ServerError>? Responses { get; set; }
    }
}