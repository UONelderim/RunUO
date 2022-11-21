using System;
using Server;
using Server.Mobiles;

namespace Server.Engines.Quests.Hag
{
	public class WitchApprenticeQuest : QuestSystem
	{
		private static Type[] m_TypeReferenceTable = new Type[]
			{
				typeof( Hag.FindApprenticeObjective ),
				typeof( Hag.FindGrizeldaAboutMurderObjective ),
				typeof( Hag.KillImpsObjective ),
				typeof( Hag.FindZeefzorpulObjective ),
				typeof( Hag.ReturnRecipeObjective ),
				typeof( Hag.FindIngredientObjective ),
				typeof( Hag.ReturnIngredientsObjective ),
				typeof( Hag.DontOfferConversation ),
				typeof( Hag.AcceptConversation ),
				typeof( Hag.HagDuringCorpseSearchConversation ),
				typeof( Hag.ApprenticeCorpseConversation ),
				typeof( Hag.MurderConversation ),
				typeof( Hag.HagDuringImpSearchConversation ),
				typeof( Hag.ImpDeathConversation ),
				typeof( Hag.ZeefzorpulConversation ),
				typeof( Hag.RecipeConversation ),
				typeof( Hag.HagDuringIngredientsConversation ),
				typeof( Hag.BlackheartFirstConversation ),
				typeof( Hag.BlackheartNoPirateConversation ),
				typeof( Hag.BlackheartPirateConversation ),
				typeof( Hag.EndConversation ),
				typeof( Hag.RecentlyFinishedConversation )
			};

		public override Type[] TypeReferenceTable{ get{ return m_TypeReferenceTable; } }

		public override object Name
		{
			get
			{
				// "The Witch's Apprentice"
				return 1055042;
			}
		}

		public override object OfferMessage
		{
			get
			{
				/* <I>The ancient, wrinkled hag looks up from her vile-smelling cauldron.
				 * Her single, unblinking eye attempts to focus in on you, but to
				 * little avail.</I><BR><BR>
				 * 
				 * Eh? Who is it? Who's there?  Come to trouble an old woman have you?<BR><BR>
				 * 
				 * I'll split ye open and swallow yer guts!  I'll turn ye into a pile
				 * o' goo, I will!  Bah!  As if I didn't have enough to worry about.  As if I've
				 * not enough trouble as it is!<BR><BR>
				 * 
				 * Another of my blasted apprentices has gone missing!  Foolish children,
				 * think they know everything.  I should turn the lot of them into toads -
				 * if only they'd return with their task complete!  But that's the trouble, innit?
				 * They never return!<BR><BR>
				 * 
				 * But you don't care, do ye?  I suppose you're another one of those meddlesome kids,
				 * come to ask me for something?  Eh?  Is that it?  You want something from me,
				 * expect me to hand it over?  I've enough troubles with my apprentices, and that
				 * vile imp, Zeefzorpul!  Why, I bet it's him who's got the lot of them!  And who
				 * knows what he's done?  Vile little thing.<BR><BR>
				 * 
				 * If you expect me to help you with your silly little desires, you'll be doing
				 * something for me first, eh?  I expect you to go seek out my apprentice.
				 * I sent him along the road west of here up towards Yew's graveyard, but he never
				 * came back. Find him, and bring him back, and I'll give you a little reward that
				 * I'm sure you'll find pleasant.<BR><BR>
				 * 
				 * But I tells ye to watch out for the imp name've Zeefzorpul!  He's a despicable
				 * little beast who likes to fool and fiddle with folk and generally make life
				 * miserable for everyone.  If ye get him on your bad side, you're sure to end up
				 * ruing the day ye were born. As if you didn't already, with an ugly mug
				 * like that!<BR><BR>
				 * 
				 * Well, you little whelp?  Going to help an old hag or not?
				 */
				return 1055001;
			}
		}

		public override TimeSpan RestartDelay{ get{ return TimeSpan.FromMinutes( 5.0 ); } }
		public override bool IsTutorial{ get{ return false; } }

		public override int Picture{ get{ return 0x15D3; } }

		public WitchApprenticeQuest( PlayerMobile from ) : base( from )
		{
		}

		// Serialization
		public WitchApprenticeQuest()
		{
		}

		public override void Accept()
		{
			base.Accept();

			AddConversation( new AcceptConversation() );
		}

		private static Point3D[] m_ZeefzorpulLocations = new Point3D[]
			{
				new Point3D( 1191, 1598, 0 ),
				new Point3D( 1495, 1425, 0 ),
				new Point3D( 1366, 2723, 0 ),
				new Point3D( 1864, 2897, 0 ),
				new Point3D( 2138, 3341, 0 ),
				new Point3D( 1561, 2671, 0 ),
				new Point3D( 1010, 2667, 5 ),
				new Point3D( 1591, 2156, 5 ),
				new Point3D( 2724, 662, 0 ),
				new Point3D( 474, 1654, 0 ),
				new Point3D( 897, 2411, 0 ),
				new Point3D( 1473, 2521, 0 ),
				new Point3D( 1282, 844, 0 ),
				new Point3D( 2809, 1118, 0 ),
				new Point3D( 2833, 1426, 0 ),
				new Point3D( 1717, 3305, 0 ),
				new Point3D( 4370, 1197, 0 ),
				new Point3D( 3702, 2227, 0 ),
				new Point3D( 3591, 690, 0 ),
				new Point3D( 569, 1309, 0 )
			};

		public static Point3D RandomZeefzorpulLocation()
		{
			int index = Utility.Random( m_ZeefzorpulLocations.Length );

			return m_ZeefzorpulLocations[index];
		}
	}
}