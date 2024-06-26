using System;
using Server.Mobiles;

namespace Server.Mobiles
{
	[CorpseName( "zwloki krowy" )]
	public class Cow : BaseCreature
	{
		[Constructable]
		public Cow() : base( AIType.AI_Animal, FightMode.Aggressor, 12, 1, 0.2, 0.4 )
		{
			Name = "krowa";
			Body = Utility.RandomList( 0xD8, 0xE7 );
			BaseSoundID = 0x78;

			SetStr( 30 );
			SetDex( 15 );
			SetInt( 5 );

			SetHits( 18 );
			SetMana( 0 );

			SetDamage( 1, 4 );

			SetDamage( 1, 4 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 5, 15 );

			SetSkill( SkillName.MagicResist, 5.5 );
			SetSkill( SkillName.Tactics, 5.5 );
			SetSkill( SkillName.Wrestling, 5.5 );

			Fame = 300;
			Karma = 0;

			VirtualArmor = 10;

			Tamable = true;
			ControlSlots = 1;
			MinTameSkill = 11.1;

			if ( Core.AOS && Utility.Random( 1000 ) == 0 ) // 0.1% chance to have mad cows
				FightMode = FightMode.Closest;

			DefecationTimer a = new DefecationTimer(this);
			a.Start();
		}

		public override int Meat{ get{ return 6; } }
		public override int Hides{ get{ return 4; } }
		public override FoodType FavoriteFood{ get{ return FoodType.FruitsAndVegies | FoodType.GrainsAndHay; } }

		public override void OnDoubleClick( Mobile from )
		{
			base.OnDoubleClick( from );

			int random = Utility.Random( 100 );

			if ( random < 5 )
				Tip();
			else if ( random < 20 )
				PlaySound( 120 );
			else if ( random < 40 )
				PlaySound( 121 );
		}

		public void Tip()
		{
			PlaySound( 121 );
			Animate( 8, 0, 3, true, false, 0 );
		}

		public Cow(Serial serial) : base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write((int) 1);
			writer.Write( DateTime.MinValue ); //MilkedOn
			writer.Write( (int)0 ); //Milk
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();
            switch ( version )
            {
				case 1:
                {
					reader.ReadDateTime(); //MilkedOn
					reader.ReadInt(); //Milk
					break;
                }
            }

			DefecationTimer a = new DefecationTimer(this);
			a.Start();
		}
	}
}