using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class CUIManager : MonoBehaviour {

    #region Singleton

    private static CUIManager m_Instance;
    private static object m_SingtonObject = new object();

    public static CUIManager Instance
    {
        get
        {
            lock (m_SingtonObject)
            {
                if (m_Instance == null)
                {
                    var go = Instantiate(Resources.Load<GameObject>("Prefabs/UIManager"));
                    m_Instance = go.GetComponent<CUIManager>();
                }
                return m_Instance;
            }
        }
    }

    #endregion

    #region Propeties

    public Action OnStart;

    [Header("Language")]
    [SerializeField]
    private CEnum.ELanguage m_Language = CEnum.ELanguage.EN;

    [Header("Start Scene")]
    [SerializeField]
    private GameObject m_StartSceneObject;
    [SerializeField]
    private Button m_PlayButton;
    [SerializeField]
    private Button m_SettingButton;
    [SerializeField]
    private Button m_CreditButton;
    [SerializeField]
    private Button m_ArchievementButton;
    [SerializeField]
    private Button m_BackCreditButton;
    [SerializeField]
    private GameObject m_CreditPanel;
    [SerializeField]
    private Button m_ShareFBButton;
    [SerializeField]
    private Button m_MuteSoundButton;
    [SerializeField]
    private Button m_OpenLeaderboardButton;
    [SerializeField]
    private Button m_CloseLeaderboardButton;
    [SerializeField]
    private GameObject m_LeaderBoardPanel;
    [SerializeField]
    private Button m_CloseArchievementButton;
    [SerializeField]
    private GameObject m_ArchievementPanel;
    [SerializeField]
    private Text m_NiceShotText;
    [SerializeField]
    private Text m_BankShotText;
    [SerializeField]
    private Text m_SkipShotText;
    [SerializeField]
    private Text m_LuckyCatchText;
    [SerializeField]
    private GameObject m_SettingPanel;
    [SerializeField]
    private Slider m_SettingBGSlider;
    [SerializeField]
    private Slider m_SettingEffectSlider;
    [SerializeField]
    private Button m_BackSettingButton;
    [SerializeField]
    private RawImage[] m_LeaderboardImages;
    [SerializeField]
    private Text[] m_LeaderboardUserNames;
    [SerializeField]
    private Text[] m_LeaderboardScores;

    [Header("Main Scene")]
    [SerializeField]
    private GameObject m_MainSceneObject;
    [SerializeField]
    private GameObject m_PlayAgainPanel;
    [SerializeField]
    private Text m_CurrentScoreText;
    [SerializeField]
    private Text m_TotalScoreText;
    [SerializeField]
    private Text m_BestScoreText;
    [SerializeField]
    private Button m_PlayAgainButton;
    [SerializeField]
    private Button m_ShareScoreFBButton;
    [SerializeField]
    private GameObject m_PausePanel;
    [SerializeField]
    private Button m_BackStartScreenButton;
    [SerializeField]
    private Button m_PauseButton;
    [SerializeField]
    private Button m_ResumeButton;
    [SerializeField]
    private Button m_RestartButton;
    [SerializeField]
    private Button m_QuitButton;

    [Header("Test")]
    [SerializeField]
    private Text m_ErrorText;

    private CGameSettingManager m_GameSetting;
    private CSceneManager m_SceneManager;
//    private CEnum.ELanguage m_CurrentLanguage;
    private CSocialNetworkManager m_SocialManager;
    private CSoundManager m_SoundManager;
    private static WaitForSeconds m_Waiting;
    private static Sprite[] m_SpriteResources;

    private static float m_ScoreToShare;

    #endregion

    #region Monobehaviour

    private void Awake() {
        m_Instance = this;
    }

    private void Start()
    {
        //m_LanguageReader = new CLanguageReader();
        //m_LanguageReader.ReadLanguageFiles();
        //m_CurrentLanguage = m_Language;
        m_SpriteResources = Resources.LoadAll<Sprite>("Images/UI");
        SetupUI();
        m_Waiting = new WaitForSeconds(2f);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Menu))
        {
            if (m_SceneManager.CheckScene(CEnum.EScene.StartScene))
            {
                m_SettingButton.onClick.Invoke();
            }
            else if (m_SceneManager.CheckScene(CEnum.EScene.MainScene))
            {
                PauseGame();
            }
        }
    }

    private void OnLevelWasLoaded(int index) {
        SetupUI();
        m_ErrorText.gameObject.SetActive(false);
    }

    #endregion

    #region Main methods

    private void SetupUI()
    {
        m_SceneManager = CSceneManager.Instance;
        m_SocialManager = CSocialNetworkManager.Instance;
        m_SoundManager = CSoundManager.Instance;
        m_GameSetting = CGameSettingManager.Instance;

        var isStartScene = m_SceneManager.CheckScene(CEnum.EScene.StartScene);
        var isMainScene = m_SceneManager.CheckScene(CEnum.EScene.MainScene);

        m_StartSceneObject.SetActive(isStartScene);
        m_MainSceneObject.SetActive(isMainScene);

        m_PlayButton.onClick.RemoveListener(PlayMainScene);
        m_PlayButton.onClick.AddListener(PlayMainScene);

        m_CreditButton.onClick.RemoveListener(OpenCredit);
        m_CreditButton.onClick.AddListener(OpenCredit);
        m_BackCreditButton.onClick.RemoveListener(OpenCredit);
        m_BackCreditButton.onClick.AddListener(OpenCredit);

        m_ArchievementButton.onClick.RemoveListener(OpenArchievement);
        m_ArchievementButton.onClick.AddListener(OpenArchievement);
        m_CloseArchievementButton.onClick.RemoveListener(OpenArchievement);
        m_CloseArchievementButton.onClick.AddListener(OpenArchievement);

        m_SettingButton.onClick.RemoveListener(OpenSetting);
        m_SettingButton.onClick.AddListener(OpenSetting);
        m_BackSettingButton.onClick.RemoveListener(OpenSetting);
        m_BackSettingButton.onClick.AddListener(OpenSetting);

        m_SettingBGSlider.onValueChanged.RemoveListener(OnChangeBGVolumn);
        m_SettingBGSlider.onValueChanged.AddListener(OnChangeBGVolumn);
        m_SettingEffectSlider.onValueChanged.RemoveListener(OnChangeEffectVolumn);
        m_SettingEffectSlider.onValueChanged.AddListener(OnChangeEffectVolumn);

        m_MuteSoundButton.onClick.RemoveListener(TurnSound);
        m_MuteSoundButton.onClick.AddListener(TurnSound);

        m_OpenLeaderboardButton.onClick.RemoveListener(OpenLeaderboard);
        m_OpenLeaderboardButton.onClick.AddListener(OpenLeaderboard);

        m_CloseLeaderboardButton.onClick.RemoveListener(OpenLeaderboard);
        m_CloseLeaderboardButton.onClick.AddListener(OpenLeaderboard);

        m_PlayAgainButton.onClick.RemoveListener(PlayMainScene);
        m_PlayAgainButton.onClick.AddListener(PlayMainScene);

        m_ShareScoreFBButton.onClick.RemoveListener(ShareScore);
        m_ShareScoreFBButton.onClick.AddListener(ShareScore);

        m_ShareFBButton.onClick.RemoveListener(ShareLinkFB);
        m_ShareFBButton.onClick.AddListener(ShareLinkFB);

        m_BackStartScreenButton.onClick.RemoveListener(PlayStartScreeen);
        m_BackStartScreenButton.onClick.AddListener(PlayStartScreeen);

        m_PauseButton.onClick.RemoveListener(PauseGame);
        m_PauseButton.onClick.AddListener(PauseGame);

        m_ResumeButton.onClick.RemoveListener(ResumeGame);
        m_ResumeButton.onClick.AddListener(ResumeGame);

        m_RestartButton.onClick.RemoveListener(RestartGame);
        m_RestartButton.onClick.AddListener(RestartGame);

        m_QuitButton.onClick.RemoveListener(QuitGame);
        m_QuitButton.onClick.AddListener(QuitGame);

        //m_PlayAgainButtonText.text = m_LanguageReader.GetString(m_CurrentLanguage.ToString(), "PLAY_AGAIN");

        //var scoreStr = m_LanguageReader.GetString(m_CurrentLanguage.ToString(), "SCORE");
        //m_CurrentScoreText.text = scoreStr + " : 0";
        //m_TotalScoreText.text = scoreStr + " : 0";
        //var bestScoreStr = m_LanguageReader.GetString(m_CurrentLanguage.ToString(), "BEST_SCORE");
        //m_BestScoreText.text = bestScoreStr + " : 0";
        m_CurrentScoreText.text = "0";
        m_TotalScoreText.text = "0";
        m_BestScoreText.text = "0";

        m_MuteSoundButton.image.sprite = m_GameSetting.GetSoundMute() ? GetSpriteResources("open_button_sound_off") : GetSpriteResources("open_button_sound");

        ShowPlayAgainPanel(0f, 0f, false);
        m_PausePanel.SetActive(false);
        m_LeaderBoardPanel.SetActive(false);
        m_ArchievementPanel.SetActive(false);

        m_ScoreToShare = 0;
    }

    private void PlayMainScene()
    {
        m_SceneManager.LoadSceneAsync(CEnum.EScene.MainScene);
    }

    private void PlayStartScreeen()
    {
        m_SceneManager.LoadScene(CEnum.EScene.StartScene);
    }

    private void TurnSound()
    {
        m_SoundManager.IsMute = !m_SoundManager.IsMute;
        // open_button_sound  open_button_sound_off
        m_MuteSoundButton.image.sprite = m_SoundManager.IsMute ? GetSpriteResources("open_button_sound_off") : GetSpriteResources("open_button_sound");
    }

    public void ShowPlayAgainPanel(float score, float bestScore, bool show)
    {
        m_ScoreToShare = score;
        SetTotalScore(score);
        SetBestScore(bestScore);
        m_PlayAgainPanel.SetActive(show);
    }

    private void OpenCredit()
    {
        var active = !m_CreditPanel.activeInHierarchy;
        m_CreditPanel.SetActive(active);
    }

    private void OpenArchievement()
    {
        var active = !m_ArchievementPanel.activeInHierarchy;
        m_ArchievementPanel.SetActive(active);

        if (active)
        {
            m_NiceShotText.text = m_GameSetting.GetNiceShot().ToString();
            m_BankShotText.text = m_GameSetting.GetBankShot().ToString();
            m_SkipShotText.text = m_GameSetting.GetSkipShot().ToString();
            m_LuckyCatchText.text = m_GameSetting.GetLuckyCatch().ToString();
        }
    }

    private void OpenSetting()
    {
        var active = !m_SettingPanel.activeInHierarchy;
        m_SettingPanel.SetActive(active);

        m_SettingBGSlider.value = m_GameSetting.GetSoundBGVolumn();
        m_SettingEffectSlider.value = m_GameSetting.GetSoundEffectVolumn();
    }

    private void OnChangeBGVolumn(float value)
    {
        m_SoundManager.BGVolumn = value;
    }

    private void OnChangeEffectVolumn(float value)
    {
        m_SoundManager.SoundEffectVolumn = value;
    }

    private void OpenLeaderboard() {
        var open = !m_LeaderBoardPanel.activeInHierarchy;
        m_LeaderBoardPanel.SetActive(open);
        if (open)
        {
            m_SocialManager.GetLeaderboard(4, (results) =>
            {
                if (results.Count > 0)
                {
                    for (int i = 0; i < results.Count; i++)
                    {
                        var score = CScoreParser.Parser(results[i] as Dictionary<string, object>);
                        m_LeaderboardUserNames[i].text = score.NameUser;
                        m_LeaderboardScores[i].text = score.Score.ToString();
                        var image = m_LeaderboardImages[i];
                        m_SocialManager.GetCoverImage(CSocialNetworkManager.CoverImageType.small, score.ID, (url) =>
                        {
                            m_SocialManager.CreateTexture(url, (coverImage) =>
                            {
                                image.texture = coverImage;
                            }, true, score.ID);
                        });
                    }
                    m_SocialManager.GetMeScore((score) =>
                    {
                        var i = 4;
                        m_LeaderboardUserNames[i].text = score.NameUser;
                        m_LeaderboardScores[i].text = score.Score.ToString();
                        var image = m_LeaderboardImages[i];
                        m_SocialManager.GetCoverImage(CSocialNetworkManager.CoverImageType.small, score.ID, (url) =>
                        {
                            m_SocialManager.CreateTexture(url, (coverImage) =>
                            {
                                image.texture = coverImage;
                            }, true, score.ID);
                        });
                    });
                }
            });
        }
    }

    private void ShareLinkFB()
    {
        m_SocialManager.ShareLinkFB();
    }

    private void ShareScore()
    {
        m_SocialManager.SetNewScore(m_ScoreToShare, (result) =>
        {
            if (result.IndexOf("success") != -1)
            {
                m_SceneManager.LoadScene(CEnum.EScene.StartScene);
            }
            else
            {
                m_ErrorText.gameObject.SetActive(true);
                m_ErrorText.text = result;
                StartCoroutine(WaitingCloseErrorText());
#if UNITY_EDITOR
                Debug.Log(result);
#endif
            }
        });
    }

    private IEnumerator WaitingCloseErrorText() {
        yield return m_Waiting;
        m_ErrorText.gameObject.SetActive(false);
    }

    public void PauseGame() {
        m_PausePanel.SetActive(true);
        CGameManager.Instance.PauseGame(true);
    }

    private void ResumeGame()
    {
        m_PausePanel.SetActive(false);
        CGameManager.Instance.PauseGame(false);
    }

    private void RestartGame()
    {
        CGameManager.Instance.PauseGame(false);
        PlayMainScene();
    }

    private void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        CGameManager.Instance.PauseGame(false);
        PlayMainScene();
#endif
    }

    #endregion

    #region Getter && Setter

    public void SetCurrentScore(float score)
    {
        //var scoreStr = m_LanguageReader.GetString(m_CurrentLanguage.ToString(), "SCORE");
        //m_CurrentScoreText.text = scoreStr + " : " + score;
        m_CurrentScoreText.text = score.ToString();
    }

    public void SetTotalScore(float score)
    {
        //var scoreStr = m_LanguageReader.GetString(m_CurrentLanguage.ToString(), "SCORE");
        //m_TotalScoreText.text = scoreStr + " : " + score;
        m_TotalScoreText.text = "Score : " + score.ToString();
    }

    public void SetBestScore(float score)
    {
        //var scoreStr = m_LanguageReader.GetString(m_CurrentLanguage.ToString(), "BEST_SCORE");
        //m_BestScoreText.text = scoreStr + " : " + score;
        m_BestScoreText.text = "Best score : " + score.ToString();
    }

    public Sprite GetSpriteResources(string name)
    {
        for (int i = 0; i < m_SpriteResources.Length; i++)
        {
            if (m_SpriteResources[i].name == name)
            {
                return m_SpriteResources[i];
            }
        }
        return null;
    }

    #endregion

}
