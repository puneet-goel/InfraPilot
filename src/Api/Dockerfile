# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

ARG BUILD_CONFIGURATION=Release

# Install Node.js
RUN apt-get update && apt-get install -y curl
RUN curl -fsSL https://deb.nodesource.com/setup_20.x | bash -
RUN apt-get install -y nodejs

WORKDIR /src

# Copy csproj files first for docker layer caching
COPY ["src/Servers/DeploymentMCP/DeploymentMCP.csproj", "src/Servers/DeploymentMCP/"]
COPY ["src/Servers/InfrastructureMCP/InfrastructureMCP.csproj", "src/Servers/InfrastructureMCP/"]
COPY ["src/Api/Api.csproj", "src/Api/"]
COPY ["src/Agents/Agents.csproj", "src/Agents/"]
COPY ["src/Database/Database.csproj", "src/Database/"]

# Restore
RUN dotnet restore "./src/Api/Api.csproj"

# Copy full source
COPY . .

# Build frontend
WORKDIR /src/src/Frontend
RUN npm install
RUN npm run build

# Copy frontend build into ASP.NET wwwroot
RUN mkdir -p /src/src/Api/wwwroot \
    && cp -r /src/src/Frontend/dist/* /src/src/Api/wwwroot/

# Build backend
WORKDIR /src/src/Api
RUN dotnet build "./Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Publish DeploymentMCP
WORKDIR /src/src/Servers/DeploymentMCP
RUN dotnet publish "./DeploymentMCP.csproj" -c $BUILD_CONFIGURATION -o /app/publish/mcp/deployment /p:UseAppHost=false

# Publish InfrastructureMCP
WORKDIR /src/src/Servers/InfrastructureMCP
RUN dotnet publish "./InfrastructureMCP.csproj" -c $BUILD_CONFIGURATION -o /app/publish/mcp/infrastructure /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Api.dll"]