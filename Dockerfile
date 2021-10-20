FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

RUN adduser -u 5678 --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src

COPY difficalcy/Difficalcy/Difficalcy.csproj ./difficalcy/Difficalcy/
RUN dotnet restore difficalcy/Difficalcy/Difficalcy.csproj
COPY Difficalcy.Osu/Difficalcy.Osu.csproj ./Difficalcy.Osu/
RUN dotnet restore Difficalcy.Osu/Difficalcy.Osu.csproj

COPY . .
RUN dotnet build Difficalcy.Osu/Difficalcy.Osu.csproj -c Release -o /app/build

FROM base AS final
WORKDIR /app
COPY --from=build /app/build .
ENTRYPOINT ["dotnet", "Difficalcy.Osu.dll"]
