using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "zwloki ognistego smoka" )]
	public class OgnistySmok : BaseCreature
	{
		[Constructable]
		public OgnistySmok () : base( AIType.AI_BattleMage, FightMode.Weakest, 12, 1, 0.2, 0.4 )
		{
			Name = "ognisty smok";

			Body = 59;
			BaseSoundID = 362;
			Hue = Utility.RandomList(0, 1568, 1569, 1570, 1571, 1572, 2586);

			SetStr( 796, 825 );
			SetDex( 86, 105 );
			SetInt( 436, 475 );

			SetHits( 450, 520 );

			SetDamage( 15, 21 );

			SetDamageType( ResistanceType.Physical, 30 );
			SetDamageType( ResistanceType.Fire, 70 );

			SetResistance( ResistanceType.Physical, 55, 65 );
			SetResistance( ResistanceType.Fire, 80, 90 );
			SetResistance( ResistanceType.Cold, 20, 35 );
			SetResistance( ResistanceType.Poison, 30, 40 );
			SetResistance( ResistanceType.Energy, 30, 40 );

            SetSkill(SkillName.EvalInt, 99.1, 100.0);
            SetSkill(SkillName.Magery, 99.1, 100.0);
            SetSkill(SkillName.MagicResist, 99.1, 100.0);
            SetSkill(SkillName.Tactics, 97.6, 100.0);
            SetSkill(SkillName.Wrestling, 90.1, 100.0);

			Fame = 15000;
			Karma = -15000;

			VirtualArmor = 60;

			Tamable = true;
			ControlSlots = 3;
			MinTameSkill = 96.9;
		}
		
		public override void OnCarve(Mobile from, Corpse corpse, Item with)
		{
            if (!IsBonded && !corpse.Carved && !IsChampionSpawn)
            {
			    if( Utility.RandomDouble() < 0.05 )
				    corpse.DropItem( new RedDragonsHeart() );
			    if( Utility.RandomDouble() < 0.10 )
				    corpse.DropItem( new DragonsBlood() );
				if ( Utility.RandomDouble() < 0.10 )
					corpse.DropItem( new VolcanicAsh() );
            }

			base.OnCarve(from, corpse, with);
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.FilthyRich, 2 );

			AddLoot( LootPack.Gems, 2 );
		}
		
		public override void AddWeaponAbilities()
		{
			WeaponAbilities.Add( WeaponAbility.DoubleStrike, 0.222 );
		}

        public override double AttackMasterChance { get { return 0.25; } }
		public override bool HasBreath{ get{ return true; } } // fire breath enabled
		public override bool AutoDispel{ get{ return true; } }
		public override int TreasureMapLevel{ get{ return 4; } }
		public override int Meat{ get{ return 8; } }
		public override int Hides{ get{ return 12; } }
		public override HideType HideType{ get{ return HideType.Horned; } }
		public override int Scales{ get{ return 5; } }
		public override ScaleType ScaleType{ get{ return ( Body == 12 ? ScaleType.Yellow : ScaleType.Red ); } }
		public override FoodType FavoriteFood{ get{ return FoodType.Meat; } }

		public OgnistySmok( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}
