using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;
using System;

public enum RoleStatus
{
    None,
    Idle,
    SetRun,
    AutoRun,
    Attack,
    Die,
    Win
}

public enum RoleCamp
{
    None,
    Red,
    Blue
}

public enum RoleType
{
    None,
    Cannon,
    Role
}



public class RoleController : TargetBase
{
    [SerializeField] private float m_AttackDistance = 3f;
    [SerializeField] private float m_FindTargetRadius = 10f;
    [SerializeField] private float m_DieTime = 5f;
    [SerializeField] private float m_IdleTime = 2f;
    [SerializeField] private float m_RunLengthTime = 0.5f;
    [SerializeField] private float m_RunLength = 0.3f;

    private NavMeshAgent m_NavMeshAgent;
    private Collider m_Collider;
    protected Animator m_Animator;
    private Camera m_Camera;
    private SkinnedMeshRenderer m_SkinnedMeshRenderer;
    private Rigidbody m_Rigidbody;

    private bool m_HasPath;
    private float m_IdleTimeTemp;
    private float m_RunLengthTimeTemp;

    protected Vector3 m_MoveTargetPosition;

    public Action<RoleController> IdleCallBack;
    public Action<RoleController> RunOverCallBack;
    public Action<RoleController> InitializeCallBack;


    public Vector3 MoveTargetPosition { get => m_MoveTargetPosition; }

    // Start is called before the first frame update
    protected virtual void Awake()
    {
        m_Camera = Camera.main;
        m_NavMeshAgent = GetComponent<NavMeshAgent>();
        m_NavMeshAgent.enabled = false;
        m_Animator = GetComponentInChildren<Animator>();
        m_Collider = GetComponent<Collider>();
        m_SkinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        m_Rigidbody = GetComponent<Rigidbody>();
        m_IdleTimeTemp = m_IdleTime;

        SetStatus(RoleStatus.Idle);
    }

    private void Start()
    {
        m_NavMeshAgent.enabled = true;

        InitializeCallBack?.Invoke(this);
    }

    private void FixedUpdate()
    {

    }

    protected virtual void Update()
    {
        if (m_Status == RoleStatus.Die)
        {
            return;
        }

        //InputTestUpdate();

        switch (m_Status)
        {
            case RoleStatus.None:
                break;
            case RoleStatus.Idle:

                bool findResult = FindTargetUpdate();

                if(!findResult)
                {
                    IdleTimeUpdate();
                }

                break;
            case RoleStatus.SetRun:
                RunAnimationUpdate();
                SetRunStatusUpdate();
                FindTargetUpdate();
                break;
            case RoleStatus.AutoRun:
                RunAnimationUpdate();
                AutoRunStatusUpdate();
                FindTargetUpdate();
                break;
            case RoleStatus.Attack:
                SetAttackStatusUpdate();
                break;
            case RoleStatus.Die:
                break;
            default:
                break;
        }
    }

    private void OnDrawGizmos()
    {
        if(m_Camp == RoleCamp.Red)
        {
            Gizmos.color = new Color(1, 0, 0, 0.3f);
            Gizmos.DrawWireSphere(transform.position, m_FindTargetRadius);

            if(m_NavMeshAgent.hasPath)
            {
                Gizmos.DrawLine(transform.position, m_MoveTargetPosition);
            }
            
        }
        
    }

