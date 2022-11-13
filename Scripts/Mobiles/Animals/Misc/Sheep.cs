using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Network;

namespace Server.Mobiles
{
	[CorpseName( "zwloki owcy" )]
	public class Sheep : BaseCreature, ICarvable
	{
		private DateTime m_NextWoolTime;

		[CommandProperty( AccessLevel.GameMaster )]
		public DateTime NextWoolTime
		{
			get{ return m_NextWoolTime; }
			set{ m_NextWoolTime = value; Body = ( DateTime.Now >= m_NextWoolTime ) ? 0xCF : 0xDF; }
		}

		public void Carve( Mobile from, Item item )
		{
			if ( DateTime.Now < m_NextWoolTime )
			{
				// This sheep is not yet ready to be shorn.
				PrivateOverheadMessage( MessageType.Regular, 0x3B2, 500449, from.NetState );
				return;
			}

			from.SendLocalizedMessage( 500452 ); // You place the gathered wool into your backpack.
            
            int amount = 3;

            double factor = 1.0 + BaseCreature.campingCarveBonus(from.Skills[SkillName.Camping].Value);
            amount = (int)Math.Round(amount*factor, MidpointRounding.AwayFromZero);

			from.AddToBackpack( new Wool( amount ) );
			if ( 0.1 > (new Random()).NextDouble())
			{
                from.AddToBackpack(new GoldenWool(1));
            }

			NextWoolTime = DateTime.Now + TimeSpan.FromHours( 1.0 ); // Zmiana z 3h na 1h
		}

		public override void OnThink()
		{
			base.OnThink();
			Body = ( DateTime.Now >= m_NextWoolTime ) ? 0xCF : 0xDF;
		}

		[Constructable]
		public Sheep() : base( AIType.AI_Animal, FightMode.Aggressor, 12, 1, 0.2, 0.4 )
		{
			Name = "owca";
			Body = 0xCF;
			BaseSoundID = 0xD6;

			SetStr( 19 );
			SetDex( 25 );
			SetInt( 5 );

			SetHits( 12 );
			SetMana( 0 );

			SetDamage( 1, 2 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 5, 10 );

			SetSkill( SkillName.MagicResist, 5.0 );
			SetSkill( SkillName.Tactics, 6.0 );
			SetSkill( SkillName.Wrestling, 5.0 );

			Fame = 300;
			Karma = 0;

			VirtualArmor = 6;

			Tamable = true;
			ControlSlots = 1;
			MinTameSkill = 11.1;
		}

		public override int Meat{ get{ return 1; } }
		public override MeatType MeatType{ get{ return MeatType.LambLeg; } }
		public override FoodType FavoriteFood{ get{ return FoodType.FruitsAndVegies | FoodType.GrainsAndHay; } }

		public override int Wool{ get{ return (Body == 0xCF ? 3 : 0); } }

		public Sheep( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 );

			writer.WriteDeltaTime( m_NextWoolTime );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 1:
				{
					NextWoolTime = reader.ReadDeltaTime();
					break;
				}
			}
		}
	}
}