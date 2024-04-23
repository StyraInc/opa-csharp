#!/usr/bin/env bash
# This script extracts the Speakeasy SDK version from `gen.yaml`.

set -e

usage() {
    echo "Usage:" >&2
    echo "  get-speakeasy-sdk-version.sh" >&2
}

# Ensure README file exists.
if [ ! -f '.speakeasy/gen.yaml' ]; then
    echo "File '$1' not found!" >&2
    usage
    exit 1
fi

# Relies on the `version` field occurring *exactly* once.
awk '/version:/ {print $2; exit}' .speakeasy/gen.yaml