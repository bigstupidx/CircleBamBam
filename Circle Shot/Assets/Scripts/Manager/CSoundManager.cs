using UnityEngine;
using System.Collections;

public class CSoundManager : MonoBehaviour {

    #region Singleton

    private static CSoundManager m_Instance;
    private static object m_SingtonObject = new object();

    public static CSoundManager Instance
    {
        get
        {
            lock (m_SingtonObject)
            {
                if (m_Instance == null)
                {
                    var go = Instantiate(Resources.Load<GameObject>("Prefabs/SoundManager"));
                    m_Instance = go.GetComponent<CSoundManager>();
                }
                return m_Instance;
            }
        }
    }

    #endregion

    #region Properties

    [Header("Sound Setting")]
    [SerializeField]
    private bool m_IsMute = false;
    [SerializeField]
    [Range(0f, 1f)]
    private float m_BGVolumn = 1f;
    [SerializeField]
    [Range(0f, 1f)]
    private float m_SoundEffectVolumn = 1f;
    [Header("Sound track")]
    [SerializeField]
    private AudioSource m_SoundTrackSource;
    [SerializeField]
    private AudioClip[] m_Sounds;
    [Header("Sound shot")]
    [SerializeField]
    private AudioSource m_SoundShotSource;
    [SerializeField]
    private AudioClip[] m_SoundShots;
    [Header("Sound Collision Bounce")]
    [SerializeField]
    private AudioSource m_SoundCollisionBounceSource;
    [SerializeField]
    private AudioClip[] m_SoundCollisionBounce;
    [Header("Sound Death")]
    [SerializeField]
    private AudioSource m_SoundDeathSource;
    [SerializeField]
    private AudioClip[] m_SoundDeaths;

    private CSceneManager m_SceneManager;
    private CGameSettingManager m_GameSetting;

    public bool IsMute
    {
        get { return m_IsMute; }
        set {
            m_IsMute = value;
            m_GameSetting.SetSoundMute(value);
        }
    }

    public float BGVolumn
    {
        get { return m_BGVolumn; }
        set
        {
            m_BGVolumn = value;
            m_GameSetting.SetSoundBGVolumn(value);
        }
    }

    public float SoundEffectVolumn
    {
        get { return m_SoundEffectVolumn; }
        set
        {
            m_SoundEffectVolumn = value;
            m_GameSetting.SetSoundEffectVolumn(value);
        }
    }

    #endregion

    #region Monobeahaviour

    void Awake() {
        m_Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        m_GameSetting = CGameSettingManager.Instance;
        m_SceneManager = CSceneManager.Instance;

        m_IsMute = m_GameSetting.GetSoundMute();
    }

    private void OnLevelWasLoaded(int index)
    {
        SetupSound();
    }

    private void LateUpdate() {
        m_SoundTrackSource.mute = m_IsMute;
        m_SoundCollisionBounceSource.mute = m_IsMute;
        m_SoundDeathSource.mute = m_IsMute;
        m_SoundShotSource.mute = m_IsMute;

        m_SoundTrackSource.volume = m_BGVolumn;
        m_SoundCollisionBounceSource.volume = m_SoundEffectVolumn;
        m_SoundDeathSource.volume = m_SoundEffectVolumn;
        m_SoundShotSource.volume = m_SoundEffectVolumn;
    }

    #endregion

    #region Main methods

    private void SetupSound() {
        m_GameSetting = CGameSettingManager.Instance;
        m_SceneManager = CSceneManager.Instance;

        m_SoundTrackSource.clip = null;
        m_SoundCollisionBounceSource.clip = null;
        m_SoundDeathSource.clip = null;
        m_SoundShotSource.clip = null;

        m_IsMute = m_GameSetting.GetSoundMute();
        m_BGVolumn = m_GameSetting.GetSoundBGVolumn();
        m_SoundEffectVolumn = m_GameSetting.GetSoundEffectVolumn();
    }

    public void PlayRandomSoundCollisionBounce() {
        if (m_SoundCollisionBounce.Length <= 0)
            return;
        PlaySoundCollisionBounce(((int)Time.time % m_SoundCollisionBounce.Length));
    }

    public void PlayRandomSoundCollisionCircle()
    {
        if (m_SoundDeaths.Length <= 0)
            return;
        PlaySoundDeath(((int)Time.time % m_SoundDeaths.Length));
    }

    public void PlayShotSound()
    {
        if (m_SoundShots.Length <= 0)
            return;
        m_SoundShotSource.mute = m_IsMute;
        m_SoundShotSource.clip = m_SoundShots[(int)Time.time % m_SoundShots.Length];
        m_SoundShotSource.loop = false;
        m_SoundShotSource.volume = m_SoundEffectVolumn;
        m_SoundShotSource.Play();
    }

    public void PlaySound(int index, bool loop) {
        if (index > m_Sounds.Length - 1 || m_Sounds.Length <= 0)
            return;
        m_SoundTrackSource.mute = m_IsMute;
        m_SoundTrackSource.clip = m_Sounds[index];
        m_SoundTrackSource.loop = loop;
        m_SoundTrackSource.volume = m_BGVolumn;
        m_SoundTrackSource.Play();
    }

    public void PlaySoundCollisionBounce(int index)
    {
        if (index > m_SoundCollisionBounce.Length - 1 || m_SoundCollisionBounce.Length <= 0)
            return;
        m_SoundCollisionBounceSource.mute = m_IsMute;
        m_SoundCollisionBounceSource.clip = m_SoundCollisionBounce[index];
        m_SoundCollisionBounceSource.loop = false;
        m_SoundCollisionBounceSource.volume = m_SoundEffectVolumn;
        m_SoundCollisionBounceSource.Play();
    }

    public void PlaySoundDeath(int index)
    {
        if (index > m_SoundDeaths.Length - 1 || m_SoundDeaths.Length <= 0)
            return;
        m_SoundDeathSource.mute = m_IsMute;
        m_SoundDeathSource.clip = m_SoundDeaths[index];
        m_SoundDeathSource.loop = false;
        m_SoundDeathSource.volume = m_SoundEffectVolumn;
        m_SoundDeathSource.Play();
    }

    #endregion

}
