

using System.Collections.Generic;
using Newtonsoft.Json;
using Styra.Ucast.Linq;

namespace Styra.Opa.Filters;

/// <summary>
/// Wrapper type for the column masking rule objects EOPA generates alongside data filtering results.
/// </summary>
public class ColumnMasks : Dictionary<string, Dictionary<string, MaskingFunc>>
{
    public ColumnMasks() : base() { }

    public ColumnMasks(int capacity) : base(capacity) { }

    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }
}