using System;
using Server;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
	[CorpseName( "zwłoki wielkiego maga vox populi" )] 
	public class DragonsFlameGrandMage : DragonsFlameMage
	{
		public override double DifficultyScalar{ get{ return 1.02; } }
		public override bool AlwaysMurderer{ get{ return true; } }
		public override bool ShowFameTitle{ get{ return false; } }

		[Constructable]
		public DragonsFlameGrandMage() : base()
		{
			Name = "wielki mag";
			Title = "- Przynależy do Vox Populi";
		}

		public DragonsFlameGrandMage( Serial serial ) : base( serial )
		{
		}
		
		public override void GenerateLoot()
		{
			AddLoot( LootPack.AosFilthyRich, 6 );
		}
		
		public override void OnDeath( Container c )
		{
			base.OnDeath( c );	
			
			/*if ( Utility.RandomDouble() < 0.3 )
				c.DropItem( new DragonFlameKey() );*/
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
