using System;
using System.Collections.Generic;

namespace ModManager.database;

public partial class World
{
    /// <summary>
    /// 世界id
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 世界名称
    /// </summary>
    public int Name { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreateTime { get; set; }

    /// <summary>
    /// 最后激活时间
    /// </summary>
    public DateTime ActiveTime { get; set; }
}
