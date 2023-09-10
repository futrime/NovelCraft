FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env
WORKDIR /app

COPY . .
RUN dotnet publish src/Agent.csproj -c Release -o build --sc false

FROM mcr.microsoft.com/dotnet/runtime:7.0 AS runtime-env
WORKDIR /app

COPY --from=build-env /app/build .

# Creates a non-root user with an explicit UID and adds permission to access the /app folder
# For more info, please refer to https://aka.ms/vscode-docker-python-configure-containers
RUN adduser -u 5678 --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

ENTRYPOINT ["/app/Agent"]
