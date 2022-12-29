// 20.08.2012 :: zombie :: tlumaczenie komend, logowanie czekow (przeniesione), delay 3s na otwarcie skrzyni

using System;
using System.Collections;
using System.Collections.Generic;
using Server.Items;
using Server.ContextMenus;
using Server.Network;
using System.Text.RegularExpressions;
using Nelderim;
using Server.Commands;

namespace Server.Mobiles
{
	public class Banker : BaseVendor
	{

        public class InternalTimer : Timer
		{
			private Mobile m_Mobile;
			private Mobile m_Banker;

			public InternalTimer( Mobile m , Mobile b ) : base( TimeSpan.FromSeconds( 3.0 )  )
			{
				Priority = TimerPriority.OneSecond;
				m_Mobile = m;
				m_Banker = b;
			}

			protected override void OnTick()
			{
				if ( m_Banker == null )
					return;
				
				if ( m_Mobile == null || !m_Mobile.Alive || m_Banker.GetDistanceToSqrt( m_Mobile ) > 4 )
				{
					m_Banker.Say( 505694 ); // Gdziez on polazl?! Nie bede biegal ze skrzynia!
				}
				else
				{
					m_Mobile.RevealingAction();
					BankBox box = m_Mobile.BankBox;

					if ( box != null )
						box.Open();
				}

				Stop();
			}
		}

		private ArrayList m_SBInfos = new ArrayList();
		protected override ArrayList SBInfos{ get { return m_SBInfos; } }

		public override NpcGuild NpcGuild{ get{ return NpcGuild.MerchantsGuild; } }

		[Constructable]
		public Banker() : base( "- bankier" )
		{
		}

		public override void InitSBInfo()
		{
			m_SBInfos.Add( new SBBanker() );
		}

        public static int GetFullBalance( Mobile from )
		{
			Backpack b = from.Backpack as Backpack;
			
			if ( b != null )
				return b.GetAmount( typeof( Gold ) ) + GetBalance( from );
			
			return GetBalance( from );
		}

		public static int GetBalance( Mobile from )
		{
			Item[] gold, checks;

			return GetBalance( from, out gold, out checks );
		}

		public static int GetBalance( Mobile from, out Item[] gold, out Item[] checks )
		{
			int balance = 0;

			Container bank = from.FindBankNoCreate();

			if ( bank != null )
			{
				gold = bank.FindItemsByType( typeof( Gold ) );
				checks = bank.FindItemsByType( typeof( BankCheck ) );

				for ( int i = 0; i < gold.Length; ++i )
					balance += gold[i].Amount;

				for ( int i = 0; i < checks.Length; ++i )
					balance += ((BankCheck)checks[i]).Worth;
			}
			else
			{
				gold = checks = new Item[0];
			}

			return balance;
		}

		public static bool FullWithdraw( Mobile from, int amount )
		{
			int balance = GetFullBalance( from );
			int oldamount = amount;

			if ( balance < amount )
				return false;

			Backpack b = from.Backpack as Backpack;
			
			if ( b != null )
			{
				int backpackgold = from.Backpack.GetAmount( typeof( Gold ) );
			
				if ( backpackgold < amount && backpackgold > 0 )
				{
					if ( from.Backpack.ConsumeTotal( typeof( Gold ), backpackgold ) )
						amount -= backpackgold;
				}
				else if ( from.Backpack.ConsumeTotal( typeof( Gold ), amount ) )
					return true;
				         
			}
			
			if ( !Withdraw( from, amount ) )
			{
				Deposit( from, oldamount - amount );
				return false;
			}
				
			return true;
		}

		public static bool Withdraw( Mobile from, int amount )
		{
			Item[] gold, checks;
			int balance = GetBalance( from, out gold, out checks );

			if ( balance < amount )
				return false;

			for ( int i = 0; amount > 0 && i < gold.Length; ++i )
			{
				if ( gold[i].Amount <= amount )
				{
					amount -= gold[i].Amount;
					gold[i].Delete();
				}
				else
				{
					gold[i].Amount -= amount;
					amount = 0;
				}
			}

			for ( int i = 0; amount > 0 && i < checks.Length; ++i )
			{
				BankCheck check = (BankCheck)checks[i];

				if ( check.Worth <= amount )
				{
					amount -= check.Worth;
					check.Delete();
				}
				else
				{
					check.Worth -= amount;
					amount = 0;
				}
			}

			return true;
		}

		public static bool Deposit( Mobile from, int amount )
		{
			BankBox box = from.FindBankNoCreate();
			if ( box == null )
				return false;

			List<Item> items = new List<Item>();

			while ( amount > 0 )
			{
				Item item;
				if ( amount < 5000 )
				{
					item = new Gold( amount );
					amount = 0;
				}
				else if ( amount <= 1000000 )
				{
					item = new BankCheck( amount );
					amount = 0;
				}
				else
				{
					item = new BankCheck( 1000000 );
					amount -= 1000000;
				}

				if ( box.TryDropItem( from, item, false ) )
				{
					items.Add( item );
				}
				else
				{
					item.Delete();
					foreach ( Item curItem in items )
					{
						curItem.Delete();
					}

					return false;
				}
			}

			return true;
		}

