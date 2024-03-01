using Microsoft.Extensions.Logging;
using ModManager.logger;
using static ModManager.localization.LocalizationManager;
using static ModManager.ModManager;

namespace ModManager.block;

/// <summary>
/// 方块管理器，用于注册各种方块
/// </summary>
public class BlockManager
{
    private static readonly ILogger Logger = SysLogger.GetLogger(typeof(BlockManager));
    private static BlockManager Instance { get; } = new();
    private readonly Dictionary<ulong, BlockMeta> _blockMeta = new();
    private ulong _idGenerator;
    private BlockManager() {}
    
    public static void RegisterBlock(BlockMeta meta)
    {
        Instance._blockMeta.Add(Instance._idGenerator++, meta);
        Logger.LogInformation("{}", Localize(ModId, "Block registered: {0}", meta.BlockId));
    }
    
    public static void RemoveBlock(ulong id)
    {
        Instance._blockMeta.Remove(id);
        Logger.LogInformation("{}", Localize(ModId, "Block removed: {0}", id));
    }
    
    public static BlockMeta? GetBlock(ulong id)
    {
        return Instance._blockMeta.TryGetValue(id, out var meta) ? meta : null;
    }
}