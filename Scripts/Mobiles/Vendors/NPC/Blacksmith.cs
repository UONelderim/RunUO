using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Engines.BulkOrders;
using Server.Engines.Craft;
using Server.Items;

namespace Server.Mobiles
{
	public class Blacksmith : BaseVendor
	{
		private ArrayList m_SBInfos = new ArrayList();
		protected override ArrayList SBInfos{ get { return m_SBInfos; } }

		public override NpcGuild NpcGuild{ get{ return NpcGuild.BlacksmithsGuild; } }

		[Constructable]
		public Blacksmith() : base( "- kowal" )
		{
			SetSkill( SkillName.ArmsLore, 65.0, 88.0 );
			SetSkill( SkillName.Blacksmith, 65.0, 88.0 );
			SetSkill( SkillName.Fencing, 60.0, 83.0 );
			SetSkill( SkillName.Macing, 61.0, 93.0 );
			SetSkill( SkillName.Swords, 60.0, 83.0 );
			SetSkill( SkillName.Tactics, 60.0, 83.0 );
			SetSkill( SkillName.Parry, 61.0, 93.0 );
		}

		public override void InitSBInfo()
		{
            m_SBInfos.Add( new SBBlacksmithBuy() );
            m_SBInfos.Add( new SBBlacksmithAddons() );

            m_SBInfos.Add(CraftSB.CraftSellBlacksmith);

		}

		public override VendorShoeType ShoeType
		{
			get{ return VendorShoeType.None; }
		}

		public override void InitOutfit()
		{
			base.InitOutfit();

			Item item = ( Utility.RandomBool() ? null : new Server.Items.RingmailChest() );

			if ( item != null && !EquipItem( item ) )
			{
				item.Delete();
				item = null;
			}

			if ( item == null )
				AddItem( new Server.Items.FullApron() );

			AddItem( new Server.Items.Bascinet() );
			AddItem( new Server.Items.SmithHammer() );
		}

		#region Bulk Orders
		public override Item CreateBulkOrder( Mobile from, bool fromContextMenu )
		{
            if (!IsAssignedBuildingWorking())
            {
                return null;
            }

			PlayerMobile pm = from as PlayerMobile;

			if ( pm != null && pm.NextSmithBulkOrder == TimeSpan.Zero && (fromContextMenu || 0.2 > Utility.RandomDouble()) )
			{
				double theirSkill = pm.Skills[SkillName.Blacksmith].Base;

				if ( theirSkill >= 70.1 )
					pm.NextSmithBulkOrder = TimeSpan.FromMinutes( 30 );
				else if ( theirSkill >= 50.1 )
					pm.NextSmithBulkOrder = TimeSpan.FromMinutes( 20 );
				else
					pm.NextSmithBulkOrder = TimeSpan.FromMinutes( 10 );

				if ( theirSkill >= 70.1 && ((theirSkill - 40.0) / 300.0) > Utility.RandomDouble() )
					return new LargeSmithBOD();

				return SmallSmithBOD.CreateRandomFor( from );
			}

			return null;
		}

		public override bool IsValidBulkOrder( Item item )
		{
            if (!IsAssignedBuildingWorking())
            {
                return false;
            }

			return ( item is SmallSmithBOD || item is LargeSmithBOD );
		}

		public override bool SupportsBulkOrders( Mobile from )
		{
            if (!IsAssignedBuildingWorking())
            {
                return false;
            }

			return ( from is PlayerMobile && from.Skills[SkillName.Blacksmith].Base > 0 );
		}

		public override TimeSpan GetNextBulkOrder( Mobile from )
		{
			if ( from is PlayerMobile )
				return ((PlayerMobile)from).NextSmithBulkOrder;

			return TimeSpan.Zero;
		}

		public override void OnSuccessfulBulkOrderReceive( Mobile from )
		{
			if( Core.SE && from is PlayerMobile )
				((PlayerMobile)from).NextSmithBulkOrder = TimeSpan.Zero;
		}
		#endregion

		public Blacksmith( Serial serial ) : base( serial )
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