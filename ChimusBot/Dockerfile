FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

RUN apt-get update \
    && apt-get install -y --no-install-recommends clang zlib1g-dev \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /src
COPY ChimusBot.csproj ./
RUN dotnet restore -r linux-x64

COPY . ./
RUN dotnet publish \
    -c Release \
    -r linux-x64 \
    --self-contained true \
    --no-restore \
    -o /app/publish

FROM mcr.microsoft.com/dotnet/runtime-deps:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish ./
ENTRYPOINT ["./ChimusBot"]
