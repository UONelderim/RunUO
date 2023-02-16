using System;
using Server.Items;
using Server.Multis;

namespace Server.Engines.Craft
{
	public class DefCarpentry : CraftSystem
	{
		public override SkillName MainSkill
		{
			get	{ return SkillName.Carpentry;	}
		}

		public override int GumpTitleNumber
		{
			get { return 1044004; } // <CENTER>CARPENTRY MENU</CENTER>
		}

		private static CraftSystem m_CraftSystem;

		public static CraftSystem CraftSystem
		{
			get
			{
				if ( m_CraftSystem == null )
					m_CraftSystem = new DefCarpentry();

				return m_CraftSystem;
			}
		}

		public override double GetChanceAtMin( CraftItem item )
		{
			return 0.5; // 50%
		}

		private DefCarpentry() : base( 1, 1, 1.25 )// base( 1, 1, 3.0 )
		{
		}

		public override int CanCraft( Mobile from, BaseTool tool, Type itemType )
		{
			if( tool == null || tool.Deleted || tool.UsesRemaining < 0 )
				return 1044038; // You have worn out your tool!
			else if ( !BaseTool.CheckAccessible( tool, from ) )
				return 1044263; // The tool must be on your person to use.

			return 0;
		}

		public override void PlayCraftEffect( Mobile from )
		{
			// no animation
			//if ( from.Body.Type == BodyType.Human && !from.Mounted )
			//	from.Animate( 9, 5, 1, true, false, 0 );

			from.PlaySound( 0x23D );
		}

		public override int PlayEndingEffect( Mobile from, bool failed, bool lostMaterial, bool toolBroken, int quality, bool makersMark, CraftItem item )
		{
			if ( toolBroken )
				from.SendLocalizedMessage( 1044038 ); // You have worn out your tool

			if ( failed )
			{
				if ( lostMaterial )
					return 1044043; // You failed to create the item, and some of your materials are lost.
				else
					return 1044157; // You failed to create the item, but no materials were lost.
			}
			else
			{
				if ( quality == 0 )
					return 502785; // You were barely able to make this item.  It's quality is below average.
				else if ( makersMark && quality == 2 )
					return 1044156; // You create an exceptional quality item and affix your maker's mark.
				else if ( quality == 2 )
					return 1044155; // You create an exceptional quality item.
				else				
					return 1044154; // You create the item.
			}
		}

