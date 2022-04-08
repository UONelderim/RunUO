using System;
using System.Collections;
using Server.Items;

namespace Server.Mobiles
{
	public class SBCarpenter: SBInfo
	{
		private ArrayList m_BuyInfo = new InternalBuyInfo();
		private IShopSellInfo m_SellInfo = new InternalSellInfo();

		public SBCarpenter()
		{
		}

		public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
		public override ArrayList BuyInfo { get { return m_BuyInfo; } }

		public class InternalBuyInfo : ArrayList
		{
			public InternalBuyInfo()
			{
				Add( new GenericBuyInfo( typeof( Nails ), 30, 50, 0x102E, 0 ) );
				Add( new GenericBuyInfo( typeof( Axle ), 50, 50, 0x105B, 0 ) );
				Add( new GenericBuyInfo( typeof( Board ), 10, 200, 0x1BD7, 0 ) );
				Add( new GenericBuyInfo( typeof( DrawKnife ), 30, 50, 0x10E4, 0 ) );
				Add( new GenericBuyInfo( typeof( Froe ), 30, 50, 0x10E5, 0 ) );
				Add( new GenericBuyInfo( typeof( Scorp ), 30, 50, 0x10E7, 0 ) );
				Add( new GenericBuyInfo( typeof( Inshave ), 30, 50, 0x10E6, 0 ) );
				Add( new GenericBuyInfo( typeof( DovetailSaw ), 30, 50, 0x1028, 0 ) );
				Add( new GenericBuyInfo( typeof( Saw ), 30, 50, 0x1034, 0 ) );
				Add( new GenericBuyInfo( typeof( Hammer ), 30, 50, 0x102A, 0 ) );
				Add( new GenericBuyInfo( typeof( MouldingPlane ), 30, 50, 0x102C, 0 ) );
				Add( new GenericBuyInfo( typeof( SmoothingPlane ), 30, 50, 0x1032, 0 ) );
				Add( new GenericBuyInfo( typeof( JointingPlane ), 30, 50, 0x1030, 0 ) );
				Add( new GenericBuyInfo( typeof( Drums ), 30, 50, 0xE9C, 0 ) );
				Add( new GenericBuyInfo( typeof( Tambourine ), 30, 50, 0xE9D, 0 ) );
				Add( new GenericBuyInfo( typeof( LapHarp ), 30, 50, 0xEB2, 0 ) );
				Add( new GenericBuyInfo( typeof( Lute ), 30, 50, 0xEB3, 0 ) );
				Add( new GenericBuyInfo( typeof( PowderForWood ), 50000, 20, 4102, 0 ) );

				Add(new GenericBuyInfo(typeof(Bokuto), 21, 20, 0x27A8, 0));
				Add(new GenericBuyInfo(typeof(Tetsubo), 43, 20, 0x27A6, 0));
				Add(new GenericBuyInfo(typeof(Fukiya), 20, 20, 0x27AA, 0));
				Add(new GenericBuyInfo(typeof(BambooFlute), 21, 20, 0x2805, 0));
				Add(new GenericBuyInfo(typeof(BambooFlute), 21, 20, 0x2805, 0));
				Add(new GenericBuyInfo(typeof(Nunchaku), 35, 20, 0x27AE, 0));

				//Add( new GenericBuyInfo( "Rzezbienie w kamieniu", typeof( MasonryBook ), 10000, 20, 0xFBE, 0 ) );
				Add( new GenericBuyInfo( "1044515", typeof( MalletAndChisel ), 100, 25, 0x12B3, 0 ) );
				
				//Add( new GenericBuyInfo( "1041280", typeof( InteriorDecorator ), 10000, 20, 0xFC1, 0 ) );
				//if ( Core.AOS )
					//Add( new GenericBuyInfo( "1060651", typeof( HousePlacementTool ), 1000, 20, 0x14F6, 0 ));
			}
		}

