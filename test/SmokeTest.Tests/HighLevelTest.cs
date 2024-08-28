using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Styra.Opa;
using Styra.Opa.OpenApi.Models.Components;
using Xunit.Abstractions;

namespace SmokeTest.Tests;

// Used to verify presence of log messages in tests.
public class ListLogger : ILogger<OpaClient>
{
  public List<string> Logs { get; } = new List<string>();

  IDisposable ILogger.BeginScope<TState>(TState state)
  {
    return null!;
  }

  public bool IsEnabled(LogLevel logLevel) => true;

  public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
  {
    if (formatter == null) { throw new ArgumentNullException(nameof(formatter)); };

    var message = formatter(state, exception);
    if (!string.IsNullOrEmpty(message))
    {
      Logs.Add(message);
    }
  }
}

// Note(philip): Run with `--logger "console;verbosity=detailed"` to see logged messages.
public class HighLevelTest : IClassFixture<OPAContainerFixture>, IClassFixture<EOPAContainerFixture>
{
  private readonly ITestOutputHelper _testOutput;
  public IContainer _containerOpa;
  public IContainer _containerEopa;

  private class CustomRBACInputObject
  {

    [JsonProperty("user")]
    public string User = "";

    [JsonProperty("action")]
    public string Action = "";

    [JsonProperty("object")]
    public string Object = "";

    [JsonProperty("type")]
    public string Type = "";

    [JsonIgnore]
    public string UUID = System.Guid.NewGuid().ToString();

    public CustomRBACInputObject() { }

    public CustomRBACInputObject(string user, string action, string obj, string type)
    {
      User = user;
      Action = action;
      Object = obj;
      Type = type;
    }
  }

  public class CustomRBACOutputObject
  {
    [JsonProperty("allow")]
    public bool Allow = false;

    [JsonProperty("user_is_admin")]
    public bool? IsAdmin;

    [JsonProperty("user_is_granted")]
    public List<object>? Grants;

    public CustomRBACOutputObject() { }
  }

  public HighLevelTest(OPAContainerFixture opaFixture, EOPAContainerFixture eopaFixture, ITestOutputHelper output)
  {
    _containerOpa = opaFixture.GetContainer();
    _containerEopa = eopaFixture.GetContainer();
    _testOutput = output;
  }

  private OpaClient GetOpaClient()
  {
    // Construct the request URI by specifying the scheme, hostname, assigned random host port, and the endpoint "uuid".
    var requestUri = new UriBuilder(Uri.UriSchemeHttp, _containerOpa.Hostname, _containerOpa.GetMappedPublicPort(8181)).Uri;
    return new OpaClient(serverUrl: requestUri.ToString());
  }

  private OpaClient GetOpaClientWithLogger(ILogger<OpaClient> logger)
  {
    var requestUri = new UriBuilder(Uri.UriSchemeHttp, _containerOpa.Hostname, _containerOpa.GetMappedPublicPort(8181)).Uri;
    return new OpaClient(serverUrl: requestUri.ToString(), logger: logger);
  }

  private OpaClient GetEOpaClient()
  {
    var requestUri = new UriBuilder(Uri.UriSchemeHttp, _containerEopa.Hostname, _containerEopa.GetMappedPublicPort(8181)).Uri;
    return new OpaClient(serverUrl: requestUri.ToString());
  }

  [Fact]
  public async Task RBACCheckDictionaryTest()
  {
    var client = GetOpaClient();

    var allow = await client.check("app/rbac/allow", new Dictionary<string, object>() {
      { "user", "alice" },
      { "action", "read" },
      { "object", "id123" },
      { "type", "dog" },
    });

    // BUG: This can fail as long as Speakeasy generates the upstream SDK with
    // deserializers occurring in the same ordering as the OpenAPI spec.
    Assert.True(allow);
  }

  [Fact]
  public async Task RBACCheckNullTest()
  {
    var client = GetOpaClient();

    var allow = await client.check("app/rbac/allow");

    Assert.False(allow);
  }

