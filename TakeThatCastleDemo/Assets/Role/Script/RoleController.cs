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
    Die
}

public enum RoleCamp
{
    None,
    Red,
    Blue
}


public class RoleController : TargetBase
{
    [SerializeField] private TargetBase m_Target;
    [SerializeField] private int m_AttackHurt = 10;

    [SerializeField] private float m_AttackDistance = 3f;
    [SerializeField] private float m_FindTargetRadius = 10f;
    [SerializeField] private float m_DieTime = 5f;
    [SerializeField] private float m_IdleTime = 2f;
    [SerializeField] private float m_RunLengthTime = 0.5f;
    [SerializeField] private float m_RunLength = 0.3f;

    private NavMeshAgent m_NavMeshAgent;
    private Collider m_Collider;
    private Animator m_Animator;
    private Camera m_Camera;
    private SkinnedMeshRenderer m_SkinnedMeshRenderer;
    private Rigidbody m_Rigidbody;

    private bool m_HasPath;
    private float m_IdleTimeTemp;
    private float m_RunLengthTimeTemp;

    private Vector3 m_MoveTargetPosition;

    public Action<RoleController> IdleCallBack;
    public Action<RoleController> RunOverCallBack;
    public Action<RoleController> InitializeCallBack;


    public Vector3 MoveTargetPosition { get => m_MoveTargetPosition; }

    // Start is called before the first frame update
    void Awake()
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

    void Update()
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
                FindTargetUpdate();
                IdleTimeUpdate();
                break;
            case RoleStatus.SetRun:
                RunAnimationUpdate();
                SetRunStatusUpdate();
                FindTargetUpdate();
                break;
            case RoleStatus.AutoRun:
                RunAnimationUpdate();
                AutoRunStatusUpdate();
                break;
            case RoleStatus.Attack:
                break;
            case RoleStatus.Die:
                break;
            default:
                break;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, 0.3f);
        Gizmos.DrawWireSphere(transform.position, m_FindTargetRadius / 2);

        Gizmos.DrawLine(transform.position, m_MoveTargetPosition);
    }

    private void InputTestUpdate()
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

    private void SetStatus(RoleStatus status)
    {
        m_Status = status;

        switch (m_Status)
        {
            case RoleStatus.None:
                break;
            case RoleStatus.Idle:
                m_Animator.speed = 1;
                m_RunLengthTimeTemp = 0;
                m_IdleTimeTemp = m_IdleTime;
                IdleAniamtion();
                SetNavActive(false, null);
                break;
            case RoleStatus.SetRun:
                StartMove(m_MoveTargetPosition);
                break;
            case RoleStatus.AutoRun:
                StartMove(m_MoveTargetPosition);
                break;
            case RoleStatus.Attack:
                m_Animator.speed = 1;
                StartAttack();
                break;
            case RoleStatus.Die:
                m_Animator.speed = 1;
                Die();
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
                if (m_NavMeshAgent.isOnNavMesh)
                {
                    m_NavMeshAgent.isStopped = true;
                    m_NavMeshAgent.ResetPath();
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
                Vector3 targetPos = m_Target.transform.position;
                Vector3 selfPos = transform.position;

                float distance = Vector2.Distance(new Vector2(targetPos.x, targetPos.z), new Vector2(selfPos.x, selfPos.z));

                if (distance <= m_AttackDistance)
                {
                    SetStatus(RoleStatus.Attack);
                }
                else
                {
                    SetStatus(RoleStatus.AutoRun);
                    m_MoveTargetPosition = targetPos;
                }
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
            IdleCallBack?.Invoke(this);
        }
    }

    private void FindTargetUpdate()
    {
        if (m_Target == null)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, m_FindTargetRadius, 1 << LayerMask.NameToLayer("Target"));

            if (colliders.Length > 0)
            {
                foreach (var item in colliders)
                {
                    TargetBase role = item.gameObject.GetComponent<TargetBase>();
                    if (role!= null && role != this && role.Status != RoleStatus.Die && role.Camp != RoleCamp.None && role.Camp != m_Camp)
                    {
                        m_Target = role;

                        m_MoveTargetPosition = m_Target.transform.position;
                        SetStatus(RoleStatus.AutoRun);

                        break;
                    }
                }
            }
        }
    }

    private void StartAttack()
    {
        //Debug.LogError(gameObject.name + " -- StartAttack");

        if (m_Target != null)
        {
            transform.forward = (m_Target.transform.position - transform.position).normalized;
        }
        m_HasPath = false;

        if(m_NavMeshAgent.isOnNavMesh)
        {
            m_NavMeshAgent.isStopped = true;
            m_NavMeshAgent.ResetPath();
            SetNavActive(false, null);
        }

        AttackAnimation();
    }

    private bool CanStartMove()
    {
        AnimatorStateInfo clips = m_Animator.GetCurrentAnimatorStateInfo(0);
        return !clips.IsName("Attack");
    }

    private void StartMove(Vector3 targetPostion)
    {
        //Debug.LogError(gameObject.name + " -- StartMove");

        if (!CanStartMove())
        {
            //Debug.LogError(gameObject.name + " -- Can not StartMove");
            return;
        }

        m_NavMeshAgent.SetDestination(targetPostion);
        RunAnimation();

        SetNavActive(true, null);
    }

    private void SetNavActive(bool active, Action act)
    {
        act?.Invoke();
    }

    private void SetNavActive1(bool active)
    {
        m_NavMeshAgent.enabled = active;
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
    #endregion

    #region 动画方法回调
    private void AttackEnd()
    {
        //Debug.LogError(gameObject.name + " -- AttackEnd");

        if (m_Target != null && m_Target.Status != RoleStatus.Die)
        {

        }
        else
        {
            m_Target = null;
            SetStatus(RoleStatus.Idle);
        }
    }

    private void StartHurt()
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
