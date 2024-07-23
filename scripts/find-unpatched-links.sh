#!/usr/bin/env bash
# This script checks for any Speakeasy docs links that were not patched by the
# README patching script.

set -e

usage() {
    echo "Usage:" >&2
    echo "  find-unpatched-links.sh <FILENAME>" >&2
    echo "Example:" >&2
    echo "  find-unpatched-links.sh README.md" >&2
}

# Ensure we have enough args.
if [[ $# -lt 1 ]]; then
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

#
if [[ $(grep 'docs/sdks/' $1 | wc -l) -gt 0 ]]; then
    echo "Found unpatched docs links in '$1':" >&2
    grep 'docs/sdks/' $1 >&2
    exit 1
fi