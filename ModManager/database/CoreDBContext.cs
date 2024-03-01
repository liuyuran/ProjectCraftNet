﻿using Microsoft.EntityFrameworkCore;
using ModManager.database.generate;

namespace ModManager.database;

/// <summary>
/// 该类用于连接数据库
/// </summary>
public class CoreDbContext: GameContext {
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        // TODO 需要把这里改为从配置文件读取
        optionsBuilder.UseNpgsql("Host=127.0.0.1;Database=game;Username=postgres;Password=example");
    }
}