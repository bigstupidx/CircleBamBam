using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ObjectPool;

public class CGameManager : MonoBehaviour {

    #region Singleton
    
    private static CGameManager m_Instance;
    private static object m_SingtonObject = new object();

    public static CGameManager Instance {
        get
        {
            lock (m_SingtonObject)
            {
                if (m_Instance == null)
                {
                    var go = Instantiate(Resources.Load<GameObject>("Prefabs/GameManager"));
                    m_Instance = go.GetComponent<CGameManager>();
                }
                return m_Instance;
            }
        }      
    }

    #endregion

    #region Properties

    [Header("Level")]
    public CEnum.ELevel Level = CEnum.ELevel.Level_Normal;
    public int ScoreToLevelUp = 100;
    [SerializeField]
    private float m_NextScoreToLevelUp;

    [Header("Group")]
    public int GroupCount = 5;

    [Header("Object Speed")]
    public float MoveSpeed = 5f;
    public float CircleRotationSpeed = 5f;
    public float CircleRotationOffset = 0.25f;
    public float CircleMoveSpeed = 2f;
    public float TrapMoveSpeed = 2f;
    public float TrapRotationSpeed = 20f;
    public float TrapScaleSpeed = 5f;
    public float TrapScaleMinSize = 10f;
    public float TrapScaleMaxSize = 30f;
    public float CameraSpeed = 5f;
    public float CameraY = 2f;

    [Header("Game Status")]
    public bool HackMode = false;
    public float TimeToFail = 2f;
    public bool LoadingComplete;
    public bool OnPlaying;

    public static string TAG_CIRCLE = "Circle";
    public static string TAG_TRAP = "Trap";
    public static string TAG_DETECT = "Detect";
    public static string TAG_BOUNCE = "Bounce";

    private int m_GroupPrefabCount = 5;
    private static Dictionary<CEnum.EGameType, ObjectPool<CObjectController>> m_GamePool;

    private CSceneManager m_SceneManager;
	private CGroupController m_CurrentGroup;
    private CCameraController m_CameraController;
    private CUIManager m_UIManager;
    private CGameSettingManager m_GameSetting;
    private int m_GroupActiveCount = 0;
    private CBallController m_BallCtrl;

    #endregion

    #region Implementation MonoBehaviour

    private void Awake() {
        m_Instance = this;
        m_GamePool = new Dictionary<CEnum.EGameType, ObjectPool<CObjectController>>();
        LoadObjectPool();
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Application.targetFrameRate = 60;
    }

    private void Start() {
        m_SceneManager = CSceneManager.Instance;
        m_CameraController = CCameraController.Instance;
        m_UIManager = CUIManager.Instance;
        m_GameSetting = CGameSettingManager.Instance;
        m_NextScoreToLevelUp = ScoreToLevelUp;
#if UNITY_EDITOR
        HackMode = false;
#endif
    }

    private void OnLevelWasLoaded(int index)
    {
        m_NextScoreToLevelUp = ScoreToLevelUp;
#if UNITY_EDITOR
        HackMode = false;
#endif
    }

    #endregion

    #region Main method

    private IEnumerator HandleLoadObjectPool(System.Action complete = null) {
        var groundNormalPrefabs = Resources.LoadAll<GameObject>("Group Normal");
        for (int i = 0; i < groundNormalPrefabs.Length; i++)
        {
            var index = CEnum.EGameType.GroupNormal + 1 + i;
            m_GamePool[index] = new ObjectPool<CObjectController>();
            for (int j = 0; j < 5; j++)
            {
                var groundController = Instantiate(groundNormalPrefabs[i]).GetComponent<CGroupController>();
                groundController.transform.SetParent(this.transform);
                groundController.SetActive(false);
                groundController.GameType = index;
                groundController.Init();
                groundController.name = index + "_" + (i * 5 + j);
                m_GamePool[index].Create(groundController);
                yield return groundController != null;
            }
        }

        var groundMediumPrefabs = Resources.LoadAll<GameObject>("Group Medium");
        for (int i = 0; i < groundMediumPrefabs.Length; i++)
        {
            var index = CEnum.EGameType.GroupMedium + 1 + i;
            m_GamePool[index] = new ObjectPool<CObjectController>();
            for (int j = 0; j < 5; j++)
            {
                var groundController = Instantiate(groundMediumPrefabs[i]).GetComponent<CGroupController>();
                groundController.transform.SetParent(this.transform);
                groundController.SetActive(false);
                groundController.GameType = index;
                groundController.Init();
                groundController.name = index + "_" + (i * 5 + j);
                m_GamePool[index].Create(groundController);
                yield return groundController != null;
            }
        }

        var groundHardPrefabs = Resources.LoadAll<GameObject>("Group Hard");
        for (int i = 0; i < groundHardPrefabs.Length; i++)
        {
            var index = CEnum.EGameType.GroupHard + 1 + i;
            m_GamePool[index] = new ObjectPool<CObjectController>();
            for (int j = 0; j < 5; j++)
            {
                var groundController = Instantiate(groundHardPrefabs[i]).GetComponent<CGroupController>();
                groundController.transform.SetParent(this.transform);
                groundController.SetActive(false);
                groundController.GameType = index;
                groundController.Init();
                groundController.name = index + "_" + (i * 5 + j);
                m_GamePool[index].Create(groundController);
                yield return groundController != null;
            }
        }

        yield return null;
        var ballPrefabs = Resources.LoadAll<GameObject>("Ball");
        m_BallCtrl = Instantiate(ballPrefabs[0]).GetComponent<CBallController>();
        m_GamePool[CEnum.EGameType.Ball] = new ObjectPool<CObjectController>();
        m_BallCtrl.transform.SetParent(this.transform);
        m_BallCtrl.SetActive(false);
        m_BallCtrl.GameType = CEnum.EGameType.Ball;
        m_BallCtrl.Init();
        m_BallCtrl.name = CEnum.EGameType.Ball.ToString();
        m_GamePool[CEnum.EGameType.Ball].Create(m_BallCtrl);
        yield return m_BallCtrl != null;
        if (complete != null)
        {
            complete();
        }
    }

