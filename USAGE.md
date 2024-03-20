<!-- Start SDK Example Usage [usage] -->
```csharp
using Api;
using Api.Models.Requests;
using Api.Models.Components;

var sdk = new Opa();

ExecutePolicyWithInputRequest req = new ExecutePolicyWithInputRequest() {
    Path = "app/rbac",
    RequestBody = new ExecutePolicyWithInputRequestBody() {
        Input = Components.CreateInputMapOfany(
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