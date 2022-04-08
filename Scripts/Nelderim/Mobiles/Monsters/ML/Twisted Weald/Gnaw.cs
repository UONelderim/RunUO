using System;
using System.Collections;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName("zwłoki gnawa")]
	public class Gnaw : BaseCreature
	{
		public override double DifficultyScalar{ get{ return 1.15; } }
		[Constructable]
		public Gnaw() : base(AIType.AI_Melee, FightMode.Closest, 12, 1, 0.2, 0.4)
		{
			Name = "Gnaw";
			Body = 23;
			Hue = 0x130;
			BaseSoundID = 0xE5;

			SetStr(100, 120);
			SetDex(124, 145);
			SetInt(60, 86);

			SetHits(817, 857);
			SetStam(124, 145);
			SetMana(60, 86);

			SetDamage(18, 20);

			SetDamageType(ResistanceType.Physical, 100);

			SetResistance(ResistanceType.Physical, 60, 70);
			SetResistance(ResistanceType.Fire, 20, 30);
			SetResistance(ResistanceType.Cold, 20, 30);
			SetResistance(ResistanceType.Poison, 20, 30);
			SetResistance(ResistanceType.Energy, 30, 40);

			SetSkill(SkillName.MagicResist, 50.6, 60.8);
			SetSkill(SkillName.Tactics, 50.5, 60.7);
			SetSkill(SkillName.Wrestling, 60.4, 80.7);

			Fame = 12500;
			Karma = 12500;

			VirtualArmor = 44;
		}

		public static double SpeedBuff = 1.20;

		public override void GenerateLoot()
		{
			AddLoot(LootPack.FilthyRich, 4);
		}

		public void SpawnDireWolves(Mobile target)
		{
			Map map = this.Map;

			if (map == null)
				return;

			int newDireWolves = Utility.RandomMinMax(3, 5);

			for (int i = 0; i < newDireWolves; ++i)
			{
				DireWolf DireWolf = new DireWolf();

				DireWolf.Team = this.Team;
				DireWolf.FightMode = FightMode.Closest;

				bool validLocation = false;
				Point3D loc = this.Location;

				for (int j = 0; !validLocation && j < 10; ++j)
				{
					int x = X + Utility.Random(3) - 1;
					int y = Y + Utility.Random(3) - 1;
					int z = map.GetAverageZ(x, y);

					if (validLocation = map.CanFit(x, y, this.Z, 16, false, false))
						loc = new Point3D(x, y, Z);
					else if (validLocation = map.CanFit(x, y, z, 16, false, false))
						loc = new Point3D(x, y, z);
				}

				DireWolf.MoveToWorld(loc, map);
				DireWolf.Combatant = target;
			}
		}

		public override void AlterDamageScalarFrom(Mobile caster, ref double scalar)
		{
			if (0.1 >= Utility.RandomDouble())
				SpawnDireWolves(caster);
		}

		public override void OnGaveMeleeAttack(Mobile defender)
		{
			base.OnGaveMeleeAttack(defender);

			defender.Damage(Utility.Random(20, 10), this);
			defender.Stam -= Utility.Random(20, 10);
			defender.Mana -= Utility.Random(20, 10);
		}

		public override void OnGotMeleeAttack(Mobile attacker)
		{
			base.OnGotMeleeAttack(attacker);

			if (0.1 >= Utility.RandomDouble())
				SpawnDireWolves(attacker);
		}

		//public override bool GivesMinorArtifact { get { return true; } }
		public override int Hides { get { return 28; } }
		public override int Meat { get { return 4; } }

		public Gnaw(Serial serial) : base(serial)
		{
		}

		public override void OnDeath(Container c)
		{
			base.OnDeath(c);

			/*if (Utility.RandomDouble() < 0.3)
				c.DropItem(new GnawsFang());*/
				if ( Utility.RandomDouble() < 0.2 )
                PackItem(new TreasureMap(5, Map.Felucca));
				
		}
		public override OppositionGroup OppositionGroup
		{
			get { return OppositionGroup.FeyAndUndead; }
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write((int)1); // version
			writer.Write( (int)0 ); // DireWolf version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt(); 
			if ( version > 0 )
				reader.ReadInt(); // DireWolf version
		}
	}
}
