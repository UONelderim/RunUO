// 31.10.2012 :: zombie :: usuniecie PowderOfTemperament
// 17.09.2013 :: mortuus :: dodanie ozdob zamiast PowderOfTemperament

using System;

namespace Server.Engines.BulkOrders
{
	public delegate Item ConstructCallback( int type );

	public sealed class RewardType
	{
		private int m_Points;
		private Type[] m_Types;

		public int Points{ get{ return m_Points; } }
		public Type[] Types{ get{ return m_Types; } }

		public RewardType( int points, params Type[] types )
		{
			m_Points = points;
			m_Types = types;
		}

		public bool Contains( Type type )
		{
			for ( int i = 0; i < m_Types.Length; ++i )
			{
				if ( m_Types[i] == type )
					return true;
			}

			return false;
		}
	}

	public sealed class RewardItem
	{
		private int m_Weight;
		private ConstructCallback m_Constructor;
		private int m_Type;

		public int Weight{ get{ return m_Weight; } }
		public ConstructCallback Constructor{ get{ return m_Constructor; } }
		public int Type{ get{ return m_Type; } }

		public RewardItem( int weight, ConstructCallback constructor ) : this( weight, constructor, 0 )
		{
		}

		public RewardItem( int weight, ConstructCallback constructor, int type )
		{
			m_Weight = weight;
			m_Constructor = constructor;
			m_Type = type;
		}

		public Item Construct()
		{
			try{ return m_Constructor( m_Type ); }
			catch{ return null; }
		}
	}

	public sealed class RewardGroup
	{
		private int m_Points;
		private RewardItem[] m_Items;

		public int Points{ get{ return m_Points; } }
		public RewardItem[] Items{ get{ return m_Items; } }

		public RewardGroup( int points, params RewardItem[] items )
		{
			m_Points = points;
			m_Items = items;
		}

		public RewardItem AcquireItem()
		{
			if ( m_Items.Length == 0 )
				return null;
			else if ( m_Items.Length == 1 )
				return m_Items[0];

            int totalWeight = 0;

            for (int i = 0; i < m_Items.Length; ++i)
                totalWeight += m_Items[i].Weight;

            int randomWeight = Utility.Random(totalWeight);

            for (int i = 0; i < m_Items.Length; ++i)
            {
                RewardItem item = m_Items[i];

                if (randomWeight < item.Weight)
                    return item;

                randomWeight -= item.Weight;
            }
			return null;
		}
	}

	public abstract class RewardCalculator
	{
		private RewardGroup[] m_Groups;

		public RewardGroup[] Groups{ get{ return m_Groups; } set{ m_Groups = value; } }

		public abstract int ComputePoints( int quantity, bool exceptional, BulkMaterialType material, int itemCount, Type type );
		public abstract int ComputeGold( int quantity, bool exceptional, BulkMaterialType material, int itemCount, Type type );

		public virtual int ComputeFame( SmallBOD bod )
		{
			int points = ComputePoints( bod ) / 50;

			return points * points;
		}

		public virtual int ComputeFame( LargeBOD bod )
		{
			int points = ComputePoints( bod ) / 50;

			return points * points;
		}

		public virtual int ComputePoints( SmallBOD bod )
		{
			return ComputePoints( bod.AmountMax, bod.RequireExceptional, bod.Material, 1, bod.Type );
		}

		public virtual int ComputePoints( LargeBOD bod )
		{
			return ComputePoints( bod.AmountMax, bod.RequireExceptional, bod.Material, bod.Entries.Length, bod.Entries[0].Details.Type );
		}

		public virtual int ComputeGold( SmallBOD bod )
		{
			return ComputeGold( bod.AmountMax, bod.RequireExceptional, bod.Material, 1, bod.Type );
		}

		public virtual int ComputeGold( LargeBOD bod )
		{
			return ComputeGold( bod.AmountMax, bod.RequireExceptional, bod.Material, bod.Entries.Length, bod.Entries[0].Details.Type );
		}

		public virtual RewardGroup LookupRewards( int points )
		{
			for ( int i = m_Groups.Length - 1; i >= 1; --i )
			{
				RewardGroup group = m_Groups[i];

				if ( points >= group.Points )
					return group;
			}

			return m_Groups[0];
		}

		public virtual int LookupTypePoints( RewardType[] types, Type type )
		{
			for ( int i = 0; i < types.Length; ++i )
			{
				if ( types[i].Contains( type ) )
					return types[i].Points;
			}

			return 0;
		}

		public RewardCalculator()
		{
		}
	}
}