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

        const string MainWindowTitle = "SpeedrunHelper";
        const string PresetEditorWindowTitle = "PresetEdit";

        Rect MainWindowPosition = new Rect(10, 10, 150, 500);
        Rect EditPresetWindowPosition = new Rect(0,0,200,400);
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
        GUIStyle GUIFrameRateTextStyle { get; } = new GUIStyle
        {
            //font = Localization.Instance.CurrentLangAsset.textFont,
            fontSize = 20,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleCenter,
            wordWrap = true,
            normal = new GUIStyleState
            {
                textColor = Color.white,
            },
        };

        bool FoldUtilities { get; set; } = false;
        bool _showPresetEditor = false;
        bool ShowPresetEditor 
        {
            get => _showPresetEditor;
            set
            {
                if (value)
                {
                    EditPresetWindowPosition.xMax = MainWindowPosition.xMin;
                    EditPresetWindowPosition.yMin = MainWindowPosition.yMin;
                }
                _showPresetEditor = value;
            } 
        }
        Preset currentEditingPreset;


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
            MapLogger.Instance.LogMessage += ExtClass_Log;

            Harmony.CreateAndPatchAll(typeof(PlayerRevivePatch), "com.NekoIce.SpeedRunHelper");
        }

        private void ExtClass_Log(string message)
        {
            Logger.LogInfo(message);
        }

        void OnGUI()
        {
            if (FoldUtilities)
            {
                MainWindowPosition.height = 120;
            }
            else
            {
                MainWindowPosition.height = 500;
            }
            MainWindowPosition = GUILayout.Window(MainWindowTitle.GetHashCode(), MainWindowPosition, GUIMainWindow, MainWindowTitle);
            if (ShowPresetEditor)
            {
                EditPresetWindowPosition = GUILayout.Window(PresetEditorWindowTitle.GetHashCode(), EditPresetWindowPosition, EditPresetGUIWindow, PresetEditorWindowTitle);
            }
        }

        void Update()
        {
            OnUpdate();
        }

        void GUIMainWindow(int id)
        {
            GUILayout.Label(TimeControl.Instance.StrTime, GUITimeStyle, GUILayout.MaxHeight(80));
            GUILayout.Label($"帧率: {1 / Time.unscaledDeltaTime:0.0#}", GUIFrameRateTextStyle);

            if (FoldUtilities == false)
            {
                int p1 = -1, p2 = -1, p3 = -1, p4 = -1;
                GUILayout.Label("", GUILayout.Height(15));
                GUILayout.Label(PresetControl.Preset1.ToString(), GUILayout.MaxHeight(50));
                p1 = GUILayout.SelectionGrid(p1, new string[] { "编辑", "应用" }, 2);
                if (p1 == 0)
                {
                    currentEditingPreset = PresetControl.Preset1;
                    ShowPresetEditor = true;
                    //PresetControl.Preset1 = Preset.CreatePreset();
                    //PresetControl.SavePresets();
                }
                else if (PresetControl.Preset1.IsNotNull && p1 == 1)
                {
                    PresetControl.Preset1.ApplyPreset();
                }
                GUILayout.Label(PresetControl.Preset2.ToString(), GUILayout.MaxHeight(50));
                p2 = GUILayout.SelectionGrid(p2, new string[] { "编辑", "应用" }, 2);
                if (p2 == 0)
                {
                    currentEditingPreset = PresetControl.Preset2;
                    ShowPresetEditor = true;
                    //PresetControl.Preset2 = Preset.CreatePreset();
                    //PresetControl.SavePresets();
                }
                else if (PresetControl.Preset2.IsNotNull && p2 == 1)
                {
                    PresetControl.Preset2.ApplyPreset();
                }
                GUILayout.Label(PresetControl.Preset3.ToString(), GUILayout.MaxHeight(50));
                p3 = GUILayout.SelectionGrid(p3, new string[] { "编辑", "应用" }, 2);
                if (p3 == 0)
                {
                    currentEditingPreset = PresetControl.Preset3;
                    ShowPresetEditor = true;
                    //PresetControl.Preset3 = Preset.CreatePreset();
                    //PresetControl.SavePresets();
                }
                if (PresetControl.Preset3.IsNotNull && p3 == 1)
                {
                    PresetControl.Preset3.ApplyPreset();
                }
                GUILayout.Label(PresetControl.Preset4.ToString(), GUILayout.MaxHeight(50));
                p4 = GUILayout.SelectionGrid(p4, new string[] { "编辑", "应用" }, 2);
                if (p4 == 0)
                {
                    currentEditingPreset = PresetControl.Preset4;
                    ShowPresetEditor = true;
                    //PresetControl.Preset4 = Preset.CreatePreset();
                    //PresetControl.SavePresets();
                }
                if (PresetControl.Preset4.IsNotNull && p4 == 1)
                {
                    PresetControl.Preset4.ApplyPreset();
                }

                GUILayout.BeginVertical(GUILayout.Height(10));
                GUILayout.EndVertical();

                if ( GUILayout.Button("折叠预设"))
                {
                    FoldUtilities = true;
                }
            }
            else
            {
                if (GUILayout.Button("展开预设"))
                {
                    FoldUtilities = false;
                }
            }

            GUI.DragWindow();
        }

        void EditPresetGUIWindow(int id)
        {



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
