using System;
using System.Collections.Generic;
using Server;
using Server.ContextMenus;
using Server.Engines.PartySystem;
using Server.Gumps;
using Server.Multis;
using Server.Network;

namespace Server.Items
{
	public class TreasureMapChest : LockableContainer
	{
		public override int LabelNumber{ get{ return 3000541; } }

		private int m_Level;
		private DateTime m_DeleteTime;
		private Timer m_Timer;
		private Mobile m_Owner;
		private bool m_Temporary;

		private List<Mobile> m_Guardians;

		[CommandProperty( AccessLevel.GameMaster )]
		public int Level{ get{ return m_Level; } set{ m_Level = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public Mobile Owner{ get{ return m_Owner; } set{ m_Owner = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public DateTime DeleteTime{ get{ return m_DeleteTime; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public bool Temporary{ get{ return m_Temporary; } set{ m_Temporary = value; } }

		public List<Mobile> Guardians { get { return m_Guardians; } }

		[Constructable]
		public TreasureMapChest( int level ) : this( null, level, false )
		{
		}

		public TreasureMapChest( Mobile owner, int level, bool temporary ) : base( 0xE40 )
		{
			m_Owner = owner;
			m_Level = level;
			m_DeleteTime = DateTime.Now + TimeSpan.FromHours( 3.0 );

			m_Temporary = temporary;
			m_Guardians = new List<Mobile>();

			m_Timer = new DeleteTimer( this, m_DeleteTime );
			m_Timer.Start();

			Fill( this, level );
		}

		private static void GetRandomAOSStats( out int attributeCount, out int min, out int max )
		{
			int rnd = Utility.Random( 15 );

			if ( rnd < 1 )
			{
				attributeCount = Utility.RandomMinMax( 2, 6 );
				min = 20; max = 70;
			}
			else if ( rnd < 3 )
			{
				attributeCount = Utility.RandomMinMax( 2, 4 );
				min = 20; max = 50;
			}
			else if ( rnd < 6 )
			{
				attributeCount = Utility.RandomMinMax( 2, 3 );
				min = 20; max = 40;
			}
			else if ( rnd < 10 )
			{
				attributeCount = Utility.RandomMinMax( 1, 2 );
				min = 10; max = 30;
			}
			else
			{
				attributeCount = 1;
				min = 10; max = 20;
			}
		}

		public static void Fill( LockableContainer cont, int level )
		{
			cont.Movable = false;
			cont.Locked = true;
			

			/*int mapka = level + 1;
			{
				Item item = TreasureMapLevel{ get{ return mapka; } };;;
				cont.DropItem( item );
			}*/

			
			


			if ( level == 0 )
			{
				cont.LockLevel = 0; // Can't be unlocked

				cont.DropItem( new Gold( Utility.RandomMinMax( 50, 100 ) ) );

                // brak mapek 0 lvl
				//if ( Utility.RandomDouble() < 0.75 )
				//	cont.DropItem( new TreasureMap( 0, Map.Trammel ) );
			}
			else
			{
				cont.TrapType = TrapType.ExplosionTrap;
				cont.TrapPower = level * 25;
				cont.TrapLevel = level;

				switch ( level )
				{
					case 1: cont.RequiredSkill = 36; break;
					case 2: cont.RequiredSkill = 76; break;
					case 3: cont.RequiredSkill = 84; break;
					case 4: cont.RequiredSkill = 92; break;
					case 5: cont.RequiredSkill = 100; break;
					case 6: cont.RequiredSkill = 100; break;
				}

				cont.LockLevel = cont.RequiredSkill - 10;
				cont.MaxLockLevel = cont.RequiredSkill + 40;

				cont.DropItem( new Gold( level * 1000 ) );
				
				if ( Utility.RandomDouble() < 0.40 )
				{
					cont.DropItem ( new TreasureMap(Math.Min(6, level + 1), Map.Felucca));
				}

                // Losowanie spell scroll:
                LootPackItem[] spellCircle = null;
                int circleMin, circleMax;
                switch(level)
                {
                    default: { circleMin = 1; circleMax = 1; break; }
                    case 1: { circleMin = 1; circleMax = 3; break; }
                    case 2: { circleMin = 2; circleMax = 4; break; }
                    case 3: { circleMin = 3; circleMax = 5; break; }
                    case 4: { circleMin = 4; circleMax = 7; break; }
                    case 5: { circleMin = 6; circleMax = 8; break; }
                    case 6: { circleMin = 7; circleMax = 8; break; }
                }
                for ( int i = 0; i < level * 5; ++i )
                {
                    switch(Utility.RandomMinMax(circleMin, circleMax))
                    {
                        default: { spellCircle = LootPack.NL_Scrolls1; break; }
                        case 1: { spellCircle = LootPack.NL_Scrolls1; break; }
                        case 2: { spellCircle = LootPack.NL_Scrolls2; break; }
                        case 3: { spellCircle = LootPack.NL_Scrolls3; break; }
                        case 4: { spellCircle = LootPack.NL_Scrolls4; break; }
                        case 5: { spellCircle = LootPack.NL_Scrolls5; break; }
                        case 6: { spellCircle = LootPack.NL_Scrolls6; break; }
                        case 7: { spellCircle = LootPack.NL_Scrolls7; break; }
                        case 8: { spellCircle = LootPack.NL_Scrolls8; break; }
                    }
                    Item scroll = Loot.Construct(spellCircle[Utility.Random(spellCircle.Length)].Type);
                    if( scroll != null )
					    cont.DropItem( scroll );
                }

                int minProp, maxProp;
                int minInt, maxInt;
				int itemLvl;
				double difficulty = 0;

                int[] typeChances = new int[]
                {
                    LootPack.WeaponChance,
                    LootPack.RangedChance,
                    LootPack.ArmorChance,
                    LootPack.HatChance,
                    LootPack.ShieldChance,
                    LootPack.JewelChance,
                };

				// Zaleznosc jakosci magicznych przedmiotow od poziomu skarbu:
				// (Jakosc przedmiotu wyrazana w 'difficulty', co przeklada sie na 'level' itemu (patrz LootPack.cs))
				switch (level)
				{
					default: { difficulty = 0; break; }		// 0,0: 100% brak itemka
					case 1: { difficulty = 30; break; }	// 1,0:	64% 1-lvl	32% 2-lvl	3% 3-lvl
					case 2: { difficulty = 100; break; }	// 1,0:	64% 1-lvl	32% 2-lvl	3% 3-lvl
					case 3: { difficulty = 400; break; }	// 7,9:	90% 2-lvl	9% 3-lvl	1% 4-lvl
					case 4: { difficulty = 1500; break; }	// 735:	86% 3-lvl	11% 4-lvl	3% 5-lvl
					case 5: { difficulty = 17000; break; }	// 15k:	39% 3-lvl	49% 4-lvl	12% 5-lvl
					case 6: { difficulty = 19000; break; }	// 20k:	30% 3-lvl	57% 4-lvl	14% 5-lvl
				}

                for (int i = 0; i < level * 4; ++i)
				{
					Item item;

					// Losowanie TYPU magicznego przedmiotu:
                    int totalChance = 0;

                    for (int j = 0; j < typeChances.Length; ++j)
                        totalChance += typeChances[j];

                    int rnd = Utility.Random( totalChance );

                    int itemType = -1;
                    for (int j = 0; j < typeChances.Length; ++j)
                    {
                        if (rnd < typeChances[j])
                        {
                            itemType = j;
                            break;
                        }
                        rnd -= typeChances[j];
                    }

                    switch(itemType)
                    {
                        default: { item = Loot.RandomArmorOrShieldOrWeaponOrJewelry(); break; }
                        case 0: { item = Loot.RandomWeapon(); break; }
                        case 1: { item = Loot.RandomRangedWeapon(); break; }
                        case 2: { item = Loot.RandomArmor(); break; }
                        case 3: { item = Loot.RandomHat(); break; }
                        case 4: { item = Loot.RandomShield(); break; }
                        case 5: { item = Loot.RandomJewelry(); break; }
                    }
			
					// wylosuj moc propsow:
					LootPack.GenPropsIntensity( difficulty, out minProp, out maxProp, out minInt, out maxInt, out itemLvl);
					int attributeCount = Utility.RandomMinMax(minProp, maxProp);

                    // dodatkowe resy:
                    LootPack.GiveAdditionalResists(item, itemLvl);

					if ( item is BaseWeapon )
					{
						BaseRunicTool.ApplyAttributesTo((BaseWeapon)item, false, 0, attributeCount, minInt, maxInt, LootPack.RandomizerForItemLevel(itemLvl) );

						cont.DropItem( item );
					}
					else if ( item is BaseArmor )
					{
						BaseRunicTool.ApplyAttributesTo((BaseArmor)item, false, 0, attributeCount, minInt, maxInt, LootPack.RandomizerForItemLevel(itemLvl) );

						cont.DropItem( item );
					}
					else if( item is BaseHat )
					{
						BaseRunicTool.ApplyAttributesTo((BaseHat)item, false, 0, attributeCount, minInt, maxInt, LootPack.RandomizerForItemLevel(itemLvl) );

						cont.DropItem( item );
					}
					else if( item is BaseJewel )
					{
						BaseRunicTool.ApplyAttributesTo((BaseJewel)item, false, 0, attributeCount, minInt, maxInt, LootPack.RandomizerForItemLevel(itemLvl) );

						cont.DropItem( item );
					}
				}
			}

			int reagents = level * 1;
			for ( int i = 0; i < reagents; i++ )
			{
				Item item = Loot.RandomPossibleReagent();
				item.Amount = Utility.RandomMinMax( 15, 25 );
				cont.DropItem( item );
			}

			int gems;
			if ( level == 0 )
				gems = 2;
			else
				gems = level * 3;

			for ( int i = 0; i < gems; i++ )
			{
				Item item = Loot.RandomGem();
				cont.DropItem( item );
			}

            if (level == 6)
                cont.DropItem(ArtifactHelper.CreateRandomCartographyArtifact());
				//cont.DropItem( (Item)Activator.CreateInstance( m_Artifacts[Utility.Random(m_Artifacts.Length)] ) );
		}

		public override bool CheckLocked( Mobile from )
		{
			if ( !this.Locked )
				return false;

			if ( this.Level == 0 && from.AccessLevel < AccessLevel.GameMaster )
			{
				foreach ( Mobile m in this.Guardians )
				{
					if ( m.Alive )
					{
						from.SendLocalizedMessage( 1046448 ); // You must first kill the guardians before you may open this chest.
						return true;
					}
				}

				LockPick( from );
				return false;
			}
			else
			{
				return base.CheckLocked( from );
			}
		}

		private List<Item> m_Lifted = new List<Item>();

		private bool CheckLoot( Mobile m, bool criminalAction )
		{
			if ( m_Temporary )
				return false;

			if ( m.AccessLevel >= AccessLevel.GameMaster || m_Owner == null || m == m_Owner )
				return true;

			Party p = Party.Get( m_Owner );

			if ( p != null && p.Contains( m ) )
				return true;

			Map map = this.Map;

			if ( map != null && (map.Rules & MapRules.HarmfulRestrictions) == 0 )
			{
				if ( criminalAction )
					m.CriminalAction( true );
				else
					m.SendLocalizedMessage( 1010630 ); // Taking someone else's treasure is a criminal offense!

				return true;
			}

			m.SendLocalizedMessage( 1010631 ); // You did not discover this chest!
			return false;
		}

		public override bool IsDecoContainer
		{
			get{ return false; }
		}

		public override bool CheckItemUse( Mobile from, Item item )
		{
			return CheckLoot( from, item != this ) && base.CheckItemUse( from, item );
		}

		public override bool CheckLift( Mobile from, Item item, ref LRReason reject )
		{
			return CheckLoot( from, true ) && base.CheckLift( from, item, ref reject );
		}

		public override void OnItemLifted( Mobile from, Item item )
		{
			bool notYetLifted = !m_Lifted.Contains( item );

			from.RevealingAction();

			if ( notYetLifted )
			{
				m_Lifted.Add( item );

				if ( 0.1 >= Utility.RandomDouble() ) // 10% chance to spawn a new monster
					TreasureMap.Spawn( m_Level, GetWorldLocation(), Map, from, false );
			}

			base.OnItemLifted( from, item );
		}

		public override bool CheckHold( Mobile m, Item item, bool message, bool checkItems, int plusItems, int plusWeight )
		{
			if ( m.AccessLevel < AccessLevel.GameMaster )
			{
				m.SendLocalizedMessage( 1048122, "", 0x8A5 ); // The chest refuses to be filled with treasure again.
				return false;
			}

			return base.CheckHold( m, item, message, checkItems, plusItems, plusWeight );
		}

		public TreasureMapChest( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 2 ); // version

			writer.Write( m_Guardians, true );
			writer.Write( (bool) m_Temporary );

			writer.Write( m_Owner );

			writer.Write( (int) m_Level );
			writer.WriteDeltaTime( m_DeleteTime );
			writer.Write( m_Lifted, true );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 2:
				{
					m_Guardians = reader.ReadStrongMobileList();
					m_Temporary = reader.ReadBool();

					goto case 1;
				}
				case 1:
				{
					m_Owner = reader.ReadMobile();

					goto case 0;
				}
				case 0:
				{
					m_Level = reader.ReadInt();
					m_DeleteTime = reader.ReadDeltaTime();
					m_Lifted = reader.ReadStrongItemList();

					if ( version < 2 )
						m_Guardians = new List<Mobile>();

					break;
				}
			}

			if ( !m_Temporary )
			{
				m_Timer = new DeleteTimer( this, m_DeleteTime );
				m_Timer.Start();
			}
			else
			{
				Delete();
			}
		}

		public override void OnAfterDelete()
		{
			if ( m_Timer != null )
				m_Timer.Stop();

			m_Timer = null;

			base.OnAfterDelete();
		}

		public override void GetContextMenuEntries( Mobile from, List<ContextMenuEntry> list )
		{
			base.GetContextMenuEntries( from, list );

			if ( from.Alive )
				list.Add( new RemoveEntry( from, this ) );
		}

		public void BeginRemove( Mobile from )
		{
			if ( !from.Alive )
				return;

			from.CloseGump( typeof( RemoveGump ) );
			from.SendGump( new RemoveGump( from, this ) );
		}

		public void EndRemove( Mobile from )
		{
			if ( Deleted || from != m_Owner || !from.InRange( GetWorldLocation(), 3 ) )
				return;

			from.SendLocalizedMessage( 1048124, "", 0x8A5 ); // The old, rusted chest crumbles when you hit it.
			this.Delete();
		}

		private class RemoveGump : Gump
		{
			private Mobile m_From;
			private TreasureMapChest m_Chest;

			public RemoveGump( Mobile from, TreasureMapChest chest ) : base( 15, 15 )
			{
				m_From = from;
				m_Chest = chest;

				Closable = false;
				Disposable = false;

				AddPage( 0 );

				AddBackground( 30, 0, 240, 240, 2620 );

				AddHtmlLocalized( 45, 15, 200, 80, 1048125, 0xFFFFFF, false, false ); // When this treasure chest is removed, any items still inside of it will be lost.
				AddHtmlLocalized( 45, 95, 200, 60, 1048126, 0xFFFFFF, false, false ); // Are you certain you're ready to remove this chest?

				AddButton( 40, 153, 4005, 4007, 1, GumpButtonType.Reply, 0 );
				AddHtmlLocalized( 75, 155, 180, 40, 1048127, 0xFFFFFF, false, false ); // Remove the Treasure Chest

				AddButton( 40, 195, 4005, 4007, 2, GumpButtonType.Reply, 0 );
				AddHtmlLocalized( 75, 197, 180, 35, 1006045, 0xFFFFFF, false, false ); // Cancel
			}

			public override void OnResponse( NetState sender, RelayInfo info )
			{
				if ( info.ButtonID == 1 )
					m_Chest.EndRemove( m_From );
			}
		}

		private class RemoveEntry : ContextMenuEntry
		{
			private Mobile m_From;
			private TreasureMapChest m_Chest;

			public RemoveEntry( Mobile from, TreasureMapChest chest ) : base( 6149, 3 )
			{
				m_From = from;
				m_Chest = chest;

				Enabled = ( from == chest.Owner );
			}

			public override void OnClick()
			{
				if ( m_Chest.Deleted || m_From != m_Chest.Owner || !m_From.CheckAlive() )
					return;

				m_Chest.BeginRemove( m_From );
			}
		}

		private class DeleteTimer : Timer
		{
			private Item m_Item;

			public DeleteTimer( Item item, DateTime time ) : base( time - DateTime.Now )
			{
				m_Item = item;
				Priority = TimerPriority.OneMinute;
			}

			protected override void OnTick()
			{
				m_Item.Delete();
			}
		}
	}
}
