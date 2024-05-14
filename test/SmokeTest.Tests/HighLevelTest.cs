using Styra.Opa;

namespace SmokeTest.Tests;

public class HighLevelTest : IClassFixture<OPAContainerFixture>
{
  public IContainer _container;

  public HighLevelTest(OPAContainerFixture fixture)
  {
    _container = fixture.GetContainer();
  }

  private OpaClient GetOpaClient()
  {
    // Construct the request URI by specifying the scheme, hostname, assigned random host port, and the endpoint "uuid".
    var requestUri = new UriBuilder(Uri.UriSchemeHttp, _container.Hostname, _container.GetMappedPublicPort(8181)).Uri;

    // Send an HTTP GET request to the specified URI and retrieve the response as a string.
    return new OpaClient(serverUrl: requestUri.ToString());
  }

  [Fact]
  public async Task OpaClientRBACTestcontainersTest()
  {
    var client = GetOpaClient();

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