  [Fact]
  public async Task RBACCheckBoolTest()
  {
    var client = GetOpaClient();

    var allow = await client.check("app/rbac/allow", true);

    Assert.False(allow);
  }

  [Fact]
  public async Task RBACCheckDoubleTest()
  {
    var client = GetOpaClient();

    var allow = await client.check("app/rbac/allow", 42);

    Assert.False(allow);
  }

  [Fact]
  public async Task RBACCheckStringTest()
  {
    var client = GetOpaClient();

    var allow = await client.check("app/rbac/allow", "alice");

    Assert.False(allow);
  }

  [Fact]
  public async Task RBACCheckListObjTest()
  {
    var client = GetOpaClient();

    var allow = await client.check("app/rbac/allow", new List<object>() { "A", "B", "C", "D" });

    Assert.False(allow);
  }

  [Fact]
  public async Task DictionaryTypeCoerceTest()
  {
    var client = GetOpaClient();

    var input = new Dictionary<string, string>() {
      { "user", "alice" },
      { "action", "read" },
      { "object", "id123" },
      { "type", "dog" },
    };

    var result = new Dictionary<string, object>();

    try
    {
      result = await client.evaluate<Dictionary<string, object>>("app/rbac", input);
    }
    catch (OpaException e)
    {
      _testOutput.WriteLine("exception while making request against OPA: " + e.Message);
    }

    var expected = new Dictionary<string, object>() {
      { "allow", true },
      { "user_is_admin", true },
      { "user_is_granted", new List<object>()},
    };

    Assert.NotNull(result);
    Assert.Equivalent(expected, result);
    Assert.Equal(expected.Count, result.Count);
  }

  [Fact]
  public async Task BooleanInputTypeCoerceTest()
  {
    var client = GetOpaClient();

    var input = false;

    var result = new Dictionary<string, object>();

    try
    {
      result = await client.evaluate<Dictionary<string, object>>("app/rbac", input);
    }
    catch (OpaException e)
    {
      _testOutput.WriteLine("exception while making request against OPA: " + e.Message);
    }

    var expected = new Dictionary<string, object>() {
      { "allow", false },
      { "user_is_granted", new List<object>()},
    };

    Assert.NotNull(result);
    Assert.Equivalent(expected, result);
    Assert.Equal(expected.Count, result.Count);
  }

  [Fact]
  public async Task CustomClassInputTypeCoerceTest()
  {
    var client = GetOpaClient();

    var input = new CustomRBACInputObject("alice", "read", "id123", "dog");

    var result = new Dictionary<string, object>();

    try
    {
      result = await client.evaluate<Dictionary<string, object>>("app/rbac", input);
    }
    catch (OpaException e)
    {
      _testOutput.WriteLine("exception while making request against OPA: " + e.Message);
    }

    var expected = new Dictionary<string, object>() {
      { "allow", true },
      { "user_is_admin", true },
      { "user_is_granted", new List<object>()},
    };

    Assert.NotNull(result);
    Assert.Equivalent(expected, result);
    Assert.Equal(expected.Count, result.Count);
  }

  [Fact]
  public async Task CustomClassOutputTypeCoerceTest()
  {
    var client = GetOpaClient();

    var input = new CustomRBACInputObject("alice", "read", "id123", "dog");

    var result = new CustomRBACOutputObject();
    try
    {
      var res = await client.evaluate<CustomRBACOutputObject>("app/rbac", input);
      if (res is CustomRBACOutputObject value)
      {
        result = value;
      }
      else
      {
        Assert.Fail("Test did not deserialize to a custom C# type properly.");
      }
    }
    catch (OpaException e)
    {
      _testOutput.WriteLine("exception while making request against OPA: " + e.Message);
    }

    var expected = new CustomRBACOutputObject()
    {
      Allow = true,
      IsAdmin = true,
      Grants = new List<object>(),
    };

    Assert.NotNull(result);
    Assert.Equivalent(expected, result);
  }

