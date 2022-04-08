// 07.07.2012 :: zombie :: dodanie Conflagration, ConfusionBlast, MaskOfDeath

using System;
using System.Collections.Generic;
using Server;
using Server.Engines.Craft;
using Server.ContextMenus;

namespace Server.Items
{
	public enum PotionEffect
	{
		Nightsight,
		CureLesser,
		Cure,
		CureGreater,
		Agility,
		AgilityGreater,
		Strength,
		StrengthGreater,
		PoisonLesser,
		Poison,
		PoisonGreater,
		PoisonDeadly,
		Refresh,
		RefreshTotal,
		HealLesser,
		Heal,
		HealGreater,
		ExplosionLesser,
		Explosion,
		ExplosionGreater,

		Conflagration,	// po�oga
		ConflagrationGreater,
		ConfusionBlast,	// obezwladniajacy podmuch
		ConfusionBlastGreater,
		MaskOfDeath,
		MaskOfDeathGreater,

		WaterElemental,
		FireElemental,
		EarthElemental,
		Invisibility,
		Revitalize,
		SuperPotion,
		PetResurrect,

		NAgilityGreater, //potezna dexa
		NStrengthGreater, //potezna sily
	}

	public abstract class BasePotion : Item, ICraftable
	{
		private PotionEffect m_PotionEffect;

		public PotionEffect PotionEffect
		{
			get
			{
				return m_PotionEffect;
			}
			set
			{
				m_PotionEffect = value;
				InvalidateProperties();
			}
		}

		public override int LabelNumber { get { return 1041314 + (int)m_PotionEffect; } }

		public BasePotion(int itemID, PotionEffect effect)
			: base(itemID)
		{
			m_PotionEffect = effect;

			Stackable = Core.ML;
			Weight = 1.0;
		}

		public BasePotion(Serial serial)
			: base(serial)
		{
		}

		public virtual bool RequireFreeHand { get { return true; } }

		public static bool HasFreeHand(Mobile m)
		{
			Item handOne = m.FindItemOnLayer(Layer.OneHanded);
			Item handTwo = m.FindItemOnLayer(Layer.TwoHanded);

			if (handTwo is BaseWeapon)
				handOne = handTwo;

			return (handOne == null || handTwo == null);
		}

		public override void OnDoubleClick(Mobile from)
		{
			if (!Movable)
				return;

			if (from.InRange(this.GetWorldLocation(), 1))
			{
				if (!RequireFreeHand || HasFreeHand(from))
					Drink(from);
				else
					from.SendLocalizedMessage(502172); // You must have a free hand to drink a potion.
			}
			else
			{
				from.SendLocalizedMessage(502138); // That is too far away for you to use
			}
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write((int)0); // version

			writer.Write((int)m_PotionEffect);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();

			switch (version)
			{
				case 0:
					{
						m_PotionEffect = (PotionEffect)reader.ReadInt();
						break;
					}
			}
		}

        public virtual void PourOut(Mobile from)
        {
            this.Consume();
            from.PlaySound(0x4B9);
            from.AddToBackpack(new Bottle());
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);
            list.Add(new PourOutPotion(from, this));
        }

		public abstract void Drink(Mobile from);

		public static void PlayDrinkEffect(Mobile m)
		{
			m.RevealingAction();

			m.PlaySound(0x2D6);

			m.AddToBackpack(new Bottle());

			if (m.Body.IsHuman /*&& !m.Mounted*/ )
				m.Animate(34, 5, 1, true, false, 0);
		}

		public static int EnhancePotions(Mobile m)
		{
			int EP = AosAttributes.GetValue(m, AosAttribute.EnhancePotions);
			int skillBonus = m.Skills.Alchemy.Fixed / 330 * 10;

			if (Core.ML && EP > 50 && m.AccessLevel <= AccessLevel.Player)
				EP = 50;

			return (EP + skillBonus);
		}

		public static TimeSpan Scale(Mobile m, TimeSpan v)
		{
			if (!Core.AOS)
				return v;

			double scalar = 1.0 + (0.01 * EnhancePotions(m));

			return TimeSpan.FromSeconds(v.TotalSeconds * scalar);
		}

		public static double Scale(Mobile m, double v)
		{
			if (!Core.AOS)
				return v;

			double scalar = 1.0 + (0.01 * EnhancePotions(m));

			return v * scalar;
		}

		public static int Scale(Mobile m, int v)
		{
			if (!Core.AOS)
				return v;

			return AOS.Scale(v, 100 + EnhancePotions(m));
		}

		public override bool StackWith(Mobile from, Item dropped, bool playSound)
		{
			if (dropped is BasePotion && ((BasePotion)dropped).m_PotionEffect == m_PotionEffect)
				return base.StackWith(from, dropped, playSound);

			return false;
		}
		#region ICraftable Members

		public int OnCraft(int quality, bool makersMark, Mobile from, CraftSystem craftSystem, Type typeRes, Type typeRes2, BaseTool tool, CraftItem craftItem, int resHue)
		{
			if (craftSystem is DefAlchemy)
			{
				Container pack = from.Backpack;

				if (pack != null)
				{
					Item[] kegs = pack.FindItemsByType(typeof(PotionKeg), true);

					for (int i = 0; i < kegs.Length; ++i)
					{
						PotionKeg keg = kegs[i] as PotionKeg;

						if (keg == null)
							continue;

						if (keg.Held <= 0 || keg.Held >= 100)
							continue;

						if (keg.Type != PotionEffect)
							continue;

						++keg.Held;

						Delete();
						from.AddToBackpack(new Bottle());

						return -1; // signal placed in keg
					}
				}
			}

			return 1;
		}

		#endregion
	}

    public class PourOutPotion : ContextMenuEntry
    {
        private Mobile m_From;
        private BasePotion m_pot;

        public PourOutPotion(Mobile from, BasePotion pot)
            : base(6163, 12)
        {
            m_From = from;
            m_pot = pot;
        }

        public override void OnClick()
        {
            m_pot.PourOut(m_From);
        }
    }
}