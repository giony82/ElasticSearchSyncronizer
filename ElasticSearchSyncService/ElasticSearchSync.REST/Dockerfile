#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["ElasticSearchSync.REST/ElasticSearchSync.REST.csproj", "ElasticSearchSync.REST/"]
COPY ["ElasticSearch.Business.Interfaces/ElasticSearch.Business.Interfaces.csproj", "ElasticSearch.Business.Interfaces/"]
COPY ["ElasticSearch.Models/ElasticSearch.Business.Models.csproj", "ElasticSearch.Models/"]
COPY ["Redis/Redis.Business.csproj", "Redis/"]
COPY ["Utils/SchoolUtils.csproj", "Utils/"]
COPY ["ElasticSearch.Business/ElasticSearch.Business.csproj", "ElasticSearch.Business/"]
COPY ["ElasticSearch.Repository/ElasticSearch.Repository.csproj", "ElasticSearch.Repository/"]
COPY ["ElasticSearch.Repository.Interfaces/ElasticSearch.Repository.Interfaces.csproj", "ElasticSearch.Repository.Interfaces/"]
COPY ["ElasticSearch.Repository.Entities/ElasticSearch.Repository.Entities.csproj", "ElasticSearch.Repository.Entities/"]
RUN dotnet restore "ElasticSearchSync.REST/ElasticSearchSync.REST.csproj"
COPY . .
WORKDIR "/src/ElasticSearchSync.REST"
RUN dotnet build "ElasticSearchSync.REST.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ElasticSearchSync.REST.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ElasticSearchSync.REST.dll"]