  [Fact]
  public async Task BadOutputTypeCoerceTest()
  {
    var client = GetOpaClient();

    var input = new CustomRBACInputObject("alice", "read", "id123", "dog");

    // Attempt to coerce an object return type into a bool. This should always fail!
    await Assert.ThrowsAsync<OpaException>(async () => { var res = await client.evaluate<bool>("app/rbac", input); });
  }

  [Fact]
  public async Task AnonymousObjectInputTypeCoerceTest()
  {
    var client = GetOpaClient();

    // Relies on Newtonsoft.Json's default serialization rules. `object` is unused by
    // the policy, thankfully, so we can get away with mangling that field's name.
    var input = new { user = "alice", action = "read", _object = "id123", type = "dog" };

    var result = new Dictionary<string, object>();

    try
    {
      result = await client.evaluate<Dictionary<string, object>>("app/rbac", input);
    }
    catch (OpaException e)
    {
      _testOutput.WriteLine("exception while making request against OPA: " + e.Message);
    }

    var expected = new Dictionary<string, object>() {
      { "allow", true },
      { "user_is_admin", true },
      { "user_is_granted", new List<object>()},
    };

    Assert.NotNull(result);
    Assert.Equivalent(expected, result);
    Assert.Equal(expected.Count, result.Count);
  }

  [Fact]
  public async Task EvaluateDefaultTest()
  {
    var client = GetOpaClient();

    var res = await client.evaluateDefault<Dictionary<string, object>>(
      new Dictionary<string, object>() {
        { "hello", "world" },
      });

    Assert.Equal(new Dictionary<string, object>() { { "hello", "world" } }, res?.GetValueOrDefault("echo", ""));
  }

  [Fact]
  public async Task EvaluateDefaultWithAnonymousObjectInputTest()
  {
    var client = GetOpaClient();

    var res = await client.evaluateDefault<Dictionary<string, object>>(new { hello = "world" });

    Assert.Equal(new Dictionary<string, object>() { { "hello", "world" } }, res?.GetValueOrDefault("echo", ""));
  }

  [Fact]
  public async Task RBACBatchAllSuccessTest()
  {
    var client = GetEOpaClient();

    var goodInput = new Dictionary<string, object>() {
      { "user", "alice" },
      { "action", "read" },
      { "object", "id123" },
      { "type", "dog" }
    };

    var (successes, failures) = await client.evaluateBatch("app/rbac/allow", new Dictionary<string, Dictionary<string, object>>() {
      {"AAA", goodInput },
      {"BBB", goodInput },
      {"CCC", goodInput },
    });

    var expSuccess = new OpaResult() { Result = Result.CreateBoolean(true) };

    // Assert that the successes dictionary has all expected elements, and the
    // failures dictionary is empty.
    Assert.Equivalent(new Dictionary<string, OpaResult>() {
      { "AAA", expSuccess },
      { "BBB", expSuccess },
      { "CCC", expSuccess },
    }, successes);
    Assert.Empty(failures);
  }

  [Fact]
  public async Task RBACBatchMixedTest()
  {
    var client = GetEOpaClient();

    var goodInput = new Dictionary<string, object>() {
      { "x", new List<int> {1, 1, 3} },
      { "y", new List<int> {1, 1, 1} },
    };

    var badInput = new Dictionary<string, object>() {
      { "x", new List<int> {1, 1, 3} },
      { "y", new List<int> {1, 2, 1} },
    };

    var (successes, failures) = await client.evaluateBatch("testmod/condfail", new Dictionary<string, Dictionary<string, object>>() {
      {"AAA", badInput },
      {"BBB", goodInput },
      {"CCC", badInput },
    });

    var expSuccess = new OpaResult()
    {
      HttpStatusCode = "200",
      Result = Result.CreateMapOfAny(
        new Dictionary<string, object>() {
          {"p", new Dictionary<string, object>() { { "1", 2 }, { "3", 4 } } }
        }
      )
    };
    var expError = new OpaError()
    {
      Code = "internal_error",
      DecisionId = null,
      HttpStatusCode = "500",
      Message = "object insert conflict"
    };

    // Assert that the failures dictionary has all expected elements, and the
    // successes dictionary is empty.
    Assert.Equivalent(new OpaBatchResults() { { "BBB", expSuccess } }, successes);
    Assert.Equivalent(new Dictionary<string, OpaError>() {
      { "AAA", expError },
      { "CCC", expError },
    }, failures);

  }

