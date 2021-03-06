using System;
using System.Collections;
using Server;
using Server.Items;
using Server.Targeting;
using Server.Engines.Quests;

namespace Server.Mobiles
{
	public class Gregorio : BaseCreature
	{						
		public override bool InitialInnocent{ get{ return true; } }
		[Constructable]
		public Gregorio() : base( AIType.AI_Melee, FightMode.Aggressor, 12, 1, 0.2, 0.4 )
		{			
			Race = None.Instance;
			Name = "Gregorio";
			Title = "the brigand";
			
			InitBody();
			InitOutfit();
			
			SetStr( 86, 100 );
			SetDex( 81, 95 );
			SetInt( 61, 75 );

			SetDamage( 15, 27 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 10, 15 );
			SetResistance( ResistanceType.Fire, 10, 15 );
			SetResistance( ResistanceType.Poison, 10, 15 );
			SetResistance( ResistanceType.Energy, 10, 15 );

			SetSkill( SkillName.MagicResist, 25.0, 50.0 );
			SetSkill( SkillName.Tactics, 80.0, 100.0 );
			SetSkill( SkillName.Wrestling, 80.0, 100.0 );	
			
			PackGold( 50, 150 );
		}
		
		public Gregorio( Serial serial ) : base( serial )
		{
		}
		
		public virtual void InitBody()
		{
			InitStats( 100, 100, 25 );
				
			Hue = 0x8412;
			Female = false;		
			
			HairItemID = 0x203C;
			HairHue = 0x47A;
			FacialHairItemID = 0x204D;
			FacialHairHue = 0x47A;
		}
		
		public virtual void InitOutfit()
		{						
			AddItem( new Sandals( 0x75E ) );
			AddItem( new Shirt() );
			AddItem( new ShortPants( 0x66C ) );
			AddItem( new SkullCap( 0x649 ) );
			AddItem( new Pitchfork() );
		}
		
		/*public static bool IsMurderer( Mobile from )
		{			
			if ( from != null && from is PlayerMobile )
			{
				BaseQuest quest = QuestHelper.GetQuest( (PlayerMobile) from, typeof( GuiltyQuest ) );

				if ( quest != null )
					return !quest.Completed;
			}
				
			return false;
		}*/
		
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