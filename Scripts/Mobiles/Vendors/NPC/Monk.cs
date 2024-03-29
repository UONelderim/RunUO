﻿
using System;
using System.Collections;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	public class Monk : BaseVendor
	{
		private ArrayList m_SBInfos = new ArrayList();
		protected override ArrayList SBInfos { get { return m_SBInfos; } }

		[Constructable]
		public Monk() : base( "Mnich" )
		{
			SetSkill( SkillName.EvalInt, 100.0 );
			SetSkill( SkillName.Tactics, 70.0, 90.0 );
			SetSkill( SkillName.Wrestling, 70.0, 90.0 );
			SetSkill( SkillName.MagicResist, 70.0, 90.0 );
			SetSkill( SkillName.Macing, 70.0, 90.0 );
		}

		public override void InitSBInfo()
		{
			m_SBInfos.Add( new SBMonk() );
		}
		public override void InitOutfit()
		{
			AddItem( new Sandals() );
			AddItem( new MonkRobe() );
		}

		public Monk( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int)0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}

	}
}
