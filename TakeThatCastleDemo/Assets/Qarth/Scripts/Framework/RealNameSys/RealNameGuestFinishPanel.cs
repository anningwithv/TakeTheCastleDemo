using System.Collections;
using System.Collections.Generic;
using Qarth;
using UnityEngine;
using UnityEngine.UI;


namespace GameWish.Game
{
	public class RealNameGuestFinishPanel : AbstractPanel
    {
        [SerializeField] private Text m_ContentText;
        [SerializeField] private Button m_CloseBtn;
        [SerializeField] private Button m_ConfirmBtn;


        protected override void OnUIInit()
        {
            base.OnUIInit();

            m_CloseBtn.onClick.AddListener(delegate
            {
                CloseSelfPanel();
            });

            m_ConfirmBtn.onClick.AddListener(delegate
            {
                UIMgr.S.OpenTopPanel(EngineUI.RealNameCommitPanel,null);

                if (PlayerPrefs.GetInt(RealNameHelper.RealName_From_ForcePanel_Guest, 0) < 1)
                {
                    //DataAnalysisMgr.S.CustomEvent("RealName_from", "ForcePanel_Guest");
                    PlayerPrefs.SetInt(RealNameHelper.RealName_From_ForcePanel_Guest, 1);
                }
            });
        }

        protected override void OnPanelOpen(params object[] args)
        {
            base.OnPanelOpen(args);
            RegisterEvent(EngineEventID.OnRealNameValid, HandleEvent);
            RegisterEvent(EngineEventID.OnRealNameCancle, HandleEvent);

            m_CloseBtn.gameObject.SetActive(RealNameRemoteConfMgr.S.data.open_guest_close);
        }

        protected override void OnClose()
        {
            base.OnClose();
        }

        private void HandleEvent(int id, params object[] param)
        {
            CloseSelfPanel();
        }
    }
	
}