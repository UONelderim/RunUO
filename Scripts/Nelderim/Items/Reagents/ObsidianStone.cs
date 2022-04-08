using System;
using Server;
using Server.Items;

namespace Server.Items
{
	public class ObsidianStone : BaseReagent, ICommodity
	{
		string ICommodity.Description
		{
			get
			{
				return String.Format( "{0} obsidian stone", Amount );
			}
		}

		int ICommodity.DescriptionNumber { get { return LabelNumber; } }

		[Constructable]
        public ObsidianStone() : this( 1 )
		{
		}

		[Constructable]
        public ObsidianStone( int amount ) : base( 3977, amount )
		{
		}

        public ObsidianStone( Serial serial ) : base( serial )
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