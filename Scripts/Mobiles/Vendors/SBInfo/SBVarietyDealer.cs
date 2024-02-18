using System; 
using System.Collections; 
using Server.Items; 

namespace Server.Mobiles 
{ 
	public class SBVarietyDealer : SBInfo
	{
		private ArrayList m_BuyInfo = new InternalBuyInfo();
		private IShopSellInfo m_SellInfo = new InternalSellInfo();

		public SBVarietyDealer()
		{
		}

		public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
		public override ArrayList BuyInfo { get { return m_BuyInfo; } }

		public class InternalBuyInfo : ArrayList
		{
			public InternalBuyInfo()
			{
				Add( new GenericBuyInfo( "1060834", typeof( Engines.Plants.PlantBowl ), 50, 5, 0x15FD, 0 ) );
			
                Add( new GenericBuyInfo( typeof( SkinningKnife ), 70, 5, 0xEC4, 0 ) );
                Add( new GenericBuyInfo( typeof( Club ), 70, 5, 0x13B4, 0 ) );
                Add( new GenericBuyInfo( typeof( Bow ), 70, 5, 0x13B2, 0 ) );
                Add( new GenericBuyInfo( typeof( Dagger ), 70, 5, 0xF52, 0 ) );
                
                Add( new GenericBuyInfo( typeof( Lute ), 300, 2, 0x0EB3, 0 ) );

				Add( new GenericBuyInfo( typeof( Bolt ), 8, 20, 0x1BFB, 0 ) );
				Add( new GenericBuyInfo( typeof( Arrow ), 6, 25, 0xF3F, 0 ) );

				Add( new GenericBuyInfo( typeof( BlackPearl ), 13, 25, 0xF7A, 0 ) );
				Add( new GenericBuyInfo( typeof( Bloodmoss ), SBHerbalist.GlobalHerbsPriceBuyDouble, 25, 0xF7B, 0 ) );
				Add( new GenericBuyInfo( typeof( Garlic ), SBHerbalist.GlobalHerbsPriceBuyDouble, 25, 0xF84, 0 ) );
				Add( new GenericBuyInfo( typeof( Ginseng ), SBHerbalist.GlobalHerbsPriceBuyDouble, 25, 0xF85, 0 ) );
				Add( new GenericBuyInfo( typeof( MandrakeRoot ), SBHerbalist.GlobalHerbsPriceBuyDouble, 25, 0xF86, 0 ) );
				Add( new GenericBuyInfo( typeof( Nightshade ), SBHerbalist.GlobalHerbsPriceBuyDouble, 25, 0xF88, 0 ) );
				Add( new GenericBuyInfo( typeof( SpidersSilk ), 10, 25, 0xF8D, 0 ) );
				Add( new GenericBuyInfo( typeof( SulfurousAsh ), 10, 25, 0xF8C, 0 ) );

                Add( new GenericBuyInfo( typeof( TinkerTools ), 70, 1, 0x1EB8, 0 ) );
                Add( new GenericBuyInfo( typeof( Shovel ), 120, 1, 0xF39, 0 ) );
                Add( new GenericBuyInfo( typeof( Scissors ), 55, 10, 0xF9F, 0 ) );
				Add( new GenericBuyInfo( typeof( BivalviaNet ), 60, 8, 0x0DD2, 0 ) );
				
				Add( new GenericBuyInfo( typeof( Bottle ), 10, 20, 0xF0E, 0 ) );
				Add( new GenericBuyInfo( typeof( Lockpick ), 20, 10, 0x14FC, 0 ) );
				
				Add( new GenericBuyInfo( typeof( RedBook ), 100, 1, 0xFF1, 0 ) );
				Add( new GenericBuyInfo( typeof( BlueBook ), 100, 1, 0xFF2, 0 ) );
				Add( new GenericBuyInfo( typeof( TanBook ), 100, 1, 0xFF0, 0 ) );
				
				Add( new GenericBuyInfo( typeof( Key ), 10, 5, 0x100E, 0 ) );

				Add( new GenericBuyInfo( typeof( Bedroll ), 20, 5, 0xA59, 0 ) );
				Add( new GenericBuyInfo( typeof( Kindling ), 10, 5, 0xDE1, 0 ) );
				
				Add( new GenericBuyInfo( typeof( Backpack ), 100, 1, 0x9B2, 0 ) );
				Add( new GenericBuyInfo( typeof( Pouch ), 100, 1, 0xE79, 0 ) );
				Add( new GenericBuyInfo( typeof( Bag ), 100, 1, 0xE76, 0 ) );

               // Add(new GenericBuyInfo(typeof(NehkrumorghRecallRune), 1500, 3, 0x1f14, 897));
               // Add(new GenericBuyInfo(typeof(MalluanRecallRune), 1500, 3, 0x1f14, 897));
               // Add(new GenericBuyInfo(typeof(BedwyrgardRecallRune), 1500, 3, 0x1f14, 897));
               // Add(new GenericBuyInfo(typeof(MagizhaarRecallRune), 1500, 3, 0x1f14, 897));
               // Add(new GenericBuyInfo(typeof(PowderOfTranslocation), 2500, 5, 0x26B8, 0));

                //if ( Core.AOS )
                //{
                //    Add( new GenericBuyInfo( typeof( SmallBagBall ), 100, 1, 0x2256, 0 ) );
                //    Add( new GenericBuyInfo( typeof( LargeBagBall ), 100, 1, 0x2257, 0 ) );
                //}

				/*
				if( !Guild.NewGuildSystem )
					Add( new GenericBuyInfo( "1041055", typeof( GuildDeed ), 200000, 1, 0x14F0, 0 ) );
				*/	
					
				/*
				Add( new GenericBuyInfo( typeof( Bandage ), 5, 20, 0xE21, 0 ) );

				Add( new GenericBuyInfo( typeof( BlankScroll ), 5, 25, 0x0E34, 0 ) );

				Add( new GenericBuyInfo( typeof( NightSightPotion ), 15, 5, 0xF06, 0 ) );
				Add( new GenericBuyInfo( typeof( AgilityPotion ), 15, 5, 0xF08, 0 ) );
				Add( new GenericBuyInfo( typeof( StrengthPotion ), 15, 5, 0xF09, 0 ) );
				Add( new GenericBuyInfo( typeof( RefreshPotion ), 15, 5, 0xF0B, 0 ) );
				Add( new GenericBuyInfo( typeof( LesserCurePotion ), 15, 5, 0xF07, 0 ) );
				Add( new GenericBuyInfo( typeof( LesserHealPotion ), 15, 5, 0xF0C, 0 ) );
				Add( new GenericBuyInfo( typeof( LesserPoisonPotion ), 15, 5, 0xF0A, 0 ) );
				Add( new GenericBuyInfo( typeof( LesserExplosionPotion ), 21, 5, 0xF0D, 0 ) );

				Add( new GenericBuyInfo( typeof( Bolt ), 6, Utility.Random( 25, 50 ), 0x1BFB, 0 ) );
				Add( new GenericBuyInfo( typeof( Arrow ), 3, Utility.Random( 25, 50 ), 0xF3F, 0 ) );

				Add( new GenericBuyInfo( typeof( BlackPearl ), 5, 20, 0xF7A, 0 ) ); 
				Add( new GenericBuyInfo( typeof( Bloodmoss ), 5, 20, 0xF7B, 0 ) ); 
				Add( new GenericBuyInfo( typeof( MandrakeRoot ), 3, 20, 0xF86, 0 ) ); 
				Add( new GenericBuyInfo( typeof( Garlic ), 3, 20, 0xF84, 0 ) ); 
				Add( new GenericBuyInfo( typeof( Ginseng ), 3, 20, 0xF85, 0 ) ); 
				Add( new GenericBuyInfo( typeof( Nightshade ), 3, 20, 0xF88, 0 ) ); 
				Add( new GenericBuyInfo( typeof( SpidersSilk ), 3, 20, 0xF8D, 0 ) ); 
				Add( new GenericBuyInfo( typeof( SulfurousAsh ), 3, 20, 0xF8C, 0 ) ); 

				Add( new GenericBuyInfo( typeof( BreadLoaf ), 7, 5, 0x103B, 0 ) );
				Add( new GenericBuyInfo( typeof( Backpack ), 15, 5, 0x9B2, 0 ) );

				Type[] types = Loot.RegularScrollTypes;

				int circles = 3;

				for ( int i = 0; i < circles*8 && i < types.Length; ++i )
				{
					int itemID = 0x1F2E + i;

					if ( i == 6 )
						itemID = 0x1F2D;
					else if ( i > 6 )
						--itemID;

					Add( new GenericBuyInfo( types[i], 12 + ((i / 8) * 10), 20, itemID, 0 ) );
				}

				if ( Core.AOS )
				{
					Add( new GenericBuyInfo( typeof( BatWing ), 3, 20, 0xF78, 0 ) );
					Add( new GenericBuyInfo( typeof( GraveDust ), 3, 20, 0xF8F, 0 ) );
					Add( new GenericBuyInfo( typeof( DaemonBlood ), 6, 20, 0xF7D, 0 ) );
					Add( new GenericBuyInfo( typeof( NoxCrystal ), 6, 20, 0xF8E, 0 ) );
					Add( new GenericBuyInfo( typeof( PigIron ), 5, 20, 0xF8A, 0 ) );

                    // zombie - 18-06-2012 - nie sprzedaje NecromancerSpellbook'ow
					//Add( new GenericBuyInfo( typeof( NecromancerSpellbook ), 115, 10, 0x2253, 0 ) );
                    // zombie
				}

				Add( new GenericBuyInfo( typeof( RecallRune ), 15, 10, 0x1f14, 0 ) );
                // zombie - 18-06-2012 - nie sprzedaje Spellbook'ow
				//Add( new GenericBuyInfo( typeof( Spellbook ), 18, 10, 0xEFA, 0 ) );
                // zombie
				Add( new GenericBuyInfo( "1041072", typeof( MagicWizardsHat ), 11, 10, 0x1718, 0 ) );
				*/
			}
		}

