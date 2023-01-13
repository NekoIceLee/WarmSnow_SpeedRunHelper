using BehaviorDesigner.Runtime.Tasks;
using BepInEx.Configuration;
using Epic.OnlineServices.Mods;
using HarmonyLib;
using NightmareMode;
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
    public class PresetControl
    {
        public static Preset Preset1 { get; set; } = new Preset();
        public static Preset Preset2 { get; set; } = new Preset();
        public static Preset Preset3 { get; set; } = new Preset();
        public static Preset Preset4 { get; set; } = new Preset();

        public static void SavePresets()
        {
            HelperMainModule.preset1JsonString.Value = JsonUtility.ToJson(Preset1);
            HelperMainModule.preset2JsonString.Value = JsonUtility.ToJson(Preset2);
            HelperMainModule.preset3JsonString.Value = JsonUtility.ToJson(Preset3);
            HelperMainModule.preset4JsonString.Value = JsonUtility.ToJson(Preset4);
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
            return $"{GetSectTitle(Sect, SectChose)}," +
                $"{TextControl.instance.SkillTitle(FirstSkillType, FirstSkill)}," +
                $"{GetPotionTitle(Potion1)}," +
                $"{GetPotionTitle(Potion2)}";
        }
        public bool IsNotNull
        {
            get
            {
                return Sect != Sect.None || FirstSkill >= 0 || Potion1 > PN.None || Potion2 > PN.None;
            }
        }
        public bool ApplyPreset()
        {
            return ApplyPreset(this);
        }
        public void GenToThisPreset()
        {
            var gen = CreatePreset();
            Sect = gen.Sect;
            SectChose = gen.SectChose;
            FirstSkill = gen.FirstSkill;
            FirstSkillType = gen.FirstSkillType;
            Potion1Position = gen.Potion1Position;
            Potion2Position = gen.Potion2Position;
            Potion1 = gen.Potion1;
            Potion2 = gen.Potion2;
        }
        public static string GetPotionPositionName(PotionType type)
        {
            return type switch
            {
                PotionType.Core => "核心",
                PotionType.Strength => "力量",
                PotionType.Agility => "敏捷",
                PotionType.Constitution => "功效",
                _ => "未知",
            };
        }
        public static string GetPotionTitle(PN potion)
        {
            int potionName = (int)potion;
            string handle = "PN_NAME_" + potionName;
            string text = Localization.Instance.GetLocalizedText(handle);
            return text;
        }
        public static string GetSectTitle(Sect sect, int sectChose)
        {
            return sect switch
            {
                Sect.Berserk => sectChose switch
                {
                    1 => "1-连击",
                    2 => "2-无影",
                    _ => "未选择",
                },
                Sect.SwordMaster => sectChose switch
                {
                    1 => "1-无量",
                    2 => "2-贯日",
                    _ => "未选择",
                },
                Sect.DrunkMaster => sectChose switch
                {
                    1 => "1-酒染",
                    2 => "2-醉歌",
                    _ => "未选择",
                },
                Sect.ThunderGod => sectChose switch
                {
                    1 => "1-迅影",
                    2 => "2-蛮雷",
                    _ => "未选择",
                },
                Sect.Venomancer => sectChose switch
                {
                    1 => "1-血溅",
                    2 => "2-缠蛊",
                    _ => "未选择",
                },
                Sect.FrozenMaster => sectChose switch
                {
                    1 => "1-刺骨",
                    2 => "2-白霜",
                    _ => "未选择",
                },
                Sect.Assassin => sectChose switch
                {
                    1 => "1-神行",
                    2 => "2-迷踪",
                    _ => "未选择",
                },
                Sect.None => "未选择",
                Sect.CommonSkill => "",
                Sect.Nightmare => "",
                _ => "",
            };
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
            if (potions.Any())
            {
                potion1 = potions.First();
                if (potions.Count() > 1)
                {
                    potion2 = potions.Skip(1).First();
                }

            }
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
        public static IEnumerator IEApplyPreset(Preset preset)
        {
            PlayerAnimControl.instance.BringBackToLife();
            yield return new WaitForFixedUpdate();
            yield return new WaitForFixedUpdate();
            yield return new WaitForFixedUpdate();
            MementoControl.instance.MementoOn();
            yield return new WaitForEndOfFrame();

            //Initialize Player Sects
            if (preset.SectChose >= 0)
            {
                Sect sect = preset.Sect;
                int sectChosen = preset.SectChose;

                PlayerAnimControl.instance.playerParameter.LEVEL++;
                SkillControl.instance.SkillOn(sect, 0);
                SkillControl.instance.SkillOn(sect, sectChosen);
                UI_CurrentSkillBar.instance.SkillPanelOn(sect, isLeft: false);
                UI_SectChoose.instance.hasChoose = true;

                var buddhas = GameObject.FindObjectsOfType<BuddhaStatueControl>();
                var currentBuddha = buddhas.FirstOrDefault(bud => bud.sect == sect);

                //foreach(var buddha in buddhas)
                //{
                //    if (buddha == currentBuddha)
                //    {
                //        PresetControl.Instance.StartCoroutine(new Traverse(currentBuddha).Method("BuddhaStatueLight").GetValue<IEnumerator>());
                //    }
                //}
                if (currentBuddha != null)
                {
                    if ((bool)currentBuddha.spiritOfBlade)
                    {
                        currentBuddha.spiritOfBlade.SetActive(value: true);
                    }
                    currentBuddha.BuddhaStatue[2].SetActive(value: true);
                    currentBuddha.door.AnimationState.SetAnimation(0, "open", false);
                    CameraControl.instance.CameraShake(0.2f, 0.2f, 0.02f, 0.2f);
                    AudioController.Instance.SoundPlay("Amb_3D_BeginStoneDoorOpen", currentBuddha.doorCols[0].transform.parent.gameObject);
                    //yield return new WaitForSeconds(1f);
                    CameraControl.instance.CameraShake(9.75f, 0.05f, 0.05f, 0.1f, CameraShakeDir.EightDirections, 0.99f);
                    //yield return new WaitForSeconds(1f);
                    currentBuddha.doorCols[0].enabled = true;
                    currentBuddha.doorCols[1].enabled = true;
                    //yield return new WaitForSeconds(9.95f);
                    CameraControl.instance.CameraShake(0.2f, 0.2f, 0.02f, 0.2f);
                }
                
            }

            //Initialize Player First Book
            if (preset.FirstSkill >= 0)
            {
                SkillControl.FastSkillOn(preset.Sect, preset.FirstSkill);
                //Remove All Generated Books
                var skillbookControls = GameObject.FindObjectsOfType<SkillDropControl>();
                skillbookControls.First().gameObject.SetActive(false);
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

            if (potion1.potionType != PotionType.None && potion2.potionType != PotionType.None)
            {
                //Remove All Potions
                PotionDropPool.instance.MoveAway();
                PlayerAnimControl.instance.MementoRefine_FirstSuperRarePotion_On = false;
            }
            else if (potion1.potionType != PotionType.None || potion2.potionType != PotionType.None)
            {
                //Remove All Potions
                PotionDropPool.instance.MoveAway();
            }
            else
            {

            }

            yield break;
        }
        public static bool ApplyPreset(Preset preset)
        {
            MenuSkillLearn.instance.StartCoroutine(IEApplyPreset(preset));
            return true;
        }

        static IEnumerator IEOpenTheDoor(BuddhaStatueControl currentBuddha)
        {
            currentBuddha.BuddhaStatue[2].SetActive(value: true);
            currentBuddha.door.AnimationState.SetAnimation(0, "open", false);
            CameraControl.instance.CameraShake(0.2f, 0.2f, 0.02f, 0.2f);
            AudioController.Instance.SoundPlay("Amb_3D_BeginStoneDoorOpen", currentBuddha.doorCols[0].transform.parent.gameObject);
            yield return new WaitForSeconds(1f);
            CameraControl.instance.CameraShake(9.75f, 0.05f, 0.05f, 0.1f, CameraShakeDir.EightDirections, 0.99f);
            yield return new WaitForSeconds(1f);
            currentBuddha.doorCols[0].enabled = true;
            currentBuddha.doorCols[1].enabled = true;
            yield return new WaitForSeconds(9.95f);
            CameraControl.instance.CameraShake(0.2f, 0.2f, 0.02f, 0.2f);
        }
    }
}
