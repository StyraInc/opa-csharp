

using System;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;
using Styra.Ucast.Linq;

namespace Styra.Opa.Filters;

/// <summary>
/// Coarse-grained types of filters. Useful for differentiating between UCAST and SQL-based data filters.
/// </summary>
public enum FilterType
{
    Invalid,
    UCAST,
    SQL,
    Multitarget,
}

/// <summary>
/// A simple interface around data filter types.
/// </summary>
public interface IFilter
{
    FilterType GetFilterType();
    string ToString();
}

/// <summary>
/// Wrapper type for the UCAST data filters Enterprise OPA generates.
/// See the <see cref="Styra.Ucast.Linq.UCASTNode" /> docs for details.
/// </summary>
public class UCASTFilter : UCASTNode, IFilter
{
    // For object initializations.
    public UCASTFilter() : base() { }

    // For direct conversion from the underlying type.
    [SetsRequiredMembers]
    public UCASTFilter(UCASTNode query) : base(query.Type, query.Op, query.Field, query.Value) { }

    public FilterType GetFilterType()
    {
        return FilterType.UCAST;
    }

    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }
}

/// <summary>
/// Wrapper type for the SQL data filters Enterprise OPA generates.
/// </summary>
public class SQLFilter : IFilter
{
    [JsonProperty("query")]
    public required string Query;
    private readonly string? dialect;

    public SQLFilter() { }

    [SetsRequiredMembers]
    public SQLFilter(string sqlQuery, string sqlDialect)
    {
        Query = sqlQuery;
        dialect = sqlDialect;
    }

    public FilterType GetFilterType()
    {
        return FilterType.SQL;
    }

    public override string ToString()
    {
        return Query;
    }
}

/// <summary>
/// Wrapper type for the multi-target data filters Enterprise OPA generates.
/// </summary>
public class CompileResultMultiTarget
{
    [JsonProperty("ucast")]
    public CompileResultUCAST? Ucast { get; set; }

    [JsonProperty("postgresql")]
    public CompileResultSQL? PostgreSql { get; set; }

    [JsonProperty("mysql")]
    public CompileResultSQL? MySql { get; set; }

    [JsonProperty("sqlserver")]
    public CompileResultSQL? SqlServer { get; set; }

    [JsonProperty("sqlite")]
    public CompileResultSQL? Sqlite { get; set; }
}

/// <summary>
/// Serialization wrapper type for multi-target data filters.
/// </summary>
/// <param name="Result"></param>
public record CompileResultMultitargetRecord([JsonProperty("result")] CompileResultMultiTarget Result);

/// <summary>
/// Serialization format for UCAST data filters and column masks.
/// </summary>
public class CompileResultUCAST
{
    [JsonProperty("query")]
    public required UCASTNode Query;

    [JsonProperty("masks")]
    public ColumnMasks? Masks;
}

/// <summary>
/// Serialization wrapper type for UCAST data filters.
/// </summary>
/// <param name="Result"></param>
public record CompileResultUCASTRecord([JsonProperty("result")] CompileResultUCAST Result);

/// <summary>
/// Serialization format for SQL data filters and column masks.
/// </summary>
public class CompileResultSQL
{
    [JsonProperty("query")]
    public required string Query;

    [JsonProperty("masks")]
    public ColumnMasks? Masks;
}

/// <summary>
/// Serialization wrapper type for SQL data filters.
/// </summary>
/// <param name="Result"></param>
public record CompileResultSQLRecord([JsonProperty("result")] CompileResultSQL Result);