#!/usr/bin/env bash
set -euo pipefail

BASE_URL="${1:-http://localhost:8080}"

echo 'Checking liveness...'
curl --fail --silent --show-error "${BASE_URL}/health/live"
printf '\n'

echo 'Checking ping and generating a log, metric and trace...'
curl --fail --silent --show-error "${BASE_URL}/api/v1/system/ping"
printf '\n'

echo 'Smoke checks passed (HTTP success). Review application logs for telemetry.'
