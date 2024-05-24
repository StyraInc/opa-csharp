//------------------------------------------------------------------------------
// <auto-generated>
// This code was generated by Speakeasy (https://speakeasyapi.dev). DO NOT EDIT.
//
// Changes to this file may cause incorrect behavior and will be lost when
// the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
#nullable enable
namespace Styra.Opa.OpenApi.Models.Requests
{
    using Styra.Opa.OpenApi.Models.Components;
    using Styra.Opa.OpenApi.Utils;
    using System.Net.Http;
    using System;
    
    public class HealthResponse
    {

        /// <summary>
        /// HTTP response content type for this operation
        /// </summary>
        public string? ContentType { get; set; } = default!;

        /// <summary>
        /// HTTP response status code for this operation
        /// </summary>
        public int StatusCode { get; set; } = default!;

        /// <summary>
        /// Raw HTTP response; suitable for custom response parsing
        /// </summary>
        public HttpResponseMessage RawResponse { get; set; } = default!;

        /// <summary>
        /// OPA service is healthy. If the bundles option is specified then all configured bundles have been activated. If the plugins option is specified then all plugins are in an OK state.
        /// </summary>
        public HealthyServer? HealthyServer { get; set; }
    }
}