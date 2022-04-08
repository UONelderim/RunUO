using System;
using System.Collections;
using System.Collections.Generic;
using Server.ContextMenus;
using Server.Items;
using Server;
using Server.Engines.Craft;

namespace Server.Mobiles
{
	public class Tinker : BaseVendor
	{
		private ArrayList m_SBInfos = new ArrayList();
		protected override ArrayList SBInfos{ get { return m_SBInfos; } }

		public override NpcGuild NpcGuild{ get{ return NpcGuild.TinkersGuild; } }

		[Constructable]
		public Tinker() : base( "- majster" )
		{
			SetSkill( SkillName.Lockpicking, 60.0, 83.0 );
			SetSkill( SkillName.RemoveTrap, 75.0, 98.0 );
			SetSkill( SkillName.Tinkering, 64.0, 100.0 );
		}

		public override void InitSBInfo()
		{
			m_SBInfos.Add( new SBTinkerBuy() );

            m_SBInfos.Add(CraftSB.CraftSellTinker);
		}
		
		public override void AddCustomContextEntries( Mobile from, List<ContextMenuEntry> list )
		{
			if ( from.Alive )
			{
				if (IsAssignedBuildingWorking())
				{
					list.Add( new ColorMetalsLearnEntry( from ) );
				}
			}

			base.AddCustomContextEntries( from, list );
		}

		private class ColorMetalsLearnEntry : ContextMenuEntry
		{
			private Mobile m_From;

			public ColorMetalsLearnEntry( Mobile from ) : base( 6073, 12 )
			{
				m_From = from;
			}

			public override void OnClick()
			{
				if (ColorMetalsBook.Price <= Banker.GetBalance(m_From))
				{
					if (ColorMetalsBook.LearnColorMetals(m_From))
						Banker.Withdraw(m_From, ColorMetalsBook.Price);
				}
				else
				{
					m_From.SendLocalizedMessage(502677); // Przykro mi, nie masz tyle zlota w skrzyni bankowej.
				}
			}
		}

		public Tinker( Serial serial ) : base( serial )
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
	}
}