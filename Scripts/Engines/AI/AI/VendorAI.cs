using System;
using System.Collections;
using Server.Targeting;
using Server.Network;
using System.Text.RegularExpressions;
using Server.Nelderim;

//
// This is a first simple AI
//
//

namespace Server.Mobiles
{
	public class VendorAI : BaseAI
	{
		public VendorAI(BaseCreature m) : base (m)
		{
		}

		public override bool DoActionWander()
		{
			m_Mobile.DebugSay( "I'm fine" );

			if ( m_Mobile.Combatant != null )
			{
				if ( m_Mobile.Debug )
					m_Mobile.DebugSay( "{0} is attacking me", m_Mobile.Combatant.Name );

				m_Mobile.Say( Utility.RandomList( 1005305, 501603 ) );

				Action = ActionType.Flee;
			}
			else
			{
				if ( m_Mobile.FocusMob != null )
				{
					if ( m_Mobile.Debug )
						m_Mobile.DebugSay( "{0} has talked to me", m_Mobile.FocusMob.Name );

					Action = ActionType.Interact;
				}
				else
				{
					m_Mobile.Warmode = false;

                    // 26.06.2012 :: zombie :: plotki
                    #region Rumors
                    Map map = m_Mobile.Map;

                    if ( map != null )
                    {
                        IPooledEnumerable eable = map.GetMobilesInRange( m_Mobile.Location, 7 );
                        bool say = false;

                        foreach ( Mobile m in eable )
                        {
                            if ( m is PlayerMobile && !m.Hidden && m.Alive && Utility.RandomDouble() < m_Mobile.CurrentSpeed / 240.0 && m_Mobile.Activation( m ) )
                            {
                                say = true;
                                // Console.WriteLine( "Plotka {0} dla gracza {1} o godzinie {2}", m_Mobile.Name, m.Name, DateTime.Now );
                                break;
                            }
                        }

                        if ( say )
                            m_Mobile.AnnounceRandomRumor( PriorityLevel.Medium );

                    }
                    #endregion
                    // zombie

					base.DoActionWander();
				}
			}

			return true;
		}

		public override bool DoActionInteract()
		{
			Mobile customer = m_Mobile.FocusMob;

			if ( m_Mobile.Combatant != null )
			{
				if ( m_Mobile.Debug )
					m_Mobile.DebugSay( "{0} is attacking me", m_Mobile.Combatant.Name );

				m_Mobile.Say( Utility.RandomList( 1005305, 501603 ) );

				Action = ActionType.Flee;
				
				return true;
			}

			if ( customer == null || customer.Deleted || customer.Map != m_Mobile.Map )
			{
				m_Mobile.DebugSay( "My customer have disapeared" );
				m_Mobile.FocusMob = null;

				Action = ActionType.Wander;
			}
			else
			{
				if ( customer.InRange( m_Mobile, m_Mobile.RangeFight ) )
				{
					if ( m_Mobile.Debug )
						m_Mobile.DebugSay( "I am with {0}", customer.Name );

					m_Mobile.Direction = m_Mobile.GetDirectionTo( customer );
				}
				else
				{
					if ( m_Mobile.Debug )
						m_Mobile.DebugSay( "{0} is gone", customer.Name );

					m_Mobile.FocusMob = null;

					Action = ActionType.Wander;	
				}
			}

			return true;
		}

		public override bool DoActionGuard()
		{
			m_Mobile.FocusMob = m_Mobile.Combatant;
			return base.DoActionGuard();
		}

		public override bool HandlesOnSpeech( Mobile from )
		{
			if ( from.InRange( m_Mobile, 4 ) )
				return true;

			return base.HandlesOnSpeech( from );
		}

        public override void OnSpeech( SpeechEventArgs e )
        {
            base.OnSpeech( e );

            Mobile from = e.Mobile;

            if ( m_Mobile is BaseVendor bv && from.InRange( bv, 2 ) && !e.Handled )
            {
                if ( Regex.IsMatch( e.Speech, "sprzed", RegexOptions.IgnoreCase ) )  // *vendor sell*
                {
                    e.Handled = true;
					if (e.Speech.ToLower() == "sprzed" || Regex.IsMatch(e.Speech, "^sprzed..?$", RegexOptions.IgnoreCase)) 
						bv.OnLazySpeech();
					else {
						bv.VendorSell(from);
						bv.Direction = bv.GetDirectionTo(from);
					}
                }
                else if ( Regex.IsMatch( e.Speech, "kup", RegexOptions.IgnoreCase ) )  // *vendor sell*
                {
                    e.Handled = true;
					if (e.Speech.ToLower() == "kup" || Regex.IsMatch(e.Speech, "^kup..?$", RegexOptions.IgnoreCase)) 
						bv.OnLazySpeech();
					else {
						bv.VendorBuy(from);
						bv.Direction = bv.GetDirectionTo(from);
					}

                }
                else if (Regex.IsMatch(e.Speech, "zlecen", RegexOptions.IgnoreCase))
                {
	                if (bv.SupportsBulkOrders(e.Mobile) && bv.checkWillingness(e.Mobile))
	                {
		                e.Handled = true;
		                if (e.Speech.ToLower() == "zlecen" || Regex.IsMatch(e.Speech, "^zlecen..?$", RegexOptions.IgnoreCase))
			                bv.OnLazySpeech();
		                else
		                {
			                bv.ProvideBulkOrder(e.Mobile);
			                bv.Direction = bv.GetDirectionTo(from);
		                }
	                }
                }
                else if ( Regex.IsMatch( e.Speech, "plotk", RegexOptions.IgnoreCase ) )
                {
                    e.Handled = true;
					if (e.Speech.ToLower() == "plotk" || Regex.IsMatch(e.Speech, "^plotk.?$", RegexOptions.IgnoreCase)) bv.OnLazySpeech();
					else {
						bv.SayAboutRumors(from);
						bv.Direction = bv.GetDirectionTo(from);
					}
                }
                else
                    bv.SayRumor( from, e );

				if (from is PlayerMobile && !from.Hidden && from.Alive && Utility.RandomDouble() < bv.GetRumorsActionPropability())
					bv.AnnounceRandomRumor(PriorityLevel.Low);
			}

        }
        // zombie
	}
}