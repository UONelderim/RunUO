using Server.Gumps;

namespace Server.Items
{
	public class KrasnoludArtifact : RaceChoiceItem
	{
	    public KrasnoludArtifact( Serial serial ) : base( serial )
        {
        }

        [Constructable]
        public KrasnoludArtifact() : base( 0x2d24 )
        {
            Name = "Artefakt Krasnoludow";
			Hue = 2001;
            m_Race = Krasnolud.Instance;
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
            writer.Write( (int)0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
    }
}
