using System.Collections.Generic;

namespace Server.Engines.BulkOrders
{
	[TypeAlias( "Scripts.Engines.BulkOrders.LargeFletcherBOD" )]
	public class LargeFletcherBOD : LargeBOD
	{
		public override int ComputeFame()
		{
			return FletcherRewardCalculator.Instance.ComputeFame( this );
		}

		public override int ComputeGold()
		{
			return FletcherRewardCalculator.Instance.ComputeGold( this );
		}

		[Constructable]
		public LargeFletcherBOD()
		{
			LargeBulkEntry[] entries = LargeBulkEntry.ConvertEntries(this, LargeBulkEntry.BowFletcher);
			bool useMaterials = Utility.RandomBool();
			
			int amountMax = Utility.RandomList( 10, 15, 20, 20 );
			bool reqExceptional = ( 0.825 > Utility.RandomDouble() );

			BulkMaterialType material;

			if ( useMaterials )
				material = SmallBOD.GetRandomMaterial(BulkMaterialType.None, BulkMaterialType.Oak, SmallFletcherBOD.m_BowFletchingMaterialChances);
			else
				material = BulkMaterialType.None;

			BulkMaterialType material2 = SmallBOD.GetRandomMaterial(BulkMaterialType.BowstringLeather, BulkMaterialType.BowstringGut, SmallFletcherBOD.m_BowFletchingMaterial2Chances);

			this.Hue = 1425;
			this.AmountMax = amountMax;
			this.Entries = entries;
			this.RequireExceptional = reqExceptional;
			this.Material = material;
			this.Material2 = material2;
		}

		public LargeFletcherBOD( int amountMax, bool reqExceptional, BulkMaterialType mat, BulkMaterialType mat2, LargeBulkEntry[] entries )
		{
			this.Hue = 1425;
			this.AmountMax = amountMax;
			this.Entries = entries;
			this.RequireExceptional = reqExceptional;
			this.Material = mat;
			if (mat2 != BulkMaterialType.None)
				this.Material2 = mat2;
			else
				this.Material2 = BulkMaterialType.BowstringLeather;
		}

		public override List<Item> ComputeRewards( bool full )
		{
			List<Item> list = new List<Item>();

			RewardGroup rewardGroup = FletcherRewardCalculator.Instance.LookupRewards(FletcherRewardCalculator.Instance.ComputePoints( this ) );

			if ( rewardGroup != null )
			{
				if ( full )
				{
					for ( int i = 0; i < rewardGroup.Items.Length; ++i )
					{
						Item item = rewardGroup.Items[i].Construct();

						if ( item != null )
							list.Add( item );
					}
				}
				else
				{
					RewardItem rewardItem = rewardGroup.AcquireItem();

					if ( rewardItem != null )
					{
						Item item = rewardItem.Construct();

						if ( item != null )
							list.Add( item );
					}
				}
			}

			return list;
		}

		public LargeFletcherBOD( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 ); // version
			writer.Write( (int)Material2 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
			switch ( version )
			{
				case 1:
					Material2 = (BulkMaterialType)reader.ReadInt();
					break;
			}

			if (Material2 == BulkMaterialType.None) Material2 = BulkMaterialType.BowstringLeather;
		}
	}
}