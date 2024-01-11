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
	public abstract class VegetableSeedling : BaseSeedling
    {
        public override bool CanGrowGarden => true;

        public override double SowMinSkill => 0.0;
        public override double SowChanceAtMinSkill => 0.0;
        public override double SowMaxSkill => 0.0;
        public override double SowChanceAtMaxSkill => 100.0;

        public VegetableSeedling( int amount, int itemID ) : base( amount, itemID )
		{
		}

		public VegetableSeedling( int itemID ) : this( 1, itemID )
		{
		}

		public VegetableSeedling( Serial serial ) : base( serial )
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

	public abstract class VegetablePlant : Plant
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

        public VegetablePlant( int itemID ) : base( itemID )
		{
        }

		public VegetablePlant( Serial serial ) : base( serial ) 
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
	
	public abstract class VegetableCrop : Crop
    {
		public override int DefaulReagentCount(Mobile m) => 2;

		public VegetableCrop( int amount, int itemID ) : base(amount, itemID)
		{
		}
		
		public VegetableCrop( Serial serial ) : base( serial )
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