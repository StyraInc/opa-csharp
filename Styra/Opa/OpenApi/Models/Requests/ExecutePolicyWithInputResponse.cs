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
    using Styra.Opa.OpenApi.Models.Components;
    using Styra.Opa.OpenApi.Utils;
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    
    public class ExecutePolicyWithInputResponse
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
        /// Success.<br/>
        /// 
        /// <remarks>
        /// The server also returns 200 if the path refers to an undefined document. In this case, the response will not contain a result property.<br/>
        /// 
        /// </remarks>
        /// </summary>
        public SuccessfulPolicyResponse? SuccessfulPolicyResponse { get; set; }

        public Dictionary<string, List<string>> Headers { get; set; } = default!;
    }
}