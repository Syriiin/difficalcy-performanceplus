#!/usr/bin/env bash
set -euo pipefail

dotnet build

diff -q docs/docs/difficalcy-performanceplus.json Difficalcy.PerformancePlus.Api/obj/Difficalcy.PerformancePlus.Api.json
