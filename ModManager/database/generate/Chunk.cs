using System;
using System.Collections.Generic;

namespace ModManager.database.generate;

/// <summary>
/// 区块数据
/// </summary>
public partial class Chunk
{
    public int WorldId { get; set; }

    /// <summary>
    /// X轴坐标
    /// </summary>
    public int PosX { get; set; }

    /// <summary>
    /// y轴坐标
    /// </summary>
    public int PosY { get; set; }

    /// <summary>
    /// z轴坐标
    /// </summary>
    public int PosZ { get; set; }

    /// <summary>
    /// 区块数据
    /// </summary>
    public string Data { get; set; } = null!;
}
