using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoteRoleController : RoleController
{
    [SerializeField] private Transform m_RemoteShootTrans;
    [SerializeField] private float m_AttackTime = 2f;
    [SerializeField] private bool m_IsIK;

    [SerializeField] private Transform m_IKTrans;
    [SerializeField] private Transform m_Gun;

    private Vector3 m_IKInitPosition;

    private float m_AttackTimeTemp;

    protected override void Awake()
    {
        base.Awake();
        m_AttackTimeTemp = m_AttackTime;
        m_IKInitPosition = m_IKTrans.position;
    }

    protected override void StartHurt()
    {
        GameObject bullet = GameObject.Instantiate(Resources.Load("RemoteBullet") as GameObject);
        bullet.transform.position = m_RemoteShootTrans.transform.position;

        RemoteBullet remote = bullet.GetComponent<RemoteBullet>();
        remote.camp = m_Camp;
        remote.hurtValue = m_AttackHurt;
        remote.speed = 30f;

        if(m_Target != null)
        {
            Vector3 targetPos = m_Target.GetBeShootPosObj().transform.position;
            bullet.transform.forward = (targetPos - m_RemoteShootTrans.position).normalized;
        }
        else
        {
            bullet.transform.forward = m_RemoteShootTrans.forward;
        }

        Destroy(bullet, 3f);
    }

    protected override void AttackEnd()
    {
        if (m_Target != null && m_Target.Status != RoleStatus.Die)
        {
            m_MoveTargetPosition = m_Target.GetTargetPosObj().transform.position;
            SetStatus(RoleStatus.AutoRun);
        }
        else
        {
            m_Target = null;
            SetStatus(RoleStatus.Idle);
        }
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
        if (m_Target == null || m_Target.Status == RoleStatus.Die) 
        {
            m_AttackTimeTemp = m_AttackTime;
            AttackEnd();
            return;
        }

        if (m_AttackTimeTemp > 0)
        {
            Vector3 IKDir = (m_Target.GetBeShootPosObj().transform.position - m_Gun.position).normalized;
            m_IKTrans.transform.position = m_Gun.position + IKDir * 4;

            Vector3 dir = (m_IKTrans.position - m_Gun.position).normalized;
            m_Gun.forward = dir;

            m_AttackTimeTemp -= Time.deltaTime;
        }
        else
        {
            m_AttackTimeTemp = m_AttackTime;

            StartHurt();
            AttackEnd();
        }
    }

    protected override void InputTestUpdate()
    {
        
    }

    protected override void Update()
    {
        base.Update();
    }

    protected void OnAnimatorIK(int layerIndex)
    {
        if (m_IsIK && m_Status == RoleStatus.Attack)
        {
            m_Animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1f);
            m_Animator.SetIKPosition(AvatarIKGoal.RightHand, m_IKTrans.transform.position);
        }
        else
        {
            m_Animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0f);
        }
    }
}
