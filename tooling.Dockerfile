FROM mcr.microsoft.com/dotnet/sdk:8.0-jammy AS base

RUN curl -fsSL https://cli.github.com/packages/githubcli-archive-keyring.gpg | dd of=/usr/share/keyrings/githubcli-archive-keyring.gpg \
    && chmod go+r /usr/share/keyrings/githubcli-archive-keyring.gpg \
    && echo "deb [arch=$(dpkg --print-architecture) signed-by=/usr/share/keyrings/githubcli-archive-keyring.gpg] https://cli.github.com/packages stable main" | tee /etc/apt/sources.list.d/github-cli.list > /dev/null \
    && apt update \
    && apt install gh -y

WORKDIR /app

COPY .config/dotnet-tools.json ./.config/

RUN dotnet tool restore

COPY difficalcy-performanceplus.sln .
COPY ./Difficalcy.PerformancePlus/Difficalcy.PerformancePlus.csproj ./Difficalcy.PerformancePlus/
COPY ./difficalcy/Difficalcy/Difficalcy.csproj ./difficalcy/Difficalcy/
COPY ./osu/osu.Game.Rulesets.Osu/osu.Game.Rulesets.Osu.csproj ./osu/osu.Game.Rulesets.Osu/
COPY ./osu/osu.Game/osu.Game.csproj ./osu/osu.Game/
COPY ./Difficalcy.PerformancePlus.Tests/Difficalcy.PerformancePlus.Tests.csproj ./Difficalcy.PerformancePlus.Tests/
COPY ./difficalcy/Difficalcy.Tests/Difficalcy.Tests.csproj ./difficalcy/Difficalcy.Tests/

RUN dotnet restore

COPY . .
