using BehaviorDesigner.Runtime.Tasks;
using BepInEx.Configuration;
using Epic.OnlineServices.Mods;
using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace WarmSnow_SpeedRunHelper
{
    public class PresetControl : MonoBehaviour
    {
        public static PresetControl Instance { get; set; } = new PresetControl();
        public static Preset Preset1 { get; set; } = new Preset();
        public static Preset Preset2 { get; set; } = new Preset();
        public static Preset Preset3 { get; set; } = new Preset();
        public static Preset Preset4 { get; set; } = new Preset();

        public delegate void LogInfoHandler(string message);
        public event LogInfoHandler LogInfo;

        PresetControl()
        {
            
        }

        public static void SavePresets()
        {
            HelperMainModule.preset1JsonString.Value = JsonUtility.ToJson(Preset1);
            HelperMainModule.preset2JsonString.Value = JsonUtility.ToJson(Preset2);
            HelperMainModule.preset3JsonString.Value = JsonUtility.ToJson(Preset3);
            HelperMainModule.preset4JsonString.Value = JsonUtility.ToJson(Preset4);
        }

        static IEnumerable<Potion> OrderedPotions(IEnumerable<Potion> originList)
        {
            if (originList.Any(p => p.potionType == PotionType.Core))
            {
                yield return originList.First(p => p.potionType == PotionType.Core);
            }
            if (originList.Any(p => p.potionType == PotionType.Strength))
            {
                yield return originList.First(p => p.potionType == PotionType.Strength);
            }
            if (originList.Any(p => p.potionType == PotionType.Agility))
            {
                yield return originList.First(p => p.potionType == PotionType.Agility);
            }
            if (originList.Any(p => p.potionType == PotionType.Constitution))
            {
                yield return originList.First(p => p.potionType == PotionType.Constitution);
            }
            yield break;
        }

        public static Preset CreatePreset(Sect sect, int sectChose, Skill firstSkill, Potion potion1, Potion potion2)
        {
            return new Preset()
            {
                Sect = sect,
                SectChose = sectChose,
                FirstSkill = firstSkill.num,
                FirstSkillType = firstSkill.sect,
                Potion1 = potion1.PotionName,
                Potion2 = potion2.PotionName,
                Potion1Position = potion1.potionType,
                Potion2Position = potion2.potionType,
            };
        }

        public static List<Potion> curOrderedPotionList { get; set; }

        public static Preset CreatePreset()
        {
            PlayerAnimControl player = PlayerAnimControl.instance;
            var playerSect = player.playerParameter.PLAYER_SECT;
            int sectchose = -1;
            var skilllist = SkillControl.instance.curSkills;


            Potion potion1 = new Potion(), potion2 = new Potion();

            var potions = from potion in PotionsSystemControl.instance.curPotions
                          where potion.PotionName != PN.None
                          select potion;

            potions = OrderedPotions(potions);
            curOrderedPotionList = potions.ToList();
            potion1 = curOrderedPotionList.First();
            if (curOrderedPotionList.Count > 1)
            {
                potion2 = curOrderedPotionList[1];
            }
            Debug.Log("test");


            switch (playerSect)
            {
                case Sect.None:
                    break;
                case Sect.Berserk:
                    if (player.BERSERK_SKILL_Combos)
                    {
                        sectchose = 1;
                    }
                    else
                    {
                        sectchose = 2;
                    }
                    break;
                case Sect.SwordMaster:
                    if (player.SWORDMASTER_SKILL_UnlimitedSwords)
                    {
                        sectchose = 1;
                    }
                    else
                    {
                        sectchose = 2;
                    }
                    break;
                case Sect.DrunkMaster:
                    if (player.DRUNKMASTER_SKILL_AlcoholSword)
                    {
                        sectchose = 1;
                    }
                    else
                    {
                        sectchose = 2;
                    }
                    break;
                case Sect.ThunderGod:
                    if (player.THUNDERGOD_SKILL_ThunderDash)
                    {
                        sectchose = 1;
                    }
                    else
                    {
                        sectchose = 2;
                    }
                    break;
                case Sect.Venomancer:
                    if (player.POISONMASTER_SKILL_BloodSpray)
                    {
                        sectchose = 1;
                    }
                    else
                    {
                        sectchose = 2;
                    }
                    break;
                case Sect.FrozenMaster:
                    if (player.FROZENMASTER_SKILL_ChilledToBone)
                    {
                        sectchose = 1;
                    }
                    else
                    {
                        sectchose = 2;
                    }
                    break;
                case Sect.CommonSkill:
                    break;
                case Sect.Assassin:
                    if (player.ASSASSIN_SKILL_ShenXing)
                    {
                        sectchose = 1;
                    }
                    else
                    {
                        sectchose = 2;
                    }
                    break;
                case Sect.Nightmare:
                    break;
                default:
                    break;
            }

            if (skilllist.Count == 0)//Not choose Sect
            {
                return new Preset()
                {
                    Sect = Sect.None,
                    SectChose = -1,
                    FirstSkill = -1,
                    FirstSkillType = Sect.CommonSkill,
                    Potion1 = potion1.PotionName,
                    Potion2 = potion2.PotionName,
                    Potion1Position = potion1.potionType,
                    Potion2Position = potion2.potionType,
                };
            }
            else if (skilllist.Count == 2)//Choose Sect, Not Select First Book
            {

                return new Preset()
                {
                    Sect = playerSect,
                    SectChose = sectchose,
                    FirstSkill = -1,
                    FirstSkillType = Sect.CommonSkill,
                    Potion1 = potion1.PotionName,
                    Potion2 = potion2.PotionName,
                    Potion1Position = potion1.potionType,
                    Potion2Position = potion2.potionType,
                };
            }
            else
            {
                var firstbook = skilllist[2];
                return new Preset
                {
                    Sect = playerSect,
                    SectChose = sectchose,
                    FirstSkill = firstbook.num,
                    FirstSkillType = firstbook.sect,
                    Potion1 = potion1.PotionName,
                    Potion2 = potion2.PotionName,
                    Potion1Position = potion1.potionType,
                    Potion2Position = potion2.potionType,
                };
            }
        }
        public static bool ApplyPreset(Preset preset)
        {
            //Initialize Player Sects
            if (preset.SectChose >= 0)
            {
                Sect sect = preset.Sect;
                int sectChosen = preset.SectChose;
                //SectChooseUI ui = new SectChooseUI();
                //var SCUIType = ui.GetType();
                //SCUIType.GetField("sect").SetValue(ui, sect);
                //SCUIType.GetField("type").SetValue(ui, sectChosen);
                //ui.SectChoose();

                PlayerAnimControl.instance.BringBackToLife();

                PlayerAnimControl.instance.playerParameter.LEVEL++;
                SkillControl.instance.SkillOn(sect, 0);
                SkillControl.instance.SkillOn(sect, sectChosen);
                UI_CurrentSkillBar.instance.SkillPanelOn(sect, isLeft: false);
            }


            //Remove All Potions
            PotionDropPool.instance.MoveAway();

            //Remove All Generated Books
            var skillbookControls = GameObject.FindObjectsOfType<SkillDropControl>();
            foreach(var skillbookControl in skillbookControls)
            {
                (skillbookControl as SkillDropControl).gameObject.SetActive(false);
            }

            //Initialize Player First Book
            if (preset.FirstSkill >= 0)
            {
                //MenuSkillLearn skillLearn = MenuSkillLearn.instance;
                //var MSLType = skillLearn.GetType();
                //var sectSkillCardsArray = MSLType.GetField("skillCardSect").GetValue(skillLearn) as Sect[];
                //var SkillCardSkillsArray = MSLType.GetField("skillCardSkill").GetValue(skillLearn) as int[];

                //sectSkillCardsArray[0] = preset.FirstSkillType;
                //SkillCardSkillsArray[0] = preset.FirstSkill;

                //MSLType.GetField("skillCardSect").SetValue(skillLearn, sectSkillCardsArray);
                //MSLType.GetField("skillCardSkill").SetValue(skillLearn, SkillCardSkillsArray);

                //switch (preset.FirstSkillType)
                //{
                //    case Sect.CommonSkill:
                //        skillLearn.CommonRandomSkill.Remove(preset.FirstSkill);
                //        break;
                //    case Sect.Nightmare:
                //        skillLearn.NightmareRandomSkill.Remove(preset.FirstSkill);
                //        break;
                //}

                //skillLearn.Texts[0].text = TextControl.instance.SkillTitle(sectSkillCardsArray[0], SkillCardSkillsArray[0]);
                //skillLearn.Describes[0].text = TextControl.instance.SkillDescribe(sectSkillCardsArray[0], SkillCardSkillsArray[0]);

                //skillLearn.ClickSkillCard(0);

                //PlayerParameter playerParameter = PlayerAnimControl.instance.playerParameter;
                //MenuSkillLearn skillLearn = MenuSkillLearn.instance;
                //var level = playerParameter.LEVEL;
                //playerParameter.LEVEL ++;
                //SkillControl.instance.SkillOn(preset.FirstSkillType, preset.FirstSkill);
                //skillLearn.CurrentSkillLayouts[level].SetActive(true);
                //LayoutRebuilder.ForceRebuildLayoutImmediate(skillLearn.CurrentSkillLayouts[level].parent.GetComponent<RectTransform>());

                //var skill = preset.FirstSkill;
                //var index = 0;
                //int num = 0;

                //GameObject skillobj = null;
                //switch (preset.FirstSkillType)
                //{
                //    case Sect.None:
                //        break;
                //    case Sect.Berserk:
                //        {
                //            skillobj = UnityEngine.Object.Instantiate<GameObject>(skillLearn.BerserkSkills[skill], skillLearn.Panels[index]);
                //            skillobj.transform.localPosition = new Vector3(0f, 160f, 0f);
                //            skillobj.transform.localScale = new Vector3(2f, 2f, 1f);
                //        }

                //        break;
                //    case Sect.SwordMaster:
                //        {
                //            if (skill == 6)
                //            {
                //                if (PlayerAnimControl.instance.SWORDMASTER_SKILL_GUANRI)
                //                {
                //                    num = 11;
                //                }
                //                else
                //                {
                //                    num = 6;
                //                }
                //            }
                //            else if (skill >= 11)
                //            {
                //                num = skill + 1;
                //            }
                //            else
                //            {
                //                num = skill;
                //            }
                //            skillobj = UnityEngine.Object.Instantiate<GameObject>(skillLearn.swordMasterSkills[num], skillLearn.Panels[index]);
                //            skillobj.transform.localPosition = new Vector3(0f, 160f, 0f);
                //            skillobj.transform.localScale = new Vector3(2f, 2f, 1f);
                //        }
                //        break;
                //    case Sect.DrunkMaster:
                //        {
                //            if (skill >= 9)
                //            {
                //                if (skill == 9)
                //                {
                //                    if (PlayerAnimControl.instance.DRUNKMASTER_SKILL_WineOfDragon)
                //                    {
                //                        num = 12;
                //                    }
                //                    else
                //                    {
                //                        num = 9;
                //                    }
                //                }
                //                else if (skill == 10)
                //                {
                //                    if (PlayerAnimControl.instance.DRUNKMASTER_SKILL_WineOfSnake)
                //                    {
                //                        num = 10;
                //                    }
                //                    else
                //                    {
                //                        num = 11;
                //                    }
                //                }
                //                else if (skill >= 11)
                //                {
                //                    num = skill + 2;
                //                }
                //            }
                //            else
                //            {
                //                num = skill;
                //            }
                //            skillobj = UnityEngine.Object.Instantiate<GameObject>(skillLearn.drunkMasterSkills[num], skillLearn.Panels[index]);
                //            skillobj.transform.localPosition = new Vector3(0f, 160f, 0f);
                //            skillobj.transform.localScale = new Vector3(2f, 2f, 1f);
                //        }
                //        break;
                //    case Sect.ThunderGod:
                //        {
                //            skillobj = UnityEngine.Object.Instantiate<GameObject>(skillLearn.thunderGodSkills[skill], skillLearn.Panels[index]);
                //            skillobj.transform.localPosition = new Vector3(0f, 160f, 0f);
                //            skillobj.transform.localScale = new Vector3(2f, 2f, 1f);
                //        }
                //        break;
                //    case Sect.Venomancer:
                //        {
                //            if (skill == 7)
                //            {
                //                if (PlayerAnimControl.instance.POISONMASTER_SKILL_BloodSpray)
                //                {
                //                    num = 7;
                //                }
                //                else
                //                {
                //                    num = 11;
                //                }
                //            }
                //            else if (skill >= 11)
                //            {
                //                num = skill + 1;
                //            }
                //            else
                //            {
                //                num = skill;
                //            }
                //            skillobj = UnityEngine.Object.Instantiate<GameObject>(skillLearn.VenomancerSkills[num], skillLearn.Panels[index]);
                //            skillobj.transform.localPosition = new Vector3(0f, 160f, 0f);
                //            skillobj.transform.localScale = new Vector3(2f, 2f, 1f);
                //        }
                //        break;
                //    case Sect.FrozenMaster:
                //        {
                //            if (skill == 4)
                //            {
                //                if (PlayerAnimControl.instance.FROZENMASTER_SKILL_ChilledToBone)
                //                {
                //                    num = 4;
                //                }
                //                else
                //                {
                //                    num = 11;
                //                }
                //            }
                //            else if (skill >= 11)
                //            {
                //                num = skill + 1;
                //            }
                //            else
                //            {
                //                num = skill;
                //            }
                //            skillobj = UnityEngine.Object.Instantiate<GameObject>(skillLearn.FrozenMasterSkills[num], skillLearn.Panels[index]);
                //            skillobj.transform.localPosition = new Vector3(0f, 160f, 0f);
                //            skillobj.transform.localScale = new Vector3(2f, 2f, 1f);
                //        }
                //        break;
                //    case Sect.CommonSkill:
                //        {
                //            skillobj = UnityEngine.Object.Instantiate<GameObject>(skillLearn.CommonSkills[skill], skillLearn.Panels[index]);
                //            skillobj.transform.localPosition = new Vector3(0f, 160f, 0f);
                //            skillobj.transform.localScale = new Vector3(2f, 2f, 1f);
                //        }
                //        break;
                //    case Sect.Assassin:
                //        {
                //            if (skill == 3)
                //            {
                //                if (PlayerAnimControl.instance.ASSASSIN_SKILL_ShenXing)
                //                {
                //                    num = 3;
                //                }
                //                else
                //                {
                //                    num = 15;
                //                }
                //            }
                //            else if (skill == 10)
                //            {
                //                if (PlayerAnimControl.instance.ASSASSIN_SKILL_ShenXing)
                //                {
                //                    num = 10;
                //                }
                //                else
                //                {
                //                    num = 16;
                //                }
                //            }
                //            else if (skill == 11)
                //            {
                //                if (PlayerAnimControl.instance.ASSASSIN_SKILL_ShenXing)
                //                {
                //                    num = 11;
                //                }
                //                else
                //                {
                //                    num = 17;
                //                }
                //            }
                //            else
                //            {
                //                num = skill;
                //            }
                //            skillobj = UnityEngine.Object.Instantiate<GameObject>(skillLearn.AssassinSkills[num], skillLearn.Panels[index]);
                //            skillobj.transform.localPosition = new Vector3(0f, 160f, 0f);
                //            skillobj.transform.localScale = new Vector3(2f, 2f, 1f);
                //        }
                //        break;
                //    case Sect.Nightmare:
                //        {
                //            skillobj = UnityEngine.Object.Instantiate<GameObject>(skillLearn.NightmareSkills[skill], skillLearn.Panels[index]);
                //            skillobj.transform.localPosition = new Vector3(0f, 160f, 0f);
                //            skillobj.transform.localScale = new Vector3(2f, 2f, 1f);
                //        }
                //        break;
                //    default:
                //        break;
                //}
                //if (skillobj == null)
                //{
                //    Instance.LogInfo("skill obj is null");
                //}
                //else
                //{
                //    Instance.LogInfo(skillobj.ToString());
                //}
                //UI_CurSkillDescribe component = skillobj.GetComponent<UI_CurSkillDescribe>();
                //component.isOn = true;
                //MenuPotionExchange.instance.menuSkillDescribe.Add(component);
                //skillLearn.skillDescribe.Add(component);

                var skilllearn = MenuSkillLearn.instance;

                var commonskillbackup = new List<int>(skilllearn.CommonRandomSkill);
                var nightmareskillbackup = new List<int>(skilllearn.NightmareRandomSkill);
                var sectskillbackup = new List<int>(skilllearn.SectRandomSkill);

                switch(preset.FirstSkillType)
                {
                    case Sect.CommonSkill:
                        skilllearn.CommonRandomSkill = new List<int> { preset.FirstSkill, preset.FirstSkill, preset.FirstSkill };
                        skilllearn.NightmareRandomSkill = new List<int>();
                        skilllearn.SectRandomSkill = new List<int> { };
                        break;
                    case Sect.Nightmare:
                        skilllearn.isNightmareBook = true;
                        skilllearn.CommonRandomSkill = new List<int> {  };
                        skilllearn.NightmareRandomSkill = new List<int> { preset.FirstSkill, preset.FirstSkill, preset.FirstSkill };
                        skilllearn.SectRandomSkill = new List<int> { };
                        break;
                    default:
                        skilllearn.isGoldenBook = true;
                        skilllearn.CommonRandomSkill = new List<int> { };
                        skilllearn.NightmareRandomSkill = new List<int>();
                        skilllearn.SectRandomSkill = new List<int> { preset.FirstSkill, preset.FirstSkill, preset.FirstSkill };
                        break;
                }

                //skilllearn.isOn = true;

                skilllearn.On();

                skilllearn.CommonRandomSkill = commonskillbackup;
                skilllearn.NightmareRandomSkill = nightmareskillbackup;
                skilllearn.SectRandomSkill = sectskillbackup;

                switch (preset.FirstSkillType)
                {
                    case Sect.CommonSkill:
                        skilllearn.CommonRandomSkill.Remove(preset.FirstSkill);
                        break;
                    case Sect.Nightmare:
                        skilllearn.isNightmareBook = false;
                        skilllearn.NightmareRandomSkill.Remove(preset.FirstSkill);
                        break;
                    default:
                        skilllearn.isGoldenBook = false;
                        skilllearn.SectRandomSkill.Remove(preset.FirstSkill);
                        break;
                }
            }

            //Initialize Player Potion

            PotionsSystemControl potionControl = PotionsSystemControl.instance;
            Potion potion1 = new Potion
            {
                PotionName = preset.Potion1,
                potionType = preset.Potion1Position,
                elementType = ElementType.None,
                Level = 2,
                ManaCost = 0,
            };
            Potion potion2 = new Potion
            {
                PotionName = preset.Potion2,
                potionType = preset.Potion2Position,
                elementType = ElementType.None,
                Level = 2,
                ManaCost = 0,
            };


            switch (potion1.potionType)
            {
                case PotionType.Strength:
                    potionControl.curPotions[1] = potion1;
                    break;
                case PotionType.Agility:
                    potionControl.curPotions[2] = potion1;
                    break;
                case PotionType.Constitution:
                    potionControl.curPotions[3] = potion1;
                    break;
                case PotionType.Core:
                    potionControl.curPotions[0] = potion1;
                    break;
                case PotionType.None:
                    break;
            }

            switch (potion2.potionType)
            {
                case PotionType.Strength:
                    potionControl.curPotions[1] = potion2;
                    break;
                case PotionType.Agility:
                    potionControl.curPotions[2] = potion2;
                    break;
                case PotionType.Constitution:
                    potionControl.curPotions[3] = potion2;
                    break;
                case PotionType.Core:
                    potionControl.curPotions[0] = potion2;
                    break;
                case PotionType.None:
                    break;
            }

            potionControl.PotionsExchange();
            return true;
        }

    }

    [Serializable]
    public class Preset
    {
        public Sect Sect;
        public int SectChose;
        public int FirstSkill;
        public Sect FirstSkillType;
        public PN Potion1;
        public PotionType Potion1Position;
        public PN Potion2;
        public PotionType Potion2Position;
        public override string ToString()
        {
            return $"{Sect}:{SectChose},{TextControl.instance.SkillTitle(FirstSkillType, FirstSkill)},{Potion1}:{Potion1Position},{Potion2}:{Potion2Position}";
        }
    }
}
