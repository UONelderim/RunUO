using System;
using Server;
using Server.Multis;
using Server.Mobiles;
using Server.Targeting;
using System.Collections.Generic;

// 01.07.2012 mortuus - Rozroznianie trofeow na dwustronne (na sciane) i jednostronne (statuetki, na podloge). Dodanie parametru Hue. Stworzenie tabeli mobow.
// 01.07.2012 mortuus - Automatyczne generowanie nazw statuek. Przystosowanie systemu dla ryb (BigFish).
// 16.10.2012 mortuus - kilka trofeow dla jednego potwora, losowanie id trofeum. Dziedziczenie HUE potwora przez trofeum.

namespace Server.Items
{
	[FlipableAttribute( 0x1EBA, 0x1EBB )]
	public class TaxidermyKit : Item
	{
		public override int LabelNumber{ get{ return 1041279; } } // a taxidermy kit

		[Constructable]
		public TaxidermyKit() : base( 0x1EBA )
		{
			Weight = 1.0;
		}

		public TaxidermyKit( Serial serial ) : base( serial )
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

		public override void OnDoubleClick(Mobile from)
		{
			if ( !IsChildOf( from.Backpack ) )
			{
				from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
			}
			// mortuus 02.07.2012 - nie wymagamy skilla Stolarstwo:
			//else if ( from.Skills[SkillName.Carpentry].Base < 90.0 )
			//{
			//	from.SendLocalizedMessage( 1042594 ); // You do not understand how to use this.
			//}
			else
			{
				from.SendLocalizedMessage( 1042595 ); // Target the corpse to make a trophy out of.
				from.Target = new CorpseTarget( this );
			}
		}

