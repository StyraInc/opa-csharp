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
    using Styra.Opa.OpenApi.Utils;
    
    public class ResponsesLocation
    {

        [JsonProperty("file")]
        public string File { get; set; } = default!;

        [JsonProperty("row")]
        public long Row { get; set; } = default!;

        [JsonProperty("col")]
        public long Col { get; set; } = default!;
    }
}