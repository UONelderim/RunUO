using System;
using System.Collections;
using System.Collections.Generic;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;
using Server.Items;
using Server.Spells;
using Server.Spells.First;
using Server.Spells.Second;
using Server.Spells.Third;
using Server.Spells.Fourth;
using Server.Spells.Fifth;
using Server.Spells.Sixth;
using Server.Spells.Seventh;
using Server.Spells.Necromancy;
using Server.Misc;
using Server.Regions;
using Server.SkillHandlers;
using System.Text;

namespace Server.Mobiles
{
    public class MageAI : SpellCasterAI
	{
		public MageAI( BaseCreature m ) : base( m )
		{
		}

		public override bool DoActionWander()
		{
            OnGuardActionWarden(); 

			if ( AcquireFocusMob( m_Mobile.RangePerception, m_Mobile.FightMode, false, false, true ) )
			{
				if ( m_Mobile.Debug )
					m_Mobile.DebugSay( "I am going to attack {0}", m_Mobile.FocusMob.Name );

                OnGuardActionAttack( m_Mobile.FocusMob );

				m_Mobile.Combatant = m_Mobile.FocusMob;
				Action = ActionType.Combat;
				m_NextCastTime = DateTime.Now;
			}
			else if ( m_Mobile.Mana < m_Mobile.ManaMax )
			{
				m_Mobile.DebugSay( "I am going to meditate" );

				m_Mobile.UseSkill( SkillName.Meditation );
			}
			else
			{
				m_Mobile.DebugSay( "I am wandering" );

				m_Mobile.Warmode = false;

				base.DoActionWander();

                if ( m_Mobile.Poisoned )
                {
                    new CureSpell( m_Mobile, null ).Cast();
                }
				else if ( !m_Mobile.Controlled )
				{
					Spell spell = CheckCastHealingSpell();

					if ( spell != null )
						spell.Cast();
				}
			}

			return true;
		}

        // 23.06.2012 :: zombie
		public override Spell ChooseSpell( Mobile c )
		{
            Spell spell = CheckCastHealingSpell();

			if ( spell != null )
				return spell;

            int maxCircle = GetMaxCircle();

            // curse, 10% chance
            if ( !HasMod( c, false ) && Utility.RandomDouble() <= 0.1 )
                return maxCircle >= 4 ? new CurseSpell( m_Mobile, null ) : GetRandomCurseSpell();

            // bless, 20% chance
            if ( maxCircle >= 3 && !HasMod( m_Mobile, true ) && Utility.RandomDouble() <= 0.2 )
                return new BlessSpell( m_Mobile, null );


            switch ( Utility.Random( 5 ) )
			{
				default:
				case 0: // Poison them
				{
                    if ( CanMeditate )
                        goto case 4;
                    else if ( CanPoison && !c.Poisoned )
                        spell = new PoisonSpell( m_Mobile, null );

                    break;
				}
                case 1: // Deal some damage
				{
                    if ( c.Hits <= c.HitsMax * 0.3 )
                    {
                        spell = GetRandomMageryDamageSpell();
                    }
                    else
                        goto case 2;

                    break;
				}
                case 2:// Deal some damage
				{
                    if ( c.Hits <= c.HitsMax * 0.3 )
                    {
                        spell = GetRandomMageryDamageSpell();
                    }
                    else
                        goto case 4;

                    break;
				}
				case 3: // Deal some damage
				{
                    if ( c.Hits <= c.HitsMax * 0.3 )
                    {
                        spell = GetRandomMageryDamageSpell();
                    }
                    else
                        goto case 4;

                    break;
				}
				case 4: // 
				{
                    // Activate medit
                    if ( Utility.RandomDouble() < 0.15 && CanParalyze && CanMeditate )
					{
						if ( c.Paralyzed && !c.Poisoned )
						{
							m_Mobile.DebugSay( "I am going to meditate" );

							m_Mobile.UseSkill( SkillName.Meditation );
						}
						else if ( !c.Poisoned )
						{
							spell = new ParalyzeSpell( m_Mobile, null );
						}
					}
                    else if ( m_SpellCombo == null ) // Set up a combo
                    {
                        m_SpellCombo = GetRandomCombo( m_Mobile, maxCircle );

                        if ( m_SpellCombo == null )
                            spell = GetRandomMageryDamageSpell();
                    }

					break;
				}
			}

            if ( ( spell is PoisonSpell && c.Poisoned ) || !CanPoison )
                spell = null;
            else if ( spell is ParalyzeSpell && ( !CanParalyze || c.Paralyzed ) )
                spell = null;

			return spell;
		}
        // zombie

