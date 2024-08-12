using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Mobiles;
using Server.Targeting;
using Server.Items;
using Server.Network;
using Server.Factions;
using Server.Spells.Seventh;
using Server.Spells.Fifth;
using Server.Spells.Necromancy;
using Server.Spells;
using Server.Spells.Ninjitsu;
using Server.Engines.XmlSpawner2;

namespace Server.SkillHandlers
{
	public class Stealing
	{
		public static void Initialize()
		{
			SkillInfo.Table[33].Callback = new SkillUseCallback( OnUse );
		}

        public static readonly bool ClassicMode = true; // Uzywane aby przez 2 minuty red/crim mogl bic przylapanego zlodzieja, ktory go okradl.
		public static readonly bool SuspendOnMurder = false;

		public static bool IsInGuild( Mobile m )
		{
			return ( m is PlayerMobile && ((PlayerMobile)m).NpcGuild == NpcGuild.ThievesGuild );
		}

		public static bool IsInnocentTo( Mobile from, Mobile to )
		{
			return ( Notoriety.Compute( from, (Mobile)to ) == Notoriety.Innocent );
		}

		private class StealingTarget : Target
		{
			private Mobile m_Thief;

			public StealingTarget( Mobile thief ) : base ( 1, false, TargetFlags.None )
			{
				m_Thief = thief;

				AllowNonlocal = true;
			}

