using System;
using Server;
using Server.Items;

namespace Server.Items
{
    // Sól
	public class Salt : BaseReagent, ICommodity
	{
		string ICommodity.Description
		{
			get
			{
                if( Amount == 1 )
                    return String.Format( "{0} szczypta soli", Amount );

                int lastDigit = Amount % 10;
				if( lastDigit == 1 || lastDigit >= 5 )
                    return String.Format( "{0} szczypt soli", Amount );
                else
                    return String.Format( "{0} szczypty soli", Amount );
			}
		}

		int ICommodity.DescriptionNumber { get { return LabelNumber; } }

		[Constructable]
        public Salt() : this( 1 )
		{
		}

		[Constructable]
        public Salt( int amount ) : base( 3979, amount )
		{
            Name  = "szczypta soli";
            ItemID = 0x26B8;
            Hue = 1150;
		}

        public Salt( Serial serial ) : base( serial )
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