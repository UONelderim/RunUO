using System;
using System.Collections;
using System.Collections.Generic;
using Server.ContextMenus;
using Server.Items;
using Server;

namespace Server.Mobiles
{
	public class PowerScrollTrader : BaseVendor
	{
		private ArrayList m_SBInfos = new ArrayList();
		protected override ArrayList SBInfos{ get { return m_SBInfos; } }

		protected override Type Currency
		{
			get { return typeof(PowerScrollPowder); }
		}

		public override bool CanTeach{ get{ return true; } }

		[Constructable]
		public PowerScrollTrader() : base( "- Handlarz Zwojow Mocy" )
		{
	
		}

		public override bool AllowLootType(Item item)
		{
			// Pozwol graczowi sprzedac "przeklete" itemy
			return true;
		}

		public override void InitSBInfo()
		{
			m_SBInfos.Add( new SBPowerScrollTrader() );
		}

		public override VendorShoeType ShoeType
		{
			get{ return Utility.RandomBool() ? VendorShoeType.Shoes : VendorShoeType.Sandals; }
		}

		public override void InitOutfit()
		{
			base.InitOutfit();

			AddItem( new Server.Items.Robe( Utility.RandomPinkHue() ) );
		}
		

		public PowerScrollTrader( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}