		private static TrophyInfo[] m_Table = new TrophyInfo[]
		{
			//  Mozliwe sposoby uzycia:
			//  TrophyInfo( typeof( typStwora ),	IdGrafikaScienna1, IdGrafikaScienna2, HUE ),// hue -99 oznacza ze trofeum bierze hue z potwora (Mobile)
			//  TrophyInfo( typeof( typStwora ),	IdGrafikaPodlogowa, -1, HUE ), // hue -99 oznacza ze trofeum bierze hue z potwora (Mobile)
			new TrophyInfo( typeof( Troll ),		0x1E6A,	0x1E63, 0 ),
			new TrophyInfo( typeof( TrollLord ),	0x1E6D,	0x1E66, 0 ),
			new TrophyInfo( typeof( Orc ),			0x1E6B,	0x1E64, 0 ),    
			new TrophyInfo( typeof( OrcishMage ),	0x1E6B,	0x1E64, 0 ),
			new TrophyInfo( typeof( OrcishLord ),	0x1E6B,	0x1E64, 0 ),
			new TrophyInfo( typeof( OrcCaptain ),	0x1E6B,	0x1E64, 0 ),
			new TrophyInfo( typeof( BigFish ),		0x1E69, 0x1E62, 0 ),
			new TrophyInfo( typeof( GrizzlyBear ),	0x1E67,	0x1E60,	0 ),
			new TrophyInfo( typeof( PolarBear ),	0x1E6C,	0x1E65, 0 ),
			new TrophyInfo( typeof( BlackBear ),	0x20CF,	-1,	0 ),							// 0x2118-zaMaly
			new TrophyInfo( typeof( BrownBear ),	0x20CF,	-1, 0x0A8C ),
			new TrophyInfo( typeof( GreyWolf ),		0x20EA,	-1, 0x03E5 ),	// (szary)
			new TrophyInfo( typeof( TimberWolf ),	0x20EA,	-1, 0     ),	// (lesny)
			new TrophyInfo( typeof( DireWolf ),		0x20EA,	-1, 0x0455 ),	// (wsciekly)
			new TrophyInfo( typeof( WhiteWolf ),	0x20EA,	-1, 0x0385 ),	// (bialy)
			//new TrophyInfo( typeof( InjuredWolf ),	0x20EA,	-1, 0x090E ),	// (injured)
			new TrophyInfo( typeof( GreatHart ),	0x1E68,	0x1E61, 0 ),
			//new TrophyInfo( typeof( Pig ),			0x1E8F,	0x1E8E, 0 ),	// za nisko sciany wisi
			new TrophyInfo( typeof( Hind ),			0x20D4,	-1, -99 ),
			new TrophyInfo( typeof( Gorilla ),		0x20C9,	-1, -99 ),
			new TrophyInfo( typeof( GiantRat ),		0x20D0,	-1, -99 ),
			new TrophyInfo( typeof( Rabbit ),		0x2125,	-1,	-99 ),
			new TrophyInfo( typeof( Dog ),			0x20D5,	-1, -99 ),
			new TrophyInfo( typeof( Bird ),			0x211A,	-1, -99 ),
			new TrophyInfo( typeof( Bird ),			0x20EE,	-1, -99 ),
			new TrophyInfo( typeof( Eagle ),		0x211D,	-1, -99 ),
			new TrophyInfo( typeof( Eagle ),		0x20F2,	-1, -99 ),
			new TrophyInfo( typeof( Walrus ),		0x20FF,	-1, -99 ),
			new TrophyInfo( typeof( Wisp ),			0x2100,	-1, -99 ),
			new TrophyInfo( typeof( Panther ),		0x2102,	-1, 0x901 ), 	// 0x2119-mniejsza (big cat,kot)
			new TrophyInfo( typeof( Cougar ),		0x2102,	-1, -99 ),
			new TrophyInfo( typeof( Cat ),			0x211B,	-1, -99 ),
			new TrophyInfo( typeof( Gazer ),		0x20F4, -1, -99 ),
			new TrophyInfo( typeof( ElderGazer ),	0x20F4,	-1, -99 ),
			new TrophyInfo( typeof( Snake ),		0x20FE,	-1, -99 ),
			new TrophyInfo( typeof( Crane ),		0x2764,	-1, -99 ),
			new TrophyInfo( typeof( SummonedDaemon ),0x2104, -1, -99 ),			
			new TrophyInfo( typeof( Dragon ),		0x2235,	0x2234, -99 ),
			new TrophyInfo( typeof( Pixie ),		0x2A72,	0x2A71, 0 ),
			new TrophyInfo( typeof( Pixie ),		0x2A74,	0x2A73, 0 ),
			new TrophyInfo( typeof( Pixie ),		0x2A76,	0x2A75, 0 ),
			new TrophyInfo( typeof( Pixie ),		0x2A78,	0x2A77, 0 ),
			new TrophyInfo( typeof( Pixie ),		0x2A7A,	0x2A79, 0 ),
			new TrophyInfo( typeof( Pixie ),		0x2D8A,	-1, -99 ),
			new TrophyInfo( typeof( Unicorn ),		0x3158,	0x3159, 0 ),			
			//new TrophyInfo( typeof( Daemon ),		0x224E,	0x224F, 0x8D ),	// za nisko sciany wisi
			//new TrophyInfo( typeof( Daemon ),		0x2250,	0x2251, 0x8D ),	// za nisko sciany wisi
			new TrophyInfo( typeof( Lizardman ),	0x20CA,	-1, 0 )
			
			/*
			//new TrophyInfo( typeof( PolarBear ),		0x20E1,	-1, 0 ),	
			//new TrophyInfo( typeof( GrizzlyBear ),	0x20DB,	-1, 0 ), // 0x20DB-za maly 0x211E-za maly
			//new TrophyInfo( typeof( Rat ),			0x2123,	-1, 0 ),	// troche za maly
			//new TrophyInfo( typeof( Sheep ),			0x20EB,	-1, 0 ),	// lipne:0x20EB
			//new TrophyInfo( typeof( Alligator ),		0x20DA,	-1, 0 ),	// za maly
			//new TrophyInfo( typeof( Chicken ),		0x20D1,	-1, 0 ),	// troche za duzy
			//new TrophyInfo( typeof( Bull ),			0x20EF,	-1, 0 ),	// ZaMale: 0x20EF-nielaciaty 0x20F0-lacaity						
			//new TrophyInfo( typeof( Dolphin ),		0x20F1,	-1, 0 ),	// dziwny
			//new TrophyInfo( typeof( Pig ),			0x2101,	-1, 0 ),	// wieksza	
			//new TrophyInfo( typeof( Cow ),			0x2103,	-1, 0 ),	// za mala
			//new TrophyInfo( typeof( MountainGoat ),	0x2108,	-1, 0 ),	// zaMala
			//new TrophyInfo( typeof( Goat ),			0x2580,	-1, 0 ),	// ladna, za zala, niepodobna	
			
			//new TrophyInfo( typeof( Llama ),			0x20F6,	-1, 0 ),	// za male
			//new TrophyInfo( typeof( Horse ),			0x211F,	-1, 0 ),	// za male: 0x2120-jasny  0x2121-ciemny  0x211F-bialy  0x2124-jasnyLepszy?
			//new TrophyInfo( typeof( Ridgeback ),		0x2615,	-1, 0 ),	// za male
			//new TrophyInfo( typeof( PackHorse ),		0x2126,	-1, 0 ),	// za male
			//new TrophyInfo( typeof( PackLlama ),		0x2127,	-1, 0 ),	// za male
			//new TrophyInfo( typeof( DesertOstard ),	0x2135,	-1, 0 ),	// za male
			//new TrophyInfo( typeof( FrenziedOstard ),	0x2136,	-1, 0 ),	// za male
			//new TrophyInfo( typeof( ForestOstard ),	0x2137,	-1, 0 ),	// za male
			//new TrophyInfo( typeof( SilverSteed ),	0x259D,	-1, 0 ),	// za male
			//new TrophyInfo( typeof( FireSteed ),		0x21F1,	-1, 0 ),	// za male
			//new TrophyInfo( typeof( Beetle ),			0x260F,	-1, 0 ),	// za male
			//new TrophyInfo( typeof( Nightmare ),		0x259C,	-1, 0 ),	// za male
		
			//new TrophyInfo( typeof( Cyclops ),	0x212D,	-1, 0 ),	// za male
			//new TrophyInfo( typeof( Titan ),		0x25CD,	-1, 0 ),	// za male
			//new TrophyInfo( typeof( Troll ),		0x20E9,	-1, 0 ),	// za male
			//new TrophyInfo( typeof( TrollLord ),	0x1E63,	0x1E6A, 0 ),	// za male
			//new TrophyInfo( typeof( Ettin ),		0x20C8,	-1, 0 ),	// za male		
			//new TrophyInfo( typeof( Ogre ),		0x20DF,	-1, 0 ),	// za male
			//new TrophyInfo( typeof( OgreLord ),	0x20CB,	-1, 0 ),	// za male
			//new TrophyInfo( typeof( Orc ),		0x20E0,	-1, 0 ),	// za male
			//new TrophyInfo( typeof( OrcCaptain ),	0x25AF,	-1, 0 ),	// za male
			//new TrophyInfo( typeof( OrcishLord ),	0x25B0,	-1, 0 ),	// za male
			//new TrophyInfo( typeof( OrcishMage ),	0x25B1,	-1, 0 ),	// za male
			//new TrophyInfo( typeof( Ratman ),		0x20E3,	-1, 0 ),	// za male
			//new TrophyInfo( typeof( RatmanArcher ),	0x20E3,	-1, 0 ),	// za male
			//new TrophyInfo( typeof( RatmanMage ),	0x20E3,	-1, 0 ),	// za male			
			//new TrophyInfo( typeof( Harpy ),		0x20DC,	-1, 0 ),	// za male
			//new TrophyInfo( typeof( StoneHarpy ),	0x2594,	-1, 0 ),	// za male
			
			//new TrophyInfo( typeof( Scorpion ),	0x20E4,	-1, 0 ),	// za male
			//new TrophyInfo( typeof( Mongbat ),	0x20F9,	-1, 0 ),	// za male
			//new TrophyInfo( typeof( Reaper ),		0x20FA,	-1, 0 ),	// za male
			//new TrophyInfo( typeof( SeaSerpent ),	0x20FB,	-1, 0 ),	// za male
			//new TrophyInfo( typeof( GiantSerpent ),	0x20FC,	-1, 0 ),	// za male		
		
			//new TrophyInfo( typeof( GiantSpider ),		0x20FD,	-1, 0 ),	// za male
			//new TrophyInfo( typeof( GiantBlackWidow ),	0x25C3,	-1, 0 ),	// za male
			//new TrophyInfo( typeof( DreadSpider ),		0x25C4,	-1, 0 ),	// za male
			//new TrophyInfo( typeof( FrostSpider ),		0x25C5,	-1, 0 ),	// za male			
		
			//new TrophyInfo( typeof( TerathanDrone ),		0x212B,	-1, 0 ),	// za male // 0x25C9-inny
			//new TrophyInfo( typeof( TerathanWarrior ),	0x212A,	-1, 0 ),	// za male // 0x25CC-inny		
			//new TrophyInfo( typeof( TerathanMatriarch ),	0x212C,	-1, 0 ),	// za male // 0x25CB-queen
			//new TrophyInfo( typeof( TerathanAvenger ),	0x25CA,	-1, 0 ),	// za male
			
			//new TrophyInfo( typeof( OphidianMage ),			0x2132,	-1, 0 ),	// za male
			//new TrophyInfo( typeof( OphidianArchmage ),		0x25AC,	-1, 0 ),	// za male
			//new TrophyInfo( typeof( OphidianWarrior ),		0x2133,	-1, 0 ),	// za male
			//new TrophyInfo( typeof( OphidianKnight ),		0x25AA,	-1, 0 ),	// za male
			//new TrophyInfo( typeof( OphidianMatriarch ),	0x2134,	-1, 0 ),	// za male // 0x25AC-bok
			
			//new TrophyInfo( typeof( Gorgoyle ),			0x20D9,	-1, 0 ),	// za male
			//new TrophyInfo( typeof( GargoyleDestroyer ),	0x258D,	-1, 0 ),	// za male
			//new TrophyInfo( typeof( StoneGargoyle ),		0x258E,	-1, 0 ),	// za male
			
			//new TrophyInfo( typeof( Zombie ),			0x20EC,	-1, 0 ),	// za male
			//new TrophyInfo( typeof( Lich ),			0x20F8,	-1, 0 ),	// za male
			//new TrophyInfo( typeof( LichLord ),		0x25A5,	-1, 0 ),	// za male
			//new TrophyInfo( typeof( Ghoul ),			0x2109,	-1, 0 ),	// za male
			//new TrophyInfo( typeof( HeadlessOne ),	0x210A,	-1, 0 ),	// za male
			//new TrophyInfo( typeof( Mummy ),			0x25A7,	-1, 0 ),	// za male
			// //new TrophyInfo( typeof( Skeleton ),	0x20E7,	-1, 0 ),	// za male
			//new TrophyInfo( typeof( Skeleton ),		0x25BC,	-1, 0 ),	// za male
			//new TrophyInfo( typeof( SkeletalKnight ),	0x25BD,	-1, 0 ),	// za male
			//new TrophyInfo( typeof( SkeletalMage ),	0x25BE,	-1, 0 ),	// za male			
			
			//new TrophyInfo( typeof( Slime ),		0x20E8,	-1, 0 ),	// za duze
			//new TrophyInfo( typeof( GiantToad ),	0x212F,	-1, 0 ),	// za male
			//new TrophyInfo( typeof( BullFrog ),	0x2130,	-1, 0 ),	// za male
			//new TrophyInfo( typeof( LavaLizard ),	0x2131,	-1, 0 ),	// za male
			
			//new TrophyInfo( typeof( Wyvern ),		0x25D4,	-1, 0 ),	// za male
			//new TrophyInfo( typeof( Dragon ),		0x20D6,	-1, 0 ),	// za male
			//new TrophyInfo( typeof( Kraken ),		0x25A2,	-1, 0 ),	// za male			
			
			//new TrophyInfo( typeof( RedSolenQueen ),	0x2602,	-1, 0 ),	// za male //red
			//new TrophyInfo( typeof( RedSolenWarrior ),	0x2603,	-1, 0 ),	// za male //red
			//new TrophyInfo( typeof( RedSolenWorker ),	0x2604,	-1, 0 ),	// za male //red			
			
			//new TrophyInfo( typeof( bagienny potwor ),	0x2608,	-1, 0 ),	// za male
			//new TrophyInfo( typeof( Quagmire ),		0x2614,	-1, 0 ),	// za male
			//new TrophyInfo( typeof( Corpser ),			0x20D2,	-1, 0 ),	// za male
			//new TrophyInfo( typeof( WhippingVine ),		0x20D2,	-1, 0 ),	// za male			

			//new TrophyInfo( typeof( FireDaemon ),		0x20D3,	-1, 0 ),	// za male
			//new TrophyInfo( typeof( ChaosDemon ),		0x2609,	-1, 0 ),	// za male
			//new TrophyInfo( typeof( ArcaneDaemon ),	0x2605,	-1, 0 ),	// za male
			//new TrophyInfo( typeof( HordeDaemon ),	0x260E,	-1, 0 ),	// za male //niebieski czteroreki
			//new TrophyInfo( typeof( HordeMinion ),	0x2611,	-1, 0 ),	// za male		
			
			//new TrophyInfo( typeof( EvilMage ),			0x258A,	-1, 0 ),	// za male
			//new TrophyInfo( typeof( MeerMage ),			0x261C,	-1, 0 ),	// za male
			//new TrophyInfo( typeof( MeerWarrior ),		0x261D,	-1, 0 ),	// za male
			//new TrophyInfo( typeof( JukaMage ),			0x261E,	-1, 0 ),	// za male
			//new TrophyInfo( typeof( JukaWarrior ),		0x261F,	-1, 0 ),	// za male			
			
			//new TrophyInfo( typeof( YumotsuElder ),	0x2772,	-1, 0 ),	// za male	
			//new TrophyInfo( typeof( YumotsuPriest ),	0x2773,	-1, 0 ),	// za male	
			
			//new TrophyInfo( typeof( AgapiteElemental ),		0x20D7,	-1, 0x1B9 ),	// za male	
			//new TrophyInfo( typeof( BronzeElemental ),		0x20D7,	-1, 0x156 ),	// za male	
			//new TrophyInfo( typeof( CopperElemental ),		0x20D7,	-1, 0xF9 ),	// za male	
			//new TrophyInfo( typeof( DullCopperElemental ),	0x20D7,	-1, 0x3A7 ),	// za male	
			//new TrophyInfo( typeof( GoldenElemental ),		0x20D7,	-1, 0x35 ),	// za male	
			//new TrophyInfo( typeof( ShadowIronElemental ),	0x20D7,	-1, 0x395 ),	// za male	
			//new TrophyInfo( typeof( ValoriteElemental ),	0x20D7,	-1, 0x5 ),	// za male	
			//new TrophyInfo( typeof( VeriteElemental ),		0x20D7,	-1, 0x41 ),	// za male	
			//new TrophyInfo( typeof( EarthElemental ),	0x20D7,	-1, 0 ),	// za male	
			//new TrophyInfo( typeof( AirElemental ),		0x20ED,	-1, 0 ),	// za male	
			//new TrophyInfo( typeof( FireElemental ),	0x20F3,	-1, 0 ),	// za male	
			//new TrophyInfo( typeof( WaterElemental ),	0x210B,	-1, 0 ),	// za male	
			//new TrophyInfo( typeof( CrystalElemental ),	0x2620,	-1, 0 ),	// za male	
			
			//new TrophyInfo( typeof( Golem ),			0x2610,	-1, 0 ),	// za male
			//new TrophyInfo( typeof( FleshGolem ),		0x2624,	-1, 0 ),	// za male
			//new TrophyInfo( typeof( GoreFiend ),		0x2625,	-1, 0 ),	// za male
			//new TrophyInfo( typeof( Imp ),			0x259F,	-1, 0 ),	// za male
			//new TrophyInfo( typeof( Gibberling ),		0x2627,	-1, 0 ),	// za male
			//new TrophyInfo( typeof(  devourer  ),		0x2623,	-1, 0 ),	// za male
			//new TrophyInfo( typeof( Doppleganger ),	0x260D,	-1, 0 ),	// za male
			
			//new TrophyInfo( typeof( TreeFellow ),		0x2621,	-1, 0 ),	// za male
			//new TrophyInfo( typeof( RaiJu ),			0x2766,	-1, 0 ),	// za male
			//new TrophyInfo( typeof( SnowLady ),		0x276C,	-1, 0 ), 	// za male
			//new TrophyInfo( typeof( Centaur ),			0x2581,	-1, 0 ),	// za male
			//new TrophyInfo( typeof( EtherealWarrior ),	0x2589,	-1, 0 ),	// za male
			//new TrophyInfo( typeof( Pixie ),			0x25B6,	-1, 0 ),	// za male
			//new TrophyInfo( typeof( Kirin ),			0x25A0,	-1, 0 ),	// za male
			//new TrophyInfo( typeof( Unicorn ),			0x25CE,	-1, 0 ),	// za male
			//new TrophyInfo( typeof( BakeKitsune ),		0x2763,	-1, 0 ),	// za male
			//new TrophyInfo( typeof( Gaman ),			0x2768,	-1, 0 ),	// za male
			//new TrophyInfo( typeof( Hiryu ),			0x276A,	-1, 0 ),	// za male
			//new TrophyInfo( typeof( Kappa ),			0x276B,	-1, 0 ),	// za male
			//new TrophyInfo( typeof( Oni ),				0x276D,	-1, 0 ),	// za male
			//new TrophyInfo( typeof( RevenantLion ),		0x276E,	-1, 0 ),	// za male
			//new TrophyInfo( typeof( RuneBeetle ),		0x276F,	-1, 0 ),	// za male
			//new TrophyInfo( typeof( TsukiWolf ),		0x2770,	-1, 0 ),	// za male
			//new TrophyInfo( typeof( Yamandon ),			0x2771,	-1, 0 )	// za male
			*/
		};

