//------------------------------------------------------------------------------
// <auto-generated>
// This code was generated by Speakeasy (https://speakeasyapi.dev). DO NOT EDIT.
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
    
    public class Errors
    {

        [JsonProperty("code")]
        public string Code { get; set; } = default!;

        [JsonProperty("message")]
        public string Message { get; set; } = default!;

        [JsonProperty("location")]
        public Models.Components.Location? Location { get; set; }
    }
}