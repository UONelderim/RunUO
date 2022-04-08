using System;
using Server;
using Server.Gumps;
using Server.Network;

namespace Server.Engines.BulkOrders
{
	public class SmallBODAcceptGump : Gump
	{
		private SmallBOD m_Deed;
		private Mobile m_From;

		public SmallBODAcceptGump( Mobile from, SmallBOD deed ) : base( 50, 50 )
		{
			m_From = from;
			m_Deed = deed;

			m_From.CloseGump( typeof( LargeBODAcceptGump ) );
			m_From.CloseGump( typeof( SmallBODAcceptGump ) );

			AddPage( 0 );

			AddBackground( 25, 10, 430, 290, 5054 );

			AddImageTiled( 33, 20, 413, 275, 2624 );
			AddAlphaRegion( 33, 20, 413, 275 );

			AddImage( 20, 5, 10460 );
			AddImage( 430, 5, 10460 );
			AddImage( 20, 275, 10460 );
			AddImage( 430, 275, 10460 );

			AddHtmlLocalized( 190, 25, 120, 20, 1045133, 0x7FFF, false, false ); // A bulk order
			AddHtmlLocalized( 40, 48, 350, 20, 1045135, 0x7FFF, false, false ); // Ah!  Thanks for the goods!  Would you help me out?

			AddHtmlLocalized( 40, 72, 210, 20, 1045138, 0x7FFF, false, false ); // Amount to make:
			AddLabel( 250, 72, 1152, deed.AmountMax.ToString() );

			AddHtmlLocalized( 40, 96, 120, 20, 1045136, 0x7FFF, false, false ); // Item requested:
			if (deed.Graphic > 2)
				AddItem( 385, 96, deed.Graphic );
			AddHtmlLocalized( 40, 120, 210, 20, deed.Number, 0xFFFFFF, false, false );

			if ( deed.RequireExceptional || deed.Material != BulkMaterialType.None || deed.Material2 != BulkMaterialType.None)
			{
				AddHtmlLocalized( 40, 144, 210, 20, 1045140, 0x7FFF, false, false ); // Special requirements to meet:

				int nextY = 168;

				if (deed.RequireExceptional) {
					AddHtmlLocalized(40, nextY, 350, 20, 1045141, 0x7FFF, false, false); // All items must be exceptional.
					nextY += 24;
				}

				if (deed.Material != BulkMaterialType.None) {
					AddHtmlLocalized(40, nextY, 350, 20, SmallBODGump.GetMaterialNumberFor(deed.Material), 0x7FFF, false, false); // All items must be made with x material.
					nextY += 24;
				}

				if (deed.Material2 != BulkMaterialType.None) {
					AddHtmlLocalized(40, nextY, 350, 20, SmallBODGump.GetMaterialNumberFor(deed.Material2), 0x7FFF, false, false); // All items must be made with x material.
				}
			}
			else if (deed.Level > 0)
			{
				AddHtmlLocalized( 40, 144, 210, 20, 1063511, 0x7FFF, false, false ); // Poziom trudnosci zlecenia:
				AddLabel( 260, 144, 1152, deed.Level.ToString() );
			}

			AddHtmlLocalized( 40, 240, 350, 20, 1045139, 0x7FFF, false, false ); // Do you want to accept this order?

			AddButton( 100, 264, 4005, 4007, 1, GumpButtonType.Reply, 0 );
			AddHtmlLocalized( 135, 264, 120, 20, 1006044, 0x7FFF, false, false ); // Ok

			AddButton( 275, 264, 4005, 4007, 0, GumpButtonType.Reply, 0 );
			AddHtmlLocalized( 310, 264, 120, 20, 1011012, 0x7FFF, false, false ); // CANCEL
		}

		public override void OnResponse( NetState sender, RelayInfo info )
		{
			if ( info.ButtonID == 1 ) // Ok
			{
				if ( m_From.PlaceInBackpack( m_Deed ) )
				{
					m_From.SendLocalizedMessage( 1045152 ); // The bulk order deed has been placed in your backpack.
				}
				else
				{
					m_From.SendLocalizedMessage( 1045150 ); // There is not enough room in your backpack for the deed.
					m_Deed.Delete();
				}
			}
			else
			{
				m_Deed.Delete();
			}
		}
	}
}