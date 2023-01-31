#region References

using Server.Items;
using Server.Mobiles;

#endregion

namespace Server.SicknessSys.Items
{
	[Flipable(0x27Ab, 0x27F6)]
	public class WereClaws : BaseSword
	{
		private PlayerMobile pm;

		public PlayerMobile PM
		{
			get { return pm; }
			set { pm = value; }
		}

		private string defaultName = "Szpony Lykana";

		public override string DefaultName
		{
			get { return defaultName; }
		}

		[Constructable]
		public WereClaws(PlayerMobile player) : base(0x27AB)
		{
			pm = player;
			Hue = 1150;
			Weight = 5.0;
			Layer = Layer.TwoHanded;
			LootType = LootType.Blessed;
			WeaponAttributes.HitLowerAttack = 30;
		}

		public WereClaws(Serial serial) : base(serial)
		{
		}

		public override bool OnEquip(Mobile from)
		{
			if (from != pm || pm == null)
				return false;

			return base.OnEquip(from);
		}
		public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.DualWield; } }
		public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.TalonStrike; } }

		//public override int StrengthReq => 10;
		//public override int MinDamage => 10;
		//public override int MaxDamage => 13;
		//public override float Speed => 2.00f;
		private int defHitSound = 0x238;
		private int defMissSound = 0x232;
		private int initMinHits = 35;
		private int initMaxHits = 60;

		public override SkillName DefSkill

		{
			get
			{
				return SkillName.Tactics;
			}
		}

		public override WeaponType DefType
		{
			get
			{
				return WeaponType.Piercing;
			}
		}

		public override WeaponAnimation DefAnimation
		{
			get
			{
				return WeaponAnimation.Pierce1H;
			}
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(0); // version

			writer.Write(pm);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();

			pm = reader.ReadMobile() as PlayerMobile;
		}
	}
}
