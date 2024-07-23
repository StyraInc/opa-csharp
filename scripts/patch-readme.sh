#!/usr/bin/env bash
# This script replaces Speakeasy's relative docs URLs with hyperlinks to
# appropriate DocFX docs for each class/method.

set -e

usage() {
    echo "Usage:" >&2
    echo "  patch-readme.sh <FILENAME> <BASE_URL>" >&2
    echo "Example:" >&2
    echo "  patch-readme.sh README.md http://localhost:8080/" >&2
}

# Ensure we have enough args.
if [[ $# -lt 2 ]]; then
    echo "Not enough parameters provided." >&2
    usage
    exit 1
fi

# Ensure README file exists.
if [ ! -f $1 ]; then
    echo "File '$1' not found!" >&2
    usage
    exit 1
fi

# OpaApiClient SDK
sed -i "s|(docs/sdks/opaapiclient/README.md)|($2/api/Styra.Opa.OpenApi.OpaApiClient.html)|g" $1
# ExecutePolicy
sed -i "s|(docs/sdks/opaapiclient/README.md#executepolicy)|($2/api/Styra.Opa.OpenApi.OpaApiClient.html#Styra_Opa_OpenApi_OpaApiClient_ExecutePolicyAsync_Styra_Opa_OpenApi_Models_Requests_ExecutePolicyRequest_)|g" $1
# ExecutePolicyWithInput
sed -i "s|(docs/sdks/opaapiclient/README.md#executepolicywithinput)|($2/api/Styra.Opa.OpenApi.OpaApiClient.html#Styra_Opa_OpenApi_OpaApiClient_ExecutePolicyWithInputAsync_Styra_Opa_OpenApi_Models_Requests_ExecutePolicyWithInputRequest_)|g" $1
# ExecuteDefaultPolicyWithInput
sed -i "s|(docs/sdks/opaapiclient/README.md#executedefaultpolicywithinput)|($2/api/Styra.Opa.OpenApi.OpaApiClient.html#Styra_Opa_OpenApi_OpaApiClient_ExecuteDefaultPolicyWithInputAsync_Styra_Opa_OpenApi_Models_Components_Input_System_Nullable_System_Boolean__System_Nullable_Styra_Opa_OpenApi_Models_Components_GzipAcceptEncoding__)|g" $1
# ExectureBatchPolicyWithInput
sed -i "s|(docs/sdks/opaapiclient/README.md#executebatchpolicywithinput)|($2/api/Styra.Opa.OpenApi.OpaApiClient.html#Styra_Opa_OpenApi_OpaApiClient_ExecuteBatchPolicyWithInputAsync_Styra_Opa_OpenApi_Models_Requests_ExecuteBatchPolicyWithInputRequest_)|g" $1
# Health
sed -i "s|(docs/sdks/opaapiclient/README.md#health)|($2/api/Styra.Opa.OpenApi.OpaApiClient.html#Styra_Opa_OpenApi_OpaApiClient_HealthAsync_System_Nullable_System_Boolean__System_Nullable_System_Boolean__System_Collections_Generic_List_System_String__)|g" $1

