using System;
using Server.Items;
using Server.Items.Crops;

namespace Server.Engines.Craft
{
    public class DefBowFletching : CraftSystem
    {
        public override SkillName MainSkill
        {
            get { return SkillName.Fletching; }
        }

        public override int GumpTitleNumber
        {
            get { return 1044006; } // <CENTER>BOWCRAFT AND FLETCHING MENU</CENTER>
        }

        private static CraftSystem m_CraftSystem;

        public static CraftSystem CraftSystem
        {
            get
            {
                if ( m_CraftSystem == null )
                    m_CraftSystem = new DefBowFletching();

                return m_CraftSystem;
            }
        }

        public override double GetChanceAtMin( CraftItem item )
        {
            return 0.5; // 50%
        }

        private DefBowFletching()
            : base( 1, 1, 1.25 )// base( 1, 2, 1.7 )
        {
        }

        public override int CanCraft( Mobile from, BaseTool tool, Type itemType )
        {
            if ( tool == null || tool.Deleted || tool.UsesRemaining < 0 )
                return 1044038; // You have worn out your tool!
            else if (!BaseTool.CheckTool(tool, from))
                return 1048146; // If you have a tool equipped, you must use that tool.
            else if ( !BaseTool.CheckAccessible( tool, from ) )
                return 1044263; // The tool must be on your person to use.

            return 0;
        }