		public class InternalSellInfo : GenericSellInfo
		{
			public InternalSellInfo()
			{
				Add( typeof( Bandage ), 1 );

				Add( typeof( BlankScroll ), 1 );
				

				Add( typeof( NightSightPotion ), 2 );
				Add( typeof( AgilityPotion ), 2 );
				Add( typeof( StrengthPotion ), 2 );
				Add( typeof( RefreshPotion ), 2 );
				Add( typeof( LesserCurePotion ), 2 );
				Add( typeof( LesserHealPotion ), 2 );
				Add( typeof( LesserPoisonPotion ), 2 );
				Add( typeof( LesserExplosionPotion ), 2 );

				Add( typeof( Bolt ), 1 );
				Add( typeof( Arrow ), 1 );
				
				Add( typeof( Log ), 1 );
				Add( typeof( Board ), 1 );

				Add( typeof( BlackPearl ), 1 );
				Add( typeof( Bloodmoss ), SBHerbalist.GlobalHerbsPriceSellHalf);
				Add( typeof( MandrakeRoot ), SBHerbalist.GlobalHerbsPriceSellHalf);
				Add( typeof( Garlic ), SBHerbalist.GlobalHerbsPriceSellHalf);
				Add( typeof( Ginseng ), SBHerbalist.GlobalHerbsPriceSellHalf);
				Add( typeof( Nightshade ), SBHerbalist.GlobalHerbsPriceSellHalf);
				Add( typeof( SpidersSilk ), 1 );
				Add( typeof( SulfurousAsh ), 1 );

				Add( typeof( BreadLoaf ), 1 );
				Add( typeof( Backpack ), 1 );
				Add( typeof( RecallRune ), 2 );
				Add( typeof( Spellbook ), 2 );
				Add( typeof( BlankScroll ), 1 );
				
				Add( typeof( BreadLoaf ), 1 ); 
				Add( typeof( FrenchBread ), 1 ); 
				Add( typeof( Cake ), 1 ); 
				Add( typeof( Cookies ), 1 ); 
				Add( typeof( Muffins ), 1 ); 
				Add( typeof( CheesePizza ), 1 ); 
				Add( typeof( ApplePie ), 1 ); 
				Add( typeof( PeachCobbler ), 1 ); 
				Add( typeof( Quiche ), 1 ); 
				Add( typeof( Dough ), 1 ); 
				Add( typeof( JarHoney ), 1 ); 
				Add( typeof( Pitcher ), 1 );
				Add( typeof( SackFlour ), 1 ); 
				Add( typeof( Eggs ), 1 ); 
				
				Add( typeof( LapHarp ), 2 ); 
				Add( typeof( Lute ), 2 ); 
				Add( typeof( Drums ), 2 ); 
				Add( typeof( Harp ), 2 ); 
				Add( typeof( Tambourine ), 2 ); 

				if ( Core.AOS )
				{
					Add( typeof( BatWing ), 1 );
					Add( typeof( GraveDust ), 1 );
					Add( typeof( DaemonBlood ), 1 );
					Add( typeof( NoxCrystal ), 1 );
					Add( typeof( PigIron ), 1 );
				}
				
				Add( typeof( Tongs ), 1 ); 
				Add( typeof( IronIngot ), 1 ); 

				Add( typeof( Buckler ), 3 );
				Add( typeof( BronzeShield ), 3 );
				Add( typeof( MetalShield ), 3 );
				Add( typeof( MetalKiteShield ), 3 );
				Add( typeof( HeaterShield ), 3 );
				Add( typeof( WoodenKiteShield ), 3 );

				Add( typeof( WoodenShield ), 2 );

				Add( typeof( PlateArms ), 5 );
				Add( typeof( PlateChest ), 5 );
				Add( typeof( PlateGloves ), 5 );
				Add( typeof( PlateGorget ), 5 );
				Add( typeof( PlateLegs ), 5 );

				Add( typeof( FemalePlateChest ), 5 );
				Add( typeof( FemaleLeatherChest ), 5 );
				Add( typeof( FemaleStuddedChest ), 3 );
				Add( typeof( LeatherShorts ), 3 );
				Add( typeof( LeatherSkirt ), 3 );
				Add( typeof( LeatherBustierArms ), 3 );
				Add( typeof( StuddedBustierArms ), 3 );

				Add( typeof( Bascinet ), 2 );
				Add( typeof( CloseHelm ), 2 );
				Add( typeof( Helmet ), 2 );
				Add( typeof( NorseHelm ), 2 );
				Add( typeof( PlateHelm ), 2 );

				Add( typeof( ChainCoif ), 3 );
				Add( typeof( ChainChest ), 3 );
				Add( typeof( ChainLegs ), 3 );

				Add( typeof( RingmailArms ), 2 );
				Add( typeof( RingmailChest ), 2 );
				Add( typeof( RingmailGloves ), 2 );
				Add( typeof( RingmailLegs ), 2 );

				Add( typeof( BattleAxe ), 2 );
				Add( typeof( DoubleAxe ), 2 );
				Add( typeof( ExecutionersAxe ), 2 );
				Add( typeof( LargeBattleAxe ), 2 );
				Add( typeof( Pickaxe ), 2 );
				Add( typeof( TwoHandedAxe ), 2 );
				Add( typeof( WarAxe ), 2 );
				Add( typeof( Axe ), 2 );

				Add( typeof( Bardiche ), 2 );
				Add( typeof( Halberd ), 2 );

				Add( typeof( Cleaver ), 1 );
				Add( typeof( Dagger ), 1 );
				Add( typeof( SkinningKnife ), 1 );

				Add( typeof( Club ), 1 );
				Add( typeof( HammerPick ), 1 );
				Add( typeof( Mace ), 1 );
				Add( typeof( Maul ), 1 );
				Add( typeof( WarHammer ), 1 );
				Add( typeof( WarMace ), 1 );

				Add( typeof( HeavyCrossbow ), 3 );
				Add( typeof( Bow ), 3 );
				Add( typeof( Crossbow ), 3 ); 

				if( Core.AOS )
				{
					Add( typeof( CompositeBow ), 3 );
					Add( typeof( RepeatingCrossbow ), 3 );
					Add( typeof( Scepter ), 2 );
					Add( typeof( BladedStaff ), 2 );
					Add( typeof( Scythe ), 2 );
					Add( typeof( BoneHarvester ), 2 );
					Add( typeof( Scepter ), 2 );
					Add( typeof( BladedStaff ), 2 );
					Add( typeof( Pike ), 2 );
					Add( typeof( DoubleBladedStaff ), 2 );
					Add( typeof( Lance ), 2 );
					Add( typeof( CrescentBlade ), 2 );
				}

				Add( typeof( Spear ), 2 );
				Add( typeof( Pitchfork ), 2 );
				Add( typeof( WarFork), 2 );
				Add( typeof( ShortSpear ), 2 );

				Add( typeof( BlackStaff ), 2 );
				Add( typeof( GnarledStaff ), 2 );
				Add( typeof( QuarterStaff ), 2 );
				Add( typeof( ShepherdsCrook ), 2 );

				Add( typeof( SmithHammer ), 1 );

				Add( typeof( Broadsword ), 2 );
				Add( typeof( Cutlass ), 2 );
				Add( typeof( Katana ), 2 );
				Add( typeof( Kryss ), 2 );
				Add( typeof( Longsword ), 2 );
				Add( typeof( Scimitar ), 2 );
				Add( typeof( ThinLongsword ), 2 );
				Add( typeof( VikingSword ), 2 );
				
				Add( typeof( SewingKit ), 1 );
				Add( typeof( Dyes ), 1 );
				Add( typeof( DyeTub ), 1 );

				Add( typeof( BoltOfCloth ), 3 );

				Add( typeof( FancyShirt ), 1 );
				Add( typeof( Shirt ), 1 );

				Add( typeof( ShortPants ), 1 );
				Add( typeof( LongPants ), 1 );

				Add( typeof( Cloak ), 1 );
				Add( typeof( FancyDress ), 1 );
				Add( typeof( Robe ), 1 );
				Add( typeof( PlainDress ), 1 );

				Add( typeof( Skirt ), 1 );
				Add( typeof( Kilt ), 1 );

				Add( typeof( Doublet ), 1 );
				Add( typeof( Tunic ), 1 );
				Add( typeof( JesterSuit ), 1 );

				Add( typeof( FullApron ), 1 );
				Add( typeof( HalfApron ), 1 );

				Add( typeof( JesterHat ), 1 );
				Add( typeof( FloppyHat ), 1 );
				Add( typeof( WideBrimHat ), 1 );
				Add( typeof( Cap ), 1 );
				Add( typeof( SkullCap ), 1 );
				Add( typeof( Bandana ), 1 );
				Add( typeof( TallStrawHat ), 1 );
				Add( typeof( StrawHat ), 1 );
				Add( typeof( WizardsHat ), 1 );
				Add( typeof( Bonnet ), 1 );
				Add( typeof( FeatheredHat ), 1 );
				Add( typeof( TricorneHat ), 1 );

				Add( typeof( SpoolOfThread ), 1 );

				Add( typeof( Flax ), 1 );
				Add( typeof( Cotton ), 1 );
				Add( typeof( Wool ), 1 );
				
				Add( typeof( LeatherArms ), 2 );
				Add( typeof( LeatherChest ), 2 );
				Add( typeof( LeatherGloves ), 2 );
				Add( typeof( LeatherGorget ), 2 );
				Add( typeof( LeatherLegs ), 2 );
				Add( typeof( LeatherCap ), 1 );

				Add( typeof( StuddedArms ), 3 );
				Add( typeof( StuddedChest ), 3 );
				Add( typeof( StuddedGloves ), 3 );
				Add( typeof( StuddedGorget ), 3 );
				Add( typeof( StuddedLegs ), 3 );

				/*
				Type[] types = Loot.RegularScrollTypes;

				for ( int i = 0; i < types.Length; ++i )
					Add( types[i], ((i / 8) + 2) * 5 );
				*/
				
				Add( typeof( RawRibs ), 1 ); 
				Add( typeof( RawLambLeg ), 1 ); 
				Add( typeof( RawChickenLeg ), 1 ); 
				Add( typeof( RawBird ), 1 ); 
				Add( typeof( Bacon ), 1 ); 
				Add( typeof( Sausage ), 1 ); 
				Add( typeof( Ham ), 1 ); 
				
				Add( typeof( Amber ), 10 );
				Add( typeof( Amethyst ), 10 );
				Add( typeof( Citrine ), 10 );
				Add( typeof( Diamond ), 20 );
				Add( typeof( Emerald ), 15 );
				Add( typeof( Ruby ), 15 );
				Add( typeof( Sapphire ), 15 );
				Add( typeof( StarSapphire ), 20 );
				Add( typeof( Tourmaline ), 15 );
				Add( typeof( GoldRing ), 5 );
				Add( typeof( SilverRing ), 5 );
				Add( typeof( Necklace ), 5 );
				Add( typeof( GoldNecklace ), 5 );
				Add( typeof( GoldBeadNecklace ), 5 );
				Add( typeof( SilverNecklace ), 5 );
				Add( typeof( SilverBeadNecklace ), 5 );
				Add( typeof( Beads ), 5 );
				Add( typeof( GoldBracelet ), 5 );
				Add( typeof( SilverBracelet ), 5 );
				Add( typeof( GoldEarrings ), 5 );
				Add( typeof( SilverEarrings ), 5 );
				
				
				Add( typeof( WoodenBox ), 1 );
				Add( typeof( SmallCrate ), 1 );
				Add( typeof( MediumCrate ), 1 );
				Add( typeof( LargeCrate ), 1 );
				Add( typeof( WoodenChest ), 2 );
              
				Add( typeof( LargeTable ), 2 );
				Add( typeof( Nightstand ), 1 );
				Add( typeof( YewWoodTable ), 1 );

				Add( typeof( Throne ), 2 );
				Add( typeof( WoodenThrone ), 1 );
				Add( typeof( Stool ), 1 );
				Add( typeof( FootStool ), 1 );

				Add( typeof( FancyWoodenChairCushion ), 1 );
				Add( typeof( WoodenChairCushion ), 1 );
				Add( typeof( WoodenChair ), 1 );
				Add( typeof( BambooChair ), 1 );
				Add( typeof( WoodenBench ), 1 );

				Add( typeof( Saw ), 1 );
				Add( typeof( Scorp ), 1 );
				Add( typeof( SmoothingPlane ), 1 );
				Add( typeof( DrawKnife ), 1 );
				Add( typeof( Froe ), 1 );
				Add( typeof( Hammer ), 1 );
				Add( typeof( Inshave ), 1 );
				Add( typeof( JointingPlane ), 1 );
				Add( typeof( MouldingPlane ), 1 );
				Add( typeof( DovetailSaw ), 1 );
				Add( typeof( BallotBoxDeed ), 1 );

				Add( typeof( Shovel ), 1 );
				Add( typeof( SewingKit ), 1 );

				Add( typeof( Nails ), 1 );

				Add( typeof( ClockParts ), 1 );
				Add( typeof( AxleGears ), 1 );
				Add( typeof( Hinge ), 1 );
				Add( typeof( Sextant ), 2 );
				Add( typeof( SextantParts ), 1 );
				Add( typeof( Springs ), 1 );

				Add( typeof( Lockpick ), 1 );
				Add( typeof( TinkerTools ), 1 );

				Add( typeof( ButcherKnife ), 1 );
				

			}
		}
	}
}