

using Newtonsoft.Json;
using Styra.Ucast.Linq;

namespace Styra.Opa.Filters;

// TODO: Figure out what we want to do here. Polymorphism? A discriminated union type?

/// <summary>
/// Wrapper type for the UCAST data filters EOPA generates.
/// </summary>
public class Filters : UCASTNode
{
    public Filters() : base() { }

    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }
}