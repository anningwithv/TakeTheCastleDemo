using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if UNITY_2018 || UNITY_2019
using UnityEngine.Networking;
#endif

namespace Qarth
{
#if UNITY_2018 || UNITY_2019
    public class BypassCertificate : CertificateHandler
    {
        protected override bool ValidateCertificate(byte[] certificateData)
        {
            //Simply return true no matter what
            return true;
        }
    }
#endif

    public class RemoteConfData
    {
        /// <summary>
        /// 
        /// </summary>
        public string app { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string k { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ns { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string v { get; set; }
    }

    public class RemoteConfResponse
    {
        /// <summary>
        /// 
        /// </summary>
        public RemoteConfData data { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string msg { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool success { get; set; }
    }
}