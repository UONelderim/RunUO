using System;
using Server.Items;
using Server.Mobiles;

namespace Server.Items
{
	public class GlassblowingBook : Item
	{
		// Koszt nauki dmuchania szkla:
		public static int Price { get{ return 10000; } }

		public static bool LearnGlassblowing(Mobile from)
		{
			PlayerMobile pm = from as PlayerMobile;
			if ( pm == null || from.Skills[SkillName.Alchemy].Base < 100.0 )
			{
				pm.SendMessage( "Aby zrozumiec tresc tej nauki musisz wpierw osiagnac mistrzostwo w alchemii." );
				return false;
			}
			else if ( pm.Glassblowing )
			{
				pm.SendMessage( "Posiadasz juz ta wiedze." );
				return false;
			}
			else
			{
				pm.Glassblowing = true;
				pm.SendMessage( "Nauczyles sie wytwarzac szklane przedmioty z piasku. Materialu do produkcji dostarczyc moga gornicy." );
				return true;
			}
		}

		public override string DefaultName
		{
			get { return "Jak wytwarzac szklo"; }
		}

		[Constructable]
		public GlassblowingBook() : base( 0xFF4 )
		{
			Weight = 1.0;
		}

		public GlassblowingBook( Serial serial ) : base( serial )
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
				if (LearnGlassblowing(from))
				{
					Delete();
				}
			}
		}
	}
}