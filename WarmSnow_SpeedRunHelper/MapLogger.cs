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
        Dictionary<string, string> sceneNameMapping = new Dictionary<string, string>();
        public delegate void LogMessageHandler(string message);
        public event LogMessageHandler LogMessage;

        public delegate void RunEndHandler(object message);
        public event LogMessageHandler RunEnd;
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
                if (sceneNameMapping.TryGetValue(name, out var mapping))
                {
                    return mapping;
                }

                return name;
            }
        }
        private string fileName;
        MapLogger()
        {
            sceneNameMapping = new Dictionary<string, string>
            {

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
            LogMessage(CurrentLogData);
        }
        public void FinishThisRun()
        {
            fileName = null;
        }
        private string StartRun()
        {
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
