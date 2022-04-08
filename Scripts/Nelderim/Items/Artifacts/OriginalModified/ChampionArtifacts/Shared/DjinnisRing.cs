using System;
using Server;

namespace Server.Items
{
	public class DjinnisRing : SilverRing
	{
        public override int LabelNumber { get { return 1065835; } } // Pierscien Dijanniego

		public override int InitMinHits{ get{ return 60; } }
		public override int InitMaxHits{ get{ return 60; } }

		[Constructable]
		public DjinnisRing()
		{
			Hue = 1152;
			Attributes.BonusInt = 5;
			Attributes.SpellDamage = 10;
			Attributes.CastSpeed = 2;
			Attributes.CastRecovery = 1;
		}

		public DjinnisRing( Serial serial ) : base( serial )
		{
		}
		
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}
