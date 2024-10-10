using System;
using System.Collections.Generic;
using System.IO;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.BulkOrders
{
	public class SmallHunterBOD : SmallBOD
	{
		public override int ComputeFame()
		{
			return 0;
		}

		public override int ComputeGold()
		{
			return HunterRewardCalculator.Instance.ComputeGold( this );
		}

		public override List<Item> ComputeRewards( bool full )
		{
			List<Item> list = new List<Item>();

			RewardGroup rewardGroup = HunterRewardCalculator.Instance.LookupRewards(HunterRewardCalculator.Instance.ComputePoints(this));

			if (rewardGroup != null) {
				if (full) {
					for (int i = 0; i < rewardGroup.Items.Length; ++i) {
						Item item = rewardGroup.Items[i].Construct();

						if (item != null)
							list.Add(item);
					}
				} else {
					RewardItem rewardItem = rewardGroup.AcquireItem();

					if (rewardItem != null) {
						Item item = rewardItem.Construct();

						if (item != null)
							list.Add(item);
					}
				}
			}

			return list;
		}
		
		public static SmallHunterBOD CreateRandomFor( Mobile m, double theirSkill )
		{
			SmallBulkEntry[] entries;

			int[] chances = {0, 0, 0, 0};
            if ( theirSkill >= 105.1 )
			{
                chances[0] = 15;
                chances[1] = 40;
                chances[2] = 25;
                chances[3] = 20;
			}
			else if( theirSkill >= 90.1 )
            {
                chances[0] = 25;
                chances[1] = 55;
                chances[2] = 20;
            }
			else if( theirSkill >= 70.1 )
            {
                chances[0] = 75;
                chances[1] = 25;
            }
			else
            {
                chances[0] = 100;
            }

            int level = Utility.RandomIndex(chances) + 1;
            
			switch (level)
			{
                default:
                case 1: entries = SmallBulkEntry.Hunter1; break;
                case 2: entries = SmallBulkEntry.Hunter2; break;
                case 3: entries = SmallBulkEntry.Hunter3; break;
                case 4: entries = SmallBulkEntry.Hunter4; break;
            }

			if ( entries.Length > 0 )
			{
				int amountMax;

				if ( theirSkill >= 70.1 )
					amountMax = Utility.RandomList( 10, 15, 20, 20 );
				else if ( theirSkill >= 50.1 )
					amountMax = Utility.RandomList( 10, 15, 15, 20 );
				else
					amountMax = Utility.RandomList( 10, 10, 15, 20 );

				SmallBulkEntry entry = entries[Utility.Random( entries.Length )];
				
				//Logowanie
				if ( !Directory.Exists( "Logi" ) )
						Directory.CreateDirectory( "Logi" );

				string directory = "Logi/HunterBulkOrders";

				if ( !Directory.Exists( directory ) )
					Directory.CreateDirectory( directory );

				try
				{
					StreamWriter m_Output = new StreamWriter( Path.Combine( directory, "GivenHunterBODs.log" ), true );
					m_Output.AutoFlush = true;
					string log = String.Format("Small\t{0}\t{1}\t{2}", DateTime.Now, entry.Type.ToString(), amountMax.ToString());
					m_Output.WriteLine( log );
					m_Output.Flush();
					m_Output.Close();
				}
				catch
				{
				}

				if ( entry != null )
					return new SmallHunterBOD( entry, amountMax );
			}

			return null;
		}

		private SmallHunterBOD( SmallBulkEntry entry, int amountMax )
		{
			Hue = 0xA8E;
			AmountMax = amountMax;
			Type = entry.Type;
			Number = entry.Number;
			Graphic = entry.Graphic;
			//this.Level = entry.Level;
		}

		[Constructable]
		public SmallHunterBOD()
		{
			SmallBulkEntry[] entries = Utility.RandomList(SmallBulkEntry.Hunter1, SmallBulkEntry.Hunter2, SmallBulkEntry.Hunter3, SmallBulkEntry.Hunter4);

            if ( entries.Length > 0 )
			{
				int hue = 0xA8E;
				int amountMax = Utility.RandomList( 10, 15, 20 );

				SmallBulkEntry entry = entries[Utility.Random( entries.Length )];

				Hue = hue;
				AmountMax = amountMax;
				Type = entry.Type;
				Number = entry.Number;
				Graphic = entry.Graphic;
				//this.Level = entry.Level;
			}
		}

		public SmallHunterBOD( int amountCur, int amountMax, Type type, int number, int graphic, int level)
		{
			Hue = 0xA8E;
			AmountMax = amountMax;
			AmountCur = amountCur;
			Type = type;
			Number = number;
			Graphic = graphic;
			//this.Level = level;
		}

		public SmallHunterBOD( Serial serial ) : base( serial )
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

		public override void EndCombine( Mobile from, object o )
		{
			if ( o is Corpse corpse && corpse.Owner != null )
			{
				Type objectType = corpse.Owner.GetType();

				if ( AmountCur >= AmountMax )
				{
					from.SendLocalizedMessage( 1045166 ); // The maximum amount of requested items have already been combined to this deed.
				}
				else if ( Type == null || (objectType != Type && !objectType.IsSubclassOf( Type )) )
				{
					from.SendLocalizedMessage( 1045169 ); // The item is not in the request.
				}
				else if (corpse.Hunters.Contains(from))
				{
					from.SendMessage("Już oddałeś te zwłoki.");
				}
				else if (corpse.HunterBods.Contains(this))
				{
					from.SendMessage("To zlecenie już zawiera te zwłoki");
				}
				else if ( !CanBeHunted( from, corpse ) )
				{
					from.SendMessage("Te zwłoki nie mogą zostać dodane.");
				} 
				else
				{ 
					corpse.Hunters.Add(from);
					corpse.HunterBods.Add(this);
					++AmountCur;

					from.SendLocalizedMessage( 1045170 ); // The item has been combined with the deed.

					from.SendGump( new SmallBODGump( from, this ) );

					if ( AmountCur < AmountMax )
						BeginCombine( from );
				}
			}
			else
			{
				from.SendMessage("To nie może zostać dodane.");
			}
		}
		
		private bool CanBeHunted( Mobile from, Corpse c )
		{
			if( c == null || c.Owner == null )
				return false;
			
			if( from is PlayerMobile && c.Owner is BaseCreature mob)
			{
				if (mob.IsChampionSpawn || mob.Summoned)
					return false;
				
				var rights = BaseCreature.GetLootingRights( mob.DamageEntries, mob.HitsMax );
				return rights.Exists(ds => ds.m_HasRight && ds.m_Mobile == from);
			}
			return false;
		}
	}
}
