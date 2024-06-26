using System;
using System.Collections;
using Server.Items;
using Server.Targeting;
using Server.Misc;

namespace Server.Mobiles
{
	[CorpseName( "zwloki wilczego lowcy" )]
	public class WolfHunter : BaseCreature
	{
        public override void AddWeaponAbilities()
        {
            WeaponAbilities.Add( WeaponAbility.BleedAttack, 0.4 );
        }

		[Constructable]
		public WolfHunter() : base( AIType.AI_Melee, FightMode.Closest, 12, 1, 0.2, 0.4 )
		{
			Body = 0x190;
			Name = "wilczy lowca";
			Hue = 2940;
	

			SetStr( 351, 400 );
			SetDex( 50, 70 );
			SetInt( 151, 200 );

			SetHits( 341, 400 );

			SetDamage( 15, 18 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 40, 50 );
			SetResistance( ResistanceType.Fire, 45, 50 );
			SetResistance( ResistanceType.Cold, 40, 50 );
			SetResistance( ResistanceType.Poison, 20, 25 );
			SetResistance( ResistanceType.Energy, 40, 50 );

			SetSkill( SkillName.Anatomy, 90.1, 100.0 );
			SetSkill( SkillName.Healing, 80.1, 100.0 );
			SetSkill( SkillName.MagicResist, 120.1, 130.0 );
			SetSkill( SkillName.Fencing, 90.1, 100.0 );
			SetSkill( SkillName.Tactics, 95.1, 100.0 );
			SetSkill( SkillName.Wrestling, 90.1, 100.0 );
			SetSkill( SkillName.Parry, 90.1, 100.0 );

			Fame = 5000;
			Karma = -5000;

                	

	            LeatherCap Helm = new LeatherCap ();
				Helm.Hue = 2707;
				Helm.LootType = LootType.Blessed;
				Helm.ItemID = 5445;
				Helm.Name = "wilcza maska";
				AddItem( Helm ); 
			StuddedChest Chest = new StuddedChest ();
				Chest.Hue = 2707;
				Chest.LootType = LootType.Blessed;
				AddItem ( Chest ); 
			StuddedGorget Gorget = new StuddedGorget ();
				Gorget.Hue = 2707;
				Gorget.LootType = LootType.Blessed;
				AddItem( Gorget );
			BoneLegs Legs = new BoneLegs ();
				Legs.Hue = 2707;
				Legs.LootType = LootType.Blessed;
				AddItem ( Legs );
			BoneArms Arms = new BoneArms ();
				Arms.Hue = 2707;
				Arms.LootType = LootType.Blessed;
				AddItem( Arms );



			Cloak Cloa = new Cloak();
				Cloa.Hue = 2707;
				Cloa.LootType = LootType.Blessed;
				AddItem ( Cloa );
			Kilt Pants = new Kilt();
				Pants.Hue = 2707;
				Pants.LootType = LootType.Blessed;
				AddItem ( Pants );

			Tekagi Szpony = new Tekagi();
				Szpony.Name = "Szpony";
				AddItem ( Szpony );
			
			new FrenziedOstard().Rider = this;
				Hue = 33885;

			HairItemID = 0x203C;
			HairHue = Utility.RandomHairHue();


			VirtualArmor = 48;

			Container pack = new Backpack();


			pack.DropItem( new Bandage( Utility.RandomMinMax( 5, 10 ) ) );
			pack.DropItem( new Bandage( Utility.RandomMinMax( 5, 10 ) ) );
			pack.DropItem( Loot.RandomGem() );
		}


		public override void GenerateLoot()
		{
			AddLoot( LootPack.FilthyRich );
			AddLoot( LootPack.Meager, 2 );
		
		}
        public override double AttackMasterChance { get { return 0.15; } }
		public override bool AlwaysMurderer{ get{ return true; } }
		public override bool AutoDispel{ get{ return true; } } 
		public override bool ShowFameTitle{ get{ return true; } }
		public override bool CanRummageCorpses{ get{ return true; } }
		public override Poison PoisonImmune{ get{ return Poison.Greater; } }


		public WolfHunter( Serial serial ) : base( serial )
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
