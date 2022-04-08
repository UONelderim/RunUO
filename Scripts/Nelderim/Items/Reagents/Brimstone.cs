using System;
using Server;
using Server.Items;

namespace Server.Items
{
	public class Brimstone : BaseReagent, ICommodity
	{
		string ICommodity.Description
		{
			get
			{
				return String.Format( "{0} brimstone", Amount );
			}
		}

		int ICommodity.DescriptionNumber { get { return LabelNumber; } }

		[Constructable]
        public Brimstone() : this( 1 )
		{
		}

		[Constructable]
        public Brimstone( int amount ) : base( 3967, amount )
		{
		}

        public Brimstone( Serial serial ) : base( serial )
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