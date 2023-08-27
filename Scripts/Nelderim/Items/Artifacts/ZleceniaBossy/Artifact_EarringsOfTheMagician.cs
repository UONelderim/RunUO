using System;
using Server;


namespace Server.Items
{
	public class EarringsOfTheMagician : GoldEarrings
	{
		public override int LabelNumber{ get{ return 1061105; } } // Earrings of the Magician
		
		public override int InitMinHits{ get{ return 50; } }
		public override int InitMaxHits{ get{ return 50; } }


		[Constructable]
		public EarringsOfTheMagician()
		{
			Name = "Kolczyki Maga";
			Hue = 0x554;
			Attributes.CastRecovery = 1;
			Attributes.Luck = -200;
			Resistances.Energy = 15;
		}
		

		public EarringsOfTheMagician( Serial serial ) : base( serial )
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