using System;
using System.Collections;
using System.Collections.Generic;
using Server.ContextMenus;
using Server.Items;
using Server;
using Server.Engines.Craft;

namespace Server.Mobiles
{
	public class Carpenter : BaseVendor
	{
		private ArrayList m_SBInfos = new ArrayList();
		protected override ArrayList SBInfos{ get { return m_SBInfos; } }

		public override NpcGuild NpcGuild{ get{ return NpcGuild.TinkersGuild; } }

		[Constructable]
		public Carpenter() : base( "- stolarz" )
		{
			SetSkill( SkillName.Carpentry, 85.0, 100.0 );
			SetSkill( SkillName.Lumberjacking, 60.0, 83.0 );
		}

		public override void InitSBInfo()
		{
			m_SBInfos.Add( new SBStavesWeaponBuy() );
            m_SBInfos.Add( new SBWoodenShieldsBuy() );
            m_SBInfos.Add( new SBCarpenterBuy() );
            m_SBInfos.Add( new SBCarpenterAddons() );
			
			if ( IsTokunoVendor )
				m_SBInfos.Add( new SBSECarpenterBuy() );

            m_SBInfos.Add( CraftSB.CraftSellCarpenter );
		}

		public override void InitOutfit()
		{
			base.InitOutfit();

			AddItem( new Server.Items.HalfApron() );
		}
		
		public override void AddCustomContextEntries( Mobile from, List<ContextMenuEntry> list )
		{
			if ( from.Alive )
			{
				if (IsAssignedBuildingWorking())
				{
					list.Add( new StonecraftingLearnEntry( from ) );
				}
			}

			base.AddCustomContextEntries( from, list );
		}

		private class StonecraftingLearnEntry : ContextMenuEntry
		{
			private Mobile m_From;

			public StonecraftingLearnEntry( Mobile from ) : base( 6069, 12 )
			{
				m_From = from;
			}

			public override void OnClick()
			{
				if (MasonryBook.Price <= Banker.GetBalance(m_From))
				{
					if (MasonryBook.LearnStonecrafting(m_From))
						Banker.Withdraw(m_From, MasonryBook.Price);
				}
				else
				{
					m_From.SendLocalizedMessage(502677); // Przykro mi, nie masz tyle zlota w skrzyni bankowej.
				}
			}
		}

		public Carpenter( Serial serial ) : base( serial )
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