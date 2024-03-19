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
    private readonly Dictionary<string, long> _link = new();
    private long _idGenerator;

    private BlockManager()
    {
        _RegisterBlock(new BlockMeta
        {
            BlockId = BlockEnum.Air,
            Material = ""
        });
        _RegisterBlock(new BlockMeta
        {
            BlockId = BlockEnum.Dirt,
            Material = "core/dirt.png"
        });
    }
    
    private void _RegisterBlock(BlockMeta meta)
    {
        var id = _idGenerator++;
        _blockMeta.Add(id, meta);
        _link.Add(meta.BlockId, id);
    }
    
    public static long RegisterBlock(BlockMeta meta)
    {
        var id = Instance._idGenerator++;
        Instance._blockMeta.Add(id, meta);
        Instance._link.Add(meta.BlockId, id);
        Logger.LogInformation("{}", Localize(ModId, "Block registered: {0}", meta.BlockId));
        return id;
    }
    
    public static long GetBlockId(string blockName)
    {
        return Instance._link.TryGetValue(blockName, out var id) ? id : 0;
    }
    
    public static void RemoveBlock(long id)
    {
        Instance._blockMeta.Remove(id);
        Logger.LogInformation("{}", Localize(ModId, "Block removed: {0}", id));
    }
    
    public static BlockMeta? GetBlock(long id)
    {
        return Instance._blockMeta.TryGetValue(id, out var meta) ? meta : null;
    }
}