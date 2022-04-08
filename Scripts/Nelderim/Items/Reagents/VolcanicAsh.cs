using System;
using Server;
using Server.Items;

namespace Server.Items
{
	public class VolcanicAsh : BaseReagent, ICommodity
	{
		string ICommodity.Description
		{
			get
			{
                return String.Format("{0} pyl wulkaniczny", Amount);
			}
		}

		int ICommodity.DescriptionNumber { get { return LabelNumber; } }

		[Constructable]
        public VolcanicAsh() : this( 1 )
		{
		}

		[Constructable]
        public VolcanicAsh( int amount ): base( 3983, amount )
		{
            Name = "Pyl wulkaniczny";
            Hue = 2105;
		}

		public VolcanicAsh( Serial serial ) : base( serial )
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