<!-- Start SDK Example Usage [usage] -->
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