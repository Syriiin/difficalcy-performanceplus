FROM mcr.microsoft.com/dotnet/runtime-deps:10.0-noble-chiseled AS base

LABEL org.opencontainers.image.source https://github.com/Syriiin/difficalcy-performanceplus

WORKDIR /app
EXPOSE 80
ENV ASPNETCORE_URLS="http://+:80"
ENV ASPNETCORE_ENVIRONMENT="Production"
ENV BEATMAP_DIRECTORY="/beatmaps"
ENV DOWNLOAD_MISSING_BEATMAPS="true"
ENV BEATMAP_DOWNLOAD_URL="https://osu.ppy.sh/osu/{beatmapId}"
ARG OSU_COMMIT_HASH
ENV OSU_COMMIT_HASH=${OSU_COMMIT_HASH}

VOLUME ${BEATMAP_DIRECTORY}

USER app

# -----------------------------------------------------------------------------

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build-base
WORKDIR /src

ARG OSU_COMMIT_HASH
ENV OSU_COMMIT_HASH=${OSU_COMMIT_HASH}

COPY ./Directory.Build.props ./

# AOT compilation dependencies
RUN apt-get update && apt-get install -y clang zlib1g-dev && rm -rf /var/lib/apt/lists/*

# Restore main project
COPY ./Difficalcy.PerformancePlus/Difficalcy.PerformancePlus.csproj ./Difficalcy.PerformancePlus/
COPY ./Difficalcy.PerformancePlus.Api/Difficalcy.PerformancePlus.Api.csproj ./Difficalcy.PerformancePlus.Api/
COPY ./difficalcy/Difficalcy/Difficalcy.csproj ./difficalcy/Difficalcy/
COPY ./osu/osu.Game.Rulesets.Osu/osu.Game.Rulesets.Osu.csproj ./osu/osu.Game.Rulesets.Osu/
COPY ./osu/osu.Game/osu.Game.csproj ./osu/osu.Game/

RUN dotnet restore ./Difficalcy.PerformancePlus.Api/Difficalcy.PerformancePlus.Api.csproj

# Copy source
COPY ./Difficalcy.PerformancePlus/ ./Difficalcy.PerformancePlus/
COPY ./Difficalcy.PerformancePlus.Api/ ./Difficalcy.PerformancePlus.Api/
COPY ./difficalcy/Difficalcy/ ./difficalcy/Difficalcy/
COPY ./osu/osu.Game.Rulesets.Osu/ ./osu/osu.Game.Rulesets.Osu/
COPY ./osu/osu.Game/ ./osu/osu.Game/

RUN mkdir -p /beatmaps && chmod -R 777 /beatmaps

# -----------------------------------------------------------------------------

FROM build-base AS build
RUN dotnet publish ./Difficalcy.PerformancePlus.Api/Difficalcy.PerformancePlus.Api.csproj -o /app/difficalcy-performanceplus --runtime linux-x64 --self-contained true \
    && rm -f /app/difficalcy-performanceplus/*.dbg /app/difficalcy-performanceplus/*.pdb /app/difficalcy-performanceplus/*.Development.json

# -----------------------------------------------------------------------------

FROM base AS publish
LABEL org.opencontainers.image.description "Lazer powered osu! PP+ difficulty calculator API"
COPY --from=build --chown=app:app /beatmaps /beatmaps
COPY --from=build /app/difficalcy-performanceplus .
ENTRYPOINT ["./Difficalcy.PerformancePlus.Api"]

# -----------------------------------------------------------------------------

FROM build-base AS build-slim
COPY ./difficalcy/tools/StripResources/StripResources.csproj ./tools/StripResources/
RUN dotnet restore ./tools/StripResources/StripResources.csproj
COPY ./difficalcy/tools/StripResources/ ./tools/StripResources/
RUN dotnet build ./tools/StripResources/StripResources.csproj -o /tools && \
    /tools/StripResources \
        /root/.nuget/packages/ppy.osu.game.resources/*/lib/netstandard2.1/osu.Game.Resources.dll \
        /tmp/osu.Game.Resources.dll && \
    cp /tmp/osu.Game.Resources.dll \
        /root/.nuget/packages/ppy.osu.game.resources/*/lib/netstandard2.1/osu.Game.Resources.dll && \
    dotnet publish ./Difficalcy.PerformancePlus.Api/Difficalcy.PerformancePlus.Api.csproj -o /app/difficalcy-performanceplus --runtime linux-x64 --self-contained true && \
    rm -f /app/difficalcy-performanceplus/*.dbg /app/difficalcy-performanceplus/*.pdb /app/difficalcy-performanceplus/*.Development.json /app/difficalcy-performanceplus/*.so /app/difficalcy-performanceplus/*.so.*

# -----------------------------------------------------------------------------

FROM base AS publish-slim
LABEL org.opencontainers.image.description "Lazer powered osu! PP+ difficulty calculator API (slim)"
COPY --from=build-slim --chown=app:app /beatmaps /beatmaps
COPY --from=build-slim /app/difficalcy-performanceplus .
ENTRYPOINT ["./Difficalcy.PerformancePlus.Api"]
