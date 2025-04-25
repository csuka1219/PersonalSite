FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["PersonalSite.WebAPI/PersonalSite.WebAPI.csproj", "PersonalSite.WebAPI/"]
COPY ["../PersonalSite.Application/PersonalSite.Application.csproj", "PersonalSite.Application/"]
COPY ["../PersonalSite.Infrastructure/PersonalSite.Infrastructure.csproj", "PersonalSite.Infrastructure/"]
COPY ["../PersonalSite.Domain/PersonalSite.Domain.csproj", "PersonalSite.Domain/"]
RUN dotnet restore "PersonalSite.WebAPI/PersonalSite.WebAPI.csproj"

COPY .. .
WORKDIR "/src/PersonalSite.WebAPI"
RUN dotnet publish "PersonalSite.WebAPI.csproj" -c Release -o /app/publishFROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8085

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy all project files
COPY ["PersonalSite.WebAPI/PersonalSite.WebAPI.csproj", "PersonalSite.WebAPI/"]
COPY ["PersonalSite.Application/PersonalSite.Application.csproj", "PersonalSite.Application/"]
COPY ["PersonalSite.Domain/PersonalSite.Domain.csproj", "PersonalSite.Domain/"]
COPY ["PersonalSite.Infrastructure/PersonalSite.Infrastructure.csproj", "PersonalSite.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "PersonalSite.WebAPI/PersonalSite.WebAPI.csproj"

# Copy the rest of the code
COPY . .

# Build the application
WORKDIR "/src/PersonalSite.WebAPI"
RUN dotnet build "PersonalSite.WebAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PersonalSite.WebAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "PersonalSite.WebAPI.dll"]