		public static int DepositUpTo( Mobile from, int amount )
		{
			BankBox box = from.FindBankNoCreate();
			if ( box == null )
				return 0;

			int amountLeft = amount;
			while ( amountLeft > 0 )
			{
				Item item;
				int amountGiven;

				if ( amountLeft < 5000 )
				{
					item = new Gold( amountLeft );
					amountGiven = amountLeft;
				}
				else if ( amountLeft <= 1000000 )
				{
					item = new BankCheck( amountLeft );
					amountGiven = amountLeft;
				}
				else
				{
					item = new BankCheck( 1000000 );
					amountGiven = 1000000;
				}

				if ( box.TryDropItem( from, item, false ) )
				{
					amountLeft -= amountGiven;
				}
				else
				{
					item.Delete();
					break;
				}
			}

			return amount - amountLeft;
		}

		public static void Deposit( Container cont, int amount )
		{
			while ( amount > 0 )
			{
				Item item;

				if ( amount < 5000 )
				{
					item = new Gold( amount );
					amount = 0;
				}
				else if ( amount <= 1000000 )
				{
					item = new BankCheck( amount );
					amount = 0;
				}
				else
				{
					item = new BankCheck( 1000000 );
					amount -= 1000000;
				}

				cont.DropItem( item );
			}
		}

		public Banker( Serial serial ) : base( serial )
		{
		}

		public override bool HandlesOnSpeech( Mobile from )
		{
			if ( from.InRange( this.Location, 12 ) )
				return true;

			return base.HandlesOnSpeech( from );
		}

        // 20.08.2012 :: zombie :: tlumaczenie (przeniesione)
		public override void OnSpeech( SpeechEventArgs e )
		{
			Mobile from = e.Mobile;

			if ( !e.Handled && e.Mobile.InRange( this.Location, 3 ) )
			{
				try
				{
					#region "Podaj skrzynie!"

					if ( Regex.IsMatch( e.Speech, "bank", RegexOptions.IgnoreCase ) && Regex.IsMatch( e.Speech, "skrzyn", RegexOptions.IgnoreCase ) )
					{
						if ( CheckVendorAccess( from ) )
						{
							e.Handled = true;
							this.Say ( 505691 ); // Zaczekaj chwile, zaraz ja znajde.
							Timer t = new InternalTimer( e.Mobile , this );
							t.Start();
						}
					}
					#endregion
					#region "Czek"
					else if ( Regex.IsMatch( e.Speech, "czek", RegexOptions.IgnoreCase ) )
					{
						if ( CheckVendorAccess( from ) )
						{
							e.Handled = true;

							string[] split = e.Speech.Split( ' ' );

							if ( split.Length >= 2 )
							{
								int amount;

								try
								{
									amount = Convert.ToInt32( split[1] );
								}
								catch
								{
									return;
								}

								if ( amount < 5000 )
								{
									this.Say ( 505692 ); // Nie wypisuje sie czekow na kwote mniejsza, niz 5 000 centarow. 
								}
								else if ( amount > 1000000 )
								{
									this.Say ( 505693 ); // Ho! Ho! Milion centarow! Za duzo na jeden czek! 
								}
								else
								{
									BankCheck check = new BankCheck( amount );

									check.LabelOfCreator = ( string ) CommandLogging.Format( e.Mobile );
									
									BankBox box = e.Mobile.BankBox;

									if ( box == null || !box.TryDropItem( e.Mobile, check, false ) )
									{
										this.Say( 500386 ); // There's not enough room in your bankbox for the check!
										check.Delete();
									}
									else if ( !Withdraw( e.Mobile, amount ) )
									{
										this.Say( 500384 ); // Ah, art thou trying to fool me? Thou hast not so much gold!
										check.Delete();
									}
									else
									{
										this.Say( 1042673, AffixType.Append, " " + amount.ToString(), "" ); // Into your bank box I have placed a check in the amount of:
										BankLog.Log(from, amount, "check");
									}
								}
							}
						}
					}
					#endregion
					#region "Pobrac"
					else if ( Regex.IsMatch( e.Speech, "pobrac", RegexOptions.IgnoreCase )  )
					{
						if ( CheckVendorAccess( from ) )
						{
							e.Handled = true;

							string[] split = e.Speech.Split( ' ' );

							if ( split.Length >= 2 )
							{
								int amount;

								try
								{
									amount = Convert.ToInt32( split[1] );
								}
								catch
								{
									return;
								}

								if ( amount > 5000 )
								{
									this.Say( 500381 ); // Thou canst not withdraw so much at one time!
								}
								else if ( amount > 0 )
								{
									// BankBox box = e.Mobile.BankBox;

									if ( !Withdraw( e.Mobile, amount ) )
									{
										this.Say( 500384 ); // Ah, art thou trying to fool me? Thou hast not so much gold!
									}
									else
									{
										e.Mobile.AddToBackpack( new Gold( amount ) );

										this.Say( 1010005 ); // Thou hast withdrawn gold from thy account.
									}
								}
							}
						}
					}
					#endregion
					#region "Saldo"
					else if ( Regex.IsMatch( e.Speech, "saldo", RegexOptions.IgnoreCase ) )
					{
						if ( CheckVendorAccess( from ) )
						{
							e.Handled = true;
							if (e.Speech.ToLower() == "saldo") OnLazySpeech();
							else this.Say( 1042759, GetBalance( e.Mobile ).ToString() ); // Thy current bank balance is ~1_AMOUNT~ gold.
						}
					}
				#endregion
				}
				catch ( Exception exc )
				{
					Console.WriteLine( exc.ToString() );
				}

				base.OnSpeech( e );
			}
		}
        //

		public override void AddCustomContextEntries( Mobile from, List<ContextMenuEntry> list )
		{
			if ( from.Alive )
				list.Add( new OpenBankEntry( from, this ) );

			base.AddCustomContextEntries( from, list );
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