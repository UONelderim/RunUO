using System; 
using System.Collections; 
using Server; 

namespace Server.Mobiles 
{ 
	public class DrowHerbalist : BaseVendor 
	{ 
		private ArrayList m_SBInfos = new ArrayList(); 
		protected override ArrayList SBInfos{ get { return m_SBInfos; } }

		public override NpcGuild NpcGuild{ get{ return NpcGuild.MagesGuild; } }

		[Constructable]
		public DrowHerbalist() : base( "- zielarz" ) 
		{ 
			SetSkill( SkillName.Alchemy, 80.0, 100.0 );
			SetSkill( SkillName.Cooking, 80.0, 100.0 );
			SetSkill( SkillName.TasteID, 80.0, 100.0 );
		} 

		public override void InitSBInfo() 
		{ 
			m_SBInfos.Add( new SBDrowHerbalist() ); 
		} 

		public override VendorShoeType ShoeType
		{
			get{ return Utility.RandomBool() ? VendorShoeType.Shoes : VendorShoeType.Sandals; } 
		}

		public DrowHerbalist( Serial serial ) : base( serial ) 
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