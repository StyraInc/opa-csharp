
//------------------------------------------------------------------------------
// <auto-generated>
// This code was generated by Speakeasy (https://speakeasyapi.dev). DO NOT EDIT.
//
// Changes to this file may cause incorrect behavior and will be lost when
// the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
#nullable enable
namespace Styra.OpenApi.Models.Requests
{
    using Styra.OpenApi.Models.Components;
    using Styra.OpenApi.Utils;
    
    public class ExecutePolicyRequest
    {

        /// <summary>
        /// The path separator is used to access values inside object and array documents. If the path indexes into an array, the server will attempt to convert the array index to an integer. If the path element cannot be converted to an integer, the server will respond with 404.
        /// </summary>
        [SpeakeasyMetadata("pathParam:style=simple,explode=false,name=path")]
        public string Path { get; set; } = "";

        /// <summary>
        /// Indicates the server should respond with a gzip encoded body. The server will send the compressed response only if its length is above `server.encoding.gzip.min_length` value. See the configuration section
        /// </summary>
        [SpeakeasyMetadata("header:style=simple,explode=false,name=Accept-Encoding")]
        public GzipAcceptEncoding? AcceptEncoding { get; set; }

        /// <summary>
        /// If parameter is `true`, response will formatted for humans.
        /// </summary>
        [SpeakeasyMetadata("queryParam:style=form,explode=true,name=pretty")]
        public bool? Pretty { get; set; }

        /// <summary>
        /// If parameter is true, response will include build/version info in addition to the result.
        /// </summary>
        [SpeakeasyMetadata("queryParam:style=form,explode=true,name=provenance")]
        public bool? Provenance { get; set; }

        /// <summary>
        /// Return query explanation in addition to result.
        /// </summary>
        [SpeakeasyMetadata("queryParam:style=form,explode=true,name=explain")]
        public Explain? Explain { get; set; }

        /// <summary>
        /// Return query performance metrics in addition to result.
        /// </summary>
        [SpeakeasyMetadata("queryParam:style=form,explode=true,name=metrics")]
        public bool? Metrics { get; set; }

        /// <summary>
        /// Instrument query evaluation and return a superset of performance metrics in addition to result.
        /// </summary>
        [SpeakeasyMetadata("queryParam:style=form,explode=true,name=instrument")]
        public bool? Instrument { get; set; }

        /// <summary>
        /// Treat built-in function call errors as fatal and return an error immediately.
        /// </summary>
        [SpeakeasyMetadata("queryParam:style=form,explode=true,name=strict-builtin-errors")]
        public bool? StrictBuiltinErrors { get; set; }
    }
}