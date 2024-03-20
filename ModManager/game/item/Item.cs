using ModManager.game.block;

namespace ModManager.game.item;

/// <summary>
/// 物品Meta信息储存类，自带Builder模式
/// </summary>
public class Item
{
    public readonly int MaxStackSize;
    public readonly string Group;

    private Item(int maxStackSize, string group)
    {
        MaxStackSize = maxStackSize;
        Group = group;
    }

    public class Builder(Block baseBlock)
    {
        private string Name { get; set; } = baseBlock.Name;
        private string Group { get; set; } = "default";
        private int MaxStackSize { get; set; } = 64;

        public Builder SetMaxStackSize(int maxStackSize)
        {
            MaxStackSize = maxStackSize;
            return this;
        }
        
        public Item Build()
        {
            return new Item(MaxStackSize, Group);
        }
    }
}