            // zombie - 19-06-2012 - dodanie mozliwosci okradania zwlok przy uzyciu umiejetnosci
			private Item TryStealItem( Item toSteal, ref bool caught, bool randomSteal )
			{
				Item stolen = null;

				object root = toSteal.RootParent;

				StealableArtifactsSpawner.StealableInstance si = null;
				if ( toSteal.Parent == null || !toSteal.Movable )
					si = StealableArtifactsSpawner.GetStealableInstance( toSteal );

				if ( !IsEmptyHanded( m_Thief ) )
				{
					m_Thief.SendLocalizedMessage( 1005584 ); // Both hands must be free to steal.
				}
				/* // okradanie mozliwe bez przynaleznosci do gildii:
                else if ( root is Mobile && ((Mobile)root).Player && IsInnocentTo( m_Thief, (Mobile)root ) && !IsInGuild( m_Thief ) )
				{
					m_Thief.SendLocalizedMessage( 1005596 ); // You must be in the thieves guild to steal from other players.
				}*/
				/* // zabojcy moga okradac:
                else if ( SuspendOnMurder && root is Mobile && ((Mobile)root).Player && IsInGuild( m_Thief ) && m_Thief.Kills > 0 )
				{
					m_Thief.SendLocalizedMessage( 502706 ); // You are currently suspended from the thieves guild.
				}*/
				else if ( root is BaseVendor && ((BaseVendor)root).IsInvulnerable )
				{
					m_Thief.SendLocalizedMessage( 1005598 ); // You can't steal from shopkeepers.
				}
				else if ( root is PlayerVendor )
				{
					m_Thief.SendLocalizedMessage( 502709 ); // You can't steal from vendors.
				}
				else if ( !m_Thief.CanSee( toSteal ) )
				{
					m_Thief.SendLocalizedMessage( 500237 ); // Target can not be seen.
				}
				else if ( m_Thief.Backpack == null || !m_Thief.Backpack.CheckHold( m_Thief, toSteal, false, true ) )
				{
					m_Thief.SendLocalizedMessage( 1048147 ); // Your backpack can't hold anything else.
				}
				#region Sigils
				else if ( toSteal is Sigil )
				{						
					PlayerState pl = PlayerState.Find( m_Thief );
					Faction faction = ( pl == null ? null : pl.Faction );

					Sigil sig = (Sigil) toSteal;

					if ( !m_Thief.InRange( toSteal.GetWorldLocation(), 1 ) )
					{
						m_Thief.SendLocalizedMessage( 502703 ); // You must be standing next to an item to steal it.
					}
					else if ( root != null ) // not on the ground
					{
						m_Thief.SendLocalizedMessage( 502710 ); // You can't steal that!
					}
					else if ( faction != null )
					{
						if ( !m_Thief.CanBeginAction( typeof( IncognitoSpell ) ) )
						{
							m_Thief.SendLocalizedMessage( 1010581 ); //	You cannot steal the sigil when you are incognito
						}
						else if ( DisguiseGump.IsDisguised( m_Thief ) )
						{
							m_Thief.SendLocalizedMessage( 1010583 ); //	You cannot steal the sigil while disguised
						}
						else if ( !m_Thief.CanBeginAction( typeof( PolymorphSpell ) ) )
						{
							m_Thief.SendLocalizedMessage( 1010582 ); //	You cannot steal the sigil while polymorphed				
						}
						else if( TransformationSpellHelper.UnderTransformation( m_Thief ) )
						{
							m_Thief.SendLocalizedMessage( 1061622 ); // You cannot steal the sigil while in that form.
						}
						else if ( AnimalForm.UnderTransformation( m_Thief ) )
						{
							m_Thief.SendLocalizedMessage( 1063222 ); // You cannot steal the sigil while mimicking an animal.
						}
						else if ( pl.IsLeaving )
						{
							m_Thief.SendLocalizedMessage( 1005589 ); // You are currently quitting a faction and cannot steal the town sigil
						}
						else if ( sig.IsBeingCorrupted && sig.LastMonolith.Faction == faction )
						{
							m_Thief.SendLocalizedMessage( 1005590 ); //	You cannot steal your own sigil
						}
						else if ( sig.IsPurifying )
						{
							m_Thief.SendLocalizedMessage( 1005592 ); // You cannot steal this sigil until it has been purified
						}
						else if ( m_Thief.CheckTargetSkill( SkillName.Stealing, toSteal, 80.0, 80.0 ) )
						{
							if ( Sigil.ExistsOn( m_Thief ) )
							{
								m_Thief.SendLocalizedMessage( 1010258 ); //	The sigil has gone back to its home location because you already have a sigil.
							}
							else if ( m_Thief.Backpack == null || !m_Thief.Backpack.CheckHold( m_Thief, sig, false, true ) )
							{
								m_Thief.SendLocalizedMessage( 1010259 ); //	The sigil has gone home because your backpack is full
							}
							else
							{
								if ( sig.IsBeingCorrupted )
									sig.GraceStart = DateTime.Now; // begin grace period

								m_Thief.SendLocalizedMessage( 1010586 ); // YOU STOLE THE SIGIL!!!   (woah, calm down now)

								if ( sig.LastMonolith != null )
									sig.LastMonolith.Sigil = null;

								sig.LastStolen = DateTime.Now;

								return sig;
							}
						}
						else
						{
							m_Thief.SendLocalizedMessage( 1005594 ); //	You do not have enough skill to steal the sigil
						}
					}
					else
					{
						m_Thief.SendLocalizedMessage( 1005588 ); //	You must join a faction to do that
					}
				}
				#endregion
				else if ( si == null && ( toSteal.Parent == null || !toSteal.Movable ) && !ItemFlags.GetStealable(toSteal) )
				{
					m_Thief.SendLocalizedMessage( 502710 ); // You can't steal that!
				}
				else if ( (toSteal.LootType == LootType.Newbied || toSteal.CheckBlessed( root )) && !ItemFlags.GetStealable(toSteal))
				{
					m_Thief.SendLocalizedMessage( 502710 ); // You can't steal that!
				}
				else if ( si == null && toSteal is Container && !ItemFlags.GetStealable(toSteal))
				{
					m_Thief.SendLocalizedMessage( 502710 ); // You can't steal that!
				}
				else if ( !m_Thief.InRange( toSteal.GetWorldLocation(), 1 ) )
				{
					m_Thief.SendLocalizedMessage( 502703 ); // You must be standing next to an item to steal it.
				}
				else if ( si != null && m_Thief.Skills[SkillName.Stealing].Value < 100.0 )
				{
					m_Thief.SendLocalizedMessage( 1060025, "", 0x66D ); // You're not skilled enough to attempt the theft of this item.
				}
				else if ( toSteal.Parent is Mobile )
				{
					m_Thief.SendLocalizedMessage( 1005585 ); // You cannot steal items which are equiped.
				}
				else if ( root == m_Thief )
				{
					m_Thief.SendLocalizedMessage( 502704 ); // You catch yourself red-handed.
				}
				else if ( root is Mobile && ((Mobile)root).AccessLevel > AccessLevel.Player )
				{
					m_Thief.SendLocalizedMessage( 502710 ); // You can't steal that!
				}
				else if ( root is Mobile && !m_Thief.CanBeHarmful( (Mobile)root ) )
				{
				}
				//else if ( root is Corpse )
				//{
					//m_Thief.SendLocalizedMessage( 502710 ); // You can't steal that!
				//}
				else
				{
                    double m1 = 0, m2 = 0;

                    if ( root is Corpse )
                    {
                        SkillName[] skillNames = new SkillName[]
                        {
                            SkillName.Snooping,
                            SkillName.Hiding,
                            SkillName.Stealth
                        };

                        double skillsTotal = 0, skillsMax = 0;

                        foreach ( SkillName skillName in skillNames )
                        {
                            int cap = 100;
                            skillsTotal += m_Thief.Skills[ skillName ].Value;

                            foreach( SkillName ps in PowerScroll.Skills )
                            {
                                if ( skillName == ps )
                                {
                                    cap += 20;
                                    break;
                                }
                            }

                            skillsMax += cap;
                        }

                        m1 = m2 = 120 - ( 120 * ( skillsTotal / skillsMax ) );

                        //m_Thief.SendMessage("m1: " + m1 + " m2: " + m2);
                    }

					double w = toSteal.Weight + toSteal.TotalWeight;

					if ( w > 10 )
					{
						m_Thief.SendMessage( "That is too heavy to steal." );
					}
					else
					{
						if ( toSteal.Stackable && toSteal.Amount > 1 )
						{
							int maxAmount = (int)((m_Thief.Skills[SkillName.Stealing].Value / 10.0) / toSteal.Weight);

							if ( maxAmount < 1 )
								maxAmount = 1;
							else if ( maxAmount > toSteal.Amount )
								maxAmount = toSteal.Amount;

							int amount = Utility.RandomMinMax( 1, maxAmount );

							if ( amount >= toSteal.Amount )
							{
								int pileWeight = (int)Math.Ceiling( toSteal.Weight * toSteal.Amount );
								pileWeight *= 10;

								if ( m_Thief.CheckTargetSkill( SkillName.Stealing, toSteal, pileWeight - 22.5 + m1, pileWeight + 27.5 + m2 ) )
									stolen = toSteal;
							}
							else
							{
								int pileWeight = (int)Math.Ceiling( toSteal.Weight * amount );
								pileWeight *= 10;

								if ( m_Thief.CheckTargetSkill( SkillName.Stealing, toSteal, pileWeight - 22.5 + m1, pileWeight + 27.5 + m2) )
								{
									stolen = Mobile.LiftItemDupe( toSteal, toSteal.Amount - amount );

									if ( toSteal.ModifiedBy != null )
									{
										stolen.ModifiedBy = toSteal.ModifiedBy;
										if(toSteal.ModifiedDate != null) {
											stolen.ModifiedDate = toSteal.ModifiedDate;
										}
									}
									
									if ( stolen == null )
										stolen = toSteal;
								}
							}
						}
						else
						{
							int iw = (int)Math.Ceiling( w );
							iw *= 10;

							if ( m_Thief.CheckTargetSkill( SkillName.Stealing, toSteal, iw - 22.5 + m1, iw + 27.5 + m2 ) )
								stolen = toSteal;
						}

						if ( stolen != null )
						{
							m_Thief.SendLocalizedMessage( 502724 ); // You succesfully steal the item.

							ItemFlags.SetTaken(stolen, true);
							ItemFlags.SetStealable(stolen, false);
							stolen.Movable = true;

							if ( si != null )
							{
								toSteal.Movable = true;
								si.Item = null;
							}
						}
						else
						{
							m_Thief.SendLocalizedMessage( 502723 ); // You fail to steal the item.
						}

                        // 04.07.2012 :: zombie :: modyfikacja szansy na zlapanie
                        // http://forum.nelderim.org/viewtopic.php?p=3720#3720
                        bool success = stolen != null;
                        Skills s = m_Thief.Skills;

                        double factor = randomSteal ? ( success ? 1.0 : 0.66 ) : ( success ? 0.8 : 0.5 );

                        // 5 * stealing + snooping + hiding + stealth 
                        double skillsTotal  = 5 * s[SkillName.Stealing].Value + s[SkillName.Snooping].Value + s[SkillName.Hiding].Value + s[SkillName.Stealth].Value;
                        double skillsMax    = 5 * 120 + 100 + 100 + 120;
                        double caughtChance = 1 - (double)( skillsTotal / skillsMax * factor );

                        caught = caughtChance > Utility.RandomDouble();

                        //m_Thief.SendMessage( 0x20, "success: {0}, randomSteal: {1}, skillsTotal: {2}, skillsMax {3}, factor: {4}, caught chance {5}, caught: {6}", success, randomSteal, skillsTotal, skillsMax, factor, caughtChance, caught );
                        // zombie
					}
				}

				return stolen;
			}
            // zombie

