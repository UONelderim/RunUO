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
				var ps = m_LootType switch
				{
					PowerScrollLootType.Wondrous => PowerScroll.CreateRandomNoCraft(5, 5),
					PowerScrollLootType.Exalted => PowerScroll.CreateRandomNoCraft(10, 10),
					PowerScrollLootType.Mythical => PowerScroll.CreateRandomNoCraft(15, 15),
					PowerScrollLootType.Legendary => PowerScroll.CreateRandomNoCraft(20, 20),
				};
				if (ps != null)
				{
					from.AddToBackpack(ps);
					Delete();
				}
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
			Name = m_LootType switch
			{
				PowerScrollLootType.Wondrous => "Skrzynia Niskiego Zwoju Mocy (105)",
				PowerScrollLootType.Exalted => "Skrzynia Średniego Zwoju Mocy (110)",
				PowerScrollLootType.Mythical => "Skrzynia Mitycznego Zwoju Mocy (115)",
				PowerScrollLootType.Legendary => "Skrzynia Legendarnego Zwoju Mocy (120)",
				_ => Name
			};
		}
	}
}