<!-- Start SDK Example Usage [usage] -->
```csharp
using Api;
using Api.Models.Requests;
using Api.Models.Components;

var sdk = new Opa();

ExecutePolicyWithInputRequest req = new ExecutePolicyWithInputRequest() {
    Path = "<value>",
    RequestBody = new ExecutePolicyWithInputRequestBody() {
        Input = "<value>",
    },
};

var res = await sdk.ExecutePolicyWithInputAsync(req);

// handle response
```
<!-- End SDK Example Usage [usage] -->