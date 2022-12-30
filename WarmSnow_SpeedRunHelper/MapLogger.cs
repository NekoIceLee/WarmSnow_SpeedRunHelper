using BepInEx;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace WarmSnow_SpeedRunHelper
{
    public class MapLogger
    {
        public static MapLogger Instance { get; } = new MapLogger();
        public Scene CurrentScene { get; private set; }
        Dictionary<int, string> sceneNameMapping = new Dictionary<int, string>();
        public delegate void LogMessageHandler(string message);
        public event LogMessageHandler LogMessage;
        
        public string CurrentLogData
        {
            get
            {
                var ingametime = UI_TimeCount.instance.m_time;
                var modtime = TimeControl.Instance.StrTime;
                var globalmapid = StageControl.instance.MapID;
                return $"{globalmapid},{CurrentSceneRename},{modtime},{ingametime}";
            }
        }
        public string CurrentSceneRename
        {
            get
            {
                var path = CurrentScene.path;
                var name = CurrentScene.name;
                var globalmapid = StageControl.instance.MapID;
                if (sceneNameMapping.TryGetValue(globalmapid, out var mapping))
                {
                    return mapping;
                }

                return name;
            }
        }
        private string fileName;
        MapLogger()
        {
            sceneNameMapping = new Dictionary<int, string>
            {
                { 314, "龙帝" },
            };
        }

        public void SceneManager_activeSceneChanged(Scene arg0, Scene arg1)
        {
            CurrentScene = arg1;
            //common Switch Maps
            if (string.IsNullOrEmpty(fileName)) { fileName= StartRun(); }
            StreamWriter sw = new StreamWriter(fileName);
            sw.WriteLine(CurrentLogData);
            sw.Flush();
            sw.Close();
            switch (StageControl.instance.MapID)
            {
                //case StageType.Wild:
                //    TimeControl.Instance.stage1EndTime = TimeControl.Instance.TimeSeconds;
                //    break;
                //case StageType.PigBoss:
                //    TimeControl.Instance.stage2EndTime = TimeControl.Instance.TimeSeconds;
                //    break;
                //case StageType.Sister:
                //    TimeControl.Instance.stage3EndTime = TimeControl.Instance.TimeSeconds;
                //    break;
                //case StageType.Gundam:
                //    TimeControl.Instance.stage4EndTime = TimeControl.Instance.TimeSeconds;
                //    break;
                //case StageType.Tiger:
                //    TimeControl.Instance.stage5EndTime = TimeControl.Instance.TimeSeconds;
                //    break;
                case 314:
                    TimeControl.Instance.stage6EndTime = TimeControl.Instance.TimeSeconds;
                    break;
                default:
                    break;
            }
            LogMessage(CurrentLogData);
        }
        public void FinishThisRun()
        {
            fileName = null;
        }
        private string StartRun()
        {
            if (PlayerAnimControl.instance.playerParameter.PLAYER_SECT == Sect.None) { return null; }
            string filename = $"{DateTime.Now:M-dd-H-m-ss}_{PlayerAnimControl.instance.playerParameter.PLAYER_SECT}";
            filename = $"D:\\{filename}.csv";
            LogMessage(filename);
            return filename;
        }
    }

    [HarmonyLib.HarmonyPatch(typeof(PlayerAnimControl), nameof(PlayerAnimControl.instance.BringBackToLife))]
    public class PlayerRevivePatch
    {
        [HarmonyLib.HarmonyPrefix]
        public static void Prefix()
        {
            TimeControl.Instance.OnRunEnd(null);
            MapLogger.Instance.FinishThisRun();
        }
    }
}
