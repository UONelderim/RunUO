using System;
using System.Collections;
using Server.Items;
using Server.Multis;

namespace Server.Mobiles
{
	public class SBShipwright : SBInfo
	{
		private ArrayList m_BuyInfo = new InternalBuyInfo();
		private IShopSellInfo m_SellInfo = new InternalSellInfo();

		public SBShipwright()
		{
		}

		public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
		public override ArrayList BuyInfo { get { return m_BuyInfo; } }

		public class InternalBuyInfo : ArrayList
		{
			public InternalBuyInfo()
			{
			    //statki 
				Add( new GenericBuyInfo( "1041205", typeof( SmallBoatDeed ), 70000, 3, 0x14F2, 0 ) );
				Add( new GenericBuyInfo( "1041206", typeof( SmallDragonBoatDeed ), 80000, 3, 0x14F2, 0 ) );
				Add( new GenericBuyInfo( "1041207", typeof( MediumBoatDeed ), 90000, 2, 0x14F2, 0 ) );
				Add( new GenericBuyInfo( "1041208", typeof( MediumDragonBoatDeed ), 100000, 2, 0x14F2, 0 ) );
				Add( new GenericBuyInfo( "1041209", typeof( LargeBoatDeed ), 110000, 1, 0x14F2, 0 ) );
				Add( new GenericBuyInfo( "1041210", typeof( LargeDragonBoatDeed ), 120000, 1, 0x14F2, 0 ) );
			}
		}

		public class InternalSellInfo : GenericSellInfo
		{
			public InternalSellInfo()
			{
				// Can you sell deeds back?
				Add( typeof( SmallBoatDeed ), 21000 );
				Add( typeof( SmallDragonBoatDeed ), 24000 );
				Add( typeof( MediumBoatDeed ), 27000 );
				Add( typeof( MediumDragonBoatDeed ), 30000 );
				Add( typeof( LargeBoatDeed ), 33000 );
				Add( typeof( LargeDragonBoatDeed ), 36000 );
			}
		}
	}
}
