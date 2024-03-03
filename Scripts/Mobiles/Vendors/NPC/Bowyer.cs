using System;
using System.Collections;
using Server;
using Server.Engines.BulkOrders;

namespace Server.Mobiles
{
	[TypeAlias( "Server.Mobiles.Bower" )]
	public class Bowyer : BaseVendor
	{
		private ArrayList m_SBInfos = new ArrayList();
		protected override ArrayList SBInfos{ get { return m_SBInfos; } }

		[Constructable]
		public Bowyer() : base( "- lukmistrz" )
		{
			SetSkill( SkillName.Fletching, 80.0, 100.0 );
			SetSkill( SkillName.Archery, 80.0, 100.0 );
		}

		public override VendorShoeType ShoeType
		{
			get{ return Female ? VendorShoeType.ThighBoots : VendorShoeType.Boots; }
		}

		public override int GetShoeHue()
		{
			return 0;
		}

		public override void InitOutfit()
		{
			base.InitOutfit();

			AddItem( new Server.Items.Bow() );
			AddItem( new Server.Items.LeatherGorget() );
		}

		public override void InitSBInfo()
		{
            m_SBInfos.Add( new SBBowyer() );
            m_SBInfos.Add( CraftSB.CraftSellFletcher );
		}

		#region Bulk Orders
		public override Item CreateBulkOrder(Mobile from, bool fromContextMenu) {
			if (!IsAssignedBuildingWorking()) {
				return null;
			}

			PlayerMobile pm = from as PlayerMobile;

			if (pm != null && pm.NextBowFletcherBulkOrder == TimeSpan.Zero && (fromContextMenu || 0.2 > Utility.RandomDouble())) {
				double theirSkill = pm.Skills[SkillName.Fletching].Base;

				if (theirSkill >= 70.1)
					pm.NextBowFletcherBulkOrder = TimeSpan.FromMinutes(30);
				else if (theirSkill >= 50.1)
					pm.NextBowFletcherBulkOrder = TimeSpan.FromMinutes(20);
				else
					pm.NextBowFletcherBulkOrder = TimeSpan.FromMinutes(10);

				if (theirSkill >= 70.1 && ((theirSkill - 40.0) / 300.0) > Utility.RandomDouble())
					return new LargeFletcherBOD();

				return SmallFletcherBOD.CreateRandomFor(from);
			}

			return null;
		}

		public override bool IsValidBulkOrder(Item item) {
			if (!IsAssignedBuildingWorking()) {
				return false;
			}

			return (item is SmallFletcherBOD || item is LargeFletcherBOD);
		}

		public override bool SupportsBulkOrders(Mobile from) {
			if (!IsAssignedBuildingWorking()) {
				return false;
			}

			return (from is PlayerMobile && from.Skills[SkillName.Fletching].Base > 0);
		}

		public override TimeSpan GetNextBulkOrder(Mobile from) {
			if (from is PlayerMobile)
				return ((PlayerMobile)from).NextBowFletcherBulkOrder;

			return TimeSpan.Zero;
		}

		public override void OnSuccessfulBulkOrderReceive(Mobile from) {
			if (Core.SE && from is PlayerMobile)
				((PlayerMobile)from).NextBowFletcherBulkOrder = TimeSpan.Zero;
		}
		#endregion

		public Bowyer( Serial serial ) : base( serial )
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