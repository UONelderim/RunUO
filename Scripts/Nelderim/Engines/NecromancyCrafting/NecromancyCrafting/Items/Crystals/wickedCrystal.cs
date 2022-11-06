using System;
using Server;
using Server.Mobiles;
using Server.Spells;

namespace Server.Items
{
	public class  wickedCrystal : Item
	{
		public override string DefaultName
		{
			get { return "Pokrecony krysztal"; }
		}

		[Constructable]
		public  wickedCrystal() : base( 0x1F19 )
		{
			Weight = 1.0;
			Hue = 0x489;
		}

		public  wickedCrystal( Serial serial ) : base( serial )
		{
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( !IsChildOf( from.Backpack ) )
			{
				from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
				return;
			}

			double NecroSkill = from.Skills[SkillName.Necromancy].Value;

			if ( NecroSkill < 50.0 )
			{
				from.SendMessage( "Musisz miec 50 umiejetnosci nekromancji, by stworzyc mumie." );
				return;
			}

			double scalar;

			if ( NecroSkill >= 100.0 )
				scalar = 2.4;
			else if ( NecroSkill >= 90.0 )
				scalar = 2.0;
			else if ( NecroSkill >= 80.0 )
				scalar = 1.9;
			else if ( NecroSkill >= 70.0 )
				scalar = 1.6;
			else
				scalar = 1.0;

			Container pack = from.Backpack;

			if ( pack == null )
				return;

			int res = pack.ConsumeTotal(
				new Type[]
				{
					typeof( WrappedBod ),
					typeof( WrappedLegs )
				},
				new int[]
				{
					1,
					1
				} );

			switch ( res )
			{
				case 0:
				{
					from.SendMessage( "Musisz mieć zmumifikowany tułów." );
					break;
				}
				case 1:
				{
					from.SendMessage( "Musisz mieć zmumifikowane nogi." );
					break;
				}
				default:
				{
					PesantMummy g = new PesantMummy( true, scalar );

					if ( g.SetControlMaster( from ) )
					{
						Delete();

						g.MoveToWorld( from.Location, from.Map );
						from.PlaySound( 0x241 );
					}

					break;
				}
			}
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