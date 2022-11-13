using System;

namespace Server.Items
{
	public abstract class Beard : Item
	{
		protected Beard( int itemID ) : this( itemID, 0 )
		{
		}

		protected Beard( int itemID, int hue ) : base( itemID )
		{
			LootType = LootType.Blessed;
			Layer = Layer.FacialHair;
			Hue = hue;
		}

		public Beard( Serial serial ) : base( serial )
		{
		}

		public override bool DisplayLootType{ get{ return false; } }

		public override bool VerifyMove( Mobile from )
		{
			return ( from.AccessLevel >= AccessLevel.GameMaster );
		}

		public override DeathMoveResult OnParentDeath( Mobile parent )
		{
			//Dupe( Amount );

			parent.FacialHairItemID = this.ItemID;
			parent.FacialHairHue = this.Hue;

			return DeathMoveResult.MoveToCorpse;
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			LootType = LootType.Blessed;

			int version = reader.ReadInt();
		}
	}
}