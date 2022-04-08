using System;
using Server.Items;
using Server.Network;

namespace Server.Items
{
	[Flipable( 0x2328, 0x2329 )]
	public class Snowman : Item, IDyable
	{
		public static string GetRandomTitle()
		{
			// All hail OSI staff
			string[] titles = new string[]
				{
					/*  1 */ "Iwanov",
					/*  2 */ "Maupishon",
					/*  3 */ "Halrand",
					/*  4 */ "jurijuice",
					/*  5 */ "Krull",
					/*  6 */ "Rainhart",
					/*  7 */ "M4edhros",
					/*  8 */ "kaczy",
					/*  9 */ "Levy",
					
				};

			if ( titles.Length > 0 )
				return titles[Utility.Random( titles.Length )];

			return null;
		}

		private string m_Title;

		[CommandProperty( AccessLevel.GameMaster )]
		public string Title
		{
			get{ return m_Title; }
			set{ m_Title = value; InvalidateProperties(); }
		}

		[Constructable]
		public Snowman() : this( Utility.RandomDyedHue(), GetRandomTitle() )
		{
		}

		[Constructable]
		public Snowman( int hue ) : this( hue, GetRandomTitle() )
		{
		}

		[Constructable]
		public Snowman( string title ) : this( Utility.RandomDyedHue(), title )
		{
		}

		[Constructable]
		public Snowman( int hue, string title ) : base( Utility.Random( 0x2328, 2 ) )
		{
			Weight = 10.0;
			Hue = hue;
			LootType = LootType.Blessed;
			Name = "bałwanek";

			m_Title = title;
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );

			if ( m_Title != null )
				list.Add( 1062841, m_Title ); // ~1_NAME~ the Snowman
		}

		public bool Dye( Mobile from, DyeTub sender )
		{
			if ( Deleted )
				return false;

			Hue = sender.DyedHue;

			return true;
		}

		public Snowman( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 ); // version

			writer.Write( (string) m_Title );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 1:
				{
					m_Title = reader.ReadString();
					break;
				}
			}

			Utility.Intern( ref m_Title );
		}
	}
}