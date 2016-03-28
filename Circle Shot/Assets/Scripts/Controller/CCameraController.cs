using UnityEngine;
using System.Collections;

public class CCameraController : MonoBehaviour {

    #region Singleton

    private static CCameraController m_Instance;
    private static object m_SingtonObject = new object();

    public static CCameraController Instance
    {
        get
        {
            lock (m_SingtonObject)
            {
                if (m_Instance == null)
                {
                    var go = Instantiate(Resources.Load<GameObject>("Prefabs/CameraController"));
                    m_Instance = go.GetComponent<CCameraController>();
                }
                return m_Instance;
            }
        }
    }

    #endregion

    #region Properties

    public GameObject Target;
    public Camera Camera;
    public Bounds Bounds;

    public float CameraShake = 0.5f;
    private Transform m_Transform;
    private CGameManager m_GameManager;
    private float m_ShakeDuration = 0f;

    #endregion

    #region Monobehaviour

    void Awake()
    {
        m_Instance = this;
        Camera = this.GetComponent<Camera>();
        GetCameraBounds();
    }

    void Start() {
        m_Transform = this.transform;
        m_GameManager = CGameManager.Instance;
    }
	
	void FixedUpdate() {
        GetCameraBounds();
        FollowTarget();
        if (m_ShakeDuration > 0f)
        {
            var newPosition = m_Transform.position;
            newPosition = newPosition + Random.insideUnitSphere * CameraShake;
            newPosition.z = -10f;
            m_Transform.position = newPosition;
            m_ShakeDuration -= Time.fixedDeltaTime;
        }
        else
        {
            m_ShakeDuration = 0f;
        }
    }

    #endregion

    #region Main methods

    public void ShakeIt() {
        m_ShakeDuration = 1f;
    }

    private void FollowTarget() {
        if (Target == null)
            return;
        var position = Target.transform.position;
        var yAxis = Target.transform.position.y + m_GameManager.CameraY;
        position.x = 0f;
        position.y = yAxis;// yAxis < m_Transform.position.y ? m_Transform.position.y : yAxis;
        position.z = m_Transform.position.z;
        position = Vector3.Lerp(m_Transform.position, position, Time.deltaTime * m_GameManager.CameraSpeed);
        m_Transform.position = position;
    }

    public bool CheckInActiveCamera(float x, float y, float width, float height)
    {
        var result = y <= m_Transform.position.y + Bounds.extents.y;
        return result;
    }

    public bool CheckOutOfCamera(float x, float y, float width, float height)
    {
        var result = m_Transform.position.y >= y + Bounds.extents.y + height;
        return result;
    }

    private Bounds GetCameraBounds() {
        var screenAspect = (float)Screen.width / (float)Screen.height;
        var cameraHeight = Camera.orthographicSize * 2;
        var extentsPos = Camera.transform.position;
        extentsPos.x = cameraHeight * screenAspect;
        extentsPos.y = cameraHeight;
        extentsPos.z = 0f;
        Bounds.center = Camera.transform.position;
        Bounds.extents = extentsPos;
        return Bounds;
    }

    #endregion

}
