FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine3.19 AS base

LABEL org.opencontainers.image.source https://github.com/Syriiin/difficalcy-performanceplus

USER app

WORKDIR /app
EXPOSE 80
ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_ENVIRONMENT=Production
ENV BEATMAP_DIRECTORY=/home/app/beatmaps

VOLUME ${BEATMAP_DIRECTORY}
RUN mkdir ${BEATMAP_DIRECTORY}

FROM mcr.microsoft.com/dotnet/sdk:8.0-jammy AS build
WORKDIR /src

COPY ./Difficalcy.PerformancePlus/Difficalcy.PerformancePlus.csproj ./Difficalcy.PerformancePlus/
COPY ./difficalcy/Difficalcy/Difficalcy.csproj ./difficalcy/Difficalcy/
COPY ./osu/osu.Game.Rulesets.Osu/osu.Game.Rulesets.Osu.csproj ./osu/osu.Game.Rulesets.Osu/
COPY ./osu/osu.Game/osu.Game.csproj ./osu/osu.Game/

RUN dotnet restore Difficalcy.PerformancePlus/Difficalcy.PerformancePlus.csproj

COPY ./Difficalcy.PerformancePlus/ ./Difficalcy.PerformancePlus/
COPY ./difficalcy/Difficalcy/ ./difficalcy/Difficalcy/
COPY ./osu/osu.Game.Rulesets.Osu/ ./osu/osu.Game.Rulesets.Osu/
COPY ./osu/osu.Game/ ./osu/osu.Game/

RUN dotnet publish Difficalcy.PerformancePlus/Difficalcy.PerformancePlus.csproj -o /app/difficalcy-performanceplus --runtime linux-musl-x64 --self-contained false

FROM base AS difficalcy-performanceplus
LABEL org.opencontainers.image.description "Lazer powered osu! PP+ difficulty calculator API"
COPY --from=build /app/difficalcy-performanceplus .
ENTRYPOINT ["./Difficalcy.PerformancePlus"]
