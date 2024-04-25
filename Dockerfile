FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine3.19 AS base

LABEL org.opencontainers.image.source https://github.com/Syriiin/difficalcy

WORKDIR /app
EXPOSE 80
ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_ENVIRONMENT=Production
ENV BEATMAP_DIRECTORY=/app/beatmaps

RUN adduser -u 5678 --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
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
RUN dotnet build Difficalcy.PerformancePlus/Difficalcy.PerformancePlus.csproj -c Release -o /app/build

FROM base AS final
WORKDIR /app
COPY --from=build /app/build .
RUN mkdir beatmaps
ENTRYPOINT ["dotnet", "Difficalcy.PerformancePlus.dll"]
