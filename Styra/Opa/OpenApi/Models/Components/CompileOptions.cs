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
    
    /// <summary>
    /// Additional options to use during partial evaluation. Only the disableInlining option is currently supported in OPA. Enterprise OPA may support additional options.
    /// </summary>
    public class CompileOptions
    {

        /// <summary>
        /// A list of paths to exclude from partial evaluation inlining.
        /// </summary>
        [JsonProperty("disableInlining")]
        public List<string>? DisableInlining { get; set; }

        /// <summary>
        /// The output targets for partial evaluation. Different targets will have different constraints.
        /// </summary>
        [JsonProperty("targetDialects")]
        public List<TargetDialects>? TargetDialects { get; set; }

        [JsonProperty("targetSQLTableMappings")]
        public TargetSQLTableMappings? TargetSQLTableMappings { get; set; }

        /// <summary>
        /// The Rego rule to evaluate for generating column masks.
        /// </summary>
        [JsonProperty("maskRule")]
        public string? MaskRule { get; set; }

        [JsonProperty("additionalProperties")]
        public Dictionary<string, object>? AdditionalProperties { get; set; }
    }
}