using System;

namespace Server.Items
{
	public abstract class Hair : Item
	{
		protected Hair( int itemID )
			: this( itemID, 0 )
		{
		}

		protected Hair( int itemID, int hue )
			: base( itemID )
		{
			LootType = LootType.Blessed;
			Layer = Layer.Hair;
			Hue = hue;
		}

		public Hair( Serial serial )
			: base( serial )
		{
		}

		public override bool DisplayLootType { get { return false; } }

		public override bool VerifyMove( Mobile from )
		{
			return (from.AccessLevel >= AccessLevel.GameMaster);
		}

		public override DeathMoveResult OnParentDeath( Mobile parent )
		{
//			Dupe( Amount );

			parent.HairItemID = this.ItemID;
			parent.HairHue = this.Hue;

			return DeathMoveResult.MoveToCorpse;
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int)0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			LootType = LootType.Blessed;

			int version = reader.ReadInt();
		}
	}
}