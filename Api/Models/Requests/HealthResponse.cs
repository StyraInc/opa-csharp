
//------------------------------------------------------------------------------
// <auto-generated>
// This code was generated by Speakeasy (https://speakeasyapi.dev). DO NOT EDIT.
//
// Changes to this file may cause incorrect behavior and will be lost when
// the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
#nullable enable
namespace Api.Models.Requests
{
    using Api.Models.Components;
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

        /// <summary>
        /// OPA service is not healthy. If the bundles option is specified this can mean any of the configured bundles have not yet been activated. If the plugins option is specified then at least one plugin is in a non-OK state.
        /// </summary>
        public UnhealthyServer? UnhealthyServer { get; set; }
    }
}