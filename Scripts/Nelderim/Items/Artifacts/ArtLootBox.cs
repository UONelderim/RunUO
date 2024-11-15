using System;
using System.Configuration;
using Server;
using Server.Items;

namespace Server
{
	public class ArtLootBox : Item
	{
		public enum ArtLootType
		{
			Random = 0,
			Boss = 1,
			Miniboss = 2,
			Paragon = 3,
			Doom = 4,
			Hunter = 5,
			Cartography = 6,
			Fishing = 7
		}

		private ArtLootType m_LootType;
		
		[CommandProperty( AccessLevel.GameMaster )]
		public ArtLootType LootType
		{
			get { return m_LootType; }
			set { m_LootType = value; SetName(); }
		}
		
		public override double DefaultWeight
		{
			get { return 0.01; }
		}
		
		[Constructable]
		public ArtLootBox() : this( ArtLootType.Random )
		{
		}
		
		[Constructable]
		public ArtLootBox(String type) : base( 0x2DF3 )
		{
			if (!ArtLootType.TryParse(type, out m_LootType))
				m_LootType = ArtLootType.Paragon;
			SetName();
		}
		
		public ArtLootBox(ArtLootType type) : base( 0x2DF3 )
		{
			m_LootType = type;
			SetName();
		}
		
		public ArtLootBox( Serial serial ) : base( serial )
		{
		}

		public override void OnDoubleClick(Mobile from)
		{
			if(Parent != from.Backpack)
				from.SendMessage("Nie mozesz tego uzyc");
			else
			{
				var artifact = m_LootType switch
				{
					ArtLootType.Random => ArtifactHelper.CreateRandomArtifact(),
					ArtLootType.Boss => ArtifactHelper.CreateRandomBossArtifact(),
					ArtLootType.Miniboss => ArtifactHelper.CreateRandomMinibossArtifact(),
					ArtLootType.Paragon => ArtifactHelper.CreateRandomParagonArtifact(),
					ArtLootType.Doom => ArtifactHelper.CreateRandomDoomArtifact(),
					ArtLootType.Hunter => ArtifactHelper.CreateRandomHunterArtifact(),
					ArtLootType.Cartography => ArtifactHelper.CreateRandomCartographyArtifact(),
					ArtLootType.Fishing => ArtifactHelper.CreateRandomFishingArtifact(),
				};
				if (artifact != null)
				{
					ArtifactHelper.GiveArtifact(from, artifact);
					Delete();

				}
			}
			base.OnDoubleClick(from);
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 ); // version
			writer.Write((int)m_LootType);
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
			if (version >= 1)
			{
				m_LootType = (ArtLootType) reader.ReadInt();
			}
		}

		private void SetName()
		{
			Name = m_LootType switch
			{
				ArtLootType.Random => "Skrzynia Artefaktu",
				ArtLootType.Boss => "Skrzynia Artefaktu Władcy Podziemi",
				ArtLootType.Miniboss => "Skrzynia Artefaktu Pomniejszego Władcy Podziemi",
				ArtLootType.Paragon => "Skrzynia Artefaktu Paragonów",
				ArtLootType.Doom => "Skrzynia Artefaktu Pana Mroku",
				ArtLootType.Hunter => "Skrzynia Artefaktu Myśliwego",
				ArtLootType.Cartography => "Skrzynia Artefaktu Poszukiwaczy Skarbów",
				ArtLootType.Fishing => "Skrzynia Artefaktu Leviathana",
				_ => Name
			};
		}
	}
}