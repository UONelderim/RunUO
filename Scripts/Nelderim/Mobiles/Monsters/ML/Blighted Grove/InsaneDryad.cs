using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "resztki szalonej driady" )]
	public class InsaneDryad : MLDryad
	{
		public override double DifficultyScalar{ get{ return 1.02; } }
		public override bool InitialInnocent{ get{ return false; } }

		[Constructable]
		public InsaneDryad() : base()
		{
			Name = "szalona driada";	
			Hue = 0x487;
			Karma = -10000;
		}
		
		public InsaneDryad( Serial serial ) : base( serial )
		{
		}
		
		public override void OnDeath( Container c )
		{
			base.OnDeath( c );		
						
			/*if ( Utility.RandomDouble() < 0.1 )				
				c.DropItem( new ParrotItem() );	*/
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
