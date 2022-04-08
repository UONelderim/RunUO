using System;
using Server;

namespace Server.Items
{
	public class ArcaneShield : WoodenKiteShield
	{
        public override int LabelNumber { get { return 1061101; } } // Arkana Obronne
		public override int InitMinHits{ get{ return 60; } }
		public override int InitMaxHits{ get{ return 60; } }

		[Constructable]
		public ArcaneShield()
		{
			ItemID = 0x1B78;
			Hue = 0x556;
			Attributes.NightSight = 1;
			Attributes.SpellChanneling = 1;
			Attributes.DefendChance = 15;
			Attributes.CastSpeed = 1;
		}

		public ArcaneShield( Serial serial ) : base( serial )
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

			if ( Attributes.NightSight == 0 )
				Attributes.NightSight = 1;
		}
	}
}