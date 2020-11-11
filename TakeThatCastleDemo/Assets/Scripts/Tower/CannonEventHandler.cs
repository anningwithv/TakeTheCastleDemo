using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonEventHandler : MonoBehaviour
{
    public Cannon m_Cannon = null;

    public void OnFire()
    {
        m_Cannon.OnFire();
    }

    public void OnFireEnd()
    {
        m_Cannon.OnFireEnd();
    }
}
