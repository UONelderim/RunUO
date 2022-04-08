    //////////////////////////////////
   //			           //
  //      Scripted by Raelis      //
 //		             	 //
//////////////////////////////////
using System; 
using System.Collections;
using Server.Items; 
using Server.Mobiles; 
using Server.Misc;
using Server.Network;

namespace Server.Items 
{ 
   	public class DragonEgg3: Item 
   	{ 
		public bool m_AllowEvolution;
		public Timer m_EvolutionTimer;
		private DateTime m_End;

		[CommandProperty( AccessLevel.GameMaster )]
		public bool AllowEvolution
		{
			get{ return m_AllowEvolution; }
			set{ m_AllowEvolution = value; }
		}

		[Constructable]
		public DragonEgg3() : base( 0x0C5D )
		{
			Weight = 1.0;
			Name = "Smocze Jajo";
			Hue = 41;
			AllowEvolution = false;

			m_EvolutionTimer = new EvolutionTimer( this, TimeSpan.FromDays( 1.0 ) );
			m_EvolutionTimer.Start();
			m_End = DateTime.Now + TimeSpan.FromDays( 1.0 );
		}

            	public DragonEgg3( Serial serial ) : base ( serial ) 
            	{             
           	}

		public override void OnDoubleClick( Mobile from )
		{
                        if ( !IsChildOf( from.Backpack ) ) 
                        { 
                                from.SendMessage( "Musisz miec jajo w plecaku." ); 
                        } 
			else if ( this.AllowEvolution == true )
			{
				this.Delete();
				from.SendMessage( "Z jaja wykluwa sie Smocza Larwa!!" );

				EvolutionDragon3 dragon = new EvolutionDragon3();

         			dragon.Map = from.Map; 
         			dragon.Location = from.Location; 

				//dragon.Controled = true;

				//dragon.ControlMaster = from;

				//dragon.IsBonded = false;
			}
			else
			{
				from.SendMessage( "To jajo nie jest jeszcze gotowe." );
			}
		}

           	public override void Serialize( GenericWriter writer ) 
           	{ 
              		base.Serialize( writer ); 
              		writer.Write( (int) 1 ); 
			writer.WriteDeltaTime( m_End );
           	} 
            
           	public override void Deserialize( GenericReader reader ) 
           	{ 
              		base.Deserialize( reader ); 
              		int version = reader.ReadInt();

			switch ( version )
			{
				case 1:
				{
					m_End = reader.ReadDeltaTime();
					m_EvolutionTimer = new EvolutionTimer( this, m_End - DateTime.Now );
					m_EvolutionTimer.Start();

					break;
				}
				case 0:
				{
					TimeSpan duration = TimeSpan.FromDays( 1.0 );

					m_EvolutionTimer = new EvolutionTimer( this, duration );
					m_EvolutionTimer.Start();
					m_End = DateTime.Now + duration;

					break;
				} 
			}
           	} 

		private class EvolutionTimer : Timer
		{ 
			private DragonEgg3 de;

			private int cnt = 0;

			public EvolutionTimer( DragonEgg3 owner, TimeSpan duration ) : base( duration )
			{ 
				de = owner;
			}

			protected override void OnTick() 
			{
				cnt += 1;

				if ( cnt == 1 )
				{
					de.AllowEvolution = true;
				}
			}
		}
        } 
} 