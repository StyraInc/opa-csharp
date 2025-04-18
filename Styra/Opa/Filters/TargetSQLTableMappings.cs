using System.Collections.Generic;
using Newtonsoft.Json;

namespace Styra.Opa.Filters;

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