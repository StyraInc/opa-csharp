# Opa SDK


## Overview

Enterprise OPA documentation
<https://docs.styra.com/enterprise-opa>
### Available Operations

* [ExecutePolicy](#executepolicy) - Execute a policy
* [ExecutePolicyWithInput](#executepolicywithinput) - Execute a policy given an input
* [Health](#health) - Verify the server is operational

## ExecutePolicy

Execute a policy

### Example Usage

```csharp
using Api;
using Api.Models.Requests;
using Api.Models.Components;

var sdk = new Opa();

ExecutePolicyRequest req = new ExecutePolicyRequest() {
    Path = "app/rbac",
};

var res = await sdk.ExecutePolicyAsync(req);

// handle response
```

### Parameters

| Parameter                                                             | Type                                                                  | Required                                                              | Description                                                           |
| --------------------------------------------------------------------- | --------------------------------------------------------------------- | --------------------------------------------------------------------- | --------------------------------------------------------------------- |
| `request`                                                             | [ExecutePolicyRequest](../../Models/Requests/ExecutePolicyRequest.md) | :heavy_check_mark:                                                    | The request object to use for the request.                            |


### Response

**[ExecutePolicyResponse](../../Models/Requests/ExecutePolicyResponse.md)**


## ExecutePolicyWithInput

Execute a policy given an input

### Example Usage

```csharp
using Api;
using Api.Models.Requests;
using Api.Models.Components;

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

### Parameters

| Parameter                                                                               | Type                                                                                    | Required                                                                                | Description                                                                             |
| --------------------------------------------------------------------------------------- | --------------------------------------------------------------------------------------- | --------------------------------------------------------------------------------------- | --------------------------------------------------------------------------------------- |
| `request`                                                                               | [ExecutePolicyWithInputRequest](../../Models/Requests/ExecutePolicyWithInputRequest.md) | :heavy_check_mark:                                                                      | The request object to use for the request.                                              |


### Response

**[ExecutePolicyWithInputResponse](../../Models/Requests/ExecutePolicyWithInputResponse.md)**


## Health

The health API endpoint executes a simple built-in policy query to verify that the server is operational. Optionally it can account for bundle activation as well (useful for “ready” checks at startup).

### Example Usage

```csharp
using Api;
using Api.Models.Requests;
using System.Collections.Generic;

var sdk = new Opa();

var res = await sdk.HealthAsync(
    bundles: false,
    plugins: false,
    excludePlugin: new List<string>() {
    "<value>",
});

// handle response
```

### Parameters

| Parameter                                                                                                                                                                                                                                                                     | Type                                                                                                                                                                                                                                                                          | Required                                                                                                                                                                                                                                                                      | Description                                                                                                                                                                                                                                                                   |
| ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `Bundles`                                                                                                                                                                                                                                                                     | *bool*                                                                                                                                                                                                                                                                        | :heavy_minus_sign:                                                                                                                                                                                                                                                            | Boolean parameter to account for bundle activation status in response. This includes any discovery bundles or bundles defined in the loaded discovery configuration.                                                                                                          |
| `Plugins`                                                                                                                                                                                                                                                                     | *bool*                                                                                                                                                                                                                                                                        | :heavy_minus_sign:                                                                                                                                                                                                                                                            | Boolean parameter to account for plugin status in response.                                                                                                                                                                                                                   |
| `ExcludePlugin`                                                                                                                                                                                                                                                               | List<*string*>                                                                                                                                                                                                                                                                | :heavy_minus_sign:                                                                                                                                                                                                                                                            | String parameter to exclude a plugin from status checks. Can be added multiple times. Does nothing if plugins is not true. This parameter is useful for special use cases where a plugin depends on the server being fully initialized before it can fully initialize itself. |


### Response

**[HealthResponse](../../Models/Requests/HealthResponse.md)**

