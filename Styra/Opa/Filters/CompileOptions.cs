using System.Collections.Generic;
using Newtonsoft.Json;

namespace Styra.Opa.Filters;

/// <summary>
/// A data object, used to set Compile API options for Enterprise OPA.
/// </summary>
public class CompileOptions
{
    /// <summary>
    /// A list of paths to exclude from partial evaluation inlining.
    /// </summary>
    [JsonProperty("disableInlining")]
    public List<string>? DisableInlining { get; set; }

    /// <summary>
    /// The output targets for partial evaluation. Different targets will have different constraints.
    /// </summary>
    [JsonProperty("targetDialects")]
    public List<TargetDialects>? TargetDialects { get; set; }

    [JsonProperty("targetSQLTableMappings")]
    public TargetSQLTableMappings? TargetSQLTableMappings { get; set; }

    /// <summary>
    /// The Rego rule to evaluate for generating column masks.
    /// </summary>
    [JsonProperty("maskRule")]
    public string? MaskRule { get; set; }
}