using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Qarth
{
    public class RealNameRemoteConfMgr : TSingleton<RealNameRemoteConfMgr>
    {
        private Dictionary<string, string> m_Headers = new Dictionary<string, string>();
        public class ParamsResponse
        {
            public bool open_enter;

            public bool open_guest_close;
            public bool open_child_close;
        }
        private ParamsResponse m_Data;
        public ParamsResponse data
        {
            get { return m_Data; }
        }

        public void Init(string url, string appName, string channel = "all")
        {
            if (!m_Headers.ContainsKey("Content-Encoding"))
                m_Headers.Add("Content-Encoding", "gzip");
            else
                m_Headers["Content-Encoding"] = "gzip";

            if (!string.IsNullOrEmpty(url) && !string.IsNullOrEmpty(appName))
            {
                CustomExtensions.FetchRemoteConfParams(
                    appName,
                    "real_name_params",
                    OnRemoteValueFetched,
                    channel,
                    url,
                    m_Headers);
            }
            RealNameMgr.S.Init();
        }

        void OnRemoteValueFetched(string value)
        {
            m_Data = LitJson.JsonMapper.ToObject<ParamsResponse>(value);
            EventSystem.S.Send(EngineEventID.OnRealNameRemoteConfFetched);
        }
    }
}