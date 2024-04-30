# OPA C# SDK

[![License](https://img.shields.io/badge/License-Apache_2.0-blue.svg)](https://opensource.org/licenses/Apache-2.0)
[![NuGet Version](https://img.shields.io/nuget/v/Styra.Opa?style=flat&color=%2324b6e0)](https://www.nuget.org/packages/Styra.Opa/)

> [!IMPORTANT]
> The documentation for this SDK lives at https://docs.styra.com/sdk, with reference documentation available at https://styrainc.github.io/opa-csharp

You can use the Styra OPA SDK to connect to [Open Policy Agent](https://www.openpolicyagent.org/) and [Enterprise OPA](https://www.styra.com/enterprise-opa/) deployments.

## SDK Installation

### Nuget

```bash
dotnet add reference Styra.Opa
```
<!-- No SDK Installation [installation] -->

## SDK Example Usage (high-level)

All the code examples that follow assume that the high-level SDK module has been imported, and that an `OpaClient` instance was created:

```csharp
using Styra.Opa;


private string serverURL = "http://opa-host:8181";
private string path = "authz/allow";
private OpaClient opa;

opa = new OPAClient(serverURL);

var input = new Dictionary<string, object>() {
    { "user", "alice" },
    { "action", "read" },
    {"resource", "/finance/reports/fy2038_budget.csv"},
};

// (local variable) bool allowed
var allowed = await opa.check("authz/allow", input);
var violations = await opa.evaluate<List<string>>("authz/violations", input);

// Normal true/false cases...
if (allowed) {
    // ...
} else {
    Console.WriteLine("Violations: " + violations);
}
```

### Input types

The `check` and `evaluate` methods are overloaded for most standard JSON types, which include the following variants for the `input` parameter:

| C# type | JSON equivalent type |
| ------- | -------------------- |
| `bool` | Boolean |
| `double` | Number |
| `string` | String |
| `List<object>` | Array |
| `Dictionary<string, object>` | Object |

### Result Types

#### `OpaClient.check`
For the `check` method, the output type is always `bool`.

#### `OpaClient.evaluate<T>`
For the `evaluate` method, the output type is configurable using generics, as shown in the example below.

```csharp
string path = "authz/accounts/max_limit";

double maxLimit = opa.evaluate<double>(path, "example");
```

<!--If the selected return type `<T>` is possible to deserialize from the returned JSON, `query<T>` will attempt to populate the variable with the value(s) present.

```csharp
public struct AuthzStatus
{
    public AuthzStatus(bool allowed)
    {
        Allowed = allowed;
    }

    public double Allowed { get; }

    public override string ToString() => $"Application authorized: {Allowed}";
}

var input = new Dictionary<string, object>() {
    { "user", "alice" },
    { "action", "read" },
};

// (local variable) AuthzStatus status
var status =opa.evaluate<AuthzStatus>(path, input);
```-->

> [!NOTE]
> For low-level SDK usage, see the sections below.

---

# OPA OpenAPI SDK (low-level)
<!-- Start SDK Example Usage [usage] -->
## SDK Example Usage

### Example

```csharp
using Styra.OpenApi;
using Styra.OpenApi.Models.Requests;
using Styra.OpenApi.Models.Components;

var sdk = new OpaApiClient();

ExecutePolicyWithInputRequest req = new ExecutePolicyWithInputRequest() {
    Path = "app/rbac",
    RequestBody = new ExecutePolicyWithInputRequestBody() {
        Input = Input.CreateInputMapOfany(
                new Dictionary<string, object>() {
                    { "user", "alice" },
                    { "action", "read" },
                    { "object", "id123" },
                    { "type", "dog" },
                },
        ),
    },
};

var res = await sdk.ExecutePolicyWithInputAsync(req);

// handle response
```
<!-- End SDK Example Usage [usage] -->

<!-- Start Available Resources and Operations [operations] -->
## Available Resources and Operations

### [OpaApiClient SDK](docs/sdks/opaapiclient/README.md)

* [ExecutePolicy](docs/sdks/opaapiclient/README.md#executepolicy) - Execute a policy
* [ExecutePolicyWithInput](docs/sdks/opaapiclient/README.md#executepolicywithinput) - Execute a policy given an input
* [Health](docs/sdks/opaapiclient/README.md#health) - Verify the server is operational
<!-- End Available Resources and Operations [operations] -->

<!-- Start Server Selection [server] -->
## Server Selection

### Select Server by Index

You can override the default server globally by passing a server index to the `serverIndex: number` optional parameter when initializing the SDK client instance. The selected server will then be used as the default on the operations that use it. This table lists the indexes associated with the available servers:

| # | Server | Variables |
| - | ------ | --------- |
| 0 | `http://localhost:8181` | None |




### Override Server URL Per-Client

The default server can also be overridden globally by passing a URL to the `serverUrl: str` optional parameter when initializing the SDK client instance. For example:
<!-- End Server Selection [server] -->

<!-- Start Error Handling [errors] -->
## Error Handling

Handling errors in this SDK should largely match your expectations.  All operations return a response object or thow an exception.  If Error objects are specified in your OpenAPI Spec, the SDK will raise the appropriate type.

| Error Object                             | Status Code                              | Content Type                             |
| ---------------------------------------- | ---------------------------------------- | ---------------------------------------- |
| Styra.OpenApi.Models.Errors.ClientError  | 400                                      | application/json                         |
| Styra.OpenApi.Models.Errors.ServerError  | 500                                      | application/json                         |
| Styra.OpenApi.Models.Errors.SDKException | 4xx-5xx                                  | */*                                      |

### Example

```csharp
using Styra.OpenApi;
using System;
using Styra.OpenApi.Models.Errors;
using Styra.OpenApi.Models.Requests;
using Styra.OpenApi.Models.Components;

var sdk = new OpaApiClient();

ExecutePolicyRequest req = new ExecutePolicyRequest() {
    Path = "app/rbac",
};

try
{
    var res = await sdk.ExecutePolicyAsync(req);
    // handle response
}
catch (Exception ex)
{
    if (ex is ClientError)
    {
        // handle exception
    }
    else if (ex is ServerError)
    {
        // handle exception
    }
    else if (ex is Styra.OpenApi.Models.Errors.SDKException)
    {
        // handle exception
    }
}

```
<!-- End Error Handling [errors] -->

<!-- Placeholder for Future Speakeasy SDK Sections -->

# Development

## Maturity

This SDK is in beta, and there may be breaking changes between versions without a major version update. Therefore, we recommend pinning usage
to a specific package version. This way, you can install the same version each time without breaking changes unless you are intentionally
looking for the latest version.
