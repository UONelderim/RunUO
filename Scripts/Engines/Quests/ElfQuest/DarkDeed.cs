using System;
using Server.Mobiles;
using Server.Items;

namespace Server.Items
{
	public class DarkDeed : Item
	{
		[Constructable]
		public DarkDeed()
		{
			ItemID = 5153;
			Weight = 1.0;
			Name = "Certyfikat Misji";
			Movable = true;
		}

		public DarkDeed( Serial serial ) : base( serial )
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
			if ( IsChildOf( from.Backpack ) )
				{
				 if ( from.Karma > 100 )
				 	{
				 	from.SendMessage( "Czyste dobro w Twoim sercu niszczy to w twoich rekach");
				 	this.Delete();
					}
				 else if ( from.Karma < 100 )
					{
						from.SendMessage( "Twoja dusza jest juz czarna. Zachowaj to !!!" );
					}
				
			else
			{
				from.SendMessage("Musisz miec przedmiot w plecaku by go uzyc");
			}
			}
		}
	}
}


