using System;
using Server;

namespace Server.Items
{
	public class LieutenantOfTheBritannianRoyalGuard : BodySash
	{
        public override int LabelNumber { get { return 1065831; } } // Znamie Kapitana
		public override int InitMinHits{ get{ return 10; } }
		public override int InitMaxHits{ get{ return 10; } }

		public override bool CanFortify{ get{ return true; } }

		[Constructable]
		public LieutenantOfTheBritannianRoyalGuard()
		{
			Hue = 0xe8;

			Attributes.BonusInt = 5;
			Attributes.RegenMana = 2;
			Attributes.LowerRegCost = 10;
		}

		public LieutenantOfTheBritannianRoyalGuard( Serial serial ) : base( serial )
		{
		}
		
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}
