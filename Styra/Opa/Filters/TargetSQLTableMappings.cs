using System.Collections.Generic;
using Newtonsoft.Json;

namespace Styra.Opa.Filters;

/// <summary>
/// Mapping between tables and columns for the Enterprise OPA Compile API.
/// Used in the <c>options.targetSQLTableMappings</c> payload field.
/// </summary>
/// <remarks>See: <see href="https://docs.styra.com/enterprise-opa/reference/api-reference/partial-evaluation-api#request-body"/></remarks>
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