  [Fact]
  public async Task RBACBatchAllFailuresTest()
  {
    var client = GetEOpaClient();

    var badInput = new Dictionary<string, object>() {
      { "x", new List<int> {1, 1, 3} },
      { "y", new List<int> {1, 2, 1} },
    };

    var (successes, failures) = await client.evaluateBatch("testmod/condfail", new Dictionary<string, Dictionary<string, object>>() {
      {"AAA", badInput },
      {"BBB", badInput },
      {"CCC", badInput },
    });

    var expError = new OpaError()
    {
      Code = "internal_error",
      DecisionId = null,
      Message = "object insert conflict"
    };

    // Assert that the failures dictionary has all expected elements, and the
    // successes dictionary is empty.
    Assert.Empty(successes);
    Assert.Equivalent(new Dictionary<string, OpaError>() {
      { "AAA", expError },
      { "BBB", expError },
      { "CCC", expError },
    }, failures);

  }

  [Fact]
  public async Task RBACBatchAllSuccessFallbackTest()
  {
    var client = GetOpaClient();

    var goodInput = new Dictionary<string, object>() {
      { "x", new List<int> {1, 1, 3} },
      { "y", new List<int> {1, 1, 1} },
    };

    var (successes, failures) = await client.evaluateBatch("testmod/condfail", new Dictionary<string, Dictionary<string, object>>() {
      {"AAA", goodInput },
      {"BBB", goodInput },
      {"CCC", goodInput },
    });

    var expSuccess = new OpaResult()
    {
      Result = Result.CreateMapOfAny(
        new Dictionary<string, object>() {
          {"p", new Dictionary<string, object>() { { "1", 2 }, { "3", 4 } } }
        }
      )
    };

    // Assert that the failures dictionary has all expected elements, and the
    // successes dictionary is empty.
    Assert.Equivalent(new OpaBatchResults() {
      { "BBB", expSuccess },
      { "AAA", expSuccess },
      { "CCC", expSuccess }
    }, successes);
    Assert.Empty(failures);

  }

  [Fact]
  public async Task RBACBatchMixedFallbackTest()
  {
    var client = GetOpaClient();

    var goodInput = new Dictionary<string, object>() {
      { "x", new List<int> {1, 1, 3} },
      { "y", new List<int> {1, 1, 1} },
    };

    var badInput = new Dictionary<string, object>() {
      { "x", new List<int> {1, 1, 3} },
      { "y", new List<int> {1, 2, 1} },
    };

    var (successes, failures) = await client.evaluateBatch("testmod/condfail", new Dictionary<string, Dictionary<string, object>>() {
      {"AAA", badInput },
      {"BBB", goodInput },
      {"CCC", badInput },
    });

    var expSuccess = new OpaResult()
    {
      HttpStatusCode = "200",
      Result = Result.CreateMapOfAny(
        new Dictionary<string, object>() {
          {"p", new Dictionary<string, object>() { { "1", 2 }, { "3", 4 } } }
        }
      )
    };
    var expError = new OpaError()
    {
      Code = "internal_error",
      DecisionId = null,
      HttpStatusCode = "500",
      Message = "error(s) occurred while evaluating query" // Note: different error message for OPA mode.
    };

    // Assert that the failures dictionary has all expected elements, and the
    // successes dictionary is empty.
    Assert.Equivalent(new OpaBatchResults() { { "BBB", expSuccess } }, successes);
    Assert.Equivalent(new Dictionary<string, OpaError>() {
      { "AAA", expError },
      { "CCC", expError },
    }, failures);

  }

