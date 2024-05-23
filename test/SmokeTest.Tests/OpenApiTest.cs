using Styra.Opa.OpenApi;
using Styra.Opa.OpenApi.Models.Requests;
using Styra.Opa.OpenApi.Models.Components;
using Styra.Opa.OpenApi.Models.Errors;
using System.Net.Http.Json;

namespace SmokeTest.Tests;

public class OpenApiTest : IClassFixture<OPAContainerFixture>, IClassFixture<EOPAContainerFixture>
{
  public IContainer _containerOpa;
  public IContainer _containerEopa;

  public OpenApiTest(OPAContainerFixture opaFixture, EOPAContainerFixture eopaFixture)
  {
    _containerOpa = opaFixture.GetContainer();
    _containerEopa = eopaFixture.GetContainer();
  }

  private OpaApiClient GetOpaApiClient()
  {
    // Construct the request URI by specifying the scheme, hostname, assigned random host port, and the endpoint "uuid".
    var requestUri = new UriBuilder(Uri.UriSchemeHttp, _containerOpa.Hostname, _containerOpa.GetMappedPublicPort(8181)).Uri;

    // Send an HTTP GET request to the specified URI and retrieve the response as a string.
    return new OpaApiClient(serverIndex: 0, serverUrl: requestUri.ToString());
  }

  private OpaApiClient GetEOpaApiClient()
  {
    // Construct the request URI by specifying the scheme, hostname, assigned random host port, and the endpoint "uuid".
    var requestUri = new UriBuilder(Uri.UriSchemeHttp, _containerEopa.Hostname, _containerEopa.GetMappedPublicPort(8181)).Uri;

    // Send an HTTP GET request to the specified URI and retrieve the response as a string.
    return new OpaApiClient(serverIndex: 0, serverUrl: requestUri.ToString());
  }

  [Fact]
  public async Task OpenApiClientRBACTestcontainersTest()
  {
    var client = GetOpaApiClient();

    // Exercise the low-level OPA C# SDK.
    ExecutePolicyWithInputRequest req = new ExecutePolicyWithInputRequest()
    {
      Path = "app/rbac",
      RequestBody = new ExecutePolicyWithInputRequestBody()
      {
        Input = Input.CreateMapOfAny(
                    new Dictionary<string, object>() {
                    { "user", "alice" },
                    { "action", "read" },
                    { "object", "id123" },
                    { "type", "dog" },
                    }),
      },
    };

    var res = await client.ExecutePolicyWithInputAsync(req);
    var resultMap = res.SuccessfulPolicyResponse?.Result?.MapOfAny;

    // Ensure we got back the expected fields from the eval.
    Assert.Equal(true, resultMap?.GetValueOrDefault("allow", false));
    Assert.Equal(true, resultMap?.GetValueOrDefault("user_is_admin", false));
    Assert.Equal(new List<string>(), resultMap?.GetValueOrDefault("user_is_granted"));
  }

  [Fact]
  public async Task OpenApiClientEncodedPathTest()
  {
    var client = GetOpaApiClient();

    // Exercise the low-level OPA C# SDK.
    ExecutePolicyRequest req = new ExecutePolicyRequest()
    {
      Path = "this/is%2fallowed/pkg",
    };

    var res = await client.ExecutePolicyAsync(req);
    var resultMap = res.SuccessfulPolicyResponse?.Result?.MapOfAny;

    Assert.Equal(true, resultMap?.GetValueOrDefault("allow", false));
  }

  [Fact]
  public async Task OpenApiClientEvaluateDefaultTest()
  {
    var client = GetOpaApiClient();

    // Note(philip): Due to how the API is generated, we have to fire off
    // requests directly-- there's no building of requests in advance for later
    // launching.
    var res = await client.ExecuteDefaultPolicyWithInputAsync(Input.CreateMapOfAny(
                    new Dictionary<string, object>() {
                    { "hello", "world" },
                    }));
    var resultMap = res.Result?.MapOfAny;

    // Ensure we got back the expected fields from the eval.
    Assert.Equal("this is the default path", resultMap?.GetValueOrDefault("msg", ""));
    Assert.Equal(new Dictionary<string, object>() { { "hello", "world" } }, resultMap?.GetValueOrDefault("echo", ""));
  }

