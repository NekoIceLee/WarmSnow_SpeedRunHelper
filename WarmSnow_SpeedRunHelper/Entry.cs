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

        Rect MainWindowPosition { get; set; } = new Rect(10, 10, 300, 500);
        //{
        //    get
        //    {
        //        if (JsonUtility.FromJson<SerializableRect>(mainWndPositionJsonString.Value).X == 0)
        //        {
        //            return new Rect(10, 10, 300, 500);
        //        }
        //        return JsonUtility.FromJson<SerializableRect>(mainWndPositionJsonString.Value).Rect;
        //    }
        //    set
        //    {
        //        var serobj = new SerializableRect();
        //        serobj.Rect = value;
        //        mainWndPositionJsonString.Value = JsonUtility.ToJson(serobj);
        //    }
        //}

        GUIStyle GUITimeStyle { get; } = new GUIStyle
        {
            //font = Localization.Instance.CurrentLangAsset.textFont,
            fontSize = 32,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleCenter,
            wordWrap = true,
            normal = new GUIStyleState
            {
                textColor = Color.white,
            },
        };

        void Start()
        {
            preset1JsonString = Config.Bind(preset1Def, "{}");
            preset2JsonString = Config.Bind(preset2Def, "{}");
            preset3JsonString = Config.Bind(preset3Def, "{}");
            preset4JsonString = Config.Bind(preset4Def, "{}");
            //mainWndPositionJsonString = Config.Bind(MainWindowPositionDef, "");

            PresetControl.Preset1 = JsonUtility.FromJson<Preset>(preset1JsonString.Value);
            PresetControl.Preset2 = JsonUtility.FromJson<Preset>(preset2JsonString.Value);
            PresetControl.Preset3 = JsonUtility.FromJson<Preset>(preset3JsonString.Value);
            PresetControl.Preset4 = JsonUtility.FromJson<Preset>(preset4JsonString.Value);

            OnUpdate += TimeControl.Instance.Update;
            SceneManager.activeSceneChanged += MapLogger.Instance.SceneManager_activeSceneChanged;
        }

        private void PresetControl_Log(string message)
        {
            Logger.LogInfo(message);
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
            GUILayout.Label(TimeControl.Instance.StrTime, GUITimeStyle);
            GUILayout.Label($"帧率: {1 / Time.unscaledDeltaTime:0.0#}");

            {
                if (GUILayout.Button("GenPreset1"))
                {
                    PresetControl.Preset1 = Preset.CreatePreset();
                    PresetControl.SavePresets();
                }
                GUILayout.Label(PresetControl.Preset1.ToString(), GUILayout.MaxHeight(50));
                if (PresetControl.Preset1.IsNotNull && GUILayout.Button("ApplyPreset1"))
                {
                    PresetControl.Preset1.ApplyPreset();
                }
            }
            {
                if (GUILayout.Button("GenPreset2"))
                {
                    PresetControl.Preset2 = Preset.CreatePreset();
                    PresetControl.SavePresets();
                }
                GUILayout.Label(PresetControl.Preset2.ToString(), GUILayout.MaxHeight(50));
                if (PresetControl.Preset2.IsNotNull && GUILayout.Button("ApplyPreset2"))
                {
                    PresetControl.Preset2.ApplyPreset();
                }
            }
            {
                if (GUILayout.Button("GenPreset3"))
                {
                    PresetControl.Preset3 = Preset.CreatePreset();
                    PresetControl.SavePresets();
                }
                GUILayout.Label(PresetControl.Preset3.ToString(), GUILayout.MaxHeight(50));
                if (PresetControl.Preset3.IsNotNull && GUILayout.Button("ApplyPreset3"))
                {
                    PresetControl.Preset3.ApplyPreset();
                }
            }
            {
                if (GUILayout.Button("GenPreset4"))
                {
                    PresetControl.Preset4 = Preset.CreatePreset();
                    PresetControl.SavePresets();
                }
                GUILayout.Label(PresetControl.Preset4.ToString(), GUILayout.MaxHeight(50));
                if (PresetControl.Preset4.IsNotNull && GUILayout.Button("ApplyPreset4"))
                {
                    PresetControl.Preset4.ApplyPreset();
                }
            }

            GUI.DragWindow();
        }
    }

    [Serializable]
    public struct SerializableRect
    {
        public float X;
        public float Y;
        public float Height;
        public float Width;
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
