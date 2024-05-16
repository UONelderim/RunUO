using System;
using Server.Items;
namespace Server.Mobiles
{
	[CorpseName( "zwloki elfiego szlachcica" )]
	public class ElfLord : BaseCreature
	{

		[Constructable]
		public ElfLord() : base(AIType.AI_Mage, FightMode.Aggressor, 10, 1, 0.2, 0.4)
		{
			Name = "Gliroin";
			Title = "- elfi szlachcic";
			Body = 400;
			BaseSoundID = 0;
			Hue = 2414;
			SetStr( 555, 920 );
			SetDex( 350, 445 );
			SetInt( 476, 1000 );
			
			SetHits( 451, 999 );
			
			SetDamage( 15, 25 );
			
			SetDamageType( ResistanceType.Physical, 70, 100 );
			SetDamageType( ResistanceType.Poison, 30, 100 );
			
			SetResistance( ResistanceType.Physical, 20, 90 );
			SetResistance( ResistanceType.Fire, 20, 95 );
			SetResistance( ResistanceType.Cold, 20, 95 );
			SetResistance( ResistanceType.Poison, 20, 100 );
			SetResistance( ResistanceType.Energy, 20, 90 );
			
			SetSkill( SkillName.MagicResist, 70.0 );
			SetSkill( SkillName.Tactics, 90.0 );
			SetSkill( SkillName.Wrestling, 90.0 );
			
			Fame = -10000;
			Karma = 900000;
			
			VirtualArmor = 45;
			
			PackItem( new LordsHead() );
			
			FancyShirt shirt = new FancyShirt();
			shirt.Hue = 2127;
			shirt.Name = "Gliroin's shirt";
			AddItem( shirt );
			
			LeatherLegs skirt = new LeatherLegs();
			skirt.Hue = 2127;
			skirt.Name = "Gliroin's pants";
			AddItem( skirt );

			LeatherGloves gloves = new LeatherGloves();
			gloves.Hue = 2127;
			gloves.Name = "Gliroin's gloves";
			gloves.Movable = false;
			AddItem( gloves );

			Sandals shoe = new Sandals();
			shoe.Hue = 2127;
			shoe.Name = "Glirion's sandals";
			AddItem( shoe );
			
		}
		
		public ElfLord( Serial serial ) : base( serial )
		{
			
		}
		
		public override void OnDoubleClick( Mobile from )
		{
			Mobile m = from;
			PlayerMobile mobile = m as PlayerMobile;
				Item c = from.FindItemOnLayer( Layer.Backpack );
				if ( c != null )
			{
			Item d = from.Backpack.FindItemByType( typeof(LightDeed) );
			if ( d == null )
			{
			if( mobile != null )
			{
				if( from.Karma == 0 )
				{
				this.Say( "Wygladasz na mocnegp? Moj oponent Ashive to mroczna elfka , chce mojej smierci. Czy przyniesiesz mi jej glowe?" );
				}
				else if( from.Karma > 0 )
				{
				this.Say( "Dziecie swiatla, czy przybywasz by mi pomoc? Jesli tak to przynies mi glowe pewnej zlej istoty! Odszukaj Ashive mroczna elfke?" );
				}
				else if( from.Karma < 0 )
				{
					this.Say( "Ciemnosc widze, Ciemnosc. Dziecko ciemnosci czy chcesz przejsc na jasna strone? Jesli tak to przynies mi glowe pewnej zlej istoty! Odszukaj Ashive mroczna elfke?" );
				}
			}

		}
						else
			{
			this.Say( "Juz wykonales to zadanie. Dziekuje jeszcze raz" );
			}
				}
		}
		
		
		
		public override bool OnDragDrop( Mobile from, Item dropped )
		{
				Item p = from.FindItemOnLayer( Layer.Backpack );
				if ( p != null )
			{
			Item a = from.Backpack.FindItemByType( typeof(LightDeed) );
			if ( a == null )
			{
				
			if( from != null )
			{
			
			if( from.Karma < 0 )
			{
				if( dropped is EvilHead )
				{
					if( dropped.Amount == 1 )
					{
						from.AddToBackpack( new LightBow() );
						from.AddToBackpack( new LightDeed() );

						
						this.Say( "W podziece za wykonanie zadanie masz oto nagrode." );
						
						dropped.Delete();
					}
				}
				else
				{
					this.Say( "To nie jest wlasciwa glowa. Szukaj dalej" );
				}
				
			}
			else if( from.Karma > 0 )
			{
				
				if( dropped is EvilHead )
				{
					if( dropped.Amount==1 )
					{
				from.AddToBackpack( new LightBow());
				from.AddToBackpack( new LightDeed());

				this.Say( "Oto Twoja nagroda Dziecie Matki" );
				from.SendMessage( "Jestes sluga dobra!Odtad wypleniaj zlo z tego swiata" );
						dropped.Delete();
					}
				}
				else
				{
					this.Say( "To nie jest wlasciwa glowa. Szukaj dalej." );

				}

			}

		}
		}
						else
			{
			this.Say( "To nie jest wlasciwa glowa. Szukaj dalej" );
			}
				}
return false;
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
