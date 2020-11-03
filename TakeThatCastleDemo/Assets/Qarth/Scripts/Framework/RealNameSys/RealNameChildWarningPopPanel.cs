using System.Collections;
using System.Collections.Generic;
using Qarth;
using UnityEngine;
using UnityEngine.UI;


namespace GameWish.Game
{
	public class RealNameChildWarningPopPanel : AbstractPanel
    {
        [SerializeField] private Button m_CloseBtn;
        [SerializeField] private Button m_QuiteBtn;

        protected override void OnUIInit()
        {
            base.OnUIInit();

            m_CloseBtn.onClick.AddListener(delegate
            {
                CloseSelfPanel();
            });

            m_QuiteBtn.onClick.AddListener(delegate
            {
                RealNameMgr.S.Quit();
                if (PlayerPrefs.GetInt(RealNameHelper.RealName_ForceOffline, 0) < 1)
                {
                    //DataAnalysisMgr.S.CustomEvent(RealNameHelper.RealName_ForceOffline);
                    PlayerPrefs.SetInt(RealNameHelper.RealName_ForceOffline, 1);
                }
                
            });
        }

        protected override void OnPanelOpen(params object[] args)
        {
            base.OnPanelOpen(args);

            m_CloseBtn.gameObject.SetActive(RealNameRemoteConfMgr.S.data.open_child_close);

        }

        protected override void OnClose()
        {
            base.OnClose();
        }
    }
	
}