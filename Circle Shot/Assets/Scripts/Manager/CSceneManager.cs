using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class CSceneManager : MonoBehaviour {

    #region Singleton

    private static CSceneManager m_Instance;
    private static object m_SingtonObject = new object();

    public static CSceneManager Instance
    {
        get
        {
            lock (m_SingtonObject)
            {
                if (m_Instance == null)
                {
                    var go = Instantiate(Resources.Load<GameObject>("Prefabs/SceneManager"));
                    m_Instance = go.GetComponent<CSceneManager>();
                }
                return m_Instance;
            }
        }
    }

    #endregion

    #region Properties
    
    private static CEnum.EScene m_Scene = CEnum.EScene.StartScene;
    private CUIManager m_UIManager;
    private static Dictionary<CEnum.EScene, string> m_SceneSetting = new Dictionary<CEnum.EScene, string>();

    #endregion

    #region Monobehaviour

    void Awake() {
        m_Instance = this;
        m_SceneSetting.Clear();
        m_SceneSetting.Add(CEnum.EScene.StartScene, "StartScene");
        m_SceneSetting.Add(CEnum.EScene.MainScene, "MainScene");
    }

    private void Start()
    {
        SetupScene(m_Scene);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (m_Scene == CEnum.EScene.StartScene)
            {
                Application.Quit();
            }
            else if (m_Scene == CEnum.EScene.MainScene)
            {
                LoadScene(CEnum.EScene.StartScene);
            }
        }
    }

    void OnApplicationFocus(bool focusStatus)
    {
        if (m_Scene == CEnum.EScene.MainScene)
        {
            if (focusStatus == false)
            {
                m_UIManager.PauseGame();
            }
        }
    }

    private void OnLevelWasLoaded(int index)
    {
        SetupScene(m_Scene);
    }

    #endregion

    #region Main methods

    private void SetupScene(CEnum.EScene scene)
    {
        m_UIManager = CUIManager.Instance;
        if (m_Scene == CEnum.EScene.StartScene)
        {
            var social = CSocialNetworkManager.Instance;
            var sound = CSoundManager.Instance;
            var setting = CGameSettingManager.Instance;

            CSoundManager.Instance.PlaySound(0, true);
        }
        else if (m_Scene == CEnum.EScene.MainScene)
        {
            var game = CGameManager.Instance;
            var camera = CCameraController.Instance;

            CSoundManager.Instance.PlaySound(1, true);
        }
    }

    public bool CheckScene(CEnum.EScene scene)
    {
        return m_Scene == scene;
    }

    public void LoadScene(CEnum.EScene scene)
    {
        m_Scene = scene;
        SceneManager.LoadScene(m_SceneSetting[scene]);
    }

    public void LoadSceneAsync(CEnum.EScene scene)
    {
        m_Scene = scene;
        SceneManager.LoadSceneAsync(m_SceneSetting[scene]);
    }

    #endregion
}
