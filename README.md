# OPA C# SDK

[![License](https://img.shields.io/badge/License-Apache_2.0-blue.svg)](https://opensource.org/licenses/Apache-2.0)
[![NuGet Version](https://img.shields.io/nuget/v/Styra.Opa?style=flat&color=%2324b6e0)](https://www.nuget.org/packages/Styra.Opa/)

> [!IMPORTANT]
> Reference documentation is available at <https://styrainc.github.io/opa-csharp>

You can use the Styra OPA SDK to connect to [Open Policy Agent](https://www.openpolicyagent.org/) and [Enterprise OPA](https://www.styra.com/enterprise-opa/) deployments.

## SDK Installation

### Nuget

```bash
dotnet add package Styra.Opa
```
<!-- No SDK Installation [installation] -->

## SDK Example Usage (high-level)

The following examples assume an OPA server at `http://localhost:8181` equipped with the following Rego policy in `authz.rego`:

```rego
package authz
import rego.v1

default allow := false
allow if input.subject == "alice"
```

and this `data.json`:

```json
{
  "roles": {
    "admin": ["read", "write"]
  }
}
```

### Simple Query

For a simple boolean response with input, use the SDK as follows:

```csharp
using Styra.Opa;

string opaUrl = "http://localhost:8181";
OpaClient opa = new OpaClient(opaUrl);

var input = new Dictionary<string, object>() {
    {"subject", "alice"},
    {"action", "read"},
};

bool allowed = false;

try
{
    allowed = await opa.Check("authz/allow", input);
}
catch (OpaException e)
{
    Console.WriteLine("exception while making request against OPA: " + e);
}

Console.WriteLine("allowed: " + allowed);
```

<details>
  <summary>Result</summary>

```txt
allowed: True
```

</details>

### Simple Query with Output

The `.Evaluate()` method can be used instead of `.Check()` for non-boolean output types:

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using Styra.Opa;

var opaUrl = "http://localhost:8181";
var opa = new OpaClient(opaUrl);

var input = new Dictionary<string, string>() {
    {"subject", "alice"},
    {"action", "read"},
};

var result = new Dictionary<string, List<string>>();

try
{
    result = await opa.Evaluate<Dictionary<string, List<string>>>("roles", input);
}
catch (OpaException e)
{
    Console.WriteLine("exception while making request against OPA: " + e.Message);
}

Console.WriteLine("content of data.roles:");
foreach (var pair in result)
{
    Console.Write("  {0} => [ ", pair.Key);
    foreach (var item in pair.Value)
    {
        Console.Write("{0} ", item);
    }
    Console.WriteLine("]");
}
```

<details>
  <summary>Result</summary>

```txt
content of data.roles:
    admin => [ read write ]
```

</details>

### Default Rule

For evaluating the default rule (configured with your OPA service), use `EvaluateDefault`. `input` is optional, and left out in this example:

```csharp
using Styra.Opa;

string opaUrl = "http://localhost:8181";
OpaClient opa = new OpaClient(opaUrl);

bool allowed = false;

try {
    allowed = await opa.EvaluateDefault<bool();
}
catch (OpaException e) {
    Console.WriteLine("exception while making request against OPA: " + e);
}

Console.WriteLine("allowed: " + allowed);
```

<details>
  <summary>Result</summary>

```txt
allowed: False
```

</details>

### Batched Queries

Enterprise OPA supports executing many queries in a single request with the [Batch API][eopa-batch-api].

   [eopa-batch-api]: /enterprise-opa/reference/api-reference/batch-api

The OPA C# SDK has native support for Enterprise OPA's batch API, with a fallback behavior of sequentially executing single queries if the Batch API is unavailable (such as with open source Open Policy Agent).

```csharp
using Styra.Opa;

string opaUrl = "http://localhost:8181";
OpaClient opa = new OpaClient(opaUrl);

var input = new Dictionary<string, Dictionary<string, object>>() {
    { "AAA", new Dictionary<string, object>() { { "subject", "alice" }, { "action", "read" } } },
    { "BBB", new Dictionary<string, object>() { { "subject", "bob" }, { "action", "write" } } },
    { "CCC", new Dictionary<string, object>() { { "subject", "dave" }, { "action", "read" } } },
    { "DDD", new Dictionary<string, object>() { { "subject", "sybil" }, { "action", "write" } } },
};

OpaBatchResults results = new OpaBatchResults();
OpaBatchErrors errors = new OpaBatchErrors();
try
{
    (results, errors) = await opa.EvaluateBatch("authz/allow", input);
}
catch (OpaException e)
{
    Console.WriteLine("exception while making request against OPA: " + e.Message);
}

Console.WriteLine("Query results, by key:");
foreach (var pair in results)
{
    Console.WriteLine("  {0} => {1}", pair.Key, pair.Value.Result.Boolean);
}

if (errors.Count > 0)
{
    Console.WriteLine("Query errors, by key:");
    foreach (var pair in errors)
    {
        Console.WriteLine("  {0} => {1}", pair.Key, pair.Value);
    }
}
```

<details>
  <summary>Result</summary>

```txt
Query results, by key:
    AAA => True
    BBB => False
    CCC => False
    DDD => False
```

</details>

See the [API Documentation](https://styrainc.github.io/opa-csharp/api/Styra.Opa.OpenApi.Models.Components.Result.html) for reference on the properties and types available from a result.

### Using Custom Classes for Input and Output

Using the OPA C# SDK, it can be more natural to use custom class types as inputs and outputs to a policy, rather than `System.Collections.Dictionary` (or `Collections.List`). Internally, the OPA C# SDK uses [`Newtonsoft.Json`](https://www.newtonsoft.com/json) to serialize and deserialize inputs and outputs JSON to the provided types.

In the example below, note:

- Using an `enum` for an input field
- Hiding the sensitive `UUID` with the `JsonIgnore` property
- Deserializing the query response to a `bool`

```csharp
using System;
using Styra.Opa;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Application
{
    class Program
    {
        public enum ActionType
        {
            invalid,
            create,
            read,
            update,
            delete
        }

        private class CustomRBACObject
        {

            [JsonProperty("user")]
            public string User = "";

            [JsonProperty("action")]
            [JsonConverter(typeof(StringEnumConverter))]
            public ActionType Action = ActionType.invalid;

            [JsonIgnore]
            public string UUID = System.Guid.NewGuid().ToString();

            public CustomRBACObject() { }

            public CustomRBACObject(string user, ActionType action)
            {
                User = user;
                Action = action;
            }
        }

        static async Task<int> Main(string[] args)
        {
            string opaUrl = "http://localhost:8181";
            OpaClient opa = new OpaClient(opaUrl);

            var input = new CustomRBACObject("bob", ActionType.read);
            Console.WriteLine("The JSON that OPA will receive: {{\"input\": {0}}}", JsonConvert.SerializeObject(input));

            bool allowed = false;
            try
            {
                allowed = await opa.Evaluate<bool>("authz/allow", input);
            }
            catch (OpaException e)
            {
                Console.WriteLine("exception while making request against OPA: " + e.Message);
            }

            Console.WriteLine("allowed: " + allowed);
            return 0;
        }
    }
}
```

<details>
  <summary>Result</summary>

```txt
The JSON that OPA will receive: {"input": {"user":"bob","action":"read"}}
allowed: False
```

</details>

### Integrating logging with the OPA C# SDK

The OPA C# SDK uses opt-in, [compile-time source generated logging](https://learn.microsoft.com/en-us/dotnet/core/extensions/logger-message-generator), which can be integrated as a part of the overall logs of a larger application.

Here's a quick example:

```csharp
using Microsoft.Extensions.Logging;
using Styra.Opa;

internal class Program
{
    static async Task<int> Main(string[] args)
    {
        using ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddConsole());
        ILogger<OpaClient> logger = factory.CreateLogger<OpaClient>();

        var opaURL = "http://localhost:8181";
        OpaClient opa = new OpaClient(opaURL, logger);

        logger.LogInformation("Initialized an OPA client for the OPA at: {Description}.", opaURL);

        var allow = await opa.Evaluate<bool>("this/rule/does/not/exist", false);

        return 0;
    }
}
```

<details>
    <summary>Result</summary>

```log
info: Styra.Opa.OpaClient[0]
      Initialized an OPA client for the OPA at: http://localhost:8181.
warn: Styra.Opa.OpaClient[2066302899]
      executing policy at 'this/rule/does/not/exist' succeeded, but OPA did not reply with a result
Unhandled exception. OpaException: executing policy at 'this/rule/does/not/exist' succeeded, but OPA did not reply with a result
    ...
```

</details>

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
- [OPA C# SDK](#opa-c-sdk)
  - [SDK Installation](#sdk-installation)
  - [SDK Example Usage (high-level)](#sdk-example-usage-high-level)
- [OPA OpenAPI SDK (low-level)](#opa-openapi-sdk-low-level)
  - [SDK Example Usage](#sdk-example-usage)
  - [Available Resources and Operations](#available-resources-and-operations)
  - [Server Selection](#server-selection)
  - [Error Handling](#error-handling)
  - [Authentication](#authentication)
  - [Community](#community)

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

- [ExecuteDefaultPolicyWithInput](docs/sdks/opaapiclient/README.md#executedefaultpolicywithinput) - Execute the default decision  given an input
- [ExecutePolicy](docs/sdks/opaapiclient/README.md#executepolicy) - Execute a policy
- [ExecutePolicyWithInput](docs/sdks/opaapiclient/README.md#executepolicywithinput) - Execute a policy given an input
- [ExecuteBatchPolicyWithInput](docs/sdks/opaapiclient/README.md#executebatchpolicywithinput) - Execute a policy given a batch of inputs
- [CompileQueryWithPartialEvaluation](docs/sdks/opaapiclient/README.md#compilequerywithpartialevaluation) - Partially evaluate a query
- [Health](docs/sdks/opaapiclient/README.md#health) - Verify the server is operational

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
