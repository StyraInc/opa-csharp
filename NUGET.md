# Styra.Opa.OpenApi


<!-- Start SDK Example Usage [usage] -->
## SDK Example Usage

### Example 1

```csharp
using Styra.Opa.OpenApi;
using Styra.Opa.OpenApi.Models.Requests;
using Styra.Opa.OpenApi.Models.Components;
using System.Collections.Generic;

var sdk = new OpaApiClient(bearerAuth: "<YOUR_BEARER_TOKEN_HERE>");

var res = await sdk.ExecuteDefaultPolicyWithInputAsync(
    input: Input.CreateNumber(
8203.11D
),
    pretty: false,
    acceptEncoding: Styra.Opa.OpenApi.Models.Components.GzipAcceptEncoding.Gzip);

// handle response
```

### Example 2

```csharp
using Styra.Opa.OpenApi;
using Styra.Opa.OpenApi.Models.Requests;
using Styra.Opa.OpenApi.Models.Components;
using System.Collections.Generic;

var sdk = new OpaApiClient(bearerAuth: "<YOUR_BEARER_TOKEN_HERE>");

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
using Styra.Opa.OpenApi.Models.Requests;
using Styra.Opa.OpenApi.Models.Components;
using System.Collections.Generic;

var sdk = new OpaApiClient(bearerAuth: "<YOUR_BEARER_TOKEN_HERE>");

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

<!-- Start Error Handling [errors] -->
## Error Handling

Handling errors in this SDK should largely match your expectations.  All operations return a response object or thow an exception.  If Error objects are specified in your OpenAPI Spec, the SDK will raise the appropriate type.

| Error Object                                 | Status Code                                  | Content Type                                 |
| -------------------------------------------- | -------------------------------------------- | -------------------------------------------- |
| Styra.Opa.OpenApi.Models.Errors.ClientError  | 400,404                                      | application/json                             |
| Styra.Opa.OpenApi.Models.Errors.ServerError  | 500                                          | application/json                             |
| Styra.Opa.OpenApi.Models.Errors.SDKException | 4xx-5xx                                      | */*                                          |

### Example

```csharp
using Styra.Opa.OpenApi;
using Styra.Opa.OpenApi.Models.Requests;
using Styra.Opa.OpenApi.Models.Components;
using System.Collections.Generic;
using System;
using Styra.Opa.OpenApi.Models.Errors;

var sdk = new OpaApiClient(bearerAuth: "<YOUR_BEARER_TOKEN_HERE>");

try
{
    var res = await sdk.ExecuteDefaultPolicyWithInputAsync(
    input: Input.CreateNumber(
8203.11D
),
    pretty: false,
    acceptEncoding: Styra.Opa.OpenApi.Models.Components.GzipAcceptEncoding.Gzip);
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

<!-- Start Authentication [security] -->
## Authentication

### Per-Client Security Schemes

This SDK supports the following security scheme globally:

| Name         | Type         | Scheme       |
| ------------ | ------------ | ------------ |
| `BearerAuth` | http         | HTTP Bearer  |

To authenticate with the API the `BearerAuth` parameter must be set when initializing the SDK client instance. For example:
```csharp
using Styra.Opa.OpenApi;
using Styra.Opa.OpenApi.Models.Requests;
using Styra.Opa.OpenApi.Models.Components;
using System.Collections.Generic;

var sdk = new OpaApiClient(bearerAuth: "<YOUR_BEARER_TOKEN_HERE>");

var res = await sdk.ExecuteDefaultPolicyWithInputAsync(
    input: Input.CreateNumber(
8203.11D
),
    pretty: false,
    acceptEncoding: Styra.Opa.OpenApi.Models.Components.GzipAcceptEncoding.Gzip);

// handle response
```
<!-- End Authentication [security] -->

<!-- Placeholder for Future Speakeasy SDK Sections -->