//  Desc:        Framework For Game Develop with Unity3d
//  Copyright:   Copyright (C) 2017 SnowCold. All rights reserved.
//  WebSite:     https://github.com/SnowCold/Qarth
//  Blog:        http://blog.csdn.net/snowcoldgame
//  Author:      SnowCold
//  E-mail:      snowcold.ouyang@gmail.com
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Threading;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Qarth.Editor
{
    public class TableExporter
    {
        public static OSPlatform GetOSPlatform()
        {
            //Set default as window
            OSPlatform osPlatform = OSPlatform.Windows;
            // Check if it's osx 
            var checker = RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
            osPlatform = checker ? OSPlatform.OSX : osPlatform;
            // Check if it's Linux 
            checker = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
            osPlatform = checker ? OSPlatform.Linux : osPlatform;
            return osPlatform;
        }


        public static bool IsLinuxSystem()
        {
            PlatformID platformID = System.Environment.OSVersion.Platform;

            //! mono's bug here: on macos the PlatformID is not MacOSX but Unix
            if (platformID == PlatformID.MacOSX || platformID == PlatformID.Unix)
            {
                return true;
            }

            return false;
        }

        public static bool IsPureUnixSystem()
        {
            return "linux" == GetOSPlatform().ToString().ToLower();
        }

        public static bool IsMacSystem()
        {
            return OSPlatform.OSX == GetOSPlatform();
        }

        [MenuItem("Assets/Qarth/Table/Build C#")]
        public static void BuildCSharpFile()
        {
            string path = ProjectPathConfig.projectToolsFolder;
            if (IsPureUnixSystem())
            {
                path += ProjectPathConfig.buildTxtDataUnixShell;
            }
            else if (IsMacSystem())
            {
                path += ProjectPathConfig.buildCSharpLinuxShell;
            }
            else if (IsLinuxSystem())
            {
                path += ProjectPathConfig.buildTxtDataUnixShell;
            }
            else
            {
                path += ProjectPathConfig.buildCSharpWinShell;
            }

            Thread newThread = new Thread(new ThreadStart(() =>
            {
                BuildCSharpThreadStart(path);
            }));
            newThread.Start();
        }

        [MenuItem("Assets/Qarth/Table/Build Data(txt)")]
        public static void BuildDataTxtMode()
        {
            string path = ProjectPathConfig.projectToolsFolder;
            if (IsLinuxSystem())
            {
                path += ProjectPathConfig.buildTxtDataLinuxShell;
            }
            else
            {
                path += ProjectPathConfig.buildTxtDataWinShell;
            }

            Thread newThread = new Thread(new ThreadStart(() =>
            {
                BuildCSharpThreadStart(path);
            }));
            newThread.Start();
        }

        [MenuItem("Assets/Qarth/Table/Build Data(lrg)")]
        public static void BuildDataLrgMode()
        {
            string path = ProjectPathConfig.projectToolsFolder;
            if (IsLinuxSystem())
            {
                path += ProjectPathConfig.buildLrgDataLinuxShell;
            }
            else
            {
                path += ProjectPathConfig.buildLrgDataWinShell;
            }

            Thread newThread = new Thread(new ThreadStart(() =>
            {
                BuildCSharpThreadStart(path);
            }));
            newThread.Start();
        }

        public static void BuildCSharpThreadStart(string path)
        {
            if (IsLinuxSystem())
            {
                CommandThreadStartLinux(path);
            }
            else
            {
                CommandThreadStartWin(path);
            }

        }

        private static void CommandThreadStartLinux(string path)
        {
            Process process = new Process();
            process.StartInfo.FileName = "/bin/sh";
            process.StartInfo.CreateNoWindow = false;
            process.StartInfo.ErrorDialog = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.Arguments = path + " arg1 arg2";

            process.Start();
            string outPutstr = process.StandardOutput.ReadToEnd();
            // if (!string.IsNullOrEmpty(outPutstr))
            // {
            //     Log.i(outPutstr);
            // }

            process.WaitForExit();
            process.Close();
        }

        private static void CommandThreadStartWin(string path)
        {
            Process process = new Process();
            process.StartInfo.FileName = path;
            process.StartInfo.CreateNoWindow = false;
            process.StartInfo.ErrorDialog = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;

            process.Start();
            string outPutstr = process.StandardOutput.ReadToEnd();
            // if (!string.IsNullOrEmpty(outPutstr))
            // {
            //     Log.i(outPutstr);
            // }

            process.WaitForExit();
            process.Close();
        }
    }
}
