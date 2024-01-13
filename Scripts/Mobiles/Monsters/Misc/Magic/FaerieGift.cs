using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	
	[CorpseName( "zwloki Athrad Math" )]
	public class ZmiennyDar : BaseCreature
	{
		
		[Constructable]
		public ZmiennyDar () : base( AIType.AI_BattleMage, FightMode.Weakest, 12, 1, 0.2, 0.4 )
		{
			Name = "Athrad Math";
			Body = 0x9e;
			Hue = 2978;
			BaseSoundID = 1084;

			SetStr( 430, 500);
			SetDex( 130, 180 );
			SetInt( 200, 280 );

			SetHits( 348, 470 );

			SetDamage( 17, 24);

			SetDamageType( ResistanceType.Poison, 75 );
			SetDamageType( ResistanceType.Energy, 20 );
			SetDamageType( ResistanceType.Physical, 5 );

			SetResistance( ResistanceType.Physical, 65, 80 );
			SetResistance( ResistanceType.Fire, 45, 70 );
			SetResistance( ResistanceType.Cold, 40 );
			SetResistance( ResistanceType.Poison, 67, 90 );
			SetResistance( ResistanceType.Energy, 55, 88 );


			SetSkill( SkillName.Meditation, 95.1, 110.0 );
			SetSkill( SkillName.Poisoning, 110.1, 120.0 );
			SetSkill( SkillName.MagicResist, 99.1, 100.0 );
			SetSkill( SkillName.Tactics, 90.1, 100.0 );
			SetSkill( SkillName.Wrestling, 120 );
			SetSkill( SkillName.EvalInt, 30, 70 );
			SetSkill( SkillName.Magery, 90, 104 );

			Fame = 15000;
			Karma = 15000;

			VirtualArmor = 80;

			Tamable = true;
			ControlSlots = 4;
			MinTameSkill = 106;
			
			var hueChangeTimer = new HueChangeTimer(this);
			hueChangeTimer.Start();
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.FilthyRich, 1 );
			AddLoot( LootPack.Gems, 5 );
			PackReg( 5, 10 );
			PackReg( 5, 10 );
		}
		
		public override void AddWeaponAbilities()
        {
            WeaponAbilities.Add( WeaponAbility.BleedAttack, 0.25 );
            WeaponAbilities.Add( WeaponAbility.ForceOfNature, 0.05 );
            WeaponAbilities.Add( WeaponAbility.TalonStrike, 0.10 );
            WeaponAbilities.Add( WeaponAbility.Feint, 0.05 );
        }
        private class HueChangeTimer : Timer
		{
			private readonly ZmiennyDar creature;

			public HueChangeTimer(ZmiennyDar creature) : base(TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(30))
			{
				this.creature = creature;
				Priority = TimerPriority.FiveSeconds;
			}

			protected override void OnTick()
			{
				if(creature.Deleted){
				  Stop();
				  }
				
				int[] hueValues = { 2978, 2415, 2978, 2978, 2978, 2978 };
				int currentIndex = Array.IndexOf(hueValues, creature.Hue);
				int nextIndex = (currentIndex + 1) % hueValues.Length;
				creature.Hue = hueValues[nextIndex];
			}
		}
		

		public override int TreasureMapLevel{ get{ return 3; } }
		public override int Meat{ get{ return 10; } }
		public override int Hides{ get{ return 10; } }
		public override HideType HideType{ get{ return HideType.Barbed; } }
		public override FoodType FavoriteFood{ get{ return FoodType.Meat; } }
		public override bool BardImmune{ get{ return false; } } 
		public override Poison PoisonImmune{ get{ return Poison.Deadly; } }		
		public override Poison HitPoison{ get{ return Poison.Deadly; } }
		public ZmiennyDar( Serial serial ) : base( serial )
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