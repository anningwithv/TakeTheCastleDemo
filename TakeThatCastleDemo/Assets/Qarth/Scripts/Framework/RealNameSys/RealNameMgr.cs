using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GameWish.Game;

namespace Qarth
{
    public class RealNameMgr : TSingleton<RealNameMgr>
    {
        private Action m_PopConfirmCallback;
        private int m_LimitTimer = -1;
        private int m_LimitCheckCount = 180;//tick 180次 1.5h
        private int m_GuestCheckCount = 120;//tick 120次 游客时间
        private int m_LimitCheckDuration = 30;//timer 30s 间隔
        private int m_TimeLimiteMin = 8;
        private int m_TimeLimiteMax = 22;

        public bool hasInit;

        private RealNameHelper.RealNameAgeType m_RealNameAgeType = RealNameHelper.RealNameAgeType.None;

        public RealNameHelper.RealNameAgeType GetRealNameState
        {
            get
            {
                return m_RealNameAgeType;
            }
        }

        public void Init()
        {
            // 目前做法是拉取不到配置的情况下,不初始化
            EventSystem.S.Register(EngineEventID.OnRealNameRemoteConfFetched, OnRealNameRemoteConfFetched);
        }

        //本地初始化 不适用云配
        public void InitLocal()
        {
            DoInitLogic();
        }

        void OnRealNameRemoteConfFetched(int key, params object[] args)
        {
            if (RealNameRemoteConfMgr.S.data.open_enter)
            {
                DoInitLogic();
            }
        }

        void DoInitLogic()
        {
            hasInit = true;

            m_RealNameAgeType = (RealNameHelper.RealNameAgeType)PlayerPrefs.GetInt(RealNameHelper.REALNAME_STATE_KEY, -1);

            EventSystem.S.Send(EngineEventID.OnRealNameInit);

            switch (m_RealNameAgeType)
            {
                case RealNameHelper.RealNameAgeType.None:
                    HandleInit();
                    break;
                case RealNameHelper.RealNameAgeType.Age16to18:
                case RealNameHelper.RealNameAgeType.Age8to16:
                case RealNameHelper.RealNameAgeType.Age0to8:
                    if (DateTime.Now.Hour < m_TimeLimiteMin || DateTime.Now.Hour >= m_TimeLimiteMax)
                        PopQuitWarning();
                    else 
                        CheckPlayTime(RealNameHelper.REALNAME_TIME_LIMIT_START_WORDS);
                    break;
                case RealNameHelper.RealNameAgeType.Over18:
                    break;
            }
        }

        private void HandleInit()
        {
            if (CustomExtensions.CheckIsNewDay(RealNameHelper.REALNAME_DAILY_KEY) > 0)
            {
                PlayerPrefs.SetInt(RealNameHelper.REALNAME_GUEST_PLAY_TIME, 0);
                //EventSystem.S.Send(EngineEventID.OnRealNameFirstIn);
                UIMgr.S.OpenTopPanel(EngineUI.RealNameSkipPanel,null);
            }
            else
            {
                int guestCount = PlayerPrefs.GetInt(RealNameHelper.REALNAME_GUEST_PLAY_TIME, 0);
                if (guestCount > 0)
                {
                    //以游客身份玩过
                    CheckGuestPlayTime();
                }
                else
                {
                    //新用户
                    //EventSystem.S.Send(EngineEventID.OnRealNameFirstIn);
                    UIMgr.S.OpenTopPanel(EngineUI.RealNameSkipPanel, null);
                }
            }
        }


        public void RegisterPanels()
        {
            UIDataTable.SetABMode(false);
            UIDataTable.AddPanelData(EngineUI.RealNameCommitPanel, null, "RealNamePanel/RealNameCommitPanel", true);
            UIDataTable.AddPanelData(EngineUI.RealNamePopPanel, null, "RealNamePanel/RealNamePopPanel", true);
            UIDataTable.AddPanelData(EngineUI.RealNameSkipPanel, null, "RealNamePanel/RealNameSkipPanel");
            UIDataTable.AddPanelData(EngineUI.RealNameTipPopPanel, null, "RealNamePanel/RealNameTipPopPanel");
            
            UIDataTable.AddPanelData(EngineUI.RealNameGuestFinishPanel, null, "RealNamePanel/RealNameGuestFinishPanel");
            UIDataTable.AddPanelData(EngineUI.RealNameChildWarningPopPanel, null, "RealNamePanel/RealNameChildWarningPopPanel");
            UIDataTable.AddPanelData(EngineUI.RealNameChildProtectedPanel, null, "RealNamePanel/RealNameChildProtectedPanel");
           
        }

