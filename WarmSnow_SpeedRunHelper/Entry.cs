using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BepInEx;
using BepInEx.Core;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;
using System.Runtime.CompilerServices;
using UnityEngine.SceneManagement;

namespace WarmSnow_SpeedRunHelper
{
    [BepInPlugin("com.NekoIce.SpeedRunHelper", "SpeedRunHelper", "0.0.1")]
    public class HelperMainModule : BaseUnityPlugin
    {
        public delegate void UpdateHandle();
        public event UpdateHandle OnUpdate;
        public static ConfigEntry<string> preset1JsonString;
        public static ConfigEntry<string> preset2JsonString;
        public static ConfigEntry<string> preset3JsonString;
        public static ConfigEntry<string> preset4JsonString;
        ConfigDefinition preset1Def = new ConfigDefinition("Preset", "Preset1");
        ConfigDefinition preset2Def = new ConfigDefinition("Preset", "Preset2");
        ConfigDefinition preset3Def = new ConfigDefinition("Preset", "Preset3");
        ConfigDefinition preset4Def = new ConfigDefinition("Preset", "Preset4");
        
        void Start()
        {
            preset1JsonString = Config.Bind(preset1Def, "");
            preset2JsonString = Config.Bind(preset2Def, "");
            preset3JsonString = Config.Bind(preset3Def, "");
            preset4JsonString = Config.Bind(preset4Def, "");
            OnUpdate += TimeControl.Instance.Update;
            SceneManager.activeSceneChanged += MapLogger.Instance.SceneManager_activeSceneChanged;
        }


        void Update()
        {


            OnUpdate();
        }
    }

    
}
