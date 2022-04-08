using System;
using Server;

namespace Server.Items
{
	public class MidnightBracers : BoneArms
	{
        public override int LabelNumber { get { return 1061093; } } // Grobowe Kajdany
        public override int InitMinHits { get { return 50; } }
        public override int InitMaxHits { get { return 50; } }

		public override int BasePhysicalResistance{ get{ return 23; } }

		[Constructable]
		public MidnightBracers()
		{
			Hue = 0x455;
			SkillBonuses.SetValues( 0, SkillName.Necromancy, 20.0 );
			Attributes.SpellDamage = 10;
			ArmorAttributes.MageArmor = 1;
		}

		public MidnightBracers( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 );
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			if ( version < 1 )
				PhysicalBonus = 0;
		}
	}
}