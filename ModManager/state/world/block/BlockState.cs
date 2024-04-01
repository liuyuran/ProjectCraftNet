using ModManager.state.world.block.interfaces;

namespace ModManager.state.world.block;

public class BlockState(long blockId, BlockPos pos): IBlockState
{
    public Block GetBlock()
    {
        return new Block
        {
            BlockId = blockId
        };
    }

    public BlockPos GetPos()
    {
        return pos;
    }
}