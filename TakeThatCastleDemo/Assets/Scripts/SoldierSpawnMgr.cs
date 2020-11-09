using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Qarth;
using System.Linq;

public class SoldierSpawnMgr : TMonoSingleton<SoldierSpawnMgr>
{
    private List<RoleController> m_RoleList = new List<RoleController>();

    [SerializeField] private List<Vector3> m_TargetPos = new List<Vector3>();

    private float m_MoveRandomRadius = 10f;
    private Vector3 m_SoldierTargetPos = Vector3.zero;


    public override void OnSingletonInit()
    {
        base.OnSingletonInit();
    }

    public void SpawnSoldier()
    {
        //TODO:
        Log.i("Spawn solider...");
        GameObject go = GameObject.Instantiate(Resources.Load("Character") as GameObject);
        go.transform.position = CheckPointMgr.S.GetSoldierSpawnPos();
        RoleController role = go.GetComponent<RoleController>();
        m_RoleList.Add(role);

        role.IdleCallBack = IdleCallBack;
        role.RunOverCallBack = RunOverCallBack;
        role.InitializeCallBack = InitializeCallBack;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 0, 1, 0.3f);
        Gizmos.DrawWireSphere(m_SoldierTargetPos, m_MoveRandomRadius);
    }

    private void InitializeCallBack(RoleController role)
    {
        //Debug.LogError(role.RoleID + " -- InitializeCallBack");
        role.SetTargetID(m_RoleList.Count);
        role.SetRoleCamp(RoleCamp.Red);
        role.SetRoleType(RoleType.Role);
        role.SetRoleHP(200);
        role.LoadMaterial();

        m_SoldierTargetPos = CheckPointMgr.S.GetSoldierTargetPos();
        StartCoroutine(SetMoveTargetPos(role, m_SoldierTargetPos));
    }

    private void IdleCallBack(RoleController role)
    {
        //Debug.LogError(role.RoleID + " -- IdleCallBack");
        m_SoldierTargetPos = CheckPointMgr.S.GetSoldierTargetPos();
        StartCoroutine(SetMoveTargetPos(role, m_SoldierTargetPos));
    }

    private void RunOverCallBack(RoleController role)
    {
        //Debug.LogError(role.RoleID + " -- RunOverCallBack -- " + role.MoveTargetPosition);
        if (m_TargetPos.Contains(role.MoveTargetPosition))
        {
            m_TargetPos.Remove(role.MoveTargetPosition);
        }
    }

    private IEnumerator SetMoveTargetPos(RoleController role, Vector3 getPos)
    {
        if(m_TargetPos.Count >= 20)
        {
            m_TargetPos.Clear();
        }

        Vector3 targetPos = Vector3.zero;

        bool result = true;

        while (result)
        {
            Vector2 randomF = Random.insideUnitCircle * m_MoveRandomRadius;
            targetPos = new Vector3(getPos.x + randomF.x, getPos.y, getPos.z + randomF.y);
            bool canUse = IsPosCanUse(role, targetPos);
            if (canUse)
            {
                result = false;
            }
            else
            {
                //Debug.LogError(role.RoleID + " -- 未找到合适的目标点");
                yield return new WaitForSeconds(0.5f);
            }
        }

        m_TargetPos.Add(targetPos);
        role.SetMove(targetPos);

        //Debug.LogError(role.RoleID + " -- " + targetPos);
    }

    private bool IsPosCanUse(RoleController role, Vector3 pos)
    {
        float judgeDistance = 2f;
        bool result = true;

        foreach (var item in m_TargetPos)
        {
            float distance = Vector3.Distance(pos, item);
            //Debug.LogError(role.RoleID + " -- " + pos + " -- " + item + " -- " + distance);
            if (distance <= judgeDistance)
            {
                result = false;
                break;
            }
        }
        return result;
    }

    public void RemoveRole(RoleController role)
    {
        if (m_RoleList.Contains(role))
        {
            m_RoleList.Remove(role);
        }
    }

    public List<RoleController> GetRoleControllerInRange(Vector3 centerPos, float range, RoleCamp roleCamp)
    {
        List<RoleController> roleList = m_RoleList.Where(i => i.Camp == roleCamp && Vector3.Distance(centerPos, i.transform.position) < range).ToList();
        return roleList;
    }
}
