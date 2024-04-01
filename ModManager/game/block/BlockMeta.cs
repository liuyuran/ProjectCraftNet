namespace ModManager.game.block;

public struct BlockMeta
{
    // 方块id，同样的方块，每次生成配置都有可能具有不同的id
    public long BlockId;
    // 方块主键，用于标识方块，模组自身定义的字段
    public string BlockKey;
    // 方块材质描述字符串
    public string Material;
}