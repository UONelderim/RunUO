using System;
using System.Configuration;
using Server;
using Server.Items;

namespace Server
{
	public class PowerScrollLootBox : Item
	{
		public enum PowerScrollLootType
		{
			Wondrous = 105,
			Exalted = 110,
			Mythical = 115,
			Legendary = 120
		}

		private PowerScrollLootType m_LootType;
		
		[CommandProperty( AccessLevel.GameMaster )]
		public PowerScrollLootType LootType
		{
			get { return m_LootType; }
			set { m_LootType = value; SetName(); }
		}
		
		public override double DefaultWeight
		{
			get { return 0.01; }
		}
		
		[Constructable]
		public PowerScrollLootBox() : this( PowerScrollLootType.Wondrous)
		{
		}
		
		[Constructable]
		public PowerScrollLootBox(String type) : base( 0x2DF3 )
		{
			if (!PowerScrollLootType.TryParse(type, out m_LootType))
				m_LootType = PowerScrollLootType.Wondrous;
			SetName();
		}
		
		public PowerScrollLootBox(PowerScrollLootType type) : base( 0x2DF3 )
		{
			m_LootType = type;
			SetName();
		}
		
		public PowerScrollLootBox( Serial serial ) : base( serial )
		{
		}

		public override void OnDoubleClick(Mobile from)
		{
			if(Parent != from.Backpack)
				from.SendMessage("Nie mozesz tego uzyc");
			else
			{
				switch (m_LootType)
				{
					case PowerScrollLootType.Wondrous: from.AddToBackpack(PowerScroll.CreateRandomNoCraft(5, 5)); break;
					case PowerScrollLootType.Exalted: from.AddToBackpack(PowerScroll.CreateRandomNoCraft(10, 10)); break;
					case PowerScrollLootType.Mythical: from.AddToBackpack(PowerScroll.CreateRandomNoCraft(15, 15)); break;
					case PowerScrollLootType.Legendary: from.AddToBackpack(PowerScroll.CreateRandomNoCraft(20, 20)); break;
				}
				Delete();
			}
			base.OnDoubleClick(from);
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version

			writer.Write((int)m_LootType);
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			m_LootType = (PowerScrollLootType) reader.ReadInt();
		}

		private void SetName()
		{
			switch (m_LootType)
			{
                case PowerScrollLootType.Wondrous: Name = "Skrzynia Niskiego Zwoju Mocy (105)"; break;
                case PowerScrollLootType.Exalted: Name = "Skrzynia Średniego Zwoju Mocy (110)"; break;
				case PowerScrollLootType.Mythical: Name = "Skrzynia Mitycznego Zwoju Mocy (115)"; break;
				case PowerScrollLootType.Legendary: Name = "Skrzynia Legendarnego Zwoju Mocy (120)"; break;
			}
			InvalidateProperties();
		}
	}
}