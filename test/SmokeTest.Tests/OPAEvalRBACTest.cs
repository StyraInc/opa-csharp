using Styra.OpenApi;
using Styra.OpenApi.Models.Requests;
using Styra.OpenApi.Models.Components;

namespace SmokeTest.Tests;

public class OPAEvalRBACTest
{
  [Fact]
  public async Task HelloTestContainers()
  {
    // Read in the test data files.
    var policy = System.IO.File.ReadAllBytes(Path.Combine("testdata", "policy.rego"));
    var secondPolicy = System.IO.File.ReadAllBytes(Path.Combine("testdata", "weird_name.rego"));
    var data = System.IO.File.ReadAllBytes(Path.Combine("testdata", "data.json"));

    // Create a new instance of a container.
    var container = new ContainerBuilder()
      .WithImage("openpolicyagent/opa:latest")
      // Bind port 8181 of the container to a random port on the host.
      .WithPortBinding(8181, true)
      .WithCommand("run", "--server", "policy.rego", "weird_name.rego", "data.json")
      // Map our policy and data files into the container instance.
      .WithResourceMapping(policy, "policy.rego")
      .WithResourceMapping(secondPolicy, "weird_name.rego")
      .WithResourceMapping(data, "data.json")
      // Wait until the HTTP endpoint of the container is available.
      .WithWaitStrategy(Wait.ForUnixContainer().UntilHttpRequestIsSucceeded(r => r.ForPort(8181).ForPath("/health")))
      // Build the container configuration.
      .Build();

    // Start the container.
    await container.StartAsync()
        .ConfigureAwait(false);

    // Create a new instance of HttpClient to send HTTP requests.
    var httpClient = new HttpClient();

    // Construct the request URI by specifying the scheme, hostname, assigned random host port, and the endpoint "uuid".
    var requestUri = new UriBuilder(Uri.UriSchemeHttp, container.Hostname, container.GetMappedPublicPort(8181)).Uri;

    // Send an HTTP GET request to the specified URI and retrieve the response as a string.
    var client = new OpaApiClient(serverIndex: 0, serverUrl: requestUri.ToString());

    // Exercise the low-level OPA C# SDK.
    ExecutePolicyWithInputRequest req = new ExecutePolicyWithInputRequest()
    {
      Path = "app/rbac",
      RequestBody = new ExecutePolicyWithInputRequestBody()
      {
        Input = Input.CreateMapOfany(
                    new Dictionary<string, object>() {
                    { "user", "alice" },
                    { "action", "read" },
                    { "object", "id123" },
                    { "type", "dog" },
                    }),
      },
    };

    var res = Task.Run(() => client.ExecutePolicyWithInputAsync(req)).GetAwaiter().GetResult();
    var resultMap = res.SuccessfulPolicyEvaluation?.Result?.MapOfany;

    // Ensure we got back the expected fields from the eval.
    Assert.Equal(true, resultMap?.GetValueOrDefault("allow", false));
    Assert.Equal(true, resultMap?.GetValueOrDefault("user_is_admin", false));
    Assert.Equal(new List<string>(), resultMap?.GetValueOrDefault("user_is_granted"));

    // Exercise the low-level OPA C# SDK.
    ExecutePolicyRequest req2 = new ExecutePolicyRequest()
    {
      Path = "this/is%2fallowed/pkg",
    };

    var res2 = Task.Run(() => client.ExecutePolicyAsync(req2)).GetAwaiter().GetResult();
    var resultMap2 = res2.SuccessfulPolicyEvaluation?.Result?.MapOfany;
    Assert.Equal(true, resultMap2?.GetValueOrDefault("allow", false));
  }
}
