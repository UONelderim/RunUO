//=================================================
//This script was created by Gizmo's Uo Quest Maker
//This script was created on 16/03/2020 14:46:01
//=================================================
using System;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "resztki wladcy otchlani" )]
	public class AbyssLord : BaseCreature
	{
		[Constructable]
		public AbyssLord()
        : base(AIType.AI_NecroMage, FightMode.Closest, 10, 1, 0.2, 0.4)
		{
			Name = "Wladca Otchlani";
            
            this.Body = 174;
            this.BaseSoundID = 0x4B0;
            this.Kills = 10;

            this.SetStr(502, 600);
            this.SetDex(102, 200);
            this.SetInt(601, 750);

            this.SetHits(10000);
            this.SetStam(103, 250);

            this.SetDamage(29, 35);

            this.SetDamageType(ResistanceType.Physical, 75);
            this.SetDamageType(ResistanceType.Fire, 25);

            this.SetResistance(ResistanceType.Physical, 75, 90);
            this.SetResistance(ResistanceType.Fire, 65, 75);
            this.SetResistance(ResistanceType.Cold, 60, 70);
            this.SetResistance(ResistanceType.Poison, 65, 75);
            this.SetResistance(ResistanceType.Energy, 65, 75);

            this.SetSkill(SkillName.EvalInt, 95.1, 100.0);
            this.SetSkill(SkillName.Magery, 90.1, 105.0);
            this.SetSkill(SkillName.Meditation, 95.1, 100.0);
            this.SetSkill(SkillName.MagicResist, 120.2, 140.0);
            this.SetSkill(SkillName.Tactics, 90.1, 105.0);
            this.SetSkill(SkillName.Wrestling, 90.1, 105.0);
            this.SetSkill(SkillName.Hiding, 95.1, 100.0);
            this.SetSkill(SkillName.Necromancy, 95.1, 100.0);
            this.SetSkill(SkillName.DetectHidden, 95.1, 100.0);

            this.Fame = 2400;
            this.Karma = -2400;
            
            
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Potions, 5 );
			AddLoot( LootPack.SuperBoss, 1 );
			AddLoot( LootPack.FilthyRich, 2 );
		}

		public AbyssLord( Serial serial ) : base( serial )
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
