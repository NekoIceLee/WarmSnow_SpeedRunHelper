using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace WarmSnow_SpeedRunHelper
{
    public class MapLogger
    {
        public static MapLogger Instance { get; } = new MapLogger();
        public Scene CurrentScene { get; private set; }
        public string CurrentLogData
        {
            get
            {
                var ingametime = UI_TimeCount.instance.m_time;
                var modtime = TimeControl.Instance.StrTime;
                var scenename = CurrentScene.name;
                return $"{scenename},{modtime},{ingametime}";
            }
        }
        private StreamWriter fileWriter;
        MapLogger()
        {

        }

        public void SceneManager_activeSceneChanged(Scene arg0, Scene arg1)
        {
            CurrentScene = arg1;
            if (arg1.name.ToUpper().Contains("BASEMENT"))
            {
                //finish event
                FinishThisRun();
                return;
            }
            if (arg1.name.ToUpper().Contains("BASEMENT"))
            {
                //start game event
                if (fileWriter != null)
                {
                    fileWriter.Close();
                    fileWriter.Dispose();
                    fileWriter = null;
                }
                fileWriter = StartRun();
                return;
            }
            //common Switch Maps
        }
        private void FinishThisRun()
        {
            if (fileWriter == null) return;
            fileWriter.Flush();
            fileWriter.Close();
            fileWriter.Dispose();
            fileWriter = null;
        }
        private StreamWriter StartRun()
        {
            string filename = $"{DateTime.Now:M-dd-H-m-ss}_{PlayerAnimControl.instance.playerParameter.PLAYER_SECT}";
            
            return new StreamWriter($".\\{filename}.csv");
        }
    }
}
