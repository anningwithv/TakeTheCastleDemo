using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoteRoleController : RoleController
{
    [SerializeField] private Transform m_RemoteShootTrans;
    [SerializeField] private float m_AttackTime = 2f;

    private float m_AttackTimeTemp;

    protected override void Awake()
    {
        base.Awake();

        m_AttackTimeTemp = m_AttackTime;
    }

    protected override void StartHurt()
    {
        GameObject bullet = GameObject.Instantiate(Resources.Load("RemoteBullet") as GameObject);
        bullet.transform.position = m_RemoteShootTrans.transform.position;

        RemoteBullet remote = bullet.GetComponent<RemoteBullet>();
        remote.camp = m_Camp;
        remote.hurtValue = m_AttackHurt;
        remote.speed = 10f;

        if(m_Target != null)
        {
            Vector3 targetPos = m_Target.transform.position + Vector3.up * 3.5f;
            bullet.transform.forward = (targetPos - m_RemoteShootTrans.position).normalized;
        }
        else
        {
            bullet.transform.forward = m_RemoteShootTrans.forward;
        }

        Destroy(bullet, 3f);
    }

    protected override bool TargetType(TargetBase target)
    {
        List<RoleType> list = new List<RoleType>() { RoleType.Cannon, RoleType.Role };
        return list.Contains(target.Type);
    }

    protected override TargetBase GetRandomTarget(CheckPoint point)
    {
        return point.GetRandomTarget(RoleCamp.Blue, new List<RoleType>() { RoleType.Cannon, RoleType.Role });
    }

    protected override void SetAttackStatusUpdate()
    {
        if (m_AttackTimeTemp > 0)
        {
            m_AttackTimeTemp -= Time.deltaTime;
        }
        else
        {
            m_AttackTimeTemp = m_AttackTime;

            StartHurt();
            AttackEnd();
        }

        base.SetAttackStatusUpdate();
    }

    protected override void InputTestUpdate()
    {
        
    }
}
