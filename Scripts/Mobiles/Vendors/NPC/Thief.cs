using System;
using System.Collections;
using Server;

namespace Server.Mobiles
{
	public class Thief : BaseVendor
	{
		private ArrayList m_SBInfos = new ArrayList();
		protected override ArrayList SBInfos{ get { return m_SBInfos; } }

		[Constructable]
		public Thief() : base( "- zlodziej" )
		{
			SetSkill( SkillName.Camping, 60.0, 83.0 );
			SetSkill( SkillName.DetectHidden, 65.0, 88.0 );
			SetSkill( SkillName.Hiding, 60.0, 83.0 );
			SetSkill( SkillName.Tracking, 65.0, 88.0 );
			SetSkill( SkillName.Stealing, 65.0, 88.0 );
			SetSkill( SkillName.Snooping, 65.0, 88.0 );
			SetSkill( SkillName.Poisoning, 60.0, 83.0 );
		}

		public override void InitSBInfo()
		{
			m_SBInfos.Add( new SBThief() );
		}

		public override void InitOutfit()
		{
			base.InitOutfit();

			AddItem( new Server.Items.Shirt( Utility.RandomNeutralHue() ) );
			AddItem( new Server.Items.LongPants( Utility.RandomNeutralHue() ) );
			AddItem( new Server.Items.Dagger() );
			AddItem( new Server.Items.ThighBoots( Utility.RandomNeutralHue() ) );
		}

		public Thief( Serial serial ) : base( serial )
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