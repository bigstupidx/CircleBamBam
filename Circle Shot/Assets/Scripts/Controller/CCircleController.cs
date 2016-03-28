using UnityEngine;
using System.Collections;

public class CCircleController : CMovableController
{
    public int Index;
    public CEnum.ECircleMoveType m_MoveType;
    public SpriteRenderer BigCircle;
    public SpriteRenderer SmallCircle;
    public bool IsBallContact = false;

    [SerializeField]
    private GameObject[] m_MovePoints;
    private Vector3 m_NextPosition;
    private int m_NextIndex = 0;

    public override void Init()
    {
        base.Init();

        if (m_MovePoints.Length > 0) { 
            m_NextPosition = m_MovePoints[m_NextIndex].transform.position;
        }
    }

    public override void OnEnable()
    {
        base.OnEnable();
        IsBallContact = false;
        if (m_MovePoints.Length > 0)
        {
            m_NextPosition = m_MovePoints[m_NextIndex].transform.position;
        }
    }

    public override void OnDisable()
    {
        base.OnDisable();
        IsBallContact = false;
    }

    public void UpdateCircles(int index, float dt, float offset)
    {
        switch (m_MoveType)
        {
            default:
            case CEnum.ECircleMoveType.RotationCenter:
                RotationCircle(index, dt, offset);
                break;
            case CEnum.ECircleMoveType.MoveAndRotation:
                RotationCircle(index, dt, offset);
                MoveToToPoint(index, dt);
                break;
        }
    }

    private void RotationCircle(int index, float dt, float offset) {
        var rotation = m_Transform.rotation.eulerAngles;
        rotation.z += m_GameManager.CircleRotationSpeed * ((float)index % 2 == 0 ? 1f : -1) * offset;
        m_Transform.rotation = Quaternion.Euler(rotation);
    }

    private void MoveToToPoint(int index, float dt)
    {
        if (IsBallContact)
            return;
        if (m_MovePoints.Length == 0)
            return;
        var direction = m_NextPosition - m_Transform.position;
        if (direction.sqrMagnitude <= 0.1f * 0.1f)
        {
            m_NextIndex = (m_NextIndex + 1) % m_MovePoints.Length;
            m_NextPosition = m_MovePoints[m_NextIndex].transform.position;
        }
        else
        {
            var moveDirection = (direction.normalized * m_GameManager.CircleMoveSpeed * dt) + m_Transform.position;
            m_Transform.position = Vector3.Lerp (m_Transform.position, moveDirection, 0.75f);
        }
    }

}
