using Microsoft.Extensions.Logging;
using ModManager.logger;
using static ModManager.game.localization.LocalizationManager;
using static ModManager.mod.ModManager;

namespace ModManager.game.block;

/// <summary>
/// 方块管理器，用于注册各种方块
/// </summary>
public class BlockManager
{
    private static readonly ILogger Logger = SysLogger.GetLogger(typeof(BlockManager));
    private static BlockManager Instance { get; } = new();
    private readonly Dictionary<long, BlockMeta> _blockMeta = new();
    private readonly Dictionary<Type, long> _link = new();
    private long _idGenerator;
    
    public static void RegisterBlock<T>() where T: Block
    {
        var id = Instance._idGenerator++;
        var block = Activator.CreateInstance<T>();
        var meta = new BlockMeta
        {
            BlockId = block.Id,
            Material = block.Material
        };
        Instance._blockMeta.Add(id, meta);
        Instance._link.Add(typeof(T), id);
        Logger.LogInformation("{}", Localize(ModId, "Block registered: {0}", block.Id));
    }
    
    public static long GetBlockId<T>() where T: Block
    {
        return Instance._link.GetValueOrDefault(typeof(T), 0);
    }
    
    public static void UnregisterBlock<T>() where T: Block
    {
        var id = Instance._link.GetValueOrDefault(typeof(T), 0);
        if (id == 0)
        {
            Logger.LogWarning("{}", Localize(ModId, "Block not found: {0}", typeof(T).Name));
            return;
        }
        Instance._blockMeta.Remove(id);
        Logger.LogInformation("{}", Localize(ModId, "Block removed: {0}", id));
    }
    
    public static BlockMeta? GetBlock(long id)
    {
        return Instance._blockMeta.TryGetValue(id, out var meta) ? meta : null;
    }
}