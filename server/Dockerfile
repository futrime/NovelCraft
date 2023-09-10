FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env
WORKDIR /app

COPY . /app
RUN dotnet publish src/Server.csproj -c Release -o build --sc false \
    -p:DebugType=none -p:PublishReadyToRun=true -p:PublishSingleFile=true

FROM mcr.microsoft.com/dotnet/runtime:7.0 AS runtime-env
WORKDIR /app

COPY --from=build-env /app/build .
COPY assets .

# Creates a non-root user with an explicit UID and adds permission to access the /app folder
RUN adduser -u 5678 --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

ENTRYPOINT ["/app/Server"]
