using System;
using Server;
using Server.Mobiles;
using Server.Network;
using Server.ACC.CSS.Systems;
using Server.Spells;

namespace Server.Items
{
    public class ObronaZywiolow : ChaosShield
    {
        public override int InitMinHits { get { return 60; } }
        public override int InitMaxHits { get { return 60; } }

        public static void Initialize()
        {
            PlayerEvent.HitByWeapon += new PlayerEvent.OnWeaponHit(InternalCallback);
        }

        [Constructable]
        public ObronaZywiolow()
        {
            Hue = 2613;
            Name = "Obrona Zywiolow - Zimno";
            Attributes.DefendChance = 15;
            Attributes.EnhancePotions = 15;
            ArmorAttributes.LowerReq = 70;
            Label1 = "okruchy magicznego lodu pokrywaja tarcze";
        }

        public ObronaZywiolow(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }

        public static void InternalCallback(Mobile attacker, Mobile defender, int damage, WeaponAbility a)
        {
            if (damage > 20 && attacker != null && defender != null)
            {
                if (attacker.Weapon is BaseMeleeWeapon)
                {
                    int coldDamage = 10;
                    attacker.Damage(coldDamage);
                    defender.FixedParticles(0x3709, 10, 30, 5052, 0x480, 0, EffectLayer.LeftFoot);
                    attacker.SendMessage("Lodowa tarcza zmraza krew w Twych zylach");
                    
                }
            }
        }
    }
}