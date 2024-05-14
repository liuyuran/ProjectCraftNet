namespace ModManager.game.inventory;

/// <summary>
/// 背包实现类
/// </summary>
/// <param name="size">背包大小</param>
public class Inventory(long size)
{
    public readonly InventoryItem[] Items = new InventoryItem[size];
}