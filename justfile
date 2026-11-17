compose := "docker compose"
compose_e2e := compose + " -f compose.e2e.yaml"
compose_app_dev := compose + " -f compose.yaml -f compose.override.yaml"
compose_docs := compose_app_dev + " run --rm --build docs"
repo := "ghcr.io/syriiin/difficalcy-performanceplus"
export OSU_COMMIT_HASH := `git rev-parse HEAD:osu`

# List available commands
[default]
help:
    @just --list

[no-exit-message]
[private]
check-flake:
    #!/usr/bin/env bash
    set -euo pipefail
    just_dir="{{ justfile_directory() }}"
    if [ -z "${DIFFICALCY_PERFORMANCEPLUS_FLAKE_HASH:-}" ]; then
        echo "Error: not in the Nix dev shell. Run 'nix develop' first." >&2
        exit 1
    fi
    current="$(cat "$just_dir/flake.nix" "$just_dir/flake.lock" | sha256sum | cut -d' ' -f1)"
    if [ "$current" != "$DIFFICALCY_PERFORMANCEPLUS_FLAKE_HASH" ]; then
        echo "Error: flake.nix or flake.lock changed since the shell started. Re-enter with 'nix develop'." >&2
        exit 1
    fi

[no-exit-message]
[private]
check-token:
    #!/usr/bin/env bash
    set -euo pipefail
    if [ -z "${GITHUB_TOKEN:-}" ]; then
        echo "Error: GITHUB_TOKEN env var is not set." >&2
        exit 1
    fi

[no-exit-message]
[private]
check-master:
    #!/usr/bin/env bash
    set -euo pipefail
    if [ "$(git branch --show-current)" != "master" ]; then
        echo "Error: this command can only be run on the master branch." >&2
        exit 1
    fi

[no-exit-message]
[private]
check-clean:
    #!/usr/bin/env bash
    set -euo pipefail
    if [ -n "$(git status --porcelain)" ]; then
        echo "Error: there are uncommitted changes in the working directory." >&2
        exit 1
    fi

# Run the test suite
test: check-flake
    dotnet test

# Run the E2E test suite (slim + full)
test-e2e: check-flake
    {{ compose_e2e }} run --rm --build e2e-test-runner
    {{ compose_e2e }} down --volumes

# Regenerate the E2E response snapshot
update-e2e-snapshot: check-flake
    UPDATE_SNAPSHOTS=true {{ compose_e2e }} run --rm --build e2e-test-runner
    {{ compose_e2e }} down --volumes

# Build development docker images
build-dev: check-flake
    {{ compose_app_dev }} build

# Start the development environment
start-dev: build-dev
    {{ compose_app_dev }} up -d

# Clean the development environment
clean-dev: check-flake
    {{ compose_app_dev }} down --remove-orphans

# Reset the development environment
reset-dev: check-flake
    {{ compose_app_dev }} down --remove-orphans --volumes

# Update OpenAPI schemas in the docs site
update-api-reference: check-flake
    scripts/update-api-reference.sh

# Check that OpenAPI schemas are up to date
check-api-reference: check-flake
    scripts/check-api-reference.sh

# Build the documentation site
build-docs: check-flake
    {{ compose_docs }} build --strict --clean

# Restore .NET tooling
restore-dotnet-tooling: check-flake
    dotnet tool restore

# Check code formatting
check-formatting: restore-dotnet-tooling
    dotnet tool run csharpier check Difficalcy.PerformancePlus Difficalcy.PerformancePlus.Api Difficalcy.PerformancePlus.Tests
    treefmt --ci
    just --fmt --check --unstable

# Fix code formatting
fix-formatting: restore-dotnet-tooling
    dotnet tool run csharpier format Difficalcy.PerformancePlus Difficalcy.PerformancePlus.Api Difficalcy.PerformancePlus.Tests
    treefmt
    just --fmt --unstable

# Push docker images to ghcr.io and create a GitHub release
release version username: check-flake check-token check-master check-clean
    echo "$GITHUB_TOKEN" | docker login ghcr.io --username "{{ username }}" --password-stdin
    docker build --build-arg OSU_COMMIT_HASH="{{ OSU_COMMIT_HASH }}" . --target publish \
        -t "{{ repo }}:{{ version }}" \
        -t "{{ repo }}:latest" \
        -t "{{ repo }}:{{ version }}-slim" \
        -t "{{ repo }}:latest-slim"
    docker build --build-arg OSU_COMMIT_HASH="{{ OSU_COMMIT_HASH }}" . --target publish-full \
        -t "{{ repo }}:{{ version }}-full" \
        -t "{{ repo }}:latest-full"
    docker push "{{ repo }}:{{ version }}"
    docker push "{{ repo }}:latest"
    docker push "{{ repo }}:{{ version }}-slim"
    docker push "{{ repo }}:latest-slim"
    docker push "{{ repo }}:{{ version }}-full"
    docker push "{{ repo }}:latest-full"
    gh release create "{{ version }}" --generate-notes

# Push docker images to ghcr.io and create a GitHub prerelease
pre-release version username: check-flake check-token check-clean
    echo "$GITHUB_TOKEN" | docker login ghcr.io --username "{{ username }}" --password-stdin
    docker build --build-arg OSU_COMMIT_HASH="{{ OSU_COMMIT_HASH }}" . --target publish \
        -t "{{ repo }}:{{ version }}" \
        -t "{{ repo }}:{{ version }}-slim"
    docker build --build-arg OSU_COMMIT_HASH="{{ OSU_COMMIT_HASH }}" . --target publish-full \
        -t "{{ repo }}:{{ version }}-full"
    docker push "{{ repo }}:{{ version }}"
    docker push "{{ repo }}:{{ version }}-slim"
    docker push "{{ repo }}:{{ version }}-full"
    gh release create "{{ version }}" --generate-notes --prerelease --target "$(git branch --show-current)"