			protected override void OnTarget( Mobile from, object target )
			{
				Item stolen = null;
				object root = null;
				bool caught = false;

				// kradziez planowana
                if ( target is Item && !( target is Corpse ) ) 
                {
                    root = ((Item)target).RootParent;
					stolen = TryStealItem( (Item)target, ref caught, false );
                }
                // kradziez losowa
                else if ( target is Mobile || target is Corpse )
				{
                    root = target;
					Container pack = target is Mobile ? ((Mobile)target).Backpack : (Container)target;
                    
					if ( pack != null && pack.Items.Count > 0 )
					{
                        List<Item> stealableItems = new List<Item>();
                        foreach (Item item in pack.Items)
                        {
                            if (item is Container || item.LootType == LootType.Newbied || item.CheckBlessed(root))
                                continue;
                            stealableItems.Add(item);
                        }

                        if (stealableItems.Count > 0)
                        {
                            int randomIndex = Utility.Random(stealableItems.Count);

                            stolen = TryStealItem(stealableItems[randomIndex], ref caught, true);
                        }
                        else
                        {
                            from.SendLocalizedMessage(1045041); // Nie ma stad czego ukrasc...
                        }
					}
                    else
                    {
                        from.SendLocalizedMessage(1045041); // Nie ma stad czego ukrasc...
                    }
				} 					
				else 
				{
					m_Thief.SendLocalizedMessage( 502710 ); // You can't steal that!
				}

                if ( stolen != null )
				{
					from.AddToBackpack( stolen );
					
                    Mobile mobRoot = null;
					if( root is Mobile )
						mobRoot = (Mobile)root;
					/*
					// zwracanie skradzionego ze zwlok itemu do plecaka ofiary w razie smierci zlodzieja:
					else if( root is Corpse );
							mobRoot = ((Corpse)root).Owner
					*/

					if( mobRoot == null )
						return;
						
                    StolenItem.Add( stolen, m_Thief, mobRoot );
				}

                if ( caught )
                {
                    from.SendLocalizedMessage(1045042, "", 38); // Zostales przylapany na kradziezy!
                    from.RevealingAction();

                    // Fetch the player character that owned the stolen item
                    Mobile mobRoot = root as Mobile;
                    if ( root is Corpse )
                        mobRoot = ((Corpse)root).Owner;
                    else if (root is BaseCreature )
                    {
                        BaseCreature bcRoot = root as BaseCreature;
                        if (bcRoot.Controlled)
                            mobRoot = bcRoot.ControlMaster;
                        if (bcRoot.Summoned)
                            mobRoot = bcRoot.SummonMaster;
                    }

                    if (mobRoot == null)
                        return;

                    if (mobRoot is PlayerMobile)
                    {
                        if (!IsInGuild(mobRoot) && m_Thief.IsHarmfulCriminal(mobRoot))
                        {
                            // Okradanie blue daje status krima zlodziejowi:
                            m_Thief.Criminal = true;
                        }
                        else
                        {
                            // Okradanie reda/crima daje status CBA zlodziejowi w oczach okradnietego na 2 min:
                            PlayerMobile pmThief = m_Thief as PlayerMobile;
                            if (pmThief != null)
                            {
                                pmThief.PermaFlags.Add(mobRoot);
                                pmThief.Delta(MobileDelta.Noto);

                                PermaGreyExpireTimer tim = new PermaGreyExpireTimer(pmThief, mobRoot, TimeSpan.FromMinutes(2.0));
                                tim.Start();
                            }
                        }
                    }

                    string message = String.Format("You notice {0} trying to steal from {1}.", m_Thief.Name, mobRoot.Name);

                    IPooledEnumerable eable = m_Thief.GetClientsInRange(8);
                    foreach ( NetState ns in eable )
                    {
                        if ( ns.Mobile != m_Thief )
                            ns.Mobile.SendMessage( message );
                    }
                    eable.Free();
                }

                /*
                if ( root is Mobile && ((Mobile)root).Player && m_Thief is PlayerMobile && IsInnocentTo( m_Thief, (Mobile)root ) && !IsInGuild( (Mobile)root ) )
                {
                    PlayerMobile pm = (PlayerMobile)m_Thief;

                    pm.PermaFlags.Add( (Mobile)root );
                    pm.Delta( MobileDelta.Noto );
                }
                 */
            }
		}

