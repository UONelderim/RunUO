using System;
using Server;
using Server.Targeting; 
using Server.Network;
using Server.Mobiles;   
using Server.Misc;           
using Server.Items;
using Server.Spells.IceDragon;

namespace Server.Timers.IceDragon
{
	public class IceBreathTimer : Timer
		{	
			protected int duration;
			protected int damage;
			private Mobile m_Mobile;
			private IceBreathSpell s_Spell = new IceBreathSpell ( null, null );
										 	
			public IceBreathTimer ( Mobile m, IceBreathSpell s ) : base(TimeSpan.FromSeconds( 3.5 ), TimeSpan.FromSeconds(5.0 ) ) 
			{
				duration = 3;
				m_Mobile = m;
				s_Spell = s; 
			}

			protected override void OnTick()
			{
				damage = m_Mobile.HitsMax/10;
				m_Mobile.Hits = m_Mobile.Hits - damage; // Armour is ignored!
				duration --;

				if(duration <= 0)
				{
					
				m_Mobile.SendMessage("The Ice Has melted");
					
				m_Mobile.Frozen = false;
				m_Mobile.Squelched = false;
                                m_Mobile.CantWalk = false;
				m_Mobile.Hue = s_Spell.Gethue(); // Return to normal hue
					
				Stop();
				return;
				
				}
			}		
				
		}
}