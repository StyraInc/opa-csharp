//------------------------------------------------------------------------------
// <auto-generated>
// This code was generated by Speakeasy (https://speakeasyapi.dev). DO NOT EDIT.
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
    
    /// <summary>
    /// Bad Request
    /// </summary>
    public class ClientError : Exception
    {

        [JsonProperty("code")]
        public string Code { get; set; } = default!;

        [JsonProperty("message")]
        private string? _message { get; set; }
        public override string Message { get {return _message ?? "";} }

        [JsonProperty("errors")]
        public List<Errors>? Errors { get; set; }
    }
}