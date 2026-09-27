#!/usr/bin/env bash
set -euo pipefail

dotnet build

cp Difficalcy.PerformancePlus.Api/obj/Difficalcy.PerformancePlus.Api.json docs/docs/difficalcy-performanceplus.json
