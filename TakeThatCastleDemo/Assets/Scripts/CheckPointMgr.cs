using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Qarth;
using System.Linq;

public class CheckPointMgr : TMonoSingleton<CheckPointMgr>
{
    public List<CheckPoint> checkPointList = new List<CheckPoint>();

    private CheckPoint m_CurCheckPoint = null;
    private CheckPoint m_TargetCheckPoint = null;

    public CheckPoint CurCheckPoint { get => m_CurCheckPoint; }
    public CheckPoint TargetCheckPoint { get => m_TargetCheckPoint; }

    private void Start()
    {
        if (checkPointList.Count > 0)
        {
            m_CurCheckPoint = checkPointList[0];
            m_TargetCheckPoint = checkPointList.Where(i => i.index == m_CurCheckPoint.index + 1).FirstOrDefault();
        }

    }

    public void OnCheckPointTriggered(CheckPoint checkPoint)
    {
        if (checkPoint == checkPointList[checkPointList.Count - 1])
        {
            Debug.LogError("Game win");
            foreach (var item in SoldierSpawnMgr.S.RoleList)
            {
                item.SetStatus(RoleStatus.Win);
            }
        }
        else
        {
            if (checkPoint.index > m_CurCheckPoint.index)
            {
                m_CurCheckPoint = checkPoint;
                m_TargetCheckPoint = checkPointList.Where(i => i.index == m_CurCheckPoint.index + 1).FirstOrDefault();

                CinemachineMgr.S.OnSelectNextVirtualCamera();
            }

            if (m_CurCheckPoint.index == 0)
            {
                CinemachineMgr.S.OnSelectNextVirtualCamera();
            }
        }
    }

    public Vector3 GetSoldierSpawnPos()
    {
        return m_CurCheckPoint.transform.position;
    }

    public Vector3 GetSoldierTargetPos()
    {
        if (m_TargetCheckPoint != null)
        {
            return m_TargetCheckPoint.transform.position;
        }

        return checkPointList.LastOrDefault().transform.position;
    }
}
