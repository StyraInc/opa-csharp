
//------------------------------------------------------------------------------
// <auto-generated>
// This code was generated by Speakeasy (https://speakeasyapi.dev). DO NOT EDIT.
//
// Changes to this file may cause incorrect behavior and will be lost when
// the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
#nullable enable
namespace Api.Models.Errors
{
    using Api.Models.Errors;
    using Api.Utils;
    using Newtonsoft.Json;
    
    public class Errors
    {

        [JsonProperty("code")]
        public string Code { get; set; } = default!;

        [JsonProperty("message")]
        public string Message { get; set; } = default!;

        [JsonProperty("location")]
        public Location? Location { get; set; }
    }
}