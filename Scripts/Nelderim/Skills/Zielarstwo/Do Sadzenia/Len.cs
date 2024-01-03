using System;
using Server.Network;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Targets;
using Server.Items;
using Server.Targeting;
using Server.Spells;
using Server.Mobiles;

namespace Server.Items.Crops
{
	// TODO: ustawic dodatkowy skill krawiectwo i zwiekszyc progi umozliwiajace zbieranie

	public class SzczepkaLen : SeedWarzywo
    {
        public override Type PlantType => typeof(KrzakLen);

        [Constructable]
		public SzczepkaLen( int amount ) : base( amount, 6946 ) 
		{
			Hue = 51;
			Name = "Ziarno lnu";
			Stackable = true;
		}

		[Constructable]
		public SzczepkaLen() : this( 1 )
		{
		}

		public SzczepkaLen( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int)0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
	
	public class KrzakLen : PlantWarzywo
    {
        public override Type SeedType => typeof(SzczepkaLen);
        public override Type CropType => typeof(Flax);

        [Constructable] 
		public KrzakLen() : base( 6811 )
		{ 
			Hue = 0;
			Name = "Krzak lnu";
			Stackable = true;
        }

		public KrzakLen( Serial serial ) : base( serial ) 
		{
		}

		public override void Serialize( GenericWriter writer ) 
		{ 
			base.Serialize( writer ); 
			writer.Write( (int) 0 ); 
		} 

		public override void Deserialize( GenericReader reader ) 
		{ 
			base.Deserialize( reader ); 
			int version = reader.ReadInt(); 
		} 
	}

	public class PlonLen : CropWarzywo
	{
		public override Type ReagentType => typeof(Flax);

		[Constructable]
		public PlonLen(int amount) : base(amount, 6809)
		{
			Hue = 0;
			Name = "Lodyga lnu";
			Stackable = true;
		}

		[Constructable]
		public PlonLen() : this(1)
		{
		}

		public PlonLen(Serial serial) : base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write((int)0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadInt();
		}
	}


}