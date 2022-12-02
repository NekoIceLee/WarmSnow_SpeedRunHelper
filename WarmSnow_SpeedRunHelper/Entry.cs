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
        public static Preset Preset1
        {
            get
            {
                return JsonUtility.FromJson<Preset>(preset1JsonString.Value);
            }
            set
            {
                preset1JsonString.Value = JsonUtility.ToJson(value);
            }
        }
        public static Preset Preset2
        {
            get
            {
                return JsonUtility.FromJson<Preset>(preset2JsonString.Value);
            }
            set
            {
                preset2JsonString.Value = JsonUtility.ToJson(value);
            }
        }
        public static Preset Preset3
        {
            get
            {
                return JsonUtility.FromJson<Preset>(preset3JsonString.Value);
            }
            set
            {
                preset3JsonString.Value = JsonUtility.ToJson(value);
            }
        }
        public static Preset Preset4
        {
            get
            {
                return JsonUtility.FromJson<Preset>(preset4JsonString.Value);
            }
            set
            {
                preset4JsonString.Value = JsonUtility.ToJson(value);
            }
        }
        void Start()
        {
            preset1JsonString = Config.Bind(preset1Def, "");
            preset2JsonString = Config.Bind(preset2Def, "");
            preset3JsonString = Config.Bind(preset3Def, "");
            preset4JsonString = Config.Bind(preset4Def, "");
            OnUpdate += TimeControl.Instance.Update;
        }
        void Update()
        {


            OnUpdate();
        }
    }

    public class PresetControl
    {
        public static PresetControl Instance { get; set; } = new PresetControl();
        ConfigEntry<string> Preset1;
        PresetControl()
        {
            Preset1 = ConfigFile.CoreConfig.Bind("Preset", "Preset1", "", "Preset1");
        }
        
    }
    [Serializable]
    public class Preset
    {
        public int SectSkill { get; set; }
        public int FirstSkill { get; set; }
        public PN Potion1 { get; set; }
        public PN Potion2 { get; set; }
    }
}
