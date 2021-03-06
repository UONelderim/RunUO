using System;
using System.Collections;
using Server.Items;

namespace Server.Mobiles
{
	public class SBHelmetArmor: SBInfo
	{
		private ArrayList m_BuyInfo = new InternalBuyInfo();
		private IShopSellInfo m_SellInfo = new InternalSellInfo();

		public SBHelmetArmor()
		{
		}

		public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
		public override ArrayList BuyInfo { get { return m_BuyInfo; } }

		public class InternalBuyInfo : ArrayList
		{
			public InternalBuyInfo()
			{
				
				Add( new GenericBuyInfo( typeof( CloseHelm ), 49, 20, 0x1408, 0 ) );
				Add( new GenericBuyInfo( typeof( CloseHelm ), 49, 20, 0x1409, 0 ) );
				Add( new GenericBuyInfo( typeof( Helmet ), 49, 20, 0x140A, 0 ) );
				Add( new GenericBuyInfo( typeof( Helmet ), 49, 20, 0x140B, 0 ) );
				Add( new GenericBuyInfo( typeof( NorseHelm ), 49, 20, 0x140E, 0 ) );
				Add( new GenericBuyInfo( typeof( NorseHelm ), 49, 20, 0x140F, 0 ) );
				Add( new GenericBuyInfo( typeof( Bascinet ), 49, 20, 0x140C, 0 ) );
				Add( new GenericBuyInfo( typeof( PlateHelm ), 49, 20, 0x1419, 0 ) );
				Add( new GenericBuyInfo( typeof( PlateHelm ), 49, 20, 0x1412, 0 ) );
			}
		}

		public class InternalSellInfo : GenericSellInfo
		{
			public InternalSellInfo()
			{
				Add( typeof( Bascinet ), 9 );
				Add( typeof( CloseHelm ), 9 );
				Add( typeof( Helmet ), 9 );
				Add( typeof( NorseHelm ), 9 );
				Add( typeof( PlateHelm ), 10 );
			}
		}
	}
}
