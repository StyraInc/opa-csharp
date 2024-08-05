//------------------------------------------------------------------------------
// <auto-generated>
// This code was generated by Speakeasy (https://speakeasy.com). DO NOT EDIT.
//
// Changes to this file may cause incorrect behavior and will be lost when
// the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
#nullable enable
namespace Styra.Opa.OpenApi.Models.Components
{
    using Newtonsoft.Json;
    using Styra.Opa.OpenApi.Utils;
    using System;
    
    public enum Explain
    {
        [JsonProperty("notes")]
        Notes,
        [JsonProperty("fails")]
        Fails,
        [JsonProperty("full")]
        Full,
        [JsonProperty("debug")]
        Debug,
    }

    public static class ExplainExtension
    {
        public static string Value(this Explain value)
        {
            return ((JsonPropertyAttribute)value.GetType().GetMember(value.ToString())[0].GetCustomAttributes(typeof(JsonPropertyAttribute), false)[0]).PropertyName ?? value.ToString();
        }

        public static Explain ToEnum(this string value)
        {
            foreach(var field in typeof(Explain).GetFields())
            {
                var attributes = field.GetCustomAttributes(typeof(JsonPropertyAttribute), false);
                if (attributes.Length == 0)
                {
                    continue;
                }

                var attribute = attributes[0] as JsonPropertyAttribute;
                if (attribute != null && attribute.PropertyName == value)
                {
                    var enumVal = field.GetValue(null);

                    if (enumVal is Explain)
                    {
                        return (Explain)enumVal;
                    }
                }
            }

            throw new Exception($"Unknown value {value} for enum Explain");
        }
    }

}