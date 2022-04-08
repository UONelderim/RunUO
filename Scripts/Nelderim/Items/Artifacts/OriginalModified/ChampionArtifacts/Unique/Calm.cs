using System;
using Server;

namespace Server.Items
{
	public class Calm : Halberd
	{
        public override int LabelNumber { get { return 1065848; } } // Spokoj
		public override int InitMinHits{ get{ return 60; } }
		public override int InitMaxHits{ get{ return 60; } }

		public override bool CanFortify{ get{ return true; } }

		[Constructable]
		public Calm()
		{
			Hue = 0x2cb;

			//Attributes.SpellChanneling = 1;
			Attributes.WeaponSpeed = 20;
			Attributes.WeaponDamage = 50;

			WeaponAttributes.HitLeechMana = 50;
			WeaponAttributes.UseBestSkill = 1;
		}

		public Calm( Serial serial ) : base( serial )
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
			if (WeaponAttributes.HitLeechMana != 50)
			WeaponAttributes.HitLeechMana = 50;
			
			if (Attributes.SpellChanneling != 0)
			Attributes.SpellChanneling = 0;
		}
	}
}
