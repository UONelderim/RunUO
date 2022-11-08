namespace Server.Items
{
    public class BaseQuestMarker : Item
    {
        public BaseQuestMarker( int itemId ) : base( itemId )
        {
            Movable = false;
        }

        public BaseQuestMarker( Serial serial ) : base( serial )
        {
        }

        public override void AddNameProperties(ObjectPropertyList list)
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
    
    public class QuestMarker1 : BaseQuestMarker
    {
        [Constructable]
        public QuestMarker1( ) : base( 0x3FE5 )
        {
        }

        public QuestMarker1( Serial serial ) : base( serial )
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

    public class QuestMarker2 : BaseQuestMarker
    {
        [Constructable]
        public QuestMarker2( ) : base( 0x3FE8 )
        {
        }

        public QuestMarker2( Serial serial ) : base( serial )
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