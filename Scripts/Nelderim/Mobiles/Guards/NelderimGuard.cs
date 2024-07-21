using Server.Items;

namespace Server.Mobiles
{
	[CorpseName("zwloki straznika")]
	public class StandardNelderimGuard : BaseNelderimGuard
	{
		[Constructable]
		public StandardNelderimGuard() : base(GuardType.StandardGuard) { }

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
		public MageNelderimGuard() : base(GuardType.MageGuard, FightMode.Criminal) { }

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
		public HeavyNelderimGuard() : base(GuardType.HeavyGuard, FightMode.Criminal) { }

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
		public MountedNelderimGuard() : base(GuardType.MountedGuard, FightMode.Criminal) { }

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
		public ArcherNelderimGuard() : base(GuardType.ArcherGuard, FightMode.Criminal) { }

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
		public EliteNelderimGuard() : base(GuardType.EliteGuard, FightMode.Criminal) { }

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
		public SpecialNelderimGuard() : base(GuardType.SpecialGuard) { }

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

