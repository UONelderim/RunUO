using Server.Items;

namespace Server.Mobiles
{
	[CorpseName("zwloki straznika")]
	public class StandardNelderimGuard : BaseNelderimGuard
	{
		[Constructable]
		public StandardNelderimGuard() : base(GuardType.StandardGuard)
		{
			PackGold(20, 80);
		}

		public StandardNelderimGuard(Serial serial) : base(serial)
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

	[CorpseName("zwloki straznika")]
	public class MageNelderimGuard : BaseNelderimGuard
	{
		public override void AddWeaponAbilities()
		{
			WeaponAbilities.Add(WeaponAbility.ParalyzingBlow, 0.225);
			WeaponAbilities.Add(WeaponAbility.Disarm, 0.225);
		}

		[Constructable]
		public MageNelderimGuard() : base(GuardType.MageGuard, AIType.AI_Mage)
		{
			PackGold(40, 80);
		}

		public MageNelderimGuard(Serial serial) : base(serial)
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

	[CorpseName("zwloki straznika")]
	public class HeavyNelderimGuard : BaseNelderimGuard
	{
		public override void AddWeaponAbilities()
		{
			WeaponAbilities.Add(WeaponAbility.WhirlwindAttack, 0.225);
			WeaponAbilities.Add(WeaponAbility.BleedAttack, 0.225);
		}

		[Constructable]
		public HeavyNelderimGuard() : base(GuardType.HeavyGuard)
		{
			PackGold(40, 80);
		}

		public HeavyNelderimGuard(Serial serial) : base(serial)
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

	[CorpseName("zwloki straznika")]
	public class MountedNelderimGuard : BaseNelderimGuard
	{
		public override void AddWeaponAbilities()
		{
			WeaponAbilities.Add(WeaponAbility.Disarm, 0.225);
			WeaponAbilities.Add(WeaponAbility.BleedAttack, 0.225);
		}

		[Constructable]
		public MountedNelderimGuard() : base(GuardType.MountedGuard)
		{
			PackGold(40, 80);
		}

		public MountedNelderimGuard(Serial serial) : base(serial)
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

	[CorpseName("zwloki straznika")]
	public class ArcherNelderimGuard : BaseNelderimGuard
	{
		public override void AddWeaponAbilities()
		{
			WeaponAbilities.Add(WeaponAbility.ParalyzingBlow, 0.2);
			WeaponAbilities.Add(WeaponAbility.ArmorIgnore, 0.2);
		}

		[Constructable]
		public ArcherNelderimGuard() : base(GuardType.ArcherGuard, AIType.AI_Archer,  rangeFight: 6)
		{
			PackGold(30, 90);
		}

		public ArcherNelderimGuard(Serial serial) : base(serial)
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

	[CorpseName("zwloki straznika")]
	public class EliteNelderimGuard : BaseNelderimGuard
	{
		public override void AddWeaponAbilities()
		{
			WeaponAbilities.Add(WeaponAbility.WhirlwindAttack, 0.25);
			WeaponAbilities.Add(WeaponAbility.BleedAttack, 0.25);
		}

		[Constructable]
		public EliteNelderimGuard() : base(GuardType.EliteGuard, rangePerception: 18)
		{
			PackGold(50, 100);
		}

		public EliteNelderimGuard(Serial serial) : base(serial)
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

	[CorpseName("zwloki straznika")]
	public class SpecialNelderimGuard : BaseNelderimGuard
	{
		public override void AddWeaponAbilities()
		{
			WeaponAbilities.Add(WeaponAbility.Disarm, 0.5);
			WeaponAbilities.Add(WeaponAbility.BleedAttack, 0.5);
		}

		[Constructable]
		public SpecialNelderimGuard() : base(GuardType.SpecialGuard, rangePerception: 20)
		{
			PackGold(60, 100);
		}

		public SpecialNelderimGuard(Serial serial) : base(serial)
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

