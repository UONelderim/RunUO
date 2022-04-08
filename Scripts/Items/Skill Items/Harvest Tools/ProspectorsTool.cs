using System;
using Server;
using Server.Targeting;
using Server.Engines.Harvest;

namespace Server.Items
{
	public class ProspectorsTool : BaseBashing, IUsesRemaining
	{
		private int m_UsesRemaining;

		[CommandProperty( AccessLevel.GameMaster )]
		public int UsesRemaining
		{
			get { return m_UsesRemaining; }
			set { m_UsesRemaining = value; InvalidateProperties(); }
		}

		public bool ShowUsesRemaining{ get{ return true; } set{} }

		public override int LabelNumber{ get{ return 1049065; } } // prospector's tool

		public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.CrushingBlow; } }
		public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.ShadowStrike; } }

		public override int AosStrengthReq{ get{ return 40; } }
		public override int AosMinDamage{ get{ return 13; } }
		public override int AosMaxDamage{ get{ return 15; } }
		public override int AosSpeed{ get{ return 33; } }

		public override int OldStrengthReq{ get{ return 10; } }
		public override int OldMinDamage{ get{ return 6; } }
		public override int OldMaxDamage{ get{ return 8; } }
		public override int OldSpeed{ get{ return 33; } }

		[Constructable]
		public ProspectorsTool() : base( 0xFB4 )
		{
			Weight = 9.0;
			UsesRemaining = 10;
		}

		public ProspectorsTool( Serial serial ) : base( serial )
		{
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( IsChildOf( from.Backpack ) || Parent == from )
				from.Target = new InternalTarget( this );
			else
				from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
		}

		public void Prospect( Mobile from, object toProspect )
		{
			if ( !IsChildOf( from.Backpack ) && Parent != from )
			{
				from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
				return;
			}

			HarvestSystem system = Mining.System;

			int tileID;
			Map map;
			Point3D loc;

			if ( !system.GetHarvestDetails( from, this, toProspect, out tileID, out map, out loc ) )
			{
				from.SendLocalizedMessage( 1049048 ); // You cannot use your prospector tool on that.
				return;
			}

			HarvestDefinition def = system.GetDefinition( tileID );
			if(def == null) {
				from.SendLocalizedMessage(1049048); // You cannot use your prospector tool on that.
				return;
			}

			HarvestVein[] veinList;
			def.GetRegionVein( out veinList, map, loc.X, loc.Y );

			if ( veinList.Length <= 1 )
			{
				from.SendLocalizedMessage( 1049048 ); // You cannot use your prospector tool on that.
				return;
			}

			HarvestBank bank = def.GetBank( map, loc.X, loc.Y );

			if ( bank == null )
			{
				from.SendLocalizedMessage( 1049048 ); // You cannot use your prospector tool on that.
				return;
			}

			HarvestVein vein = bank.Vein, defaultVein = bank.DefaultVein;

			if ( vein == null || defaultVein == null )
			{
				from.SendLocalizedMessage( 1049048 ); // You cannot use your prospector tool on that.
				return;
			}
			else if ( vein != defaultVein )
			{
				from.SendLocalizedMessage( 1049049 ); // That ore looks to be prospected already.
				return;
			}

			int veinIndex = Array.IndexOf( veinList, vein );

			if ( veinIndex < 0 )
			{
				from.SendLocalizedMessage( 1049048 ); // You cannot use your prospector tool on that.
			}
			else if ( veinIndex >= (veinList.Length - 1) )
			{
				from.SendLocalizedMessage( 1049061 ); // You cannot improve valorite ore through prospecting.
			}
			else
			{
				bank.Vein = veinList[veinIndex + 1];

				int indexInAllResources = Array.IndexOf( def.Resources, bank.Vein.PrimaryResource );
				if( indexInAllResources < 8 && indexInAllResources > 0 )
					from.SendLocalizedMessage( 1049049 + indexInAllResources );
				else
					from.SendMessage( "Przesiewasz rude, zauwazajac ze mozna tu kopac lepszy metal." );

				--UsesRemaining;

				if ( UsesRemaining <= 0 )
				{
					from.SendLocalizedMessage( 1049062 ); // You have used up your prospector's tool.
					Delete();
				}
			}
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 ); // version
			writer.Write( (int) m_UsesRemaining );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 1:
				{
					m_UsesRemaining = reader.ReadInt();
					break;
				}
				case 0:
				{
					m_UsesRemaining = 50;
					break;
				}
			}
		}

		private class InternalTarget : Target
		{
			private ProspectorsTool m_Tool;

			public InternalTarget( ProspectorsTool tool ) : base( 2, true, TargetFlags.None )
			{
				m_Tool = tool;
			}

			protected override void OnTarget( Mobile from, object targeted )
			{
				m_Tool.Prospect( from, targeted );
			}
		}
	}
}