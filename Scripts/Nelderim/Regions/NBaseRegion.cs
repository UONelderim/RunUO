using System;
using Server.Accounting;
using Server.Mobiles;
using Server.Spells;
using Arya.Jail;
using Server.Nelderim;
using Server.Spells.Necromancy;
using Server.Spells.Eighth;
using System.Collections.Generic;
using System.Xml;
using System.Text.RegularExpressions;

namespace Server.Regions
{
	public class NBaseRegion : BaseRegion
	{
		private static List<Mobile> m_FirstWarning;
		private static List<Mobile> m_SecondWarning;
		private static ViolationsTimer m_Timer;
		private bool m_Allowed;
		protected bool m_SeasonOverride;
		protected SeasonList m_Season;
		
		public string PrettyName
		{
			get { return Regex.Replace(Name.Replace('_', ' '), @"([a-z])([A-Z])", m => string.Format("{0} {1}", m.Groups[1], m.Groups[2])); }
		}
			
		public static void Initialize()
		{
			m_FirstWarning = new List<Mobile>();
			m_SecondWarning = new List<Mobile>();
			m_Timer = new ViolationsTimer();
			m_Timer.Start();
		}
		
		public NBaseRegion( XmlElement xml, Map map, Region parent ) : base( xml, map, parent )
		{
            string allowedAttrName = "allowed";
            m_Allowed = xml.HasAttribute( allowedAttrName ) && XmlConvert.ToInt32( xml.GetAttribute( allowedAttrName ) ) == 0 ? false : true;
		}
			
		public static List<Mobile> FirstWarning => m_FirstWarning;

		public static List<Mobile> SecondWarning => m_SecondWarning;

		private static bool Violator( Mobile m )
		{
			return FirstWarning.Contains( m ) || SecondWarning.Contains( m );
		}
		
		private static void Disviolate( Mobile m )
		{
			FirstWarning.Remove( m );
			SecondWarning.Remove( m );
		}
		
		public override void OnEnter( Mobile m )
		{ 	
			#region zakazane regiony
			
			if ( m.Player && m.AccessLevel == AccessLevel.Player )
			{
				bool violator = Violator( m );
				
				if ( !m_Allowed )
				{
					m.SendLocalizedMessage( 505616, "", 0x25 );
					if ( !violator ) FirstWarning.Add( m );
				}
				else if ( violator )
				{
					m.SendLocalizedMessage( 505617, "", 167 );
					
					Disviolate( m );
				}
			}
			
			#endregion
			
			#region Zakaz summonow
			
			try
			{
				if ( m is BaseCreature )
				{
					BaseCreature bc = m as BaseCreature;
					
					if ( bc.Controlled || bc.Summoned )
					{
						if ( NelderimRegionSystem.GetRegion(Name).PetIsBanned(bc) )
						{
						   Mobile owner = bc.Summoned ? bc.SummonMaster : bc.ControlMaster;

                           if ( owner != null )
                           {
                               if ( owner is PlayerMobile && owner.AccessLevel == AccessLevel.Player && owner.Player && owner.Kills < 5 )
                               {
                                   owner.SendLocalizedMessage( 505618, "", 0x25 );
                                   new BannedPetTimer( bc ).Start();
                               }
                           }
						}
					}	
				}
			}
			catch ( Exception e )
			{
				Console.WriteLine( e.ToString() );
			}
			#endregion

			#region Zakaz magicznych transofmracji
			try
			{
				if ( m is PlayerMobile && m.AccessLevel == AccessLevel.Player )
				{
					TransformContext transformContext = TransformationSpellHelper.GetContext( m );
					if ( transformContext != null && NelderimRegionSystem.GetRegion(Name).CastIsBanned((Spell)transformContext.Spell) && !(transformContext.Spell is VampiricEmbraceSpell))
					{

						m.Criminal = m.Kills < 5 ? true : false;
						m.SendLocalizedMessage( 505619, "", 0x25 );
					}
				}
			}
			catch (Exception e )
			{
				Console.WriteLine( e );
			}
            #endregion
        }

        public override void OnExit( Mobile m )
		{ 
		}

		public static void CheckUndershadowRules(Mobile m, ref int global, ref int personal)
		{
            if (IsWithinUndershadow(m))
            {
                global = LightCycle.DungeonLevel;
            }
        }
        private static bool IsWithinUndershadow(Mobile m)
		{
            foreach (var region in m.Map.Regions.Values)
            {
				if (!region.Contains(m.Location))
					continue;

                if (region.GetType() == typeof(Undershadow))
				{
                    return true;
				}
            }
			return false;
        }