		public class TrophyInfo // mortuus 01.07.2012 - dodane skladowe: m_WestID, m_HueVal
		{
			public TrophyInfo( Type type, int id_W, int id_N, int hue )
			{
				m_CreatureType = type;
				m_WestID = id_W;
				m_NorthID = id_N;				
				m_HueVal = hue;
			}

			private Type m_CreatureType;
			private int m_WestID;
			private int m_NorthID;			
			private int m_HueVal;

			public Type CreatureType { get { return m_CreatureType; } }
			public int WestID { get { return m_WestID; } }
			public int NorthID { get { return m_NorthID; } }			
			public int HueVal { get { return m_HueVal; } }			
		}

		private class CorpseTarget : Target
		{
			private TaxidermyKit m_Kit;

			public CorpseTarget( TaxidermyKit kit ) : base( 3, false, TargetFlags.None )
			{
				m_Kit = kit;
			}

			protected override void OnTarget( Mobile from, object targeted )
			{
				if ( m_Kit.Deleted )
					return;

				if ( !(targeted is Corpse) && !(targeted is BigFish) )
				{
					from.SendLocalizedMessage( 1042600 ); // That is not a corpse!
				}
				else if ( targeted is Corpse && ((Corpse)targeted).VisitedByTaxidermist )
				{
					from.SendLocalizedMessage( 1042596 ); // That corpse seems to have been visited by a taxidermist already.
				}
				else if ( !m_Kit.IsChildOf( from.Backpack ) )
				{
					from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
				}
				else if ( from.Skills[SkillName.Camping].Base < 80.0 )	// 07:10.2012 mortuus - robienie trofeow wymaga 80% Mylistwa
				{
					from.SendLocalizedMessage( 1042603 ); // You would not understand how to use the kit.
				}
				else
				{
					object obj = targeted;

					if ( obj is Corpse )
						obj = ((Corpse)obj).Owner;

					if ( obj != null )
					{
						// 16.10.2012 mortuus - kilka trofeow dla jednego potwora, losowanie id trofeum
						List<int> list = new List<int>();
						for ( int i = 0; i < m_Table.Length; i++ )
						{							
							if ( m_Table[i].CreatureType == obj.GetType() )
							{
								list.Add(i);
							}
						}
						
						if( list.Count > 0 )
						{
							int ind = list[Utility.Random(list.Count)];
							
							// 07:10.2012 mortuus - Szansa na trofeum: 60% przy 90 skilla, 80% przy 100 skilla. Gain.
							if( from.CheckTargetSkill( SkillName.Camping, targeted, 50.0, 90.0 ) )
							{
								//from.SendLocalizedMessage( 1042278 ); // You review the corpse and find it worthy of a trophy.
								from.SendLocalizedMessage( 1042602 ); // You use your kit up making the trophy.

								string animalName;
								int weight;
								int trophyHue = m_Table[ind].HueVal;
								if( targeted is BigFish )
								{
									weight = (int) ((BigFish)targeted).Weight;
									//animalName = (string) ((BigFish)targeted).Name;
									animalName = "Duza ryba";	// 1041112 - cliloc: "a big fish"
								}
								else
								{
									weight = 0;
									animalName = (string) ((Mobile)obj).Name;
									if( m_Table[ind].HueVal == -99 )
										trophyHue = ((Mobile)obj).Hue;
								}

								from.AddToBackpack( new TrophyDeed( m_Table[ind].WestID, m_Table[ind].NorthID, trophyHue, from, animalName, weight ) );
								
								if ( targeted is Corpse )
									((Corpse)targeted).VisitedByTaxidermist = true;
								else if ( targeted is BigFish )
									((BigFish)targeted).Consume();
																		
								m_Kit.Delete();
							}
							else
							{
								from.SendMessage( "Nie udalo ci sie stworzyc trofeum. Zmarnowales narzedzie." );
								m_Kit.Delete();
							}
							return;

							//Container pack = from.Backpack;
							//if ( pack != null && pack.ConsumeTotal( typeof( Board ), 0 ) )
							//{						
							// ... 07.10.2012 mortuus - powyzszy if powodowal wymaganie desek, mimo ze nie byly konsumowane
							//}
							//else
							//{
							//	from.SendLocalizedMessage( 1042598 ); // You do not have enough boards.
							//	return;
							//}
						}
					}

					from.SendLocalizedMessage( 1042599 ); // That does not look like something you want hanging on a wall.
				}
			}
		}
	}

