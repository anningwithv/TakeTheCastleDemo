using System;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using GameWish.Game;

namespace Qarth
{
    [RequireComponent(typeof(Canvas))]
    public class AbstractAnimPanel : AbstractPanel
    {
        protected Action m_OnShowBeginListener;
        protected Action m_OnHideBeginListener;
        protected Action m_OnShowCompletedListener;
        protected Action m_OnHideCompletedListener;

        protected override void OnUIInit()
        {
            base.OnUIInit();
            DoozyEvtIntegration(true);
#if dUI_DoozyUI

            //if (uiID == (int)UIID.BlurMaskPanel)
            //{
            //    DoozyUI.UIElement element = GetComponent<DoozyUI.UIElement>();
            //    element.startHidden = true;
            //    element.animateAtStart = false;
            //    element.Init(true);
            //}
            //else
            {
                DoozyUI.UIElement[] elements = GetComponentsInChildren<DoozyUI.UIElement>();
                for (int i = 0; i < elements.Length; ++i)
                {
                    //if a panel is loaded on the fly, we force to set 'startHidden' to 'true' and 'animateAtStart' to 'false'
                    elements[i].startHidden = true;
                    elements[i].animateAtStart = false;

                    //do start after cloned immediately for every child uielement
                    elements[i].Init();
                }
            }
#endif
        }

        protected override void BeforDestroy()
        {
            base.BeforDestroy();
            DoozyEvtIntegration(false);
            m_ShowSubElementCalls.Clear();
            m_HideSubElementCalls.Clear();
        }

        protected override void OnOpen()
        {
            base.OnOpen();
//#if dUI_DoozyUI
//            if(uiID == (int)UIID.BlurMaskPanel)
//            {
//                DoozyUI.UIManager.ShowUiElement(uiName, false, true);
//            }
//            else
//            {
//                DoozyUI.UIManager.ShowUiElement(uiName);
//            }
//#endif
        }

        public void HideSelfWithAnim()
        {
#if dUI_DoozyUI
            DoozyUI.UIManager.HideUiElement(uiName, false);
#endif
        }

        public void HideSelfWithAnim(bool isSingle)
        {
#if dUI_DoozyUI
            DoozyUI.UIManager.HideUiElement(uiName, false, isSingle);
#endif
        }

        protected void DoozyEvtIntegration(bool state)
        {
            //根节点如果有UIElement，处理事件
            DoozyUI.UIElement doozyElem = GetComponent<DoozyUI.UIElement>();
            if (doozyElem != null)
            {
                if (state)
                {
                    doozyElem.OnInAnimationsStart.AddListener(OnPanelShowBegin);
                    //doozyElem.OnInAnimationsFinish.AddListener(OnPanelShowComplete);
                    doozyElem.OnOutAnimationsStart.AddListener(OnPanelHideBegin);
                    doozyElem.OnOutAnimationsFinish.AddListener(OnPanelHideComplete);
                }
                else
                {
                    doozyElem.OnInAnimationsStart.RemoveListener(OnPanelShowBegin);
                    //doozyElem.OnInAnimationsFinish.RemoveListener(OnPanelShowComplete);
                    doozyElem.OnOutAnimationsStart.RemoveListener(OnPanelHideBegin);
                    doozyElem.OnOutAnimationsFinish.RemoveListener(OnPanelHideComplete);
                }
            }

            DoozyUI.UIElement[] doozyElemArr = GetComponentsInChildren<DoozyUI.UIElement>();
            if (doozyElemArr != null && doozyElemArr.Length > 0)
            {
                DoozyUI.UIElement timeMaxElem = null;
                float timeTemp = 0;
                foreach (var item in doozyElemArr)
                {
                    float temp = item.inAnimations.TotalDuration;
                    //Debug.LogError("DoozyEvtIntegration : " + temp);
                    if (timeTemp <= temp)
                    {
                        timeTemp = temp;
                        timeMaxElem = item;
                    }
                }

                if (timeMaxElem != null)
                {
                    if (state)
                    {
                        timeMaxElem.OnInAnimationsFinish.AddListener(OnPanelShowComplete);
                        //Debug.LogError("DoozyEvtIntegration Show MaxTime: " + uiName + " -- " + timeMaxElem.inAnimations.TotalDuration);
                    }
                    else
                    {
                        timeMaxElem.OnInAnimationsFinish.RemoveListener(OnPanelShowComplete);
                        //Debug.LogError("DoozyEvtIntegration Close MaxTime: " + uiName + " -- " + timeMaxElem.inAnimations.TotalDuration);
                    }
                }
                else
                {
                    Debug.LogError("DoozyEvtIntegration No Animation: " + uiName);
                }
            }
        }


