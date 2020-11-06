using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    public int index;

    [SerializeField] private List<TargetBase> m_RoleList = new List<TargetBase>();
    [SerializeField] private bool m_IsEnd;

    private void Start()
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

    private void DieCallBack(TargetBase role)
    {
        if(m_RoleList.Contains(role))
        {
            m_RoleList.Remove(role);
        }
    }

    public TargetBase GetRandomTarget(RoleCamp camp)
    {
        List<TargetBase> tempList = new List<TargetBase>();
        foreach (var item in m_RoleList)
        {
            if (item.Status != RoleStatus.Die && item.Camp == camp)
            {
                tempList.Add(item);
            }
        }

        if (tempList.Count == 0)
        {
            return null;
        }

        return tempList[Random.Range(0, tempList.Count - 1)];
    }

}
