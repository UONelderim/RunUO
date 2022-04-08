using System;
using System.Collections;
using System.Collections.Generic;
using Server.ContextMenus;
using Server.Items;
using Server;

namespace Server.Mobiles
{
	public class ArtTrader : BaseVendor
	{
		private ArrayList m_SBInfos = new ArrayList();
		protected override ArrayList SBInfos{ get { return m_SBInfos; } }

		protected override Type Currency
		{
			get { return typeof(ArtefaktowyPyl); }
		}

		public override bool CanTeach{ get{ return true; } }

		[Constructable]
		public ArtTrader() : base( "- Handlarz Artefaktów" )
		{
	
		}

		public override void InitSBInfo()
		{
			m_SBInfos.Add( new SBArtTrader() );
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
		

		public ArtTrader( Serial serial ) : base( serial )
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