        protected virtual void OnPanelShowBegin()
        {
            Log.i(uiName + " show in");
            if (m_OnShowBeginListener != null)
                m_OnShowBeginListener.Invoke();
            m_OnShowBeginListener = null;
        }
        protected virtual void OnPanelShowComplete()
        {
            Log.i(uiName + " end in");
            if (m_OnShowCompletedListener != null)
                m_OnShowCompletedListener.Invoke();
            m_OnShowCompletedListener = null;
        }
        protected virtual void OnPanelHideBegin()
        {
            Log.i(uiName + " show out");
            if (m_OnHideBeginListener != null)
                m_OnHideBeginListener.Invoke();
            m_OnHideBeginListener = null;
        }
        protected virtual void OnPanelHideComplete()
        {
            Log.i(uiName + " end out");
            if (m_OnHideCompletedListener != null)
                m_OnHideCompletedListener.Invoke();
            m_OnHideCompletedListener = null;
        }


        #region DoozyFunc

        private Dictionary<string, UnityAction> m_ShowSubElementCalls = new Dictionary<string, UnityAction>();
        private Dictionary<string, UnityAction> m_HideSubElementCalls = new Dictionary<string, UnityAction>();

        public void ShowSubElements(string elementName, UnityAction callOnComplete = null)
        {
#if dUI_DoozyUI
            if (elementName == uiName)
            {
                return;
            }

            DoozyUI.UIElement[] elements = GetComponentsInChildren<DoozyUI.UIElement>();
            for (int i = 0; i < elements.Length; ++i)
            {
                if (elements[i] == this.GetComponent<DoozyUI.UIElement>() || elements[i].elementName != elementName || elements[i].isVisible)
                    continue;
                //一个元素始终只有一个一次性事件可以触发
                if (m_ShowSubElementCalls.ContainsKey(elementName))
                {
                    elements[i].OnInAnimationsFinish.RemoveListener(m_ShowSubElementCalls[elementName]);
                    m_ShowSubElementCalls.Remove(elementName);
                }

                if (callOnComplete != null)
                {
                    m_ShowSubElementCalls.Add(elementName, callOnComplete);
                    elements[i].OnInAnimationsFinish.AddListener(callOnComplete);
                }

                DoozyUI.UIManager.ShowUiElement(elementName);
            }
#endif
        }

        public void HideSubElements(string elementName, UnityAction callOnComplete = null)
        {
#if dUI_DoozyUI
            if (elementName == uiName)
            {
                return;
            }

            DoozyUI.UIElement[] elements = GetComponentsInChildren<DoozyUI.UIElement>();
            for (int i = 0; i < elements.Length; ++i)
            {
                if (elements[i] == this.GetComponent<DoozyUI.UIElement>() || elements[i].elementName != elementName || !elements[i].isVisible)
                    continue;
                //一个元素始终只有一个一次性事件可以触发
                if (m_HideSubElementCalls.ContainsKey(elementName))
                {
                    elements[i].OnOutAnimationsFinish.RemoveListener(m_HideSubElementCalls[elementName]);
                    m_HideSubElementCalls.Remove(elementName);
                }

                if (callOnComplete != null)
                {
                    m_HideSubElementCalls.Add(elementName, callOnComplete);
                    elements[i].OnOutAnimationsFinish.AddListener(callOnComplete);
                }

                DoozyUI.UIManager.HideUiElement(elementName,false);
            }
#endif
        }
        #endregion
    }
}