  [Fact]
  public async Task OpenApiClientBatchPolicyNoInputTest()
  {
    // Currently, this API only exists in Enterprise OPA.
    var client = GetEOpaApiClient();

    ExecuteBatchPolicyWithInputRequest req = new ExecuteBatchPolicyWithInputRequest()
    {
      Path = "app/rbac",
      RequestBody = new ExecuteBatchPolicyWithInputRequestBody()
      {
        Inputs = new Dictionary<string, Input>() { },
      }
    };

    var res = await client.ExecuteBatchPolicyWithInputAsync(req);

    // Assert we get the expected "success" response
    Assert.Equal(200, res.StatusCode);

    // Assert no responses.
    Assert.NotNull(res.BatchSuccessfulPolicyEvaluation);
    Assert.Null(res.BatchSuccessfulPolicyEvaluation?.Responses);
  }

  [Fact]
  public async Task OpenApiClientBatchPolicyAllSuccessTest()
  {
    // Currently, this API only exists in Enterprise OPA.
    var client = GetEOpaApiClient();

    ExecuteBatchPolicyWithInputRequest req = new ExecuteBatchPolicyWithInputRequest()
    {
      Path = "app/rbac",
      RequestBody = new ExecuteBatchPolicyWithInputRequestBody()
      {
        Inputs = new Dictionary<string, Input>() {
          {"AAA", Input.CreateMapOfAny(
            new Dictionary<string, object>() {
              { "user", "alice" },
              { "action", "read" },
              { "object", "id123" },
              { "type", "dog" },
            })
          },
          {"BBB", Input.CreateMapOfAny(
            new Dictionary<string, object>() {
            { "user", "eve" },
            { "action", "write" },
            { "object", "id123" },
            { "type", "dog" },
          })
        },
      }
      }
    };

    var res = await client.ExecuteBatchPolicyWithInputAsync(req);

    // Assert we get the expected "success" response
    Assert.Equal(200, res.StatusCode);

    var responsesMap = res.BatchSuccessfulPolicyEvaluation?.Responses;
    {
      var resp = responsesMap?.GetValueOrDefault("AAA");
      Assert.NotNull(resp);
      Assert.Equal(true, resp?.Result?.MapOfAny?.GetValueOrDefault("allow"));
    }
    {
      var resp = responsesMap?.GetValueOrDefault("BBB");
      Assert.Equal(false, resp?.Result?.MapOfAny?.GetValueOrDefault("allow"));
    }
  }

  [Fact]
  public async Task OpenApiClientBatchPolicyAllFailureTest()
  {
    // Currently, this API only exists in Enterprise OPA.
    var client = GetEOpaApiClient();

    ExecuteBatchPolicyWithInputRequest req = new ExecuteBatchPolicyWithInputRequest()
    {
      Path = "testmod/condfail",
      RequestBody = new ExecuteBatchPolicyWithInputRequestBody()
      {
        Inputs = new Dictionary<string, Input>() {
          {"AAA", Input.CreateMapOfAny(
            new Dictionary<string, object>() {
              { "x", new List<int> {1, 1, 3} },
              { "y", new List<int> {1, 1, 1} },
            })
          },
          {"BBB", Input.CreateMapOfAny(
            new Dictionary<string, object>() {
              { "x", new List<int> {1, 1, 3} },
              { "y", new List<int> {1, 2, 1} },
            })
          },
          {"CCC", Input.CreateMapOfAny(
            new Dictionary<string, object>() {
              { "x", new List<int> {1, 1, 3} },
              { "y", new List<int> {1, 1, 1} },
            })
          }
        },
      }
    };

    var res = await client.ExecuteBatchPolicyWithInputAsync(req);

    // Assert we get the expected "success" response
    Assert.Equal(207, res.StatusCode);

    var responsesMap = res.BatchMixedResults?.Responses;
    {
      var resp = responsesMap?.GetValueOrDefault("AAA");
      Assert.NotNull(resp);
      Assert.Equivalent(new Dictionary<string, object>() { { "1", 2 }, { "3", 4 } }, resp?.ResponsesSuccessfulPolicyResponse?.Result?.MapOfAny?.GetValueOrDefault("p"));
      Assert.Equal("200", resp?.ResponsesSuccessfulPolicyResponse?.HttpStatusCode);
    }
    {
      var resp = responsesMap?.GetValueOrDefault("BBB");
      Assert.NotNull(resp?.ServerError);
      Assert.Equal("internal_error", resp?.ServerError?.Code);
      Assert.Equal("500", resp?.ServerError?.HttpStatusCode);
      Assert.Equal("object insert conflict", resp?.ServerError?.Message);
    }
    {
      var resp = responsesMap?.GetValueOrDefault("CCC");
      Assert.NotNull(resp);
      Assert.Equivalent(new Dictionary<string, object>() { { "1", 2 }, { "3", 4 } }, resp?.ResponsesSuccessfulPolicyResponse?.Result?.MapOfAny?.GetValueOrDefault("p"));
      Assert.Equal("200", resp?.ResponsesSuccessfulPolicyResponse?.HttpStatusCode);
    }
  }

