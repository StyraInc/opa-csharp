# Development Notes for the OPA C# SDK

## Project structure

Currently, the project is split carefully between two Solution files:
 - `Styra.sln`
 - `test/test.sln`

This is intentional, because it is currently the most convenient way to prevent development/testing dependencies from accidentally being included into our NuGet package builds.
This is caused by [NuGet not supporting the "development dependency" metadata feature](https://github.com/NuGet/Home/issues/4125) for referenced packages, and [requires hacky workarounds](https://github.com/NuGet/Home/wiki/DevelopmentDependency-support-for-PackageReference) for those who want to emulate the desired functionality.
In our case, it was simpler to just cleave the project in half; one side for testing, one side for development of the SDK itself.


## Project Automation

Currently, we have relied on Speakeasy's automation for SDK generation and SDK publishing workflows.

We have a few custom workflows for DocFX publishing, and for Pull Request checks.

### `docfx-publish`

This workflow patches the README, and then follows the recommended DocFX Github Pages publishing workflow setup.

### `pull-request`

This workflow builds and runs tests on the PR branch, ensuring that we don't commit non-working code to `main`.

It also includes a `version-update` job, which automatically bumps the NuGet version for the project, and adds the commit to the PR.
Because Github does not allow Actions-generated PRs to generate more Actions jobs, a human-generated action is then required to re-trigger PR checks.
Currently, the job provides a helpful informational message in the PR to instruct the humans there about what to do after it adds the new commit.


## Updating Speakeasy/DocFX method links in the README

Speakeasy's automated docs links in the README are replaced during DocFX builds by `scripts/patch-readme.sh`, a script that search/replaces each of the method URLs with their equivalent DocFX method link.

Whenever the API surface changes, these hard-coded URLs may need to be updated in the script.
To do this, the easiest way is to manually run the local DocFX build, and figure out the links from the local build of the docs site.

To get the local docs site up and running, try the following invocation:

    docfx docs/docfx.json --serve

An example of the sorts of replacement operations that happen:
 - Speakeasy markdown relative link: `docs/sdks/opaapiclient/README.md#executepolicy`
 - Equivalent DocFX URL: `$<HOST>/api/Styra.Opa.OpenApi.OpaApiClient.html#Styra_Opa_OpenApi_OpaApiClient_ExecutePolicyAsync_Styra_Opa_OpenApi_Models_Requests_ExecutePolicyRequest_`

This is obviously not trivial to reverse engineer, so we update the links by hand for now.
The manual updating works well enough because these APIs are fairly stable for now, and only need updating when one of the referenced method signatures changes.
