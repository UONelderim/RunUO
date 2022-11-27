using System; 
using Server.Items; 
using Server.Mobiles; 

namespace Server.Engines.Craft 
{ 
	public class DefMasonry : CraftSystem 
	{ 
		public override SkillName MainSkill 
		{ 
			get{ return SkillName.Carpentry; } 
		} 

		public override int GumpTitleNumber 
		{ 
			get{ return 1044500; } // <CENTER>MASONRY MENU</CENTER> 
		} 

		private static CraftSystem m_CraftSystem; 

		public static CraftSystem CraftSystem 
		{ 
			get 
			{ 
				if ( m_CraftSystem == null ) 
					m_CraftSystem = new DefMasonry(); 

				return m_CraftSystem; 
			} 
		} 

		public override double GetChanceAtMin( CraftItem item ) 
		{ 
			return 0.0; // 0% 
		} 

		private DefMasonry() : base( 1, 1, 1.25 )// base( 1, 2, 1.7 ) 
		{ 
		} 

		public override bool RetainsColorFrom( CraftItem item, Type type )
		{
			return true;
		}

		public override int CanCraft( Mobile from, BaseTool tool, Type itemType )
		{
			if( tool == null || tool.Deleted || tool.UsesRemaining < 0 )
				return 1044038; // You have worn out your tool!
			else if ( !BaseTool.CheckTool( tool, from ) )
				return 1048146; // If you have a tool equipped, you must use that tool.
			else if ( !(from is PlayerMobile && ((PlayerMobile)from).Masonry && from.Skills[SkillName.Carpentry].Base >= 100.0) )
				return 1044633; // You havent learned stonecraft.
			else if ( !BaseTool.CheckAccessible( tool, from ) )
				return 1044263; // The tool must be on your person to use.

			return 0;
		} 

		public override void PlayCraftEffect( Mobile from ) 
		{ 
			// no effects
			//if ( from.Body.Type == BodyType.Human && !from.Mounted ) 
			//	from.Animate( 9, 5, 1, true, false, 0 ); 
			//new InternalTimer( from ).Start(); 
		} 

		// Delay to synchronize the sound with the hit on the anvil 
		private class InternalTimer : Timer 
		{ 
			private Mobile m_From; 

			public InternalTimer( Mobile from ) : base( TimeSpan.FromSeconds( 0.7 ) ) 
			{ 
				m_From = from; 
			} 

			protected override void OnTick() 
			{ 
				m_From.PlaySound( 0x23D ); 
			} 
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
			// Decorations
			AddCraft( typeof( Vase ), 1044501, 1022888, 52.5, 102.5, typeof( Granite ), 1044514, 1, 1044513 );
			AddCraft( typeof( LargeVase ), 1044501, 1022887, 52.5, 102.5, typeof( Granite ), 1044514, 3, 1044513 );
			AddCraft( typeof( KamiennaWaza ), 1044501, "kamienna waza", 60.0, 120.0, typeof( Granite ), 1044514, 7, 1044513 );
			AddCraft( typeof( Nagrobek ), 1044501, "nagrobek", 60.0, 120.0, typeof( Granite ), 1044514, 7, 1044513 );

			
			if( Core.SE )
			{
				int index = AddCraft( typeof( SmallUrn ), 1044501, 1029244, 72.0, 122.0, typeof( Granite ), 1044514, 3, 1044513 );
				SetNeededExpansion( index, Expansion.SE );

				index = AddCraft( typeof( SmallTowerSculpture ), 1044501, 1029242, 72.0, 122.0, typeof( Granite ), 1044514, 3, 1044513 );
				SetNeededExpansion( index, Expansion.SE );
			}
			
			// Furniture
			AddCraft( typeof( StoneChair ), 1044502, 1024635, 55.0, 105.0, typeof( Granite ), 1044514, 4, 1044513 );
			AddCraft( typeof( MediumStoneTableEastDeed ), 1044502, 1044508, 55.0, 105.0, typeof( Granite ), 1044514, 6, 1044513 );
			AddCraft( typeof( MediumStoneTableSouthDeed ), 1044502, 1044509, 55.0, 105.0, typeof( Granite ), 1044514, 6, 1044513 );
			AddCraft( typeof( LargeStoneTableEastDeed ), 1044502, 1044511, 55.0, 105.0, typeof( Granite ), 1044514, 9, 1044513 );
			AddCraft( typeof( LargeStoneTableSouthDeed ), 1044502, 1044512, 55.0, 105.0, typeof( Granite ), 1044514, 9, 1044513 );
			
