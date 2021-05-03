#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Hangfire.REST/Hangfire.REST.csproj", "Hangfire.REST/"]
COPY ["Hangfire.Business.Interfaces/Hangfire.Business.Interfaces.csproj", "Hangfire.Business.Interfaces/"]
COPY ["Hangfire.Business/Hangfire.Business.csproj", "Hangfire.Business/"]
COPY ["Hangfire.Repository/Hangfire.Repository.csproj", "Hangfire.Repository/"]
COPY ["Hangfire.Repository.Interfaces/Hangfire.Repository.Interfaces.csproj", "Hangfire.Repository.Interfaces/"]
COPY ["Utils/SchoolUtils.csproj", "Utils/"]
RUN dotnet restore "Hangfire.REST/Hangfire.REST.csproj"
COPY . .
WORKDIR "/src/Hangfire.REST"
RUN dotnet build "Hangfire.REST.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Hangfire.REST.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Hangfire.REST.dll"]