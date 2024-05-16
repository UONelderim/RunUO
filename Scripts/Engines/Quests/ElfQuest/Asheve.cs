using System;
using Server.Items;
namespace Server.Mobiles
{
	[CorpseName( "zwloki mrocznego elfa" )]
	public class Vampy : BaseCreature
	{

		[Constructable]
		public Vampy() : base(AIType.AI_Mage, FightMode.Aggressor, 10, 1, 0.2, 0.4)
		{
			Name = "Ashive";
			Title = "- mroczna elfka";
			Body = 401;
			BaseSoundID = 0;
			Hue = 1102;
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
			
			Fame = 4500;
			Karma = -4500;
			
			VirtualArmor = 45;
			
			FancyShirt shirt = new FancyShirt();
			shirt.Hue = 1175;
			shirt.Name = "ashive's shirt";
			AddItem( shirt );

			

			Skirt skirt = new Skirt();
			skirt.Hue = 1175;
			skirt.Name = "ashive's skirt";
			AddItem( skirt );

			LeatherGloves gloves = new LeatherGloves();
			gloves.Hue = 1175;
			gloves.Name = "ashive's gloves";
			gloves.Movable = false;
			AddItem( gloves );

			Sandals shoe = new Sandals();
			shoe.Hue = 1260;
			shoe.Name = "ashive's sandals";
			AddItem( shoe );

			PackItem( new EvilHead());
			
		}
		
		public Vampy( Serial serial ) : base( serial )
		{
			
		}
		public override bool AlwaysMurderer{get{return true;}}
		public override void OnDoubleClick( Mobile from )
		{
			
			Mobile m = from;
			PlayerMobile mobile = m as PlayerMobile;
			
				Item p = from.FindItemOnLayer( Layer.Backpack );
				if ( p != null )
			{
			Item d = from.Backpack.FindItemByType( typeof(DarkDeed) );
			if ( d == null )
			{
			
				if( mobile != null )
				{
					if( from.Karma == 0 )
					{
						this.Say( "Jak widze nie zdecydowano jeszcze o Twojej sciezce zycia? Ah, przejdz na mroczna strone i przynies mi glowe elfiego szlachcica z Lotharn! Otrzymasz wielka nagrode." );
					}
					else if( from.Karma > 0 )
					{
						this.Say( "Kupo odchodow!! Dlaczegp nachodzisz mnie bedac po stronie tfuu swiatlosci? Ah, Rozumiem. Zmadrzales i chcesz przejsc na nasza strone?? Zatem przymies mi glowe elfiego szlachcica z Lotharn! Otrzymasz wielka nagrode." );
					}
					else if( from.Karma < 0 )
					{
						this.Say( "Podejdz do mnie Dziecko Ciemnosci. Loethe raduje sie. Czy zadowolisz nas oboje podrzynajac czyjes gardlo? Przynies mi glowe elfiego szlachcica z Lotharn a otrzymasz wielka nagrode." );
					}
				}
			}
			else
			{
				this.Say( "Odejdz, Nie potrzebuje juz Ciebie.");
			}
			}
			
		}
		
		
		
		public override bool OnDragDrop( Mobile from, Item dropped )
		{
			Item c = from.FindItemOnLayer( Layer.Backpack );
				if ( c != null )
			{
				
			
			Item a = from.Backpack.FindItemByType( typeof(DarkDeed) );
			if ( a == null )
			{
			
			
			if( from != null )
			{
			
			if( from.Karma < 0 )
			{
				if( dropped is LordsHead )
				{
					if( dropped.Amount == 1 )
					{
						from.AddToBackpack( new DarkBow() );

						
						this.Say( "Tak!!! Nareszcie!! Nasza Bogini w podzięce poderznie wiele gardel . Oto twoja nagroda" );
						
						dropped.Delete();
					}
				}
				else
				{
					this.Say( "Co ty chcesz mnie w ch..oszukac? To nie glowa szlachcica z Lotharn" );
				}
				
			}
			else if( from.Karma > 0 )
			{
				
				if( dropped is LordsHead )
				{
					if( dropped.Amount==1 )
					{
				from.AddToBackpack( new DarkBow());
				from.AddToBackpack( new DarkDeed());

				this.Say( "W podziece za Twoje uslugi masz ten oto Symbol Ciemnosci. Zatrzymaj go drogie dziecko!" );
				from.SendMessage( "Podazaj dalej za ciemnoscia! Niech swiatlo nie przeszkodzi w Twej drodze!" );
						dropped.Delete();
					}
				}
				else
				{
					this.Say( "Co ty chcesz mnie w ch..oszukac? To nie glowa szlachcica z Lotharn" );

				}

			}

			}
						else
			{
			this.Say( "Co ty chcesz mnie w ch..oszukac? To nie glowa szlachcica z Lotharn" );
			}
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
