using System;
using Server;
using Server.Items;

namespace Server.Items
{
	public class Bloodspawn : BaseReagent, ICommodity
	{
		string ICommodity.Description
		{
			get
			{
				return String.Format( "{0} serce demona", Amount );
			}
		}

		int ICommodity.DescriptionNumber { get { return LabelNumber; } }

		[Constructable]
        public Bloodspawn() : this( 1 )
		{
		}

		[Constructable]
        public Bloodspawn( int amount ) : base( 3964, amount )
		{
			Name = "Serce demona";
		}

        public Bloodspawn( Serial serial ) : base( serial )
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