using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Qarth;
using System.Linq;

public class SoldierSpawnMgr : TMonoSingleton<SoldierSpawnMgr>
{
    private List<RoleController> m_RoleList = new List<RoleController>();

    private List<Vector3> m_TargetPos = new List<Vector3>();

    public void SpawnSoldier()
    {
        //TODO:
        Log.i("Spawn solider...");
        GameObject go = GameObject.Instantiate(Resources.Load("Character") as GameObject);
        go.transform.position = CheckPointMgr.S.GetSoldierSpawnPos();
        RoleController role = go.GetComponent<RoleController>();
        m_RoleList.Add(role);
        role.SetRoleID(m_RoleList.Count);
        role.SetRoleCamp(RoleCamp.Red);
        role.SetRoleHP(200);
        role.IdleCallBack = IdleCallBack;

        StartCoroutine(SetMoveTargetPos(role, CheckPointMgr.S.GetSoldierTargetPos()));
    }

    private void IdleCallBack(RoleController role)
    {
        StartCoroutine(SetMoveTargetPos(role, CheckPointMgr.S.GetSoldierTargetPos()));
    }

    private IEnumerator SetMoveTargetPos(RoleController role, Vector3 getPos)
    {
        Vector3 targetPos = Vector3.zero;

        float radius = 2f;
        bool result = true;

        while (result)
        {
            Vector2 randomF = Random.insideUnitCircle * radius;
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
        float judgeDistance = 0.2f;
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

    public List<RoleController> GetRoleControllerInRange(Vector3 centerPos, float range, RoleCamp roleCamp)
    {
        List<RoleController> roleList = m_RoleList.Where(i => i.Camp == roleCamp && Vector3.Distance(centerPos, i.transform.position) < range).ToList();
        return roleList;
    }
}
