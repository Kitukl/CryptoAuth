FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["CryptoAnalyzer.Auth/CryptoAnalyzer.Auth.csproj", "CryptoAnalyzer.Auth/"]
COPY ["CryptoAuth.DAL/CryptoAuth.DAL.csproj", "CryptoAuth.DAL/"]
COPY ["CryptoAuth.BLL/CryptoAuth.BLL.csproj", "CryptoAuth.BLL/"]
RUN dotnet restore "CryptoAnalyzer.Auth/CryptoAnalyzer.Auth.csproj"
COPY . .
WORKDIR "/src/CryptoAnalyzer.Auth"
RUN dotnet build "./CryptoAnalyzer.Auth.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./CryptoAnalyzer.Auth.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

RUN dotnet tool install --global dotnet-ef --version 9.0.*
ENV PATH="${PATH}:/root/.dotnet/tools"

COPY --from=build /src/CryptoAnalyzer.Auth/ /app/CryptoAnalyzer.Auth/
COPY --from=build /src/CryptoAuth.DAL/ /app/CryptoAuth.DAL/
COPY --from=build /src/CryptoAuth.BLL/ /app/CryptoAuth.BLL/

ENTRYPOINT ["/bin/bash", "-c", "dotnet ef database update --project /app/CryptoAuth.DAL/CryptoAuth.DAL.csproj --startup-project /app/CryptoAnalyzer.Auth/CryptoAnalyzer.Auth.csproj && dotnet CryptoAnalyzer.Auth.dll"]
