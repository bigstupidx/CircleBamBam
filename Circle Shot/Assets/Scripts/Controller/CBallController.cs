using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class CBallController : CMovableController {

    #region Properties

    protected CircleCollider2D m_Collider2D;

	public CCircleController CirleContain;
    public float m_Score;

    [SerializeField]
    private GameObject m_Ball;
    [Header("Particle Effect")]
    [SerializeField]
    private GameObject m_Line;
    [SerializeField]
    private ParticleSystem m_Trail;
    [SerializeField]
    private ParticleSystem m_Death;
    [SerializeField]
    private GameObject m_NiceShot;
    [SerializeField]
    private GameObject m_SkipShot;
    [SerializeField]
    private GameObject m_LuckyCatch;
    [SerializeField]
    private GameObject m_BankShot;
    [SerializeField]
    private ParticleSystem m_Contact;
    [SerializeField]
    private bool m_IsEnable = false;

    private bool m_IsShot;
    private bool m_IsBounce;
    private int m_CircleCurrentIndex = 999999;
    private CUIManager m_UIManager;
    private CSoundManager m_SoundManager;
    private CGameSettingManager m_GameSetting;
    private WaitForSeconds m_Waiting;
    private int m_NiceShotCount = 0;
    private int m_BankShotCount = 0;
    private int m_SkipShotCount = 0;
    private int m_LuckyCatchCount = 0;

    private bool m_NeedShowText;
    private float m_CountDownText = 1f;
    private bool m_FirstShow = false;

    #endregion

    #region Mono behaviour

    public override void Init ()
	{
        base.Init();
        m_Collider2D = this.GetComponent<CircleCollider2D>();
        m_Trail = m_Ball.GetComponent<ParticleSystem>();
        m_UIManager = CUIManager.Instance;
        m_SoundManager = CSoundManager.Instance;
        m_GameSetting = CGameSettingManager.Instance;
        m_IsEnable = false;
        m_Death.playOnAwake = false;
        m_Contact.playOnAwake = false;
        m_NeedShowText = false;
        m_Waiting = new WaitForSeconds(0.3f);
        m_NiceShot = GameObject.Find("Nice Shot");
        m_BankShot = GameObject.Find("Bank Shot");
        m_SkipShot = GameObject.Find("Skip Shot");
        m_LuckyCatch = GameObject.Find("Lucky Catch");
        SetActiveText(false);
        m_FirstShow = false;
    }

    public override void Update()
	{
		base.Update();
        if (m_IsEnable == false)
        {
            m_Rigidbody2D.velocity = Vector2.zero;
        }
        if (m_GameManager.OnPlaying == false)
        {
            return;
        }
        if (CirleContain != null)
        {
            m_Transform.position = CirleContain.transform.position;// Vector3.Lerp(m_Transform.position, CirleContain.transform.position, 1f);
            m_Transform.rotation = CirleContain.transform.rotation;
            m_Rigidbody2D.velocity = Vector2.zero;
        }

        if (IsUsed && ActiveInput())
        {
            Shoot();
        }

        if (m_IsEnable && m_CameraController.CheckOutOfCamera(m_Transform.position.x, m_Transform.position.y, m_Collider2D.radius, m_Collider2D.radius))
        {
            m_IsEnable = false;
            m_GameManager.HavePlayerFail(m_Score);
            m_SoundManager.PlaySoundDeath(1);
            m_NiceShotCount = 0;
        }
    }

    public override void LateUpdate() {
        base.LateUpdate();
        m_Collider2D.isTrigger = false;

        if (m_NeedShowText)
        {
            m_CountDownText -= Time.deltaTime;
            if (m_CountDownText <= 0f)
            {
                m_CountDownText = 1f;
                m_NeedShowText = false;
                SetActiveText(false);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.name.IndexOf (CGameManager.TAG_DETECT) != -1 && CirleContain == null) {
            m_SoundManager.PlayShotSound();
            SetCircleContain(other.transform.parent.gameObject.GetComponent<CCircleController>());
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
#if UNITY_EDITOR
        if (other.gameObject.name.IndexOf(CGameManager.TAG_CIRCLE) != -1 && CirleContain == null && m_GameManager.HackMode)
        {
            m_SoundManager.PlayShotSound();
            SetCircleContain(other.gameObject.GetComponent<CCircleController>());
        }
#endif
        if (CirleContain == null && other.gameObject.name.IndexOf(CGameManager.TAG_TRAP) != -1)
        {
            m_Ball.SetActive(false);
            m_Death.Play();
            m_CameraController.ShakeIt();
            m_Rigidbody2D.isKinematic = true;
            m_GameManager.HavePlayerFail(m_Score);
            m_SoundManager.PlaySoundDeath(0);
        }
        if (other.gameObject.name.IndexOf (CGameManager.TAG_BOUNCE) != -1)
        {
            m_SoundManager.PlayRandomSoundCollisionBounce();
            m_IsBounce = true;
        }
    }

    #endregion

    #region Main methods

    private void SetActiveText(bool value)
    {
        m_NiceShot.SetActive(value);
        m_SkipShot.SetActive(value);
        m_LuckyCatch.SetActive(value);
        m_BankShot.SetActive(value);

        var position = m_Transform.position;
        position.y += 1.5f;
        m_NiceShot.transform.position = position;
        m_SkipShot.transform.position = position;
        m_LuckyCatch.transform.position = position;
        m_BankShot.transform.position = position;
    }

    private bool ActiveInput() {
        var result = false;
        if (Application.isMobilePlatform == false)
        {
            var isTouchedUI = EventSystem.current.IsPointerOverGameObject();
            result = (Input.GetKeyDown(KeyCode.A) || Input.GetMouseButtonDown(0)) && isTouchedUI == false;
        }
        else if (Input.touchCount == 1)
        {
            var tap = Input.GetTouch(0);
            switch (tap.phase)
            {
                case TouchPhase.Began:
                    var isTouchedUI = EventSystem.current.IsPointerOverGameObject(tap.fingerId);
                    result = isTouchedUI == false;
                    break;
            }
        }
        return result;
    }

    public void Shoot()
    {
        m_Collider2D.isTrigger = false;
        m_Rigidbody2D.isKinematic = IsUsed = false;
        var direction = CirleContain.transform.TransformDirection (Vector3.up * m_GameManager.MoveSpeed);
        m_Rigidbody2D.AddForce ((Vector2)direction, ForceMode2D.Impulse);
        CirleContain.IsBallContact = false;
        CirleContain = null;
        m_Line.SetActive(IsUsed);
        m_IsBounce = false;
        m_IsShot = true;
        m_Trail.enableEmission = true;
    }

	public void SetCircleContain(CCircleController value)
    {
        CirleContain = value;
        m_Collider2D.isTrigger = true;
        CirleContain.IsBallContact = true;
        m_Rigidbody2D.isKinematic = true;
        CalculateScore(value);
        StartCoroutine(WaitingAlready());
        m_IsShot = false;
        m_Trail.enableEmission = false;
        if (m_FirstShow)
        {
            m_Contact.Play();
        }
        m_FirstShow = true;
    }

    private IEnumerator WaitingAlready() {
        yield return m_Waiting;
        IsUsed = true;
        m_Line.SetActive(IsUsed);
    }

    private void CalculateScore(CCircleController value)
    {
        if (value.Index < m_CircleCurrentIndex - 1 && m_IsShot == true)
        {
#if UNITY_EDITOR
            Debug.Log("Lucky catch");
#endif
            m_Score += 50f;
            SetActiveText(false);
            m_LuckyCatch.SetActive(true);
            m_NeedShowText = true;
            m_LuckyCatchCount++;
            m_GameSetting.SetLuckyCatch(m_LuckyCatchCount);
        }
        else
        {
            if (value.Index > m_CircleCurrentIndex + 1)
            {
#if UNITY_EDITOR
                Debug.Log("Skip Shot");
#endif
                var score = (value.Index - m_CircleCurrentIndex + 1) * 10;
                m_Score += score <= 30f ? 30f : score;
                SetActiveText(false);
                m_SkipShot.SetActive(true);
                m_NeedShowText = true;
                m_SkipShotCount++;
                m_GameSetting.SetSkipShot(m_SkipShotCount);
            }
            else if (value.Index == m_CircleCurrentIndex + 1 && m_IsBounce == false)
            {
#if UNITY_EDITOR
                Debug.Log("Shot");
#endif
                m_Score += 10f;
                m_NiceShotCount++;
                if (m_NiceShotCount % 3 == 0)
                {
                    SetActiveText(false);
                    m_NiceShot.SetActive(true);
                    m_NeedShowText = true;
                }
                m_GameSetting.SetNiceShot(m_NiceShotCount);
            } else if (value.Index == m_CircleCurrentIndex + 1 && m_IsBounce == true)
            {
#if UNITY_EDITOR
                Debug.Log("Bank Shot");
#endif
                m_Score += 20f;
                SetActiveText(false);
                m_BankShot.SetActive(true);
                m_NeedShowText = true;
                m_BankShotCount++;
                m_GameSetting.SetBankShot(m_BankShotCount);
            }
        }
        m_CircleCurrentIndex = value.Index;
        m_UIManager.SetCurrentScore(m_Score);
        m_GameManager.SetCurrentScore(m_Score);
        m_IsBounce = false;
    }

    public override void SetActive(bool value)
    {
        base.SetActive(value);
        m_IsEnable = value;
        m_Line.SetActive(value);
    }

    #endregion

}
