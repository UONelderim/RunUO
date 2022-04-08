using System;
using Server;
using System.Collections;
using Server.Items;

namespace Server.Mobiles
{
	public class Necromancer : BaseVendor
	{
		private ArrayList m_SBInfos = new ArrayList();
		protected override ArrayList SBInfos{ get { return m_SBInfos; } }

		[Constructable]
		public Necromancer() : base(" - nekromanta")
		{
			SetSkill( SkillName.Necromancy, 64.0, 80.0 );
			SetSkill( SkillName.SpiritSpeak, 64.0, 80.0 );
		}
		
		public override void InitSBInfo()
		{
			m_SBInfos.Add( new SBNecromancer() );
		}

		public Necromancer( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 1 ); // version
			writer.Write( (byte)0 ); // Dummy byte 
			writer.Write( (byte)0 ); // Dummy byte 
			writer.Write( (byte)0 ); // Dummy byte 
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
			if ( version > 0 )
			{
				reader.ReadByte();// Dummy byte 
				reader.ReadByte(); // Dummy byte 
				reader.ReadByte(); // Dummy byte 
			}
		}
	}
}
