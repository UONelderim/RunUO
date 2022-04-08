// 05.08.30 :: troyan :: zmiany predkosci & zasiegu & FightMode

using System;
using System.Collections;
using Server.Items;
using Server.Targeting;
using Server.Engines.Quests;
using Server.Engines.Quests.Haven;
using Server.ContextMenus;
using System.Collections.Generic;

namespace Server.Mobiles
{
	[CorpseName( "resztki wygnanej duszy" )]
	public class NelderimRestlessSoul : BaseCreature
	{
		[Constructable]
		public NelderimRestlessSoul() : base( AIType.AI_Melee, FightMode.Closest, 8, 1, 0.2, 0.4 )
		{
			Name = "wygnana dusza";
			Body = 0x3CA;
			Hue = 0x453;

			SetStr( 26, 40 );
			SetDex( 26, 40 );
			SetInt( 26, 40 );

			SetHits( 16, 124 );

			SetDamage( 1, 20 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 15, 55 );
			SetResistance( ResistanceType.Fire, 5, 55 );
			SetResistance( ResistanceType.Cold, 25, 50 );
			SetResistance( ResistanceType.Poison, 5, 50 );
			SetResistance( ResistanceType.Energy, 10, 50 );

			SetSkill( SkillName.MagicResist, 20.1, 100.0 );
			SetSkill( SkillName.Swords, 20.1, 100.0 );
			SetSkill( SkillName.Tactics, 20.1, 100.0 );
			SetSkill( SkillName.Wrestling, 20.1, 100.0 );

			Fame = 500;
			Karma = -500;

			VirtualArmor = 20;
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Poor, 5 );
		}

		public override bool AlwaysAttackable{ get{ return true; } }

		public override void DisplayPaperdollTo(Mobile to)
		{
		}

        public override void GetContextMenuEntries( Mobile from, List<ContextMenuEntry> list )
        {
			base.GetContextMenuEntries( from, list );

			for ( int i = 0; i < list.Count; ++i )
			{
				if ( list[i] is ContextMenus.PaperdollEntry )
					list.RemoveAt( i-- );
			}
		}

		public override int GetIdleSound()
		{
			return 0x1BF;
		}

		public override int GetAngerSound()
		{
			return 0x107;
		}

		public override int GetDeathSound()
		{
			return 0xFD;
		}

		public override bool IsEnemy( Mobile m )
		{
			PlayerMobile player = m as PlayerMobile;

			if ( player != null && Map == Map.Trammel && X >= 5199 && X <= 5271 && Y >= 1812 && Y <= 1865 ) // Schmendrick's cave
			{
				QuestSystem qs = player.Quest;

				if ( qs is UzeraanTurmoilQuest && qs.IsObjectiveInProgress( typeof( FindSchmendrickObjective ) ) )
				{
					return false;
				}
			}

			return base.IsEnemy( m );
		}

		public NelderimRestlessSoul( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}
