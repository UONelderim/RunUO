using System;
using Server;
using Server.Items;

namespace Server.Items
{
	public class DragonsBlood : BaseReagent, ICommodity
	{
		string ICommodity.Description
		{
			get
			{
				return String.Format( "{0} krew smoka", Amount );
			}
		}

		int ICommodity.DescriptionNumber { get { return LabelNumber; } }

		[Constructable]
        public DragonsBlood() : this( 1 )
		{
		}

		[Constructable]
        public DragonsBlood( int amount ) : base( 3970, amount )
		{
            Name = "Krew smoka";
		}

        public DragonsBlood( Serial serial ) : base( serial )
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