        private void SetPopConfirmCallback(Action callback)
        {
            m_PopConfirmCallback = callback;
        }

        public bool InvokePopConfirmCallback()
        {
            if (m_PopConfirmCallback != null)
            {
                m_PopConfirmCallback.Invoke();
                m_PopConfirmCallback = null;
                return true;
            }
            return false;
        }

        //实名认证成功
        public void DoConfirmSuccess()
        {
            m_RealNameAgeType = (RealNameHelper.RealNameAgeType)PlayerPrefs.GetInt(RealNameHelper.REALNAME_STATE_KEY, -1);

            EventSystem.S.Send(EngineEventID.OnRealNameValid, m_RealNameAgeType);

            string title = "实名认证成功";
            string content = "";
            
            switch (m_RealNameAgeType)
            {
                case RealNameHelper.RealNameAgeType.Age16to18:
                    content = "您的年龄在（16-18岁），属于未成年人，将受到防沉迷限制";
                    if (DateTime.Now.Hour < m_TimeLimiteMin || DateTime.Now.Hour >= m_TimeLimiteMax)
                        PopQuitWarning();
                    else
                        CheckPlayTime(RealNameHelper.REALNAME_TIME_LIMIT_START_WORDS);
                    break;
                    
                case RealNameHelper.RealNameAgeType.Age8to16:
                    content = "您的年龄在（8-16岁），属于未成年人，将受到防沉迷限制";
                    if (DateTime.Now.Hour < m_TimeLimiteMin || DateTime.Now.Hour >= m_TimeLimiteMax)
                        PopQuitWarning();
                    else
                        CheckPlayTime(RealNameHelper.REALNAME_TIME_LIMIT_START_WORDS);
                    break;
                    
                case RealNameHelper.RealNameAgeType.Age0to8:
                    content = "您的年龄小于8岁，属于未成年人，将受到防沉迷限制";
                    if (DateTime.Now.Hour < m_TimeLimiteMin || DateTime.Now.Hour >= m_TimeLimiteMax)
                        PopQuitWarning();
                    else
                         CheckPlayTime(RealNameHelper.REALNAME_TIME_LIMIT_START_WORDS);
                    break;
                case RealNameHelper.RealNameAgeType.Over18:
                    content = "您是成年人，将不受到防沉迷限制";
                    break;
            }
            
            UIMgr.S.OpenTopPanel(EngineUI.RealNameTipPopPanel, null, title, content);
        }
        //实名认证跳过
        public void DoConfirmCancle()
        {
            EventSystem.S.Send(EngineEventID.OnRealNameCancle);

            CheckGuestPlayTime();

            float hour = m_GuestCheckCount * m_LimitCheckDuration/3600f;
            string content = string.Format("仅可体验{0}小时，超时后无法游戏",hour.ToString("N1"));
            UIMgr.S.OpenTopPanel(EngineUI.RealNameTipPopPanel, null, "您当前是游客模式",content );
        }

        public void CheckGuestPlayTime()
        {
            bool isOver = false;
            if (CustomExtensions.CheckIsNewDay(RealNameHelper.REALNAME_DAILY_KEY) > 0)
            {
                PlayerPrefs.SetInt(RealNameHelper.REALNAME_GUEST_PLAY_TIME, 0);
            }
            else
            {
                isOver = CheckGuestTimeOut();
            }

            if (!isOver)
            {
                if (m_LimitTimer != -1)
                {
                    Timer.S.Cancel(m_LimitTimer);
                }
                
                m_LimitTimer = Timer.S.Post2Really(OnGuestTimerTick, m_LimitCheckDuration, -1);
            }
            else
            {
                PopGuestTimeOutWarning();
            }
        }

