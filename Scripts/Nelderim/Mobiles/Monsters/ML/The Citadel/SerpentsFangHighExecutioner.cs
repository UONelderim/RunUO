using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "zwłoki egzekutora vox populi" )] 
	public class SerpentsFangHighExecutioner : SerpentsFangAssassin
	{
public override double DifficultyScalar{ get{ return 1.02; } }
		public override bool AlwaysMurderer{ get{ return true; } }
		public override bool ShowFameTitle{ get{ return false; } }
		[Constructable]
		public SerpentsFangHighExecutioner() : base()
		{
			Name = "egzekutor";
			Title = "- Przynależy do Vox Populi";
		}

		public SerpentsFangHighExecutioner( Serial serial ) : base( serial )
		{
		}
		
		public override void GenerateLoot()
		{
			AddLoot( LootPack.AosFilthyRich, 6 );
		}
		
		public override void AlterMeleeDamageFrom( Mobile from, ref int damage )
		{
			if ( from != null )
				from.Damage( damage / 2, from );
		}
		
		public override void OnDeath( Container c )
		{
			base.OnDeath( c );	
			
			/*if ( Utility.RandomDouble() < 0.2 )
				c.DropItem( new SerpentFangKey() );*/
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
