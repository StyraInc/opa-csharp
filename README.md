# OPA C# SDK

[![License](https://img.shields.io/badge/License-Apache_2.0-blue.svg)](https://opensource.org/licenses/Apache-2.0)
[![NuGet Version](https://img.shields.io/nuget/v/Styra.Opa?style=flat&color=%2324b6e0)](https://www.nuget.org/packages/Styra.Opa/)

> [!IMPORTANT]
> The documentation for this SDK lives at https://docs.styra.com/sdk, with reference documentation available at https://styrainc.github.io/opa-csharp

You can use the Styra OPA SDK to connect to [Open Policy Agent](https://www.openpolicyagent.org/) and [Enterprise OPA](https://www.styra.com/enterprise-opa/) deployments.

## SDK Installation

### Nuget

```bash
dotnet add package Styra.Opa
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
// (local variable) violations List<string>?
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

double maxLimit
try {
    maxLimit = opa.evaluate<double?>(path, "example");
}
catch (OpaException) {
    maxLimit = 0.0f;
}
```

Nullable types are also allowed for output types, and if an error occurs during evaluation, a null result will be returned to the caller.

```csharp
string path = "authz/accounts/max_limit";

double? maxLimit = opa.evaluate<double?>(path, "example");
```

If the selected return type `<T>` is possible to deserialize from the returned JSON, `evaluate<T>` will attempt to populate the variable with the value(s) present.

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

AuthzStatus status;
try {
    status = opa.evaluate<AuthzStatus>(path, input);
}
catch (OpaException) {
    status = new AuthzStatus(false);
}
```

> [!NOTE]
> For low-level SDK usage, see the sections below.

---

# OPA OpenAPI SDK (low-level)

<!-- Start Summary [summary] -->
## Summary

For more information about the API: [Enterprise OPA documentation](https://docs.styra.com/enterprise-opa)
<!-- End Summary [summary] -->

<!-- Start Table of Contents [toc] -->
## Table of Contents
<!-- $toc-max-depth=2 -->
* [OPA C# SDK](#opa-c-sdk)
  * [SDK Installation](#sdk-installation)
  * [SDK Example Usage (high-level)](#sdk-example-usage-high-level)
* [OPA OpenAPI SDK (low-level)](#opa-openapi-sdk-low-level)
  * [SDK Example Usage](#sdk-example-usage)
  * [Available Resources and Operations](#available-resources-and-operations)
  * [Server Selection](#server-selection)
  * [Error Handling](#error-handling)
  * [Authentication](#authentication)
  * [Community](#community)

<!-- End Table of Contents [toc] -->

<!-- Start SDK Example Usage [usage] -->
## SDK Example Usage

### Example 1

```csharp
using Styra.Opa.OpenApi;
using Styra.Opa.OpenApi.Models.Components;

var sdk = new OpaApiClient();

var res = await sdk.ExecuteDefaultPolicyWithInputAsync(
    input: Input.CreateNumber(
        4963.69D
    ),
    pretty: false,
    acceptEncoding: GzipAcceptEncoding.Gzip
);

// handle response
```

### Example 2

```csharp
using Styra.Opa.OpenApi;
using Styra.Opa.OpenApi.Models.Requests;

var sdk = new OpaApiClient();

ExecutePolicyWithInputRequest req = new ExecutePolicyWithInputRequest() {
    Path = "app/rbac",
    RequestBody = new ExecutePolicyWithInputRequestBody() {
        Input = Input.CreateBoolean(
            false
        ),
    },
};

var res = await sdk.ExecutePolicyWithInputAsync(req);

// handle response
```

### Example 3

```csharp
using Styra.Opa.OpenApi;
using Styra.Opa.OpenApi.Models.Components;
using Styra.Opa.OpenApi.Models.Requests;
using System.Collections.Generic;

var sdk = new OpaApiClient();

ExecuteBatchPolicyWithInputRequest req = new ExecuteBatchPolicyWithInputRequest() {
    Path = "app/rbac",
    RequestBody = new ExecuteBatchPolicyWithInputRequestBody() {
        Inputs = new Dictionary<string, Input>() {
            { "key", Input.CreateStr(
                "<value>"
            ) },
        },
    },
};

var res = await sdk.ExecuteBatchPolicyWithInputAsync(req);

// handle response
```
<!-- End SDK Example Usage [usage] -->

<!-- Start Available Resources and Operations [operations] -->
## Available Resources and Operations

<details open>
<summary>Available methods</summary>

### [OpaApiClient SDK](docs/sdks/opaapiclient/README.md)

* [ExecuteDefaultPolicyWithInput](docs/sdks/opaapiclient/README.md#executedefaultpolicywithinput) - Execute the default decision  given an input
* [ExecutePolicy](docs/sdks/opaapiclient/README.md#executepolicy) - Execute a policy
* [ExecutePolicyWithInput](docs/sdks/opaapiclient/README.md#executepolicywithinput) - Execute a policy given an input
* [ExecuteBatchPolicyWithInput](docs/sdks/opaapiclient/README.md#executebatchpolicywithinput) - Execute a policy given a batch of inputs
* [CompileQueryWithPartialEvaluation](docs/sdks/opaapiclient/README.md#compilequerywithpartialevaluation) - Partially evaluate a query
* [Health](docs/sdks/opaapiclient/README.md#health) - Verify the server is operational

</details>
<!-- End Available Resources and Operations [operations] -->

<!-- Start Server Selection [server] -->
## Server Selection

### Override Server URL Per-Client

The default server can be overridden globally by passing a URL to the `serverUrl: string` optional parameter when initializing the SDK client instance. For example:
```csharp
using Styra.Opa.OpenApi;
using Styra.Opa.OpenApi.Models.Components;

var sdk = new OpaApiClient(serverUrl: "http://localhost:8181");

var res = await sdk.ExecuteDefaultPolicyWithInputAsync(
    input: Input.CreateNumber(
        4963.69D
    ),
    pretty: false,
    acceptEncoding: GzipAcceptEncoding.Gzip
);

// handle response
```
<!-- End Server Selection [server] -->

<!-- Start Error Handling [errors] -->
## Error Handling

Handling errors in this SDK should largely match your expectations. All operations return a response object or throw an exception.

By default, an API error will raise a `Styra.Opa.OpenApi.Models.Errors.SDKException` exception, which has the following properties:

| Property      | Type                  | Description           |
|---------------|-----------------------|-----------------------|
| `Message`     | *string*              | The error message     |
| `StatusCode`  | *int*                 | The HTTP status code  |
| `RawResponse` | *HttpResponseMessage* | The raw HTTP response |
| `Body`        | *string*              | The response content  |

When custom error responses are specified for an operation, the SDK may also throw their associated exceptions. You can refer to respective *Errors* tables in SDK docs for more details on possible exception types for each operation. For example, the `ExecuteDefaultPolicyWithInputAsync` method throws the following exceptions:

| Error Type                                   | Status Code | Content Type     |
| -------------------------------------------- | ----------- | ---------------- |
| Styra.Opa.OpenApi.Models.Errors.ClientError  | 400, 404    | application/json |
| Styra.Opa.OpenApi.Models.Errors.ServerError  | 500         | application/json |
| Styra.Opa.OpenApi.Models.Errors.SDKException | 4XX, 5XX    | \*/\*            |

### Example

```csharp
using Styra.Opa.OpenApi;
using Styra.Opa.OpenApi.Models.Components;
using Styra.Opa.OpenApi.Models.Errors;

var sdk = new OpaApiClient();

try
{
    var res = await sdk.ExecuteDefaultPolicyWithInputAsync(
        input: Input.CreateNumber(
            4963.69D
        ),
        pretty: false,
        acceptEncoding: GzipAcceptEncoding.Gzip
    );

    // handle response
}
catch (Exception ex)
{
    if (ex is ClientError)
    {
        // Handle exception data
        throw;
    }
    else if (ex is Models.Errors.ServerError)
    {
        // Handle exception data
        throw;
    }
    else if (ex is Styra.Opa.OpenApi.Models.Errors.SDKException)
    {
        // Handle default exception
        throw;
    }
}
```
<!-- End Error Handling [errors] -->

<!-- Start Authentication [security] -->
## Authentication

### Per-Client Security Schemes

This SDK supports the following security scheme globally:

| Name         | Type | Scheme      |
| ------------ | ---- | ----------- |
| `BearerAuth` | http | HTTP Bearer |

To authenticate with the API the `BearerAuth` parameter must be set when initializing the SDK client instance. For example:
```csharp
using Styra.Opa.OpenApi;
using Styra.Opa.OpenApi.Models.Components;

var sdk = new OpaApiClient(bearerAuth: "<YOUR_BEARER_TOKEN_HERE>");

var res = await sdk.ExecuteDefaultPolicyWithInputAsync(
    input: Input.CreateNumber(
        4963.69D
    ),
    pretty: false,
    acceptEncoding: GzipAcceptEncoding.Gzip
);

// handle response
```
<!-- End Authentication [security] -->

<!-- Placeholder for Future Speakeasy SDK Sections -->

## Community

For questions, discussions and announcements related to Styra products, services and open source projects, please join
the Styra community on [Slack](https://communityinviter.com/apps/styracommunity/signup)!
