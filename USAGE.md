<!-- Start SDK Example Usage [usage] -->
```csharp
using Openapi;
using Openapi.Models.Requests;
using System.Collections.Generic;
using Openapi.Models.Components;

var sdk = new Opa();

ExecutePolicyWithInputRequest req = new ExecutePolicyWithInputRequest() {
    Path = "<value>",
    RequestBody = new ExecutePolicyWithInputRequestBody() {
        Input = new Dictionary<string, object>() {
            { "key", "<value>" },
        },
    },
};

var res = await sdk.ExecutePolicyWithInputAsync(req);

// handle response
```
<!-- End SDK Example Usage [usage] -->