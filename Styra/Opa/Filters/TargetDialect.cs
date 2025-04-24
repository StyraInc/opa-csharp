
using System;
using Newtonsoft.Json;

namespace Styra.Opa.Filters;

/// <summary>
/// An enum of all data filter formats that Enterprise OPA supports in the Compile API.
/// Used in the <c>options.targetDialects</c> payload field, and for <c>Accept</c> header selection.
/// </summary>
/// <remarks>See: <see href="https://docs.styra.com/enterprise-opa/reference/api-reference/partial-evaluation-api"/></remarks>
public enum TargetDialects
{
    [JsonProperty("ucast+all")]
    UcastAll,
    [JsonProperty("ucast+minimal")]
    UcastMinimal,
    [JsonProperty("ucast+prisma")]
    UcastPrisma,
    [JsonProperty("ucast+linq")]
    UcastLinq,
    [JsonProperty("sql+sqlserver")]
    SqlSqlserver,
    [JsonProperty("sql+mysql")]
    SqlMysql,
    [JsonProperty("sql+postgresql")]
    SqlPostgresql,
    [JsonProperty("sql+sqlite")]
    SqlSqlite,
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

    /// <summary>
    /// Generates an HTTP <c>Accept</c> header string for use with Enterprise OPA's Compile API.
    /// </summary>
    /// <param name="value">The TargetDialects value to use.</param>
    /// <returns>An <c>Accept</c> header string of the format <c>application/vnd.styra.${data_filter_type}+json</c>.</returns>
    /// <remarks>See: <see href="https://docs.styra.com/enterprise-opa/reference/api-reference/partial-evaluation-api#accept-header--controlling-the-target-response-format"/></remarks>
    public static string ToAcceptHeader(this TargetDialects value)
    {
        return value switch
        {
            TargetDialects.UcastAll => "application/vnd.styra.ucast.all+json",
            TargetDialects.UcastMinimal => "application/vnd.styra.ucast.minimal+json",
            TargetDialects.UcastPrisma => "application/vnd.styra.ucast.prisma+json",
            TargetDialects.UcastLinq => "application/vnd.styra.ucast.linq+json",
            TargetDialects.SqlSqlserver => "application/vnd.styra.sql.sqlserver+json",
            TargetDialects.SqlMysql => "application/vnd.styra.sql.mysql+json",
            TargetDialects.SqlPostgresql => "application/vnd.styra.sql.postgresql+json",
            TargetDialects.SqlSqlite => "application/vnd.styra.sql.sqlite+json",
            _ => "application/json",
        };
    }

    /// <summary>
    /// Generates a dialect string for use in Enterprise OPA's Compile API.
    /// The string can be used in the <c>options.targetDialects</c> payload field.
    /// </summary>
    /// <param name="value">The TargetDialects value to use.</param>
    /// <returns>A dialect string for the <c>options.targetDialects</c> payload field.</returns>
    /// <remarks>See: <see href="https://docs.styra.com/enterprise-opa/reference/api-reference/partial-evaluation-api#request-body"/></remarks>
    public static string ToOptionString(this TargetDialects value)
    {
        return value switch
        {
            TargetDialects.UcastAll => "ucast+all",
            TargetDialects.UcastMinimal => "ucast+minimal",
            TargetDialects.UcastPrisma => "ucast+prisma",
            TargetDialects.UcastLinq => "ucast+linq",
            TargetDialects.SqlSqlserver => "sql+sqlserver",
            TargetDialects.SqlMysql => "sql+mysql",
            TargetDialects.SqlPostgresql => "sql+postgresql",
            TargetDialects.SqlSqlite => "sql+sqlite",
            _ => "unknown",
        };
    }
}