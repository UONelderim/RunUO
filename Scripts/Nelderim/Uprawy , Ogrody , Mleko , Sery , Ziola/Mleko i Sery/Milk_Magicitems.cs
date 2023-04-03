using System;

namespace Server.Items
{
	public abstract class BaseMagicCheese : Item
	{

		public BaseMagicCheese( int hue ) : base( 0x97E )
		{
			Weight = 1.0;
			// Hue = hue;  they're all alike - yellowish

			if ( Utility.RandomBool() )
				Hue = Utility.RandomList(0x135, 0xcd, 0x38, 0x3b, 0x42, 0x4f, 0x11e, 0x60, 0x317, 0x10, 0x136, 0x1f9, 0x1a, 0xeb, 0x86, 0x2e, 0x0497, 0x0481); 
			else
				Hue = 3 + (Utility.Random( 20 ) * 5);


		}
		
		public BaseMagicCheese( Serial serial ) : base( serial )
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
	
	public class FromageDeChevreMagic : BaseMagicCheese
	{
		public override void OnDoubleClick( Mobile from )
		{
			if ( !IsChildOf( from.Backpack ) )
			{
				from.SendLocalizedMessage( 1042038 ); // You must have the object in your backpack to use it.
			}
			else if ( from.GetStatMod( "[Ser] STR" ) != null )
			{
				from.SendLocalizedMessage( 1062927 ); // You have eaten one of these recently and eating another would provide no benefit.
			}
			else
			{
				from.PlaySound( 0x1EE );
				from.AddStatMod( new StatMod( StatType.Str, "[Ser] STR", 5, TimeSpan.FromMinutes( 5.0 ) ) );

				Consume();
			}
		}
		
		//public override int LabelNumber{ get{ return 1041073; } } // prized fish
		
		[Constructable]
		public FromageDeChevreMagic() : base( 151 )
		{
			this.Name = "crottin de Chavignol (magiczny owczy ser)";
		}
		
		public FromageDeChevreMagic( Serial serial ) : base( serial )
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
	public class FromageDeVacheMagic : BaseMagicCheese
	{
		public override void OnDoubleClick( Mobile from )
		{
			if ( !IsChildOf( from.Backpack ) )
			{
				from.SendLocalizedMessage( 1042038 ); // You must have the object in your backpack to use it.
			}
			else if ( from.GetStatMod( "[Ser] INT" ) != null )
			{
				from.SendLocalizedMessage( 1062927 ); // You have eaten one of these recently and eating another would provide no benefit.
			}
			else
			{
				from.PlaySound( 0x1EE );
				from.AddStatMod( new StatMod( StatType.Int, "[Ser] INT", 5, TimeSpan.FromMinutes( 5.0 ) ) );

				Consume();
			}
		}
		
		//public override int LabelNumber{ get{ return 1041073; } } // prized fish
		
		[Constructable]
		public FromageDeVacheMagic() : base( 151 )
		{
			this.Name = "Maroille (magiczny krowi ser)";
		}
		
		public FromageDeVacheMagic( Serial serial ) : base( serial )
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
	
	public class FromageDeBrebisMagic : BaseMagicCheese
	{
		public override void OnDoubleClick( Mobile from )
		{
			if ( !IsChildOf( from.Backpack ) )
			{
				from.SendLocalizedMessage( 1042038 ); // You must have the object in your backpack to use it.
			}
			else if ( from.GetStatMod( "[Ser] DEX" ) != null )
			{
				from.SendLocalizedMessage( 1062927 ); // You have eaten one of these recently and eating another would provide no benefit.
			}
			else
			{
				from.PlaySound( 0x1EE );
				from.AddStatMod( new StatMod( StatType.Dex, "[Ser] DEX", 5, TimeSpan.FromMinutes( 5.0 ) ) );

				Consume();
			}
		}
		
		//public override int LabelNumber{ get{ return 1041073; } } // prized fish
		
		[Constructable]
		public FromageDeBrebisMagic() : base( 151 )
		{
			this.Name = "Roquefort (magiczny owczy ser)";
		}
		
		public FromageDeBrebisMagic( Serial serial ) : base( serial )
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
