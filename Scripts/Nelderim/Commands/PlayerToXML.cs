using System;
using System.Collections.Generic;
using System.Text;
using Server.Items;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Commands
{
    public class PlayerToXml
    {
        public static void Initialize()
        {
            // CommandSystem.Register("playerToXml", AccessLevel.Administrator, new CommandEventHandler(PlayerToXml_OnCommand));
        }

        public static void PlayerToXml_OnCommand(CommandEventArgs e)
        {
            e.Mobile.SendMessage("Wskaz postac");
            e.Mobile.Target = new PlayerToXmlTarget();
        }
        
        public class PlayerToXmlTarget : Target
        {
            private static List<Layer> layers = new List<Layer> {Layer.OneHanded,
                Layer.TwoHanded,
                Layer.Shoes,
                Layer.Pants,
                Layer.Shirt,
                Layer.Helm,
                Layer.Gloves,
                Layer.Ring,
                Layer.Talisman,
                Layer.Neck,
                Layer.Waist,
                Layer.InnerTorso,
                Layer.Bracelet,
                Layer.MiddleTorso,
                Layer.Earrings,
                Layer.Arms,
                Layer.Cloak,
                Layer.OuterTorso,
                Layer.OuterLegs,
                Layer.InnerLegs 
            };
            
            public PlayerToXmlTarget( ) : base ( -1, false, TargetFlags.None )
            { 
            }
            
            protected override void OnTarget( Mobile from, object targeted )
            {
                if ( targeted is PlayerMobile )
                {
                    PlayerMobile pm = targeted as PlayerMobile;
                    StringBuilder r = new StringBuilder();
                    add(r,"RawName", pm.RawName);
                    add(r,"Race",pm.Race.Name);
                    add(r,"Hue", pm.Hue);
                    add(r,"HairItemID", pm.HairItemID);
                    add(r,"HairHue", pm.HairHue);
                    add(r,"FacialHairItemID", pm.FacialHairItemID);
                    add(r,"FacialHairHue", pm.FacialHairHue);
                    add(r,"Fame", pm.Fame);
                    add(r,"Karma", pm.Karma);
                    add(r,"Female", pm.Female);
                    add(r,"BodyValue", pm.BodyValue);
                    add(r, "SkillsCap", pm.SkillsCap);
                    add(r, "StatCap", pm.StatCap);
                    foreach (SkillName skillName in Enum.GetValues(typeof(SkillName)))
                    {
                        Skill skill = pm.Skills[skillName];
                        if(skill.Cap > 100) add(r, "Skills." + skillName + ".Cap", skill.Cap);
                        if(skill.Base > 0) add(r, "Skills." + skillName + ".Base", skill.Base);
                    }
                    add(r, "RawStr", pm.RawStr);
                    add(r, "RawDex", pm.RawDex);
                    add(r, "RawInt", pm.RawInt);
                    addIf(r, "SandMining", pm.SandMining, false);
                    addIf(r, "Spellweaving", pm.Spellweaving, false);
                    addIf(r, "StoneMining", pm.StoneMining, false);
                    addIf(r, "Label1", pm.Label1, null);
                    addIf(r, "Label2", pm.Label2, null);
                    addIf(r, "Label3", pm.Label3, null);
                    addIf(r, "Kills", pm.Kills, 0);
                    add(r, "Profile", pm.Profile);
                    add(r, "SpeechHue", pm.SpeechHue);
                    addIf(r, "TithingPoints", pm.TithingPoints, 0);
                    addIf(r, "Title", pm.Title, null);
                    
                    foreach (Layer layer in layers)
                    {
                        Item item = pm.FindItemOnLayer(layer);
                        if (item != null) writeItem(r, item);
                    }
                    
                    Console.WriteLine(r);
                }
                else
                    from.SendLocalizedMessage( 505852 ); // "Cel nie jest graczem."
            }

            private void writeItem(StringBuilder r, Item item)
            {
                add(r, "EQUIP", "<" + item.GetType().Name);
                if (item is BaseArmor)
                {
                    addArmor(r, item as BaseArmor);
                }
                if (item is BaseWeapon)
                {
                    addWeapon(r, item as BaseWeapon);
                }
                if (item is BaseJewel)
                {
                    addJewel(r, item as BaseJewel);
                }
                if (item is Spellbook)
                {
                    addSpellBook(r, item as Spellbook);
                }
                add(r, "Hue", item.Hue);
                add(r, "ItemID", item.ItemID);
                addIf(r, "Label1", item.Label1, null);
                addIf(r, "Label2", item.Label2, null);
                addIf(r, "Label3", item.Label3, null);
                addIf(r, "LabelOfCreator", item.LabelOfCreator, null);
                add(r, "LootType", item.LootType);
                add(r, "Movable", item.Movable);
                addIf(r, "Name", item.Name, null);
                add(r, "Visible", item.Visible);
                r.Append(">");
            }

            private void addArmor(StringBuilder r, BaseArmor ba)
            {
                foreach (AosArmorAttribute attribute in Enum.GetValues(typeof(AosArmorAttribute)))
                {
                    addIf(r, "ArmorAttributes." + attribute, ba.ArmorAttributes[attribute], 0);
                }

                foreach (AosAttribute attribute in Enum.GetValues(typeof(AosAttribute)))
                {
                    addIf(r, "Attributes." + attribute, ba.Attributes[attribute], 0);
                }

                addIf(r, "ColdBonus", ba.ColdBonus, 0);
                addIf(r, "DexBonus", ba.DexBonus, 0);
                addIf(r, "EnergyBonus", ba.EnergyBonus, 0);
                addIf(r, "FireBonus", ba.FireBonus, 0);
                add(r, "HitPoints", ba.HitPoints);
                addIf(r, "IntBonus", ba.IntBonus, 0);
                add(r, "MaxHitPoints", ba.MaxHitPoints);
                addIf(r, "PhysicalBonus", ba.PhysicalBonus, 0);
                addIf(r, "PoisonBonus", ba.PoisonBonus, 0);
                add(r, "Quality", ba.Quality);
                add(r, "Resource", ba.Resource);
                add(r, "Resource2", ba.Resource2);
                if (ba.SkillBonuses.Skill_1_Value != 0)
                {
                    add(r, "SkillBonuses.Skill_1_Value", ba.SkillBonuses.Skill_1_Value);
                    add(r, "SkillBonuses.Skill_1_Name", ba.SkillBonuses.Skill_1_Name);
                }

                if (ba.SkillBonuses.Skill_2_Value != 0)
                {
                    add(r, "SkillBonuses.Skill_2_Value", ba.SkillBonuses.Skill_2_Value);
                    add(r, "SkillBonuses.Skill_2_Name", ba.SkillBonuses.Skill_2_Name);
                }

                if (ba.SkillBonuses.Skill_3_Value != 0)
                {
                    add(r, "SkillBonuses.Skill_3_Value", ba.SkillBonuses.Skill_3_Value);
                    add(r, "SkillBonuses.Skill_3_Name", ba.SkillBonuses.Skill_3_Name);
                }

                if (ba.SkillBonuses.Skill_4_Value != 0)
                {
                    add(r, "SkillBonuses.Skill_4_Value", ba.SkillBonuses.Skill_4_Value);
                    add(r, "SkillBonuses.Skill_4_Name", ba.SkillBonuses.Skill_4_Name);
                }

                if (ba.SkillBonuses.Skill_5_Value != 0)
                {
                    add(r, "SkillBonuses.Skill_5_Value", ba.SkillBonuses.Skill_5_Value);
                    add(r, "SkillBonuses.Skill_5_Name", ba.SkillBonuses.Skill_5_Name);
                }

                addIf(r, "StrBonus", ba.StrBonus, 0);
            }

            private void addWeapon(StringBuilder r, BaseWeapon bw)
            {
                foreach (AosElementAttribute attribute in Enum.GetValues(typeof(AosElementAttribute)))
                {
                    addIf(r, "AosElementDamages." + attribute, bw.AosElementDamages[attribute], 0);
                }
                foreach (AosAttribute attribute in Enum.GetValues(typeof(AosAttribute)))
                {
                    addIf(r, "Attributes." + attribute, bw.Attributes[attribute], 0);
                }
                add(r, "Consecrated", bw.Consecrated);
                add(r, "Cursed", bw.Cursed);
                add(r, "HitPoints", bw.HitPoints);
                add(r, "MaxHitPoints", bw.MaxHitPoints);
                addIf(r, "Poison", bw.Poison, null);
                addIf(r, "PoisonCharges", bw.PoisonCharges, 0);
                add(r, "Quality", bw.Quality);
                add(r, "Resource", bw.Resource);
                add(r, "Resource2", bw.Resource2);
                if (bw.SkillBonuses.Skill_1_Value != 0)
                {
                    add(r, "SkillBonuses.Skill_1_Value", bw.SkillBonuses.Skill_1_Value);
                    add(r, "SkillBonuses.Skill_1_Name", bw.SkillBonuses.Skill_1_Name);
                }

                if (bw.SkillBonuses.Skill_2_Value != 0)
                {
                    add(r, "SkillBonuses.Skill_2_Value", bw.SkillBonuses.Skill_2_Value);
                    add(r, "SkillBonuses.Skill_2_Name", bw.SkillBonuses.Skill_2_Name);
                }

                if (bw.SkillBonuses.Skill_3_Value != 0)
                {
                    add(r, "SkillBonuses.Skill_3_Value", bw.SkillBonuses.Skill_3_Value);
                    add(r, "SkillBonuses.Skill_3_Name", bw.SkillBonuses.Skill_3_Name);
                }

                if (bw.SkillBonuses.Skill_4_Value != 0)
                {
                    add(r, "SkillBonuses.Skill_4_Value", bw.SkillBonuses.Skill_4_Value);
                    add(r, "SkillBonuses.Skill_4_Name", bw.SkillBonuses.Skill_4_Name);
                }

                if (bw.SkillBonuses.Skill_5_Value != 0)
                {
                    add(r, "SkillBonuses.Skill_5_Value", bw.SkillBonuses.Skill_5_Value);
                    add(r, "SkillBonuses.Skill_5_Name", bw.SkillBonuses.Skill_5_Name);
                }
                add(r, "Slayer", bw.Slayer);
                add(r, "Slayer2", bw.Slayer2);
                foreach (AosWeaponAttribute attribute in Enum.GetValues(typeof(AosWeaponAttribute)))
                {
                    addIf(r, "WeaponAttributes." + attribute, bw.WeaponAttributes[attribute], 0);
                }
            }

            private void addJewel(StringBuilder r, BaseJewel bj)
            {
                foreach (AosAttribute attribute in Enum.GetValues(typeof(AosAttribute)))
                {
                    addIf(r, "Attributes." + attribute, bj.Attributes[attribute], 0);
                }
                add(r, "HitPoints", bj.HitPoints);
                add(r, "MaxHitPoints", bj.MaxHitPoints);
                foreach (AosElementAttribute attribute in Enum.GetValues(typeof(AosElementAttribute)))
                {
                    addIf(r, "Resistances." + attribute, bj.Resistances[attribute], 0);
                }
                add(r, "Resource", bj.Resource);
                if (bj.SkillBonuses.Skill_1_Value != 0)
                {
                    add(r, "SkillBonuses.Skill_1_Value", bj.SkillBonuses.Skill_1_Value);
                    add(r, "SkillBonuses.Skill_1_Name", bj.SkillBonuses.Skill_1_Name);
                }

                if (bj.SkillBonuses.Skill_2_Value != 0)
                {
                    add(r, "SkillBonuses.Skill_2_Value", bj.SkillBonuses.Skill_2_Value);
                    add(r, "SkillBonuses.Skill_2_Name", bj.SkillBonuses.Skill_2_Name);
                }

                if (bj.SkillBonuses.Skill_3_Value != 0)
                {
                    add(r, "SkillBonuses.Skill_3_Value", bj.SkillBonuses.Skill_3_Value);
                    add(r, "SkillBonuses.Skill_3_Name", bj.SkillBonuses.Skill_3_Name);
                }

                if (bj.SkillBonuses.Skill_4_Value != 0)
                {
                    add(r, "SkillBonuses.Skill_4_Value", bj.SkillBonuses.Skill_4_Value);
                    add(r, "SkillBonuses.Skill_4_Name", bj.SkillBonuses.Skill_4_Name);
                }

                if (bj.SkillBonuses.Skill_5_Value != 0)
                {
                    add(r, "SkillBonuses.Skill_5_Value", bj.SkillBonuses.Skill_5_Value);
                    add(r, "SkillBonuses.Skill_5_Name", bj.SkillBonuses.Skill_5_Name);
                }
                if (bj.SkillBonuses.Skill_1_Value != 0)
                {
                    add(r, "SkillBonuses.Skill_1_Value", bj.SkillBonuses.Skill_1_Value);
                    add(r, "SkillBonuses.Skill_1_Name", bj.SkillBonuses.Skill_1_Name);
                }

                if (bj.SkillBonuses.Skill_2_Value != 0)
                {
                    add(r, "SkillBonuses.Skill_2_Value", bj.SkillBonuses.Skill_2_Value);
                    add(r, "SkillBonuses.Skill_2_Name", bj.SkillBonuses.Skill_2_Name);
                }

                if (bj.SkillBonuses.Skill_3_Value != 0)
                {
                    add(r, "SkillBonuses.Skill_3_Value", bj.SkillBonuses.Skill_3_Value);
                    add(r, "SkillBonuses.Skill_3_Name", bj.SkillBonuses.Skill_3_Name);
                }

                if (bj.SkillBonuses.Skill_4_Value != 0)
                {
                    add(r, "SkillBonuses.Skill_4_Value", bj.SkillBonuses.Skill_4_Value);
                    add(r, "SkillBonuses.Skill_4_Name", bj.SkillBonuses.Skill_4_Name);
                }

                if (bj.SkillBonuses.Skill_5_Value != 0)
                {
                    add(r, "SkillBonuses.Skill_5_Value", bj.SkillBonuses.Skill_5_Value);
                    add(r, "SkillBonuses.Skill_5_Name", bj.SkillBonuses.Skill_5_Name);
                }
            }

            private void addSpellBook(StringBuilder r, Spellbook sb)
            {
                foreach (AosAttribute attribute in Enum.GetValues(typeof(AosAttribute)))
                {
                    addIf(r, "Attributes." + attribute, sb.Attributes[attribute], 0);
                }
                add(r, "Content", sb.Content);
                add(r, "HitPoints", sb.HitPoints);
                add(r, "MaxHitPoints", sb.MaxHitPoints);
                if (sb.SkillBonuses.Skill_1_Value != 0)
                {
                    add(r, "SkillBonuses.Skill_1_Value", sb.SkillBonuses.Skill_1_Value);
                    add(r, "SkillBonuses.Skill_1_Name", sb.SkillBonuses.Skill_1_Name);
                }

                if (sb.SkillBonuses.Skill_2_Value != 0)
                {
                    add(r, "SkillBonuses.Skill_2_Value", sb.SkillBonuses.Skill_2_Value);
                    add(r, "SkillBonuses.Skill_2_Name", sb.SkillBonuses.Skill_2_Name);
                }

                if (sb.SkillBonuses.Skill_3_Value != 0)
                {
                    add(r, "SkillBonuses.Skill_3_Value", sb.SkillBonuses.Skill_3_Value);
                    add(r, "SkillBonuses.Skill_3_Name", sb.SkillBonuses.Skill_3_Name);
                }

                if (sb.SkillBonuses.Skill_4_Value != 0)
                {
                    add(r, "SkillBonuses.Skill_4_Value", sb.SkillBonuses.Skill_4_Value);
                    add(r, "SkillBonuses.Skill_4_Name", sb.SkillBonuses.Skill_4_Name);
                }

                if (sb.SkillBonuses.Skill_5_Value != 0)
                {
                    add(r, "SkillBonuses.Skill_5_Value", sb.SkillBonuses.Skill_5_Value);
                    add(r, "SkillBonuses.Skill_5_Name", sb.SkillBonuses.Skill_5_Name);
                }
                add(r, "Slayer", sb.Slayer);
                add(r, "Slayer2", sb.Slayer2);
            }
            
            private void addIf(StringBuilder sb, string props, object value, object defaultValue)
            {
                if(value != null && !value.Equals(defaultValue)) sb.Append("/" + props + "/" + value);
            }

            private void add(StringBuilder sb, string props, object value)
            {
                sb.Append("/" + props + "/" + value);
            }
        }
    }
}