using UnityEngine;
using System.Collections;

public class CGameSettingManager : MonoBehaviour {

    #region Singleton

    private static CGameSettingManager m_Instance;
    private static object m_SingtonObject = new object();

    public static CGameSettingManager Instance
    {
        get
        {
            lock (m_SingtonObject)
            {
                if (m_Instance == null)
                {
                    var go = Instantiate(Resources.Load<GameObject>("Prefabs/GameSettingManager"));
                    m_Instance = go.GetComponent<CGameSettingManager>();
                }
                return m_Instance;
            }
        }
    }

    #endregion

    #region Properties

    public const string SKIP_SHOT_COUNT = "skip_shot_count";
    public const string BANK_SHOT_COUNT = "bank_shot_count";
    public const string NICE_SHOT_COUNT = "nice_shot_count";
    public const string LUCKY_CATCH_COUNT = "lucky_catch_count";
    public const string BEST_SCORE = "best_score";
    public const string SOUND_MUTE = "sound_mute";
    public const string SOUND_BG_VOLUMN = "sound_bg_volumn";
    public const string SOUND_EFFECT_VOLUMN = "sound_effect_volumn";

    #endregion

    #region Implementation MonoBehaviour

    void Awake()
    {
        m_Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    void OnApplicationQuit() {
        PlayerPrefs.Save();
    }

    #endregion

    #region Getter && Setter

    public void SetSoundEffectVolumn(float value)
    {
        PlayerPrefs.SetFloat(SOUND_EFFECT_VOLUMN, value);
    }

    public float GetSoundEffectVolumn()
    {
        return PlayerPrefs.GetFloat(SOUND_EFFECT_VOLUMN, 1f);
    }

    public void SetSoundBGVolumn(float value)
    {
        PlayerPrefs.SetFloat(SOUND_BG_VOLUMN, value);
    }

    public float GetSoundBGVolumn()
    {
        return PlayerPrefs.GetFloat(SOUND_BG_VOLUMN, 1f);
    }

    public void SetSoundMute(bool value)
    {
        PlayerPrefs.SetString(SOUND_MUTE, value ? "1" : "0");
    }

    public bool GetSoundMute()
    {
        return PlayerPrefs.GetString(SOUND_MUTE, "0") == "1";
    }

    public void SetBestScore(float score)
    {
        var bestScore = GetBestScore();
        if (bestScore < score)
        {
            PlayerPrefs.SetFloat(BEST_SCORE, Mathf.Clamp(score, 0, 9999));
        }
    }

    public float GetBestScore()
    {
        return PlayerPrefs.GetFloat(BEST_SCORE, 0f); ;
    }

    public int GetNiceShot()
    {
        return PlayerPrefs.GetInt(NICE_SHOT_COUNT, 0);
    }

    public void SetNiceShot(int value)
    {
        var niceShot = GetNiceShot();
        if (niceShot < value)
        {
            PlayerPrefs.SetInt(NICE_SHOT_COUNT, Mathf.Clamp(value, 0, 9999));
        }
    }

    public int GetSkipShot()
    {
        return PlayerPrefs.GetInt(SKIP_SHOT_COUNT, 0);
    }

    public void SetSkipShot(int value)
    {
        var skipShot = GetSkipShot();
        if (skipShot < value)
        {
            PlayerPrefs.SetInt(SKIP_SHOT_COUNT, Mathf.Clamp(value, 0, 9999));
        }
    }

    public int GetBankShot()
    {
        return PlayerPrefs.GetInt(BANK_SHOT_COUNT, 0);
    }

    public void SetBankShot(int value)
    {
        var bankShot = GetBankShot();
        if (bankShot < value)
        {
            PlayerPrefs.SetInt(BANK_SHOT_COUNT, Mathf.Clamp(value, 0, 9999));
        }
    }

    public int GetLuckyCatch()
    {
        return PlayerPrefs.GetInt(LUCKY_CATCH_COUNT, 0);
    }

    public void SetLuckyCatch(int value)
    {
        var luckyCatch = GetLuckyCatch();
        if (luckyCatch < value)
        {
            PlayerPrefs.SetInt(LUCKY_CATCH_COUNT, Mathf.Clamp(value, 0, 9999));
        }
    }

    #endregion

}
