using UnityEngine;
using System.Collections.Generic;

public class CGroupController : CObjectController {

    public float StartRotationAngle;
    [SerializeField]
	private int m_GridCollumn = 2;
	[SerializeField]
	private int m_GridRow = 3;
    [SerializeField]
    private Color[] m_ColorChange;
    [SerializeField]
    private CCircleController[] m_ChildCircles;
    [SerializeField]
	private GameObject[] m_Grid;
    [SerializeField]
    private int m_Index;
    public int Index
    {
        get { return m_Index; }
        set {
            m_Index = value;
            for (int i = 0; i < m_ChildCircles.Length; i++)
            {
                m_ChildCircles[i].Index = value + (value - 1) + i;
            }
        }
    }
    public CCircleController FirstCircle;
	protected BoxCollider2D m_BoxCollider;
    private HashSet<GameObject> m_PointAlready;

    public override void Init()
    {
		base.Init();
		m_BoxCollider = this.GetComponent<BoxCollider2D>();
		if (m_BoxCollider != null) {
			XSize = m_BoxCollider.size.x * m_Transform.localScale.x;
			YSize = m_BoxCollider.size.y * m_Transform.localScale.y;
		}

        m_PointAlready = new HashSet<GameObject>();
        LoadAllChildCircles();
        LoadAllChildGrid();
        LoadRandomCircle();
    }

    public override void Start()
    {
        base.Start();
        Init();
    }

    public override void Update()
    {
        base.Update();
        if (m_GameManager.OnPlaying == false)
            return;
        UpdateCircles();

        if (m_CameraController.CheckInActiveCamera(m_Transform.position.x, m_Transform.position.y, XSize, YSize) && IsUsed == false)
        {
            var currentRotation = m_ChildCircles[m_ChildCircles.Length - 1].gameObject;
            m_GameManager.LoadRandomGroup(currentRotation, m_Transform.position);
            IsUsed = true;
        }

        if (m_CameraController.CheckOutOfCamera(m_Transform.position.x, m_Transform.position.y, XSize, YSize) && IsUsed)
        {
            m_GameManager.DeleteOldObject(GameType, this);
        }
    }

    private void UpdateCircles() {
        for (int i = 0; i < m_ChildCircles.Length; i++)
        {
            m_ChildCircles[i].UpdateCircles(i, Time.deltaTime, ((i % 2) * m_GameManager.CircleRotationOffset) + 1);
        }
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.white;
    //    for (int i = 0; i < m_ChildCircles.Length; i++)
    //    {
    //        var startposition = m_ChildCircles[i].transform.position;
    //        var endPosition = m_ChildCircles[i].transform.TransformDirection(Vector3.up * 5f);
    //        Gizmos.DrawLine(startposition, startposition + endPosition);
    //    }
    //}

    public void LoadAllChildCircles() 
	{
        var childCircles = m_Transform.FindChild ("Circles");
        m_ChildCircles = new CCircleController[childCircles.childCount];
        for (int i = 0; i < m_ChildCircles.Length; i++)
        {
            var child = childCircles.GetChild(i).gameObject;
            m_ChildCircles[i] = child.GetComponent<CCircleController>();
            m_ChildCircles[i].Init();
        }
    }

    public void LoadAllChildGrid()
    {
        var childGrid = m_Transform.FindChild("Grid");
        m_Grid = new GameObject[childGrid.childCount];
        for (int i = 0; i < m_Grid.Length; i++)
        {
            m_Grid[i] = childGrid.GetChild(i).gameObject;
        }
    }

    public void LoadRandomCircle() {
		if (m_ChildCircles == null || m_Grid == null || m_PointAlready == null || m_ChildCircles.Length == 0)
            return;
        m_PointAlready.Clear();
        var colorRangeOffset = Random.Range(0, 9999) % m_ColorChange.Length;
        var prevertCircle = m_ChildCircles[0];
        var j = 0;
        for (int i = 0; i < m_ChildCircles.Length; i++)
        {
			var random = Random.Range(0, 9999) % m_GridCollumn;
			var randomIndex = random + i * m_GridCollumn;
			if (m_PointAlready.Contains(m_Grid[randomIndex < m_Grid.Length ? randomIndex : 0])) {
                i--;
                continue;
            }
			var point = m_Grid[randomIndex].transform.position;
			m_ChildCircles[i].transform.position = point;
			m_ChildCircles[i].transform.rotation = Quaternion.Euler (Vector3.forward * StartRotationAngle);
            var randomColor1 = (colorRangeOffset + i + j) % m_ColorChange.Length;
            var randomColor2 = (colorRangeOffset + i + 1 + j) % m_ColorChange.Length;
            m_ChildCircles[i].BigCircle.color = m_ColorChange[randomColor1];
            m_ChildCircles[i].SmallCircle.color = m_ColorChange[randomColor2];

            if (i != 0)
            {
                var childAngle = m_ChildCircles[i].transform.rotation.eulerAngles;
                var direction = prevertCircle.transform.position - m_ChildCircles[i].transform.position;
                var angle = Vector3.Angle(Vector3.up, direction); // Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                m_ChildCircles[i].transform.rotation = Quaternion.AngleAxis(angle - childAngle.z, Vector3.forward);
                prevertCircle = m_ChildCircles[i];
            }
            else {
                FirstCircle = m_ChildCircles[i];
            }

            j++;
            m_PointAlready.Add(m_Grid[randomIndex]);
        }
    }

}
