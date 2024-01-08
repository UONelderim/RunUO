using System;
using Server.Network;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Targets;
using Server.Items;
using Server.Targeting;
using Server.Spells;
using Server.Mobiles;

namespace Server.Items.Crops
{
	// Tutaj zmieniaj wlasnosci dotyczace wszystkich ziol uprawnych.


	// Klasa ogolnie reprezentujaca szczepke dowolnego ziela.
	public abstract class SeedWarzywo : WeedSeedZiolaUprawne
    {
        public override bool CanGrowGarden => true;

        public override double SowMinSkill => 0.0;
        public override double SowChanceAtMinSkill => 0.0;
        public override double SowMaxSkill => 0.0;
        public override double SowChanceAtMaxSkill => 100.0;

        public SeedWarzywo( int amount, int itemID ) : base( amount, itemID )
		{
		}

		public SeedWarzywo( int itemID ) : this( 1, itemID )
		{
		}

		public SeedWarzywo( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
		}
	}

	// Klasa ogolnie reprezentujaca krzaczek dowolnego warzywa.
	public abstract class PlantWarzywo : WeedPlantZiolaUprawne
    {
        public override int DefaultSeedCount(Mobile from) => 1;
        public override int DefaultCropCount(Mobile from) => 6;

        public override double SeedAcquireMinSkill => 0.0;
        public override double SeedAcquireChanceAtMinSkill => 0.0;
        public override double SeedAcquireMaxSkill => 0.0;
        public override double SeedAcquireChanceAtMaxSkill => 80.0;

        public override double HarvestMinSkill => 0.0;
        public override double HarvestChanceAtMinSkill => 0.0;
        public override double HarvestMaxSkill => 0.0;
        public override double HarvestChanceAtMaxSkill => 100.0;

        public override double DestroyAtSkill => 0.0;

        public PlantWarzywo( int itemID ) : base( itemID )
		{
            GrowingTimeInSeconds = 3600;	// Ma zastosowanie tylko dla roslin sadzonych ze szczepki. Rosliny tworzone spawnerem maja prehistoryczny m_PlantedTime
        }

		public PlantWarzywo( Serial serial ) : base( serial ) 
		{ 
		}

		public override void Serialize( GenericWriter writer ) 
		{ 
			base.Serialize( writer );
		} 

		public override void Deserialize( GenericReader reader ) 
		{ 
			base.Deserialize( reader );
		} 
	} 
	
	// Klasa ogolnie reprezentujaca plon z dowolnego ziela.
	public abstract class CropWarzywo : WeedCropZiolaUprawne
    {
		public override int DefaulReagentCount(Mobile m) => 2;

		//public override string MsgCreatedZeroReagent		{ get{ return "Nie uzyskales wystarczajacej ilosci reagentu."; } }
		//public override string MsgFailedToCreateReagents	{ get{ return "Nie udalo ci sie uzyskac reagentow."; } }
		//public override string MsgCreatedReagent			{ get{ return "Uzyskales nieco reagentu."; } }
		//public override string MsgStartedToCut				{ get{ return "Zaczynasz obrabiac surowe ziolo..."; } }

		public CropWarzywo( int amount, int itemID ) : base(amount, itemID)
		{
		}
		
		public CropWarzywo( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
		}
	}
}