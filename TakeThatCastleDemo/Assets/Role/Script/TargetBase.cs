using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetBase : MonoBehaviour
{
    [SerializeField] protected RoleStatus m_Status;
    [SerializeField] protected RoleCamp m_Camp;
    [SerializeField] protected RoleType m_Type;

    [SerializeField] protected TargetBase m_Target;
    [SerializeField] protected int m_HP = 50;
    [SerializeField] protected int m_AttackHurt = 10;
    [SerializeField] protected GameObject m_BeShootObj;
    protected int m_ID;

    public Action<TargetBase> DieCallBack;

    public RoleStatus Status { get => m_Status; }
    public RoleCamp Camp { get => m_Camp; }
    public int ID { get => m_ID; }
    public RoleType Type { get => m_Type; set => m_Type = value; }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected virtual void StartAttack()
    {
        
    }

    public virtual void LoadMaterial()
    {
        
    }

    public void SetRoleCamp(RoleCamp camp)
    {
        m_Camp = camp;
    }

    public void SetRoleType(RoleType type)
    {
        m_Type = type;
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

    public virtual GameObject GetTargetPosObj()
    {
        return gameObject;
    }

    public virtual GameObject GetBeShootPosObj()
    {
        return m_BeShootObj;
    }
}