    private void LoadObjectPool() {
        StartCoroutine(HandleLoadObjectPool(LoadFirstObject));
    }

    private void LoadFirstObject()
    {
        LoadingComplete = OnPlaying = true;
        var groupController = LoadObject(CEnum.EGameType.GroupNormal_1);
        groupController.transform.position = Vector3.zero;
        m_CurrentGroup = groupController as CGroupController;
        m_GroupActiveCount ++;
        m_CurrentGroup.Index = m_GroupActiveCount;

        var ballController = LoadObject(CEnum.EGameType.Ball);
        ballController.transform.position = Vector3.zero;
        (ballController as CBallController).SetCircleContain(m_CurrentGroup.FirstCircle);

        m_CameraController.Target = ballController.gameObject;
    }

    public CObjectController LoadRandomGroup(GameObject circle, Vector3 nextPosition) {
        var random = Random.Range(0, 9999) % (m_GroupPrefabCount - 1) + 1;
        return LoadGroup(CEnum.EGameType.GroupNormal + random + (int)Level, circle, nextPosition);
    }

    public CGroupController LoadGroup(CEnum.EGameType objectType, GameObject circle, Vector3 position) {
        var groupCtrl = m_GamePool[objectType].Get() as CGroupController;
        var pos = Vector3.up * (groupCtrl.YSize + position.y);
        m_GroupActiveCount ++;
        groupCtrl.Index = m_GroupActiveCount;
        groupCtrl.transform.position = pos;
        groupCtrl.StartRotationAngle = 180f - circle.transform.rotation.eulerAngles.z;
        groupCtrl.IsUsed = false;
        groupCtrl.SetActive(true);
        groupCtrl.LoadRandomCircle();
        return groupCtrl;
    }

    public CObjectController LoadObject(CEnum.EGameType objectType)
    {
        var objectController = m_GamePool[objectType].Get();
        //var position = Vector3.right * (objectController.YSize + ((offset - SpeedBoost) * Time.fixedDeltaTime) + (offset * Time.fixedDeltaTime));
        objectController.transform.position = Vector3.zero;
        objectController.IsUsed = false;
        objectController.SetActive(true);
        return objectController;
    }

    public void DeleteOldObject(CEnum.EGameType gameType, CObjectController objectController) {
        m_GamePool[gameType].Set(objectController);
        objectController.SetActive(false);
    }

    public void HavePlayerFail(float score = 0f)
    {
        if (OnPlaying == true)
        {
            m_GameSetting.SetBestScore(score);
            StartCoroutine(HandlePlayerFail(() => { ShowPlayAgainPanel(score); }));
            OnPlaying = false;
        }
    }

    private IEnumerator HandlePlayerFail(System.Action complete) {
        var waiting = 0f;
        while (waiting < TimeToFail)
        {
            waiting += Time.deltaTime;
            yield return null;
        }
        if (complete != null) {
            complete();
        }
    }

    private void ShowPlayAgainPanel(float score) {
        m_UIManager.ShowPlayAgainPanel(score, m_GameSetting.GetBestScore(), true);
    }

    public void PauseGame(bool value) {
        Time.timeScale = value ? 0f : 1f;
        OnPlaying = !value;
    }

    #endregion

    #region Getter && Setter

    public void SetCurrentScore(float score)
    {
        if (score > m_NextScoreToLevelUp)
        {
            Level = Level + 100 > CEnum.ELevel.Level_Hard ? CEnum.ELevel.Level_Hard : Level + 100;
            m_NextScoreToLevelUp += ScoreToLevelUp;
        }
    }

    #endregion

}
