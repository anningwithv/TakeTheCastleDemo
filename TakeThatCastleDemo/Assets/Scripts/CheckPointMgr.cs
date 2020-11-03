using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Qarth;
using System.Linq;

public class CheckPointMgr : TMonoSingleton<CheckPointMgr>
{
    public List<CheckPoint> checkPointList = new List<CheckPoint>();

    private CheckPoint m_CurCheckPoint = null;

    private void Start()
    {
        m_CurCheckPoint = checkPointList[0];
    }

    public void OnCheckPointTriggered(CheckPoint checkPoint)
    {
        if (checkPoint.index > m_CurCheckPoint.index)
        {
            m_CurCheckPoint = checkPoint;
        }
        else
        {
            if (checkPoint == checkPointList.LastOrDefault())
            {
                Log.i("Game win");
            }
        }
    }

    public Vector3 GetSoldierSpawnPos()
    {
        return m_CurCheckPoint.transform.position;
    }

    public Vector3 GetSoldierTargetPos()
    {
        CheckPoint checkPoint = checkPointList.Where(i => i.index == m_CurCheckPoint.index + 1).FirstOrDefault();
        if (checkPoint != null)
        {
            return checkPoint.transform.position;
        }

        return checkPointList.LastOrDefault().transform.position;
    }
}
