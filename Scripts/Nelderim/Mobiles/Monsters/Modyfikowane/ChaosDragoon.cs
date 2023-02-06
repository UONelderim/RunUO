using System;
using Server;
using Server.Misc;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "zwloki smoczego wojownika" )]
	public class ChaosDragoon : BaseCreature
	{
		//private static int m_MinTime = 5;
		//private static int m_MaxTime = 10;
		//private DateTime m_NextAbilityTime;

		[Constructable]
		public ChaosDragoon() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.15, 0.4 )
		{
			//AtackMasterChance = 0.25;
			
			Title = "- wojownik";
			Hue = Utility.RandomSkinHue();

			if ( this.Female = Utility.RandomBool() )
			{
				Body = 0x191;
				Name = NameList.RandomName( "female" );
				EquipItem( new Skirt( Utility.RandomNeutralHue() ) );
			}
			else
			{
				Body = 0x190;
				Name = NameList.RandomName( "male" );
				EquipItem( new ShortPants( Utility.RandomNeutralHue() ) );
			}

			SetStr( 176, 225 );
			SetDex( 81, 95 );
			SetInt( 61, 85 );

			SetDamage( 24, 26 );

			SetDamageType( ResistanceType.Physical, 25 );
			SetDamageType( ResistanceType.Fire, 25 );
			SetDamageType( ResistanceType.Cold, 25 );
			SetDamageType( ResistanceType.Energy, 25 );

			SetResistance( ResistanceType.Physical, 45, 55 );
			SetResistance( ResistanceType.Fire, 15, 25 );
			SetResistance( ResistanceType.Cold, 50 );
			SetResistance( ResistanceType.Poison, 25, 35 );
			SetResistance( ResistanceType.Energy, 25, 35 );

			SetSkill( SkillName.Anatomy, 77.6, 87.5 );
			SetSkill( SkillName.MagicResist, 77.6, 97.5 );
			SetSkill( SkillName.Tactics, 77.6, 87.5 );

			SetSkill( SkillName.Fencing, 77.6, 92.5 );

			Fame = 5000;
			Karma = -5000;

			InitOutfit( this );

			SwampDragon mount = new SwampDragon();

			mount.ControlMaster = this;
			mount.Controlled = true;
			mount.InvalidateProperties();
			
			mount.Rider = this;
			
			
			EquipItem( new Shirt( Utility.RandomDyedHue() ) );
		}

		public static void InitOutfit( Mobile m )
		{
			CraftResource res = CraftResource.None;

			switch (Utility.Random( 6 ))
			{
				case 0: res = CraftResource.BlackScales; break;
				case 1: res = CraftResource.RedScales; break;
				case 2: res = CraftResource.BlueScales; break;
				case 3: res = CraftResource.YellowScales; break;
				case 4: res = CraftResource.GreenScales; break;
				case 5: res = CraftResource.WhiteScales; break;
			}

			BaseWeapon melee = new Kryss();

			DragonHelm helm = new DragonHelm();
			helm.Name = "exceptional iron sea serpent scale helm";
			helm.Resource = res;
			helm.Movable = false;
			m.EquipItem( helm );

			ChainChest chest = new ChainChest();
			chest.Name = "exceptional iron sea serpent scale tunic";
			chest.Resource = res;
			chest.Movable = false;
			m.EquipItem( chest );

			DragonArms arms = new DragonArms();
			arms.Name = "exceptional iron sea serpent scale arms";
			arms.Resource = res;
			arms.Movable = false;
			m.EquipItem( arms );

			DragonGloves gloves = new DragonGloves();
			gloves.Name = "exceptional iron sea serpent scale gloves";
			gloves.Resource = res;
			gloves.Movable = false;
			m.EquipItem( gloves );

			DragonLegs legs = new DragonLegs();
			legs.Name = "exceptional iron sea serpent scale legs";
			legs.Resource = res;
			legs.Movable = false;
			m.EquipItem( legs );

			melee.Movable = false;
			m.EquipItem( melee );

			ChaosShield shield = new ChaosShield();
			shield.Movable = false;
			m.EquipItem( shield );

			Shoes shoes = new Shoes( Utility.RandomNeutralHue() );
			shoes.Movable = false;
			m.EquipItem( shoes );

			int amount = Utility.RandomMinMax( 1, 3 );

			BaseCreature bc = m as BaseCreature;
			
			if ( bc == null )
				return;
			
			switch ( res )
			{
				case CraftResource.BlackScales: bc.PackItem( new BlackScales( amount ) ); break;
				case CraftResource.RedScales: bc.PackItem( new RedScales( amount ) ); break;
				case CraftResource.BlueScales: bc.PackItem( new BlueScales( amount ) ); break;
				case CraftResource.YellowScales: bc.PackItem( new YellowScales( amount ) ); break;
				case CraftResource.GreenScales: bc.PackItem( new GreenScales( amount ) ); break;
				case CraftResource.WhiteScales: bc.PackItem( new WhiteScales( amount ) ); break;
			}
		}

		public static void StompAttack( Mobile target )
		{
			StatType type;

			if ( target.GetStatMod("[Stomp Attack]") != null) return;

			switch( Utility.Random(3) )
			{
			default:
			case 0: type = StatType.Str; break;
			case 1: type = StatType.Dex; break;
			case 2: type = StatType.Int; break;
			}

			string name = "[Stomp Attack]";

			int offset = 10 + Utility.Random( 10 );

			int duration = 60 + Utility.Random( 60 );

			target.AddStatMod( new StatMod( type, name, -offset, TimeSpan.FromSeconds( duration) ) );
		}

		public override void GenerateLoot()
		{
			AddItem( new Gold( 250, 300 ) );
			AddLoot( LootPack.Gems, 1 );
		}

		public override bool AlwaysMurderer{ get{ return true; } }
		public override bool ShowFameTitle{ get{ return false; } }
		public override bool HasBreath{ get{ return true; } }

		public ChaosDragoon( Serial serial ) : base( serial )
		{
		}

		/*
		public override void OnGaveMeleeAttack( Mobile defender )
		{
			base.OnGaveMeleeAttack( defender );
			
			if ( DateTime.Now >= m_NextAbilityTime )
			{
				StompAttack( defender );

				m_NextAbilityTime = DateTime.Now + TimeSpan.FromSeconds( Utility.RandomMinMax( m_MinTime, m_MaxTime ) );
			}
		}
		*/

		public override bool OnBeforeDeath()
		{
			if ( Mount != null )
				Mount.Rider = null;

			return base.OnBeforeDeath();
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
