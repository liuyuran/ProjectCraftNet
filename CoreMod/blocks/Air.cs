using ModManager.game.block;

namespace CoreMod.blocks;

public class Air: Block
{
    public override string Id => "core:air";
    public override string Name => "空气";
    public override BlockType BlockType => BlockType.Gas;
}