	public partial class TrophyAddon : Item, IAddon
	{
		public override bool ForceShowProperties { get { return ObjectPropertyList.Enabled; } }

		private int m_WestID;
		private int m_NorthID;

		private Mobile m_Hunter;
		private string m_AnimalName;
		private int m_AnimalWeight;

		[CommandProperty( AccessLevel.GameMaster )]
		public int WestID{ get{ return m_WestID; } set{ m_WestID = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int NorthID{ get{ return m_NorthID; } set{ m_NorthID = value; } }

		[Constructable]
		public TrophyAddon( Mobile from, int itemID, int westID, int northID, int hue, string name ) : this( from, itemID, westID, northID, hue, null, name, 0 )
		{
		}

		public TrophyAddon( Mobile from, int itemID, int westID, int northID, int hue, Mobile hunter, string name, int animalWeight ) : base( itemID )
		{
			m_WestID = westID;
			m_NorthID = northID;

			m_Hunter = hunter;
			m_AnimalName = name;
			m_AnimalWeight = animalWeight;
			
			Name = "Trofeum: " + name;

			if( hue >= 0 )
				Hue = hue;
			
			Movable = false;

			MoveToWorld( from.Location, from.Map );
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );
			
			if ( m_Hunter != null )
				list.Add( 1070857, m_Hunter.ToString() ); // Caught by ~1_fisherman~

			if( m_AnimalWeight >= 20 )
				list.Add( 1070858, m_AnimalWeight.ToString() ); // ~1_weight~ stones
		}

		public TrophyAddon( Serial serial ) : base( serial )
		{
		}

		public bool CouldFit( IPoint3D p, Map map )
		{
			if ( !map.CanFit( p.X, p.Y, p.Z, this.ItemData.Height ) )
				return false;

			if ( this.ItemID == m_NorthID )
				return BaseAddon.IsWall( p.X, p.Y - 1, p.Z, map ); // North wall
			else
				return BaseAddon.IsWall( p.X - 1, p.Y, p.Z, map ); // West wall
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 3 ); // version

			writer.Write( DateTime.MinValue );

			writer.Write( m_Hunter );
			writer.Write( (int) m_AnimalWeight );

			writer.Write( (int) m_WestID );
			writer.Write( (int) m_NorthID );

			writer.Write( (int)0 );
			writer.Write( (int)0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 3:
                {
					reader.ReadDateTime();//DateCaught

					m_Hunter = reader.ReadMobile();
					m_AnimalWeight = reader.ReadInt();

					m_WestID = reader.ReadInt();
					m_NorthID = reader.ReadInt();

					reader.ReadInt(); //DeedNumber
					reader.ReadInt(); //AddonNumber.
					break;
                }
				case 2:
				case 1:
				{
					reader.ReadString();
					m_Hunter = null;
					m_AnimalWeight = reader.ReadInt();
					AnimalName = reader.ReadString();					
					goto case 0;
				}
				case 0:
				{
					m_WestID = reader.ReadInt();
					m_NorthID = reader.ReadInt();
					break;
				}
			}

			Timer.DelayCall( TimeSpan.Zero, new TimerCallback( FixMovingCrate ) );
		}

