﻿
namespace SmokeTest.Tests;

public class OPAContainerFixture : IAsyncLifetime
{
    // Note: We disable this warning because we control when/how the constructor
    // will be invoked for this class.
#pragma warning disable CS8618
    private IContainer _container;
#pragma warning restore CS8618

    public async ValueTask InitializeAsync()
    {
        string[] startupFiles = {
            "testdata/policy.rego",
            "testdata/weird_name.rego",
            "testdata/simple/system.rego",
            "testdata/condfail.rego",
            "testdata/data.json"
        };
        string[] opaCmd = { "run", "--server", "--addr=0.0.0.0:8181" };
        var startupCommand = new List<string>().Concat(opaCmd).Concat(startupFiles).ToArray();

        // Create a new instance of a container.
        var container = new ContainerBuilder()
          .WithImage("openpolicyagent/opa:latest")
          // Bind port 8181 of the container to a random port on the host.
          .WithPortBinding(8181, true)
          .WithCommand(startupCommand)
          // Debugging aid, helpful if the Rego files have syntax errors.
          .WithOutputConsumer(Consume.RedirectStdoutAndStderrToConsole())
          // Map our policy and data files into the container instance.
          .WithResourceMapping(new DirectoryInfo("testdata"), "/testdata/")
          // Wait until the HTTP endpoint of the container is available.
          .WithWaitStrategy(Wait.ForUnixContainer().UntilHttpRequestIsSucceeded(r => r.ForPort(8181).ForPath("/health")))
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
    public async ValueTask DisposeAsync()
    {
        await _container.DisposeAsync();
    }

    // Expose the container for tests
    public IContainer GetContainer() => _container;
}
