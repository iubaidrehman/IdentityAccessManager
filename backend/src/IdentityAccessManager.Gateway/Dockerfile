FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["IdentityAccessManager.Shared/IdentityAccessManager.Shared.csproj", "IdentityAccessManager.Shared/"]
COPY ["IdentityAccessManager.Gateway/IdentityAccessManager.Gateway.csproj", "IdentityAccessManager.Gateway/"]
RUN dotnet restore "IdentityAccessManager.Gateway/IdentityAccessManager.Gateway.csproj"
COPY . .
WORKDIR "/src/IdentityAccessManager.Gateway"
RUN dotnet build "IdentityAccessManager.Gateway.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "IdentityAccessManager.Gateway.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "IdentityAccessManager.Gateway.dll"] 