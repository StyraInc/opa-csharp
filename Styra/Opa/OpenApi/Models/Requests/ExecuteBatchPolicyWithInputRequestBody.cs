//------------------------------------------------------------------------------
// <auto-generated>
// This code was generated by Speakeasy (https://speakeasy.com). DO NOT EDIT.
//
// Changes to this file may cause incorrect behavior and will be lost when
// the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
#nullable enable
namespace Styra.Opa.OpenApi.Models.Requests
{
    using Newtonsoft.Json;
    using Styra.Opa.OpenApi.Models.Components;
    using Styra.Opa.OpenApi.Utils;
    using System.Collections.Generic;
    
    /// <summary>
    /// The batch of inputs
    /// </summary>
    public class ExecuteBatchPolicyWithInputRequestBody
    {

        [JsonProperty("inputs")]
        public Dictionary<string, Input> Inputs { get; set; } = default!;
    }
}