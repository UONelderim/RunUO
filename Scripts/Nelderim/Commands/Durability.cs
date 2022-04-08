using System;
using Server.Network;
using Server.Mobiles;
using System.Reflection;
using System.Collections.Generic;
using System.IO;
using Server.Items;
using System.Collections;
using Server.Spells;
using Server.Engines.Craft;

namespace Server.Commands
{
	public class Durabilities
	{
		public static void Initialize()
		{
			CommandSystem.Register( "dur", AccessLevel.Administrator, new CommandEventHandler( Dur_OnCommand ) );
            CommandSystem.Register( "durWeapon", AccessLevel.Administrator, new CommandEventHandler( DurWeap_OnCommand ) );
		}

        public static void DurWeap_OnCommand( CommandEventArgs e )
		{
            return;
            // Nie uzywac tych funkcji na serwerze dla graczy!!! tylko lokalnie na swoim kompie!
            // Funkcja modyfikuje globalny parametr regulujacy predkosc rozpadu ekqipunku!

            Mobile from = e.Mobile;

            int start_durability = 60;
            double break_chance = Config.ItemDurabilityLostChance; // backup
            int damage = 40;
            bool use_maul = false;

            CraftResource resource = CraftResource.Valorite;
            //CraftResource.Valorite;
            string restr = "";
            switch(resource) {
                case CraftResource.None:		restr = "none"; break;
                case CraftResource.Valorite:	restr = "valorite"; break;
            };

            if ( e.Length > 0 && e.GetInt32(0) > 0 )
            {
                start_durability = e.GetInt32(0);
            }
            
            if ( e.Length > 1 && e.GetInt32(1) > 0 )
            {
                Config.ItemDurabilityLostChance = e.GetInt32(1);
            }

            if ( e.Length > 2 )
            {
                damage = e.GetInt32(2);
                from.SendMessage("Damage: {0}", damage);
            }

            if ( e.Length > 3 )
            {
                use_maul = true;
                from.SendMessage("Uderzamy mlotkiem!");
            }

            string logsDirectory = "Logi/Durability";
			if( !Directory.Exists( logsDirectory ) )
				Directory.CreateDirectory( logsDirectory );
            DateTime now = DateTime.Now;
            string fileName = String.Format( "dur({0})_ch({1})_{2}_{3}-{4}-{5} {6}-{7}-{8} {9} Kryss", start_durability, Config.ItemDurabilityLostChance, from.Account, now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second, restr );
			fileName = Path.Combine( Core.BaseDirectory, logsDirectory + "/" + fileName + ".log" );
            FileLogger logger = new FileLogger( fileName, true );

            logger.WriteLine( String.Format("Start Durability: {0}", start_durability) );
            logger.WriteLine( String.Format("ItemDurabilityLostChance: {0}", Config.ItemDurabilityLostChance) );
            logger.WriteLine( String.Format("damage: {0}", damage) );
            logger.WriteLine( String.Format("maul: {0}", (use_maul?"true":"false") ) );

            Mobile defender = new Ogre();
            defender.Str = 500;
            defender.Hits = defender.HitsMax;

			//foreach( Type type in armors )
            {

                List<int> StartDurability = new List<int>();
                List<int> StartDurabilityHits = new List<int>();
                List<int> RepairsFailed = new List<int>();
                List<int> Repairs = new List<int>();

                //for(int startDur=5; startDur<=80; startDur+=5)
                int startDur = start_durability;
                {

                    int repeat = 10;
                    
                    int sum_hits = 0;
                    int sum_hits_between = 0;
                    int sum_fails = 0;
                    int sum_repairs = 0;
                    
                    for(int s=0; s<repeat; ++s ) // kilka prob, dla usrednienia wyniku
                    {
                        Kryss weapon = (Kryss)Activator.CreateInstance(typeof(Kryss));    // Resource ? ? ?
                        from.AddToBackpack(weapon);

                        weapon.Resource = resource;
                        weapon.MaxHitPoints = startDur;
                        weapon.HitPoints = weapon.MaxHitPoints;

                        //armor.Resource = resource;

                        //Console.WriteLine("{0}/{1} start", armor.HitPoints, armor.MaxHitPoints);

                        logger.WriteLine( String.Format("{0}: {1}\t\t({2},{3})", weapon.MaxHitPoints, -1, s, -1) );

                        int hits = 0;
                        int hits_between = 0;
                        int repairs = 0;
                        int failedRepairs = 0;
                        for(int i=0; true; ++i) // uderzenia w zbroje
                        {
                            if( weapon.MaxHitPoints <= 1 )
                            {
                                //Console.WriteLine("Item rozjebany");
                                break;
                            }

                            while ( weapon.HitPoints <= 1 )
                            {
                                if( hits_between != 0 ) // tylko raz
                                {
                                    logger.WriteLine( String.Format("{0}: {1}\t{2}\t\t({3},{4})", weapon.MaxHitPoints, hits_between, ( (double)hits_between/(double)weapon.MaxHitPoints ), s, i) );
                                    hits_between = 0;
                                }

                                //Console.WriteLine("{0}/{1} przed ({2})", armor.HitPoints, armor.MaxHitPoints, repairs);
                                // repair
                                if( !Repair.Simulate( from, weapon )) 
                                {
                                    return;
                                }
                               // Console.WriteLine("{0}/{1} po    ({2})", armor.HitPoints, armor.MaxHitPoints, repairs);
                                ++repairs;
                                if( weapon.HitPoints != weapon.MaxHitPoints )
                                    ++failedRepairs;
                                else
                                    break;

                                if( weapon.MaxHitPoints <= 1 )
                                    break;
                            }

                            if( weapon == null || weapon.Deleted )
                            {
                                //Console.WriteLine("rozjebane {0} {1}", i, repairs);
                                break;
                            }

                            if( repairs > 300 ) {
                                Console.WriteLine("Przerywamy!");
                                return;
                            }
                            
                            // hit
                            //armor.OnHit(sword, damage);
                            weapon.OnSwing(from,defender);
                            defender.Hits = defender.HitsMax;
                            ++hits;
                            ++hits_between;
                        }

                        if( weapon != null && !weapon.Deleted )
                            weapon.Delete();

                        sum_hits += hits;
                        sum_fails += failedRepairs;
                        sum_repairs += repairs;

                        //Console.WriteLine("hits({0}) repairs({1}) failed({2})", hits, repairs, failedRepairs);
                    }// s

                    sum_hits = (int) ( (double)sum_hits / (double)repeat );
                    sum_fails = (int) ( (double)sum_fails / (double)repeat );
                    sum_repairs = (int) ( (double)sum_repairs / (double)repeat );

                    //Console.WriteLine("startDur({0}) average hits({0})", startDur, hits);

                    StartDurability.Add(startDur);
                    StartDurabilityHits.Add(sum_hits);
                    RepairsFailed.Add(sum_fails);
                    Repairs.Add(sum_repairs);

                }// startDur

                for( int i=0; i<StartDurability.Count; ++i ) {
                    logger.WriteLine( String.Format("{0}:\t{1}\t{2}\t{3}", StartDurability[i], StartDurabilityHits[i], RepairsFailed[i], Repairs[i] ) );
                }

            }//type in armors
            
            defender.Delete();

            Config.ItemDurabilityLostChance = break_chance;
		}


