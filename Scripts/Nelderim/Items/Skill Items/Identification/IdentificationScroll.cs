using System;
using Server;
using Server.Items;
using Server.Targeting;

namespace Server.Items
{
	public class IdentificationScroll : Item
	{
        private int m_scrollLevel = new int();
        public int ScrollLevel
        {
            get { return m_scrollLevel; }
            set { m_scrollLevel = value; }
        }

        public IdentificationScroll(int lvl) : base(0x1F72)
		{
            m_scrollLevel = lvl;
            switch (m_scrollLevel)
            {
                case 1:
                    base.Hue = 0x381;
                    break;
                case 2:
                    base.Hue = 0x371;
                    break;
                case 3:
                    base.Hue = 0x351;
                    break;
                case 4:
                    base.Hue = 0x331;
                    break;
                default:
                    base.Hue = 0x381;
                    break;
            }
            base.Weight = 1.0;
            base.Stackable = false;
		}

        public IdentificationScroll(Serial serial)
            : base(serial)
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version

            writer.Write((int)m_scrollLevel);
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

            m_scrollLevel = reader.ReadInt();
		}

        public override void OnDoubleClick(Mobile m)
        {
            m.BeginTarget(4, false, TargetFlags.None, new TargetCallback(Identify_OnTarget));
        }

        private bool ScrollLevelEnoughForIdentifyLevel(IdLevel idlvl)
        { 
            bool canIdentify = false;
            switch (idlvl)
            {
                case IdLevel.None:
                    canIdentify = false;
                    break;
                case IdLevel.Level1:
                    if (m_scrollLevel > 0)
                        canIdentify = true;
                    break;
                case IdLevel.Level2:
                    if (m_scrollLevel > 1)
                        canIdentify = true;
                    break;
                case IdLevel.Level3:
                    if (m_scrollLevel > 2)
                        canIdentify = true;
                    break;
                case IdLevel.Level4:
                    if (m_scrollLevel > 3)
                        canIdentify = true;
                    break;
                case IdLevel.Level5:
                    canIdentify = false;
                    break;
                case IdLevel.Level6:
                    canIdentify = false;
                    break;
                default:
                    break;
            }
            return canIdentify;
        }

        private void Identify_OnTarget(Mobile from, object obj)
        {
            if (obj is Item)
            {
                if (!((Item)obj).IsChildOf(from.Backpack))
                {
                    from.SendLocalizedMessage(1042001); // Musisz miec przedmiot w plecaku, zeby go uzyc.
                }
                else
                {
                    if (obj is IIdentifiable)
                    {
                        IIdentifiable ido = (IIdentifiable)obj;

                        if (ido.Identified == true)
                        {
                            from.SendLocalizedMessage(1064630);   // Wiesz juz wszystko o tym przedmiocie
                        }
                        else
                        {
                            if (ScrollLevelEnoughForIdentifyLevel(ido.IdentifyLevel))
                            {
                                ido.Identified = true;
                                from.SendLocalizedMessage(1064631);   // Rozpoznales magiczne cechy przedmiotu
                                Delete();
                            }
                            else
                            {
                                from.SendLocalizedMessage(1064633);   // Nie masz najmniejszych szans na identyfikacje tego przedmiotu.
                            }
                        }
                    }
                }
            }
            else
            {
                from.SendLocalizedMessage(500353); // Nie jestes pewien...
            }
        }
	}

    public class IdentificationScrollLvl1 : IdentificationScroll
    {
        public override int LabelNumber { get { return 1065950; } } // Zwoj identyfikacji poziomu 1
        [Constructable]
        public IdentificationScrollLvl1() : base(1)
        {
        }

        public IdentificationScrollLvl1(Serial serial) : base(serial)
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

    public class IdentificationScrollLvl2 : IdentificationScroll
    {
        public override int LabelNumber { get { return 1065951; } } // Zwoj identyfikacji poziomu 2
        [Constructable]
        public IdentificationScrollLvl2()
            : base(2)
        {
        }

        public IdentificationScrollLvl2(Serial serial)
            : base(serial)
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

    public class IdentificationScrollLvl3 : IdentificationScroll
    {
        public override int LabelNumber { get { return 1065952; } } // Zwoj identyfikacji poziomu 3
        [Constructable]
        public IdentificationScrollLvl3()
            : base(3)
        {
        }

        public IdentificationScrollLvl3(Serial serial)
            : base(serial)
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

    public class IdentificationScrollLvl4 : IdentificationScroll
    {
        public override int LabelNumber { get { return 1065953; } } // Zwoj identyfikacji poziomu 4
        [Constructable]
        public IdentificationScrollLvl4()
            : base(4)
        {
        }

        public IdentificationScrollLvl4(Serial serial)
            : base(serial)
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