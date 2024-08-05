//------------------------------------------------------------------------------
// <auto-generated>
// This code was generated by Speakeasy (https://speakeasy.com). DO NOT EDIT.
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
    using System.Reflection;
    using System.Text;
    using System.Web;


    internal class SecurityMetadata
    {
        private Dictionary<string, string> headerParams { get; } = new Dictionary<string, string>();
        private Dictionary<string, string> queryParams { get; } = new Dictionary<string, string>();

        public SecurityMetadata(Func<object> securitySource)
        {
            ParseSecuritySource(securitySource);
        }

        public HttpRequestMessage Apply(HttpRequestMessage request)
        {
            foreach (var kvp in headerParams)
            {
                request.Headers.Add(kvp.Key, kvp.Value);
            }

            if(request.RequestUri != null)
            {
                var uriBuilder = new UriBuilder(request.RequestUri);
                var query = HttpUtility.ParseQueryString(uriBuilder.Query);
                foreach (var kvp in queryParams)
                {
                    query.Add(kvp.Key, kvp.Value);
                }
                uriBuilder.Query = query.ToString();
                request.RequestUri =  uriBuilder.Uri;
            }

            return request;
        }

        private void ParseSecuritySource(Func<object> securitySource)
        {
            if (securitySource == null)
            {
                return;
            }

            var security = securitySource();
            if (security == null)
            {
                return;
            }

            foreach (var prop in security.GetType().GetProperties())
            {
                var value = prop.GetValue(security, null);
                if (value == null)
                {
                    continue;
                }

                var secMetadata = prop.GetCustomAttribute<SpeakeasyMetadata>()?.GetSecurityMetadata();
                if (secMetadata == null)
                {
                    continue;
                }

                if (secMetadata.Option)
                {
                    ParseOption(value);
                }
                else if (secMetadata.Scheme)
                {
                    if (secMetadata.SubType == "basic" && !Utilities.IsClass(value))
                    {
                        ParseScheme(secMetadata, security);
                        return;
                    }
                    else
                    {
                        ParseScheme(secMetadata, value);
                    }
                }
            }

            return;
        }

        private void ParseOption(object option)
        {
            foreach (var prop in option.GetType().GetProperties())
            {
                var value = prop.GetValue(option, null);
                if (value == null)
                {
                    continue;
                }

                var secMetadata = prop.GetCustomAttribute<SpeakeasyMetadata>()?.GetSecurityMetadata();
                if (secMetadata == null || !secMetadata.Scheme)
                {
                    continue;
                }

                ParseScheme(secMetadata, value);
            }
        }

        private void ParseScheme(SpeakeasyMetadata.SecurityMetadata schemeMetadata, object scheme)
        {
            if (Utilities.IsClass(scheme))
            {
                if (schemeMetadata.Type == "http" && schemeMetadata.SubType == "basic")
                {
                    ParseBasicAuthScheme(scheme);
                    return;
                }

                foreach (var prop in scheme.GetType().GetProperties())
                {
                    var value = prop.GetValue(scheme, null);
                    if (value == null)
                    {
                        continue;
                    }

                    var secMetadata = prop.GetCustomAttribute<SpeakeasyMetadata>()?.GetSecurityMetadata();
                    if (secMetadata == null || secMetadata.Name == "")
                    {
                        continue;
                    }

                    ParseSchemeValue(schemeMetadata, secMetadata, value);
                }
            }
            else
            {
                ParseSchemeValue(schemeMetadata, schemeMetadata, scheme);
            }
        }

        private void ParseSchemeValue(
            SpeakeasyMetadata.SecurityMetadata schemeMetadata,
            SpeakeasyMetadata.SecurityMetadata valueMetadata,
            object value
        )
        {
            var key = valueMetadata.Name;
            if (key == "")
            {
                return;
            }

            var valStr = Utilities.ValueToString(value);

            switch (schemeMetadata.Type)
            {
                case "apiKey":
                    switch (schemeMetadata.SubType)
                    {
                        case "header":
                            headerParams.Add(key, valStr);
                            break;
                        case "query":
                            queryParams.Add(key, valStr);
                            break;
                        case "cookie":
                            headerParams.Add("cookie", $"{key}={valStr}");
                            break;
                        default:
                            throw new Exception($"Unknown apiKey subType: {schemeMetadata.SubType}");
                    }
                    break;
                case "openIdConnect":
                    headerParams.Add(key, Utilities.PrefixBearer(valStr));
                    break;
                case "oauth2":
                    headerParams.Add(key, Utilities.PrefixBearer(valStr));
                    break;
                case "http":
                    switch (schemeMetadata.SubType)
                    {
                        case "bearer":
                            headerParams.Add(key, Utilities.PrefixBearer(valStr));
                            break;
                        default:
                            throw new Exception($"Unknown http subType: {schemeMetadata.SubType}");
                    }
                    break;
                default:
                    throw new Exception($"Unknown security type: {schemeMetadata.Type}");
            }
        }

        private void ParseBasicAuthScheme(object scheme)
        {

            string username = "";
            string password = "";

            foreach (var prop in scheme.GetType().GetProperties())
            {
                var value = prop.GetValue(scheme, null);
                if (value == null)
                {
                    continue;
                }

                var secMetadata = prop.GetCustomAttribute<SpeakeasyMetadata>()?.GetSecurityMetadata();
                if (secMetadata == null || secMetadata.Name == "")
                {
                    continue;
                }

                if (secMetadata.Name == "username")
                {
                    username = Utilities.ValueToString(value);
                }
                else if (secMetadata.Name == "password")
                {
                    password = Utilities.ValueToString(value);
                }
            }

            var auth = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{password}"));
            headerParams.Add("Authorization", $"Basic {auth}");
        }
    }
}
