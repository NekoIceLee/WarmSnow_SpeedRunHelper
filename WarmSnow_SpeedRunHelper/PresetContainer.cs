using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace WarmSnow_SpeedRunHelper
{
    public class PresetControl
    {
        public static PresetControl Instance { get; set; } = new PresetControl();
        public static Preset Preset1
        {
            get
            {
                return JsonUtility.FromJson<Preset>(HelperMainModule.preset1JsonString.Value);
            }
            set
            {
                HelperMainModule.preset1JsonString.Value = JsonUtility.ToJson(value);
            }
        }
        public static Preset Preset2
        {
            get
            {
                return JsonUtility.FromJson<Preset>(HelperMainModule.preset2JsonString.Value);
            }
            set
            {
                HelperMainModule.preset2JsonString.Value = JsonUtility.ToJson(value);
            }
        }
        public static Preset Preset3
        {
            get
            {
                return JsonUtility.FromJson<Preset>(HelperMainModule.preset3JsonString.Value);
            }
            set
            {
                HelperMainModule.preset3JsonString.Value = JsonUtility.ToJson(value);
            }
        }
        public static Preset Preset4
        {
            get
            {
                return JsonUtility.FromJson<Preset>(HelperMainModule.preset4JsonString.Value);
            }
            set
            {
                HelperMainModule.preset4JsonString.Value = JsonUtility.ToJson(value);
            }
        }
        PresetControl()
        {
            
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
