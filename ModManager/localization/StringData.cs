namespace ModManager.localization;

public record StringData(string Category, string LocalizedString)
{
    public required string Category { get; init; } = Category;
    public required string LocalizedString { get; init; } = LocalizedString;
}