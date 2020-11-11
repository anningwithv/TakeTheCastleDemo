using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Qarth;
using Cinemachine;

public class CinemachineMgr : TMonoSingleton<CinemachineMgr>
{
    public List<CinemachineVirtualCamera> virtualCameraList = new List<CinemachineVirtualCamera>();

    private int m_SelectedVirtualCameraIndex = 0;
    private CinemachineVirtualCamera m_CurVirtualCamera = null;

    private void Start()
    {
        m_CurVirtualCamera = virtualCameraList[m_SelectedVirtualCameraIndex];
        m_CurVirtualCamera.Priority = 10;
    }

    public void OnSelectNextVirtualCamera()
    {
        if (m_CurVirtualCamera != null)
            m_CurVirtualCamera.Priority = 1;

        m_SelectedVirtualCameraIndex++;
        m_SelectedVirtualCameraIndex = Mathf.Clamp(m_SelectedVirtualCameraIndex, 0, virtualCameraList.Count - 1);

        m_CurVirtualCamera = virtualCameraList[m_SelectedVirtualCameraIndex];
        m_CurVirtualCamera.Priority = 10;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            OnSelectNextVirtualCamera();
        }
    }
}
