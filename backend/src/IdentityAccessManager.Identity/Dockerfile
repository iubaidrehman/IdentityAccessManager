FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["IdentityAccessManager.Shared/IdentityAccessManager.Shared.csproj", "IdentityAccessManager.Shared/"]
COPY ["IdentityAccessManager.Identity/IdentityAccessManager.Identity.csproj", "IdentityAccessManager.Identity/"]
RUN dotnet restore "IdentityAccessManager.Identity/IdentityAccessManager.Identity.csproj"
COPY . .
WORKDIR "/src/IdentityAccessManager.Identity"
RUN dotnet build "IdentityAccessManager.Identity.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "IdentityAccessManager.Identity.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "IdentityAccessManager.Identity.dll"] 