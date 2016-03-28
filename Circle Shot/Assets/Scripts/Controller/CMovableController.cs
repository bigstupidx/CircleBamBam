using UnityEngine;
using System.Collections;

public class CMovableController : CObjectController
{
    protected Rigidbody2D m_Rigidbody2D;

    public override void Init() {
        base.Init();
        m_Rigidbody2D = this.GetComponent<Rigidbody2D>();
    }

    public override void OnResetObject()
    {
        base.OnResetObject();
        m_GameManager.DeleteOldObject(GameType, this);
    }
}
