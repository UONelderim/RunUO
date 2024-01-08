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
	// TODO: mozliwe jest uzycie umiejetnosci TworzenieLukow, zatem mozna zwiekszyc progi umozliwiajace zbieranie
	public class ZrodloJedwab : WeedPlantZbieractwo
    {
        public override Type CropType => typeof(SurowiecJedwab);

        public static SkillName[] silkSkills = new SkillName[] { SkillName.Zielarstwo, SkillName.Fletching };
        public override SkillName[] SkillsRequired { get{ return silkSkills; } }

		public override bool GivesSeed{ get{ return false; } }

		[Constructable] 
		public ZrodloJedwab() : base( Utility.Random(3153, 4) )
		{ 
			Hue = 1946;
			Name = "Roslina z jedwabnikiem"; // 1032611
			Stackable = true;
		}

		public ZrodloJedwab( Serial serial ) : base( serial ) 
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
	
	public class SurowiecJedwab : WeedCropZbieractwo
	{
		public override int DefaulReagentCount(Mobile m) => 12;
		public override Type ReagentType => typeof(SilkFiber);
		public override SkillName[] SkillsRequired { get { return ZrodloJedwab.silkSkills; } }

		[Constructable]
		public SurowiecJedwab( int amount ) : base( amount, 0x0DF9 ) //0x0DEF
		{
			Hue = 2886;
			Name = "Kokon jedwabiu"; // 1032614
			Stackable = true;
		}

		[Constructable]
		public SurowiecJedwab() : this( 1 )
		{
		}

		public SurowiecJedwab( Serial serial ) : base( serial )
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
    
    public class SilkFiber : Item
    {
        [Constructable]
		public SilkFiber( int amount ) : base( 3166 )
		{
            Stackable = true;
            Amount = amount;
			Hue = 1150; // 2796
			Name = "Jedwabne wlokno"; // 1032617
            Weight = 0.15;
		}

        [Constructable]
		public SilkFiber() : this(1)
		{
        }

		public SilkFiber( Serial serial ) : base( serial )
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


}