FROM gcc:latest AS build-env
WORKDIR /app

# Install prerequisites
RUN apt-get update && apt-get install -y cmake

# Build the agent
COPY . /app
RUN rm -rf build && \
    mkdir build && \
    cmake -S . -B build -DCMAKE_BUILD_TYPE=Release && \
    cmake --build build --config Release


FROM busybox:latest AS runtime-env
WORKDIR /app

# Copy the agent from the build-env
COPY --from=build-env /app/build/agent .

# Creates a non-root user with an explicit UID and adds permission to access the /app folder
# For more info, please refer to https://aka.ms/vscode-docker-python-configure-containers
RUN adduser -u 5678 --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

# Set the entrypoint
ENTRYPOINT ["/app/agent"]
