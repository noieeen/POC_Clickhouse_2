﻿FROM mcr.microsoft.com/dotnet/aspnet:8.0-jammy-chiseled-extra AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["EmptyAPI/EmptyAPI.csproj", "EmptyAPI/"]
RUN dotnet restore "EmptyAPI/EmptyAPI.csproj"
COPY . .
WORKDIR "/src/EmptyAPI"
RUN dotnet build "EmptyAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "EmptyAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EmptyAPI.dll"]
