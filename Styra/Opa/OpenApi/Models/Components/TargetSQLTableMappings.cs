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
    using System.Collections.Generic;
    
    public class TargetSQLTableMappings
    {

        [JsonProperty("sqlserver")]
        public Dictionary<string, object>? Sqlserver { get; set; }

        [JsonProperty("mysql")]
        public Dictionary<string, object>? Mysql { get; set; }

        [JsonProperty("postgresql")]
        public Dictionary<string, object>? Postgresql { get; set; }

        [JsonProperty("sqlite")]
        public Dictionary<string, object>? Sqlite { get; set; }
    }
}