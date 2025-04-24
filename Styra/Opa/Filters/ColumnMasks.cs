

using System.Collections.Generic;
using Newtonsoft.Json;
using Styra.Ucast.Linq;

namespace Styra.Opa.Filters;

/// <summary>
/// Wrapper type for the column masking rule objects Enterprise OPA generates alongside data filtering results.
/// </summary>
/// <remarks>See: <see href="https://docs.styra.com/apps/data/explanation/column-masks"/></remarks>
public class ColumnMasks : Dictionary<string, Dictionary<string, MaskingFunc>>
{
    public ColumnMasks() : base() { }
    public ColumnMasks(int capacity) : base(capacity) { }

    [JsonConstructor]
    public ColumnMasks(IDictionary<string, Dictionary<string, MaskingFunc>> dictionary)
        : base(dictionary) { }

    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }
}