using System;
using Server;
using Server.Items;
using Server.Regions;
using Server.Spells.Ninjitsu;
using System.Collections;
using Server.Commands;
using System.Collections.Generic;

namespace Server.Mobiles
{
	public class Paragon
	{
		public static double ChestChance = .20;         // Chance that a paragon will carry a paragon chest
		
		public static int    Hue   = 0x501;        // Paragon hue
		
		// Buffs
		public static double HitsBuff   = 5.0;
		public static double StrBuff    = 1.05;
		public static double IntBuff    = 1.20;
		public static double DexBuff    = 1.20;
		public static double SkillsBuff = 1.20;
		public static double SpeedBuff  = 1.20;
		public static double FameBuff   = 1.40;
		public static double KarmaBuff  = 1.40;
		public static int    DamageBuff = 5;

		public static void Convert( BaseCreature bc )
		{
			if ( bc.IsParagon )
				return;

			bc.Hue = Hue;

			if ( bc.HitsMaxSeed >= 0 )
				bc.HitsMaxSeed = (int)( bc.HitsMaxSeed * HitsBuff );
			
			bc.RawStr = (int)( bc.RawStr * StrBuff );
			bc.RawInt = (int)( bc.RawInt * IntBuff );
			bc.RawDex = (int)( bc.RawDex * DexBuff );

			bc.Hits = bc.HitsMax;
			bc.Mana = bc.ManaMax;
			bc.Stam = bc.StamMax;

			for( int i = 0; i < bc.Skills.Length; i++ )
			{
				Skill skill = (Skill)bc.Skills[i];

				if ( skill.Base > 0.0 )
					skill.Base *= SkillsBuff;
			}

			bc.PassiveSpeed /= SpeedBuff;
			bc.ActiveSpeed /= SpeedBuff;

			bc.DamageMin += DamageBuff;
			bc.DamageMax += DamageBuff;

			if ( bc.Fame > 0 )
				bc.Fame = (int)( bc.Fame * FameBuff );

			if ( bc.Fame > 32000 )
				bc.Fame = 32000;

			// TODO: Mana regeneration rate = Sqrt( buffedFame ) / 4

			if ( bc.Karma != 0 )
			{
				bc.Karma = (int)( bc.Karma * KarmaBuff );

				if( Math.Abs( bc.Karma ) > 32000 )
					bc.Karma = 32000 * Math.Sign( bc.Karma );
			}
		}

		public static void UnConvert( BaseCreature bc )
		{
			if ( !bc.IsParagon )
				return;

			bc.Hue = 0;

			if ( bc.HitsMaxSeed >= 0 )
				bc.HitsMaxSeed = (int)( bc.HitsMaxSeed / HitsBuff );
			
			bc.RawStr = (int)( bc.RawStr / StrBuff );
			bc.RawInt = (int)( bc.RawInt / IntBuff );
			bc.RawDex = (int)( bc.RawDex / DexBuff );

			bc.Hits = bc.HitsMax;
			bc.Mana = bc.ManaMax;
			bc.Stam = bc.StamMax;

			for( int i = 0; i < bc.Skills.Length; i++ )
			{
				Skill skill = (Skill)bc.Skills[i];

				if ( skill.Base > 0.0 )
					skill.Base /= SkillsBuff;
			}
			
			bc.PassiveSpeed *= SpeedBuff;
			bc.ActiveSpeed *= SpeedBuff;

			bc.DamageMin -= DamageBuff;
			bc.DamageMax -= DamageBuff;

			if ( bc.Fame > 0 )
				bc.Fame = (int)( bc.Fame / FameBuff );
			if ( bc.Karma != 0 )
				bc.Karma = (int)( bc.Karma / KarmaBuff );
		}

		public static bool CheckConvert( BaseCreature bc )
		{
			return CheckConvert( bc, bc.Location, bc.Map );
		}

		public static bool CheckConvert( BaseCreature bc, Point3D location, Map m )
		{
			if ( !Core.AOS )
				return false;

			if (!IsInParagonRegion(location, m))
				return false;

			if ( bc is BaseChampion || bc is Harrower || bc is BaseVendor || bc is BaseEscortable || bc is Clone || bc.Summoned || bc.Controlled || bc is Clone || bc is KhaldunSummoner || bc is KhaldunZealot)
				return false;

			int fame = bc.Fame;

			if ( fame > 32000 )
				fame = 32000;

			double chance = 1 / Math.Round( 20.0 - ( fame / 3200 ));

			return ( chance > Utility.RandomDouble() );
		}

		private static bool IsInParagonRegion(Point3D location, Map m) {
			List<Region> regions = Region.Regions.FindAll(x => x.Map.Equals(m) && x is ParagonsRegion);
			foreach(Region region in regions) {
				if (region.Contains(location)){
					return true;
				}
			}
			return false;
		}

		public static bool CheckArtifactChance( Mobile m, BaseCreature bc )
		{
			if ( !Core.AOS )
				return false;

			double fame = (double)bc.Fame;

			if ( fame > 32000 )
				fame = 32000;

			double chance = 1 / ( Math.Max( 10, 100 * ( 0.83 - Math.Round( Math.Log( Math.Round( fame / 6000, 3 ) + 0.001, 10 ), 3 ) ) ) * ( 100 - Math.Sqrt( m.Luck ) ) / 100.0 );

			return chance > Utility.RandomDouble();
		}

		public static void GiveArtifactTo( Mobile m )
		{
			Item item = ArtifactHelper.CreateRandomParagonArtifact();
			m.Emote("Postac zdobyla artefakt!"); 
			int itemID= 0xF5F;
			IEntity from = new Entity( Serial.Zero, new Point3D( m.X, m.Y, m.Z ), m.Map );
			IEntity to = new Entity( Serial.Zero, new Point3D( m.X, m.Y, m.Z + 50 ), m.Map );
			Effects.SendMovingParticles( from, to, itemID, 1, 0, false, false, 33, 3, 9501, 1, 0, EffectLayer.Head, 0x100 );

			if ( m.AccessLevel > AccessLevel.Player )
			{
				item.ModifiedBy = m.Account.Username;
				item.ModifiedDate = DateTime.Now;
				
				string log = m.AccessLevel + " " + CommandLogging.Format( m );
				log += " recived Paragon Artifact " + CommandLogging.Format( item ) + " [Paragon]";
				
				CommandLogging.WriteLine( m, log );
			}

			item.LabelOfCreator = CommandLogging.Format( m ) as string;
			
			if ( m.AddToBackpack( item ) )
			{
				m.SendMessage( "W nagrode za pokonanie istoty otrzymujesz artefakt" );
			}
			else
			{
				Container bank = m.BankBox;

				if ( bank != null && bank.TryDropItem( m, item, false ) )
				{
					m.SendMessage( "W nagrode za pokonanie istoty otrzymujesz artefakt. Znajdziesz go w skrzyni bankowej." );
				}
			else
				{
					m.SendLocalizedMessage( 1072523 ); // You find an artifact, but your backpack and bank are too full to hold it.

					item.MoveToWorld( m.Location, m.Map );
				}
			}
		}
	}
}