using DotNet.Testcontainers.Containers;
using Docker.DotNet.Models;

namespace SmokeTest.Tests;
public class OPAContainerFixture : IAsyncLifetime
{
    // Note: We disable this warning because we control when/how the constructor
    // will be invoked for this class.
#pragma warning disable CS8618
    private IContainer _container;
#pragma warning restore CS8618 

    public async Task InitializeAsync()
    {
        // Read in the test data files.
        var policy = System.IO.File.ReadAllBytes(Path.Combine("testdata", "policy.rego"));
        var secondPolicy = System.IO.File.ReadAllBytes(Path.Combine("testdata", "weird_name.rego"));
        var data = System.IO.File.ReadAllBytes(Path.Combine("testdata", "data.json"));

        // Create a new instance of a container.
        IContainer container = new ContainerBuilder()
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
        _container = container;
    }
    public async Task DisposeAsync()
    {
        await _container.DisposeAsync();
    }

    // Expose the container for tests
    public IContainer GetContainer() => _container;
}
