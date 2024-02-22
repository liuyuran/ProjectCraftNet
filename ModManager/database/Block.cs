using System;
using System.Collections.Generic;

namespace ModManager.database;

public partial class Block
{
    public long Id { get; set; }

    public string BlockName { get; set; } = null!;

    public string ModName { get; set; } = null!;

    /// <summary>
    /// 方块状态，如是否被禁用
    /// </summary>
    public int State { get; set; }
}
