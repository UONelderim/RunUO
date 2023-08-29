using System;
using Server;


namespace Server.Items
{
	public class ArcaneTunic : LeatherChest
	{
		public override int InitMinHits{ get{ return 60; } }
		public override int InitMaxHits{ get{ return 60; } }


		public override int LabelNumber{ get{ return 1061101; } } // Arcane Tunic 


		[Constructable]
		public ArcaneTunic()
		{
			Name = "Tunika Arkanisty z Thila";
			Hue = 0x556;
			Attributes.DefendChance = 10; 
			Attributes.CastSpeed = 1; 
			Attributes.LowerManaCost = 10;
			Attributes.LowerRegCost = 5; 
			Attributes.SpellDamage = 4;
		}


  //      public override void AddNameProperties(ObjectPropertyList list)
	//	{
    //        base.AddNameProperties(list);
	//		list.Add( 1070722, "Artifact");
     //   }


		public ArcaneTunic( Serial serial ) : base( serial )
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