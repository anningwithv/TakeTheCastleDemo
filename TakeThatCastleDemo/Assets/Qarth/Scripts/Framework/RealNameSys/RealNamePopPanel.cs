using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Qarth
{
    public class RealNamePopPanel : AbstractPanel
    {
        [SerializeField]
        private Button m_BtnAgree;
        [SerializeField]
        private Button m_BtnBack;

        [SerializeField]
        private Text m_TxtTitle;

        [SerializeField]
        private Text m_TxtContent;


        protected override void OnUIInit()
        {
            base.OnUIInit();
            m_BtnAgree.onClick.AddListener(OnClickAgree);
            m_BtnBack.onClick.AddListener(OnClickBack);
        }

        protected override void OnPanelOpen(params object[] args)
        {
            base.OnPanelOpen(args);
            OpenDependPanel(EngineUI.MaskPanel, -1, null);
            if (args != null && args.Length > 2)
            {
                m_TxtTitle.text = args[0].ToString();
                m_TxtContent.text = args[1].ToString();
                var showConfirm = (bool)args[2];
                m_BtnAgree.gameObject.SetActive(showConfirm);
                m_BtnBack.gameObject.SetActive(!showConfirm);
            }
        }

        protected override void OnClose()
        {
            base.OnClose();
        }

        void OnClickAgree()
        {
            RealNameMgr.S.InvokePopConfirmCallback();
            CloseSelfPanel();
        }

        void OnClickBack()
        {
            CloseSelfPanel();
        }
    }
}
