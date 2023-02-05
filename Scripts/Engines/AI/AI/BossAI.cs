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

namespace Server.Mobiles
{
    public class BossAI : SpellCasterAI
    {
        private DateTime m_NextBossAbility;

        public virtual bool BossAbilityDefined { get { return false; } }
        public virtual TimeSpan BossAbilityDelay { get { return TimeSpan.FromSeconds( 120 ); } }
        public virtual bool CanDoBossAbility { get { return BossAbilityDefined && DateTime.Now > m_NextBossAbility; } }

        public BossAI( BaseCreature m ) : base( m )
        {
        }

        public override double HealChance { get { return 0.20; } }
        public override double TeleportChance { get { return 0.15; } }
        public override double DispelChance { get { return 0.75; } }
        public virtual double DetectHiddenChance { get { return 0.20; } }
        public virtual double HideChance { get { return 0.20; } }

        public virtual bool DoBossAbility()
        {
            m_NextBossAbility = DateTime.Now + BossAbilityDelay;

            return true;
        }

        public override void OnActionChanged()
        {
            m_Mobile.RangeFight = m_DefaultRange;
            m_NextRangeChange += TimeSpan.FromMinutes( 1 );

            base.OnActionChanged();
        }

        public override bool DoActionWander()
        {
            if ( AcquireFocusMob( m_Mobile.RangePerception, m_Mobile.FightMode, false, false, true ) )
            {
                if ( m_Mobile.Debug )
                    m_Mobile.DebugSay( "I am going to attack {0}", m_Mobile.FocusMob.Name );

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

                if ( m_Mobile.Poisoned && CanCastMagerySpells )
                {
                    new CureSpell( m_Mobile, null ).Cast();
                }
                else if ( CanCastMagerySpells && ScaleByMagery( HealChance ) > Utility.RandomDouble() )
                {
                    if ( m_Mobile.Hits < ( m_Mobile.HitsMax - 50 ) )
                    {
                        if ( !new GreaterHealSpell( m_Mobile, null ).Cast() )
                            new HealSpell( m_Mobile, null ).Cast();
                    }
                    else if ( m_Mobile.Hits < ( m_Mobile.HitsMax - 10 ) )
                    {
                        new HealSpell( m_Mobile, null ).Cast();
                    }
                }
                else if ( CanChannel && m_Mobile.Hits < m_Mobile.HitsMax * 0.95 )
                {
                    m_Mobile.UseSkill( SkillName.SpiritSpeak );
                }
            }

            return true;
        }

        public virtual bool ChooseSchool( out SkillName skill )
        {
            skill = SkillName.Magery;

            if ( !CanCastMagerySpells && !CanCastNecroSpells )
                return false;
            else if ( !CanCastMagerySpells )
                skill = SkillName.Necromancy;
            else if ( !CanCastNecroSpells )
                skill = SkillName.Magery;
            else
            {
                double necro = m_Mobile.Skills[ SkillName.Necromancy ].Value;
                double magery = m_Mobile.Skills[ SkillName.Magery ].Value;
                double factor = necro / ( necro + magery );

                if ( Utility.RandomDouble() > factor )
                    skill = SkillName.Magery;
                else
                    skill = SkillName.Necromancy;
            }

            return true;
        }

