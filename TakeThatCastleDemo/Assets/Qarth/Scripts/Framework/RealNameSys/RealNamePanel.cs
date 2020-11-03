using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Qarth
{
    public class RealNamePanel : AbstractPanel
    {
        [SerializeField]
        private Button m_BtnAgree;
        [SerializeField]
        private Button m_BtnClose;

        [SerializeField]
        private Text m_TxtContent;


        protected override void OnUIInit()
        {
            base.OnUIInit();
            m_BtnAgree.onClick.AddListener(OnClickAgree);
            m_BtnClose.onClick.AddListener(OnClickClose);
            //m_BtnClose.gameObject.SetActive(RealNameRemoteConfMgr.S.data == null ?
            //    false : RealNameRemoteConfMgr.S.data.open_close);
        }

        protected override void OnOpen()
        {
            base.OnOpen();
            OpenDependPanel(EngineUI.MaskPanel, -1, null);
            m_TxtContent.text = string.Format(m_TxtContent.text, Application.productName);
        }

        protected override void OnClose()
        {
            base.OnClose();
        }

        void OnClickAgree()
        {
            UIMgr.S.OpenTopPanel(EngineUI.RealNameCommitPanel, null);
            CloseSelfPanel();
        }

        void OnClickClose()
        {
            CloseSelfPanel();
        }
    }
}
