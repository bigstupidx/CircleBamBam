using UnityEngine;

public class CObjectController : MonoBehaviour {

    protected CGameManager m_GameManager;
    protected Transform m_Transform;
    protected CCameraController m_CameraController;

    public CEnum.EGameType GameType;
    public float XSize;
    public float YSize;
    public bool IsUsed;

    public virtual void Init()
    {
        m_GameManager = CGameManager.Instance;
        m_Transform = this.transform;
        m_CameraController = CCameraController.Instance;
    }

    public virtual void OnEnable()
    {

    }

    public virtual void OnDisable()
    {

    }

    public virtual void Start () {
	
	}

    public virtual void FixedUpdate () {

	}

    public virtual void Update()
    {

    }

    public virtual void LateUpdate()
    {

    }

    public virtual void OnEndActionObject()
    {

    }

    public virtual void OnResetObject() {

    }

    public virtual void SetActive(bool value)
    {
        this.gameObject.SetActive(value);
    }

}