    protected virtual void InputTestUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(m_Camera.ScreenPointToRay(Input.mousePosition), out RaycastHit hit))
            {
                SetMove(hit.point);
            }
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            SetStatus(RoleStatus.Attack);
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            SetStatus(RoleStatus.Die);
        }
    }

    public override void LoadMaterial()
    {
        switch (m_Camp)
        {
            case RoleCamp.None:
                break;
            case RoleCamp.Red:
                m_SkinnedMeshRenderer.material = Resources.Load("Character/Material/Red") as Material;
                break;
            case RoleCamp.Blue:
                m_SkinnedMeshRenderer.material = Resources.Load("Character/Material/Blue") as Material;
                break;
            default:
                break;
        }
    }

    public void SetStatus(RoleStatus status)
    {
        m_Status = status;

        switch (m_Status)
        {
            case RoleStatus.None:
                break;
            case RoleStatus.Idle:
                m_MoveTargetPosition = Vector3.zero;
                m_Animator.speed = 1;
                m_RunLengthTimeTemp = 0;
                m_IdleTimeTemp = m_IdleTime;
                IdleAniamtion();
                break;
            case RoleStatus.SetRun:
                StartMove(m_MoveTargetPosition);
                break;
            case RoleStatus.AutoRun:
                StartMove(m_MoveTargetPosition);
                break;
            case RoleStatus.Attack:
                m_MoveTargetPosition = Vector3.zero;
                m_Animator.speed = 1;
                m_RunLengthTimeTemp = 0;
                m_IdleTimeTemp = m_IdleTime;
                StartAttack();
                break;
            case RoleStatus.Die:
                m_Animator.speed = 1;
                Die();
                break;
            case RoleStatus.Win:
                StopNav();
                WinAnimation();
                break;
            default:
                break;
        }
    }

    private void SetRunStatusUpdate()
    {
        if (m_HasPath != m_NavMeshAgent.hasPath)
        {
            m_HasPath = m_NavMeshAgent.hasPath;

            if (!m_HasPath)
            {
                RunOverCallBack?.Invoke(this);
                SetStatus(RoleStatus.Idle);
            }
        }
    }

    protected virtual void SetAttackStatusUpdate()
    {

    }

    private void RunAnimationUpdate()
    {
        float length = m_NavMeshAgent.velocity.magnitude;

        float animationSpeed = Mathf.Clamp(length / m_NavMeshAgent.speed, 0.2f, 1);
        m_Animator.speed = animationSpeed;

        if (length <= m_RunLength)
        {
            m_RunLengthTimeTemp += Time.deltaTime;
            if (m_RunLengthTimeTemp >= m_RunLengthTime)
            {
                if(StopNav())
                {
                    RunOverCallBack?.Invoke(this);
                    SetStatus(RoleStatus.Idle);
                }
                m_RunLengthTimeTemp = 0;
            }
        }
        else
        {
            m_RunLengthTimeTemp = 0;
        }
    }

    private void AutoRunStatusUpdate()
    {
        if (m_Target != null)
        {
            if (m_Target.Status == RoleStatus.Die)
            {
                m_Target = null;
            }
            else
            {
                Vector3 targetPos = m_Target.GetTargetPosObj().transform.position;
                Vector3 selfPos = transform.position;

                float distance = Vector2.Distance(new Vector2(targetPos.x, targetPos.z), new Vector2(selfPos.x, selfPos.z));

                if (distance <= m_AttackDistance)
                {
                    SetStatus(RoleStatus.Attack);
                }
                else
                {
                    if (m_MoveTargetPosition != targetPos)
                    {
                        SetStatus(RoleStatus.AutoRun);
                        m_MoveTargetPosition = targetPos;
                    }
                    else
                    {
                        if(!m_NavMeshAgent.hasPath)
                        {
                            SetStatus(RoleStatus.AutoRun);
                        }
                    }
                }

                FindTargetUpdate();
            }
        }
        else
        {
            FindTargetUpdate();

            if(m_Target == null)
            {
                SetStatus(RoleStatus.Idle);
            }
        }
    }

    private void IdleTimeUpdate()
    {
        if (m_IdleTimeTemp > 0)
        {
            m_IdleTimeTemp -= Time.deltaTime;
        }
        else
        {
            m_IdleTimeTemp = m_IdleTime;

            bool result = IdleFindTargetUpdate();

            if(!result)
            {
                IdleCallBack?.Invoke(this);
            }
        }
    }

    private bool FindTargetUpdate()
    {
        bool result = false;
        if (m_Target == null)
        {
            TargetBase target = GetDistanceTarget();
            {
                if (target != null)
                {
                    m_Target = target;
                    m_MoveTargetPosition = m_Target.GetTargetPosObj().transform.position;
                    SetStatus(RoleStatus.AutoRun);
                    result = true;
                }
            }
        }

        return result;
    }

    private TargetBase GetDistanceTarget()
    {
        TargetBase target = null;

        Collider[] colliders = Physics.OverlapSphere(transform.position, m_FindTargetRadius, 1 << LayerMask.NameToLayer("Target"));

        if (colliders.Length > 0)
        {
            List<TargetBase> findRoleList = new List<TargetBase>();

            foreach (var item in colliders)
            {
                TargetBase role = item.gameObject.GetComponent<TargetBase>();
                if (role != null && role != this && role.Status != RoleStatus.Die && role.Camp != RoleCamp.None && role.Camp != m_Camp)
                {
                    if (TargetType(role))
                    {
                        findRoleList.Add(role);
                    }
                }
            }

            if (findRoleList.Count > 0)
            {
                Dictionary<float, TargetBase> targetDic = new Dictionary<float, TargetBase>();
                float minDis = 10000f;
                foreach (var item in findRoleList)
                {
                    float dis = Vector3.Distance(item.GetTargetPosObj().transform.position, transform.position);
                    targetDic.Add(dis, item);

                    if (minDis > dis)
                    {
                        minDis = dis;
                    }
                }
                if (targetDic.ContainsKey(minDis))
                {
                    target = targetDic[minDis];
                }
            }
        }

        return target;
    }

    private bool IdleFindTargetUpdate()
    {
        bool result = false;
        if(m_Camp == RoleCamp.Blue)
        {
            return false;
        }

        if (m_Target == null)
        {
            CheckPoint point = CheckPointMgr.S.TargetCheckPoint;
            if (point != null)
            {
                TargetBase target = GetRandomTarget(point);
                if (target != null)
                {
                    //Debug.LogError(gameObject.name + " -- IdleFindTargetUpdate -- " + target.gameObject.name);
                    m_Target = target;
                    m_MoveTargetPosition = m_Target.GetTargetPosObj().transform.position;
                    SetStatus(RoleStatus.AutoRun);
                    result = true;
                }
            }
        }

        return result;
    }

    protected virtual bool TargetType(TargetBase target)
    {
        return target.Type == RoleType.Role;
    }

    protected virtual TargetBase GetRandomTarget(CheckPoint point)
    {
        return point.GetRandomTarget(RoleCamp.Blue, new List<RoleType>() { RoleType.Role });
    }

    protected override void StartAttack()
    {
        //Debug.LogError(gameObject.name + " -- StartAttack");

        StopNav();

        m_HasPath = false;

        if (m_Target != null)
        {
            transform.forward = (m_Target.transform.position - transform.position).normalized;
        }

        AttackAnimation();
    }

    private bool StopNav()
    {
        bool result = m_NavMeshAgent.isOnNavMesh;
        if (result)
        {
            m_NavMeshAgent.isStopped = true;
            m_NavMeshAgent.ResetPath();
            m_NavMeshAgent.velocity = Vector3.zero;
        }
        return result;
    }

    private bool CanStartMove()
    {
        AnimatorStateInfo clips = m_Animator.GetCurrentAnimatorStateInfo(0);
        return !clips.IsName("Attack");
    }

    private void StartMove(Vector3 targetPostion)
    {
        if (CanStartMove() && m_NavMeshAgent.isOnNavMesh)
        {
            //Debug.LogError(gameObject.name + " -- Move -- " + targetPostion);
            m_NavMeshAgent.SetDestination(targetPostion);
            RunAnimation();
        }
        else
        {
            //Debug.LogError(gameObject.name + " -- Can Not Move");
        }
    }

    #region 动画设置
    private void RunAnimation()
    {
        m_Animator.SetBool("Run", true);
        m_Animator.SetBool("Idle", false);
        m_Animator.SetBool("Attack", false);
    }

    private void IdleAniamtion()
    {
        m_Animator.SetBool("Run", false);
        m_Animator.SetBool("Idle", true);
        m_Animator.SetBool("Attack", false);
    }

    private void AttackAnimation()
    {
        m_Animator.SetBool("Run", false);
        m_Animator.SetBool("Idle", false);
        m_Animator.SetBool("Attack", true);
    }

    private void DieAnimation()
    {
        m_Animator.SetBool("Run", false);
        m_Animator.SetBool("Idle", false);
        m_Animator.SetBool("Attack", false);
        m_Animator.SetBool("Die", true);
    }

    private void WinAnimation()
    {
        m_Animator.SetBool("Run", false);
        m_Animator.SetBool("Idle", false);
        m_Animator.SetBool("Attack", false);
        m_Animator.SetBool("Die", false);
        m_Animator.SetBool("Win", true);

    }
    #endregion

    #region 动画方法回调
    protected virtual void AttackEnd()
    {
        //Debug.LogError(gameObject.name + " -- AttackEndCallBack");
        
        if (m_Target != null && m_Target.Status != RoleStatus.Die)
        {
            //Debug.LogError(gameObject.name + " -- Attack Resume");
            float distance = Vector3.Distance(m_Target.GetTargetPosObj().transform.position, transform.position);
            if (distance > m_AttackDistance)
            {
                m_MoveTargetPosition = m_Target.GetTargetPosObj().transform.position;
                SetStatus(RoleStatus.AutoRun);
            }
        }
        else
        {
            //Debug.LogError(gameObject.name + " -- Attack Idle");
            m_Target = null;
            SetStatus(RoleStatus.Idle);
        }
    }

    protected virtual void StartHurt()
    {
        //Debug.LogError(gameObject.name + " -- StartHurt");

        if (m_Target != null)
        {
            m_Target.Hurt(m_AttackHurt);
        }
    }
    #endregion

    public override void Die()
    {
        base.Die();

        m_Collider.enabled = false;
        m_NavMeshAgent.enabled = false;
        
        IdleCallBack = null;
        RunOverCallBack = null;
        InitializeCallBack = null;

        DieAnimation();

        SoldierSpawnMgr.S.RemoveRole(this);
        DieCallBack?.Invoke(this);
        Destroy(gameObject, m_DieTime);
    }

    public override void Hurt(int value)
    {
        m_HP -= value;
        if (m_HP <= 0)
        {
            SetStatus(RoleStatus.Die);
        }
    }

    public void SetMove(Vector3 targetPos)
    {
        m_Target = null;
        m_MoveTargetPosition = targetPos;
        SetStatus(RoleStatus.SetRun);
    }
}
