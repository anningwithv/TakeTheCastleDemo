using System.Collections;
using System.Collections.Generic;
using Qarth;
using UnityEngine;
using UnityEngine.UI;


namespace GameWish.Game
{
	public class RealNameSkipPanel : AbstractPanel
    {
        [SerializeField] private Button m_SkipBtn;
        [SerializeField] private Button m_ConfirmBtn;
        [SerializeField] private Button m_ProtectBtn;

        protected override void OnUIInit()
        {
            base.OnUIInit();

            m_SkipBtn.onClick.AddListener(delegate
            {
                CloseSelfPanel();
                RealNameMgr.S.DoConfirmCancle();
            });

            m_ConfirmBtn.onClick.AddListener(delegate
            {
                UIMgr.S.OpenTopPanel(EngineUI.RealNameCommitPanel,null);
            });

            m_ProtectBtn.onClick.AddListener(delegate
            {
                UIMgr.S.OpenTopPanel(EngineUI.RealNameChildProtectedPanel, null);
            });
        }

        protected override void OnPanelOpen(params object[] args)
        {
            base.OnPanelOpen(args);
            RegisterEvent(EngineEventID.OnRealNameValid,HandleEvent);
            RegisterEvent(EngineEventID.OnRealNameCancle,HandleEvent);

            if (PlayerPrefs.GetInt(RealNameHelper.RealName_PanelShow_Confirm2ndPanel, 0) < 1)
            {
                //DataAnalysisMgr.S.CustomEvent("RealName_panelShow", "Confirm2ndPanel");
                PlayerPrefs.SetInt(RealNameHelper.RealName_PanelShow_Confirm2ndPanel, 1);
            }
        }

        protected override void OnClose()
        {
            base.OnClose();
        }

        private void HandleEvent(int id ,params object[] param)
        {
            CloseSelfPanel();
        }
    }
	
}