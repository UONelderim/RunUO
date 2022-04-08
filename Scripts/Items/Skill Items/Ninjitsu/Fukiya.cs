using System;
using Server;
using Server.Targeting;
using System.Collections;
using System.Collections.Generic;
using Server.ContextMenus;

namespace Server.Items
{
	[FlipableAttribute( 0x27AA, 0x27F5 )]
	public class Fukiya : Item, IUsesRemaining
	{
        private static Hashtable m_Table = new Hashtable();
		private int m_UsesRemaining;

		private Poison m_Poison;
		private int m_PoisonCharges;

		[CommandProperty( AccessLevel.GameMaster )]
		public int UsesRemaining
		{
			get { return m_UsesRemaining; }
			set { m_UsesRemaining = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public Poison Poison
		{
			get{ return m_Poison; }
			set{ m_Poison = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int PoisonCharges
		{
			get { return m_PoisonCharges; }
			set { m_PoisonCharges = value; InvalidateProperties(); }
		}

		public bool ShowUsesRemaining{ get{ return true; } set{} }

		[Constructable]
		public Fukiya() : base( 0x27AA )
		{
			Weight = 4.0;
			Layer = Layer.OneHanded;
		}

		public Fukiya( Serial serial ) : base( serial )
		{
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );

			list.Add( 1060584, m_UsesRemaining.ToString() ); // uses remaining: ~1_val~

			if ( m_Poison != null && m_PoisonCharges > 0 )
				list.Add( 1062412 + m_Poison.Level, m_PoisonCharges.ToString() );
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( !IsChildOf( from ) )
				return;

			if ( m_UsesRemaining < 1 )
			{
				// You have no fukiya darts!
				from.SendLocalizedMessage( 1063325 );
			}
			else if ( IsThrowingNinjaThrowable(from) )
			{
				// You are already using that fukiya.
				from.SendLocalizedMessage( 1063326 );
			}
			// 09.09.2013 :: mortuus - GM ninja moze uzywac fukija majac zajete rece
			else if ( !BasePotion.HasFreeHand( from ) && from.Skills[SkillName.Ninjitsu].Value<100 )
			{
				// You must have a free hand to use a fukiya.
				from.SendLocalizedMessage( 1063327 );
			}
			else
			{
				from.BeginTarget( 5, false, TargetFlags.Harmful, new TargetCallback( OnTarget ) );
			}
		}

        public static bool IsThrowingNinjaThrowable( Mobile m )
        {
            return m_Table.Contains(m);
        }

		public void Shoot( Mobile from, Mobile target )
		{
			if ( from == target )
				return;

			if ( m_UsesRemaining < 1 )
			{
				// You have no fukiya darts!
				from.SendLocalizedMessage( 1063325 );
			}
            else if (IsThrowingNinjaThrowable(from))
			{
				// You are already using that fukiya.
				from.SendLocalizedMessage( 1063326 );
			}
			// 09.09.2013 :: mortuus - GM ninja moze uzywac fukija majac zajete rece
			else if ( !BasePotion.HasFreeHand( from ) && from.Skills[SkillName.Ninjitsu].Value<100 )
			{
				// You must have a free hand to use a fukiya.
				from.SendLocalizedMessage( 1063327 );
			}
			else if ( from.CanBeHarmful( target ) )
			{
				from.Direction = from.GetDirectionTo( target );

				from.RevealingAction();

				if ( from.Body.IsHuman && !from.Mounted )
					from.Animate( 33, 2, 1, true, true, 0 );

				from.PlaySound( 0x223 );
				from.MovingEffect( target, 0x2804, 5, 0, false, false );

				// 09.09.2013 :: mortuus - Szansa na trafienie Fukiya taka sama jak na uderzenie bronia, skill Ninja jako bojowka
				BaseWeapon defWeapon = target.Weapon as BaseWeapon;
				double atkSkillVal = from.Skills[SkillName.Ninjitsu].Value;
				double defSkillVal = defWeapon.GetDefendSkillValue( from, target );
				double chance = BaseWeapon.HitChance(atkSkillVal, defSkillVal, from, target);

				if ( from.CheckSkill( SkillName.Ninjitsu, chance ) )
					Timer.DelayCall( TimeSpan.FromSeconds( 1.0 ), new TimerStateCallback( OnDartHit ), new object[]{ from, target } );
				else
					ConsumeUse();

                AddUsing(from, 3.5);
			}
		}

		private void OnDartHit( object state )
		{
			object[] states = (object[])state;
			Mobile from = (Mobile)states[0];
			Mobile target = (Mobile)states[1];

			if ( !from.CanBeHarmful( target ) )
				return;

			from.DoHarmful( target );

			AOS.Damage( target, from, Utility.RandomMinMax( 4, 6 ), 100, 0, 0, 0, 0 );

			if ( m_Poison != null && m_PoisonCharges > 0 )
				target.ApplyPoison( from, m_Poison );

			ConsumeUse();
		}

		public void ConsumeUse()
		{
			if ( m_UsesRemaining < 1 )
				return;

			--m_UsesRemaining;

			if ( m_PoisonCharges > 0 )
			{
				--m_PoisonCharges;

				if ( m_PoisonCharges == 0 )
					m_Poison = null;
			}

			InvalidateProperties();
		}

        public static void AddUsing(Mobile m, double delay) 
        {
            Timer t = (Timer)m_Table[m];
            if (t != null)
                t.Stop();
            t = new InternalTimer(m, TimeSpan.FromSeconds(delay));
            m_Table[m] = t;
            t.Start();
        }

        public static void ResetUsing(Mobile m)
        {
            Timer t = (Timer)m_Table[m];

            if (t != null)
                t.Stop();

            m_Table.Remove(m);
        }

		private const int MaxUses = 10;

		public void Unload( Mobile from )
		{
			if ( UsesRemaining < 1 )
				return;

			FukiyaDarts darts = new FukiyaDarts( UsesRemaining );

			darts.Poison = m_Poison;
			darts.PoisonCharges = m_PoisonCharges;

			from.AddToBackpack( darts );

			m_UsesRemaining = 0;
			m_PoisonCharges = 0;
			m_Poison = null;

			InvalidateProperties();
		}

		public void Reload( Mobile from, FukiyaDarts darts )
		{
			int need = ( MaxUses - m_UsesRemaining );

			if ( need <= 0 )
			{
				// You cannot add anymore fukiya darts
				from.SendLocalizedMessage( 1063330 );
			}
			else if ( darts.UsesRemaining > 0 )
			{
				if ( need > darts.UsesRemaining )
					need = darts.UsesRemaining;

				if ( darts.Poison != null && darts.PoisonCharges > 0 )
				{
					if ( m_PoisonCharges <= 0 || m_Poison == null || m_Poison.Level <= darts.Poison.Level )
					{
						if ( m_Poison != null && m_Poison.Level < darts.Poison.Level )
							Unload( from );

						if ( need > darts.PoisonCharges )
							need = darts.PoisonCharges;

						if ( m_Poison == null || m_PoisonCharges <= 0 )
							m_PoisonCharges = need;
						else
							m_PoisonCharges += need;

						m_Poison = darts.Poison;

						darts.PoisonCharges -= need;

						if ( darts.PoisonCharges <= 0 )
							darts.Poison = null;

						m_UsesRemaining += need;
						darts.UsesRemaining -= need;
					}
					else
					{
						from.SendLocalizedMessage( 1070767 ); // Loaded projectile is stronger, unload it first
					}
				}
				else
				{
					m_UsesRemaining += need;
					darts.UsesRemaining -= need;
				}

				if ( darts.UsesRemaining <= 0 )
					darts.Delete();

				InvalidateProperties();
			}
		}

		public void OnTarget( Mobile from, object obj )
		{
			if ( Deleted || !IsChildOf( from ) )
				return;

			if ( obj is Mobile )
				Shoot( from, (Mobile) obj );
			else if ( obj is FukiyaDarts )
				Reload( from, (FukiyaDarts) obj );
			else
				from.SendLocalizedMessage( 1063329 ); // You can only load fukiya darts
		}

		public override void GetContextMenuEntries( Mobile from, List<ContextMenuEntry> list )
		{
			base.GetContextMenuEntries( from, list );

			if ( IsChildOf( from ) )
			{
				list.Add( new LoadEntry( this ) );
				list.Add( new UnloadEntry( this ) );
			}
		}

		private class LoadEntry : ContextMenuEntry
		{
			private Fukiya m_Fukiya;

			public LoadEntry( Fukiya fukiya ) : base( 6224, 0 )
			{
				m_Fukiya = fukiya;
			}

			public override void OnClick()
			{
				if ( !m_Fukiya.Deleted && m_Fukiya.IsChildOf( Owner.From ) )
					Owner.From.BeginTarget( 5, false, TargetFlags.Harmful, new TargetCallback( m_Fukiya.OnTarget ) );
			}
		}

		private class UnloadEntry : ContextMenuEntry
		{
			private Fukiya m_Fukiya;

			public UnloadEntry( Fukiya fukiya ) : base( 6225, 0 )
			{
				m_Fukiya = fukiya;

				Enabled = ( fukiya.UsesRemaining > 0 );
			}

			public override void OnClick()
			{
				if ( !m_Fukiya.Deleted && m_Fukiya.IsChildOf( Owner.From ) )
					m_Fukiya.Unload( Owner.From );
			}
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );

			writer.Write( (int) m_UsesRemaining );

			Poison.Serialize( m_Poison, writer );
			writer.Write( (int) m_PoisonCharges );
		}
		
		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 0:
				{
					m_UsesRemaining = reader.ReadInt();

					m_Poison = Poison.Deserialize( reader );
					m_PoisonCharges = reader.ReadInt();

					break;
				}
			}
		}

        public class InternalTimer : Timer
        {
            private Mobile m_Mobile;

            public InternalTimer(Mobile m, TimeSpan delay) : base(delay)
            {
                m_Mobile = m;
                Priority = TimerPriority.TwoFiftyMS;
            }

            protected override void OnTick()
            {
                Fukiya.ResetUsing(m_Mobile);
            }
        }
	}
}