		public override void InitCraftList()
		{
			int index = -1;

			// Other Items
			index =	AddCraft( typeof( Board ),				1044294, 1027127,	 0.0,   0.0,	typeof( Log ), 1044466,  1, 1044465 );
			SetUseAllRes( index, true );

			AddCraft( typeof( BarrelStavesRC ),				1044294, 1027857,	00.0,  25.0,	typeof( Log ), 1044041,  5, 1044351 );
			AddCraft( typeof( BarrelLidRC ),				1044294, 1027608,	11.0,  36.0,	typeof( Log ), 1044041,  4, 1044351 );
			AddCraft( typeof( ShortMusicStandRC ),			1044294, 1044313,	78.9, 103.9,	typeof( Log ), 1044041, 15, 1044351 );
			AddCraft( typeof( TallMusicStandRC ),			1044294, 1044315,	81.5, 106.5,	typeof( Log ), 1044041, 20, 1044351 );
			AddCraft( typeof( EasleRC ),					1044294, 1044317,	86.8, 111.8,	typeof( Log ), 1044041, 20, 1044351 );
			if( Core.SE )
			{
				index = AddCraft( typeof( RedHangingLanternRC ), 1044294, 1029412, 65.0, 90.0, typeof( Log ), 1044041, 5, 1044351 );
				AddRes( index, typeof( BlankScroll ), 1044377, 10, 1044378 );
				SetNeededExpansion( index, Expansion.SE );

				index = AddCraft( typeof( WhiteHangingLanternRC ), 1044294, 1029416, 65.0, 90.0, typeof( Log ), 1044041, 5, 1044351 );
				AddRes( index, typeof( BlankScroll ), 1044377, 10, 1044378 );
				SetNeededExpansion( index, Expansion.SE );

				index = AddCraft( typeof( ShojiScreenRC ), 1044294, 1029423, 80.0, 105.0, typeof( Log ), 1044041, 75, 1044351 );
				AddSkill( index, SkillName.Tailoring, 50.0, 55.0 );
				AddRes( index, typeof( Cloth ), 1044286, 60, 1044287 );
				SetNeededExpansion( index, Expansion.SE );

				index = AddCraft( typeof( BambooScreenRC ), 1044294, 1029428, 80.0, 105.0, typeof( Log ), 1044041, 75, 1044351 );
				AddSkill( index, SkillName.Tailoring, 50.0, 55.0 );
				AddRes( index, typeof( Cloth ), 1044286, 60, 1044287 );
				SetNeededExpansion( index, Expansion.SE );
			}

			if( Core.AOS )	//Duplicate Entries to preserve ordering depending on era 
			{
				index = AddCraft( typeof( FishingPole ), 1044294, 1023519, 40.0, 90.0, typeof( Log ), 1044041, 5, 1044351 ); //This is in the categor of Other during AoS
				//AddSkill( index, SkillName.Tailoring, 40.0, 45.0 );
				AddRes( index, typeof( Cloth ), 1044286, 5, 1044287 );
			}
			AddCraft( typeof( SeedBox ),					1044294, 1054153,	65.0, 90.0,	typeof( Log ), 1044041, 50, 1044351 );
			index =AddCraft( typeof( Canvas ),						1044294, "Plotno", 60.0, 88.0, typeof( Log ), 1044041, 5, 1044351 );
			AddSkill( index, SkillName.Tailoring, 50.0, 55.0 );
			AddRes( index, typeof( Cloth ), 1044286, 60, 1044287 );
			
			// MLowa zbroja
			index = AddCraft( typeof( RavenHelm ), "zbroja", "Sokoli Hełm", 70.0, 95.0, typeof( Log ), 1044041, 6, 1044351 );

			AddRes( index, typeof( Feather ), "pióro", 25, 1044037 );
			index = AddCraft( typeof( VultureHelm ), "zbroja", "Orli Hełm", 70.0, 95.0, typeof( Log ), 1044041, 7, 1044351 );

			AddRes( index, typeof( Feather ), "pióro", 25, 1044037 );
			index = AddCraft( typeof( WingedHelm ), "zbroja", "Skrzydlaty hełm", 70.0, 95.0, typeof( Log ), 1044041, 7, 1044351 );

			AddRes( index, typeof( Feather ), "pióro", 60, 1044037 );
			index = AddCraft( typeof( WoodlandArms ), "zbroja", "Inkrustowane naramienniki", 85.0, 105.9, typeof( Log ), 1044041, 6, 1044351 );

			index = AddCraft( typeof( WoodlandChest ), "zbroja", "inkrustowana tunika", 99.0, 115.9, typeof( Log ), 1044041, 16, 1044351 );

			index = AddCraft( typeof( WoodlandGloves ), "zbroja", "inkrustowane rękawice", 80.0, 100.9, typeof( Log ), 1044041, 6, 1044351 );

			index = AddCraft( typeof( WoodlandGorget ), "zbroja", "inkrustowany karczek", 80.0, 100.9, typeof( Log ), 1044041, 4, 1044351 );

			index = AddCraft( typeof( WoodlandLegs ), "zbroja", "inkrustowane nogwice", 80.0, 100.9, typeof( Log ), 1044041, 14, 1044351 );
		
			
			// Furniture
			AddCraft( typeof( FootStoolRC ),				1044291, 1022910,	11.0,  36.0,	typeof( Log ), 1044041,  9, 1044351 );
			AddCraft( typeof( StoolRC ),					1044291, 1022602,	11.0,  36.0,	typeof( Log ), 1044041,  9, 1044351 );
			AddCraft( typeof( BambooChairRC ),				1044291, 1044300,	21.0,  46.0,	typeof( Log ), 1044041, 13, 1044351 );
			AddCraft( typeof( WoodenChairRC ),				1044291, 1044301,	21.0,  46.0,	typeof( Log ), 1044041, 13, 1044351 );
			AddCraft( typeof( FancyWoodenChairCushionRC ),	1044291, 1044302,	42.1,  67.1,	typeof( Log ), 1044041, 15, 1044351 );
			AddCraft( typeof( WoodenChairCushionRC ),		1044291, 1044303,	42.1,  67.1,	typeof( Log ), 1044041, 13, 1044351 );
			AddCraft( typeof( WoodenBenchRC ),				1044291, 1022860,	52.6,  77.6,	typeof( Log ), 1044041, 17, 1044351 );
			AddCraft( typeof( WoodenThroneRC ),				1044291, 1044304,	52.6,  77.6,	typeof( Log ), 1044041, 17, 1044351 );
			AddCraft( typeof( ThroneRC ),					1044291, 1044305,	73.6,  98.6,	typeof( Log ), 1044041, 19, 1044351 );
			AddCraft( typeof( NightstandRC ),				1044291, 1044306,	42.1,  67.1,	typeof( Log ), 1044041, 17, 1044351 );
			AddCraft( typeof( WritingTableRC ),				1044291, 1022890,	63.1,  88.1,	typeof( Log ), 1044041, 17, 1044351 );
			AddCraft( typeof( YewWoodTableRC ),				1044291, 1044307,	63.1,  88.1,	typeof( Log ), 1044041, 23, 1044351 );
			AddCraft( typeof( LargeTableRC ),				1044291, 1044308,	84.2, 109.2,	typeof( Log ), 1044041, 27, 1044351 );
			AddCraft( typeof( ArcaneBookshelfSouthDeed ),				1044291, "półka arkanisty PD",	94.2, 119.2,	typeof( Log ), 1044041, 40, 1044351 );
			AddCraft( typeof( ArcaneBookshelfEastDeed ),				1044291, "półka arkanisty WS",	94.2, 119.2,	typeof( Log ), 1044041, 40, 1044351 );
			AddCraft( typeof( ElvenDresserSouthDeed ),				1044291, "komoda w stylu elfickim PD",	84.2, 100.2,	typeof( Log ), 1044041, 30, 1044351 );
			AddCraft( typeof( ElvenDresserEastDeed ),				1044291, "komoda w stylu elfickim WS",	84.2, 100.2,	typeof( Log ), 1044041, 30, 1044351 );
			AddCraft( typeof( ElvenWashBasinEastDeed ),				1044291, "umywalka w stylu elfickim WS",	74.2, 90.6,	typeof( Log ), 1044041, 30, 1044351 );
			AddCraft( typeof( ElvenWashBasinSouthDeed ),				1044291, "umywalka w stylu elfickim PD",	74.2, 90.6,	typeof( Log ), 1044041, 30, 1044351 );
			AddCraft( typeof( ElvenLoveseatEastDeed ),				1044291,"kozetka WS",	78.2, 98.6,	typeof( Log ), 1044041, 15, 1044351 );
			AddCraft( typeof( ElvenLoveseatSouthDeed ),				1044291, "kozetka WS",	78.2, 98.6,	typeof( Log ), 1044041, 15, 1044351 );
			AddCraft( typeof( TallElvenBedEastDeed ),				1044291,"wysokie łóżko WS",	77.7, 97.7,	typeof( Log ), 1044041, 45, 1044351 );
			AddCraft( typeof( TallElvenBedSouthDeed ),				1044291, "wysokie łóżko PD",	77.7, 97.7,	typeof( Log ), 1044041, 45, 1044351 );
			AddCraft( typeof( ElvenForgeDeed ),				1044291, "elfickie kowadło duszy",	94.2, 112.2,	typeof( IronIngot ), "sztaby żelaza", 100, 1044351 );
			AddCraft( typeof( StoneFireplaceSouthDeed ),				1044291, 1061849,	94.2, 112.2,	typeof( Granite ), "granit", 40, 1044351 );
			AddCraft( typeof( StoneFireplaceEastDeed ),				1044291, 1061848,	94.2, 112.2,	typeof( Granite ), "granit", 40, 1044351 );
			AddCraft(typeof(GrayBrickFireplaceSouthDeed), 1044291, 1061847, 94.2, 112.2, typeof(DullCopperGranite ), "granit matowej miedzi", 40, 1044351 );
			AddCraft(typeof(GrayBrickFireplaceEastDeed), 1044291, 1061846, 94.2, 112.2, typeof(DullCopperGranite ), "granit matowej miedzi", 40, 1044351 );
			AddCraft(typeof(SandstoneFireplaceSouthDeed ), 1044291, 1061845, 94.2, 112.2, typeof(GoldGranite ), "granit ze zlota", 40, 1044351 );
			AddCraft(typeof(SandstoneFireplaceEastDeed ), 1044291, 1061844, 94.2, 112.2, typeof(GoldGranite ), "granit ze zlota", 40, 1044351 );

			if( Core.SE )
			{
				index = AddCraft( typeof( ElegantLowTableRC ),	1044291, 1030265,	80.0, 105.0,	typeof( Log ), 1044041, 35, 1044351 );
				SetNeededExpansion( index, Expansion.SE );

				index = AddCraft( typeof( PlainLowTableRC ),		1044291, 1030266,	80.0, 105.0,	typeof( Log ), 1044041, 35, 1044351 );
				SetNeededExpansion( index, Expansion.SE );
			}

			// Containers
			AddCraft( typeof( WoodenBox ),					1044292, 1023709,	21.0,  46.0,	typeof( Log ), 1044041, 10, 1044351 );
			AddCraft( typeof( SmallCrate ),					1044292, 1044309,	10.0,  35.0,	typeof( Log ), 1044041, 8 , 1044351 );
			AddCraft( typeof( MediumCrate ),				1044292, 1044310,	31.0,  56.0,	typeof( Log ), 1044041, 15, 1044351 );
			AddCraft( typeof( LargeCrate ),					1044292, 1044311,	47.3,  72.3,	typeof( Log ), 1044041, 18, 1044351 );
			AddCraft( typeof( WoodenChest ),				1044292, 1023650,	73.6,  98.6,	typeof( Log ), 1044041, 20, 1044351 );
			AddCraft( typeof( EmptyBookcaseRC ),			1044292, 1022718,	31.5,  56.5,	typeof( Log ), 1044041, 25, 1044351 );
			AddCraft( typeof( FancyArmoireRC ),				1044292, 1044312,	84.2, 109.2,	typeof( Log ), 1044041, 35, 1044351 );
			AddCraft( typeof( ArmoireRC ),					1044292, 1022643,	84.2, 109.2,	typeof( Log ), 1044041, 35, 1044351 );	

			if( Core.SE )
			{
				index = AddCraft( typeof( PlainWoodenChest ),	1044292, 1030251, 90.0, 115.0,	typeof( Log ), 1044041, 30, 1044351 );
				SetNeededExpansion( index, Expansion.SE );

				index = AddCraft( typeof( OrnateWoodenChest ),	1044292, 1030253, 90.0, 115.0,	typeof( Log ), 1044041, 30, 1044351 );
				SetNeededExpansion( index, Expansion.SE );

				index = AddCraft( typeof( GildedWoodenChest ),	1044292, 1030255, 90.0, 115.0,	typeof( Log ), 1044041, 30, 1044351 );
				SetNeededExpansion( index, Expansion.SE );

				index = AddCraft( typeof( WoodenFootLocker ),	1044292, 1030257, 90.0, 115.0,	typeof( Log ), 1044041, 30, 1044351 );
				SetNeededExpansion( index, Expansion.SE );

				index = AddCraft( typeof( FinishedWoodenChest ),1044292, 1030259, 90.0, 115.0,	typeof( Log ), 1044041, 30, 1044351 );
				SetNeededExpansion( index, Expansion.SE );

				index = AddCraft( typeof( TallCabinetRC ),	1044292, 1030261, 90.0, 115.0,	typeof( Log ), 1044041, 35, 1044351 );
				SetNeededExpansion( index, Expansion.SE );

				index = AddCraft( typeof( ShortCabinetRC ),	1044292, 1030263, 90.0, 115.0,	typeof( Log ), 1044041, 35, 1044351 );
				SetNeededExpansion( index, Expansion.SE );

				index = AddCraft( typeof( RedArmoireRC ),	1044292, 1030328, 90.0, 115.0,	typeof( Log ), 1044041, 40, 1044351 );
				SetNeededExpansion( index, Expansion.SE );
				
				index = AddCraft( typeof( ElegantArmoireRC ),	1044292, 1030330, 90.0, 115.0,	typeof( Log ), 1044041, 40, 1044351 );
				SetNeededExpansion( index, Expansion.SE );
				
				index = AddCraft( typeof( MapleArmoireRC ),	1044292, 1030328, 90.0, 115.0,	typeof( Log ), 1044041, 40, 1044351 );
				SetNeededExpansion( index, Expansion.SE );
				
				index = AddCraft( typeof( CherryArmoireRC ),	1044292, 1030328, 90.0, 115.0,	typeof( Log ), 1044041, 40, 1044351 );
				SetNeededExpansion( index, Expansion.SE );
			}

			index = AddCraft( typeof( Keg ), 1044292, 1023711, 57.8, 82.8, typeof( BarrelStavesRC ), 1044288, 3, 1044253 );
			AddRes( index, typeof( BarrelHoops ), 1044289, 1, 1044253 );
			AddRes( index, typeof( BarrelLidRC ), 1044251, 1, 1044253 );

            index = AddCraft(typeof(WoodenBox), 1044292, 1023705, 25.0, 50.0, typeof(Log), 1044041, 5, 1044351);

			// Staves and Shields
			AddCraft( typeof( ShepherdsCrook ), 1044295, 1023713, 78.9, 103.9, typeof( Log ), 1044041, 7, 1044351 );
			AddCraft( typeof( QuarterStaff ), 1044295, "drewniana laska", 73.6, 98.6, typeof( Log ), 1044041, 6, 1044351 );
			AddCraft( typeof( GnarledStaff ), 1044295, 1025112, 78.9, 103.9, typeof( Log ), 1044041, 7, 1044351 );
			AddCraft( typeof( WildStaff ), 1044295, "laska pustelnika" , 78.9, 103.9, typeof( Log ), 1044041, 7, 1044351 );
			AddCraft( typeof( WoodenShield ), 1044295, 1027034, 52.6, 77.6, typeof( Log ), 1044041, 9, 1044351 );
			AddCraft( typeof( BlackStaff ), 1044295, "czarna laska" , 78.9, 103.9, typeof( Log ), 1044041, 7, 1044351 );

			if( !Core.AOS )	//Duplicate Entries to preserve ordering depending on era 
			{
				index = AddCraft( typeof( FishingPole ), 1044295, 1023519, 68.4, 93.4, typeof( Log ), 1044041, 5, 1044351 ); //This is in the categor of Other during AoS
				AddSkill( index, SkillName.Tailoring, 40.0, 45.0 );
				AddRes( index, typeof( Cloth ), 1044286, 5, 1044287 );
			}

			if( Core.SE )
			{
				index = AddCraft( typeof( Bokuto ), 1044295, 1030227, 70.0, 95.0, typeof( Log ), 1044041, 6, 1044351 );
				SetNeededExpansion( index, Expansion.SE );

				index = AddCraft( typeof( Fukiya ), 1044295, 1030229, 60.0, 85.0, typeof( Log ), 1044041, 6, 1044351 );
				SetNeededExpansion( index, Expansion.SE );

				index = AddCraft( typeof( Tetsubo ), 1044295, 1030225, 85.0, 110.0, typeof( Log ), 1044041, 8, 1044351 );
				AddSkill( index, SkillName.Tinkering, 40.0, 45.0 );
				AddRes( index, typeof( IronIngot ), 1044036, 5, 1044037 );
				SetNeededExpansion( index, Expansion.SE );
				
			}

			// Instruments
			index = AddCraft( typeof( LapHarp ), 1044293, 1023762, 63.1, 88.1, typeof( Log ), 1044041, 20, 1044351 );
			AddSkill( index, SkillName.Musicianship, 45.0, 50.0 );
			AddRes( index, typeof( Cloth ), 1044286, 10, 1044287 );

			index = AddCraft( typeof( Harp ), 1044293, 1023761, 78.9, 103.9, typeof( Log ), 1044041, 35, 1044351 );
			AddSkill( index, SkillName.Musicianship, 45.0, 50.0 );
			AddRes( index, typeof( Cloth ), 1044286, 15, 1044287 );
			
			index = AddCraft( typeof( Drums ), 1044293, 1023740, 57.8, 82.8, typeof( Log ), 1044041, 20, 1044351 );
			AddSkill( index, SkillName.Musicianship, 45.0, 50.0 );
			AddRes( index, typeof( Cloth ), 1044286, 10, 1044287 );
			
			index = AddCraft( typeof( Lute ), 1044293, 1023763, 68.4, 93.4, typeof( Log ), 1044041, 25, 1044351 );
			AddSkill( index, SkillName.Musicianship, 45.0, 50.0 );
			AddRes( index, typeof( Cloth ), 1044286, 10, 1044287 );
			
			index = AddCraft( typeof( Tambourine ), 1044293, 1023741, 57.8, 82.8, typeof( Log ), 1044041, 15, 1044351 );
			AddSkill( index, SkillName.Musicianship, 45.0, 50.0 );
			AddRes( index, typeof( Cloth ), 1044286, 10, 1044287 );

			index = AddCraft( typeof( TambourineTassel ), 1044293, 1044320, 57.8, 82.8, typeof( Log ), 1044041, 15, 1044351 );
			AddSkill( index, SkillName.Musicianship, 45.0, 50.0 );
			AddRes( index, typeof( Cloth ), 1044286, 15, 1044287 );	

			if( Core.SE )
			{
				index = AddCraft( typeof( BambooFlute ), 1044293, 1030247, 80.0, 105.0, typeof( Log ), 1044041, 15, 1044351 );
				AddSkill( index, SkillName.Musicianship, 45.0, 50.0 );
				SetNeededExpansion( index, Expansion.SE );
			}

			// Misc
			index = AddCraft( typeof( SmallBedSouthDeed ), 1044290, 1044321, 94.7, 113.1, typeof( Log ), 1044041, 100, 1044351 );
			AddSkill( index, SkillName.Tailoring, 75.0, 80.0 );
			AddRes( index, typeof( Cloth ), 1044286, 100, 1044287 );
			index = AddCraft( typeof( SmallBedEastDeed ), 1044290, 1044322, 94.7, 113.1, typeof( Log ), 1044041, 100, 1044351 );
			AddSkill( index, SkillName.Tailoring, 75.0, 80.0 );
			AddRes( index, typeof( Cloth ), 1044286, 100, 1044287 );
			index = AddCraft( typeof( LargeBedSouthDeed ), 1044290,1044323, 94.7, 113.1, typeof( Log ), 1044041, 150, 1044351 );
			AddSkill( index, SkillName.Tailoring, 75.0, 80.0 );
			AddRes( index, typeof( Cloth ), 1044286, 150, 1044287 );
			index = AddCraft( typeof( LargeBedEastDeed ), 1044290, 1044324, 94.7, 113.1, typeof( Log ), 1044041, 150, 1044351 );
			AddSkill( index, SkillName.Tailoring, 75.0, 80.0 );
			AddRes( index, typeof( Cloth ), 1044286, 150, 1044287 );
			AddCraft( typeof( DartBoardSouthDeed ), 1044290, 1044325, 15.7, 40.7, typeof( Log ), 1044041, 5, 1044351 );
			AddCraft( typeof( DartBoardEastDeed ), 1044290, 1044326, 15.7, 40.7, typeof( Log ), 1044041, 5, 1044351 );
			AddCraft( typeof( BallotBoxDeed ), 1044290, 1044327, 47.3, 72.3, typeof( Log ), 1044041, 5, 1044351 );
			index = AddCraft( typeof( PentagramDeed ), 1044290, 1044328, 100.0, 125.0, typeof( Log ), 1044041, 100, 1044351 );
			AddSkill( index, SkillName.Magery, 75.0, 80.0 );
			AddRes( index, typeof( IronIngot ), 1044036, 40, 1044037 );
			index = AddCraft( typeof( AbbatoirDeed ), 1044290, 1044329, 100.0, 125.0, typeof( Log ), 1044041, 100, 1044351 );
			AddSkill( index, SkillName.Magery, 50.0, 55.0 );
			AddRes( index, typeof( IronIngot ), 1044036, 40, 1044037 );

			if ( Core.AOS )
			{
				AddCraft( typeof( PlayerBBEast ), 1044290, 1062420, 85.0, 110.0, typeof( Log ), 1044041, 50, 1044351 );
				AddCraft( typeof( PlayerBBSouth ), 1044290, 1062421, 85.0, 110.0, typeof( Log ), 1044041, 50, 1044351 );
			}
            
			// Blacksmithy
			index = AddCraft( typeof( SmallForgeDeed ), 1044296, 1044330, 73.6, 98.6, typeof( Log ), 1044041, 5, 1044351 );
			AddSkill( index, SkillName.Blacksmith, 75.0, 80.0 );
			AddRes( index, typeof( IronIngot ), 1044036, 75, 1044037 );
			index = AddCraft( typeof( LargeForgeEastDeed ), 1044296, 1044331, 78.9, 103.9, typeof( Log ), 1044041, 5, 1044351 );
			AddSkill( index, SkillName.Blacksmith, 80.0, 85.0 );
			AddRes( index, typeof( IronIngot ), 1044036, 100, 1044037 );
			index = AddCraft( typeof( LargeForgeSouthDeed ), 1044296, 1044332, 78.9, 103.9, typeof( Log ), 1044041, 5, 1044351 );
			AddSkill( index, SkillName.Blacksmith, 80.0, 85.0 );
			AddRes( index, typeof( IronIngot ), 1044036, 100, 1044037 );
			index = AddCraft( typeof( AnvilEastDeed ), 1044296, 1044333, 73.6, 98.6, typeof( Log ), 1044041, 5, 1044351 );
			AddSkill( index, SkillName.Blacksmith, 75.0, 80.0 );
			AddRes( index, typeof( IronIngot ), 1044036, 150, 1044037 );
			index = AddCraft( typeof( AnvilSouthDeed ), 1044296, 1044334, 73.6, 98.6, typeof( Log ), 1044041, 5, 1044351 );
			AddSkill( index, SkillName.Blacksmith, 75.0, 80.0 );
			AddRes( index, typeof( IronIngot ), 1044036, 150, 1044037 );

			// Training
			index = AddCraft( typeof( TrainingDummyEastDeed ), 1044297, 1044335, 68.4, 93.4, typeof( Log ), 1044041, 55, 1044351 );
			AddSkill( index, SkillName.Tailoring, 50.0, 55.0 );
			AddRes( index, typeof( Cloth ), 1044286, 60, 1044287 );
			index = AddCraft( typeof( TrainingDummySouthDeed ), 1044297, 1044336, 68.4, 93.4, typeof( Log ), 1044041, 55, 1044351 );
			AddSkill( index, SkillName.Tailoring, 50.0, 55.0 );
			AddRes( index, typeof( Cloth ), 1044286, 60, 1044287 );
			index = AddCraft( typeof( PickpocketDipEastDeed ), 1044297, 1044337, 73.6, 98.6, typeof( Log ), 1044041, 65, 1044351 );
			AddSkill( index, SkillName.Tailoring, 50.0, 55.0 );
			AddRes( index, typeof( Cloth ), 1044286, 60, 1044287 );
			index = AddCraft( typeof( PickpocketDipSouthDeed ), 1044297, 1044338, 73.6, 98.6, typeof( Log ), 1044041, 65, 1044351 );
			AddSkill( index, SkillName.Tailoring, 50.0, 55.0 );
			AddRes( index, typeof( Cloth ), 1044286, 60, 1044287 );

			// Tailoring
			index = AddCraft( typeof( DressformRC ), 1044298, 1044339, 63.1, 88.1, typeof( Log ), 1044041, 25, 1044351 );
			AddSkill( index, SkillName.Tailoring, 65.0, 70.0 );
			AddRes( index, typeof( Cloth ), 1044286, 10, 1044287 );
			index = AddCraft( typeof( SpinningwheelEastDeed ), 1044298, 1044341, 73.6, 98.6, typeof( Log ), 1044041, 75, 1044351 );
			AddSkill( index, SkillName.Tailoring, 65.0, 70.0 );
			AddRes( index, typeof( Cloth ), 1044286, 25, 1044287 );
			index = AddCraft( typeof( SpinningwheelSouthDeed ), 1044298, 1044342, 73.6, 98.6, typeof( Log ), 1044041, 75, 1044351 );
			AddSkill( index, SkillName.Tailoring, 65.0, 70.0 );
			AddRes( index, typeof( Cloth ), 1044286, 25, 1044287 );
			index = AddCraft( typeof( LoomEastDeed ), 1044298, 1044343, 84.2, 109.2, typeof( Log ), 1044041, 85, 1044351 );
			AddSkill( index, SkillName.Tailoring, 65.0, 70.0 );
			AddRes( index, typeof( Cloth ), 1044286, 25, 1044287 );
			index = AddCraft( typeof( LoomSouthDeed ), 1044298, 1044344, 84.2, 109.2, typeof( Log ), 1044041, 85, 1044351 );
			AddSkill( index, SkillName.Tailoring, 65.0, 70.0 );
			AddRes( index, typeof( Cloth ), 1044286, 25, 1044287 );
			AddCraft( typeof( ElvenSpinningwheelEastDeed ),				1044298,"kołowrotek w stylu elfickim WS",	73.6, 98.6, typeof( Log ), 1044041, 75, 1044351 );
			AddSkill( index, SkillName.Tailoring, 65.0, 70.0 );
			AddCraft( typeof( ElvenSpinningwheelSouthDeed ),				1044298, "kołowrotek w stylu elfickim PD",	73.6, 98.6, typeof( Log ), 1044041, 75, 1044351 );
			AddSkill( index, SkillName.Tailoring, 65.0, 70.0 );

			// Cooking
			index = AddCraft( typeof( StoneOvenEastDeed ), 1044299, 1044345, 68.4, 93.4, typeof( Log ), 1044041, 85, 1044351 );
			AddSkill( index, SkillName.Tinkering, 50.0, 55.0 );
			AddRes( index, typeof( IronIngot ), 1044036, 125, 1044037 );
			index = AddCraft( typeof( StoneOvenSouthDeed ), 1044299, 1044346, 68.4, 93.4, typeof( Log ), 1044041, 85, 1044351 );
			AddSkill( index, SkillName.Tinkering, 50.0, 55.0 );
			AddRes( index, typeof( IronIngot ), 1044036, 125, 1044037 );
			index = AddCraft( typeof( FlourMillEastDeed ), 1044299, 1044347, 94.7, 119.7, typeof( Log ), 1044041, 100, 1044351 );
			AddSkill( index, SkillName.Tinkering, 50.0, 55.0 );
			AddRes( index, typeof( IronIngot ), 1044036, 50, 1044037 );
			index = AddCraft( typeof( FlourMillSouthDeed ), 1044299, 1044348, 94.7, 119.7, typeof( Log ), 1044041, 100, 1044351 );
			AddSkill( index, SkillName.Tinkering, 50.0, 55.0 );
			AddRes( index, typeof( IronIngot ), 1044036, 50, 1044037 );
			AddCraft( typeof( WaterTroughEastDeed ), 1044299, 1044349, 94.7, 119.7, typeof( Log ), 1044041, 150, 1044351 );
			AddCraft( typeof( WaterTroughSouthDeed ), 1044299, 1044350, 94.7, 119.7, typeof( Log ), 1044041, 150, 1044351 );
            AddCraft(typeof(BarrelOfWaterDeed), 1044299, 1025453, 90.7, 115.7, typeof(Log), 1044041, 100, 1044351);

// 20.05.11 :: Maupishon :: wylaczylem produkcje statkow
            // Produkcja statkow:
           // index = AddCraft(typeof(Deck), 1045036, 1045025, 60.0, 110.0, typeof(Log), 1044041, 500, 1044351);
            //AddRes(index, typeof(IronIngot), 1044036, 200, 1044037);
            //AddRes(index, typeof(ArcaneGem), 1032584, 5, 1044253);

            //index = AddCraft(typeof(Rudder), 1045036, 1045026, 60.0, 110.0, typeof(Log), 1044041, 100, 1044351);
            //AddRes(index, typeof(IronIngot), 1044036, 200, 1044037);
            //AddRes(index, typeof(ClockworkAssembly), 1073426, 5, 1044253);

           // index = AddCraft(typeof(Mast), 1045036, 1045027, 60.0, 110.0, typeof(Log), 1044041, 200, 1044351);
           // AddRes(index, typeof(BarrelHoops), 1044289, 10, 1044253);
            //AddRes(index, typeof(PowerCrystal), 1045035, 1, 1044253);

            //index = AddCraft(typeof(Oars), 1045036, 1045028, 60.0, 110.0, typeof(Log), 1044041, 100, 1044351);
            //AddRes(index, typeof(IronWire), 1026262, 2, 1044253);

            //index = AddCraft(typeof(Prow), 1045036, 1045029, 60.0, 110.0, typeof(Log), 1044041, 200, 1044351);
            //AddRes(index, typeof(IronIngot), 1044036, 300, 1044037);
            //AddRes(index, typeof(BarrelStavesRC), 1044288, 10, 1044253);
            //AddRes(index, typeof(Spyglass), 1025365, 1, 1044253);

            //index = AddCraft(typeof(Hatch), 1045036, 1045030, 60.0, 110.0, typeof(Log), 1044041, 200, 1044351);
            //AddRes(index, typeof(IronIngot), 1044036, 200, 1044037);
            //AddRes(index, typeof(Sextant), 1024183, 1, 1044253);
            //AddRes(index, typeof(Key), 1024112, 1, 1044253);

           // index = AddCraft(typeof(Sail), 1045036, 1045031, 60.0, 110.0, typeof(IronWire), 1026262, 1, 1044253);
            //AddRes(index, typeof(BoltOfCloth), 1047019, 10, 1044253);
           // AddRes(index, typeof(DarkYarn), 1023613, 20, 1044253);

            //index = AddCraft(typeof(Side), 1045036, 1045032, 60.0, 110.0, typeof(Log), 1044041, 200, 1044351);
            //AddRes(index, typeof(IronIngot), 1044036, 1000, 1044037);
            //AddRes(index, typeof(BarrelStavesRC), 1044288, 5, 1044253);
            //AddRes(index, typeof(Beeswax), 1025154, 10, 1044253);

           // index = AddCraft(typeof(BoatFront), 1045036, 1045033, 60.0, 100.0, typeof(Prow), 1045029, 1, 1044253);
           // AddRes(index, typeof(Hatch), 1045030, 1, 1044253);
            //AddRes(index, typeof(Sail), 1045031, 1, 1044253);
            //AddRes(index, typeof(Side), 1045032, 1, 1044253);

           // index = AddCraft(typeof(BoatBack), 1045036, 1045034, 60.0, 100.0, typeof(Deck), 1045025, 1, 1044253);
           // AddRes(index, typeof(Rudder), 1045026, 1, 1044253);
          //  AddRes(index, typeof(Mast), 1045027, 1, 1044253);
          //  AddRes(index, typeof(Oars), 1045028, 1, 1044253);

           // index = AddCraft(typeof(LargeBoatDeed), 1045036, 1041209, 60.0, 100.0, typeof(BoatBuildProject), 1045037, 1, 1044253);
          //  AddRes(index, typeof(BoatFront), 1045033, 1, 1044253);
          //  AddRes(index, typeof(BoatBack), 1045034, 1, 1044253);            

			MarkOption = true;
			Repair = Core.AOS;
            RecycleHelper = new Rechop();

            // zombie - 17-06-2012 
            CanEnhance = Core.AOS;

            SetSubRes( typeof( Log ), 1072643 );

            // dodanie surowcow do gumpa
            AddSubRes( typeof( Log ), 1072643, 00.0, 505821, 1072653 );
            AddSubRes( typeof( OakLog ), 1072644, 65.0, 505822, 1072653 );
            AddSubRes( typeof( AshLog ), 1072645, 75.0, 505823, 1072653 );
            AddSubRes( typeof( YewLog ), 1072646, 85.0, 505824, 1072653 );
            AddSubRes( typeof( HeartwoodLog ), 1072648, 95.0, 505826, 1072653 );
            AddSubRes( typeof( BloodwoodLog ), 1072647, 99.0, 505825, 1072653 );
            AddSubRes( typeof( FrostwoodLog ), 1072649, 99.0, 505827, 1072653 );
            /*
            1072643	ZWYKLE DREWNO (~1_NUMBER~)
            1072644	ZYWICZNE DREWNO (~1_NUMBER~)
            1072645	PUSTE DREWNO (~1_NUMBER~)
            1072646	SKAMIENIALE DREWNO (~1_NUMBER~)
            1072647	OPALONE DREWNO (~1_NUMBER~)
            1072648	GIETKIE DREWNO (~1_NUMBER~)
            1072649	ZMARZNIETE DREWNO (~1_NUMBER~)
            505821	Zwykle drewno
            505822	Zywiczne drewno
            505823	Puste drewno
            505824	Skamieniale drewno
            505825	Opalone drewno
            505826	Gietkie drewno
            505827	Zmarzniete drewno
            */
        }

        // zombie - 17-06-2012 - nadawanie itemom koloru drewna
        public override bool RetainsColorFrom(CraftItem item, Type type)
        {
            return true;
        }
        // zombie
	}
}