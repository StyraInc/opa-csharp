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
    using Styra.Opa.OpenApi.Utils;
    using System.Collections.Generic;
    using System.Numerics;
    using System.Reflection;
    using System;
    

    public class ResultType
    {
        private ResultType(string value) { Value = value; }

        public string Value { get; private set; }
        public static ResultType Boolean { get { return new ResultType("boolean"); } }
        
        public static ResultType Str { get { return new ResultType("str"); } }
        
        public static ResultType Number { get { return new ResultType("number"); } }
        
        public static ResultType ArrayOfAny { get { return new ResultType("arrayOfAny"); } }
        
        public static ResultType MapOfAny { get { return new ResultType("mapOfAny"); } }
        
        public static ResultType Null { get { return new ResultType("null"); } }

        public override string ToString() { return Value; }
        public static implicit operator String(ResultType v) { return v.Value; }
        public static ResultType FromString(string v) {
            switch(v) {
                case "boolean": return Boolean;
                case "str": return Str;
                case "number": return Number;
                case "arrayOfAny": return ArrayOfAny;
                case "mapOfAny": return MapOfAny;
                case "null": return Null;
                default: throw new ArgumentException("Invalid value for ResultType");
            }
        }
        public override bool Equals(object? obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            return Value.Equals(((ResultType)obj).Value);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
    }


    /// <summary>
    /// The base or virtual document referred to by the URL path. If the path is undefined, this key will be omitted.
    /// </summary>
    [JsonConverter(typeof(Result.ResultConverter))]
    public class Result {
        public Result(ResultType type) {
            Type = type;
        }
        public bool? Boolean { get; set; }
        public string? Str { get; set; }
        public double? Number { get; set; }
        public List<object>? ArrayOfAny { get; set; }
        public Dictionary<string, object>? MapOfAny { get; set; }

        public ResultType Type { get; set; }


        public static Result CreateBoolean(bool boolean) {
            ResultType typ = ResultType.Boolean;

            Result res = new Result(typ);
            res.Boolean = boolean;
            return res;
        }

        public static Result CreateStr(string str) {
            ResultType typ = ResultType.Str;

            Result res = new Result(typ);
            res.Str = str;
            return res;
        }

        public static Result CreateNumber(double number) {
            ResultType typ = ResultType.Number;

            Result res = new Result(typ);
            res.Number = number;
            return res;
        }

        public static Result CreateArrayOfAny(List<object> arrayOfAny) {
            ResultType typ = ResultType.ArrayOfAny;

            Result res = new Result(typ);
            res.ArrayOfAny = arrayOfAny;
            return res;
        }

        public static Result CreateMapOfAny(Dictionary<string, object> mapOfAny) {
            ResultType typ = ResultType.MapOfAny;

            Result res = new Result(typ);
            res.MapOfAny = mapOfAny;
            return res;
        }

        public static Result CreateNull() {
            ResultType typ = ResultType.Null;
            return new Result(typ);
        }

        public class ResultConverter : JsonConverter
        {

            public override bool CanConvert(System.Type objectType) => objectType == typeof(Result);

            public override bool CanRead => true;

            public override object? ReadJson(JsonReader reader, System.Type objectType, object? existingValue, JsonSerializer serializer)
            {
                var json = JRaw.Create(reader).ToString();
                if (json == "null")
                {
                    return null;
                }

                var fallbackCandidates = new List<(System.Type, object, string)>();

                try
                {
                    var converted = Convert.ToBoolean(json);
                    return new Result(ResultType.Boolean)
                    {
                        Boolean = converted
                    };
                }
                catch (System.FormatException)
                {
                    // try next option
                }

                if (json[0] == '"' && json[^1] == '"'){
                    return new Result(ResultType.Str)
                    {
                        Str = json[1..^1]
                    };
                }

                try
                {
                    var converted = Convert.ToDouble(json);
                    return new Result(ResultType.Number)
                    {
                        Number = converted
                    };
                }
                catch (System.FormatException)
                {
                    // try next option
                }

                try
                {
                    return new Result(ResultType.ArrayOfAny)
                    {
                        ArrayOfAny = ResponseBodyDeserializer.DeserializeUndiscriminatedUnionMember<List<object>>(json)
                    };
                }
                catch (ResponseBodyDeserializer.MissingMemberException)
                {
                    fallbackCandidates.Add((typeof(List<object>), new Result(ResultType.ArrayOfAny), "ArrayOfAny"));
                }
                catch (ResponseBodyDeserializer.DeserializationException)
                {
                    // try next option
                }
                catch (Exception)
                {
                    throw;
                }

                try
                {
                    return new Result(ResultType.MapOfAny)
                    {
                        MapOfAny = ResponseBodyDeserializer.DeserializeUndiscriminatedUnionMember<Dictionary<string, object>>(json)
                    };
                }
                catch (ResponseBodyDeserializer.MissingMemberException)
                {
                    fallbackCandidates.Add((typeof(Dictionary<string, object>), new Result(ResultType.MapOfAny), "MapOfAny"));
                }
                catch (ResponseBodyDeserializer.DeserializationException)
                {
                    // try next option
                }
                catch (Exception)
                {
                    throw;
                }

                if (fallbackCandidates.Count > 0)
                {
                    fallbackCandidates.Sort((a, b) => ResponseBodyDeserializer.CompareFallbackCandidates(a.Item1, b.Item1, json));
                    foreach(var (deserializationType, returnObject, propertyName) in fallbackCandidates)
                    {
                        try
                        {
                            return ResponseBodyDeserializer.DeserializeUndiscriminatedUnionFallback(deserializationType, returnObject, propertyName, json);
                        }
                        catch (ResponseBodyDeserializer.DeserializationException)
                        {
                            // try next fallback option
                        }
                        catch (Exception)
                        {
                            throw;
                        }
                    }
                }

                throw new InvalidOperationException("Could not deserialize into any supported types.");
            }

            public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
            {
                if (value == null) {
                    writer.WriteRawValue("null");
                    return;
                }
                Result res = (Result)value;
                if (ResultType.FromString(res.Type).Equals(ResultType.Null))
                {
                    writer.WriteRawValue("null");
                    return;
                }
                if (res.Boolean != null)
                {
                    writer.WriteRawValue(Utilities.SerializeJSON(res.Boolean));
                    return;
                }
                if (res.Str != null)
                {
                    writer.WriteRawValue(Utilities.SerializeJSON(res.Str));
                    return;
                }
                if (res.Number != null)
                {
                    writer.WriteRawValue(Utilities.SerializeJSON(res.Number));
                    return;
                }
                if (res.ArrayOfAny != null)
                {
                    writer.WriteRawValue(Utilities.SerializeJSON(res.ArrayOfAny));
                    return;
                }
                if (res.MapOfAny != null)
                {
                    writer.WriteRawValue(Utilities.SerializeJSON(res.MapOfAny));
                    return;
                }

            }

        }

    }
}