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
    using System.Collections.Generic;
    using System.Net.Http;
    using System;
    
    public class ExecuteDefaultPolicyWithInputResponse
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
        /// Evaluating the default policy has the same response behavior as a successful policy evaluation, but with only the result as the response.<br/>
        /// 
        /// </remarks>
        /// </summary>
        public Result? Result { get; set; }

        public Dictionary<string, List<string>> Headers { get; set; } = default!;
    }
}