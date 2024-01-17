using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("zwloki Athrad Math")]
    public class ZmiennyDar : BaseCreature
    {
        [Constructable]
        public ZmiennyDar() : base(AIType.AI_BattleMage, FightMode.Weakest, 12, 1, 0.2, 0.4)
        {
            Name = "Athrad Math";
            Body = 0x9e;
            Hue = 2978;
            BaseSoundID = 1084;

            SetStr( 402, 480 );
            SetDex( 118, 156 );
            SetInt( 212, 252 );

            SetHits( 348, 400 );

            SetDamage( 13, 21 );

            SetDamageType(ResistanceType.Poison, 80);
            SetDamageType(ResistanceType.Energy, 20);
            SetDamageType(ResistanceType.Physical, 0);

            SetResistance( ResistanceType.Physical, 65, 80 );
            SetResistance( ResistanceType.Fire, 45, 70 );
            SetResistance( ResistanceType.Cold, 50, 55 );
            SetResistance( ResistanceType.Poison, 55, 80 );
            SetResistance( ResistanceType.Energy, 55, 62 );


            SetSkill( SkillName.Meditation, 95.1, 110.0 );
            SetSkill( SkillName.EvalInt, 110.1, 120.0 );
            SetSkill( SkillName.MagicResist, 99.1, 100.0 );
            SetSkill( SkillName.Tactics, 90.1, 100.0 );
            SetSkill( SkillName.Wrestling, 120 );

            Fame = 15000;
            Karma = 15000;

            VirtualArmor = 50;

            Tamable = true;
            ControlSlots = 3;
            MinTameSkill = 99.9;

            var hueChangeTimer = new HueChangeTimer(this);
            hueChangeTimer.Start();
        }

        public override void GenerateLoot()
        {
            AddLoot( LootPack.FilthyRich, 1 );
            AddLoot( LootPack.Gems, 5 );
            PackReg( 5, 10 );
            PackReg( 5, 10 );
        }

        public override void AddWeaponAbilities()
        {
            WeaponAbilities.Add(WeaponAbility.ForceOfNature, 0.05);
        }

        private class HueChangeTimer : Timer
        {
            private readonly ZmiennyDar m_Creature;

            public HueChangeTimer(ZmiennyDar creature) : base(TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(30))
            {
                this.m_Creature = creature;
                Priority = TimerPriority.FiveSeconds;
            }

            protected override void OnTick()
            {
                if (m_Creature.Deleted)
                {
                    Stop();
                }

                int[] hueValues = { 2978, 2415, 2978, 2978, 2978, 2978 };
                int currentIndex = Array.IndexOf(hueValues, m_Creature.Hue);
                int nextIndex = (currentIndex + 1) % hueValues.Length;
                m_Creature.Hue = hueValues[nextIndex];
               // Console.WriteLine("Zmieniam kolorek"); FOR DEBUG PURPOSES
            }
        }


        public override int TreasureMapLevel => 3;

        public override int Meat => 10;

        public override int Hides => 10;

        public override HideType HideType => HideType.Barbed;

        public override FoodType FavoriteFood => FoodType.Meat;

        public override bool BardImmune => false;

        public override Poison PoisonImmune => Poison.Deadly;

        public override Poison HitPoison => Poison.Deadly;

        public ZmiennyDar(Serial serial) : base(serial)
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
    }
}