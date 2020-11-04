using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;

public enum RoleStatus
{
    None,
    Idle,
    SetRun,
    AutoRun,
    Attack,
    Die
}


public class RoleController : MonoBehaviour
{
    [SerializeField] private RoleController m_Target;
    [SerializeField] private RoleStatus m_Status;

    [SerializeField] private int m_AttackHurt = 10;
    [SerializeField] private int m_Life = 50;
    [SerializeField] private float m_AttackDistance = 1f;
    [SerializeField] private float m_FindTargetRadius = 100f;

    private int m_RoleID;

    private NavMeshAgent m_NavMeshAgent;
    private NavMeshObstacle m_NavMeshObstacle;
    private Collider m_Collider;
    private Animator m_Animator;
    private Camera m_Camera;

    private bool m_HasPath;

    private Vector3 m_MoveTargetPosition;

    public int RoleID { get => m_RoleID; }
    public RoleStatus Status { get => m_Status; }

    //private bool m_IsGet;
    //private float m_Delay = 0.5f;

    // Start is called before the first frame update
    void Awake()
    {
        m_Camera = Camera.main;
        m_NavMeshAgent = GetComponent<NavMeshAgent>();
        m_NavMeshObstacle = GetComponent<NavMeshObstacle>();
        m_Animator = GetComponent<Animator>();
        m_Collider = GetComponent<Collider>();

        SetStatus(RoleStatus.Idle);
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

        switch (m_Status)
        {
            case RoleStatus.None:
                break;
            case RoleStatus.Idle:
                FindTargetUpdate();
                break;
            case RoleStatus.SetRun:
                SetRunStatusUpdate();
                FindTargetUpdate();
                break;
            case RoleStatus.AutoRun:
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

    private void SetStatus(RoleStatus status)
    {
        m_Status = status;

        switch (m_Status)
        {
            case RoleStatus.None:
                break;
            case RoleStatus.Idle:
                IdleAniamtion();
                SetNavActive(false);
                break;
            case RoleStatus.SetRun:
                StartMove(m_MoveTargetPosition);
                break;
            case RoleStatus.AutoRun:
                StartMove(m_MoveTargetPosition);
                break;
            case RoleStatus.Attack:
                StartAttack();
                break;
            case RoleStatus.Die:
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
                SetStatus(RoleStatus.Idle);
            }
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

    private void FindTargetUpdate()
    {
        if (m_Target == null)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, m_FindTargetRadius, 1 << LayerMask.NameToLayer("Role"));

            if (colliders.Length > 0)
            {
                foreach (var item in colliders)
                {
                    RoleController role = item.gameObject.GetComponent<RoleController>();
                    if (role != this && role.Status != RoleStatus.Die)
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
            SetNavActive(false);
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

        SetNavActive(true);
        m_NavMeshAgent.SetDestination(targetPostion);
        RunAnimation();
    }

    private void SetNavActive(bool active)
    {
        m_NavMeshAgent.enabled = active;
        m_NavMeshObstacle.enabled = !active;
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
        if (m_Target != null)
        {
            m_Target.Hurt(m_AttackHurt);
        }
    }
    #endregion

    public void SetRoleID(int id)
    {
        m_RoleID = id;
    }

    public void Die()
    {
        m_Collider.enabled = false;
        SetNavActive(false);

        DieAnimation();

        Destroy(gameObject, 3f);
    }

    public void Hurt(int value)
    {
        m_Life -= value;
        if (m_Life <= 0)
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
