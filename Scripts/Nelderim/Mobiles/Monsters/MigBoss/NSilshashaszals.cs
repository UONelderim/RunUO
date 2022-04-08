using System;
using System.Collections;
using Server;
using Server.Items;
using Server.Engines.CannedEvil;

namespace Server.Mobiles
{
	[CorpseName( "zwloki silshashaszalsa" )]
	public class NSilshashaszals : BaseCreature
	{
		public override bool BardImmune{ get{ return false; } }
		public override int TreasureMapLevel{ get{ return 5; } }
        public override double AttackMasterChance { get { return 0.15; } }
        public override double SwitchTargetChance { get { return 0.15; } }
		public override Poison PoisonImmune { get { return Poison.Greater; } }
		public override Poison HitPoison { get { return Poison.Lesser; } }

	    public override void AddWeaponAbilities()
        {
            WeaponAbilities.Add( WeaponAbility.MortalStrike, 0.4 );
        }
		
		[Constructable]
		public NSilshashaszals () : base( AIType.AI_Melee, FightMode.Closest, 12, 1, 0.25, 0.5 )
		{
			Body = 36;
			Hue = 2150;
			Name = "silshashaszals - krol jaszczuroludzi";

			BaseSoundID = 417;

			SetStr( 250, 300 );
			SetDex( 280, 300 );
			SetInt( 300, 320 );
			SetHits( 3000, 5000	);

			SetDamage( 18, 22 );

			SetDamageType( ResistanceType.Physical, 50 );
			SetDamageType( ResistanceType.Energy, 50 );

			SetResistance( ResistanceType.Physical, 45, 55 );
			SetResistance( ResistanceType.Fire, 40, 50 );
			SetResistance( ResistanceType.Cold, 25, 35 );
			SetResistance( ResistanceType.Poison, 25, 35 );
			SetResistance( ResistanceType.Energy, 25, 35 );

			SetSkill( SkillName.MagicResist, 120.7, 140.0 );
			SetSkill( SkillName.Tactics, 80.0, 90.0 );
			SetSkill( SkillName.Wrestling, 80.0, 90.0 );
			SetSkill( SkillName.EvalInt, 80.0, 90.0 );
			SetSkill( SkillName.Magery, 80.0, 90.0 );

			Fame = 22500;
			Karma = -22500;

			VirtualArmor = 80;
			
			AddItem( new LightSource() );
				
			// minory: 24
			/*int szansa=0; 
			int art = Utility.Random( 24 ); 
			int artis = Utility.Random( 100 ); 
			if (artis<8) szansa=1; 
			if (szansa>0) 
			{
			if (art<1)
			PackItem( new AlchemistsBauble() );
			else if (art<2)
			PackItem( new ArcticDeathDealer() );
			else if (art<3)
			PackItem( new BlazeOfDeath() );
			else if (art<4)
			PackItem( new BowOfTheJukaKing() );
			else if (art<5)
			PackItem( new BurglarsBandana() ); 
			else if (art<6)
			PackItem( new CaptainQuacklebushsCutlass() ); 
			else if (art<7)
			PackItem( new CavortingClub() );
			else if (art<8)
			PackItem( new ColdBlood() );
			else if (art<9)
			PackItem( new DreadPirateHat() );
			else if (art<10)
			PackItem( new EnchantedTitanLegBone() );
			else if (art<11)
			PackItem( new GlovesOfThePugilist() );
			else if (art<12)
			PackItem( new GwennosHarp() );
			else if (art<13)
			PackItem( new HeartOfTheLion() );
			else if (art<14)
			PackItem( new IolosLute() );
			else if (art<15)
			PackItem( new LunaLance() );
			else if (art<16)
			PackItem( new NightsKiss() );
			else if (art<17)
			PackItem( new NoxRangersHeavyCrossbow() );
			else if (art<18)
			PackItem( new OrcishVisage() );
			else if (art<19)
			PackItem( new PixieSwatter() );
			else if (art<20)
			PackItem( new PolarBearMask() );
			else if (art<21)
			PackItem( new ShieldOfInvulnerability() );
			else if (art<22)
			PackItem( new StaffOfPower() );
			else if (art<23)
			PackItem( new VioletCourage() );
			else 
			PackItem( new WrathOfTheDryad() );
			}
			
			//doom steal: 4 + doom:  28
			int szansa2=0; 
			int art2 = Utility.Random( 32 ); 
			int artis2 = Utility.Random( 100 ); 
			if (artis2<1) szansa2=1; 
			if (szansa2>0) 
			{
			if (art2<1)
			PackItem( new BladeOfTheRighteous() );
			else if (art2<2)
			PackItem( new TitansHammer() );
			else if (art2<3)
			PackItem( new ZyronicClaw() );
			else if (art2<4)
			PackItem( new InquisitorsResolution() );
			else if (art2<5)
			PackItem( new ArcaneShield() );
			else if (art2<6)
			PackItem( new ArmorOfFortune() );
			else if (art<7)
			PackItem( new AxeOfTheHeavens() );
			else if (art2<8)
			PackItem( new BladeOfInsanity() );
			else if (art2<9)
			PackItem( new BoneCrusher() );
			else if (art2<10)
			PackItem( new BraceletOfHealth() );
			else if (art2<11)
			PackItem( new BreathOfTheDead() );
			else if (art2<12)
			PackItem( new Frostbringer() );
			else if (art2<13)
			PackItem( new GauntletsOfNobility() );
			else if (art2<14)
			PackItem( new HelmOfInsight() );
			else if (art2<15)
			PackItem( new HolyKnightsBreastplate() );
			else if (art2<16)
			PackItem( new JackalsCollar() );
			else if (art2<17)
			PackItem( new LeggingsOfBane() );
			else if (art2<18)
			PackItem( new LegacyOfTheDreadLord() );
			else if (art2<19)
			PackItem( new MidnightBracers() );
			else if (art2<20)
			PackItem( new OrnamentOfTheMagician() );
			else if (art2<21)
			PackItem( new OrnateCrownOfTheHarrower() );
			else if (art2<22)
			PackItem( new RingOfTheElements() );
			else if (art2<23)
			PackItem( new RingOfTheVile() );
			else if (art2<24)
			PackItem( new ShadowDancerLeggings() );
			else if (art2<25)
			PackItem( new SerpentsFang() );
			else if (art2<26)
			PackItem( new StaffOfTheMagi() );
			else if (art2<27)
			PackItem( new TheBeserkersMaul() );
			else if (art2<28)
			PackItem( new TheDragonSlayer() );
			else if (art2<29)
			PackItem( new TheDryadBow() );
			else if (art2<30)
			PackItem( new TheTaskmaster() );
			else if (art2<31)
			PackItem( new TunicOfFire() );
			else 
			PackItem( new VoiceOfTheFallenKing() );
			
			}
			*/


		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.AosSuperBoss );
			// 07.01.2013 :: szczaw :: usuniecie PackGold
			//PackGold(800, 1000 );
		}
public override void OnDeath( Container c )
		{
			base.OnDeath( c );

            ArtifactHelper.ArtifactDistribution(this);
		}
		public NSilshashaszals( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}
