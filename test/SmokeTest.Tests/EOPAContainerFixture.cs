using DotNet.Testcontainers.Containers;
using Docker.DotNet.Models;
using System.Runtime.InteropServices;

namespace SmokeTest.Tests;
public class EOPAContainerFixture : IAsyncLifetime
{
    // Note: We disable this warning because we control when/how the constructor
    // will be invoked for this class.
#pragma warning disable CS8618
    private IContainer _container;
#pragma warning restore CS8618 

    public async Task InitializeAsync()
    {
        string[] startupFiles = {
            "testdata/policy.rego",
            "testdata/weird_name.rego",
            "testdata/simple/system.rego",
            "testdata/condfail.rego",
            "testdata/data.json"
        };
        string[] opaCmd = { "run", "--server" };
        string[] startupCommand = new List<string>().Concat(opaCmd).Concat(startupFiles).ToArray();

        // Create a new instance of a container.
        IContainer container = new ContainerBuilder()
          .WithImage("ghcr.io/styrainc/enterprise-opa:1.22.0")
          .WithEnvironment("EOPA_LICENSE_TOKEN", Environment.GetEnvironmentVariable("EOPA_LICENSE_TOKEN"))
          .WithEnvironment("EOPA_LICENSE_KEY", Environment.GetEnvironmentVariable("EOPA_LICENSE_KEY"))
          // Bind port 8181 of the container to a random port on the host.
          .WithPortBinding(8181, true)
          .WithCommand(startupCommand)
          // Map our policy and data files into the container instance.
          .WithResourceMapping(new DirectoryInfo("testdata"), "/testdata/")
          // Wait until the HTTP endpoint of the container is available.
          .WithWaitStrategy(Wait.ForUnixContainer().UntilHttpRequestIsSucceeded(r => r.ForPort(8181).ForPath("/health")))
          //.WithOutputConsumer(Consume.RedirectStdoutAndStderrToConsole()) // DEBUG
          // Build the container configuration.
          .Build();

        // Start the container.
        await container.StartAsync()
          .ConfigureAwait(false);
        // DEBUG:
        // var (stderr, stdout) = await container.GetLogsAsync(default);
        // Console.WriteLine("STDERR: {0}", stderr);
        // Console.WriteLine("STDOUT: {0}", stdout);

        _container = container;
    }
    public async Task DisposeAsync()
    {
        await _container.DisposeAsync();
    }

    // Expose the container for tests
    public IContainer GetContainer() => _container;
}
