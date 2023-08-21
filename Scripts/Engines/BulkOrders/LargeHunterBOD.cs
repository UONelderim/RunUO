// 08.03.16 :: emfor :: bulki mysliwego
// 09.08.24 :: emfor :: logowanie do pliku

using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Items;
using Mat = Server.Engines.BulkOrders.BulkMaterialType;
using System.Diagnostics;

namespace Server.Engines.BulkOrders
{
	[TypeAlias( "Scripts.Engines.BulkOrders.LargeHunterBOD" )]
	public class LargeHunterBOD : LargeBOD
	{

		public override int ComputeFame()
		{
			return HunterRewardCalculator.Instance.ComputeFame( this );
		}

		public override int ComputeGold()
		{
			return HunterRewardCalculator.Instance.ComputeGold( this );
		}

        
        [Constructable]
        public LargeHunterBOD() : this((double)Utility.RandomMinMax(71, 120))
        {
        }

        [Constructable]
		public LargeHunterBOD( double theirSkill)
		{
			LargeBulkEntry[] entries;

            int[] chances = { 0, 0, 0, 0 };
            if (theirSkill >= 105.1)
            {
				chances[0] = 31; // Level 1
				chances[1] = 35; // Level 2
				chances[2] = 34; // Level 3
				chances[3] = 0;  // Level 4 (bossy)
			}
            else if (theirSkill >= 90.1)
            {
                chances[0] = 33; // Level 1
                chances[1] = 67; // Level 2
                chances[2] = 0;  // Level 3
                chances[3] = 0;  // Level 4 (bossy)
            }
            else if (theirSkill >= 70.1)
            {
                chances[0] = 100; // Level 1
                chances[1] = 0;   // Level 2
                chances[2] = 0;   // Level 3
                chances[3] = 0;   // Level 4 (bossy)
            }
            else
            {
                chances[0] = 100; // Level 1
                chances[1] = 0;   // Level 2
                chances[2] = 0;   // Level 3
                chances[3] = 0;   // Level 4 (bossy)
            }

            int level = 0;

            double rand = Utility.Random(100);
            double rangeMin = 0;
            for (int i = 0; i < 4; i++)
            {
                double rangeMax = rangeMin + chances[i];
                if (rand < rangeMax && rand >= rangeMin)
                {
                    level = i + 1;
                    break;
                }
                rangeMin += chances[i];
            }

            switch (level)
			{
				default:
				case 1:
					switch (Utility.Random( 12 ))
					{
						default:
						case 0: entries = LargeBulkEntry.ConvertEntries(this, LargeBulkEntry.Animal_1); break;
						case 1: entries = LargeBulkEntry.ConvertEntries(this, LargeBulkEntry.Animal_2); break;
						case 2: entries = LargeBulkEntry.ConvertEntries(this, LargeBulkEntry.Ants); break;
						case 3: entries = LargeBulkEntry.ConvertEntries(this, LargeBulkEntry.Elementals_1); break;
						case 4: entries = LargeBulkEntry.ConvertEntries(this, LargeBulkEntry.Horda_1); break;
						case 5: entries = LargeBulkEntry.ConvertEntries(this, LargeBulkEntry.Horda_2); break;
						case 6: entries = LargeBulkEntry.ConvertEntries(this, LargeBulkEntry.Horda_3); break;
						case 7: entries = LargeBulkEntry.ConvertEntries(this, LargeBulkEntry.Orcs); break;
						case 8: entries = LargeBulkEntry.ConvertEntries(this, LargeBulkEntry.OreElementals); break;
						case 9: entries = LargeBulkEntry.ConvertEntries(this, LargeBulkEntry.Plants); break;
						case 10: entries = LargeBulkEntry.ConvertEntries(this, LargeBulkEntry.Rozne); break;
						case 11: entries = LargeBulkEntry.ConvertEntries(this, LargeBulkEntry.Undead_1); break;
					}
					break;

				case 2:
					switch (Utility.Random( 9 ))
					{
						default:
						case 0: entries = LargeBulkEntry.ConvertEntries(this, LargeBulkEntry.Elementals_2); break;
						case 1: entries = LargeBulkEntry.ConvertEntries(this, LargeBulkEntry.Gargoyles); break;
						case 2: entries = LargeBulkEntry.ConvertEntries(this, LargeBulkEntry.Jukas); break;
						case 3: entries = LargeBulkEntry.ConvertEntries(this, LargeBulkEntry.Mech); break;
						case 4: entries = LargeBulkEntry.ConvertEntries(this, LargeBulkEntry.Minotaurs); break;
						case 5: entries = LargeBulkEntry.ConvertEntries(this, LargeBulkEntry.Ophidians); break;
						case 6: entries = LargeBulkEntry.ConvertEntries(this, LargeBulkEntry.Strong); break;
						case 7: entries = LargeBulkEntry.ConvertEntries(this, LargeBulkEntry.Terathans); break;
						case 8: entries = LargeBulkEntry.ConvertEntries(this, LargeBulkEntry.Undead_2); break;
					}
					break;

				case 3:
					switch (Utility.Random( 5 ))
					{
						default:
						case 0: entries = LargeBulkEntry.ConvertEntries(this, LargeBulkEntry.Kox_1); break;
						case 1: entries = LargeBulkEntry.ConvertEntries(this, LargeBulkEntry.Kox_2); break;
						case 2: entries = LargeBulkEntry.ConvertEntries(this, LargeBulkEntry.Kox_3); break;
						case 3: entries = LargeBulkEntry.ConvertEntries(this, LargeBulkEntry.Kox_4); break;
						case 4: entries = LargeBulkEntry.ConvertEntries(this, LargeBulkEntry.Kox_5); break;
					}
					break;

                case 4:
                    switch (Utility.Random( 8 ))
                    {
                        default:
                        case 0: entries = LargeBulkEntry.ConvertEntries(this, LargeBulkEntry.Boss_1); break;
                        case 1: entries = LargeBulkEntry.ConvertEntries(this, LargeBulkEntry.Boss_2); break;
                        case 2: entries = LargeBulkEntry.ConvertEntries(this, LargeBulkEntry.Boss_3); break;
                        case 3: entries = LargeBulkEntry.ConvertEntries(this, LargeBulkEntry.Boss_4); break;
                        case 4: entries = LargeBulkEntry.ConvertEntries(this, LargeBulkEntry.Boss_5); break;
                        case 5: entries = LargeBulkEntry.ConvertEntries(this, LargeBulkEntry.Boss_6); break;
                        case 6: entries = LargeBulkEntry.ConvertEntries(this, LargeBulkEntry.Boss_7); break;
                        case 7: entries = LargeBulkEntry.ConvertEntries(this, LargeBulkEntry.Boss_8); break;
                    }
                    break;
            }		

			int hue = 0xA7E;
			int amountMax = Utility.RandomList( 10, 15, 20, 20 );

			this.Hue = hue;
			this.AmountMax = amountMax;
			this.Entries = entries;
			//this.Level = type;
			
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
				StringBuilder strB = new StringBuilder();
				strB.Append(entries[0].Details.Type.ToString());
				for(int i = 1; i < entries.Length; i++)
				{
					strB.Append(", ");
					strB.Append(entries[i].Details.Type.ToString());
				}
				string log = String.Format("Large\t{0}\t{1}\t{2}", DateTime.Now, strB.ToString(), amountMax.ToString());
				m_Output.WriteLine( log );
				m_Output.Flush();
				m_Output.Close();
			}
			catch
			{
			}
		}

		public LargeHunterBOD( int amountMax, LargeBulkEntry[] entries, int level )
		{
			this.Hue = 0xA7E;
			this.AmountMax = amountMax;
			this.Entries = entries;
			//this.Level = level;
		}

		public override List<Item> ComputeRewards( bool full )
		{
			List<Item> list = new List<Item>();

			RewardGroup rewardGroup = HunterRewardCalculator.Instance.LookupRewards( HunterRewardCalculator.Instance.ComputePoints( this ) );

			if ( rewardGroup != null )
			{
                RewardItem rewardItem = rewardGroup.AcquireItem();

				if ( rewardItem != null )
				{
					Item item = rewardItem.Construct();

					if ( item != null )
						list.Add( item );
				}
			}

			return list;
		}

		public LargeHunterBOD( Serial serial ) : base( serial )
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
	}
}
