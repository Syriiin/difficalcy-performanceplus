#!/bin/bash
set -euo pipefail

dotnet build

dotnet tool run swagger tofile Difficalcy.PerformancePlus/bin/Debug/net8.0/Difficalcy.PerformancePlus.dll v1 > docs/docs/difficalcy-performanceplus.json
