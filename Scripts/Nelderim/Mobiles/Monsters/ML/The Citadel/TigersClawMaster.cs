using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "zwłoki mistrza vox populi" )] 
	public class TigersClawMaster : TigersClawThief
	{
		public override double DifficultyScalar{ get{ return 1.02; } }
		public override bool AlwaysMurderer{ get{ return true; } }
		public override bool ShowFameTitle{ get{ return false; } }

		[Constructable]
		public TigersClawMaster() : base()
		{
			Name = "Mistrz";
			Title = "- Przynależy do Vox Populi";
		}

		public TigersClawMaster( Serial serial ) : base( serial )
		{
		}
		
		public override void GenerateLoot()
		{
			AddLoot( LootPack.AosFilthyRich, 6 );
		}		
		
		public override void OnDeath( Container c )
		{
			base.OnDeath( c );	
			
			/*if ( Utility.RandomDouble() < 0.2 )
				c.DropItem( new TigerClawKey() );*/
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
