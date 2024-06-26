﻿// ReSharper disable ClassNeverInstantiated.Global

using Tomlyn.Model;

namespace ModManager.config;

public sealed class Config : ITomlMetadataProvider {
    public Core? Core { get; set; }
    public NetworkTcp? NetworkTcp { get; set; }
    public Database? Database { get; set; }
    public TomlPropertiesMetadata? PropertiesMetadata { get; set; }
}

public sealed class Core : ITomlMetadataProvider {
    public required string Title { get; set; }
    public required string ModPath { get; set; }
    public required int MaxPlayer { get; set; }
    public required string LocalizationPath { get; set; }
    public required string LogLevel { get; set; }
    public required int MaxTps { get; set; }
    public required int Sight { get; set; }
    public required int ChunkSize { get; set; }
    public TomlPropertiesMetadata? PropertiesMetadata { get; set; }
}

public sealed class NetworkTcp : ITomlMetadataProvider {
    public required string Host { get; set; }
    public required int Port { get; set; }
    public TomlPropertiesMetadata? PropertiesMetadata { get; set; }
}

public sealed class Database : ITomlMetadataProvider {
    public required string Host { get; set; }
    public required int Port { get; set; }
    public required string Username { get; set; }
    public required string Password { get; set; }
    public required string Db { get; set; }
    public TomlPropertiesMetadata? PropertiesMetadata { get; set; }
}