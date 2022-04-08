using System; 
using System.Collections; 
using Server.Items;
using Server.Multis;

namespace Server.Mobiles 
{ 
	public class SBFisherman : SBInfo 
	{ 
		private ArrayList m_BuyInfo = new InternalBuyInfo(); 
		private IShopSellInfo m_SellInfo = new InternalSellInfo(); 

		public SBFisherman() 
		{ 
		} 

		public override IShopSellInfo SellInfo { get { return m_SellInfo; } } 
		public override ArrayList BuyInfo { get { return m_BuyInfo; } } 

		public class InternalBuyInfo : ArrayList 
		{ 
			public InternalBuyInfo() 
			{ 
				Add( new GenericBuyInfo( typeof( RawFishSteak ), 3, 50, 0x97A, 0 ) );
				//TODO: Add( new GenericBuyInfo( typeof( SmallFish ), 3, 20, 0xDD6, 0 ) );
				//TODO: Add( new GenericBuyInfo( typeof( SmallFish ), 3, 20, 0xDD7, 0 ) );
				Add( new GenericBuyInfo( typeof( Fish ), 10, 50, 0x9CC, 0 ) );
				Add( new GenericBuyInfo( typeof( Fish ), 10, 50, 0x9CD, 0 ) );
				Add( new GenericBuyInfo( typeof( Fish ), 10, 50, 0x9CE, 0 ) );
				Add( new GenericBuyInfo( typeof( Fish ), 10, 50, 0x9CF, 0 ) );
				Add( new GenericBuyInfo( typeof( FishingPole ), 30, 50, 0xDC0, 0 ) );
				
				//Add( new GenericBuyInfo( "1060740", typeof( BroadcastCrystal ),  68, 5, 0x1ED0, 0, new object[] {  500 } ) ); // 500 charges
				//Add( new GenericBuyInfo( "1060740", typeof( BroadcastCrystal ), 131, 5, 0x1ED0, 0, new object[] { 1000 } ) ); // 1000 charges
				//Add( new GenericBuyInfo( "1060740", typeof( BroadcastCrystal ), 256, 5, 0x1ED0, 0, new object[] { 2000 } ) ); // 2000 charges

				//Add( new GenericBuyInfo( "1060740", typeof( ReceiverCrystal ), 6, 20, 0x1ED0, 0 ) );

				Add( new GenericBuyInfo( typeof( StarSapphire ), 150, 50, 0xF21, 0 ) );
				Add( new GenericBuyInfo( typeof( Emerald ), 100, 50, 0xF10, 0 ) );
				Add( new GenericBuyInfo( typeof( Sapphire ), 100, 50, 0xF19, 0 ) );
				Add( new GenericBuyInfo( typeof( Ruby ), 100, 50, 0xF13, 0 ) );
				Add( new GenericBuyInfo( typeof( Citrine ), 60, 50, 0xF15, 0 ) );
				Add( new GenericBuyInfo( typeof( Amethyst ), 80, 50, 0xF16, 0 ) );
				Add( new GenericBuyInfo( typeof( Tourmaline ), 100, 50, 0xF2D, 0 ) );
				Add( new GenericBuyInfo( typeof( Amber ), 60, 50, 0xF25, 0 ) );
				Add( new GenericBuyInfo( typeof( Diamond ), 150, 50, 0xF26, 0 ) );
				
				Add( new GenericBuyInfo( typeof( BlankMap ), 10, 50, 0x14EC, 0 ) );
				Add( new GenericBuyInfo( typeof( MapmakersPen ), 30, 50, 0x0FBF, 0 ) );
				Add( new GenericBuyInfo( typeof( BlankScroll ), 12, 50, 0xEF3, 0 ) );
				
				Add( new GenericBuyInfo( typeof( BivalviaNet ), 20, 50, 0xDD2, 0 ) );
                //Add(new GenericBuyInfo(typeof(BoatBuildProject), 1000, 50, 0x14ED, 6));
				// Mapy miedzy miastami
				//for ( int i = 0; i < PresetMapEntry.Table.Length; ++i )
				//	Add( new PresetMapBuyInfo( PresetMapEntry.Table[i], Utility.RandomMinMax( 7, 10 ), 20 ) );

            }    
		} 

		public class InternalSellInfo : GenericSellInfo 
		{ 
			public InternalSellInfo() 
			{
                Add(typeof(Salt), 2);
				Add( typeof( RawFishSteak ), 2 );
				Add( typeof( FishSteak ), 4 );
				Add( typeof( Fish ), 2 );
				Add( typeof( BigFish ), 500 );
				//Add( typeof( SmallFish ), 1 );
				Add( typeof( FishingPole ), 6 );
				Add( typeof( PeculiarFish ), 10 );
				Add( typeof( PrizedFish ), 10 );
				Add( typeof( WondrousFish ), 10 );
				Add( typeof( TrulyRareFish ), 10 );
				
				Add( typeof( Amber ), 30 );
				Add( typeof( Amethyst ), 30 );
				Add( typeof( Citrine ), 30 );
				Add( typeof( Diamond ), 60 );
				Add( typeof( Emerald ), 45 );
				Add( typeof( Ruby ), 45 );
				Add( typeof( Sapphire ), 45 );
				Add( typeof( StarSapphire ), 60 );
				Add( typeof( Tourmaline ), 45 );
				Add( typeof( GoldRing ), 15 );
				Add( typeof( SilverRing ), 15 );
				Add( typeof( Necklace ), 15 );
				Add( typeof( GoldNecklace ), 15 );
				Add( typeof( GoldBeadNecklace ), 15 );
				Add( typeof( SilverNecklace ), 15 );
				Add( typeof( SilverBeadNecklace ), 15 );
				Add( typeof( Beads ), 15 );
				Add( typeof( GoldBracelet ), 15 );
				Add( typeof( SilverBracelet ), 15 );
				Add( typeof( GoldEarrings ), 15 );
				Add( typeof( SilverEarrings ), 15 );
				
				Add( typeof( BlankScroll ), 2 );
				Add( typeof( MapmakersPen ), 8 );
				Add( typeof( BlankMap ), 2 );
				Add( typeof( CityMap ), 3 );
				Add( typeof( LocalMap ), 4 );
				Add( typeof( WorldMap ), 5 );
				Add( typeof( PresetMapEntry ), 3 );
				//TODO: Buy back maps that the mapmaker sells!!!
			} 
		} 
	} 
}