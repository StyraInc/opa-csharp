using Styra.Opa.OpenApi;
using Styra.Opa.OpenApi.Models.Requests;
using Styra.Opa.OpenApi.Models.Components;

namespace SmokeTest.Tests;

public class OpenApiTest : IClassFixture<OPAContainerFixture>
{
  public IContainer _container;

  public OpenApiTest(OPAContainerFixture fixture)
  {
    _container = fixture.GetContainer();
  }

  [Fact]
  public async Task OpenApiClientRBACTestcontainersTest()
  {
    // Construct the request URI by specifying the scheme, hostname, assigned random host port, and the endpoint "uuid".
    var requestUri = new UriBuilder(Uri.UriSchemeHttp, _container.Hostname, _container.GetMappedPublicPort(8181)).Uri;

    // Send an HTTP GET request to the specified URI and retrieve the response as a string.
    var client = new OpaApiClient(serverIndex: 0, serverUrl: requestUri.ToString());

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
    var resultMap = res.SuccessfulPolicyEvaluation?.Result?.MapOfAny;

    // Ensure we got back the expected fields from the eval.
    Assert.Equal(true, resultMap?.GetValueOrDefault("allow", false));
    Assert.Equal(true, resultMap?.GetValueOrDefault("user_is_admin", false));
    Assert.Equal(new List<string>(), resultMap?.GetValueOrDefault("user_is_granted"));
  }

  [Fact]
  public async Task OpenApiClientEncodedPathTest()
  {
    // Construct the request URI by specifying the scheme, hostname, assigned random host port, and the endpoint "uuid".
    var requestUri = new UriBuilder(Uri.UriSchemeHttp, _container.Hostname, _container.GetMappedPublicPort(8181)).Uri;

    // Send an HTTP GET request to the specified URI and retrieve the response as a string.
    var client = new OpaApiClient(serverIndex: 0, serverUrl: requestUri.ToString());

    // Exercise the low-level OPA C# SDK.
    ExecutePolicyRequest req = new ExecutePolicyRequest()
    {
      Path = "this/is%2fallowed/pkg",
    };

    var res = await client.ExecutePolicyAsync(req);
    var resultMap = res.SuccessfulPolicyEvaluation?.Result?.MapOfAny;

    Assert.Equal(true, resultMap?.GetValueOrDefault("allow", false));
  }

  [Fact]
  public async Task OpenApiClientEvaluateDefaultTest()
  {
    // Construct the request URI by specifying the scheme, hostname, assigned random host port, and the endpoint "uuid".
    var requestUri = new UriBuilder(Uri.UriSchemeHttp, _container.Hostname, _container.GetMappedPublicPort(8181)).Uri;

    // Send an HTTP GET request to the specified URI and retrieve the response as a string.
    var client = new OpaApiClient(serverIndex: 0, serverUrl: requestUri.ToString());

    // Exercise the low-level OPA C# SDK.
    ExecuteDefaultPolicyWithInputRequest req = new ExecuteDefaultPolicyWithInputRequest()
    {
      Input = Input.CreateMapOfAny(
                    new Dictionary<string, object>() {
                    { "user", "alice" },
                    { "action", "read" },
                    { "object", "id123" },
                    { "type", "dog" },
                    }),
    };

    // Note(philip): To to how the API is generated, we have to fire off
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
}
