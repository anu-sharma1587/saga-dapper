FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY ["src/BuildingBlocks/Common/HotelManagement.BuildingBlocks.Common/HotelManagement.BuildingBlocks.Common.csproj", "BuildingBlocks/Common/HotelManagement.BuildingBlocks.Common/"]
COPY ["src/BuildingBlocks/Security/HotelManagement.BuildingBlocks.Security/HotelManagement.BuildingBlocks.Security.csproj", "BuildingBlocks/Security/HotelManagement.BuildingBlocks.Security/"]
COPY ["src/BuildingBlocks/Resilience/HotelManagement.BuildingBlocks.Resilience/HotelManagement.BuildingBlocks.Resilience.csproj", "BuildingBlocks/Resilience/HotelManagement.BuildingBlocks.Resilience/"]
COPY ["src/BuildingBlocks/Observability/HotelManagement.BuildingBlocks.Observability/HotelManagement.BuildingBlocks.Observability.csproj", "BuildingBlocks/Observability/HotelManagement.BuildingBlocks.Observability/"]
COPY ["src/Services/ApiGateway/HotelManagement.Services.ApiGateway/HotelManagement.Services.ApiGateway.csproj", "Services/ApiGateway/HotelManagement.Services.ApiGateway/"]

RUN dotnet restore "Services/ApiGateway/HotelManagement.Services.ApiGateway/HotelManagement.Services.ApiGateway.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/Services/ApiGateway/HotelManagement.Services.ApiGateway"
RUN dotnet build "HotelManagement.Services.ApiGateway.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "HotelManagement.Services.ApiGateway.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HotelManagement.Services.ApiGateway.dll"]
