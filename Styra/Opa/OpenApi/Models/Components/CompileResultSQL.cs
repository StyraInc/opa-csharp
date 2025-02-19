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
    
    /// <summary>
    /// The partially evaluated result of the query, in SQL format. Result will be empty if the query is never true.
    /// </summary>
    public class CompileResultSQL
    {

        /// <summary>
        /// The partially evaluated result of the query as SQL.
        /// </summary>
        [JsonProperty("result")]
        public CompileResultSQLResult? Result { get; set; }
    }
}