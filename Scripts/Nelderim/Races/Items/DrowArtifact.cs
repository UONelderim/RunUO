using Server.Gumps;

namespace Server.Items
{
	public class DrowArtifact : RaceChoiceItem
	{
	    public DrowArtifact( Serial serial ) : base( serial )
        {
        }

        [Constructable]
        public DrowArtifact() : base( 0x1401 )
        {
            Name = "Artefakt Drowow";
			Hue = 1946;
            m_Race = Drow.Instance;
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
