using System;
using System.Collections;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	public class Olaeni : BaseVendor
	{
		private ArrayList m_SBInfos = new ArrayList(); 
		protected override ArrayList SBInfos{ get { return m_SBInfos; } } 
		
		public override bool CanTeach{ get{ return false; } }
		public override bool IsInvulnerable{ get{ return true; } }
		
		public override void InitSBInfo()
		{		
		}
		
		[Constructable]
		public Olaeni() : base( "the thaumaturgist" )
		{			
			Name = "Olaeni";
		}
		
		public Olaeni( Serial serial ) : base( serial )
		{
		}
		
		public override void InitBody()
		{
			InitStats( 100, 100, 25 );
			
			Female = true;
			//Race = Race.Elhedini;
			
			Hue = 0x851D;
			HairItemID = 0x2FCF;
			HairHue = 0x322;			
		}
		
		public override void InitOutfit()
		{
			AddItem( new Shoes( 0x736 ) );
			AddItem( new FemaleElvenRobe( 0x1C ) );
			AddItem( new GemmedCirclet() );
			AddItem( new MagicWand() );
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