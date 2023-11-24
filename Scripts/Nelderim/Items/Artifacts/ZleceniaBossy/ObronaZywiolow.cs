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

        private DateTime m_ShieldEquipTime; // Track the time when the shield is equipped
        private Timer m_RemoveEffectTimer; // Timer to remove the effect

        public static void Initialize()
        {
            PlayerEvent.HitByWeapon += new PlayerEvent.OnWeaponHit(InternalCallback);
        }

        [Constructable]
        public ObronaZywiolow()
        {
            Hue = 2613;
            Name = "Obrona Zywiolow - Zimno";
            Attributes.DefendChance = 25;
            Attributes.EnhancePotions = 15;
            Label1 = "okruchy magicznego lodu pokrywaja tarcze";
        }

        public ObronaZywiolow(Serial serial) : base(serial)
        {
        }

        public override bool OnEquip(Mobile from)
        {
            bool baseResult = base.OnEquip(from);

            m_ShieldEquipTime = DateTime.UtcNow;
            StartRemoveEffectTimer();

            return baseResult;
        }

        public override void OnRemoved(object parent)
        {
            base.OnRemoved(parent);

            StopRemoveEffectTimer();

            Mobile mobile = parent as Mobile;
            if (mobile != null)
            {
                RemoveDamagingEffect(mobile);
            }
        }

        private void StartRemoveEffectTimer()
        {
            if (m_RemoveEffectTimer != null)
                StopRemoveEffectTimer();

            TimeSpan duration = TimeSpan.FromSeconds(10); // Adjust the duration as needed

            m_RemoveEffectTimer = Timer.DelayCall(duration, RemoveEffectCallback);
        }

        private void StopRemoveEffectTimer()
        {
            if (m_RemoveEffectTimer != null)
            {
                m_RemoveEffectTimer.Stop();
                m_RemoveEffectTimer = null;
            }
        }

        private void RemoveEffectCallback()
        {
            Mobile mobile = RootParent as Mobile;
            if (mobile != null)
            {
                RemoveDamagingEffect(mobile);
            }
        }

        private void RemoveDamagingEffect(Mobile mobile)
        {
            int coldDamage = 10;
            mobile.Damage(coldDamage);
            mobile.FixedParticles(0x3709, 10, 30, 5052, 0x480, 0, EffectLayer.LeftFoot);
            mobile.SendMessage("The cold shield effect wears off.");
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
            writer.Write(m_ShieldEquipTime);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            m_ShieldEquipTime = reader.ReadDateTime();
            StartRemoveEffectTimer();
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
