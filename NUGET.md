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
        4963.69D
    ),
    pretty: false,
    acceptEncoding: Styra.Opa.OpenApi.Models.Components.GzipAcceptEncoding.Gzip
);

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
            4963.69D
        ),
        pretty: false,
        acceptEncoding: Styra.Opa.OpenApi.Models.Components.GzipAcceptEncoding.Gzip
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

<!-- Start Server Selection [server] -->
## Server Selection

### Override Server URL Per-Client

The default server can also be overridden globally by passing a URL to the `serverUrl: string` optional parameter when initializing the SDK client instance. For example:
```csharp
using Styra.Opa.OpenApi;
using Styra.Opa.OpenApi.Models.Requests;
using Styra.Opa.OpenApi.Models.Components;
using System.Collections.Generic;

var sdk = new OpaApiClient(
    serverUrl: "http://localhost:8181",
    bearerAuth: "<YOUR_BEARER_TOKEN_HERE>"
);

var res = await sdk.ExecuteDefaultPolicyWithInputAsync(
    input: Input.CreateNumber(
        4963.69D
    ),
    pretty: false,
    acceptEncoding: Styra.Opa.OpenApi.Models.Components.GzipAcceptEncoding.Gzip
);

// handle response
```
<!-- End Server Selection [server] -->

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
using Styra.Opa.OpenApi.Models.Requests;
using Styra.Opa.OpenApi.Models.Components;
using System.Collections.Generic;

var sdk = new OpaApiClient(bearerAuth: "<YOUR_BEARER_TOKEN_HERE>");

var res = await sdk.ExecuteDefaultPolicyWithInputAsync(
    input: Input.CreateNumber(
        4963.69D
    ),
    pretty: false,
    acceptEncoding: Styra.Opa.OpenApi.Models.Components.GzipAcceptEncoding.Gzip
);

// handle response
```
<!-- End Authentication [security] -->

<!-- Placeholder for Future Speakeasy SDK Sections -->