using Server;
using System;
using Server.ContextMenus;
using Server.Targeting;
using Server.Mobiles;
using Server.Network;
using System.Collections.Generic;
using Server.Items;

namespace Server.Items
{
    public class FeedTarget : Target
    {
        private gluttonousblade m_Item;

        public FeedTarget(gluttonousblade item)
            : base(1, false, TargetFlags.None)
        {
            if (item == null)
            {
                Console.WriteLine("Item is null!"); // You can replace this with your desired error handling logic
                return; // Exit the constructor without setting m_Item
            }

            m_Item = item;
        }


        protected override void OnTarget(Mobile from, object target)
        {
            if (target is Gold)
            {
                gluttonousblade.m_HungerT = TimeSpan.Zero;

                if (gluttonousblade.m_HungerT == TimeSpan.Zero)
                {
                    gluttonousblade.m_HungerT = TimeSpan.FromMinutes(((Gold)target).Amount / 100.0 ); // how many minutes to add depending on Gold dropped
                    from.SendMessage("bron konsumuje {0} centarow", ((Gold)target).Amount);
                    from.PlaySound(1073);
                    m_Item.StartTimer();
                    ((Item)target).Delete();
                }
            }
            else
            {
                from.PlaySound(1094);
                from.Say("Obrzydlistwo... To nie jest zloto!");
            }
        }
    }

    public class gluttonousblade : Kryss
    {
        private Timer m_Timer;
        public static TimeSpan m_HungerT;

        public override int InitMinHits { get { return 255; } }
        public override int InitMaxHits { get { return 255; } }

        public override int AosMinDamage { get { return 15; } }
        public override int AosMaxDamage { get { return 18; } }
        public override int AosSpeed { get { return 60; } }

        [Constructable]
        public gluttonousblade()
            : base()
        {
            Name = "Krys Przekletego Glodu";
            Hue = 1287;
            LootType = LootType.Cursed;

            m_HungerT = TimeSpan.FromMinutes(10); // set starting minutes until it resets back to basic weapon
            StartTimer();
        }

        public gluttonousblade(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public TimeSpan TimeLeft
        {
            get { return m_HungerT; }
            set
            {
                m_HungerT = value;
                InvalidateProperties();
            }
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (m_Timer != null)
            {
                TimeSpan t = m_HungerT;
                int hours = t.Hours;
                int minutes = t.Minutes;

                list.Add(1153090, hours.ToString());
                list.Add(1153089, minutes.ToString());

                // Use DelayCall to invalidate properties
                Timer.DelayCall(TimeSpan.FromSeconds(1.0), new TimerCallback(InvalidatePropertiesCallback));
            }
            else
            {
                list.Add(1114057, "[Jestem głodny!] ");
            }
        }

        private void InvalidatePropertiesCallback()
        {
            InvalidateProperties();
        }

        public override void OnHit(Mobile attacker, Mobile defender, double damageBonus)
        {
            if (defender is BaseCreature)
            {
                if (0.03 > Utility.RandomDouble()) // 3% of the time this weapon will set a defender's hit points to 0 almost killing it
                {
                    defender.Hits -= 50;
                    defender.PlaySound(1067); // play groan sound
                    attacker.SendMessage("Klątwa ostrza zaczyna się uaktywniać.");
                    attacker.Hits -= 15;
                    attacker.Mana -= 15;
                    attacker.Stam -= 15;
                    attacker.Say("DAJ MI ZŁOTA!!!");
                }
            }
            base.OnHit(attacker, defender, damageBonus);
        }

        public virtual void StartTimer()
        {
            if (m_Timer != null)
                return;

            //Attributes to add once "fed"
            Name = "Krys Przekletego Glodu";
            Attributes.AttackChance = 10;
            Attributes.WeaponDamage = 45;
            Attributes.DefendChance = -10;
            Attributes.SpellDamage = 3;
            WeaponAttributes.HitFireball = 30;
            WeaponAttributes.HitLightning = 10;
            Attributes.BonusStr = 3;
            Attributes.BonusDex = 3;
            Attributes.BonusInt = 3;
            Hue = 2675;

            // Use DelayCall to implement the timer
            Timer.DelayCall(TimeSpan.FromSeconds(1.0), TimeSpan.FromSeconds(1.0), new TimerCallback(SliceCallback));
        }

        private void SliceCallback()
        {
            Slice();
        }

        public virtual void Slice()
        {
            InvalidateProperties();

            if (m_HungerT <= TimeSpan.Zero)
            {
                Delete();
                return;
            }
            m_HungerT -= TimeSpan.FromSeconds(1);
        }

        public virtual void Delete()
        {
            Effects.SendLocationParticles(EffectItem.Create(Location, Map, EffectItem.DefaultDuration), 0x3728, 8, 20, 5042);
            Effects.PlaySound(Location, Map, 0x201);

            StopTimer();
            m_HungerT = TimeSpan.Zero;

            // Remove the weapon attributes
            Name = "Nienazarty kryss";
            Attributes.AttackChance = 0;
            Attributes.WeaponDamage = 0;
            Attributes.DefendChance = 0;
            Attributes.SpellDamage = 0;
            WeaponAttributes.HitFireball = 0;
            WeaponAttributes.HitLightning = 0;
            Attributes.BonusStr = 0;
            Attributes.BonusDex = 0;
            Attributes.BonusInt = 0;
            Hue = 1287;
        }

        public virtual void StopTimer()
        {
            if (m_Timer != null)
                m_Timer.Stop();

            m_Timer = null;
        }

        public override void OnDoubleClick(Mobile from) // Override double click of the deed to call our target
        {
            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042001);
            }
            else
            {
                from.SendMessage("Nakarm mnie!!!");
                from.Target = new FeedTarget(this); // Call our target
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(1);
            writer.Write(TimeLeft);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            TimeLeft = reader.ReadTimeSpan();
            StartTimer();
        }
    }
}