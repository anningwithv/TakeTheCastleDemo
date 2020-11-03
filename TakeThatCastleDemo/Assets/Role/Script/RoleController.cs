using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RoleController : MonoBehaviour
{
    private NavMeshAgent m_NavMeshAgent;
    private NavMeshObstacle m_NavMeshObstacle;
    private Animator m_Animator;
    private Camera m_Camera;

    private bool m_HasPath;
    //private bool m_IsGet;
    //private float m_Delay = 0.5f;

    // Start is called before the first frame update
    void Awake()
    {
        m_Camera = Camera.main;
        m_NavMeshAgent = GetComponent<NavMeshAgent>();
        m_NavMeshObstacle = GetComponent<NavMeshObstacle>();
        m_Animator = GetComponent<Animator>();

        AnimationClip[] clips = m_Animator.runtimeAnimatorController.animationClips;

        foreach (var item in clips)
        {
            if (item.name == "Attack1")
            {
                AnimationEvent aniEvent = new AnimationEvent();
                aniEvent.time = 2f;
                aniEvent.functionName = "AttackEnd";
                item.AddEvent(aniEvent);
            }
        }

        m_HasPath = false;
        HasNoPath();
    }

    private void FixedUpdate()
    {

    }

    void Update()
    {
        //if (Input.GetMouseButtonDown(0))
        //{
        //    if (Physics.Raycast(m_Camera.ScreenPointToRay(Input.mousePosition), out RaycastHit hit))
        //    {
        //        StartMove(hit.point);
        //    }
        //}

        //m_Delay -= Time.deltaTime;

        //if (!m_IsGet)
        //{
        //    if (m_Delay <= 0 && m_NavMeshAgent.isOnNavMesh && m_NavMeshAgent.remainingDistance <= m_NavMeshAgent.stoppingDistance)
        //    {
        //        //Debug.LogError(m_NavMeshAgent.remainingDistance);

        //        m_IsGet = true;

        //        HasNoPath();
        //    }
        //}

        if (m_HasPath != m_NavMeshAgent.hasPath)
        {
            m_HasPath = m_NavMeshAgent.hasPath;

            if (m_HasPath)
            {
                HasPath();
            }
            else
            {
                HasNoPath();
            }
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            StartAttack();
        }
    }

    private void StartAttack()
    {
        //Debug.LogError("StartAttack");
        m_HasPath = false;

        if(m_NavMeshAgent.isOnNavMesh)
        {
            m_NavMeshAgent.isStopped = true;
            m_NavMeshAgent.ResetPath();
            SetNavActive(false);
        }
        
        m_Animator.SetBool("Run", false);
        m_Animator.SetBool("Idle", false);
        m_Animator.SetBool("Attack", true);
    }

    private void AttackEnd()
    {
        //Debug.LogError("AttackEnd");
        if (m_NavMeshAgent.hasPath)
        {
            HasPath();
        }
        else
        {
            HasNoPath();
        }
    }

    public void StartMove(Vector3 targetPostion)
    {
        //Debug.LogError("StartMove");
        if (!CanStartMove())
        {
            return;
        }

        SetNavActive(true);
        m_NavMeshAgent.SetDestination(targetPostion);
        //m_IsGet = false;
        //m_Delay = 0.5f;
    }

    private bool CanStartMove()
    {
        AnimatorStateInfo clips = m_Animator.GetCurrentAnimatorStateInfo(0);
        return !clips.IsName("Attack");
    }

    private void HasPath()
    {
        //Debug.LogError("HasPath");

        m_Animator.SetBool("Run", true);
        m_Animator.SetBool("Idle", false);
        m_Animator.SetBool("Attack", false);
    }

    private void HasNoPath()
    {
        //Debug.LogError("HasNoPath");
        
        m_Animator.SetBool("Run", false);
        m_Animator.SetBool("Idle", true);
        m_Animator.SetBool("Attack", false);

        SetNavActive(false);
    }

    private void SetNavActive(bool active)
    {
        m_NavMeshAgent.enabled = active;
        m_NavMeshObstacle.enabled = !active;
    }
}
