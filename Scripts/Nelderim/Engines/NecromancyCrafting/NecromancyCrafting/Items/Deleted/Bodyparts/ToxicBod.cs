using Server.Helpers;

namespace Server.Items
{
	public class ToxicBod : Item
	{
		public override string DefaultName
		{
			get { return "toksyczne ciało"; }
		}

		[Constructable]
		public ToxicBod() : base( 0x1CDE )
		{
			Weight = 1.0;
			Stackable = true;
		}

		public ToxicBod( Serial serial ) : base( serial )
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
			
			this.ReplaceWith(new ToxicTorso{ Amount = Amount });
		}
	}
}