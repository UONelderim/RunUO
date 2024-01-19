using System;
using Server.Mobiles;

namespace Server.Mobiles
{
	[CorpseName( "zwloki swini" )]
	public class Pig : BaseCreature
	{
		[Constructable]
		public Pig() : base( AIType.AI_Animal, FightMode.Aggressor, 12, 1, 0.2, 0.4 )
		{
			Name = "swinia";
			Body = 0xCB;
			BaseSoundID = 0xC4;

			SetStr( 20 );
			SetDex( 20 );
			SetInt( 5 );

			SetHits( 12 );
			SetMana( 0 );

			SetDamage( 2, 4 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 10, 15 );

			SetSkill( SkillName.MagicResist, 5.0 );
			SetSkill( SkillName.Tactics, 5.0 );
			SetSkill( SkillName.Wrestling, 5.0 );

			Fame = 150;
			Karma = 0;

			VirtualArmor = 12;

			Tamable = true;
			ControlSlots = 1;
			MinTameSkill = 11.1;

			DefecationTimer a = new DefecationTimer(this);
			a.Start();
		}

		public override int Meat{ get{ return 3; } }
		public override FoodType FavoriteFood{ get{ return FoodType.FruitsAndVegies | FoodType.GrainsAndHay; } }

		public Pig(Serial serial) : base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write((int) 0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();

			DefecationTimer a = new DefecationTimer(this);
			a.Start();
		}
	}
}