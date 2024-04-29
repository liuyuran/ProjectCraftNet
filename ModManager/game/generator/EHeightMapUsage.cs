namespace ModManager.game.generator;

public enum EHeightMapUsage
{
    LIGHT_BLOCKING, // 可见光方块的最大高度
    MOTION_BLOCKING, // 可移动方块的最大高度（如树苗）
    MOTION_BLOCKING_NO_LEAVES, // 可移动方块的最大高度（不含树叶）
    OCEAN_FLOOR, // 海底高度
    OCEAN_FLOOR_WG, // 海底高度（世界）
    WORLD_SURFACE, // 海平线 
    WORLD_SURFACE_WG, // 海平线（世界）
}