  [Fact]
  public async Task RBACBatchAllFailuresFallbackTest()
  {
    var client = GetOpaClient();

    var badInput = new Dictionary<string, object>() {
      { "x", new List<int> {1, 1, 3} },
      { "y", new List<int> {1, 2, 1} },
    };

    var (successes, failures) = await client.evaluateBatch("testmod/condfail", new Dictionary<string, Dictionary<string, object>>() {
      {"AAA", badInput },
      {"BBB", badInput },
      {"CCC", badInput },
    });

    var expError = new OpaError()
    {
      Code = "internal_error",
      DecisionId = null,
      Message = "error(s) occurred while evaluating query" // Note: different error message for OPA mode.
    };

    // Assert that the failures dictionary has all expected elements, and the
    // successes dictionary is empty.
    Assert.Empty(successes);
    Assert.Equivalent(new Dictionary<string, OpaError>() {
      { "AAA", expError },
      { "BBB", expError },
      { "CCC", expError },
    }, failures);

  }

  // The generic version of the batch queries.
  [Fact]
  public async Task RBACBatchGenericAllSuccessTest()
  {
    var client = GetEOpaClient();

    var goodInput = new Dictionary<string, object>() {
      { "user", "alice" },
      { "action", "read" },
      { "object", "id123" },
      { "type", "dog" }
    };

    var (successes, failures) = await client.evaluateBatch<bool>("app/rbac/allow", new Dictionary<string, Dictionary<string, object>>() {
      {"AAA", goodInput },
      {"BBB", goodInput },
      {"CCC", goodInput },
    });

    var expSuccess = true;

    // Assert that the successes dictionary has all expected elements, and the
    // failures dictionary is empty.
    Assert.Equivalent(new Dictionary<string, object>() {
      { "AAA", expSuccess },
      { "BBB", expSuccess },
      { "CCC", expSuccess },
    }, successes);
    Assert.Empty(failures);
  }

  [Fact]
  public async Task RBACBatchGenericMixedTest()
  {
    var client = GetEOpaClient();

    var goodInput = new Dictionary<string, object>() {
      { "x", new List<int> {1, 1, 3} },
      { "y", new List<int> {1, 1, 1} },
    };

    var badInput = new Dictionary<string, object>() {
      { "x", new List<int> {1, 1, 3} },
      { "y", new List<int> {1, 2, 1} },
    };

    var (successes, failures) = await client.evaluateBatch<Dictionary<string, object>>("testmod/condfail", new Dictionary<string, Dictionary<string, object>>() {
      {"AAA", badInput },
      {"BBB", goodInput },
      {"CCC", badInput },
    });

    var expSuccess =
        new Dictionary<string, object>() {
          {"p", new Dictionary<string, object>() { { "1", 2 }, { "3", 4 } } }
        };
    var expError = new OpaError()
    {
      Code = "internal_error",
      DecisionId = null,
      HttpStatusCode = "500",
      Message = "object insert conflict"
    };

    // Assert that the failures dictionary has all expected elements, and the
    // successes dictionary is empty.
    Assert.Equivalent(new OpaBatchResultGeneric<Dictionary<string, object>>() { { "BBB", expSuccess } }, successes);
    Assert.Equivalent(new Dictionary<string, OpaError>() {
      { "AAA", expError },
      { "CCC", expError },
    }, failures);

  }

  [Fact]
  public async Task RBACBatchGenericAllFailuresTest()
  {
    var client = GetEOpaClient();

    var badInput = new Dictionary<string, object>() {
      { "x", new List<int> {1, 1, 3} },
      { "y", new List<int> {1, 2, 1} },
    };

    var (successes, failures) = await client.evaluateBatch<Dictionary<string, object>>("testmod/condfail", new Dictionary<string, Dictionary<string, object>>() {
      {"AAA", badInput },
      {"BBB", badInput },
      {"CCC", badInput },
    });

    var expError = new OpaError()
    {
      Code = "internal_error",
      DecisionId = null,
      Message = "object insert conflict"
    };

    // Assert that the failures dictionary has all expected elements, and the
    // successes dictionary is empty.
    Assert.Empty(successes);
    Assert.Equivalent(new Dictionary<string, OpaError>() {
      { "AAA", expError },
      { "BBB", expError },
      { "CCC", expError },
    }, failures);

  }

