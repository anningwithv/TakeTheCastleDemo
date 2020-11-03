using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Qarth
{
    public class RealNameHelper
    {
        public enum RealNamePopType
        {
            Success,
            Invalid,
            Warning,
        }

        public enum RealNameAgeType
        {
            None=-1,
            Over18=0,
            Age8to16=1,
            Age0to8=2,
            Age16to18,
        }

        //打点存档
        public const string RealName_PanelShow_BonusPanel = "RealName_PanelShow_BonusPanel";
        public const string RealName_PanelShow_Confirm2ndPanel = "RealName_PanelShow_Confirm2ndPanel";
        public const string RealName_PanelShow_BindWechat = "RealName_PanelShow_BindWechat";
        public const string RealName_PanelShow_RulesPanel = "RealName_PanelShow_RulesPanel";
        public const string RealName_From_BonusPanel = "RealName_From_BonusPanel";
        public const string RealName_From_ForcePanel_Guest = "RealName_From_ForcePanel_Guest";
        public const string RealName_ForceOffline = "RealName_ForceOffline";

        public const string REALNAME_STATE_KEY = "real_name_sys_record";
        public const string REALNAME_DAILY_KEY = "real_name_sys_new_day_key";
        public const string REALNAME_LIMIT_PLAY_TIME = "real_name_sys_limit_play_time";
        public const string REALNAME_GUEST_PLAY_TIME = "real_name_sys_guest_play_time";

        public const string REALNAME_REWARD_GET = "real_name_reward_get";
        public const string REALNAME_REWARD_MISSION_ID = "real_name_mission";

        public const string REALNAME_NEEDS = "请填写身份信息";
        public const string REALNAME_ID_ERROR = "身份信息格式不正确,请检查重试";
        public const string REALNAME_AGE_LIMIT_WORDS = "根据国家相关法律规定，未满8周岁的用户限制进入游戏";

        public const string REALNAME_TIME_LIMIT_OVER_WORDS = "您的账号已到达每日游戏时间上限,即将强制离线,请注意休息";
        public const string REALNAME_TIME_LIMIT_RESTART_WORDS = "您的账号已用完每日游戏时间上限，请明日8点以后再次登陆";
        public const string REALNAME_TIME_LIMIT_AFTER22_WORDS = "您的账号已被纳入防沉迷系统,每日22点00分至次日8点00分不能登录游戏";
        public const string REALNAME_TIME_LIMIT_NEAR_WORDS = "您的账号还有5分钟即将被强制下线";
        public const string REALNAME_TIME_LIMIT_START_WORDS = "您的账号已被纳入防沉迷系统,每日游戏时间为1.5小时（节假日3小时）,每日22点00分至次日8点00分不能登录游戏,请合理安排游戏时间";

        public static string[] HOLIDAY_DATE_2020 = { "01-01",
            "01-24", "01-25", "01-26", "01-27", "01-28", "01-29", "01-30", "01-31", "02-01", "02-02",
            "04-04", "04-05", "04-07",
            "05-01", "05-02", "05-03", "05-04", "05-05",
            "06-25", "06-26", "06-27",
            "10-01", "10-02", "10-03", "10-04", "10-05", "10-06", "10-07", "10-08" };
        public static string[] HOLIDAY_DATE_2021 = { "01-01",
            "02-11", "02-12", "02-13", "02-14", "02-15", "02-16", "02-17",
            "04-03", "04-04", "04-05",
            "05-01", "05-02", "05-03",
            "06-12", "06-13", "06-14",
            "09-19", "09-20", "09-21",
            "10-01", "10-02", "10-03", "10-04", "10-05", "10-06", "10-07" };

        // 验证18岁
        public static RealNameAgeType ValidAge18(string idNumber)
        {
            var dateStr = idNumber.Substring(6, 8);
            dateStr = dateStr.Substring(0, 4) + "-" + dateStr.Substring(4, 2) + "-" + dateStr.Substring(6);
            DateTime birthday;

            if (DateTime.TryParse(dateStr, out birthday))
            {
                //Log.e(birthday);
                var disYear = DateTime.Now.Year - birthday.Year;
                var disMonth = DateTime.Now.Month - birthday.Month;
                if (disYear < 8)
                {
                    //DataAnalysisMgr.S.CustomEvent("RealName_SuccessAges", "0_8");
                    return RealNameAgeType.Age0to8; 
                }
                else if (disYear > 18)
                {
                    //DataAnalysisMgr.S.CustomEvent("RealName_SuccessAges", "18plus");
                    return RealNameAgeType.Over18;
                }
                else if (disYear == 18)
                {
                    if (disMonth >= 0)
                    {
                        //DataAnalysisMgr.S.CustomEvent("RealName_SuccessAges", "18plus");
                        return RealNameAgeType.Over18;
                    }
                    else
                    {
                        //DataAnalysisMgr.S.CustomEvent("RealName_SuccessAges", "16_18");
                        return RealNameAgeType.Age16to18;
                    }

                }
                else if(disYear>16)
                {
                    //DataAnalysisMgr.S.CustomEvent("RealName_SuccessAges", "16_18");
                    return RealNameAgeType.Age16to18;
                }
                else if(disYear==16)
                {
                    if (disMonth >= 0)
                    {
                        //DataAnalysisMgr.S.CustomEvent("RealName_SuccessAges", "16_18");
                        return RealNameAgeType.Age16to18;
                    }
                    else
                    {
                        //DataAnalysisMgr.S.CustomEvent("RealName_SuccessAges", "8_16");
                        return RealNameAgeType.Age8to16;
                    }
                }
                else
                {
                    //DataAnalysisMgr.S.CustomEvent("RealName_SuccessAges", "8_16");
                    return RealNameAgeType.Age8to16;
                }
            }

            //DataAnalysisMgr.S.CustomEvent("RealName_SuccessAges", "0_8");
            return RealNameAgeType.Age0to8;
        }

        /// <summary>  
        /// 验证身份证合理性  
        /// </summary>  
        /// <param name="Id"></param>  
        /// <returns></returns>  
        public static bool ValidIDCard(string idNumber)
        {
            if (idNumber.Length == 18)
            {
                bool check = ValidIDCard18(idNumber);
                return check;
            }
            else if (idNumber.Length == 15)
            {
                bool check = ValidIDCard15(idNumber);
                return check;
            }
            else
            {
                return false;
            }
        }


        /// <summary>  
        /// 18位身份证号码验证  
        /// </summary>  
        private static bool ValidIDCard18(string idNumber)
        {
            long n = 0;
            if (long.TryParse(idNumber.Remove(17), out n) == false
                || n < Math.Pow(10, 16) || long.TryParse(idNumber.Replace('x', '0').Replace('X', '0'), out n) == false)
            {
                return false;//数字验证  
            }
            string address = "11x22x35x44x53x12x23x36x45x54x13x31x37x46x61x14x32x41x50x62x15x33x42x51x63x21x34x43x52x64x65x71x81x82x91";
            if (address.IndexOf(idNumber.Remove(2)) == -1)
            {
                return false;//省份验证  
            }
            string birth = idNumber.Substring(6, 8).Insert(6, "-").Insert(4, "-");
            DateTime time = new DateTime();
            if (DateTime.TryParse(birth, out time) == false)
            {
                return false;//生日验证  
            }
            string[] arrVarifyCode = ("1,0,x,9,8,7,6,5,4,3,2").Split(',');
            string[] Wi = ("7,9,10,5,8,4,2,1,6,3,7,9,10,5,8,4,2").Split(',');
            char[] Ai = idNumber.Remove(17).ToCharArray();
            int sum = 0;
            for (int i = 0; i < 17; i++)
            {
                sum += int.Parse(Wi[i]) * int.Parse(Ai[i].ToString());
            }
            int y = -1;
            Math.DivRem(sum, 11, out y);
            if (arrVarifyCode[y] != idNumber.Substring(17, 1).ToLower())
            {
                return false;//校验码验证  
            }
            return true;//符合GB11643-1999标准  
        }


        /// <summary>  
        /// 15位身份证号码验证  
        /// </summary>  
        private static bool ValidIDCard15(string idNumber)
        {
            long n = 0;
            if (long.TryParse(idNumber, out n) == false || n < Math.Pow(10, 14))
            {
                return false;//数字验证  
            }
            string address = "11x22x35x44x53x12x23x36x45x54x13x31x37x46x61x14x32x41x50x62x15x33x42x51x63x21x34x43x52x64x65x71x81x82x91";
            if (address.IndexOf(idNumber.Remove(2)) == -1)
            {
                return false;//省份验证  
            }
            string birth = idNumber.Substring(6, 6).Insert(4, "-").Insert(2, "-");
            DateTime time = new DateTime();
            if (DateTime.TryParse(birth, out time) == false)
            {
                return false;//生日验证  
            }
            return true;
        }

        public static bool IsHoliday(DateTime dateTime)
        {
            string shortDT = dateTime.ToString("MM-dd");
            //Log.e(shortDT);
            if (dateTime.Year == 2020)
            {
                return Array.IndexOf(HOLIDAY_DATE_2020, shortDT) >= 0;
            }
            else if (dateTime.Year == 2021)
            {
                return Array.IndexOf(HOLIDAY_DATE_2021, shortDT) >= 0;
            }
            else
            {
                switch (shortDT)
                {
                    case "01-01":
                    case "04-05":
                    case "05-01":
                    case "10-01":
                    case "10-02":
                    case "10-03":
                    case "10-04":
                    case "10-05":
                    case "10-06":
                    case "10-07":
                        return true;
                }
            }

            return false;
        }
    }
}