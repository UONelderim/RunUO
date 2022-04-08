using System;
using Server;

namespace Server.Items
{
	public class Pacify : Pike
	{
        public override int LabelNumber { get { return 1065843; } } // Pokoj
		public override int InitMinHits{ get{ return 60; } }
		public override int InitMaxHits{ get{ return 60; } }

		public override bool CanFortify{ get{ return true; } } //bylo false

		[Constructable]
		public Pacify()
		{
			Hue = 0x835;

			//Attributes.SpellChanneling = 1;
			Attributes.AttackChance = 10;
			Attributes.WeaponSpeed = 20;
			Attributes.WeaponDamage = 50;

			WeaponAttributes.HitLeechMana = 50; //bylo 100
			WeaponAttributes.UseBestSkill = 1;
		}

		public Pacify( Serial serial ) : base( serial )
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
