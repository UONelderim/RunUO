using System;
using Server;
using Server.Network;

namespace Server.Items
{
	public class GiantSpikeTrap : BaseTrap
	{
        [Constructable]
		public GiantSpikeTrap() : this( 0 )
		{
		}

		[Constructable]
		public GiantSpikeTrap( int level ) : base( 0x1B71, level )
		{
		}

		public override bool PassivelyTriggered{ get{ return false; } }
		public override TimeSpan PassiveTriggerDelay{ get{ return TimeSpan.Zero; } }
		public override int PassiveTriggerRange{ get{ return 3; } }
		public override TimeSpan ResetDelay{ get{ return TimeSpan.FromSeconds( 0.0 ); } }

		public override void OnTrigger( Mobile from )
		{
			if ( from.AccessLevel > AccessLevel.Player )
				return;

			Effects.SendLocationEffect( Location, Map, 0x1D99, 48, 2, GetEffectHue(), 0 );
            Effects.PlaySound( GetWorldLocation(), Map, 0x22D );

			if ( from.Alive && CheckRange( from.Location, 0 ) )
				AOS.Damage( from, Damage, true, 100, 0, 0, 0, 0 );
		}

		public GiantSpikeTrap( Serial serial ) : base( serial )
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