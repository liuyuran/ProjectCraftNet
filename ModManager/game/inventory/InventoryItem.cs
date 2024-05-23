namespace ModManager.game.inventory;

/// <summary>
/// 背包格子信息
/// </summary>
public struct InventoryItem
{
    public long ItemId; // 物品ID
    public long Count; // 数量
    public string Clob; // JSON化扩展信息
}