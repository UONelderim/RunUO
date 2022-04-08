using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName("pozostałości czerwonej śmierci")]
	public class RedDeath : BaseCreature
	{
		public override double DifficultyScalar{ get{ return 1.10; } }
        public override void AddWeaponAbilities()
        {
            WeaponAbilities.Add( WeaponAbility.WhirlwindAttack, 0.4 );
        }

		[Constructable]
		public RedDeath() : base(AIType.AI_Melee, FightMode.Closest, 12, 1, 0.2, 0.4)
		{
			Name = "Czerwona Śmierć";
			Body = Body = 793;
			BaseSoundID = 0xA8;
			Hue = 0x21;

			SetStr(317, 325);
			SetDex(241, 252);
			SetInt(241, 255);

			SetHits(1520, 1615);

			SetDamage(25, 29);

			SetDamageType(ResistanceType.Physical, 25);
			SetDamageType(ResistanceType.Fire, 75);

			SetResistance(ResistanceType.Physical, 60, 70);
			SetResistance(ResistanceType.Fire, 90);
			SetResistance(ResistanceType.Poison, 100);

			SetSkill(SkillName.Wrestling, 121.4, 143.7);
			SetSkill(SkillName.Tactics, 120.9, 142.2);
			SetSkill(SkillName.MagicResist, 120.1, 142.3);
			SetSkill(SkillName.Anatomy, 120.2, 144.0);

			Fame = 12500;
			Karma = 12500;

			for (int i = 0; i < 1; i++)
				if (Utility.RandomBool())
					PackNecroScroll(Utility.RandomMinMax(5, 9));
				else
					PackScroll(4, 7);
		}

		public static double SpeedBuff = 1.20;

		public override void GenerateLoot()
		{
			AddLoot(LootPack.FilthyRich, 2);
		}

		public override void OnDeath(Container c)
		{
			base.OnDeath(c);

			//c.DropItem(new ResolvesBridle());
		}

		//public override bool GivesMinorArtifact { get { return true; } }
		public override bool HasBreath { get { return true; } } // Change to chaso breath later
		public override Poison PoisonImmune { get { return Poison.Lethal; } }

		public RedDeath(Serial serial) : base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write((int)1); // version
			writer.Write( DateTime.MinValue );
			writer.Write( (Mobile)null );// BaseMount rider
			writer.Write( (Item)null );// BaseMount item
			writer.Write( (int)0 ); // SkeletalMount version
			writer.Write( (int)0 ); // BaseMount version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();
			if (version > 0 )
            {
				reader.ReadDateTime();// Basemount NextMountAbility
				reader.ReadMobile();// BaseMount rider
				reader.ReadItem();// BaseMount item
				reader.ReadInt(); // SkeletalMount version
				reader.ReadInt(); // BaseMount version
			}				
		}
	}
}
