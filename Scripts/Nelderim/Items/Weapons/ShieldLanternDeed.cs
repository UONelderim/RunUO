
using System;
using Server.Network;
using Server.Prompts;
using Server.Items;
using Server.Mobiles;
using Server.Targeting;
using Server.ACC.CSS;

namespace Server.Items
{
    public class ShieldLanternTarget : Target
    {
        private ShieldLanternDeed m_Deed;
        private BaseShield m_Shield;

        public ShieldLanternTarget(ShieldLanternDeed deed) : base(1, false, TargetFlags.None)
        {
            m_Deed = deed;
        }

        protected override void OnTarget(Mobile from, object target)
        {
            if (target is BaseShield && !(target is ShieldLantern))
            {
                m_Shield = (BaseShield)target;

                if (!m_Shield.IsChildOf(from.Backpack))
                {
                    from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it. 
                    return;
                }

                if (m_Shield.Attributes.SpellChanneling == 0)
                {
                    from.SendMessage("Ta tarcza nie przepuszcza zaklec.");
                    return;
                }

                ShieldLantern created = new ShieldLantern();
                created.Hue = m_Shield.Hue;

                CopyShieldAttributes(created);

                from.AddToBackpack(created);

                m_Shield.Delete();
                m_Deed.Delete();

                from.SendMessage("Zamieniles wyglad tarczy.");
            }
            else
            {
                from.SendMessage("To nie jest tarcza.");
            }
        }

        private void CopyShieldAttributes(ShieldLantern target)
        {
            target.Attributes.RegenHits += m_Shield.Attributes.RegenHits;
            target.Attributes.RegenStam += m_Shield.Attributes.RegenStam;
            target.Attributes.RegenMana += m_Shield.Attributes.RegenMana;
            target.Attributes.DefendChance += m_Shield.Attributes.DefendChance;
            target.Attributes.AttackChance += m_Shield.Attributes.AttackChance;
            target.Attributes.BonusStr += m_Shield.Attributes.BonusStr;
            target.Attributes.BonusDex += m_Shield.Attributes.BonusDex;
            target.Attributes.BonusInt += m_Shield.Attributes.BonusInt;
            target.Attributes.BonusHits += m_Shield.Attributes.BonusHits;
            target.Attributes.BonusStam += m_Shield.Attributes.BonusStam;
            target.Attributes.BonusMana += m_Shield.Attributes.BonusMana;
            target.Attributes.WeaponDamage += m_Shield.Attributes.WeaponDamage;
            target.Attributes.WeaponSpeed += m_Shield.Attributes.WeaponSpeed;
            target.Attributes.SpellDamage += m_Shield.Attributes.SpellDamage;
            target.Attributes.CastRecovery += m_Shield.Attributes.CastRecovery;
            target.Attributes.CastSpeed += m_Shield.Attributes.CastSpeed;
            target.Attributes.LowerManaCost += m_Shield.Attributes.LowerManaCost;
            target.Attributes.LowerRegCost += m_Shield.Attributes.LowerRegCost;
            target.Attributes.ReflectPhysical += m_Shield.Attributes.ReflectPhysical;
            target.Attributes.EnhancePotions += m_Shield.Attributes.EnhancePotions;
            target.Attributes.Luck += m_Shield.Attributes.Luck;
            target.Attributes.NightSight = Math.Max(m_Shield.Attributes.NightSight, target.Attributes.NightSight);

            target.Attributes.SpellChanneling = Math.Max(m_Shield.Attributes.SpellChanneling, target.Attributes.SpellChanneling);

            target.ArmorAttributes.SelfRepair += m_Shield.ArmorAttributes.SelfRepair;
            target.ArmorAttributes.DurabilityBonus += m_Shield.ArmorAttributes.DurabilityBonus;
            target.ArmorAttributes.LowerStatReq += m_Shield.ArmorAttributes.LowerStatReq;

            target.StrRequirement = m_Shield.StrRequirement;
            target.DexRequirement = m_Shield.DexRequirement;
            target.IntRequirement = m_Shield.IntRequirement;
            target.MaxHitPoints = m_Shield.MaxHitPoints;
            target.HitPoints = m_Shield.HitPoints;

            // Base resistances are zero on Shield Landern.
            // Any resist of the source shield (be it base or bonus) must be copied directly as bonus to the Shield Lantern.
            target.PhysicalBonus = m_Shield.PhysicalResistance;
            target.FireBonus = m_Shield.FireResistance;
            target.ColdBonus = m_Shield.ColdResistance;
            target.PoisonBonus = m_Shield.PoisonResistance;
            target.EnergyBonus = m_Shield.EnergyResistance;
        }
    }

    public class ShieldLanternDeed : Item
    {
        public static string NameText { get { return "zwoj na latarnie maga"; } } // uzyte rowniez w menu rzemieslniczym

        [Constructable]
        public ShieldLanternDeed() : base(0x14F0)
        {
            Weight = 1.0;
            Name = NameText;
            LootType = LootType.Blessed;
            Hue = 592;
        }

        public ShieldLanternDeed(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version 
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            LootType = LootType.Blessed;

            int version = reader.ReadInt();
        }

        public override bool DisplayLootType { get { return false; } }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it. 
            }
            else
            {
                from.SendMessage("Ktora tarcze chcesz przemienic?");
                from.Target = new ShieldLanternTarget(this);
            }
        }
    }
}