		public class InternalSellInfo : GenericSellInfo
		{
			public InternalSellInfo()
			{
				Add( typeof( WoodenBox ), 7 );
				Add( typeof( SmallCrate ), 5 );
				Add( typeof( MediumCrate ), 6 );
				Add( typeof( LargeCrate ), 7 );
				Add( typeof( WoodenChest ), 15 );
              
				Add( typeof( LargeTable ), 10 );
				Add( typeof( Nightstand ), 7 );
				Add( typeof( YewWoodTable ), 10 );

				Add( typeof( Throne ), 24 );
				Add( typeof( WoodenThrone ), 6 );
				Add( typeof( Stool ), 6 );
				Add( typeof( FootStool ), 6 );

				Add( typeof( FancyWoodenChairCushion ), 12 );
				Add( typeof( WoodenChairCushion ), 10 );
				Add( typeof( WoodenChair ), 8 );
				Add( typeof( BambooChair ), 6 );
				Add( typeof( WoodenBench ), 6 );

				Add( typeof( Saw ), 8 );
				Add( typeof( Scorp ), 6 );
				Add( typeof( SmoothingPlane ), 6 );
				Add( typeof( DrawKnife ), 6 );
				Add( typeof( Froe ), 6 );
				Add( typeof( Hammer ), 3 );
				Add( typeof( Inshave ), 6 );
				Add( typeof( JointingPlane ), 6 );
				Add( typeof( MouldingPlane ), 6 );
				Add( typeof( DovetailSaw ), 7 );
				Add( typeof( Axle ), 1 );

				Add( typeof( WoodenShield ), 15 );
                Add( typeof( BlackStaff ), 11 );
				Add( typeof( GnarledStaff ), 12 );
				Add( typeof( QuarterStaff ), 15 );
				Add( typeof( ShepherdsCrook ), 12 );
				Add( typeof( Club ), 13 );

				Add( typeof( Lute ), 10 );
				Add( typeof( LapHarp ), 10 );
				Add( typeof( Tambourine ), 10 );
				Add( typeof( Drums ), 10 );

				Add( typeof( Log ), 20 );
                Add(typeof( Board ), 3);

				Add(typeof(Tetsubo), 21);
				Add(typeof(Fukiya), 10);
				Add(typeof(BambooFlute), 10);
				Add(typeof(Bokuto), 10);
				Add(typeof(Nunchaku), 17);

				//Add( typeof( MasonryBook ), 3000 );
				Add( typeof( MalletAndChisel ), 10 );
				
				Add( typeof( InteriorDecorator ), 2000 );
				if ( Core.AOS )
					Add( typeof( HousePlacementTool ), 200 );
			}
		}
	}

    public class SBCarpenterAddons : SBInfo
    {
        private ArrayList m_BuyInfo = new InternalBuyInfo();
        private IShopSellInfo m_SellInfo = new InternalSellInfo();

        public SBCarpenterAddons()
        {
        }

        public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
        public override ArrayList BuyInfo { get { return m_BuyInfo; } }

        public class InternalBuyInfo : ArrayList
        {
            public InternalBuyInfo()
            {
                // pusto
            }
        }

        public class InternalSellInfo : GenericSellInfo
        {
            public InternalSellInfo()
            {
                Add(typeof(Log), 3);
                Add(typeof(Board), 3);

                Add(typeof(Saw), 9);
                Add(typeof(Scorp), 6);
                Add(typeof(SmoothingPlane), 6);
                Add(typeof(DrawKnife), 6);
                Add(typeof(Froe), 6);
                Add(typeof(Hammer), 3);
                Add(typeof(Inshave), 6);
                Add(typeof(JointingPlane), 6);
                Add(typeof(MouldingPlane), 6);
                Add(typeof(DovetailSaw), 7);
                Add(typeof(Axle), 1);;
                Add(typeof(Drums), 10);
                Add(typeof(MalletAndChisel), 10);

                //Add( typeof( MasonryBook ), 3000 );               

                Add(typeof(InteriorDecorator), 2000);
                if (Core.AOS)
                    Add(typeof(HousePlacementTool), 200);
            }
        }
    }

    // To samo co w SBCarpenter, ale bez sprzedawania do NPC
    public class SBCarpenterBuy : SBInfo
    {
        private ArrayList m_BuyInfo = new SBCarpenter.InternalBuyInfo();
        private IShopSellInfo m_SellInfo = new GenericSellInfo();  // umyslnie puste

        public SBCarpenterBuy()
        {
        }

        public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
        public override ArrayList BuyInfo { get { return m_BuyInfo; } }
    }
}
