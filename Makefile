COMPOSE_TOOLING_RUN = docker compose -f compose.tooling.yaml run --rm --build tooling
COMPOSE_E2E = docker compose -f compose.yaml -f compose.override.e2e.yaml
COMPOSE_E2E_RUN = $(COMPOSE_E2E) run --rm --build e2e-test-runner
COMPOSE_APP_DEV = docker compose -f compose.yaml -f compose.override.yaml
COMPOSE_RUN_DOCS = docker compose -f compose.yaml -f compose.override.yaml run docs
COMPOSE_PUBLISH = docker compose -f compose.yaml -f compose.override.publish.yaml

export OSU_COMMIT_HASH = $(shell git rev-parse HEAD:osu)

help:	## Show this help
	@fgrep -h "##" $(MAKEFILE_LIST) | fgrep -v fgrep | sed -e 's/\\$$//' | sed -e 's/##//'

bash:	## Opens bash shell in tooling container
	$(COMPOSE_TOOLING_RUN) bash

test:	## Runs test suite
	$(COMPOSE_TOOLING_RUN) dotnet test

test-e2e:	## Runs E2E test suite
	$(COMPOSE_E2E_RUN)
	$(COMPOSE_E2E) down

build-dev:	## Builds development docker images
	$(COMPOSE_APP_DEV) build

start-dev: build-dev	## Starts development environment
	$(COMPOSE_APP_DEV) up -d

clean-dev:	## Cleans development environment
	$(COMPOSE_APP_DEV) down --remove-orphans

reset-dev:	## Resets development environment
	$(COMPOSE_APP_DEV) down --remove-orphans --volumes

update-api-reference:	## Updates OpenAPI schemas in docs site
	$(COMPOSE_TOOLING_RUN) scripts/update-api-reference.sh

check-api-reference: ## Checks OpenAPI schemas are updated
	$(COMPOSE_TOOLING_RUN) scripts/check-api-reference.sh

build-docs:	## Builds documentation site
	$(COMPOSE_RUN_DOCS) build --strict --clean

check-formatting:	## Checks code formatting
	$(COMPOSE_TOOLING_RUN) dotnet tool run csharpier check Difficalcy.PerformancePlus Difficalcy.PerformancePlus.Tests

fix-formatting:	## Fix code formatting
	$(COMPOSE_TOOLING_RUN) dotnet tool run csharpier format Difficalcy.PerformancePlus Difficalcy.PerformancePlus.Tests

# TODO: move gh into tooling container (requires env var considerations)
VERSION =
release:	## Pushes docker images to ghcr.io and create a github release
ifndef VERSION
	$(error VERSION is undefined)
endif
ifndef GITHUB_TOKEN
	$(error GITHUB_TOKEN env var is not set)
endif
ifndef GITHUB_USERNAME
	$(error GITHUB_USERNAME env var is not set)
endif
ifneq "$(shell git branch --show-current)" "master"
	$(error This command can only be run on the master branch)
endif
ifneq "$(shell git diff --name-only master)" ""
	$(error There are uncommitted changes in the working directory)
endif
	echo $$GITHUB_TOKEN | docker login ghcr.io --username $$GITHUB_USERNAME --password-stdin
	VERSION=$(VERSION) $(COMPOSE_PUBLISH) build
	VERSION=$(VERSION) $(COMPOSE_PUBLISH) push difficalcy-performanceplus
	VERSION=latest $(COMPOSE_PUBLISH) build
	VERSION=latest $(COMPOSE_PUBLISH) push difficalcy-performanceplus
	gh release create "$(VERSION)" --generate-notes
