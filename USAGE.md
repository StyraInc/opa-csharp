<!-- Start SDK Example Usage [usage] -->
```csharp
using Api.SDK;
using Api.SDK.Models.Requests;
using Api.SDK.Models.Components;

var sdk = new Opa();

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