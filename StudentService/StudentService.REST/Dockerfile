#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["StudentService.REST/StudentService.REST.csproj", "StudentService.REST/"]
COPY ["StudentService.Business.Interfaces/StudentService.Business.Interfaces.csproj", "StudentService.Business.Interfaces/"]
COPY ["StudentService.Business.Models/StudentService.Business.Models.csproj", "StudentService.Business.Models/"]
COPY ["CommandsStack/StudentService.Business.csproj", "CommandsStack/"]
COPY ["StudentService.Repository.Interfaces/StudentService.Repository.Interfaces.csproj", "StudentService.Repository.Interfaces/"]
COPY ["StudentService.Data/StudentService.Data.csproj", "StudentService.Data/"]
COPY ["StudentService.Repository/StudentService.Repository.csproj", "StudentService.Repository/"]
COPY ["CommandsStack.Interfaces/Commands.Infrastructure.csproj", "CommandsStack.Interfaces/"]
RUN dotnet restore "StudentService.REST/StudentService.REST.csproj"
COPY . .
WORKDIR "/src/StudentService.REST"
RUN dotnet build "StudentService.REST.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "StudentService.REST.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "StudentService.REST.dll"]