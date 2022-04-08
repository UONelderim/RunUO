using System; 
using System.Collections; 
using Server.Items; 

namespace Server.Mobiles 
{ 
	public class SBHairStylist : SBInfo 
	{ 
		private ArrayList m_BuyInfo = new InternalBuyInfo(); 
		private IShopSellInfo m_SellInfo = new InternalSellInfo(); 

		public SBHairStylist() 
		{ 
		} 

		public override IShopSellInfo SellInfo { get { return m_SellInfo; } } 
		public override ArrayList BuyInfo { get { return m_BuyInfo; } } 

		public class InternalBuyInfo : ArrayList 
		{ 
			public InternalBuyInfo() 
			{ 
				Add( new GenericBuyInfo( "special beard dye", typeof( SpecialBeardDye ), 50000, 50, 0xE26, 0 ) ); 
				Add( new GenericBuyInfo( "special hair dye", typeof( SpecialHairDye ), 50000, 50, 0xE26, 0 ) ); 
				Add( new GenericBuyInfo( "1041060", typeof( HairDye ), 1000, 50, 0xEFF, 0 ) ); 
			} 
		} 

		public class InternalSellInfo : GenericSellInfo 
		{ 
			public InternalSellInfo() 
			{ 
				Add( typeof( HairDye ), 200 ); 
				Add( typeof( SpecialBeardDye ), 15000 ); 
				Add( typeof( SpecialHairDye ), 15000 ); 
			} 
		} 
	} 
}