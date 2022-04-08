using System; 
using System.Collections;
using Server;
using Server.Engines.Craft;
using Server.Items;

namespace Server.Mobiles 
{ 
	public class LeatherWorker : BaseVendor 
	{ 
		private ArrayList m_SBInfos = new ArrayList(); 
		protected override ArrayList SBInfos{ get { return m_SBInfos; } } 

		[Constructable]
		public LeatherWorker() : base( "- kusnierz" ) 
		{ 
			SetSkill( SkillName.Tailoring, 80.0, 100.0 );
		} 
		public override void InitSBInfo() 
		{ 
			m_SBInfos.Add( new SBLeatherArmorBuy() ); 
			m_SBInfos.Add( new SBStuddedArmorBuy() );
            m_SBInfos.Add( new SBLeatherWorkerBuy() );
            m_SBInfos.Add( new SBLeatherWorkerAddons() );

            m_SBInfos.Add( CraftSB.CraftSellLeatherWorker );
		} 
		public LeatherWorker( Serial serial ) : base( serial ) 
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
