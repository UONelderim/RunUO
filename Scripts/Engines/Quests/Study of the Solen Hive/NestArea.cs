using System;
using Server;

namespace Server.Engines.Quests.Naturalist
{
	public class NestArea
	{
		private static readonly NestArea[] m_Areas = new NestArea[]
			{
				new NestArea( false,    new Rectangle2D( 5878, 3635, 6, 8 ) ),

				new NestArea( false,    new Rectangle2D( 6097, 2972, 6, 8 ) ),

				new NestArea( false,    new Rectangle2D( 6003, 2564, 6, 6 ) ),

				new NestArea( false,    new Rectangle2D( 5182, 3631, 7, 7 ) ),

				new NestArea( true,        new Rectangle2D( 5588, 873, 10, 8 ) ),
			};

		public static int NonSpecialCount
		{
			get
			{
				int n = 0;
				foreach ( NestArea area in m_Areas )
				{
					if ( !area.Special )
						n++;
				}
				return n;
			}
		}

		public static NestArea Find( IPoint2D p )
		{
			foreach ( NestArea area in m_Areas )
			{
				if ( area.Contains( p ) )
					return area;
			}
			return null;
		}

		public static NestArea GetByID( int id )
		{
			if ( id >= 0 && id < m_Areas.Length )
				return m_Areas[id];
			else
				return null;
		}

		private bool m_Special;
		private Rectangle2D[] m_Rects;

		public bool Special{ get{ return m_Special; } }

		public int ID
		{
			get
			{
				for ( int i = 0; i < m_Areas.Length; i++ )
				{
					if ( m_Areas[i] == this )
						return i;
				}
				return 0;
			}
		}

		private NestArea( bool special, params Rectangle2D[] rects )
		{
			m_Special = special;
			m_Rects = rects;
		}

		public bool Contains( IPoint2D p )
		{
			foreach ( Rectangle2D rect in m_Rects )
			{
				if ( rect.Contains( p ) )
					return true;
			}
			return false;
		}
	}
}