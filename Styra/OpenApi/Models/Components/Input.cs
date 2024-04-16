
//------------------------------------------------------------------------------
// <auto-generated>
// This code was generated by Speakeasy (https://speakeasyapi.dev). DO NOT EDIT.
//
// Changes to this file may cause incorrect behavior and will be lost when
// the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
#nullable enable
namespace Styra.OpenApi.Models.Components
{
    using Newtonsoft.Json.Linq;
    using Newtonsoft.Json;
    using Styra.OpenApi.Utils;
    using System.Collections.Generic;
    using System.Numerics;
    using System;
    

    public class InputType
    {
        private InputType(string value) { Value = value; }

        public string Value { get; private set; }
        public static InputType MapOfany { get { return new InputType("mapOfany"); } }
        
        public static InputType Boolean { get { return new InputType("boolean"); } }
        
        public static InputType Str { get { return new InputType("str"); } }
        
        public static InputType ArrayOfany { get { return new InputType("arrayOfany"); } }
        
        public static InputType Number { get { return new InputType("number"); } }
        
        public static InputType Null { get { return new InputType("null"); } }

        public override string ToString() { return Value; }
        public static implicit operator String(InputType v) { return v.Value; }
        public static InputType FromString(string v) {
            switch(v) {
                case "mapOfany": return MapOfany;
                case "boolean": return Boolean;
                case "str": return Str;
                case "arrayOfany": return ArrayOfany;
                case "number": return Number;
                case "null": return Null;
                default: throw new ArgumentException("Invalid value for InputType");
            }
        }
        public override bool Equals(object? obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            return Value.Equals(((InputType)obj).Value);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
    }
    
/// <summary>
/// Arbitrary JSON used within your policies by accessing `input`
/// </summary>
    [JsonConverter(typeof(Input.InputConverter))]
    public class Input {
        public Input(InputType type) {
            Type = type;
        }
        public Dictionary<string, object>? MapOfany { get; set; } 
        public bool? Boolean { get; set; } 
        public string? Str { get; set; } 
        public List<object>? ArrayOfany { get; set; } 
        public double? Number { get; set; } 

        public InputType Type { get; set; }


        public static Input CreateMapOfany(Dictionary<string, object> mapOfany) {
            InputType typ = InputType.MapOfany;

            Input res = new Input(typ);
            res.MapOfany = mapOfany;
            return res;
        }

        public static Input CreateBoolean(bool boolean) {
            InputType typ = InputType.Boolean;

            Input res = new Input(typ);
            res.Boolean = boolean;
            return res;
        }

        public static Input CreateStr(string str) {
            InputType typ = InputType.Str;

            Input res = new Input(typ);
            res.Str = str;
            return res;
        }

        public static Input CreateArrayOfany(List<object> arrayOfany) {
            InputType typ = InputType.ArrayOfany;

            Input res = new Input(typ);
            res.ArrayOfany = arrayOfany;
            return res;
        }

        public static Input CreateNumber(double number) {
            InputType typ = InputType.Number;

            Input res = new Input(typ);
            res.Number = number;
            return res;
        }

        public static Input CreateNull() {
            InputType typ = InputType.Null;
            return new Input(typ);
        }

        public class InputConverter : JsonConverter
        {

            public override bool CanConvert(System.Type objectType) => objectType == typeof(Input);

            public override bool CanRead => true;

            public override object? ReadJson(JsonReader reader, System.Type objectType, object? existingValue, JsonSerializer serializer)
            { 
                var json = JRaw.Create(reader).ToString();

                if (json == "null") {
                    return null;
                }
                try
                {
                    Dictionary<string, object>? mapOfany = ResponseBodyDeserializer.Deserialize<Dictionary<string, object>>(json, missingMemberHandling: MissingMemberHandling.Error);
                    return new Input(InputType.MapOfany) {
                        MapOfany = mapOfany
                    };
                }
                catch (Exception ex)
                {
                    if (!(ex is Newtonsoft.Json.JsonReaderException || ex is Newtonsoft.Json.JsonSerializationException)) {
                        throw ex;
                    }
                } 
                try {
                    var converted = Convert.ToBoolean(json);
                    return new Input(InputType.Boolean) {
                        Boolean = converted
                    };
                } catch (System.FormatException) {
                    // try next option
                }
                if (json[0] == '"' && json[^1] == '"'){
                    return new Input(InputType.Str) {
                        Str = json[1..^1]
                    };
                }
                try
                {
                    List<object>? arrayOfany = ResponseBodyDeserializer.Deserialize<List<object>>(json, missingMemberHandling: MissingMemberHandling.Error);
                    return new Input(InputType.ArrayOfany) {
                        ArrayOfany = arrayOfany
                    };
                }
                catch (Exception ex)
                {
                    if (!(ex is Newtonsoft.Json.JsonReaderException || ex is Newtonsoft.Json.JsonSerializationException)) {
                        throw ex;
                    }
                } 
                try {
                    var converted = Convert.ToDouble(json);
                    return new Input(InputType.Number) {
                        Number = converted
                    };
                } catch (System.FormatException) {
                    // try next option
                }

                throw new InvalidOperationException("Could not deserialize into any supported types.");
            }

            public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
            {
                if (value == null) {
                    writer.WriteRawValue("null");
                    return;
                }
                Input res = (Input)value;
                if (InputType.FromString(res.Type).Equals(InputType.Null))
                {
                    writer.WriteRawValue("null");
                    return;
                }
                if (res.MapOfany != null)
                {
                    writer.WriteRawValue(Utilities.SerializeJSON(res.MapOfany));
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
                if (res.ArrayOfany != null)
                {
                    writer.WriteRawValue(Utilities.SerializeJSON(res.ArrayOfany));
                    return;
                }
                if (res.Number != null)
                {
                    writer.WriteRawValue(Utilities.SerializeJSON(res.Number));
                    return;
                }

            }
        }

    }

}