		public override bool DoActionFlee()
		{
			Mobile c = m_Mobile.Combatant;

			if ( (m_Mobile.Mana > 20 || m_Mobile.Mana == m_Mobile.ManaMax) && m_Mobile.Hits > (m_Mobile.HitsMax / 2) )
			{
				m_Mobile.DebugSay( "I am stronger now, my guard is up" );
				Action = ActionType.Guard;
			}
			else if ( AcquireFocusMob( m_Mobile.RangePerception, m_Mobile.FightMode, false, false, true ) )
			{
				if ( m_Mobile.Debug )
					m_Mobile.DebugSay( "I am scared of {0}", m_Mobile.FocusMob.Name );

				RunFrom( m_Mobile.FocusMob );
				m_Mobile.FocusMob = null;

				if ( m_Mobile.Poisoned && Utility.Random( 0, 5 ) == 0 )
					new CureSpell( m_Mobile, null ).Cast();
			}
			else
			{
				m_Mobile.DebugSay( "Area seems clear, but my guard is up" );

				Action = ActionType.Guard;
				m_Mobile.Warmode = true;
			}

			return true;
		}

		public override bool DoActionCombat()
		{
			Mobile c = m_Mobile.Combatant;
			m_Mobile.Warmode = true;

			if ( c == null || c.Deleted || !c.Alive || c.IsDeadBondedPet || !m_Mobile.CanSee( c ) || !m_Mobile.CanBeHarmful( c, false ) || c.Map != m_Mobile.Map )
			{
				// Our combatant is deleted, dead, hidden, or we cannot hurt them
				// Try to find another combatant

				if ( AcquireFocusMob( m_Mobile.RangePerception, m_Mobile.FightMode, false, false, true ) )
				{
					if ( m_Mobile.Debug )
						m_Mobile.DebugSay( "Something happened to my combatant, so I am going to fight {0}", m_Mobile.FocusMob.Name );

					m_Mobile.Combatant = c = m_Mobile.FocusMob;
					m_Mobile.FocusMob = null;
				}
				else
				{
					m_Mobile.DebugSay( "Something happened to my combatant, and nothing is around. I am on guard." );
					Action = ActionType.Guard;
					return true;
				}
			}

			if ( !m_Mobile.InLOS( c ) )
			{
				m_Mobile.DebugSay( "I can't see my target" );

				if ( AcquireFocusMob( m_Mobile.RangePerception, m_Mobile.FightMode, false, false, true ) )
				{
					m_Mobile.DebugSay( "Nobody else is around" );
					m_Mobile.Combatant = c = m_Mobile.FocusMob;
					m_Mobile.FocusMob = null;
				}
			}

			// Mobki uzywaja ataku specjalnego ze skilla Wrestling:
            //if ( !m_Mobile.StunReady && m_Mobile.Skills[SkillName.Wrestling].Value >= 90.0 )
			//	EventSink.InvokeStunRequest( new StunRequestEventArgs( m_Mobile ) );

			if ( !m_Mobile.InRange( c, m_Mobile.RangePerception ) )
			{
				// They are somewhat far away, can we find something else?

				if ( AcquireFocusMob( m_Mobile.RangePerception, m_Mobile.FightMode, false, false, true ) )
				{
					m_Mobile.Combatant = m_Mobile.FocusMob;
					m_Mobile.FocusMob = null;
				}
				else if ( !m_Mobile.InRange( c, m_Mobile.RangePerception * 3 ) )
				{
					m_Mobile.Combatant = null;
				}

				c = m_Mobile.Combatant;

				if ( c == null )
				{
                    OnCombatantFled( c );

					return true;
				}
			}

			if ( !m_Mobile.Controlled && !m_Mobile.Summoned && !m_Mobile.IsParagon )
			{
				if ( m_Mobile.Hits < m_Mobile.HitsMax * 20/100 )
				{
					// We are low on health, should we flee?

					bool flee = false;

					if ( m_Mobile.Hits < c.Hits )
					{
						// We are more hurt than them

						int diff = c.Hits - m_Mobile.Hits;

						flee = ( Utility.Random( 0, 100 ) > (10 + diff) ); // (10 + diff)% chance to flee
					}
					else
					{
						flee = Utility.Random( 0, 100 ) > 10; // 10% chance to flee
					}
					
					if ( flee )
					{
						if ( m_Mobile.Debug )
							m_Mobile.DebugSay( "I am going to flee from {0}", c.Name );

						Action = ActionType.Flee;
						return true;
					}
				}
			}

            // 23.06.2012 :: zombie :: Sprawdzamy czy nie podejsc do przeciwnika
			/* wylaczam funkcje walki dystansowej:
            if ( DateTime.Now > m_NextRangeChange )
			{
				if( !m_Mobile.Controlled )
				{
					int defRange = m_DefaultRange;

					if ( m_Mobile.Hits > m_Mobile.HitsMax * 0.5 && c.Hits < c.HitsMax * 0.25 )
						m_Mobile.RangeFight = 1;
					else if ( ( Utility.RandomDouble() <= 0.15 && c.Hits >= c.HitsMax * 0.7 ) || m_Mobile.Hits <= m_Mobile.HitsMax * 0.3 || ( CanMeditate && m_Mobile.Mana < m_Mobile.ManaMax * 0.1 ) )
						m_Mobile.RangeFight = 12;
					else
						m_Mobile.RangeFight = defRange;

					m_NextRangeChange = DateTime.Now + TimeSpan.FromSeconds( 5 );
				}
				else
				{
					m_Mobile.RangeFight = m_DefaultRange;	// przywroc range po oswojeniu
				}
            }
			*/

            // zombie

			if ( m_Mobile.Spell == null && DateTime.Now > m_NextCastTime && m_Mobile.InRange( c, 12 ) )
			{
				// We are ready to cast a spell

				Spell spell = null;
                Mobile toDispel = FindDispelTarget(true);

				if ( m_Mobile.Poisoned ) // Top cast priority is cure
				{
                    m_Mobile.DebugSay( "I am going to cure myself" );

                    spell = GetCureSpell();
				}
				else if ( toDispel != null && CheckDispel() ) // Something dispellable is attacking us
				{
					m_Mobile.DebugSay( "I am going to dispel {0}", toDispel );
					spell = DoDispel( toDispel );
				}
				else if ( m_SpellCombo != null ) // We are doing a spell combo
				{
                    spell = CheckCastHealingSpell();
                    
                    if( spell == null )
                        spell = m_SpellCombo.GetSpell();
				}
				else if ( CanPoison && ( Utility.RandomDouble() <= 0.3 || c.Spell is HealSpell || c.Spell is GreaterHealSpell ) && !c.Poisoned ) // They have a heal spell out
				{
					spell = new PoisonSpell( m_Mobile, null );
				}
				else
				{
					spell = ChooseSpell( c );
				}
                
				// Now we have a spell picked
				// Move first before casting

                if (toDispel != null && false) // uciekanie przed przywolancem
				{
					if ( m_Mobile.InRange( toDispel, 10 ) )
						RunFrom( toDispel );
					else if ( !m_Mobile.InRange( toDispel, 12 ) )
						RunTo( toDispel );
				}
				else
				{
					RunTo( c );
				}

                if ( spell != null )
                {
                    if ( spell.Cast() )
                    {
                        if ( m_SpellCombo != null )
                        {
                            m_SpellCombo.MoveNext();

                            if ( m_SpellCombo.Finished )
                                m_SpellCombo = null;     
                        }
                    }
                }

				TimeSpan delay = TimeSpan.FromSeconds( m_Mobile.ActiveSpeed );

                m_NextCastTime = DateTime.Now + delay;
			}
			else if ( m_Mobile.Spell == null || !m_Mobile.Spell.IsCasting )
			{
				RunTo( c );
			}

			return true;
		}

		public override bool DoActionGuard()
		{
			if ( AcquireFocusMob( m_Mobile.RangePerception, m_Mobile.FightMode, false, false, true ) )
			{
				m_Mobile.DebugSay( "I am going to attack {0}", m_Mobile.FocusMob.Name );

				m_Mobile.Combatant = m_Mobile.FocusMob;
				Action = ActionType.Combat;
			}
			else
			{
				if ( !m_Mobile.Controlled )
				{
					ProcessTarget();

					Spell spell = CheckCastHealingSpell();

					if ( spell != null )
						spell.Cast();
				}

				base.DoActionGuard();
			}

			return true;
		}
	}
}
