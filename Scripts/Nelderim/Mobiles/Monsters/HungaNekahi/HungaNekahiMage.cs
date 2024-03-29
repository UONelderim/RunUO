using System;
using Server;
using Server.Items;


namespace Server.Mobiles 
{ 
	public class HungaNekahiMage : BaseCreature 
	{ 
		public override double AttackMasterChance { get { return 0.2; } }
        public override double SwitchTargetChance { get { return 0.2; } }	
		
		[Constructable] 
		public HungaNekahiMage() : base( AIType.AI_BattleMage, FightMode.Weakest, 12, 6, 0.2, 0.4 ) 
		{ 

			Title = "- Nekahi Mag";
			Hue = Utility.RandomSkinHue();

			if ( this.Female = Utility.RandomBool() )
			{
				Body = 0x191;
				Name = NameList.RandomName( "female" );
			}
			else
			{
				Body = 0x190;
				Name = NameList.RandomName( "male" );
			}
			
			
			Cloak Cloa = new Cloak();
			Cloa.Hue = 33;
			Cloa.Movable = false;
			AddItem ( Cloa );
			
			Sandals Boot = new Sandals ();
			Boot.Hue = 33;
			AddItem ( Boot );
						
			FemaleElvenRobe rob = new FemaleElvenRobe();
			rob.Hue = 556;
			AddItem ( rob );
			rob.Movable = false;
	
			QuarterStaff staff = new QuarterStaff();
			staff.Attributes.SpellChanneling = 1;
			AddItem(staff);
			staff.Movable = false;
			
			SetStr( 250, 300 );
			SetDex( 80, 120 );
			SetInt( 260, 300 );

			SetHits( 260, 320 );

			SetDamage( 12, 14 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 35, 45 );
			SetResistance( ResistanceType.Fire, 35, 50 );
			SetResistance( ResistanceType.Cold, 35, 50 );
			SetResistance( ResistanceType.Poison, 35, 50 );
			SetResistance( ResistanceType.Energy, 30, 45 );

			SetSkill( SkillName.EvalInt, 100.0, 120.0 );
			SetSkill( SkillName.Magery, 100.0, 120.0 );
			SetSkill( SkillName.Meditation, 80.5, 90.0 );
			SetSkill( SkillName.MagicResist, 90.0, 110.0 );
			SetSkill( SkillName.Tactics, 90.0, 100.0 );
			SetSkill( SkillName.Wrestling, 90.0, 100.0 );

			Fame = 8000;
			Karma = -8000;

			VirtualArmor = 30;

			HairItemID = 0x203C;
			HairHue = Utility.RandomHairHue();
			
			PackReg( 15, 20 );

		}


		public override void GenerateLoot()
		{
			AddLoot( LootPack.Rich, 2 );
			AddLoot( LootPack.Gems, 1 );	
		}

		public override bool ShowFameTitle{ get{ return true; } }
		public override bool AutoDispel{ get{ return true; } }
		public override bool AlwaysMurderer{ get{ return true; } }
		public override bool CanRummageCorpses{ get{ return true; } }
		public override Poison PoisonImmune{ get{ return Poison.Greater; } }
		public override int TreasureMapLevel{ get{ return 1; } }
		
		public HungaNekahiMage( Serial serial ) : base( serial ) 
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
