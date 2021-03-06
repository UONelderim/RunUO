using System;
using Server;
using Server.Items;

namespace Server.Items
{
	public class Pumice : BaseReagent, ICommodity
	{
		string ICommodity.Description
		{
			get
			{
				return String.Format( "{0} pumeks", Amount );
			}
		}

		int ICommodity.DescriptionNumber { get { return LabelNumber; } }

		[Constructable]
        public Pumice() : this( 1 )
		{
		}

		[Constructable]
        public Pumice( int amount ) : base( 3979, amount )
		{
            Name = "Pumeks";
		}

        public Pumice( Serial serial ) : base( serial )
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