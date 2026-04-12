# Stage 1: Build frontend
FROM node:20-alpine AS frontend-build
WORKDIR /app
COPY package.json yarn.lock tsconfig.json ./
RUN yarn install --frozen-lockfile
COPY frontend/ ./frontend/
RUN yarn build --env production

# Stage 2: Build backend
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS backend-build
ARG TARGETARCH
WORKDIR /app
COPY src/ ./src/
WORKDIR /app/src
RUN dotnet_rid="linux-$(echo $TARGETARCH | sed 's/amd64/x64/' | sed 's/arm64/arm64/')" && \
    dotnet publish NzbDrone.Console/Releasarr.Console.csproj \
    -c Release \
    -f net8.0 \
    -o /app/publish \
    -r "$dotnet_rid" \
    --self-contained false \
    -p:TreatWarningsAsErrors=false \
    -p:EnforceCodeStyleInBuild=false

# Stage 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Install SQLite native library
RUN apt-get update && \
    apt-get install -y --no-install-recommends libsqlite3-0 && \
    rm -rf /var/lib/apt/lists/*

COPY --from=backend-build /app/publish .
COPY --from=frontend-build /app/_output/net8.0/UI ./UI

ENV RELEASARR__APP__INSTANCENAME=Releasarr
ENV RELEASARR__SERVER__PORT=9898

EXPOSE 9898
VOLUME /config

ENTRYPOINT ["dotnet", "Releasarr.Console.dll", "--nobrowser", "--data=/config"]
