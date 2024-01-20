using Server.Items;
using Server;
using System;
using Ultima;

public class SwampTobacco : BaseTobacco
{
	public override void OnSmoke(Mobile m)
	{
		m.SendMessage("Dym z bagiennego ziela napelnia twoje pluca, czujesz niesamowita lekkosc.");

		m.Emote("*wypuszcza z ust kleby dymu z bagiennego ziela*");

		m.PlaySound(0x226);

		int hue = 87;
		SmokeTimer a = new SmokeTimer(m, TimeSpan.FromSeconds(20), 87);
		a.Start();
		CoughTimer b = new CoughTimer(m, hue);
		b.Start();
		m.FixedParticles(0x376A, 9, 32, 5030, hue, 0, EffectLayer.Waist);

		m.RevealingAction();
	}

	private class CoughTimer : Timer
	{
		private Mobile m_Smoker;
		private int m_Hue;

		public CoughTimer(Mobile smoker, int hue) : base(TimeSpan.FromSeconds(13))
		{
			m_Smoker = smoker;
			m_Hue = hue;
		}

		protected override void OnTick()
		{
			base.OnTick();

			// kaszel

			if (m_Smoker.Female)
				m_Smoker.PlaySound(785);
			else
				m_Smoker.PlaySound(1056);

			if (!m_Smoker.Mounted)
				m_Smoker.Animate(33, 5, 1, true, false, 0);

			m_Smoker.FixedParticles(0x376A, 9, 32, 5030, m_Hue, 0, EffectLayer.Waist);

			switch (Utility.Random(4))
			{
				case 0:
					m_Smoker.Emote("*dym z bagiennego ziela wydaje sie ulatywac nawet z uszu postaci*");
					break;
				case 1:
					m_Smoker.Emote("*dym z bagiennego ziela zawirowal fantazyjnie wokol glowy postaci*");
					break;
				case 2:
				default:
					m_Smoker.Emote("*wykaszluje niewielkie klebki dymu bagiennego ziela*");
					break;
			}
		}
	}

	[Constructable]
	public SwampTobacco() : this(1)
	{
	}

	[Constructable]
	public SwampTobacco(int amount) : base(amount)
	{
		Name = "bagienne ziele";
		Hue = 2130; //82;
	}

	public SwampTobacco(Serial serial) : base(serial)
	{
	}

	public override void Serialize(GenericWriter writer)
	{
		base.Serialize(writer);

		writer.Write((int)0); // version
	}

	public override void Deserialize(GenericReader reader)
	{
		base.Deserialize(reader);

		int version = reader.ReadInt();
	}

}
