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
    
    public class Ucast
    {

        /// <summary>
        /// UCAST JSON object describing the conditions under which the query is true.
        /// </summary>
        [JsonProperty("query")]
        public CompileResultMultitargetQuery? Query { get; set; }

        /// <summary>
        /// Column masking rules, where the first two nested keys represent the entity name and the property name, and the value describes which masking function to use.
        /// </summary>
        [JsonProperty("masks")]
        public Dictionary<string, Dictionary<string, MaskingRule>>? Masks { get; set; }
    }
}