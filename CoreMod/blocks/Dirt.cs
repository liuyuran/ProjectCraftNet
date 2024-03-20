using ModManager.game.block;
using ModManager.game.item;

namespace CoreMod.blocks;

public class Dirt: Block, IItemProvider
{
    public override string Id => "core:dirt";
    public override string Name => "泥土";
    public override string Material => "core/dirt";
    public override BlockType BlockType => BlockType.Solid;

    public Item AsItem()
    {
        return new Item.Builder(this)
            .SetMaxStackSize(64)
            .Build();
    }
}