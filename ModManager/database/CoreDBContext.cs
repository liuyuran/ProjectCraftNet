using Microsoft.EntityFrameworkCore;
using ModManager.config;
using ModManager.database.generate;

namespace ModManager.database;

/// <summary>
/// 该类用于连接数据库
/// </summary>
public class CoreDbContext: GameContext {
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        var config = ConfigUtil.Instance.GetConfig();
        if (config.Database == null) {
            throw new Exception("Config.Database is null.");
        }
        optionsBuilder.UseNpgsql($"Host={config.Database.Host};Database={config.Database.Db};Username={config.Database.Username};Password={config.Database.Password}");
    }
}