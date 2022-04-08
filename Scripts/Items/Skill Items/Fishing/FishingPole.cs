using System;
using System.Collections;
using Server.Targeting;
using Server.Items;
using Server.Engines.Harvest;
using System.Collections.Generic;
using Server.ContextMenus;

namespace Server.Items
{
    // 07.01.2012 :: zombie :: Item -> BaseHarvestTool
    public class FishingPole : BaseHarvestTool
    {
        // 07.01.2012 :: zombie :: dodanie uses do wedki
        public override HarvestSystem HarvestSystem{ get{ return Fishing.System; } }

        [Constructable]
        public FishingPole() : this ( BaseHarvestTool.UsesDefault() )
        {
        }

        public FishingPole( int uses ) : base( uses, 0x0DC0 )
        {
            Layer = Layer.OneHanded;
            Weight = 1.0;
        }
        // zombie

        public FishingPole( Serial serial ) : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int) 0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
    }
}