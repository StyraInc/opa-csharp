using System.Collections.Generic;
using Newtonsoft.Json;
using Styra.Opa.OpenApi.Models.Components;

namespace Styra.Opa;

public class OpaBatchInputs : Dictionary<string, Dictionary<string, object>>
{
    public OpaBatchInputs() : base() { }

    public OpaBatchInputs(int capacity) : base(capacity) { }

    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }
}

public class OpaBatchResults : Dictionary<string, OpaResult>
{
    public OpaBatchResults() : base() { }

    public OpaBatchResults(int capacity) : base(capacity) { }

    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }
}

public class OpaBatchResultGeneric<T> : Dictionary<string, T>
{
    public OpaBatchResultGeneric() : base() { }

    public OpaBatchResultGeneric(int capacity) : base(capacity) { }

    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }
}

public class OpaBatchErrors : Dictionary<string, OpaError>
{
    public OpaBatchErrors() : base() { }

    public OpaBatchErrors(int capacity) : base(capacity) { }

    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }
}

// Used for converting inputs for the Batch Query API, and converting result types
// into useful higher-level types.
public static class DictionaryExtensions
{
    public static Dictionary<string, Input> ToOpaBatchInputRaw(this Dictionary<string, Dictionary<string, object>> inputs)
    {
        var opaBatchInputs = new Dictionary<string, Input>(inputs.Count);
        foreach (var kvp in inputs)
        {
            opaBatchInputs[kvp.Key] = Input.CreateMapOfAny(kvp.Value);
        }
        return opaBatchInputs;
    }

    // The OpenApi.Models.Errors variant of the ServerError type.
    public static OpaBatchErrors ToOpaBatchErrors(this Dictionary<string, Styra.Opa.OpenApi.Models.Errors.ServerError> errors)
    {
        var opaBatchErrors = new OpaBatchErrors(errors.Count);
        foreach (var kvp in errors)
        {
            opaBatchErrors[kvp.Key] = new OpaError(kvp.Value);
        }
        return opaBatchErrors;
    }

    // The OpenApi.Models.Components variant of the ServerError type.
    public static OpaBatchErrors ToOpaBatchErrors(this Dictionary<string, Styra.Opa.OpenApi.Models.Components.ServerErrorWithStatusCode> errors)
    {
        var opaBatchErrors = new OpaBatchErrors(errors.Count);
        foreach (var kvp in errors)
        {
            opaBatchErrors[kvp.Key] = new OpaError(kvp.Value);
        }
        return opaBatchErrors;
    }

    public static OpaBatchErrors ToOpaBatchErrors(this Dictionary<string, Styra.Opa.OpenApi.Models.Components.ServerError> errors)
    {
        var opaBatchErrors = new OpaBatchErrors(errors.Count);
        foreach (var kvp in errors)
        {
            opaBatchErrors[kvp.Key] = new OpaError(kvp.Value);
        }
        return opaBatchErrors;
    }

    public static OpaBatchResults ToOpaBatchResults(this Dictionary<string, SuccessfulPolicyResponse> responses)
    {
        var opaBatchResults = new OpaBatchResults(responses.Count);
        foreach (var kvp in responses)
        {
            opaBatchResults[kvp.Key] = new OpaResult(kvp.Value);
        }
        return opaBatchResults;
    }

    public static OpaBatchResultGeneric<T> ToOpaBatchResults<T>(this Dictionary<string, SuccessfulPolicyResponse> responses)
    {
        var output = new OpaBatchResultGeneric<T>(responses.Count);
        foreach (var kvp in responses)
        {
            output[kvp.Key] = OpaClient.ConvertResult<T>(kvp.Value.Result!);
        }
        return output;
    }
}
