using System;
using Server.Helpers;
using Server.Network;

namespace Server.Items
{
	public class SkelBod : Item
	{
		public override string DefaultName
		{
			get { return "Tułów szkieleta"; }
		}

		[Constructable]
		public SkelBod() : base( 0x1D91 )
		{
			Weight = 1.0;
			Stackable = true;
		}

		public SkelBod( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
			
			this.ReplaceWith(new SkeletonTorso{ Amount = Amount });
		}
	}
}