		public static bool IsEmptyHanded( Mobile from )
		{
			if ( from.FindItemOnLayer( Layer.OneHanded ) != null )
				return false;

			if ( from.FindItemOnLayer( Layer.TwoHanded ) != null )
				return false;

			return true;
		}

		public static TimeSpan OnUse( Mobile m )
		{
			if ( !IsEmptyHanded( m ) )
			{
				m.SendLocalizedMessage( 1005584 ); // Both hands must be free to steal.
			}
			else
			{
				m.Target = new Stealing.StealingTarget( m );
                // zombie - 19-06-2012
				//m.RevealingAction();
                // zombie

				m.SendLocalizedMessage( 502698 ); // Which item do you want to steal?
			}

            return TimeSpan.FromSeconds( 10.0 );
		}

        private class PermaGreyExpireTimer : Timer
        {
            private PlayerMobile m_Thief;
            private Mobile m_Victim;

            public PermaGreyExpireTimer(PlayerMobile thief, Mobile victim, TimeSpan delay)
                : base(delay)
            {
                Priority = TimerPriority.OneSecond;
                m_Thief = thief;
                m_Victim = victim;
            }

            protected override void OnTick()
            {
                m_Thief.PermaFlags.Remove(m_Victim);
                m_Thief.Delta(MobileDelta.Noto);
            }
        }
	}