        public void CheckPlayTime(string desc)
        {
            bool isOver = false;
            if (CustomExtensions.CheckIsNewDay(RealNameHelper.REALNAME_DAILY_KEY) > 0)
            {
                PlayerPrefs.SetInt(RealNameHelper.REALNAME_LIMIT_PLAY_TIME, 0);
            }
            else
            {
                isOver = CheckLimitTimeOut();
            }

            if (!isOver)
            {
                if (m_LimitTimer != -1)
                {
                    Timer.S.Cancel(m_LimitTimer);
                }

                m_LimitTimer = Timer.S.Post2Really(OnLimitTimerTick, m_LimitCheckDuration, -1);
                //SetPopConfirmCallback(() =>
                //{
                //    EventSystem.S.Send(EngineEventID.OnRealNameValidOver18, false);
                //    m_LimitTimer = Timer.S.Post2Really(OnLimitTimerTick, m_LimitCheckDurtion, -1);
                //    UIMgr.S.ClosePanelAsUIID(EngineUI.RealNameCommitPanel);
                //});
                //UIMgr.S.OpenTopPanel(EngineUI.RealNamePopPanel, null, "提示", desc, true);
            }
            else
                PopQuitWarning();
        }

        public void PopNormalWarning(string desc)
        {
            UIMgr.S.OpenTopPanel(EngineUI.RealNamePopPanel, null, "提示", desc, false);
        }

        //未成年人防沉迷提示
        public void PopQuitWarning()
        {
            //SetPopConfirmCallback(() =>
            //{
            //    Quit();
            //});
            //UIMgr.S.OpenTopPanel(EngineUI.RealNamePopPanel, null, "未成年人防沉迷提示", desc,
            //true);

            UIMgr.S.OpenTopPanel(EngineUI.RealNameChildWarningPopPanel,null);
        }

        public void PopGuestTimeOutWarning()
        {
            UIMgr.S.OpenTopPanel(EngineUI.RealNameGuestFinishPanel, null);
        }

        void OnGuestTimerTick(int count)
        {
            var t = PlayerPrefs.GetInt(RealNameHelper.REALNAME_GUEST_PLAY_TIME, 0);
            PlayerPrefs.SetInt(RealNameHelper.REALNAME_GUEST_PLAY_TIME, t + 1);
            CheckGuestTimeOut();

            //if (t == m_GuestCheckCount - 10)
            //{
            //    PopNormalWarning(RealNameHelper.REALNAME_TIME_LIMIT_NEAR_WORDS);
            //}
        }

        void OnLimitTimerTick(int count)
        {
            var min = PlayerPrefs.GetInt(RealNameHelper.REALNAME_LIMIT_PLAY_TIME, 0);
            PlayerPrefs.SetInt(RealNameHelper.REALNAME_LIMIT_PLAY_TIME, min + 1);
            CheckLimitTimeOut();

            //var limitCheckCount = RealNameHelper.IsHoliday(DateTime.Now) ? m_LimitCheckCount * 2 : m_LimitCheckCount;
            //if (min == limitCheckCount - 10)
            //{
            //    PopNormalWarning(RealNameHelper.REALNAME_TIME_LIMIT_NEAR_WORDS);
            //}
        }

        //判断时间是否结束
        private bool CheckLimitTimeOut()
        {
            var limitCheckCount = RealNameHelper.IsHoliday(DateTime.Now) ? m_LimitCheckCount * 2 : m_LimitCheckCount;

            if (DateTime.Now.Hour >= m_TimeLimiteMax || PlayerPrefs.GetInt(RealNameHelper.REALNAME_LIMIT_PLAY_TIME, 0) >= limitCheckCount)
            {
                if (m_LimitTimer > 0)
                {
                    Timer.S.Cancel(m_LimitTimer);
                    m_LimitTimer = -1;
                    PopQuitWarning();
                }

                return true;
            }
            return false;
        }

        private bool CheckGuestTimeOut()
        {
            if (PlayerPrefs.GetInt(RealNameHelper.REALNAME_GUEST_PLAY_TIME, 0) >= m_GuestCheckCount)
            {
                if (m_LimitTimer > 0)
                {
                    Timer.S.Cancel(m_LimitTimer);
                    m_LimitTimer = -1;
                    PopGuestTimeOutWarning();
                }

                return true;
            }
            return false;
        }

        public void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            //Application.Quit();
            System.Diagnostics.Process.GetCurrentProcess().Kill();
#endif
        }
    }
}