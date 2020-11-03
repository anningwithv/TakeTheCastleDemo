using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Qarth;

public class SoldierSpawnMgr : TMonoSingleton<SoldierSpawnMgr>
{
    private List<RoleController> m_RoleList = new List<RoleController>();

    public void SpawnSoldier()
    {
        //TODO:
        Log.i("Spawn solider...");
        GameObject go = GameObject.Instantiate(Resources.Load("StoneKing") as GameObject);
        go.transform.position = CheckPointMgr.S.GetSoldierSpawnPos();
        RoleController role = go.GetComponent<RoleController>();
        role.StartMove(CheckPointMgr.S.GetSoldierTargetPos());
        m_RoleList.Add(role);
    }
}
