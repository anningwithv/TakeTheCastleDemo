using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetBase : MonoBehaviour
{
    [SerializeField] protected RoleStatus m_Status;
    [SerializeField] protected RoleCamp m_Camp;
    [SerializeField] protected int m_HP = 50;

    protected int m_ID;

    public Action<TargetBase> DieCallBack;

    public RoleStatus Status { get => m_Status; }
    public RoleCamp Camp { get => m_Camp; }
    public int ID { get => m_ID; }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void LoadMaterial()
    {
        
    }

    public void SetRoleCamp(RoleCamp camp)
    {
        m_Camp = camp;
    }

    public void SetTargetID(int id)
    {
        m_ID = id;
    }

    public virtual void Hurt(int value)
    {
        
    }

    public virtual void SetRoleHP(int value)
    {
        m_HP = value;
    }

    public virtual void Die()
    {
        
    }
}