	public class StolenItem
	{
		public static readonly TimeSpan StealTime = TimeSpan.FromMinutes( 2.0 );

		private Item m_Stolen;
		private Mobile m_Thief;
		private Mobile m_Victim;
		private DateTime m_Expires;

		public Item Stolen{ get{ return m_Stolen; } }
		public Mobile Thief{ get{ return m_Thief; } }
		public Mobile Victim{ get{ return m_Victim; } }
		public DateTime Expires{ get{ return m_Expires; } }

		public bool IsExpired{ get{ return ( DateTime.Now >= m_Expires ); } }

		public StolenItem( Item stolen, Mobile thief, Mobile victim )
		{
			m_Stolen = stolen;
			m_Thief = thief;
			m_Victim = victim;

			m_Expires = DateTime.Now + StealTime;
		}

		private static Queue m_Queue = new Queue();

		public static void Add( Item item, Mobile thief, Mobile victim )
		{
			Clean();

			m_Queue.Enqueue( new StolenItem( item, thief, victim ) );
		}

		public static bool IsStolen( Item item )
		{
			Mobile victim = null;

			return IsStolen( item, ref victim );
		}

		public static bool IsStolen( Item item, ref Mobile victim )
		{
			Clean();

			foreach ( StolenItem si in m_Queue )
			{
				if ( si.m_Stolen == item && !si.IsExpired )
				{
					victim = si.m_Victim;
					return true;
				}
			}

			return false;
		}

		public static void ReturnOnDeath( Mobile killed, Container corpse )
		{
			Clean();

			foreach ( StolenItem si in m_Queue )
			{
				if ( si.m_Stolen.RootParent == corpse && si.m_Victim != null && !si.IsExpired )
				{
					if ( si.m_Victim.AddToBackpack( si.m_Stolen ) )
						si.m_Victim.SendLocalizedMessage( 1010464 ); // the item that was stolen is returned to you.
					else
						si.m_Victim.SendLocalizedMessage( 1010463 ); // the item that was stolen from you falls to the ground.

					si.m_Expires = DateTime.Now; // such a hack
				}
			}
		}

		public static void Clean()
		{
			while ( m_Queue.Count > 0 )
			{
				StolenItem si = (StolenItem) m_Queue.Peek();

				if ( si.IsExpired )
					m_Queue.Dequeue();
				else
					break;
			}
		}
	}
}