using System; 
using System.Collections;
using Server.Network; 
using Server.Mobiles; 
using Server.Items; 
using Server.Gumps;

namespace Server.Items.Crops 
{ 
	public class WheatSeed : BaseCrop // DEPRECATED (usuwane przy Deserializacji)
    { 
		public override bool CanGrowGarden{ get{ return true; } }

		public WheatSeed() : this( 1 )
		{
            // DEPRECATED (usuwane przy Deserializacji)
        }

        public WheatSeed( int amount ) : base( 0xF27 )
		{
            // DEPRECATED (usuwane przy Deserializacji)

            Stackable = true; 
			Weight = .5; 
			Hue = 0x5E2; 

			Movable = true; 
			
			Amount = amount;
			Name = "przegnite nasiona pszenicy"; 
		}

		public WheatSeed( Serial serial ) : base( serial ) 
		{ 
		} 

		public override void Serialize( GenericWriter writer ) 
		{ 
			base.Serialize( writer ); 
			writer.Write( (int) 0 ); 
		} 

		public override void Deserialize( GenericReader reader ) 
		{ 
			base.Deserialize( reader ); 
			int version = reader.ReadInt();

			// Delete();
		} 
	} 


	public class WheatSeedling : BaseCrop // DEPRECATED (usuwane przy Deserializacji)
    { 
		private static Mobile m_sower;
		public Timer thisTimer;

		[CommandProperty( AccessLevel.GameMaster )]
		public Mobile Sower{ get{ return m_sower; } set{ m_sower = value; } }
		

		public WheatSeedling( Mobile sower ) : base( Utility.RandomList ( 0xDAE, 0xDAF ) ) 
		{
            // DEPRECATED (usuwane przy Deserializacji)

            Movable = false; 
			Name = "sadzonka pszenicy"; 
			m_sower = sower;
			
		} 

		public WheatSeedling( Serial serial ) : base( serial ) 
		{ 
		} 

		public override void Serialize( GenericWriter writer ) 
		{ 
			base.Serialize( writer ); 
			writer.Write( (int) 0 ); 
			writer.Write( m_sower );
		} 

		public override void Deserialize( GenericReader reader ) 
		{ 
			base.Deserialize( reader ); 
			int version = reader.ReadInt(); 
			m_sower = reader.ReadMobile();

			// Delete();
		} 
	} 

	public class WheatCrop : BaseCrop // DEPRECATED (usuwane przy Deserializacji)
    { 
		private const int max = 1;
		private int fullGraphic;
		private int pickedGraphic;
		private DateTime lastpicked;

		private Mobile m_sower;
		private int m_yield;

		public Timer regrowTimer;

		private DateTime m_lastvisit;

		[CommandProperty( AccessLevel.GameMaster )] 
		public DateTime LastSowerVisit{ get{ return m_lastvisit; } }

		[CommandProperty( AccessLevel.GameMaster )] 
		public bool Growing{ get{ return regrowTimer.Running; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public Mobile Sower{ get{ return m_sower; } set{ m_sower = value; } }
		
		[CommandProperty( AccessLevel.GameMaster )]
		public int Yield{ get{ return m_yield; } set{ m_yield = value; } }

		public int Capacity{ get{ return max; } }
		public int FullGraphic{ get{ return fullGraphic; } set{ fullGraphic = value; } }
		public int PickGraphic{ get{ return pickedGraphic; } set{ pickedGraphic = value; } }
		public DateTime LastPick{ get{ return lastpicked; } set{ lastpicked = value; } }
		

		public WheatCrop( Mobile sower ) : base( Utility.RandomList( 0xC55, 0xC56 ) ) 
		{
            // DEPRECATED (usuwane przy Deserializacji)

            Movable = false; 
			Name = "pszenica"; 

			m_sower = sower;
			m_lastvisit = DateTime.Now;

		}

		public WheatCrop( Serial serial ) : base( serial ) 
		{ 
		} 

		public override void Serialize( GenericWriter writer ) 
		{ 
			base.Serialize( writer ); 
			writer.Write( (int) 1 ); 
			writer.Write( m_lastvisit );
			writer.Write( m_sower );
		} 

		public override void Deserialize( GenericReader reader ) 
		{ 
			base.Deserialize( reader ); 
			int version = reader.ReadInt(); 
			switch ( version )
			{
				case 1:
				{
					m_lastvisit = reader.ReadDateTime();
					goto case 0;
				}
				case 0:
				{
					m_sower = reader.ReadMobile();
					break;
				}
			}

			if ( version == 0 ) 
				m_lastvisit = DateTime.Now;

			// Delete();
		} 
	} 
} 
