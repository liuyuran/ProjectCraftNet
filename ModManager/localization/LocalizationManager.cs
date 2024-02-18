using System.Text.Json;

namespace ModManager.localization;

public class LocalizationManager
{
    private static Dictionary<string, Dictionary<string, StringData>> _localization = new();
    private static HashSet<string> _supportedLanguages = new();
    private static string currentLanguage = "zh-cn";
    private static HashSet<string> _modIds = new();
    
    private LocalizationManager(string modId)
    {
        if (_modIds.Contains(modId))
        {
            throw new ArgumentException($"Mod {modId} already exists.");
        }

        _modIds.Add(modId);
    }
    
    public void LoadLocalization(string lang, string path)
    {
        var data = File.ReadAllText(path);
        // TODO 这里确定用JSON吗？
        var localization = JsonSerializer.Deserialize<Dictionary<string, StringData>>(data);
        _localization[lang] = localization ?? throw new ArgumentException($"Failed to deserialize localization file {path}.");
        _supportedLanguages.Add(lang);
    }

    public static LocalizationManager GetLocalizationManager(string modId)
    {
        return new LocalizationManager(modId);
    }
    
    public static void SetCurrentLanguage(string lang)
    {
        if (!_supportedLanguages.Contains(lang))
        {
            throw new KeyNotFoundException($"Language {lang} not found.");
        }

        currentLanguage = lang;
    }
    
    /// <summary>
    /// 获取本地化字符串
    /// </summary>
    /// <param name="key">识别字符串</param>
    /// <param name="lang">目标语言</param>
    /// <returns>本地化字符串</returns>
    public static string Localize(string key, string? lang = null)
    {
        lang ??= currentLanguage;
        return Localize(key, lang, Array.Empty<object>());
    }

    /// <summary>
    /// 获取本地化字符串
    /// </summary>
    /// <param name="key">识别字符串</param>
    /// <param name="lang">目标语言</param>
    /// <param name="args">扩展参数</param>
    /// <returns>本地化字符串</returns>
    public static string Localize(string key, string? lang = null, params object[] args)
    {
        lang ??= currentLanguage;
        var localization = _localization[lang];
        if (localization == null)
        {
            throw new KeyNotFoundException($"Language {lang} not found.");
        }

        if (!localization.TryGetValue(key, out var value))
        {
            throw new KeyNotFoundException($"Key {key} not found in language {lang}.");
        }

        return string.Format(value.LocalizedString, args);
    }
}