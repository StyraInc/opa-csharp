

using Newtonsoft.Json;
using Styra.Ucast.Linq;

namespace Styra.Opa.Filters;

public enum FilterType
{
    Invalid,
    UCAST,
    SQL,
    Multitarget,
}

public interface IFilter
{
    FilterType GetFilterType();
    string ToString();
}

/// <summary>
/// Wrapper type for the UCAST data filters EOPA generates.
/// </summary>
public class UCASTFilter : UCASTNode, IFilter
{
    public UCASTFilter() : base() { }

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
/// Wrapper type for the SQL data filters EOPA generates.
/// </summary>
public class SQLFilter : IFilter
{
    [JsonProperty("query")]
    public string Query;
    private readonly string dialect;

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


public class CompileResultMultiTarget
{
    [JsonProperty("ucast")]
    public Filters.CompileResultUCAST? Ucast { get; set; }

    [JsonProperty("postgresql")]
    public Filters.CompileResultSQL? PostgreSql { get; set; }

    [JsonProperty("mysql")]
    public Filters.CompileResultSQL? MySql { get; set; }

    [JsonProperty("sqlserver")]
    public Filters.CompileResultSQL? SqlServer { get; set; }

    [JsonProperty("sqlite")]
    public Filters.CompileResultSQL? Sqlite { get; set; }
}

public record CompileResultMultitargetRecord([JsonProperty("result")] CompileResultMultiTarget Result);

public class CompileResultUCAST
{
    [JsonProperty("query")]
    public required UCASTNode Query;

    [JsonProperty("masks")]
    public ColumnMasks? Masks;
}

public record CompileResultUCASTRecord([JsonProperty("result")] CompileResultUCAST Result);

public class CompileResultSQL
{
    [JsonProperty("query")]
    public required string Query;

    [JsonProperty("masks")]
    public ColumnMasks? Masks;
}

public record CompileResultSQLRecord([JsonProperty("result")] CompileResultSQL Result);