using System;
using Server.Items;
using Server.Mobiles;

namespace Server.Items
{
	public class StoneMiningBook : Item
	{
		// Koszt nauki wydobywania granitu:
		public static int Price { get{ return 10000; } }

		public static bool LearnStoneMining(Mobile from)
		{
			PlayerMobile pm = from as PlayerMobile;
			if ( pm == null || from.Skills[SkillName.Mining].Base < 100.0 )
			{
				from.SendMessage( "Aby zrozumiec tresc tej nauki musisz wpierw osiagnac mistrzostwo w gornictwie." );
				return false;
			}
			else if ( pm.StoneMining )
			{
				pm.SendMessage( "Posiadasz juz ta wiedze." );
				return false;
			}
			else
			{
				pm.StoneMining = true;
				pm.SendMessage( "Nauczyles sie pozyskiwac dobrej jakosci granit." );
				return true;
			}
		}

		public override string DefaultName
		{
			get { return "Wydobycie dobrej jakosci granitu"; }
		}

		[Constructable]
		public StoneMiningBook() : base( 0xFBE )
		{
			Weight = 1.0;
		}

		public StoneMiningBook( Serial serial ) : base( serial )
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
				if (LearnStoneMining(from))
				{
					Delete();
				}
			}
		}
	}
}