			//Oltarze
			AddCraft( typeof( OltarzKoncaF ), "Oltarze", "Oltarz Konca", 80.0, 130.0, typeof( Granite ), 1044514, 15, 1044513 );
			AddCraft( typeof( OltarzMatkiF ), "Oltarze", "Oltarz Matki", 80.0, 130.0, typeof( Granite ), 1044514, 15, 1044513 );
		    AddCraft( typeof( OltarzPoczatkuF ), "Oltarze", "Oltarz Poczltku", 80.0, 130.0, typeof( Granite ), 1044514, 15, 1044513 );

			//Pomniki
			AddCraft( typeof( PomnikOrlaS ), "Pomniki", "Pomnik Orla (Poludnie)", 85.0, 135.0, typeof( Granite ), 1044514, 25, 1044513 );
			AddCraft( typeof( PomnikOrlaE ), "Pomniki", "Pomnik Orla (Wschod)", 85.0, 135.0, typeof( Granite ), 1044514, 25, 1044513 );
			AddCraft( typeof( PomnikFeniksaS ), "Pomniki", "Pomnik Feniksa (Poludnie)", 90.0, 140.0, typeof( Granite ), 1044514, 30, 1044513 );
			AddCraft( typeof( PomnikFeniksaE ), "Pomniki", "Pomnik Feniksa (Wschod)", 90.0, 140.0, typeof( Granite ), 1044514, 30, 1044513 );
			AddCraft( typeof( PomnikDuchaS ), "Pomniki", "Pomnik Ducha (Poludnie)", 90.0, 140.0, typeof( Granite ), 1044514, 30, 1044513 );
			AddCraft( typeof( PomnikDuchaE ), "Pomniki", "Pomnik Ducha (Wschod)", 90.0, 140.0, typeof( Granite ), 1044514, 30, 1044513 );
			AddCraft( typeof( PomnikOrdyjskaS ), "Pomniki", "Pomnik Ordyjski (Poludnie)", 90.0, 140.0, typeof( Granite ), 1044514, 30, 1044513 );
			AddCraft( typeof( PomnikOrdyjskaE ), "Pomniki", "Pomnik Ordyjski (Wschod)", 90.0, 140.0, typeof( Granite ), 1044514, 30, 1044513 );
			AddCraft( typeof( PomnikLahlithS ), "Pomniki", "Pomnik Lahlith (Poludnie)", 90.0, 140.0, typeof( Granite ), 1044514, 30, 1044513 );
			AddCraft( typeof( PomnikLahlithE ), "Pomniki", "Pomnik Lahlith (Wschod)", 90.0, 140.0, typeof( Granite ), 1044514, 30, 1044513 );
			AddCraft( typeof( PomnikLowcyS ), "Pomniki", "Pomnik lowcy (Poludnie)", 90.0, 140.0, typeof( Granite ), 1044514, 30, 1044513 );
			AddCraft( typeof( PomnikLowcyE ), "Pomniki", "Pomnik lowcy (Wschod)", 90.0, 140.0, typeof( Granite ), 1044514, 30, 1044513 );
			AddCraft( typeof( PomnikElfaS ), "Pomniki", "Pomnik Elfa (Poludnie)", 90.0, 140.0, typeof( Granite ), 1044514, 30, 1044513 );
			AddCraft( typeof( PomnikElfaE ), "Pomniki", "Pomnik Elfa (Wschod)", 90.0, 140.0, typeof( Granite ), 1044514, 30, 1044513 );
			AddCraft( typeof( PomnikGargulcaF ), "Pomniki", "Pomnik Gargulca", 90.0, 140.0, typeof( Granite ), 1044514, 30, 1044513 );
			AddCraft( typeof( PomnikSmokaS ), "Pomniki", "Pomnik Smoka (Poludnie)", 90.0, 140.0, typeof( Granite ), 1044514, 30, 1044513 );
			AddCraft( typeof( PomnikSmokaE ), "Pomniki", "Pomnik Smoka (Wschod)", 90.0, 140.0, typeof( Granite ), 1044514, 30, 1044513 );
			AddCraft( typeof( PomnikGryfaS ), "Pomniki", "Pomnik Gryfa (Poludnie)", 90.0, 140.0, typeof( Granite ), 1044514, 30, 1044513 );
			AddCraft( typeof( PomnikGryfaE ), "Pomniki", "Pomnik Gryfa (Wschod)", 90.0, 140.0, typeof( Granite ), 1044514, 30, 1044513 );
			AddCraft( typeof( PomnikKaplankiF ), "Pomniki", "Pomnik Kaplanki", 90.0, 140.0, typeof( Granite ), 1044514, 30, 1044513 );
			AddCraft( typeof( PomnikJezdzcaS ), "Pomniki", "Pomnik Jezdzca (Poludnie)", 90.0, 140.0, typeof( Granite ), 1044514, 30, 1044513 );
			AddCraft( typeof( PomnikJezdzcaE ), "Pomniki", "Pomnik Jezdzca (Wschod)", 90.0, 140.0, typeof( Granite ), 1044514, 30, 1044513 );
			AddCraft( typeof( PomnikPanaKoncaS ), "Pomniki", "Pomnik Pana Konca (Poludnie)", 90.0, 140.0, typeof( Granite ), 1044514, 30, 1044513 );
			AddCraft( typeof( PomnikPanaKoncaE ), "Pomniki", "Pomnik Pana Konca (Wschod)", 90.0, 140.0, typeof( Granite ), 1044514, 30, 1044513 );
			AddCraft( typeof( PomnikLotheS ), "Pomniki", "Pomnik Lothe (Poludnie)", 90.0, 140.0, typeof( Granite ), 1044514, 30, 1044513 );
			AddCraft( typeof( PomnikLotheE ), "Pomniki", "Pomnik Lothe (Wschod)", 90.0, 140.0, typeof( Granite ), 1044514, 30, 1044513 );
			AddCraft( typeof( PomnikMagaS ), "Pomniki", "Pomnik Maga (Poludnie)", 90.0, 140.0, typeof( Granite ), 1044514, 30, 1044513 );
			AddCraft( typeof( PomnikMagaE ), "Pomniki", "Pomnik Maga (Wschod)", 90.0, 140.0, typeof( Granite ), 1044514, 30, 1044513 );
			AddCraft( typeof( PomnikOrbaS ), "Pomniki", "Pomnik Orba (Poludnie)", 90.0, 140.0, typeof( Granite ), 1044514, 30, 1044513 );
			AddCraft( typeof( PomnikOrbaE ), "Pomniki", "Pomnik Orba (Wschod)", 90.0, 140.0, typeof( Granite ), 1044514, 30, 1044513 );
			AddCraft( typeof( PomnikSfinksaS ), "Pomniki", "Pomnik Sfinksa (Poludnie)", 90.0, 140.0, typeof( Granite ), 1044514, 30, 1044513 );
			AddCraft( typeof( PomnikSfinksaE ), "Pomniki", "Pomnik Sfinksa (Wschod)", 90.0, 140.0, typeof( Granite ), 1044514, 30, 1044513 );
			AddCraft( typeof( PomnikStraznikaS ), "Pomniki", "Pomnik Straznika (Poludnie)", 90.0, 140.0, typeof( Granite ), 1044514, 30, 1044513 );
			AddCraft( typeof( PomnikStraznikaE ), "Pomniki", "Pomnik Straznika (Wschod)", 90.0, 140.0, typeof( Granite ), 1044514, 30, 1044513 );
            AddCraft( typeof( Piedestal ), "Pomniki", "Granitowy piedestal", 90.0, 140.0, typeof(Granite), 1044514, 30, 1044513) ;

