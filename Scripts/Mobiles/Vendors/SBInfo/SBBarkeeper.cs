using System;
using System.Collections;
using Server.Items;

namespace Server.Mobiles
{
	public class SBBarkeeper : SBInfo
	{
		private ArrayList m_BuyInfo = new InternalBuyInfo();
		private IShopSellInfo m_SellInfo = new InternalSellInfo();

		public SBBarkeeper()
		{
		}

		public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
		public override ArrayList BuyInfo { get { return m_BuyInfo; } }

		public class InternalBuyInfo : ArrayList
		{
			public InternalBuyInfo()
			{

				if ( Multis.BaseHouse.NewVendorSystem )
					Add( new GenericBuyInfo( "1062332", typeof( VendorRentalContract ), 1252, 50, 0x14F0, 0x672 ) );

				Add( new GenericBuyInfo( "a barkeep contract", typeof( BarkeepContract ), 1252, 50, 0x14F0, 0 ) );
				Add( new GenericBuyInfo( "1041243", typeof( ContractOfEmployment ), 1252, 50, 0x14F0, 0 ) );
				Add( new GenericBuyInfo( typeof( Dices ), 2, 50, 0xFA7, 0 ) );
				Add( new GenericBuyInfo( typeof( Backgammon ), 2, 50, 0xE1C, 0 ) );
				Add( new GenericBuyInfo( "1016449", typeof( CheckerBoard ), 2, 50, 0xFA6, 0 ) );
				Add( new GenericBuyInfo( "1016450", typeof( Chessboard ), 2, 50, 0xFA6, 0 ) );
				// TODO: Bowl of *, tomato soup, baked pie
				Add( new GenericBuyInfo( typeof( LambLeg ), 8, 50, 0x160A, 0 ) );
				Add( new GenericBuyInfo( typeof( CookedBird ), 17, 50, 0x9B7, 0 ) );
				Add( new GenericBuyInfo( typeof( CheeseWheel ), 4, 50, 0x97E, 0 ) );
				Add( new GenericBuyInfo( typeof( BreadLoaf ), 7, 50, 0x103B, 0 ) );
				Add( new BeverageBuyInfo( typeof( Pitcher ), BeverageType.Water, 11, 50, 0x1F9D, 0 ) );
				Add( new BeverageBuyInfo( typeof( Pitcher ), BeverageType.Wine, 11, 50, 0x1F9B, 0 ) );
				Add( new BeverageBuyInfo( typeof( Pitcher ), BeverageType.Milk, 7, 50, 0x9F0, 0 ) );
				Add( new BeverageBuyInfo( typeof( Pitcher ), BeverageType.Liquor, 11, 50, 0x1F99, 0 ) );
				Add( new GenericBuyInfo( typeof( Pitcher ), 7, 50, 0xFF6, 0 ) );
				Add( new BeverageBuyInfo( typeof( Pitcher ), BeverageType.Cider, 11, 50, 0x1F97, 0 ) );
				Add( new BeverageBuyInfo( typeof( Pitcher ), BeverageType.Ale, 11, 50, 0x1F95, 0 ) );
				Add( new BeverageBuyInfo( typeof( Jug ), BeverageType.Cider, 13, 50, 0x9C8, 0 ) );
				Add( new BeverageBuyInfo( typeof( BeverageBottle ), BeverageType.Liquor, 7, 50, 0x99B, 0 ) );
				Add( new BeverageBuyInfo( typeof( BeverageBottle ), BeverageType.Wine, 7, 50, 0x9C7, 0 ) );
				Add( new BeverageBuyInfo( typeof( BeverageBottle ), BeverageType.Ale, 7, 50, 0x99F, 0 ) );
			}
		}

		public class InternalSellInfo : GenericSellInfo
		{
			public InternalSellInfo()
			{
				Add( typeof( BeverageBottle ), 3 );
				Add( typeof( Jug ), 6 );
				Add( typeof( Pitcher ), 5 );
				Add( typeof( GlassMug ), 1 );
				Add( typeof( BreadLoaf ), 3 );
				Add( typeof( CheeseWheel ), 2 );
				Add( typeof( Chessboard ), 1 );
				Add( typeof( CheckerBoard ), 1 );
				Add( typeof( Backgammon ), 1 );
				Add( typeof( Dices ), 1 );
				Add( typeof( ContractOfEmployment ), 626 );
				Add( typeof( BarkeepContract ), 626 );
			}
		}
	}
}