# OPA C# SDK

[![License](https://img.shields.io/badge/License-Apache_2.0-blue.svg)](https://opensource.org/licenses/Apache-2.0)
[![NuGet Version](https://img.shields.io/nuget/v/Styra?style=flat&color=%2324b6e0)](https://www.nuget.org/packages/Styra/)


<!-- Start SDK Installation [installation] -->
## SDK Installation

### Nuget

```bash
dotnet add package Styra.Opa.OpenApi
```
<!-- End SDK Installation [installation] -->

## SDK Example Usage (high-level)

All the code examples that follow assume that the high-level SDK module has been imported, and that an `OpaClient` instance was created:

```csharp
using Styra;

public class MyExample {
    private string serverURL = "http://opa-host:8181";
    private string path = "authz/allow";
    private OpaClient opa;

    public MyExample() {
        opa = new OPAClient(serverURL);
    }

    // ...
}
```

### Simple query

For a simple boolean response with a dictionary input, use the SDK as follows:

```csharp
var input = new Dictionary<string, object>() {
    { "user", "alice" },
    { "action", "read" },
};

// (local variable) bool? allowed
var allowed = opa.check(input, path);

// Logic for the `undefined` case ...
if (allowed == null) {
    // ...
}
// Normal true/false cases...
if (allowed) {
    // ...
}
```

In the above example, a `null` value for `allowed` indicates the `/authz/allow` endpoint returned `undefined`.
A non-null result can be used as a normal true/false value.

<details><summary>HTTP Request</summary>

```http
POST /v1/data/authz/allow
Content-Type: application/json

{ "input": { "user": "alice", "action": "read" } }
```

</details>

### Input types

The `check` and `query` methods are overloaded for most standard JSON types, which include the following variants for the `input` parameter:

| C# type | JSON equivalent type |
| ------- | -------------------- |
| `bool` | Boolean |
| `double` | Number |
| `string` | String |
| `List<object>` | Array |
| `Dictionary<string, object>` | Object |

### Result Types

#### `OpaClient.check`
For the `check` method, the output type is always `bool?`, allowing API users to disambiguate `undefined` from normal `true`/`false` results.

#### `OpaClient.query<T>`
For the `query` method, the output type is configurable using generics, as shown in the example below.

```csharp
string path = "authz/accounts/max_limit";

double maxLimit = opa.query<double>("example", path);
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
var status =opa.query<AuthzStatus>(input, path);
```-->

> [!NOTE]
> For low-level SDK usage, see the sections below.

---

<!-- Start SDK Example Usage [usage] -->
## SDK Example Usage

### Example

```csharp
using Styra.Opa.OpenApi;
using Styra.Opa.OpenApi.Models.Requests;
using Styra.Opa.OpenApi.Models.Components;

var sdk = new OpaApiClient();

ExecutePolicyWithInputRequest req = new ExecutePolicyWithInputRequest() {
    Path = "app/rbac",
    RequestBody = new ExecutePolicyWithInputRequestBody() {
        Input = Input.CreateInputBoolean(
        false,
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

| Error Object                                 | Status Code                                  | Content Type                                 |
| -------------------------------------------- | -------------------------------------------- | -------------------------------------------- |
| Styra.Opa.OpenApi.Models.Errors.ClientError  | 400                                          | application/json                             |
| Styra.Opa.OpenApi.Models.Errors.ServerError  | 500                                          | application/json                             |
| Styra.Opa.OpenApi.Models.Errors.SDKException | 4xx-5xx                                      | */*                                          |

### Example

```csharp
using Styra.Opa.OpenApi;
using System;
using Styra.Opa.OpenApi.Models.Errors;
using Styra.Opa.OpenApi.Models.Requests;
using Styra.Opa.OpenApi.Models.Components;

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
    else if (ex is Styra.Opa.OpenApi.Models.Errors.SDKException)
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