        public override Spell ChooseSpell( Mobile c )
        {
            Spell spell = null;

            int healChance = ( m_Mobile.Hits == 0 ? m_Mobile.HitsMax : ( m_Mobile.HitsMax / m_Mobile.Hits ) );

            if ( m_Mobile.Summoned )
                healChance = 0;

            int maxCircle = GetMaxCircle();

            // curse, 50% chance
            if ( !HasMod( c, false ) && Utility.RandomDouble() <= 0.5 )
                return maxCircle >= 4 ? new CurseSpell( m_Mobile, null ) : GetRandomCurseSpell();

            // bless, 50% chance
            if ( maxCircle >= 3 && !HasMod( m_Mobile, true ) && Utility.RandomDouble() <= 0.5 )
                return new BlessSpell( m_Mobile, null );

            if ( m_Mobile.Hits < m_Mobile.HitsMax * 0.5 && Utility.RandomDouble() < 0.7 && !m_Mobile.Summoned )
            {
                if ( maxCircle >= 4 && m_Mobile.Hits < ( m_Mobile.HitsMax - 50 ) )
                    spell = new GreaterHealSpell( m_Mobile, null );
                else if ( m_Mobile.Hits < ( m_Mobile.HitsMax - 10 ) )
                    spell = new HealSpell( m_Mobile, null );
            }

            switch ( Utility.Random( 5 ) )
			{
				default:
				case 0: // Poison them
				{
                    if ( CanPoison && !c.Poisoned )
                        spell = new PoisonSpell( m_Mobile, null );
                    else
                        goto case 1;
                    break;
				}
				case 1: // Deal some damage
				{
                    if ( c.Hits * 0.3 <= c.HitsMax )
                    {
                        spell = GetRandomDamageSpell();
                    }
                    else
                        goto case 4;

                    break;
				}
                case 2:
                case 3:
				case 4: 
				{
                    if ( m_SpellCombo == null ) // Set up a combo
                    {
                        m_SpellCombo = GetRandomCombo( m_Mobile, maxCircle );

                        if( m_SpellCombo == null )
                            spell = GetRandomDamageSpell();
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

        public override bool DoActionCombat()
        {
            Mobile c = m_Mobile.Combatant;
            m_Mobile.Warmode = true;

            if ( c != null && c.Hidden && DetectHiddenChance >= Utility.RandomDouble() )
            {
                m_Mobile.DebugSay( "My target hid from me... detecting!" );

                DetectHidden();
            }

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
                if ( AcquireFocusMob( m_Mobile.RangePerception, m_Mobile.FightMode, false, false, true ) )
                {
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
                    m_Mobile.DebugSay( "My combatant has fled, so I am on guard" );
                    Action = ActionType.Guard;

                    return true;
                }
            }

            if ( m_Mobile.Hits < m_Mobile.HitsMax * 10 / 100 )
            {
                // We are low on health, should we flee?

                if ( m_DefaultRange != m_Mobile.RangeFight )
                    m_Mobile.RangeFight = m_DefaultRange;

                bool flee = false;

                if ( m_Mobile.Hits < c.Hits )
                {
                    // We are more hurt than them

                    int diff = c.Hits - m_Mobile.Hits;
                    flee = ( Utility.Random( 0, 100 ) > ( 10 + diff ) ); // (10 + diff)% chance to flee
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

            if ( m_Mobile.Combatant == null && CanMeditate )
            {
                m_Mobile.DebugSay( "I am going to meditate" );

                m_Mobile.UseSkill( SkillName.Meditation );
                return true;
            }

            // 23.06.2012 :: zombie :: Sprawdzamy czy nie podejsc do przeciwnika
			/* wyłączam modyfikacje RangeFight:
            if ( DateTime.Now > m_NextRangeChange && !m_Mobile.Controlled )
            {
                int defRange = m_DefaultRange;

                if ( m_Mobile.Hits > m_Mobile.HitsMax * 0.5 && c.Hits < c.HitsMax * 0.2 )
                    m_Mobile.RangeFight = 1;
                else if ( Utility.RandomDouble() <= 0.15 || m_Mobile.Hits <= m_Mobile.HitsMax * 0.3 || ( CanMeditate && m_Mobile.Mana < m_Mobile.ManaMax * 0.1 ) )
                    m_Mobile.RangeFight = 12;
                else
                    m_Mobile.RangeFight = defRange;

                m_NextRangeChange = DateTime.Now + TimeSpan.FromSeconds( 3 );
            }
			*/
            // zombie

            if ( CanDoBossAbility && DoBossAbility() )
                return true;

            if ( m_Mobile.Spell == null && DateTime.Now > m_NextCastTime && m_Mobile.InRange( c, 12 ) )
            {
                // We are ready to cast a spell

                Spell spell = null;
                Mobile toDispel = FindDispelTarget( true );

                if ( IsInHarmfulField )
                {
                    spell = new TeleportSpell( m_Mobile, null );
                }
                else if ( m_Mobile.Poisoned ) // Top cast priority is cure
                {
                    spell = new CureSpell( m_Mobile, null );
                }
                else if ( toDispel != null && CheckDispel() ) // Something dispellable is attacking us
                {
                    spell = DoDispel( toDispel );
                }
                else if ( m_SpellCombo != null ) // We are doing a spell combo
                {
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

                if ( toDispel != null && false ) // uciekanie przed przywolancem
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

				TimeSpan delay;

				delay = TimeSpan.FromSeconds( m_Mobile.ActiveSpeed );

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
                if ( m_Mobile.Debug )
                    m_Mobile.DebugSay( "I am going to attack {0}", m_Mobile.FocusMob.Name );

                m_Mobile.Combatant = m_Mobile.FocusMob;
                Action = ActionType.Combat;
            }
            else
            {
                if ( m_Mobile.Poisoned )
                {
                    new CureSpell( m_Mobile, null ).Cast();
                }
                else if ( ( ScaleByMagery( HealChance ) > Utility.RandomDouble() ) )
                {
                    if ( m_Mobile.Hits < ( m_Mobile.HitsMax - 50 ) )
                    {
                        if ( !new GreaterHealSpell( m_Mobile, null ).Cast() )
                            new HealSpell( m_Mobile, null ).Cast();
                    }
                    else if ( m_Mobile.Hits < ( m_Mobile.HitsMax - 10 ) )
                    {
                        new HealSpell( m_Mobile, null ).Cast();
                    }
                    else
                    {
                        base.DoActionGuard();
                    }
                }
                else if ( CanChannel && m_Mobile.Hits < m_Mobile.HitsMax * 0.99 )
                {
                    m_Mobile.UseSkill( SkillName.SpiritSpeak );
                }
                else
                {
                    base.DoActionGuard();
                }
            }

            return true;
        }

        public override bool DoActionFlee()
        {
            if ( ( m_Mobile.Mana > 20 || m_Mobile.Mana == m_Mobile.ManaMax ) && m_Mobile.Hits > ( m_Mobile.HitsMax / 2 ) )
            {
                m_Mobile.DebugSay( "I am stronger now, my guard is up" );
                Action = ActionType.Guard;
            }
            else if ( m_Mobile.Hidden )
                return true;
            else if ( AcquireFocusMob( m_Mobile.RangePerception, m_Mobile.FightMode, false, false, true ) )
            {
                if ( m_Mobile.Debug )
                    m_Mobile.DebugSay( "I am scared of {0}", m_Mobile.FocusMob.Name );

                if ( Utility.RandomDouble() < HideChance )
                {
                    m_Mobile.UseSkill( SkillName.Hiding );

                    if ( m_Mobile.Hidden )
                        return true;
                }

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

        public override void DetectHidden()
        {
            if ( m_Mobile.Deleted )
                return;

            Map map = m_Mobile.Map;

            if ( map != null )
            {
                IPooledEnumerable eable = m_Mobile.GetMobilesInRange( m_Mobile.RangePerception );
                foreach ( Mobile trg in eable )
                {
                    if ( trg != m_Mobile && trg.Hidden && trg.AccessLevel == AccessLevel.Player && m_Mobile.CheckSkill( SkillName.DetectHidden, 0.0, 100.0 ) )
                    {
                        m_Mobile.DebugSay( "Revealing {0}...", trg.Name );

                        trg.RevealingAction();
                        trg.SendLocalizedMessage( 500814 ); // You have been revealed!  
                    }
                }
                eable.Free();
            }
        }
    }
}
