using System.Collections.Generic;
using Newtonsoft.Json;

[System.Serializable]
public class ThemeEntry
{
    public string key;
    public List<string> value;
}

[System.Serializable]
public class ThemesContainer
{
    [JsonProperty("themes")]
    public Dictionary<string, List<string>> Themes { get; set; }
}

[System.Serializable]
public class ThemesContainerForUnity
{
    public List<ThemeEntry> themes;

    public Dictionary<string, List<string>> ToDictionary()
    {
        var dictionary = new Dictionary<string, List<string>>();
        foreach (var entry in themes)
        {
            dictionary[entry.key] = entry.value;
        }
        return dictionary;
    }
}
