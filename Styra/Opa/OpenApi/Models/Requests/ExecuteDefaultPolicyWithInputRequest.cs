
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
    
    public class ExecuteDefaultPolicyWithInputRequest
    {

        /// <summary>
        /// The input document
        /// </summary>
        [SpeakeasyMetadata("request:mediaType=application/json")]
        public Input Input { get; set; } = default!;

        /// <summary>
        /// If parameter is `true`, response will formatted for humans.
        /// </summary>
        [SpeakeasyMetadata("queryParam:style=form,explode=true,name=pretty")]
        public bool? Pretty { get; set; }

        /// <summary>
        /// Indicates the server should respond with a gzip encoded body. The server will send the compressed response only if its length is above `server.encoding.gzip.min_length` value. See the configuration section
        /// </summary>
        [SpeakeasyMetadata("header:style=simple,explode=false,name=Accept-Encoding")]
        public GzipAcceptEncoding? AcceptEncoding { get; set; }
    }
}