		public static void Dur_OnCommand( CommandEventArgs e )
		{
            return;
            // Nie uzywac tych funkcji na serwerze dla graczy!!! tylko lokalnie na swoim kompie!
            // Funkcja modyfikuje globalny parametr regulujacy predkosc rozpadu ekqipunku!

            Mobile from = e.Mobile;

            int start_durability = 60;
            double break_chance = Config.ItemDurabilityLostChance; // backup
            int damage = 4;
            bool use_maul = false;

            CraftResource resource = CraftResource.None;
            //CraftResource.Valorite;
            string restr = "";
            switch(resource) {
                case CraftResource.None:		restr = "none"; break;
                case CraftResource.Valorite:	restr = "valorite"; break;
            };

            if ( e.Length > 0 && e.GetInt32(0) > 0 )
            {
                start_durability = e.GetInt32(0);
            }
            
            if ( e.Length > 1 && e.GetInt32(1) > 0 )
            {
                Config.ItemDurabilityLostChance = e.GetInt32(1);
            }

            if ( e.Length > 2 )
            {
                damage = e.GetInt32(2);
                from.SendMessage("Damage: {0}", damage);
            }

            if ( e.Length > 3 )
            {
                use_maul = true;
                from.SendMessage("Uderzamy mlotkiem!");
            }

            string logsDirectory = "Logi/Durability";
			if( !Directory.Exists( logsDirectory ) )
				Directory.CreateDirectory( logsDirectory );
            DateTime now = DateTime.Now;
            string fileName = String.Format( "dur({0})_ch({1})_{2}_{3}-{4}-{5} {6}-{7}-{8} {9} BronzeShield", start_durability, Config.ItemDurabilityLostChance, from.Account, now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second, restr );
			fileName = Path.Combine( Core.BaseDirectory, logsDirectory + "/" + fileName + ".log" );
            FileLogger logger = new FileLogger( fileName, true );

            logger.WriteLine( String.Format("Start Durability: {0}", start_durability) );
            logger.WriteLine( String.Format("ItemDurabilityLostChance: {0}", Config.ItemDurabilityLostChance) );
            logger.WriteLine( String.Format("damage: {0}", damage) );
            logger.WriteLine( String.Format("maul: {0}", (use_maul?"true":"false") ) );

            Type [] armors = new Type[] { // w nazwie pliku tez zmien klase
                //typeof(ChainCoif),
                //typeof(PlateHelm),
                //typeof(LeatherCap),
                typeof(BronzeShield),
                //typeof(PlateLegs),
                /*
                typeof(PlateChest),
                typeof(PlateGorget),
                typeof(PlateHelm),
                typeof(PlateArms),
                typeof(PlateGloves)*/
            };
            
            Kryss sword = new Kryss();
            Maul maul = new Maul();
            BaseWeapon weapon;
            if( use_maul )
                weapon = maul;
            

            BaseTool tool = new Tongs();

			foreach( Type type in armors )
            {

                List<int> StartDurability = new List<int>();
                List<int> StartDurabilityHits = new List<int>();
                List<int> RepairsFailed = new List<int>();
                List<int> Repairs = new List<int>();

                //for(int startDur=5; startDur<=80; startDur+=5)
                int startDur = start_durability;
                {

                    int repeat = 10;
                    
                    int sum_hits = 0;
                    int sum_hits_between = 0;
                    int sum_fails = 0;
                    int sum_repairs = 0;
                    
                    for(int s=0; s<repeat; ++s ) // kilka prob, dla usrednienia wyniku
                    {
                        BronzeShield armor = (BronzeShield)Activator.CreateInstance(type);    // Resource ? ? ?
                        from.AddToBackpack(armor);

                        armor.MaxHitPoints = startDur;
                        armor.HitPoints = armor.MaxHitPoints;

                        //armor.Resource = resource;

                        //Console.WriteLine("{0}/{1} start", armor.HitPoints, armor.MaxHitPoints);

                        logger.WriteLine( String.Format("{0}: {1}\t\t({2},{3})", armor.MaxHitPoints, -1, s, -1) );

                        int hits = 0;
                        int hits_between = 0;
                        int repairs = 0;
                        int failedRepairs = 0;
                        for(int i=0; true; ++i) // uderzenia w zbroje
                        {
                            if( armor.MaxHitPoints <= 1 )
                            {
                                //Console.WriteLine("Item rozjebany");
                                break;
                            }

                            while ( armor.HitPoints <= 1 )
                            {
                                if( hits_between != 0 ) // tylko raz
                                {
                                    logger.WriteLine( String.Format("{0}: {1}\t{2}\t\t({3},{4})", armor.MaxHitPoints, hits_between, ( (double)hits_between/(double)armor.MaxHitPoints ), s, i) );
                                    hits_between = 0;
                                }

                                //Console.WriteLine("{0}/{1} przed ({2})", armor.HitPoints, armor.MaxHitPoints, repairs);
                                // repair
                                if( !Repair.Simulate( from, armor )) 
                                {
                                    return;
                                }
                               // Console.WriteLine("{0}/{1} po    ({2})", armor.HitPoints, armor.MaxHitPoints, repairs);
                                ++repairs;
                                if( armor.HitPoints != armor.MaxHitPoints )
                                    ++failedRepairs;
                                else
                                    break;

                                if( armor.MaxHitPoints <= 1 )
                                    break;
                            }

                            if( armor == null || armor.Deleted )
                            {
                                //Console.WriteLine("rozjebane {0} {1}", i, repairs);
                                break;
                            }

                            if( repairs > 300 ) {
                                Console.WriteLine("Przerywamy!");
                                return;
                            }

                            // hit
                            armor.OnHit(sword, damage);
                            ++hits;
                            ++hits_between;
                        }

                        if( armor != null && !armor.Deleted )
                            armor.Delete();

                        sum_hits += hits;
                        sum_fails += failedRepairs;
                        sum_repairs += repairs;

                        //Console.WriteLine("hits({0}) repairs({1}) failed({2})", hits, repairs, failedRepairs);
                    }// s

                    sum_hits = (int) ( (double)sum_hits / (double)repeat );
                    sum_fails = (int) ( (double)sum_fails / (double)repeat );
                    sum_repairs = (int) ( (double)sum_repairs / (double)repeat );

                    //Console.WriteLine("startDur({0}) average hits({0})", startDur, hits);

                    StartDurability.Add(startDur);
                    StartDurabilityHits.Add(sum_hits);
                    RepairsFailed.Add(sum_fails);
                    Repairs.Add(sum_repairs);

                }// startDur

                for( int i=0; i<StartDurability.Count; ++i ) {
                    logger.WriteLine( String.Format("{0}:\t{1}\t{2}\t{3}", StartDurability[i], StartDurabilityHits[i], RepairsFailed[i], Repairs[i] ) );
                }

            }//type in armors

            Config.ItemDurabilityLostChance = break_chance;
		}
	
	}
}
