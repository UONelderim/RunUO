using System;
using Server;
using System.Collections;
using Server.Items;

namespace Server.Mobiles
{
	public class Samurai : BaseVendor
	{
		private ArrayList m_SBInfos = new ArrayList();
		protected override ArrayList SBInfos{ get { return m_SBInfos; } }

		[Constructable]
		public Samurai() : base(" - samuraj")
		{
			SetSkill( SkillName.Bushido, 64.0, 85.0 );
			SetSkill( SkillName.Parry, 64.0, 80.0 );
			//SetSkill( SkillName.Focus, 64.0, 80.0 );
		}
		
		public override void InitSBInfo()
		{
		}

		public Samurai( Serial serial ) : base( serial )
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
