FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
# 编译主工程
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ProjectCraftNet/ProjectCraftNet.csproj", "./"]
RUN dotnet restore "ProjectCraftNet.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "ProjectCraftNet/ProjectCraftNet.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "ProjectCraftNet/ProjectCraftNet.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ProjectCraftNet.dll"]
