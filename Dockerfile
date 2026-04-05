# Stage 1: Build Angular Frontend
FROM node:18-alpine AS build-fe
WORKDIR /app
ENV NODE_OPTIONS=--openssl-legacy-provider
COPY angular/package*.json ./
RUN npm install
COPY angular/ ./
RUN npm run build -- --configuration production

# Stage 2: Build .NET Backend
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-be
WORKDIR /src

# Copy all projects first for restoration to optimize layer caching
COPY ["aspnet-core/src/EC.Web.Host/EC.Web.Host.csproj", "EC.Web.Host/"]
COPY ["aspnet-core/src/EC.Web.Core/EC.Web.Core.csproj", "EC.Web.Core/"]
COPY ["aspnet-core/src/EC.Application/EC.Application.csproj", "EC.Application/"]
COPY ["aspnet-core/src/EC.EntityFrameworkCore/EC.EntityFrameworkCore.csproj", "EC.EntityFrameworkCore/"]
COPY ["aspnet-core/src/EC.Core/EC.Core.csproj", "EC.Core/"]

RUN dotnet restore "EC.Web.Host/EC.Web.Host.csproj"

# Copy remaining files and build
COPY aspnet-core/src/ .

# Ensure wwwroot exists for frontend files
RUN mkdir -p /src/EC.Web.Host/wwwroot

# Copy built Angular files to wwwroot
COPY --from=build-fe /app/dist /src/EC.Web.Host/wwwroot

WORKDIR "/src/EC.Web.Host"
RUN dotnet publish "EC.Web.Host.csproj" -c Release -o /app/publish

# Stage 3: Final Runtime
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS final
WORKDIR /app
COPY --from=build-be /app/publish .

# Install necessary dependencies for System.Drawing.Common (used by iTextSharp) on Linux
RUN apt-get update && apt-get install -y libgdiplus libc6-dev fontconfig && rm -rf /var/lib/apt/lists/*

# Cloud Run environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 8080

ENTRYPOINT ["dotnet", "EC.Web.Host.dll"]