        public override void PlayCraftEffect( Mobile from )
        {
            // no animation
            //if ( from.Body.Type == BodyType.Human && !from.Mounted )
            //	from.Animate( 33, 5, 1, true, false, 0 );

            from.PlaySound( 0x55 );
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

        public override CraftECA ECA { get { return CraftECA.FiftyPercentChanceMinusTenPercent; } }

        public override void InitCraftList()
        {
            int index = -1;

            // Materials
            AddCraft( typeof( Kindling ), 1044457, 1023553, 0.0, 00.0, typeof( Log ), 1044041, 1, 1044351 );

            index = AddCraft( typeof( Shaft ), 1044457, 1027124, 0.0, 40.0, typeof( Log ), 1044041, 1, 1044351 );
            SetUseAllRes( index, true );

            // 1032732	Cieciwy
            // 1032733	Skorzana cieciwa
            // 1032734	Konopna cieciwa
            // 1032735	Jedwabna cieciwa
            // 1032736	Jelitowa cieciwa

            // Bowstrings
            index = AddCraft( typeof( BowstringLeather ),   1032732, 1032733, 0.0, 40.0, typeof(Leather),       1044462, 1, 1044463 );
            SetUseAllRes( index, true );
            index = AddCraft( typeof( BowstringGut ),       1032732, 1032736, 0.0, 40.0, typeof(Gut),           1032618, 1, 1044463 );
            SetUseAllRes( index, true );
            index = AddCraft( typeof( BowstringCannabis ),  1032732, 1032734, 0.0, 40.0, typeof(CannabisFiber), 1032616, 1, 1044463 );
            SetUseAllRes( index, true );
            index = AddCraft( typeof( BowstringSilk ),      1032732, 1032735, 0.0, 40.0, typeof(SilkFiber),     1032617, 1, 1044463 );
            SetUseAllRes( index, true );
            
            // Ammunition
            index = AddCraft( typeof( Arrow ), 1044565, 1023903, 0.0, 40.0, typeof( Shaft ), 1044560, 1, 1044561 );
            AddRes( index, typeof( Feather ), 1044562, 1, 1044563 );
            SetUseAllRes( index, true );

            index = AddCraft( typeof( Bolt ), 1044565, 1027163, 0.0, 40.0, typeof( Shaft ), 1044560, 1, 1044561 );
            AddRes( index, typeof( Feather ), 1044562, 1, 1044563 );
            SetUseAllRes( index, true );

            if ( Core.SE )
            {
                index = AddCraft( typeof( FukiyaDarts ), 1044565, 1030246, 50.0, 90.0, typeof( Log ), 1044041, 1, 1044351 );
                SetUseAllRes( index, true );
                SetNeededExpansion( index, Expansion.SE );
            }

            // 1032732	Cieciwy
            // 1032594	Brakuje ci odpowiednich cieciw do zrobienia tego.
            
            // Weapons
            index = AddCraft( typeof( Bow ), 1044566, 1025042, 30.0, 70.0, typeof( Log ), 1044041, 8, 1044351 );
            AddRes( index, typeof(BowstringLeather), 1032732, 1, 1032594 );

            index = AddCraft(typeof(CompositeBow), 1044566, 1029922, 60.0, 102.0, typeof(Log), 1044041, 9, 1044351);
            AddRes(index, typeof(BowstringLeather), 1032732, 1, 1032594);

            index = AddCraft(typeof(Yumi), 1044566, 1030224, 80.0, 105.0, typeof(Log), 1044041, 10, 1044351);
            AddRes(index, typeof(BowstringLeather), 1032732, 1, 1032594);

            index = AddCraft( typeof( Crossbow ), 1044566, 1023919, 40.0, 80.0, typeof( Log ), 1044041, 8, 1044351 );
            AddRes( index, typeof(BowstringLeather), 1032732, 1, 1032594 );

            index = AddCraft(typeof(HeavyCrossbow), 1044566, 1025117, 70.0, 102.0, typeof(Log), 1044041, 9, 1044351);
            AddRes(index, typeof(BowstringLeather), 1032732, 1, 1032594);

            index = AddCraft(typeof(RepeatingCrossbow), 1044566, 1029923, 80.0, 110.0, typeof(Log), 1044041, 10, 1044351);
            AddRes(index, typeof(BowstringLeather), 1032732, 1, 1032594);
			
            index = AddCraft(typeof(MagicalShortbow), 1044566, "Krótki Łuk w stylu Wschodnim", 85.0, 115.0, typeof(Log), 1044041, 15, 1044351);
            AddRes(index, typeof(BowstringLeather), 1032732, 1, 1032594);
			
            index = AddCraft(typeof(ElvenCompositeLongbow), 1044566, "Długi łuk w stylu wschodnim", 95.0, 125.0, typeof(Log), 1044041, 20, 1044351);
            AddRes(index, typeof(BowstringLeather), 1032732, 1, 1032594);
			
            
            // Rozne
            // 13.07.2012 :: zombie :: dodanie kolczanow
            if( Core.ML )
			{
                // Zwykly kolczan dostepny do craftu dla kazdego
				index = AddCraft( typeof( ElvenQuiver ), 1015283, 1032657, 65.0, 115.0, typeof( Leather ), 1044462, 28, 1044463 );
				//AddRecipe( index, 501 );
				SetNeededExpansion( index, Expansion.ML );

                // Customowe kołczany
                index = AddCraft( typeof( kolczanwstyluzachodnim ), 1015283, "kołczan w stylu zachodnim", 65.0, 115.0, typeof( BarbedLeather ), "zielone skóry", 20, 1044463 );
                AddRes(index, typeof(DragonsBlood), "krew smoka", 1, "masz za malo krwii smoka");

                index = AddCraft( typeof( kolczanwstylupolnocnym ), 1015283, "kołczan w stylu północnym", 85.0, 135.0, typeof( SpinedLeather ), "niebieskie skóry", 28, 1044463 );
                AddRes(index, typeof(WyrmsHeart), "serce wyrma", 1, "masz za malo serc wyrma");

                // Specjalne kolczany dostepny z BODow
				//index = AddCraft( typeof( QuiverOfFire ), 1015283, 1073109, 65.0, 115.0, typeof( Leather ), 1044462, 28, 1044463 );
				//AddRes( index, typeof( FireRuby ), 1032695, 15, 1042081 );
				//AddRecipe( index, 502 );
				//SetNeededExpansion( index, Expansion.ML );

				//index = AddCraft( typeof( QuiverOfIce ), 1015283, 1073110, 65.0, 115.0, typeof( Leather ), 1044462, 28, 1044463 );
				//AddRes( index, typeof( WhitePearl ), 1032694, 15, 1042081 );
				//AddRecipe( index, 503 );
				//SetNeededExpansion( index, Expansion.ML );

				//index = AddCraft( typeof( QuiverOfBlight ), 1015283, 1073111, 65.0, 115.0, typeof( Leather ), 1044462, 28, 1044463 );
				//AddRes( index, typeof( Blight ), 1032675, 10, 1042081 );
				//AddRecipe( index, 504 );
				//SetNeededExpansion( index, Expansion.ML );

				//index = AddCraft( typeof( QuiverOfLightning ), 1015283, 1073112, 65.0, 115.0, typeof( Leather ), 1044462, 28, 1044463 );
				//AddRes( index, typeof( Corruption ), 1032676, 10, 1042081 );
				//AddRecipe( index, 505 );
				//SetNeededExpansion( index, Expansion.ML );
			}
            // zombie

            MarkOption = true;
            Repair = Core.AOS;
            RecycleHelper = new Rechop();

            // 17.06.2012 :: zombie
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

            // Dodanie cieciw jako surowca:
            SetSubRes2( typeof( BowstringLeather ), 1032722 );

            AddSubRes2( typeof( BowstringLeather ),     1032722,  00.0, 1032753, 1032595 );
            AddSubRes2 (typeof( BowstringGut ),         1032725,  00.0, 1032756, 1032595);
            //AddSubRes2( typeof( BowstringSpinedLeather ),   1032775,  00.0, 1032771, 1032595 );
            //AddSubRes2( typeof( BowstringHornedLeather ),   1032776,  00.0, 1032772, 1032595 );
            //AddSubRes2( typeof( BowstringBarbedLeather ),   1032777,  00.0, 1032773, 1032595 );
            AddSubRes2( typeof( BowstringCannabis ),    1032723,  00.0, 1032754, 1032595 );
            AddSubRes2( typeof( BowstringSilk ),        1032724,  00.0, 1032755, 1032595 );
            AddSubRes2( typeof( BowstringFiresteed ),   1032726, 100.0, 1032757, 1032595 );
            AddSubRes2( typeof( BowstringUnicorn ),     1032727, 100.0, 1032758, 1032595 );
            AddSubRes2( typeof( BowstringNightmare ),   1032728, 100.0, 1032759, 1032595 );
            AddSubRes2( typeof( BowstringKirin ),       1032729, 100.0, 1032760, 1032595 );
            
            // 1072643  ZWYKLE DREWNO
            // 1032722	SKORZANE CIECIWY (~1_NUMBER~)
            // 1032723	KONOPNE CIECIWY (~1_NUMBER~)
            // 1032724	JEDWABNE CIECIWY (~1_NUMBER~)
            // 1032725	JELITOWE CIECIWY (~1_NUMBER~)
            // 1032726	CIECIWY Z WLOSIA OGNISTEGO (~1_NUMBER~)
            // 1032727	CIECIWY Z WLOSIA JEDNOROZCA (~1_NUMBER~)
            // 1032728	CIECIWY Z WLOSIA KOSZMARA (~1_NUMBER~)
            // 1032729	CIECIWY Z WLOSIA KI-RINA (~1_NUMBER~)
            // 1032753	Skorzane cieciwy
            // 1032754	Konopne cieciwy
            // 1032755	Jedwabne cieciwy
            // 1032756	Jelitowe cieciwy
            // 1032757	Cieciwy z wlosia ognistego rumaka
            // 1032758	Cieciwy z wlosia jednorozca
            // 1032759	Cieciwy z wlosia koszmara
            // 1032760	Cieciwy z wlosia ki-rina
            // 1032595	Nie wiesz jak zrobic cos z cieciw tego typu.
        }
    }
}