using Tomlyn;
using Tomlyn.Model;

namespace ModManager.localization;

/// <summary>
/// 本地化管理器，用于加载和管理本地化文件
/// </summary>
public class LocalizationManager
{
    private static readonly Dictionary<string, LocalizationManager> Cache = new();
    private static readonly Dictionary<string, Dictionary<string, Dictionary<string, StringData>>> Localization = new();
    private static readonly HashSet<string> SupportedLanguages = [];
    private static string _currentLanguage = "zh-cn";
    private static readonly HashSet<string> ModIds = [];
    private readonly string _modId;

    private LocalizationManager(string modId)
    {
        if (!ModIds.Add(modId))
        {
            throw new ArgumentException($"Mod {modId} already exists.");
        }

        if (!Localization.ContainsKey(modId))
        {
            Localization[modId] = new Dictionary<string, Dictionary<string, StringData>>();
        }

        _modId = modId;
    }

    /// <summary>
    /// 加载本地化文件夹
    /// </summary>
    /// <param name="path">语言文件夹地址</param>
    public void LoadLocalization(string path)
    {
        var files = Directory.GetFiles(path);
        foreach (var file in files)
        {
            var lang = Path.GetFileNameWithoutExtension(file);
            LoadLocalizationFile(_modId, lang, file);
        }
    }

    /// <summary>
    /// 加载特定本地化文件
    /// </summary>
    /// <param name="modId">mod标识符</param>
    /// <param name="lang">语言编号</param>
    /// <param name="file">语言文件地址</param>
    private static void LoadLocalizationFile(string modId, string lang, string file)
    {
        var tomlStr = File.ReadAllText(file);
        var localization = Toml.ToModel<TomlTable>(tomlStr);
        foreach (var (categoryName, categoryContent) in localization)
        {
            if (categoryContent is not TomlTable catalog) continue;
            foreach (var translate in catalog)
            {
                var data = new StringData
                {
                    Category = categoryName,
                    LocalizedString = translate.Value.ToString() ?? ""
                };
                if (!Localization[modId].TryGetValue(lang, out var value))
                {
                    value = new Dictionary<string, StringData>();
                }

                Localization[modId][lang] = value;

                value[translate.Key] = data;
            }
        }

        SupportedLanguages.Add(lang);
    }

    /// <summary>
    /// 获取特定mod所属的本地化管理器
    /// </summary>
    /// <param name="modId">mod标识符</param>
    /// <returns>本地化管理器实例</returns>
    public static LocalizationManager GetLocalizationManager(string modId)
    {
        if (Cache.TryGetValue(modId, out var manager))
        {
            return manager;
        }

        Cache[modId] = new LocalizationManager(modId);
        return Cache[modId];
    }

    /// <summary>
    /// 设置当前语言
    /// </summary>
    /// <param name="lang">语言枚举值</param>
    public static void SetCurrentLanguage(string lang)
    {
        if (!SupportedLanguages.Contains(lang))
        {
            throw new KeyNotFoundException($"Language {lang} not found.");
        }

        _currentLanguage = lang;
    }

    /// <summary>
    /// 获取本地化字符串
    /// </summary>
    /// /// <param name="modId">mod标识符</param>
    /// <param name="key">识别字符串</param>
    /// <param name="args">参数</param>
    /// <returns>本地化字符串</returns>
    public static string Localize(string modId, string key, params object[] args)
    {
        return Localize(modId, key, _currentLanguage, args);
    }

    /// <summary>
    /// 获取本地化字符串
    /// </summary>
    /// <param name="modId">mod标识符</param>
    /// <param name="key">识别字符串</param>
    /// <param name="lang">目标语言</param>
    /// <param name="args">扩展参数</param>
    /// <returns>本地化字符串</returns>
    private static string Localize(string modId, string key, string? lang = null, params object[] args)
    {
        lang ??= _currentLanguage;
        if (!Localization.TryGetValue(modId, out var modLocalization))
        {
            return string.Format(key, args);
        }

        if (!modLocalization.TryGetValue(lang, out var localization))
        {
            return string.Format(key, args);
        }

        return string.Format(!localization.TryGetValue(key, out var value) ? key : value.LocalizedString, args);
    }
}