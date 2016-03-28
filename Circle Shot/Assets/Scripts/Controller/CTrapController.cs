using UnityEngine;
using System.Collections;

public class CTrapController : CMovableController
{
    [SerializeField]
    private CEnum.ETrapMoveType m_TrapType;
    [SerializeField]
    private Transform m_Trap;
    [SerializeField]
    private GameObject[] m_MovePoints;
    [SerializeField]
    private Color[] m_TrapColors;

    private SpriteRenderer m_Sprite;
    private Vector3 m_NextPosition;
    private int m_NextIndex;
    private bool m_IsScaling = true;

    public override void Init()
    {
        base.Init();
        LoadAllObject();
        if (m_TrapType != CEnum.ETrapMoveType.NoneMove && m_TrapType != CEnum.ETrapMoveType.ScaleX && m_TrapType != CEnum.ETrapMoveType.Rotation)
        {
            m_NextPosition = m_Trap.position = m_MovePoints[m_NextIndex].transform.position;
            var localScale = m_Trap.localScale;
            localScale.x = m_GameManager.TrapScaleMinSize;
            m_Trap.localScale = localScale;
            m_IsScaling = false;
        }
    }

    public override void Start()
    {
        base.Start();
        Init();
    }

    public override void Update()
    {
        base.Update();
        switch (m_TrapType)
        {
            default:
            case CEnum.ETrapMoveType.NoneMove:
                // TODO
                break;
            case CEnum.ETrapMoveType.MovePoint:
                MoveToPoint();
                break;
            case CEnum.ETrapMoveType.MoveAndRotatonPoint:
                MoveAndRotationToPoint();
                break;
            case CEnum.ETrapMoveType.Rotation:
                RotationPoint();
                break;
            case CEnum.ETrapMoveType.ScaleX:
                ScaleToPoint();
                break;
        }
    }

    public override void OnEnable()
    {
        base.OnEnable();
        m_NextPosition = m_MovePoints[m_NextIndex].transform.position;
    }

    private void LoadAllObject() {
        m_Sprite = m_Trap.GetComponent<SpriteRenderer>();
        if (m_TrapColors.Length != 0)
        {
            m_Sprite.color = m_TrapColors[Random.Range(0, 9999) % m_TrapColors.Length];
        }
    }

    private bool m_SwapPoint = true;
    private void MoveToPoint() {
        var direction = m_NextPosition - m_Trap.transform.position;
        if (direction.sqrMagnitude <= 0.025f)
        {
            if (m_NextIndex >= 0 && m_SwapPoint == true)
            {
                m_NextIndex = (m_NextIndex + 1) % m_MovePoints.Length;
                if (m_NextIndex >= m_MovePoints.Length - 1)
                {
                    m_SwapPoint = false;
                }
            }
            else if (m_NextIndex <= m_MovePoints.Length && m_SwapPoint == false)
            {
                m_NextIndex = m_NextIndex - 1;
                if (m_NextIndex <= 0)
                {
                    m_SwapPoint = true;
                }
            }
            //m_NextIndex = (m_NextIndex + 1) % m_MovePoints.Length;
            m_NextPosition = m_MovePoints[m_NextIndex].transform.position;
        }
        else
        {
            var moveDirection = (direction.normalized * m_GameManager.TrapMoveSpeed * Time.deltaTime) + m_Trap.position;
            m_Trap.position = Vector3.Lerp(m_Trap.position, moveDirection, 0.75f);
        }
    }

    private void MoveAndRotationToPoint()
    {
        MoveToPoint();
        RotationPoint();
    }

    private void RotationPoint() {
        var rotation = m_Trap.transform.rotation.eulerAngles;
        rotation.z += m_GameManager.TrapRotationSpeed;
        m_Trap.transform.rotation = Quaternion.Euler(rotation);
    }

    private void ScaleToPoint()
    {
        var localScale = m_Trap.transform.localScale;
        if (localScale.x <= m_GameManager.TrapScaleMaxSize && m_IsScaling == true)
        {
            localScale.x += m_GameManager.TrapScaleSpeed * Time.deltaTime;
            if (localScale.x >= m_GameManager.TrapScaleMaxSize)
            {
                m_IsScaling = false;
            }
        }
        else if (localScale.x >= m_GameManager.TrapScaleMinSize && m_IsScaling == false)
        {
            localScale.x -= m_GameManager.TrapScaleSpeed * Time.deltaTime;
            if (localScale.x <= m_GameManager.TrapScaleMinSize)
            {
                m_IsScaling = true;
            }
        }
        else
        {
            m_IsScaling = true;
        }
        m_Trap.transform.localScale = localScale;
    }

}
