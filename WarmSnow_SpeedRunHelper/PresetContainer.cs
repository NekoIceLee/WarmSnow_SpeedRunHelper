using BepInEx.Configuration;
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

        IEnumerable<Potion> OrderedPotions(IEnumerable<Potion> originList)
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

        public Preset CreatePreset()
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
            potion1 = potions.First();
            potion2 = potions.Skip(1).First();
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
        public bool ApplyPreset(Preset preset)
        {
            //Initialize Player Sects
            if (preset.SectChose >= 0)
            {
                Sect sect = preset.Sect;
                int sectChosen = preset.SectChose;
                SectChooseUI ui = new SectChooseUI();
                var SCUIType = ui.GetType();
                SCUIType.GetField("sect").SetValue(ui, sect);
                SCUIType.GetField("type").SetValue(ui, sectChosen);
                ui.SectChoose();
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
                MenuSkillLearn skillLearn = MenuSkillLearn.instance;
                var MSLType = skillLearn.GetType();
                var sectSkillCardsArray = MSLType.GetField("skillCardSect").GetValue(skillLearn) as Sect[];
                var SkillCardSkillsArray = MSLType.GetField("skillCardSkill").GetValue(skillLearn) as int[];

                sectSkillCardsArray[0] = preset.FirstSkillType;
                SkillCardSkillsArray[0] = preset.FirstSkill;

                MSLType.GetField("skillCardSect").SetValue(skillLearn, sectSkillCardsArray);
                MSLType.GetField("skillCardSkill").SetValue(skillLearn, SkillCardSkillsArray);

                switch (preset.FirstSkillType)
                {
                    case Sect.CommonSkill:
                        skillLearn.CommonRandomSkill.Remove(preset.FirstSkill);
                        break;
                    case Sect.Nightmare:
                        skillLearn.NightmareRandomSkill.Remove(preset.FirstSkill);
                        break;
                }

                skillLearn.Texts[0].text = TextControl.instance.SkillTitle(sectSkillCardsArray[0], SkillCardSkillsArray[0]);
                skillLearn.Describes[0].text = TextControl.instance.SkillDescribe(sectSkillCardsArray[0], SkillCardSkillsArray[0]);

                skillLearn.ClickSkillCard(0);
            }

            //Initialize Player Potion
            if (preset.Potion1 > PN.None)
            {
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

                potionControl.PotionOn(potion1);
                potionControl.PotionOn(potion2);

            }


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
    }
}