            // Statues
            AddCraft( typeof( StatueSouth ), 1044503, 1044505, 50.0, 110.0, typeof( Granite ), 1044514, 3, 1044513 );
			AddCraft( typeof( GargulecStatua ), 1044503, "statuetka gargulca", 60.0, 120.0, typeof( Granite ), 1044514, 7, 1044513 );
			AddCraft( typeof( WojownikStatua ), 1044503, "statuetka wojownika", 60.0, 120.0, typeof( Granite ), 1044514, 7, 1044513 );
			AddCraft( typeof( StatueNorth ), 1044503, 1044506, 50.0, 110.0, typeof( Granite ), 1044514, 3, 1044513 );
			AddCraft( typeof( StatueEast ), 1044503, 1044507, 50.0, 110.0, typeof( Granite ), 1044514, 3, 1044513 );
			AddCraft( typeof( StatuePegasus ), 1044503, 1044510, 60.0, 120.0, typeof( Granite ), 1044514, 4, 1044513 );
			
			AddCraft( typeof( StatuetkaDemonaS ), 1044503, "Statuetka Demona (Poludnie)", 70.0, 120.0, typeof( Granite ), 1044514, 3, 1044513 );
			AddCraft( typeof( StatuetkaDemonaE ), 1044503, "Statuetka Demona (Wschod)", 70.0, 120.0, typeof( Granite ), 1044514, 3, 1044513 );
			AddCraft( typeof( StatuetkaDuchaS ), 1044503, "Statuetka Ducha (Poludnie)", 70.0, 120.0, typeof( Granite ), 1044514, 3, 1044513 );
			AddCraft( typeof( StatuetkaDuchaE ), 1044503, "Statuetka Ducha (Wschod)", 70.0, 120.0, typeof( Granite ), 1044514, 3, 1044513 );
			AddCraft( typeof( StatuetkaOrdyjskaS ), 1044503, "Statuetka Ordyjska (Poludnie)", 70.0, 120.0, typeof( Granite ), 1044514, 3, 1044513 );
			AddCraft( typeof( StatuetkaOrdyjskaE ), 1044503, "Statuetka Ordyjska (Wschod)", 70.0, 120.0, typeof( Granite ), 1044514, 3, 1044513 );
			AddCraft( typeof( StatuetkaLahlithS ), 1044503, "Statuetka Lahlith (Poludnie)", 70.0, 120.0, typeof( Granite ), 1044514, 3, 1044513 );
			AddCraft( typeof( StatuetkaLahlithE ), 1044503, "Statuetka Lahlith (Wschod)", 70.0, 120.0, typeof( Granite ), 1044514, 3, 1044513 );
			AddCraft( typeof( StatuetkaLowcyS ), 1044503, "Statuetka lowcy (Poludnie)", 70.0, 120.0, typeof( Granite ), 1044514, 3, 1044513 );
			AddCraft( typeof( StatuetkaLowcyE ), 1044503, "Statuetka lowcy (Wschod)", 70.0, 120.0, typeof( Granite ), 1044514, 3, 1044513 );
			AddCraft( typeof( StatuetkaElfaS ), 1044503, "Statuetka Elfa (Poludnie)", 70.0, 120.0, typeof( Granite ), 1044514, 3, 1044513 );
			AddCraft( typeof( StatuetkaElfaE ), 1044503, "Statuetka Elfa (Wschod)", 70.0, 120.0, typeof( Granite ), 1044514, 3, 1044513 );
			AddCraft( typeof( StatuetkaGargulcaF ), 1044503, "Statuetka Gargulca", 70.0, 120.0, typeof( Granite ), 1044514, 3, 1044513 );
			AddCraft( typeof( StatuetkaSmokaS ), 1044503, "Statuetka Smoka (Poludnie)", 70.0, 120.0, typeof( Granite ), 1044514, 3, 1044513 );
			AddCraft( typeof( StatuetkaSmokaE ), 1044503, "Statuetka Smoka (Wschod)", 70.0, 120.0, typeof( Granite ), 1044514, 3, 1044513 );
			AddCraft( typeof( StatuetkaGryfaS ), 1044503, "Statuetka Gryfa (Poludnie)", 70.0, 120.0, typeof( Granite ), 1044514, 3, 1044513 );
			AddCraft( typeof( StatuetkaGryfaE ), 1044503, "Statuetka Gryfa (Wschod)", 70.0, 120.0, typeof( Granite ), 1044514, 3, 1044513 );
			AddCraft( typeof( StatuetkaKaplankiF ), 1044503, "Statuetka Kaplanki", 70.0, 120.0, typeof( Granite ), 1044514, 3, 1044513 );
			AddCraft( typeof( StatuetkaJezdzcaS ), 1044503, "Statuetka Jezdzca (Poludnie)", 70.0, 120.0, typeof( Granite ), 1044514, 3, 1044513 );
			AddCraft( typeof( StatuetkaJezdzcaE ), 1044503, "Statuetka Jezdzca (Wschod)", 70.0, 120.0, typeof( Granite ), 1044514, 3, 1044513 );
			AddCraft( typeof( StatuetkaKrasnoludaS ), 1044503, "Statuetka Krasnoluda (Poludnie)", 70.0, 120.0, typeof( Granite ), 1044514, 3, 1044513 );
			AddCraft( typeof( StatuetkaKrasnoludaE ), 1044503, "Statuetka Krasnoluda (Wschod)", 70.0, 120.0, typeof( Granite ), 1044514, 3, 1044513 );
			AddCraft( typeof( StatuetkaBerserkeraS ), 1044503, "Statuetka Berserkera (Poludnie)", 70.0, 120.0, typeof( Granite ), 1044514, 3, 1044513 );
			AddCraft( typeof( StatuetkaBerserkeraE ), 1044503, "Statuetka Berserkera (Wschod)", 70.0, 120.0, typeof( Granite ), 1044514, 3, 1044513 );
			AddCraft( typeof( StatuetkaLotheS ), 1044503, "Statuetka Lothe (Poludnie)", 70.0, 120.0, typeof( Granite ), 1044514, 3, 1044513 );
			AddCraft( typeof( StatuetkaLotheE ), 1044503, "Statuetka Lothe (Wschod)", 70.0, 120.0, typeof( Granite ), 1044514, 3, 1044513 );
			AddCraft( typeof( StatuetkaPanaF ), 1044503, "Statuetka Pana", 70.0, 120.0, typeof( Granite ), 1044514, 3, 1044513 );
			AddCraft( typeof( StatuetkaStraznikaF ), 1044503, "Statuetka Straznika (Wschod)", 70.0, 120.0, typeof( Granite ), 1044514, 3, 1044513 );
			AddCraft( typeof( StatuetkaSmierciS ), 1044503, "Statuetka Smierci (Poludnie)", 70.0, 120.0, typeof( Granite ), 1044514, 3, 1044513 );
			AddCraft( typeof( StatuetkaSmierciE ), 1044503, "Statuetka Smierci (Wschod)", 70.0, 120.0, typeof( Granite ), 1044514, 3, 1044513 );
			AddCraft( typeof( StatuetkaDuszka ), 1044503, "Statuetka Duszka", 70.0, 120.0, typeof( Granite ), 1044514, 4, 1044513 );
			AddCraft( typeof( StatuetkaDuszkaF ), 1044503, "Statuetka Duszka", 70.0, 120.0, typeof( Granite ), 1044514, 4, 1044513 );
			AddCraft( typeof( StatuetkaDuszkaaF ), 1044503, "Statuetka Duszka", 70.0, 120.0, typeof( Granite ), 1044514, 4, 1044513 );
			AddCraft( typeof( StatuetkaDuszkafF ), 1044503, "Statuetka Duszka", 70.0, 120.0, typeof( Granite ), 1044514, 4, 1044513 );

//Wazy

AddCraft( typeof( WazaMalaF ), "Wazy", "Mala Waza", 70.0, 120.0, typeof( Granite ), 1044514, 3, 1044513 );
AddCraft( typeof( WazaMala1F ), "Wazy", "Mala Waza", 70.0, 120.0, typeof( Granite ), 1044514, 3, 1044513 );
AddCraft( typeof( WazaMala2F ), "Wazy", "Mala Waza", 70.0, 120.0, typeof( Granite ), 1044514, 3, 1044513 );
AddCraft( typeof( WazaMala3F ), "Wazy", "Mala Waza", 70.0, 120.0, typeof( Granite ), 1044514, 3, 1044513 );
AddCraft( typeof( WazaMala4F ), "Wazy", "Mala Waza", 70.0, 120.0, typeof( Granite ), 1044514, 3, 1044513 );
AddCraft( typeof( WazaMala5F ), "Wazy", "Mala Waza", 70.0, 120.0, typeof( Granite ), 1044514, 3, 1044513 );

AddCraft( typeof( Waza1F ), "Wazy", "Waza", 72.6, 122.6, typeof( Granite ), 1044514, 4, 1044513 );
AddCraft( typeof( Waza2F ), "Wazy", "Waza", 72.6, 122.6, typeof( Granite ), 1044514, 4, 1044513 );
AddCraft( typeof( Waza3F ), "Wazy", "Waza", 72.6, 122.6, typeof( Granite ), 1044514, 4, 1044513 );
AddCraft( typeof( Waza4F ), "Wazy", "Waza", 72.6, 122.6, typeof( Granite ), 1044514, 4, 1044513 );
AddCraft( typeof( Waza5F ), "Wazy", "Waza", 72.6, 122.6, typeof( Granite ), 1044514, 4, 1044513 );

AddCraft( typeof( WazaDuza1F ), "Wazy", "Dula Waza", 75.0, 125.0, typeof( Granite ), 1044514, 6, 1044513 );
AddCraft( typeof( WazaDuza2F ), "Wazy", "Dula Waza", 75.0, 125.0, typeof( Granite ), 1044514, 6, 1044513 );
AddCraft( typeof( WazaDuza3F ), "Wazy", "Dula Waza", 75.0, 125.0, typeof( Granite ), 1044514, 6, 1044513 );
AddCraft( typeof( WazaDuza4F ), "Wazy", "Dula Waza", 75.0, 125.0, typeof( Granite ), 1044514, 6, 1044513 );
AddCraft( typeof( WazaDuza5F ), "Wazy", "Dula Waza", 75.0, 125.0, typeof( Granite ), 1044514, 6, 1044513 );
AddCraft( typeof( WazaDuza6F ), "Wazy", "Dula Waza", 75.0, 125.0, typeof( Granite ), 1044514, 6, 1044513 );

AddCraft( typeof( WazaOgromna1F ), "Wazy", "B.Dula Waza", 80.0, 130.0, typeof( Granite ), 1044514, 10, 1044513 );
AddCraft( typeof( WazaOgromna2F ), "Wazy", "B.Dula Waza", 80.0, 130.0, typeof( Granite ), 1044514, 10, 1044513 );
AddCraft( typeof( WazaOgromna3F ), "Wazy", "B.Dula Waza", 80.0, 130.0, typeof( Granite ), 1044514, 10, 1044513 );
AddCraft( typeof( WazaOgromna4F ), "Wazy", "B.Dula Waza", 80.0, 130.0, typeof( Granite ), 1044514, 10, 1044513 );
AddCraft( typeof( WazaOgromna5F ), "Wazy", "B.Dula Waza", 80.0, 130.0, typeof( Granite ), 1044514, 10, 1044513 );
AddCraft( typeof( WazaOgromna6F ), "Wazy", "B.Dula Waza", 80.0, 130.0, typeof( Granite ), 1044514, 10, 1044513 );


			SetSubRes( typeof( Granite ), 1044525 );

			AddSubRes( typeof( Granite ),			1044525, 00.0, 1044514, 1044526 );
			AddSubRes( typeof( DullCopperGranite ),	1044023, 65.0, 1044514, 1044527 );
			AddSubRes( typeof( ShadowIronGranite ),	1044024, 70.0, 1044514, 1044527 );
			AddSubRes( typeof( CopperGranite ),		1044025, 75.0, 1044514, 1044527 );
			AddSubRes( typeof( BronzeGranite ),		1044026, 80.0, 1044514, 1044527 );
			AddSubRes( typeof( GoldGranite ),		1044027, 85.0, 1044514, 1044527 );
			AddSubRes( typeof( AgapiteGranite ),	1044028, 90.0, 1044514, 1044527 );
			AddSubRes( typeof( VeriteGranite ),		1044029, 95.0, 1044514, 1044527 );
			AddSubRes( typeof( ValoriteGranite ),	1044030, 99.0, 1044514, 1044527 );
		}
	}
}