		public override void AlterLightLevel(Mobile m, ref int global, ref int personal)
		{
			base.AlterLightLevel(m, ref global, ref personal);

            // Apply Undershadow light level to cities/villages within Undershadow region.
            // This is to prevent from light level in Undershadow being dependant on sun/moon state.
            CheckUndershadowRules(m, ref global, ref personal);
        }

		public override void OnSpellCast(Mobile m, ISpell s)
		{
			// Sprawdza czy dana szkola magii, lub zaklecie nie sa zakazane w regionie
			if ( m.AccessLevel == AccessLevel.Player && NelderimRegionSystem.GetRegion( Name).CastIsBanned((Spell)s) )
			{
				m.Criminal = m.Kills < 5;
				m.SendLocalizedMessage( 505619, "", 0x25 );
			}			
			else if ( m.AccessLevel == AccessLevel.Player 
			         && ( s is SummonFamiliarSpell || s is AirElementalSpell || s is EarthElementalSpell
			        	|| s is FireElementalSpell || s is SummonDaemonSpell || s is WaterElementalSpell ) )
			{
				if ( NelderimRegionSystem.GetRegion( Name).CastIsBanned((Spell)s) )
				{
					m.Criminal = m.Kills < 5;
					m.SendLocalizedMessage( 505620, "", 0x25 );
				}
			}
			
			base.OnSpellCast( m, s );
		}

		private class BannedPetTimer : Timer
		{		
			private BaseCreature m_Pet;
			
			public BannedPetTimer( BaseCreature bc ) : base( TimeSpan.FromSeconds( 30 ) )
			{
				m_Pet = bc;
				Priority = TimerPriority.FiveSeconds;
			}

			protected override void OnTick()
			{
				try
				{
					if ( !m_Pet.Deleted )
					{
						if ( m_Pet.Controlled || m_Pet.Summoned )
						{
							if ( NelderimRegionSystem.GetRegion( m_Pet.Region.Name).PetIsBanned( m_Pet ))
							{
								Mobile owner = m_Pet.Summoned ? m_Pet.SummonMaster : m_Pet.ControlMaster;
								    	
								if ( owner != null )
									if ( owner is PlayerMobile && owner.AccessLevel == AccessLevel.Player && owner.Player )
							    	{
										owner.Criminal = owner.Kills < 5;
							    		owner.SendLocalizedMessage( 505621, "", 0x25 );
							    	}
							}	
						}
					}
				}
				catch ( Exception e )
				{
					Console.WriteLine( e.ToString() );
				}
			}
		}
		
		private class ViolationsTimer : Timer
		{		
			private NBaseRegion m_Region;
			
			public ViolationsTimer() : base( TimeSpan.FromMinutes( 0 ), TimeSpan.FromMinutes( 1 ) )
			{
				Priority = TimerPriority.FiveSeconds;
			}

			protected override void OnTick()
			{
				for ( int i = 0; i < SecondWarning.Count; i++)
				{
					Mobile m = SecondWarning[i];
					
					if ( !( m.Region is NBaseRegion ) )
					{
						m.SendLocalizedMessage( 505617, "", 167 );
						continue;
					}
					
					Console.WriteLine( "{0} laduje w wiezieniu za naruszenie zakazanych regionow!", m.Name );
					
					if ( JailSystem.CanBeJailed( m ) )
					{
						m.SendLocalizedMessage( 505622, "", 0x25 );
						JailSystem.CommitJailing( m, m.Account as Account, m, "Wkroczenie na zakazany teren", true, 
						              TimeSpan.FromHours( 1 ), true, 
						              "Automatyczne wiezienie za wkroczenie na zakazany teren -> X: " + m.X 
						              + " Y: " + m.Y, JailSystem.m_Jail[ 0 ] );
						
					}
				}
				
				SecondWarning.Clear();
				
				for ( int i = 0; i < FirstWarning.Count; i++)
				{
					Mobile m = FirstWarning[i];
					
					if ( !( m.Region is NBaseRegion ) )
					{
						m.SendLocalizedMessage( 505617, "", 167 );
						continue;
					}
					
					m.SendLocalizedMessage( 505623, "", 0x25 );
					SecondWarning.Add( m );
				}
				
				FirstWarning.Clear();
			}
		}
	}
}
