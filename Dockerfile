# Build Stage

FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
 
WORKDIR /src

# restoring dependencies
COPY ["AuthenticationAPI/AuthenticationAPI.csproj", "AuthenticationAPI/"]
RUN dotnet restore "AuthenticationAPI/AuthenticationAPI.csproj"

# build project
COPY ["AuthenticationAPI/", "AuthenticationAPI/"]
WORKDIR /src/AuthenticationAPI
RUN dotnet build "AuthenticationAPI.csproj" -c Release -o /app/build

# Publish Stage

# publish project
FROM build AS publish
RUN dotnet publish "AuthenticationAPI.csproj" -c Release -o /app/publish

# Run Stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS base

WORKDIR /app

COPY ["AuthenticationAPI/Database" ,"./Database"]
COPY --from=publish /app/publish .

ENV ASPNETCORE_HTTP_PORTS=5001
ENV ASPNETCORE_ENVIRONMENT=Development
EXPOSE 5001

ENTRYPOINT ["dotnet", "AuthenticationAPI.dll"]
