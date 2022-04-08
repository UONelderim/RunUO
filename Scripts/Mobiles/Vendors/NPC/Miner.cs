using System;
using System.Collections;
using System.Collections.Generic;
using Server.ContextMenus;
using Server.Items;
using Server;

namespace Server.Mobiles
{
	public class Miner : BaseVendor
	{
		private ArrayList m_SBInfos = new ArrayList();
		protected override ArrayList SBInfos{ get { return m_SBInfos; } }

		[Constructable]
		public Miner() : base( "- gornik" )
		{
			SetSkill( SkillName.Mining, 70.0, 90.0 );
		}

		public override void InitSBInfo()
		{
			m_SBInfos.Add( new SBMiner() );
		}

		public override void InitOutfit()
		{
			base.InitOutfit();

			AddItem( new Server.Items.FancyShirt( 0x3E4 ) );
			AddItem( new Server.Items.LongPants( 0x192 ) );
			AddItem( new Server.Items.Pickaxe() );
			AddItem( new Server.Items.ThighBoots( 0x283 ) );
		}
		
		public override void AddCustomContextEntries( Mobile from, List<ContextMenuEntry> list )
		{
			if ( from.Alive )
			{
				if (IsAssignedBuildingWorking())
				{
					list.Add( new SandMiningLearnEntry( from ) );
					list.Add( new StoneMiningLearnEntry( from ) );
				}
			}

			base.AddCustomContextEntries( from, list );
		}

		private class SandMiningLearnEntry : ContextMenuEntry
		{
			private Mobile m_From;

			public SandMiningLearnEntry( Mobile from ) : base( 6070, 4 )
			{
				m_From = from;
			}

			public override void OnClick()
			{
				if (SandMiningBook.Price <= Banker.GetBalance(m_From))
				{
					if (SandMiningBook.LearnSandMining(m_From))
						Banker.Withdraw(m_From, SandMiningBook.Price);
				}
				else
				{
					m_From.SendLocalizedMessage(502677); // Przykro mi, nie masz tyle zlota w skrzyni bankowej.
				}
			}
		}

		private class StoneMiningLearnEntry : ContextMenuEntry
		{
			private Mobile m_From;

			public StoneMiningLearnEntry( Mobile from ) : base( 6071, 4 )
			{
				m_From = from;
			}

			public override void OnClick()
			{
				if (StoneMiningBook.Price <= Banker.GetBalance(m_From))
				{
					if (StoneMiningBook.LearnStoneMining(m_From))
						Banker.Withdraw(m_From, StoneMiningBook.Price);
				}
				else
				{
					m_From.SendLocalizedMessage(502677); // Przykro mi, nie masz tyle zlota w skrzyni bankowej.
				}
			}
		}

		public Miner( Serial serial ) : base( serial )
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