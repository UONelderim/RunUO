using System;
using System.Collections;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName("zwloki Baronowej Frozen")]
	public class BaronowaFrozen : BaseCreature
	{
		[Constructable]
		public BaronowaFrozen() : base(AIType.AI_BattleMage, FightMode.Closest, 12, 1, 0.2, 0.4)
		{
			Name = "Baronowa Frozen";
			Label1 = "Skapana w krwi dziedziczka Lodowego Pazura";
			BaseSoundID = 0x4B0;

			Race = Jarling.Instance;

			Body = 401;
			FacialHairItemID = 0;
			Hue = 137;

			Kills = 1000;

			SetStr(150);
			SetDex(200);
			SetInt(500);

			SetHits(20000);
			SetMana(1000);

			SetDamage(18, 28);

			SetDamageType(ResistanceType.Physical, 75);
			SetDamageType(ResistanceType.Energy, 25);

			SetResistance(ResistanceType.Physical, 80, 90);
			SetResistance(ResistanceType.Fire, 70, 80);
			SetResistance(ResistanceType.Cold, 40, 50);
			SetResistance(ResistanceType.Poison, 50, 60);
			SetResistance(ResistanceType.Energy, 50, 60);

			SetSkill(SkillName.EvalInt, 90.1, 100.0);
			SetSkill(SkillName.Magery, 200.0);
			SetSkill(SkillName.Meditation, 100.0);
			SetSkill(SkillName.MagicResist, 100.5, 150.0);
			SetSkill(SkillName.Tactics, 80.1, 90.0);
			SetSkill(SkillName.Wrestling, 150.0);
			SetSkill(SkillName.SpiritSpeak, 150.0);

			Fame = 15000;
			Karma = -24000;

			VirtualArmor = 80;

			MagicDamageAbsorb = 50;
			MeleeDamageAbsorb = 30;

			PackReg(5);
			PackReg(5);
			PackReg(5);
		}

		public override bool OnBeforeDeath()
		{
			PackItem(new Gold(12000));
			if (Utility.RandomDouble() < 0.20)
				PackItem(new KsiegaSagaWyzwoleniaOgniaILodu1());

			return base.OnBeforeDeath();
		}

		public override void OnCarve(Mobile from, Corpse corpse, Item with)
		{
			if (!IsBonded && !corpse.Carved && !IsChampionSpawn)
			{
				if (Utility.RandomDouble() < 0.5)
					corpse.DropItem(new TrappedGhost());
				corpse.DropItem(new Brain());
			}

			base.OnCarve(from, corpse, with);
		}

		public override void GenerateLoot()
		{
			AddLoot(LootPack.FilthyRich, 2);
		}

		public override int Bones { get { return 2; } }
		public override int Meat { get { return 1; } }
		public override int TreasureMapLevel { get { return 5; } }

		public void DrainLife()
		{
			ArrayList list = new ArrayList();

			IPooledEnumerable eable = GetMobilesInRange(2);
			foreach (Mobile m in eable)
			{
				if (m == this || !CanBeHarmful(m))
					continue;

				if (m is BaseCreature && (((BaseCreature)m).Controlled || ((BaseCreature)m).Summoned || ((BaseCreature)m).Team != this.Team))
					list.Add(m);
				else if (m.Player)
					list.Add(m);
			}
			eable.Free();

			foreach (Mobile m in list)
			{
				DoHarmful(m);

				m.FixedParticles(0x374A, 10, 15, 5013, 0x496, 0, EffectLayer.Waist);
				m.PlaySound(0x231);

				m.SendMessage("You feel the life drain out of you!");

				int toDrain = Utility.RandomMinMax(10, 40);

				Hits += toDrain;
				m.Damage(toDrain, this);
			}
		}

		public override void OnGaveMeleeAttack(Mobile defender)
		{
			base.OnGaveMeleeAttack(defender);

			if (0.1 >= Utility.RandomDouble())
				DrainLife();
		}

		public override void OnGotMeleeAttack(Mobile attacker)
		{
			base.OnGotMeleeAttack(attacker);

			if (0.1 >= Utility.RandomDouble())
				DrainLife();
		}

		public BaronowaFrozen(Serial serial) : base(serial)
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