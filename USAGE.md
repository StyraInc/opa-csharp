<!-- Start SDK Example Usage [usage] -->
```csharp
using Styra.OpenApi;
using Styra.OpenApi.Models.Requests;
using Styra.OpenApi.Models.Components;

var sdk = new OpaApiClient();

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