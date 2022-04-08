// 20.08.2012 :: zombie :: sprawdzanie dostepu, dodanie delaya

using System;
using Server.Items;
using Server.Mobiles;

namespace Server.ContextMenus
{
	public class OpenBankEntry : ContextMenuEntry
	{
		private Mobile m_Banker;
        private Mobile m_From;

		public OpenBankEntry( Mobile from, Mobile banker ) : base( 6105, 12 )
		{
			m_Banker = banker;
            m_From = from;
		}

		public override void OnClick()
		{
			if ( !Owner.From.CheckAlive() )
				return;

			if ( m_Banker is BaseVendor && ((BaseVendor)m_Banker).CheckVendorAccess( m_From ) )
			{
				//this.Owner.From.BankBox.Open();
                m_Banker.Say( 505691 ); // Zaczekaj chwile, zaraz ja znajde.
				Timer t = new Server.Mobiles.Banker.InternalTimer( m_From, m_Banker );
                t.Start();
			}
		}
	}
}