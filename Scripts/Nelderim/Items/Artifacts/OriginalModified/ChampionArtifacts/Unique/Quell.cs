using System;
using Server;

namespace Server.Items
{
	public class Quell : Bardiche
	{
        public override int LabelNumber { get { return 1065842; } } // Zdlawiony
		public override int InitMinHits{ get{ return 60; } }
		public override int InitMaxHits{ get{ return 60; } }

		public override bool CanFortify{ get{ return true; } } //bylo false

		[Constructable]
		public Quell()
		{
			Hue = 0x225;

			//Attributes.SpellChanneling = 1;
			Attributes.WeaponSpeed = 20;
			Attributes.WeaponDamage = 50;
			Attributes.AttackChance = 10;

			WeaponAttributes.HitLeechMana = 50; //bylo 100
			WeaponAttributes.UseBestSkill = 1;
		}

		public Quell( Serial serial ) : base( serial )
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

			if ( Attributes.SpellChanneling == 1 )
				Attributes.SpellChanneling = 0;

			if ( WeaponAttributes.HitLeechMana != 50 )
				WeaponAttributes.HitLeechMana = 50;
		}
	}
}
