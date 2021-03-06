using System;
using Server;
using Server.Items;

namespace Server.Items
{
	public class WyrmsHeart : BaseReagent, ICommodity
	{
		string ICommodity.Description
		{
			get
			{
				return String.Format( "{0} wyrm's heart", Amount );
			}
		}

		int ICommodity.DescriptionNumber { get { return LabelNumber; } }

		[Constructable]
        public WyrmsHeart() : this( 1 )
		{
		}

		[Constructable]
        public WyrmsHeart( int amount ) : base( 3985, amount )
		{
		}

        public WyrmsHeart( Serial serial ) : base( serial )
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