		private void FixMovingCrate()
		{
			if ( this.Deleted )
				return;

			if ( this.Movable || this.IsLockedDown )
			{
				Item deed = this.Deed;

				if ( this.Parent is Item )
				{
					((Item)this.Parent).AddItem( deed );
					deed.Location = this.Location;
				}
				else
				{
					deed.MoveToWorld( this.Location, this.Map );
				}

				Delete();
			}
		}

		public Item Deed
		{
			get{ return new TrophyDeed( m_WestID, m_NorthID, Hue, m_Hunter, m_AnimalName, m_AnimalWeight ); }
		}

		public override void OnDoubleClick( Mobile from )
		{
			BaseHouse house = BaseHouse.FindHouseAt( this );

			if ( house != null && house.IsCoOwner( from ) )
			{
				if ( from.InRange( GetWorldLocation(), 1 ) )
				{
					from.AddToBackpack( this.Deed );
					Delete();
				}
				else
				{
					from.SendLocalizedMessage( 500295 ); // You are too far away to do that.
				}
			}
		}
	}

	[Flipable( 0x14F0, 0x14EF )]
	public partial class TrophyDeed : Item	// 01.07.2012 mortuus - dodana skladowa: m_HueVal
	{
		private int m_WestID;
		private int m_NorthID;

