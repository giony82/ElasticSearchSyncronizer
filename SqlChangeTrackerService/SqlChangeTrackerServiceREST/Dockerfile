#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["ElasticSearchWebService/SqlChangeTrackerService.REST.csproj", "ElasticSearchWebService/"]
COPY ["../Nuget/Redis.Interfaces/Redis.Interfaces.csproj", "../Nuget/Redis.Interfaces/"]
COPY ["../Nuget/Redis.Business/Redis.Business.csproj", "../Nuget/Redis.Business/"]
COPY ["../Nuget/Utils/SchoolUtils.csproj", "../Nuget/Utils/"]
COPY ["SqlChangeTracker/SqlChangeTrackerService.Business.csproj", "SqlChangeTracker/"]
RUN dotnet restore "ElasticSearchWebService/SqlChangeTrackerService.REST.csproj"
COPY . .
WORKDIR "/src/ElasticSearchWebService"
RUN dotnet build "SqlChangeTrackerService.REST.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SqlChangeTrackerService.REST.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SqlChangeTrackerService.REST.dll"]