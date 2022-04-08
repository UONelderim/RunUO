using System;
using Server.Items;
using Server.Mobiles;

namespace Server.Items
{
	public class MasonryBook : Item
	{
		// Koszt nauki kamieniarstwa:
		public static int Price { get{ return 10000; } }

		public static bool LearnStonecrafting(Mobile from)
		{
			PlayerMobile pm = from as PlayerMobile;
			if ( pm == null || from.Skills[SkillName.Carpentry].Base < 100.0 )
			{
				pm.SendMessage( "Aby zrozumiec tresc tej nauki musisz wpierw osiagnac mistrzostwo w stolarstwie." );
				return false;
			}
			else if ( pm.Masonry )
			{
				pm.SendMessage( "Posiadasz juz ta wiedze." );
				return false;
			}
			else
			{
				pm.Masonry = true;
				pm.SendMessage( "Nauczyles sie wytwarzac przedmioty z granitu. Materialu do produkcji dostarczyc moga gornicy." );
				return true;
			}
		}

		public override string DefaultName
		{
			get { return "Rzezbienie w kamieniu"; }
		}

		[Constructable]
		public MasonryBook() : base( 0xFBE )
		{
			Weight = 1.0;
		}

		public MasonryBook( Serial serial ) : base( serial )
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

		public override void OnDoubleClick( Mobile from )
		{
			if ( !IsChildOf( from.Backpack ) )
			{
				from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
			}
			else 
			{
				if (LearnStonecrafting(from))
				{
					Delete();
				}
			}
		}
	}
}