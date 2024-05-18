#!/bin/bash
set -euo pipefail

dotnet build

diff -q docs/docs/difficalcy-performanceplus.json <(dotnet tool run swagger tofile Difficalcy.PerformancePlus/bin/Debug/net8.0/Difficalcy.PerformancePlus.dll v1)