		private Mobile m_Hunter;
		private int m_AnimalWeight;

		[CommandProperty( AccessLevel.GameMaster )]
		public int WestID{ get{ return m_WestID; } set{ m_WestID = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int NorthID{ get{ return m_NorthID; } set{ m_NorthID = value; } }
				
		[CommandProperty( AccessLevel.GameMaster )]
		public Mobile Hunter{ get{ return m_Hunter; } set{ m_Hunter = value; InvalidateProperties(); } }
		
		[CommandProperty( AccessLevel.GameMaster )]
		public int AnimalWeight{ get{ return m_AnimalWeight; } set{ m_AnimalWeight = value; InvalidateProperties(); } }

		//public override int LabelNumber{ get{ return m_DeedNumber; } }	// ???

		[Constructable]
		public TrophyDeed( int westID, int northID, int hue, Mobile hunter, string name, int animalWeight ) : base( 0x14F0 )
		{
			m_WestID = westID;
			m_NorthID = northID;
			HueVal = ( hue >= 0 ) ? hue : 0;
			m_Hunter = hunter;
			AnimalName = name;
			m_AnimalWeight = animalWeight;
			
			Name = "Zwoj trofeum: " + name;
		}
		/*
		public TrophyDeed( TaxidermyKit.TrophyInfo info, string hunter, string name, int animalWeight, int animalHue ) : this( info.WestID, info.NorthID, info.HueVal, hunter, name, animalWeight )
		{
		}
		*/
		public TrophyDeed( Serial serial ) : base( serial )
		{
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );

			if ( m_Hunter != null )
				list.Add( 1070857, m_Hunter.ToString() ); // Caught by ~1_fisherman~

			if( m_AnimalWeight >= 20 )
				list.Add( 1070858, m_AnimalWeight.ToString() ); // ~1_weight~ stones
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 3 ); // version
            
			writer.Write( DateTime.MinValue ); //DateCaught

			writer.Write( m_Hunter );
			writer.Write( (int) m_AnimalWeight );

			writer.Write( (int) m_WestID );
			writer.Write( (int) m_NorthID );

			writer.Write( (int)0 ); //DeedNumber
			writer.Write( (int)0 ); //AddonNumber
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 3:
                {
					reader.ReadDateTime();

					m_Hunter = reader.ReadMobile();
					m_AnimalWeight = reader.ReadInt();

					m_WestID = reader.ReadInt();
					m_NorthID = reader.ReadInt();

					reader.ReadInt();
					reader.ReadInt();
					break;
                }
                case 2:
                {
                    HueVal = reader.ReadInt();

                    goto case 1;
                }
				case 1:
				{
					reader.ReadString();
					m_Hunter = null;
					AnimalName = reader.ReadString();
					m_AnimalWeight = reader.ReadInt();
					goto case 0;
				}
				case 0:
				{
					m_WestID = reader.ReadInt();
					m_NorthID = reader.ReadInt();

					break;
				}
			}
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( IsChildOf( from.Backpack ) )
			{
				BaseHouse house = BaseHouse.FindHouseAt( from );

				if ( house != null && house.IsCoOwner( from ) )
				{
					int itemID;
					
					if( m_NorthID < 0 )	// mortuus 01.07.2012 - statuetka jednostronna (nie na sciane):
					{
						itemID = m_WestID;
						if ( itemID > 0 )
						{
							house.Addons.Add( new TrophyAddon( from, itemID, m_WestID, m_NorthID,  HueVal, m_Hunter, AnimalName, m_AnimalWeight ) );
							Delete();
						}
					}
					else		// mortuus 01.07.2012 - statuetka dwustronna (na sciane):
					{
						bool northWall = BaseAddon.IsWall( from.X, from.Y - 1, from.Z, from.Map );
						bool westWall = BaseAddon.IsWall( from.X - 1, from.Y, from.Z, from.Map );

						if ( northWall && westWall )
						{
							switch ( from.Direction & Direction.Mask )
							{
								case Direction.North:
								case Direction.South: northWall = true; westWall = false; break;

								case Direction.East:
								case Direction.West:  northWall = false; westWall = true; break;

								default: from.SendMessage( "Turn to face the wall on which to hang this trophy." ); return;
							}
						}

						itemID = 0;

						if ( northWall )
							itemID = m_NorthID;
						else if ( westWall )
							itemID = m_WestID;
						else
							from.SendLocalizedMessage( 1042626 ); // The trophy must be placed next to a wall.

						if ( itemID > 0 )
						{
							house.Addons.Add( new TrophyAddon( from, itemID, m_WestID, m_NorthID, HueVal, m_Hunter, AnimalName, m_AnimalWeight ) );
							Delete();
						}
					}
				}
				else
				{
					from.SendLocalizedMessage( 502092 ); // You must be in your house to do this.
				}
			}
			else
			{
				from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
			}
		}
	}
}