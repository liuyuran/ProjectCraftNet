FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
# 编译主工程
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ModManager/ModManager.csproj", "ModManager/ModManager.csproj"]
RUN dotnet restore "ModManager/ModManager.csproj"
COPY ["ProjectCraftNet/ProjectCraftNet.csproj", "ProjectCraftNet/ProjectCraftNet.csproj"]
RUN dotnet restore "ProjectCraftNet/ProjectCraftNet.csproj"
COPY ["CoreMod/CoreMod.csproj", "CoreMod/CoreMod.csproj"]
RUN dotnet restore "CoreMod/CoreMod.csproj"
COPY . .
RUN dotnet build "ProjectCraftNet/ProjectCraftNet.csproj" -c $BUILD_CONFIGURATION -o /app/build/main
RUN dotnet build "CoreMod/CoreMod.csproj" -c $BUILD_CONFIGURATION -o /app/build/core-mod

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "ProjectCraftNet/ProjectCraftNet.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false
RUN dotnet publish "CoreMod/CoreMod.csproj" -c $BUILD_CONFIGURATION -o /app/publish/mods/core-mod /p:UseAppHost=false
COPY CoreMod/mod.toml /app/publish/mods/core-mod/mod.toml

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
RUN mkdir config;
COPY ./ProjectCraftNet/resources .
ENTRYPOINT ["dotnet", "ProjectCraftNet.dll"]
