namespace ModManager.localization;

public record StringData
{
    public required string Category { get; init; }
    public required string LocalizedString { get; init; }
}