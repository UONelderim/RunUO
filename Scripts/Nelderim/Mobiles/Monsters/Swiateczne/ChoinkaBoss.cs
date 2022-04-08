using Server.Items;

namespace Server.Mobiles.Swiateczne
{
    public class ChoinkaBoss : DamageableItem
    {
        [Constructable]
        public ChoinkaBoss() : base(0x0CD6, 0x0CD6)
        {
            
        }
        
        public ChoinkaBoss( Serial serial )
            : base( serial )
        {
        }


        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( ( int )0 ); //version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt( );
        }
    }
}