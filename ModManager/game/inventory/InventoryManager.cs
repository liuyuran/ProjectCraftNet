namespace ModManager.game.inventory;

/// <summary>
/// 背包和装备管理器
/// </summary>
public class InventoryManager
{
    private readonly Dictionary<long, List<Inventory>> _inventories = new();
    private static InventoryManager Instance => new();
    
    public static void AttachInventory(long userId, long size)
    {
        if (!Instance._inventories.ContainsKey(userId))
            Instance._inventories[userId] = [];
        Instance._inventories[userId].Add(new Inventory(size));
    }
    
    public static void DetachInventory(long userId)
    {
        Instance._inventories.Remove(userId);
    }
    
    public static List<Inventory> GetInventory(long userId)
    {
        return !Instance._inventories.TryGetValue(userId, out var value) ? [] : value;
    }
}