#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Services/KeylessGateways.Management/KeylessGateways.Management.csproj", "Services/KeylessGateways.Management/"]
COPY ["Services/Shared/KeylessGateways.Services.Shared/KeylessGateways.Services.Shared.csproj", "Services/Shared/KeylessGateways.Services.Shared/"]
COPY ["Common/KeylessGateways.Common/KeylessGateways.Common.csproj", "Common/KeylessGateways.Common/"]
RUN dotnet restore "Services/KeylessGateways.Management/KeylessGateways.Management.csproj"
COPY . .
WORKDIR "/src/Services/KeylessGateways.Management"
RUN dotnet build "KeylessGateways.Management.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "KeylessGateways.Management.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "KeylessGateways.Management.dll"]