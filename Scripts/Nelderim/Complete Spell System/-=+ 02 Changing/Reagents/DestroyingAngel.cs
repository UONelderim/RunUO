using System;
using Server;

namespace Server.Items
{
	public class DestroyingAngel : BaseReagent, ICommodity
	{
		string ICommodity.Description
		{
			get
			{
				return String.Format( "{0} niszczejący anioł", Amount );
			}
		}

        int ICommodity.DescriptionNumber { get { return LabelNumber; } }

		[Constructable]
		public DestroyingAngel() : this( 1 )
		{
		}

		[Constructable]
		public DestroyingAngel( int amount ) : base( 0xE1F, amount )
		{
			Hue = 0x290;
			Name = "niszczejący anioł";
		}

		public DestroyingAngel( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 1 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}