  [Fact]
  public async Task RBACBatchGenericAllSuccessFallbackTest()
  {
    var client = GetOpaClient();

    var goodInput = new Dictionary<string, object>() {
      { "user", "alice" },
      { "action", "read" },
      { "object", "id123" },
      { "type", "dog" }
    };

    var (successes, failures) = await client.evaluateBatch<bool>("app/rbac/allow", new Dictionary<string, Dictionary<string, object>>() {
      {"AAA", goodInput },
      {"BBB", goodInput },
      {"CCC", goodInput },
    });

    var expSuccess = true;

    // Assert that the successes dictionary has all expected elements, and the
    // failures dictionary is empty.
    Assert.Equivalent(new Dictionary<string, object>() {
      { "AAA", expSuccess },
      { "BBB", expSuccess },
      { "CCC", expSuccess },
    }, successes);
    Assert.Empty(failures);
  }

  [Fact]
  public async Task RBACBatchGenericMixedFallbackTest()
  {
    var client = GetOpaClient();

    var goodInput = new Dictionary<string, object>() {
      { "x", new List<int> {1, 1, 3} },
      { "y", new List<int> {1, 1, 1} },
    };

    var badInput = new Dictionary<string, object>() {
      { "x", new List<int> {1, 1, 3} },
      { "y", new List<int> {1, 2, 1} },
    };

    var (successes, failures) = await client.evaluateBatch<Dictionary<string, object>>("testmod/condfail", new Dictionary<string, Dictionary<string, object>>() {
      {"AAA", badInput },
      {"BBB", goodInput },
      {"CCC", badInput },
    });

    var expSuccess =
        new Dictionary<string, object>() {
          {"p", new Dictionary<string, object>() { { "1", 2 }, { "3", 4 } } }
        };
    var expError = new OpaError()
    {
      Code = "internal_error",
      DecisionId = null,
      HttpStatusCode = "500",
      Message = "error(s) occurred while evaluating query" // Note: different error message for OPA mode.
    };

    // Assert that the failures dictionary has all expected elements, and the
    // successes dictionary is empty.
    Assert.Equivalent(new OpaBatchResultGeneric<Dictionary<string, object>>() { { "BBB", expSuccess } }, successes);
    Assert.Equivalent(new Dictionary<string, OpaError>() {
      { "AAA", expError },
      { "CCC", expError },
    }, failures);

  }

  [Fact]
  public async Task RBACBatchGenericAllFailuresFallbackTest()
  {
    var client = GetOpaClient();

    var badInput = new Dictionary<string, object>() {
      { "x", new List<int> {1, 1, 3} },
      { "y", new List<int> {1, 2, 1} },
    };

    var (successes, failures) = await client.evaluateBatch<Dictionary<string, object>>("testmod/condfail", new Dictionary<string, Dictionary<string, object>>() {
      {"AAA", badInput },
      {"BBB", badInput },
      {"CCC", badInput },
    });

    var expError = new OpaError()
    {
      Code = "internal_error",
      DecisionId = null,
      Message = "error(s) occurred while evaluating query" // Note: different error message for OPA mode.
    };

    // Assert that the failures dictionary has all expected elements, and the
    // successes dictionary is empty.
    Assert.Empty(successes);
    Assert.Equivalent(new Dictionary<string, OpaError>() {
      { "AAA", expError },
      { "BBB", expError },
      { "CCC", expError },
    }, failures);

  }

  [Fact]
  public async Task LogsExistTest()
  {
    var logger = new ListLogger();
    var client = GetOpaClientWithLogger(logger);

    var badInput = new Dictionary<string, object>() {
      { "x", new List<int> {1, 1, 3} },
      { "y", new List<int> {1, 2, 1} },
    };

    try
    {
      var result = await client.evaluate<bool>("testmod/condfail", badInput);
    }
    catch (OpaException e)
    {
      // Do nothing.
      _testOutput.WriteLine(e.Message);
    }

    Assert.Single(logger.Logs);
    Assert.Contains("executing policy 'testmod/condfail' failed with exception: ", logger.Logs[0]);
  }
}