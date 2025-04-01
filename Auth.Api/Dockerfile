FROM mcr.microsoft.com/dotnet/aspnet:8.0-jammy-chiseled-extra AS base

## Install ICU (for Globalization support)
#RUN apk add --no-cache icu-libs
#RUN apt-get update && \
#    apt-get install -y libicu-dev && \
#    rm -rf /var/lib/apt/lists/*
#
## Set environment variable
#ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
#RUN apt-get update && apt-get install -y locales
#RUN locale-gen en_US.UTF-8
#ENV LANG en_US.UTF-8
#ENV LANGUAGE en_US:en
#ENV LC_ALL en_US.UTF-8

USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Auth.Api/Auth.Api.csproj", "Auth.Api/"]
RUN dotnet restore "Auth.Api/Auth.Api.csproj"
COPY . .
WORKDIR "/src/Auth.Api"
RUN dotnet build "Auth.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Auth.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Auth.Api.dll"]
