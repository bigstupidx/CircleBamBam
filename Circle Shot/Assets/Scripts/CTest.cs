using UnityEngine;
using System.Collections;

public class CTest : MonoBehaviour {

    [SerializeField]
    private Transform m_Other;
    [SerializeField]
    private float m_Angle = 0f;
    [SerializeField]
    private float m_OtherAngle = 0f;

    private Transform m_Transform;

    void Start () {
        m_Transform = this.transform;
    }
	
	void Update () {
        if (Input.GetKey(KeyCode.A))
        {
            var rotation = m_Transform.rotation.eulerAngles;
            rotation.z += 3f;
            m_Transform.rotation = Quaternion.Euler(rotation);

            var otherRotation = m_Other.rotation.eulerAngles;
            otherRotation.z -= 3f;
            m_Other.rotation = Quaternion.Euler(otherRotation);
        }
        else
        {
            var direction = m_Other.position - m_Transform.position;
            m_Angle = Vector3.Angle(m_Transform.TransformDirection(Vector3.up), direction);
            var lookAt = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            m_Other.rotation = Quaternion.Euler(0f, 0f, 360f - m_Angle - 180f);
        }
    }
}
