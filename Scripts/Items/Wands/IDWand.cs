using System;
using Server;
using Server.Targeting;

namespace Server.Items
{
	public class IDWand : BaseWand
    {
		public override TimeSpan GetUseDelay{ get{ return TimeSpan.Zero; } }

		[Constructable]
		public IDWand() : base( WandEffect.Identification, 20, 30 )
		{
		}

        [Constructable]
        public IDWand(int uses) : base(WandEffect.Identification, uses)
        {
        }

		public IDWand( Serial serial ) : base( serial )
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

		public override bool OnWandTarget( Mobile from, object o )
		{
			if( o is Item )
			{
                if( !((Item)o).IsChildOf( from.Backpack ) )
                {
                    from.SendLocalizedMessage(1042001); // Musisz miec przedmiot w plecaku, zeby go uzyc.
				    return false;
			    }

                if ( o is IIdentifiable)
			    {
                    IIdentifiable ido = (IIdentifiable)o;
                    Mobile fromTmp = new Mobile();
                    fromTmp.Skills.ItemID.Base = 100;// Holds default ID wand identify skill

                    if (ido.Identified == true)
                    {
                        from.SendLocalizedMessage( 1064630 );   // Wiesz juz wszystko o tym przedmiocie
                    }
                    if (ItemIdentification.IdentifyCheck(fromTmp, ido))
                    {
                        ido.Identified = true;
                        from.SendLocalizedMessage( 1064631 );   // Rozpoznalez magiczne cechy przedmiotu
                    }
                    else
                    {
                        from.SendLocalizedMessage( 1064632 );   // Nie masz najmniejszych szans na identyfikacje tego przedmiotu.
                    }
                }
            }
            else if (o is Mobile)
            {
                ((Mobile)o).OnSingleClick(from);
            }
            else
            {
                from.SendLocalizedMessage(500353); // Nie jestes pewien...
                return false;
            }
			if ( !Core.AOS && o is Item )
				((Item)o).OnSingleClick( from );
			return ( o is Item );
		}
	}

    public class IDWand10uses : IDWand
    {
        [Constructable]
        public IDWand10uses(): base(10)
        {
        }

        public IDWand10uses(Serial serial) : base(serial)
		{
		}

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class IDWand20uses : IDWand
    {
        [Constructable]
        public IDWand20uses(): base(20)
        {
        }

        public IDWand20uses(Serial serial) : base(serial)
		{
		}

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class IDWand30uses : IDWand
    {
        [Constructable]
        public IDWand30uses(): base(30)
        {
        }

        public IDWand30uses(Serial serial) : base(serial)
		{
		}

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class IDWand50uses : IDWand
    {
        [Constructable]
        public IDWand50uses(): base(50)
        {
        }

        public IDWand50uses(Serial serial): base(serial)
		{
		}

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}