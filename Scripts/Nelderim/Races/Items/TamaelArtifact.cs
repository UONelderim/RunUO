using Server.Gumps;

namespace Server.Items
{
	public class TamaelArtifact : RaceChoiceItem
	{
	    public TamaelArtifact( Serial serial ) : base( serial )
        {
        }

        [Constructable]
        public TamaelArtifact() : base( 0x26CE )
        {
            Name = "Artefakt Tamaelow";
			Hue = 11;
            m_Race = Tamael.Instance;
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
