using System;
using System.Collections;
using Server;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;
using Server.Spells;

namespace Server.Items
{
	public class PumpkinBomb : Item, ICarvable
	{
		private PumpkinTimer m_Timer;

		[Constructable]
		public PumpkinBomb() : base( 0xC6A )
		{
			Movable = false;
			Name = "Wybuchajaca Dyniowa Glowa";

			m_Timer = new PumpkinTimer( this );
			m_Timer.Start();
		}

		public void Carve( Mobile from, Item item )
		{
			Effects.PlaySound( GetWorldLocation(), Map, 0x48F );
			Effects.SendLocationEffect( GetWorldLocation(), Map, 0x3728, 10, 10, 0, 0 );

			if ( 0.3 > Utility.RandomDouble() )
			{

				from.SendMessage( "Niszczysz dynie." );

				Gold gold = new Gold( 2, 10 );

				gold.MoveToWorld( GetWorldLocation(), Map );

				Delete();

				m_Timer.Stop();
			}
			else
			{
				from.SendMessage( "Niszczysz dynie." );
			}
		}

		public PumpkinBomb( Serial serial ) : base( serial )
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

			m_Timer = new PumpkinTimer( this );
			m_Timer.Start();
		}

		private class PumpkinTimer : Timer
		{
			private Item m_Item;

			public PumpkinTimer( Item item ) : base( TimeSpan.FromSeconds( Utility.RandomMinMax( 2, 6 ) ) )
			{
				Priority = TimerPriority.FiftyMS;

				m_Item = item;
			}

			protected override void OnTick()
			{
				if ( m_Item.Deleted )
					return;
					
				Map map = m_Item.Map;
				if ( map == null )
					return;

				ArrayList list = new ArrayList();					

				foreach (Mobile mob in m_Item.GetMobilesInRange(2))  
				{
					if ( mob == null || mob.Deleted || mob.Map != map || !mob.InRange( m_Item, 2 ) || !mob.Alive || mob.IsDeadBondedPet )
						continue;
						
					if ( mob is BaseCreature && (((BaseCreature)mob).Controlled || ((BaseCreature)mob).Summoned) )
						list.Add( mob );
					else if ( mob.Player )
						list.Add( mob );
				}
			
				Effects.SendLocationParticles( EffectItem.Create( m_Item.Location, m_Item.Map, EffectItem.DefaultDuration ), 0x36BD, 20, 10, 5044 );
				Effects.PlaySound( m_Item.Location, m_Item.Map, 0x307);
						
				for ( int i = 0; i < list.Count; ++i )
				{
					Mobile mob = (Mobile)list[i];
					
					AOS.Damage( mob, mob, Utility.RandomMinMax( 5, 15 ), 0, 0, 0, 0, 100 );
					
					if ( mob.Alive && mob.Body.IsHuman && !mob.Mounted )
						mob.Animate( 20, 7, 1, true, false, 0 ); // take hit
				}

				m_Item.Delete();
			}
		}
	}
}