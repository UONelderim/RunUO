using System;
using Server.Mobiles;

namespace Server.Mobiles
{
	[CorpseName( "zwloki kurczaka" )]
	public class Chicken : BaseCreature
	{
		[Constructable]
		public Chicken() : base( AIType.AI_Animal, FightMode.Aggressor, 12, 1, 0.2, 0.4 )
		{
			Name = "kurczak";
			Body = 0xD0;
			BaseSoundID = 0x6E;

			SetStr( 5 );
			SetDex( 15 );
			SetInt( 5 );

			SetHits( 3 );
			SetMana( 0 );

			SetDamage( 1 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 1, 5 );

			SetSkill( SkillName.MagicResist, 4.0 );
			SetSkill( SkillName.Tactics, 5.0 );
			SetSkill( SkillName.Wrestling, 5.0 );

			Fame = 150;
			Karma = 0;

			VirtualArmor = 2;

			Tamable = true;
			ControlSlots = 1;
			MinTameSkill = -0.9;

            // 07.01.2013 :: szczaw :: dodane - jaja do lootu
            Bird.AddEggs(this);

			DefecationTimer a = new DefecationTimer(this);
			a.Start();
		}

		public override int Meat{ get{ return 1; } }
		public override MeatType MeatType{ get{ return MeatType.Bird; } }
		public override FoodType FavoriteFood{ get{ return FoodType.GrainsAndHay; } }

		public override int Feathers{ get{ return 5; } }

		public Chicken(Serial serial) : base(serial)
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