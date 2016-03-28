using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ParticleSystem))]
public class LineParticle : MonoBehaviour {

    [SerializeField]
    [Range(10, 1000)]
    private int m_Resolution = 10;
    [SerializeField]
    private float m_Size = 0.25f;
    [SerializeField]
    private Gradient m_ColorLine;
    [SerializeField]
    private Vector3[] m_Points;

    private int m_CurrentResolution = 0;
    private ParticleSystem m_ParticleSystem;
    private ParticleSystem.Particle[] m_ParticlePoints;
    private Transform m_Transform;

    private void OnEnable() {
        if (m_ParticleSystem != null)
        {
            DrawPoint();
        }
    }

    private void OnDisable() {
        if (m_ParticleSystem != null)
        {
            m_ParticleSystem.Stop();
        }
    }

    private void Awake() {
        m_Transform = this.GetComponent<Transform>();
        m_ParticleSystem = this.GetComponent<ParticleSystem>();
        m_ParticleSystem.loop = false;
        //m_ParticleSystem.scalingMode = ParticleSystemScalingMode.Shape;
        m_ParticleSystem.playOnAwake = false;
        CreatePoints();
    }

    private void Update()
    {
        if (m_CurrentResolution != m_Resolution || m_ParticlePoints == null)
        {
            CreatePoints();
        }
        m_Transform.rotation = Quaternion.identity;
        m_ParticleSystem.SetParticles(m_ParticlePoints, m_ParticlePoints.Length);
    }

    public void CreatePoints()
    {
        m_CurrentResolution = m_Resolution;
        m_ParticlePoints = new ParticleSystem.Particle[m_Resolution * (m_Points.Length - 1)];
        DrawPoint();
    }

    public void DrawPoint() {
        if (m_Points.Length <= 1)
            return;
        var segment = m_Resolution / (m_Points.Length - 1);
        for (int i = 0, j = 1; i < m_Points.Length; i++, j = j + 1 > m_Points.Length - 1 ? j = 0 : j + 1)
        {
            var point1 = m_Points[i];
            var point2 = m_Points[j];
            var direction = (point2 - point1) / m_Resolution;
            for (int x = m_Resolution * i, y = 0; x < m_Resolution * j; x++, y++)
            {
                m_ParticlePoints[x].position = point1 + (direction * y) - m_Transform.position;
                m_ParticlePoints[x].startColor = m_ColorLine.Evaluate((float)x / m_ParticlePoints.Length);
                m_ParticlePoints[x].startSize = m_Size;
            }
        }
        m_ParticleSystem.SetParticles(m_ParticlePoints, m_ParticlePoints.Length);
    }

    public void SetPosition(int index, Vector3 position) {
        if (index > m_Points.Length - 1 || index < 0 || m_Points == null)
            return;
        m_Points[index] = position;
        DrawPoint();
    }

    public void SetActive(bool value)
    {
        if (value == false)
        {
            m_ParticleSystem.Clear();
        }
        else
        {
            DrawPoint();
        }
        gameObject.SetActive(value);
    }

}
