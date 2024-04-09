namespace ModManager.game.inventory;

/// <summary>
/// 背包和装备管理器
/// </summary>
public class InventoryManager
{
    private readonly Dictionary<long, Inventory> _inventories = new();
    private static InventoryManager Instance => new();
    
    public static void AttachInventory(long userId)
    {
        Instance._inventories[userId] = new Inventory();
    }
    
    public static void DetachInventory(long userId)
    {
        Instance._inventories.Remove(userId);
    }
    
    public static Inventory? GetInventory(long userId)
    {
        return Instance._inventories.GetValueOrDefault(userId);
    }
}