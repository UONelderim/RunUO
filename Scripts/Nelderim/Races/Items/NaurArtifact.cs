using Server.Gumps;

namespace Server.Items
{
	public class NaurArtifact : RaceChoiceItem
	{
	    public NaurArtifact( Serial serial ) : base( serial )
        {
        }

        [Constructable]
        public NaurArtifact() : base( 0xF62 )
        {
            Name = "Artefakt Naurow";
			Hue = 25;
            m_Race = Naur.Instance;
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
