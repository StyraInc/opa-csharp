
//------------------------------------------------------------------------------
// <auto-generated>
// This code was generated by Speakeasy (https://speakeasyapi.dev). DO NOT EDIT.
//
// Changes to this file may cause incorrect behavior and will be lost when
// the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
#nullable enable
namespace Styra.Opa.OpenApi.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;

    public interface ISpeakeasyHttpClient
    {
        Task<HttpResponseMessage> SendAsync(HttpRequestMessage request);
    }

    public class SpeakeasyHttpClient : ISpeakeasyHttpClient
    {
        private ISpeakeasyHttpClient? client;

        internal SpeakeasyHttpClient(ISpeakeasyHttpClient? client = null)
        {
            this.client = client;
        }

        public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
        {
            if (client != null)
            {
                return await client.SendAsync(request);
            }

            return await new HttpClient().SendAsync(request);
        }
    }
}