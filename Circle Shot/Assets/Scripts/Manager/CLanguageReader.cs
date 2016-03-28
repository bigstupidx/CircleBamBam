using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MiniJSON;

public class CLanguageReader {

    private Dictionary<string, Dictionary<string, string>> m_LanguageData;

    public CLanguageReader()
    {
        m_LanguageData = new Dictionary<string, Dictionary<string, string>>();
    }

    public void ReadLanguageFiles() {
        var resources = Resources.LoadAll<TextAsset>("Languages");
        for (int i = 0; i < resources.Length; i++)
        {
            var languageData = Json.Deserialize(resources[i].text);
            ReadLanguageData(languageData as Dictionary<string, object>);
        }
    }

    private void ReadLanguageData(Dictionary<string, object> data)
    {
        var languageType = data["Language"].ToString();
        var languageList = data["List"] as Dictionary<string, object>;
        m_LanguageData[languageType] = new Dictionary<string, string>();
        m_LanguageData[languageType]["SCORE"] = languageList["SCORE"].ToString();
        m_LanguageData[languageType]["BEST_SCORE"] = languageList["BEST_SCORE"].ToString();
        m_LanguageData[languageType]["PLAY"] = languageList["PLAY"].ToString();
        m_LanguageData[languageType]["PLAY_AGAIN"] = languageList["PLAY_AGAIN"].ToString();
        m_LanguageData[languageType]["START"] = languageList["START"].ToString();
        m_LanguageData[languageType]["SETTING"] = languageList["SETTING"].ToString();
    }

    public string GetString(string language, string name)
    {
        return m_LanguageData[language][name];
    }

}
