using Styra;

namespace SmokeTest.Tests;

public class HighLevelTest
{
  [Fact]
  public async Task OpaClientRBACTestcontainersTest()
  {
    // Read in the test data files.
    var policy = System.IO.File.ReadAllBytes(Path.Combine("testdata", "policy.rego"));
    var data = System.IO.File.ReadAllBytes(Path.Combine("testdata", "data.json"));

    // Create a new instance of a container.
    var container = new ContainerBuilder()
      .WithImage("openpolicyagent/opa:latest")
      // Bind port 8181 of the container to a random port on the host.
      .WithPortBinding(8181, true)
      .WithCommand("run", "--server", "policy.rego", "data.json")
      // Map our policy and data files into the container instance.
      .WithResourceMapping(policy, "policy.rego")
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
    var client = new OpaClient(serverUrl: requestUri.ToString());


    // Exercise the high-level OPA C# SDK.
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
}