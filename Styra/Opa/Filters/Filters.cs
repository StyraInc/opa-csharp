

using Newtonsoft.Json;
using Styra.Ucast.Linq;

namespace Styra.Opa.Filters;

public enum FilterType
{
    Invalid,
    UCAST,
    SQL,
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
    private readonly string query;
    private readonly string dialect;

    public SQLFilter(string sqlQuery, string sqlDialect)
    {
        query = sqlQuery;
        dialect = sqlDialect;
    }

    public FilterType GetFilterType()
    {
        return FilterType.SQL;
    }

    public override string ToString()
    {
        return query;
    }
}