  [Fact]
  public async Task OpenApiClientBatchPolicyMixedTest()
  {
    // Currently, this API only exists in Enterprise OPA.
    var client = GetEOpaApiClient();

    ExecuteBatchPolicyWithInputRequest req = new ExecuteBatchPolicyWithInputRequest()
    {
      Path = "testmod/condfail",
      RequestBody = new ExecuteBatchPolicyWithInputRequestBody()
      {
        Inputs = new Dictionary<string, Input>() {
          {"AAA", Input.CreateMapOfAny(
            new Dictionary<string, object>() {
              { "x", new List<int> {1, 1, 3} },
              { "y", new List<int> {1, 2, 1} },
            })
          },
          {"BBB", Input.CreateMapOfAny(
            new Dictionary<string, object>() {
              { "x", new List<int> {1, 1, 3} },
              { "y", new List<int> {1, 2, 1} },
            })
          },
          {"CCC", Input.CreateMapOfAny(
            new Dictionary<string, object>() {
              { "x", new List<int> {1, 1, 3} },
              { "y", new List<int> {1, 2, 1} },
            })
          }
        },
      }
    };

    try
    {
      var res = await client.ExecuteBatchPolicyWithInputAsync(req);
      // Assert we get the expected "success" response
      Assert.Equal(500, res.StatusCode);
    }
    catch (Exception ex)
    {
      if (ex is ClientError)
      {
        Console.WriteLine("ClientError:", ex.Message, ex.Data);
      }
      else if (ex is Styra.Opa.OpenApi.Models.Errors.ServerError)
      {
        Console.WriteLine("ServerError:", ex.Message, ex.Data);
      }
      else if (ex is Styra.Opa.OpenApi.Models.Errors.SDKException)
      {
        Console.WriteLine("SDKException: {0}, {1}", ex.Message, ex.StackTrace);
      }
    }
    Assert.Fail();


    // {
    //   var resp = responsesMap?.GetValueOrDefault("AAA");
    //   Assert.NotNull(resp);
    //   Assert.Equivalent(new Dictionary<string, object>() { { "1", 2 }, { "3", 4 } }, resp?.ResponsesSuccessfulPolicyResponse?.Result?.MapOfAny?.GetValueOrDefault("p"));
    //   Assert.Equal("200", resp?.ResponsesSuccessfulPolicyResponse?.HttpStatusCode);
    // }
    // {
    //   var resp = responsesMap?.GetValueOrDefault("BBB");
    //   Assert.NotNull(resp?.ServerError);
    //   Assert.Equal("internal_error", resp?.ServerError?.Code);
    //   Assert.Equal("500", resp?.ServerError?.HttpStatusCode);
    //   Assert.Equal("object insert conflict", resp?.ServerError?.Message);
    // }
    // {
    //   var resp = responsesMap?.GetValueOrDefault("CCC");
    //   Assert.NotNull(resp);
    //   Assert.Equivalent(new Dictionary<string, object>() { { "1", 2 }, { "3", 4 } }, resp?.ResponsesSuccessfulPolicyResponse?.Result?.MapOfAny?.GetValueOrDefault("p"));
    //   Assert.Equal("200", resp?.ResponsesSuccessfulPolicyResponse?.HttpStatusCode);
    // }
  }
}
