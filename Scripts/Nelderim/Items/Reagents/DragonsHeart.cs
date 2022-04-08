using System;
using Server;
using Server.Items;

namespace Server.Items
{

	public class RedDragonsHeart : BaseReagent, ICommodity
	{
		string ICommodity.Description
		{
			get
			{
				return String.Format( "{0} serce ognistego smoka", Amount );
			}
		}

		int ICommodity.DescriptionNumber { get { return LabelNumber; } }

		[Constructable]
        public RedDragonsHeart() : this( 1 )
		{
		}

		[Constructable]
        public RedDragonsHeart( int amount ) : base( 3985, amount )
		{
			Name = "Serce ognistego smoka";
			Hue = 1939;
		}

        public RedDragonsHeart( Serial serial ) : base( serial )
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

	public class BlueDragonsHeart : BaseReagent, ICommodity
	{
		string ICommodity.Description
		{
			get
			{
				return String.Format( "{0} serce lodowego smoka", Amount );
			}
		}

		int ICommodity.DescriptionNumber { get { return LabelNumber; } }

		[Constructable]
        public BlueDragonsHeart() : this( 1 )
		{
		}

		[Constructable]
        public BlueDragonsHeart( int amount ) : base( 3985, amount )
		{
			Name = "Serce lodowego smoka";
			Hue = 2150;
		}

        public BlueDragonsHeart( Serial serial ) : base( serial )
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

	public class DragonsHeart : BaseReagent, ICommodity
	{
		string ICommodity.Description
		{
			get
			{
				return String.Format( "{0} serce smoka", Amount );
			}
		}

		int ICommodity.DescriptionNumber { get { return LabelNumber; } }

		[Constructable]
        public DragonsHeart() : this( 1 )
		{
		}

		[Constructable]
        public DragonsHeart( int amount ) : base( 3985, amount )
		{
			Name = "Serce smoka";
			Hue = 1460;
		}

        public DragonsHeart( Serial serial ) : base( serial )
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