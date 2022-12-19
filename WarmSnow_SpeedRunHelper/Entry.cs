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
        public static ConfigEntry<string> mainWndPositionJsonString;
        readonly ConfigDefinition preset1Def = new ConfigDefinition("Preset", "Preset1");
        readonly ConfigDefinition preset2Def = new ConfigDefinition("Preset", "Preset2");
        readonly ConfigDefinition preset3Def = new ConfigDefinition("Preset", "Preset3");
        readonly ConfigDefinition preset4Def = new ConfigDefinition("Preset", "Preset4");
        readonly ConfigDefinition MainWindowPositionDef = new ConfigDefinition("Main", "MainWindowPosition");

        Rect MainWindowPosition
        {
            get
            {
                if (JsonUtility.FromJson<SerializableRect>(mainWndPositionJsonString.Value).X == 0)
                {
                    return new Rect(10, 10, 300, 500);
                }
                return JsonUtility.FromJson<SerializableRect>(mainWndPositionJsonString.Value).Rect;
            }
            set
            {
                var serobj = new SerializableRect();
                serobj.Rect = value;
                mainWndPositionJsonString.Value = JsonUtility.ToJson(serobj);
            }
        }

        void Start()
        {
            preset1JsonString = Config.Bind(preset1Def, "{}");
            preset2JsonString = Config.Bind(preset2Def, "{}");
            preset3JsonString = Config.Bind(preset3Def, "{}");
            preset4JsonString = Config.Bind(preset4Def, "{}");
            mainWndPositionJsonString = Config.Bind(MainWindowPositionDef, "{}");

            OnUpdate += TimeControl.Instance.Update;
            SceneManager.activeSceneChanged += MapLogger.Instance.SceneManager_activeSceneChanged;
            
        }

        void OnGUI()
        {
            MainWindowPosition = GUILayout.Window("Main".GetHashCode(), MainWindowPosition, GUIMainWindow, "Main");
        }

        void Update()
        {


            OnUpdate();
        }

        void GUIMainWindow(int id)
        {
            GUILayout.Label(TimeControl.Instance.StrTime);
            GUILayout.Label($"ContinueTimeGo: {TimeControl.Instance.ContinueTimeGo}");

            if (GUILayout.Button("GenPreset1"))
            {
                PresetControl.Preset1 = PresetControl.CreatePreset();
            }
            GUILayout.Label(PresetControl.Preset1.ToString(), GUILayout.MaxHeight(400));
            if (GUILayout.Button("ApplyPreset1"))
            {
                PresetControl.ApplyPreset(PresetControl.Preset1);
            }
        }
    }

    [Serializable]
    public struct SerializableRect
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Height { get; set; }
        public float Width { get; set; }
        public Rect Rect 
        { 
            get
            {
                return new Rect(X, Y, Width, Height);
            }
            set
            {
                X = value.x;
                Y = value.y;
                Width = value.width;
                Height = value.height;
            }
        }
    }
}
