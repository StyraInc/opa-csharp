
using System;
using Newtonsoft.Json;

namespace Styra.Opa.Filters;

public enum TargetDialects
{
    [JsonProperty("ucast+all")]
    UcastPlusAll,
    [JsonProperty("ucast+minimal")]
    UcastPlusMinimal,
    [JsonProperty("ucast+prisma")]
    UcastPlusPrisma,
    [JsonProperty("ucast+linq")]
    UcastPlusLinq,
    [JsonProperty("sql+sqlserver")]
    SqlPlusSqlserver,
    [JsonProperty("sql+mysql")]
    SqlPlusMysql,
    [JsonProperty("sql+postgresql")]
    SqlPlusPostgresql,
    [JsonProperty("sql+sqlite")]
    SqlPlusSqlite,
}

public static class TargetDialectsExtension
{
    public static string Value(this TargetDialects value)
    {
        return ((JsonPropertyAttribute)value.GetType().GetMember(value.ToString())[0].GetCustomAttributes(typeof(JsonPropertyAttribute), false)[0]).PropertyName ?? value.ToString();
    }

    public static TargetDialects ToEnum(this string value)
    {
        foreach (var field in typeof(TargetDialects).GetFields())
        {
            var attributes = field.GetCustomAttributes(typeof(JsonPropertyAttribute), false);
            if (attributes.Length == 0)
            {
                continue;
            }

            if (attributes[0] is JsonPropertyAttribute attribute && attribute.PropertyName == value)
            {
                var enumVal = field.GetValue(null);

                if (enumVal is TargetDialects dialects)
                {
                    return dialects;
                }
            }
        }

        throw new Exception($"Unknown value {value} for enum TargetDialects");
    }
}