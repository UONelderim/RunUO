using System;
using System.Collections;
using Server;
using Server.Mobiles;
using Server.Items;
using Server.Network;
using Server.Targeting;
using Server.Gumps;
using System.Collections.Generic;

namespace Server.Items
{
    public class QuestDecorItem : Item
    {
        [CommandProperty(AccessLevel.Counselor)]
        public string qName
        {
            get { return Name; }
            set { Name = value; }
        }

        [CommandProperty(AccessLevel.Counselor)]
        public string qLabel1
        {
            get { return Label1; }
            set { Label1 = value; }
        }

        [CommandProperty(AccessLevel.Counselor)]
        public string qLabel2
        {
            get { return Label2; }
            set { Label2 = value; }
        }

        [CommandProperty(AccessLevel.Counselor)]
        public int qItemID
        {
            get { return ItemID; }
            set { ItemID = value; }
        }

        [CommandProperty(AccessLevel.Counselor)]
        public int qHue
        {
            get { return Hue; }
            set { Hue = value; }
        }

        [CommandProperty(AccessLevel.Counselor)]
        public double qWeight
        {
            get { return Weight; }
            set { Weight = value; }
        }

        [CommandProperty(AccessLevel.Counselor)]
        public LootType qLootType
        {
            get { return LootType; }
            set { LootType = value; }
        }

        [CommandProperty(AccessLevel.Counselor)]
        public bool qMovable
        {
            get { return Movable; }
            set { Movable = value; }
        }

        [Constructable(AccessLevel.Counselor)]
        public QuestDecorItem() : this(0x14F0)
        {
        }

        [Constructable(AccessLevel.Counselor)]
        public QuestDecorItem(int itmeID) : base(itmeID)
        {
            Name = "Przedmiot potrzebny do zadania";
            Stackable = false;
            Weight = 1;
        }

        public QuestDecorItem(Serial serial) : base(serial)
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