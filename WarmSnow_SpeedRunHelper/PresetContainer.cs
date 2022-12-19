using BepInEx.Configuration;
using Epic.OnlineServices.Mods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace WarmSnow_SpeedRunHelper
{
    public class PresetControl
    {
        public static PresetControl Instance { get; set; } = new PresetControl();
        public static Preset Preset1 { get; set; }
        public static Preset Preset2 { get; set; }
        public static Preset Preset3 { get; set; }
        public static Preset Preset4 { get; set; }

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
            Instance.LogInfo("GetCurrentSkill On Line 77");
            PlayerAnimControl player = PlayerAnimControl.instance;
            Instance.LogInfo("GetCurrentSkill On Line 78");
            var playerSect = player.playerParameter.PLAYER_SECT;
            Instance.LogInfo("GetCurrentSkill On Line 80");
            int sectchose = -1;
            Instance.LogInfo("GetCurrentSkill On Line 82");
            var skilllist = SkillControl.instance.curSkills;

            Instance.LogInfo("GetCurrentSkill On Line 112");

            Potion potion1 = new Potion(), potion2 = new Potion();

            Instance.LogInfo("GetCurrentSkill On Line 116");
            var potions = from potion in PotionsSystemControl.instance.curPotions
                          where potion.PotionName != PN.None
                          select potion;

            Instance.LogInfo("GetCurrentSkill On Line 120");
            potions = OrderedPotions(potions);
            Instance.LogInfo("GetCurrentSkill On Line 123");
            curOrderedPotionList = potions.ToList();
            potion1 = potions.First();
            Instance.LogInfo("GetCurrentSkill On Line 125");
            potion2 = potions.Skip(1).First();


            Instance.LogInfo("GetCurrentPotion On Line 123");

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
            Instance.LogInfo("GetCurrentSect On Line 206");

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
            Instance.LogInfo("ApplyPreset On Line 232");
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


            Instance.LogInfo("ApplyPreset On Line 246");
            //Remove All Potions
            PotionDropPool.instance.MoveAway();

            Instance.LogInfo("ApplyPreset On Line 250");
            //Remove All Generated Books
            var skillbookControls = GameObject.FindObjectsOfType<SkillDropControl>();
            foreach(var skillbookControl in skillbookControls)
            {
                (skillbookControl as SkillDropControl).gameObject.SetActive(false);
            }

            Instance.LogInfo("ApplyPreset On Line 258");
            //Initialize Player First Book
            //if (preset.FirstSkill >= 0)
            //{
            //    MenuSkillLearn skillLearn = MenuSkillLearn.instance;
            //    var MSLType = skillLearn.GetType();
            //    var sectSkillCardsArray = MSLType.GetField("skillCardSect").GetValue(skillLearn) as Sect[];
            //    var SkillCardSkillsArray = MSLType.GetField("skillCardSkill").GetValue(skillLearn) as int[];

            //    sectSkillCardsArray[0] = preset.FirstSkillType;
            //    SkillCardSkillsArray[0] = preset.FirstSkill;

            //    MSLType.GetField("skillCardSect").SetValue(skillLearn, sectSkillCardsArray);
            //    MSLType.GetField("skillCardSkill").SetValue(skillLearn, SkillCardSkillsArray);

            //    switch (preset.FirstSkillType)
            //    {
            //        case Sect.CommonSkill:
            //            skillLearn.CommonRandomSkill.Remove(preset.FirstSkill);
            //            break;
            //        case Sect.Nightmare:
            //            skillLearn.NightmareRandomSkill.Remove(preset.FirstSkill);
            //            break;
            //    }

            //    skillLearn.Texts[0].text = TextControl.instance.SkillTitle(sectSkillCardsArray[0], SkillCardSkillsArray[0]);
            //    skillLearn.Describes[0].text = TextControl.instance.SkillDescribe(sectSkillCardsArray[0], SkillCardSkillsArray[0]);

            //    skillLearn.ClickSkillCard(0);
            //}

            Instance.LogInfo("ApplyPreset On Line 289");
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
                //potionControl.PotionOn(potion1);
                //potionControl.PotionOn(potion2);
                
            


            return true;
        }
    }

    [Serializable]
    public class Preset
    {
        public Sect Sect { get; set; }
        public int SectChose { get; set; }
        public int FirstSkill { get; set; }
        public Sect FirstSkillType { get; set; }
        public PN Potion1 { get; set; }
        public PotionType Potion1Position { get; set; }
        public PN Potion2 { get; set; }
        public PotionType Potion2Position { get; set; }
        public override string ToString()
        {
            return $"{Sect}:{SectChose},{TextControl.instance.SkillTitle(FirstSkillType, FirstSkill)},{Potion1}:{Potion1Position},{Potion2}:{Potion2Position}";
        }
    }
}
