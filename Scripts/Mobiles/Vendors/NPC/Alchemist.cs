using System;
using System.Collections;
using System.Collections.Generic;
using Server.ContextMenus;
using Server.Items;
using Server;

namespace Server.Mobiles
{
	public class Alchemist : BaseVendor
	{
		private ArrayList m_SBInfos = new ArrayList();
		protected override ArrayList SBInfos{ get { return m_SBInfos; } }

		public override NpcGuild NpcGuild{ get{ return NpcGuild.MagesGuild; } }

		public override bool CanTeach{ get{ return true; } }

		[Constructable]
		public Alchemist() : base( "- alchemik" )
		{
			SetSkill( SkillName.Anatomy, 85.0, 100.0 );
			SetSkill( SkillName.Alchemy, 85.0, 100.0 );
            SetSkill( SkillName.Zielarstwo, 65.0, 88.0 );
            SetSkill( SkillName.Healing, 65.0, 88.0 );
			SetSkill( SkillName.SpiritSpeak, 65.0, 88.0 );
			SetSkill( SkillName.TasteID, 85.0, 100.0 );			
		}

		public override void InitSBInfo()
		{
			m_SBInfos.Add( new SBAlchemist() );
		}

		public override VendorShoeType ShoeType
		{
			get{ return Utility.RandomBool() ? VendorShoeType.Shoes : VendorShoeType.Sandals; }
		}

		public override void InitOutfit()
		{
			base.InitOutfit();

			AddItem( new Server.Items.Robe( Utility.RandomPinkHue() ) );
		}

		public override void AddCustomContextEntries( Mobile from, List<ContextMenuEntry> list )
		{
			if ( from.Alive )
			{
				if (IsAssignedBuildingWorking())
				{
					list.Add( new GlassLearnEntry( from ) );
				}
			}

			base.AddCustomContextEntries( from, list );
		}

		private class GlassLearnEntry : ContextMenuEntry
		{
			private Mobile m_From;

			public GlassLearnEntry( Mobile from ) : base( 6068, 4 )
			{
				m_From = from;
			}

			public override void OnClick()
			{
				if (GlassblowingBook.Price <= Banker.GetBalance(m_From))
				{
					if (GlassblowingBook.LearnGlassblowing(m_From))
						Banker.Withdraw(m_From, GlassblowingBook.Price);
				}
				else
				{
					m_From.SendLocalizedMessage(502677); // Przykro mi, nie masz tyle zlota w skrzyni bankowej.
				}
			}
		}

		public Alchemist( Serial serial ) : base( serial )
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