#region AuthorHeader
//
//	Auction version 2.1, by Xanthos and Arya
//
//  Based on original ideas and code by Arya
//
#endregion AuthorHeader
using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.ContextMenus;

namespace Arya.Auction
{
	#region Context Menu

	public class TradeHouseEntry : ContextMenuEntry
	{
		private Auctioner m_Auctioner;
		
		 

		public TradeHouseEntry( Auctioner auctioner ) : base( 6103, 10 )
		{
			m_Auctioner = auctioner;
		}

		public override void OnClick()
		{
			Mobile m = Owner.From;

			if ( ! m.CheckAlive() )
				return;

			if ( AuctionSystem.Running )
			{
				m.SendGump( new AuctionGump( m ) );
			}
			else if ( m_Auctioner != null )
			{
				m_Auctioner.SayTo( m, AuctionSystem.ST[ 145 ] );
			}
		}
	}

	#endregion

	/// <summary>
	/// Summary description for Auctioner.
	/// </summary>
	public class Auctioner : BaseVendor
	{

		//private readonly List<SBInfo> m_SBInfos = new List<SBInfo>();
		private readonly ArrayList m_SBInfos = new ArrayList();

		[ Constructable ]
		public Auctioner() : base ( "the Auctioner" )
		{
			RangePerception = 10;
		}
		
		//protected override List<SBInfo> SBInfos { get { return m_SBInfos; } }
		protected override ArrayList SBInfos { get { return m_SBInfos; } }


		public override void InitOutfit()
		{
			AddItem( new LongPants( GetRandomHue() ) );
			AddItem( new Boots( GetRandomHue() ) );
			AddItem( new FeatheredHat( GetRandomHue() ) );

			if ( Female )
			{
				AddItem( new Kilt( GetRandomHue() ) );
				AddItem( new Shirt( GetRandomHue() ) );
				
				GoldBracelet bracelet = new GoldBracelet();
				bracelet.Hue = GetRandomHue();
				AddItem( bracelet );

				GoldNecklace neck = new GoldNecklace();
				neck.Hue = GetRandomHue();
				AddItem( neck );
			}
			else
			{
				AddItem( new FancyShirt( GetRandomHue() ) );
				AddItem( new Doublet( GetRandomHue() ) );
			}
		}

		public Auctioner( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize (writer);

			writer.Write( 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize (reader);

			reader.ReadInt();
		}

		public override void AddCustomContextEntries( Mobile from, List<ContextMenuEntry> list )
		{
			list.Add( new TradeHouseEntry( this ) );
		}

		public override void InitSBInfo()
		{
		}

		// protected override System.Collections.ArrayList SBInfos
		// {
			// get
			// {
				// return new ArrayList();
			// }
		// }

		public override void OnSpeech(SpeechEventArgs e)
		{
			if ( e.Speech.ToLower().IndexOf( "auction" ) > -1 )
			{
				e.Handled = true;

				if ( ! e.Mobile.CheckAlive() )
				{
					SayTo( e.Mobile, "Am I hearing voices?" );
				}
				else if ( AuctionSystem.Running )
				{
					e.Mobile.SendGump( new AuctionGump( e.Mobile ) );
				}
				else
				{
					SayTo( e.Mobile, "Sorry, we're closed at this time. Please try again later." );
				}
			}
			else if ( e.Speech.ToLower().IndexOf( "version" ) > -1 )
				SayTo( e.Mobile, "Auction version 2.1, by Xanthos and Arya" );

			base.OnSpeech (e);
		}
	}
}