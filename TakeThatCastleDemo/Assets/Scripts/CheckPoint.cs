using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    public int index;

    [SerializeField] private List<RoleController> m_RoleList = new List<RoleController>();
    [SerializeField] private bool m_IsEnd;

    private void Awake()
    {
        foreach (var item in m_RoleList)
        {
            item.DieCallBack = DieCallBack;
            item.LoadMaterial();
        }
    }

    private void Update()
    {
        if (!m_IsEnd && m_RoleList.Count == 0)
        {
            m_IsEnd = true;
            CheckPointMgr.S.OnCheckPointTriggered(this);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position, 1f);
    }

    private void OnTriggerEnter(Collider other)
    {

    }

    private void DieCallBack(RoleController role)
    {
        if(m_RoleList.Contains(role))
        {
            m_RoleList.Remove(role);
        }
    }
}
