using System.Collections;
using System.Collections.Generic;
using Qarth;
using UnityEngine;
using UnityEngine.UI;


namespace GameWish.Game
{
	public class RealNameTipPopPanel : AbstractPanel
    {
        [SerializeField] private Text m_TitleText;
        [SerializeField] private Text m_ContentText;

        protected override void OnUIInit()
        {
            base.OnUIInit();
        }

        protected override void OnPanelOpen(params object[] args)
        {
            base.OnPanelOpen(args);

            if (args.Length > 0)
            {
                string title = args[0] as string;
                string content = args[1] as string;

                m_TitleText.text = title;
                m_ContentText.text = content;
            }

            StartCoroutine(DelayClose());
        }

        protected override void OnClose()
        {
            base.OnClose();
        }

        private IEnumerator DelayClose()
        {
            yield return new WaitForSeconds(2);
            CloseSelfPanel();
        }
    }
	
}