using System;
using Server;
using System.Collections;
using Server.Items;

namespace Server.Mobiles
{
	public class Ninja : BaseVendor
	{
		private ArrayList m_SBInfos = new ArrayList();
		protected override ArrayList SBInfos{ get { return m_SBInfos; } }

		[Constructable]
		public Ninja() : base(" - skrytobojca")
		{
			SetSkill( SkillName.Ninjitsu, 64.0, 80.0 );
		}
		
		public override void InitSBInfo()
		{
			m_SBInfos.Add( new SBNinja() );
		}

		public Ninja( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}
}
