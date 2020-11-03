using System.Collections;
using System.Collections.Generic;
using Qarth;
using UnityEngine;
using UnityEngine.UI;


namespace GameWish.Game
{
	public class RealNameChildProtectedPanel : AbstractPanel
    {
        [SerializeField] private Button m_CloseBtn;

        protected override void OnUIInit()
        {
            base.OnUIInit();

            m_CloseBtn.onClick.AddListener(delegate
            {
                CloseSelfPanel();
            });
        }

        protected override void OnPanelOpen(params object[] args)
        {
            base.OnPanelOpen(args);

            if (PlayerPrefs.GetInt(RealNameHelper.RealName_PanelShow_RulesPanel, 0) < 1)
            {
                //DataAnalysisMgr.S.CustomEvent("RealName_panelShow", "RulesPanel");
                PlayerPrefs.SetInt(RealNameHelper.RealName_PanelShow_RulesPanel, 1);
            }
        }

        protected override void OnClose()
        {
            base.OnClose();
        }
    }
	
}