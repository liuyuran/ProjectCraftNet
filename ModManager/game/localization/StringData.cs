namespace ModManager.localization;

/// <summary>
/// 本地化字符串单元
/// </summary>
public record StringData
{
    /// <summary>
    /// 字符串分类，如log、error等
    /// </summary>
    public required string Category { get; init; }
    
    /// <summary>
    /// 翻译后的字符串
    /// </summary>
    public required string LocalizedString { get; init; }
}