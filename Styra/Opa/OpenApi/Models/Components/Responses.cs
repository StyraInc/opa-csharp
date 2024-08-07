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
    using Newtonsoft.Json.Linq;
    using Newtonsoft.Json;
    using Styra.Opa.OpenApi.Models.Components;
    using Styra.Opa.OpenApi.Utils;
    using System.Collections.Generic;
    using System.Numerics;
    using System.Reflection;
    using System;
    

    public class ResponsesType
    {
        private ResponsesType(string value) { Value = value; }

        public string Value { get; private set; }
        
        public static ResponsesType TwoHundred { get { return new ResponsesType("200"); } }
        public static ResponsesType FiveHundred { get { return new ResponsesType("500"); } }
        public static ResponsesType Null { get { return new ResponsesType("null"); } }

        public override string ToString() { return Value; }
        public static implicit operator String(ResponsesType v) { return v.Value; }
        public static ResponsesType FromString(string v) {
            switch(v) {
                case "200": return TwoHundred;
                case "500": return FiveHundred;
                case "null": return Null;
                default: throw new ArgumentException("Invalid value for ResponsesType");
            }
        }
        public override bool Equals(object? obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            return Value.Equals(((ResponsesType)obj).Value);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
    }


    [JsonConverter(typeof(Responses.ResponsesConverter))]
    public class Responses {
        public Responses(ResponsesType type) {
            Type = type;
        }
        public ResponsesSuccessfulPolicyResponse? ResponsesSuccessfulPolicyResponse { get; set; }
        public Models.Components.ServerError? ServerError { get; set; }

        public ResponsesType Type { get; set; }


        public static Responses CreateTwoHundred(ResponsesSuccessfulPolicyResponse twoHundred) {
            ResponsesType typ = ResponsesType.TwoHundred;
        
            string typStr = ResponsesType.TwoHundred.ToString();
            
            twoHundred.HttpStatusCode = typStr;
            Responses res = new Responses(typ);
            res.ResponsesSuccessfulPolicyResponse = twoHundred;
            return res;
        }
        public static Responses CreateFiveHundred(Models.Components.ServerError fiveHundred) {
            ResponsesType typ = ResponsesType.FiveHundred;
        
            string typStr = ResponsesType.FiveHundred.ToString();
            
            fiveHundred.HttpStatusCode = typStr;
            Responses res = new Responses(typ);
            res.ServerError = fiveHundred;
            return res;
        }
        public static Responses CreateNull() {
            ResponsesType typ = ResponsesType.Null;
            return new Responses(typ);
        }

        public class ResponsesConverter : JsonConverter
        {

            public override bool CanConvert(System.Type objectType) => objectType == typeof(Responses);

            public override bool CanRead => true;

            public override object? ReadJson(JsonReader reader, System.Type objectType, object? existingValue, JsonSerializer serializer)
            {
                JObject jo = JObject.Load(reader);
                string discriminator = jo.GetValue("http_status_code")?.ToString() ?? throw new ArgumentNullException("Could not find discriminator field.");
                if (discriminator == ResponsesType.TwoHundred.ToString())
                {
                    ResponsesSuccessfulPolicyResponse? responsesSuccessfulPolicyResponse = ResponseBodyDeserializer.Deserialize<ResponsesSuccessfulPolicyResponse>(jo.ToString());
                    return CreateTwoHundred(responsesSuccessfulPolicyResponse!);
                }
                if (discriminator == ResponsesType.FiveHundred.ToString())
                {
                    Models.Components.ServerError? serverError = ResponseBodyDeserializer.Deserialize<Models.Components.ServerError>(jo.ToString());
                    return CreateFiveHundred(serverError!);
                }

                throw new InvalidOperationException("Could not deserialize into any supported types.");
            }

            public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
            {
                if (value == null) {
                    writer.WriteRawValue("null");
                    return;
                }
                Responses res = (Responses)value;
                if (ResponsesType.FromString(res.Type).Equals(ResponsesType.Null))
                {
                    writer.WriteRawValue("null");
                    return;
                }
                if (res.ResponsesSuccessfulPolicyResponse != null)
                {
                    writer.WriteRawValue(Utilities.SerializeJSON(res.ResponsesSuccessfulPolicyResponse));
                    return;
                }
                if (res.ServerError != null)
                {
                    writer.WriteRawValue(Utilities.SerializeJSON(res.ServerError));
                    return;
                }

            }

        }

    }
}