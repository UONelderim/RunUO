// 2004.09.26 :: Khad :: Zmiana komunikatow powitalnych
// 2005.01.07 :: LogoS :: Drobne poprawki
// 2005.03.07 :: Evantis :: Drobne poprawki jezykowe
// 06.03.16 :: troyan :: lokalizacja

using System;
using Server.Network;

namespace Server.Misc
{
	/// <summary>
	/// This timer spouts some welcome messages to a user at a set interval. It is used on character creation and login.
	/// </summary>
	public class WelcomeTimer : Timer
	{
		private Mobile m_Mobile;
		private int m_State, m_Count;

		private static int[] m_Messages = new int[]
		{
			505812, //	"Witaj {0} w Komnatach Poczatku.", 
			505813, //	"Tu ustalisz, do ktorej boskiej rasy nalezysz, ",
			505814, //	"Wskazesz plemie swego ojca i ustalisz wyglad.", 
			505815, //	"Udaj sie do Komnaty wybranej rasy.", 
			505816	//	"Jesli nie rozpoznasz jej po wygladzie, to co tu robisz? "
		};

		public WelcomeTimer( Mobile m ) : this( m, m_Messages.Length )
		{
		}

		public WelcomeTimer( Mobile m, int count ) : base( TimeSpan.FromSeconds( 5.0 ), TimeSpan.FromSeconds( 4.0 ) )
		{
			m_Mobile = m;
			m_Count = count;
		}

		protected override void OnTick()
		{
			if ( m_State < m_Count )
				m_Mobile.SendLocalizedMessage( m_Messages[m_State++], m_Mobile.Name, 0x35 );

			if ( m_State == m_Count )
				Stop();
		}
	}
}
