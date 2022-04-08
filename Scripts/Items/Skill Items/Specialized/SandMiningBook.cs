using System;
using Server.Items;
using Server.Mobiles;

namespace Server.Items
{
	public class SandMiningBook : Item
	{
		// Koszt nauki wydobywania piasku:
		public static int Price { get{ return 10000; } }

		public static bool LearnSandMining(Mobile from)
		{
			PlayerMobile pm = from as PlayerMobile;
			if ( pm == null || from.Skills[SkillName.Mining].Base < 100.0 )
			{
				pm.SendMessage( "Aby zrozumiec tresc tej nauki musisz wpierw osiagnac mistrzostwo w gornictwie." );
				//Only a Grandmaster Miner can learn from this book.
				return false;
			}
			else if ( pm.SandMining )
			{
				pm.SendMessage( "Posiadasz juz ta wiedze." );
				//You have already learned this information
				return false;
			}
			else
			{
				pm.SandMining = true;
				pm.SendMessage( "Nauczyles sie pozyskiwac dobrej jakosci piasek. Wskaz piaszczysty teren podczas kopania, aby wydobyc piasek." );
				//You have learned how to mine fine sand. Target sand areas when mining to look for fine sand
				return true;
			}
		}

		public override string DefaultName
		{
			get { return "Wydobycie dobrej jakosci piasku"; }
		}

		[Constructable]
		public SandMiningBook() : base( 0xFF4 )
		{
			Weight = 1.0;
		}

		public SandMiningBook( Serial serial ) : base( serial )
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
				if (LearnSandMining(from))
				{
					Delete();
				}
			}
		}
	}
}