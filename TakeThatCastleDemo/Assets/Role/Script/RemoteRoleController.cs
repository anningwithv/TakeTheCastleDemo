using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoteRoleController : RoleController
{
    [SerializeField] private Transform m_RemoteShootTrans;

    protected override void StartHurt()
    {
        GameObject bullet = GameObject.Instantiate(Resources.Load("RemoteBullet") as GameObject);
        bullet.transform.position = m_RemoteShootTrans.transform.position;

        RemoteBullet remote = bullet.GetComponent<RemoteBullet>();
        remote.camp = m_Camp;
        remote.hurtValue = m_AttackHurt;
        remote.speed = 1f;

        if(m_Target != null)
        {
            Vector3 dir = (m_Target.transform.position - transform.position).normalized;
            remote.flyDir = dir;
        }
        else
        {
            remote.flyDir = transform.forward;
        }

        Destroy(bullet, 3f);

        //Debug.LogError("StartHurt");
    }

    protected override bool TargetType(TargetBase target)
    {
        return target.Type == RoleType.Cannon;
    }

    protected override TargetBase GetRandomTarget(CheckPoint point)
    {
        return point.GetRandomTarget(RoleCamp.Blue, new List<RoleType>() { RoleType.Cannon });
    }
}
