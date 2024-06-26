using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.ContextMenus;
using Server.Gumps;
using Server.Items;
using Server.Network;
using Server.Targeting;
using System.Text.RegularExpressions;

namespace Server.Mobiles
{
    public class DrowAnimalTrainer : AnimalTrainer
    {
        private ArrayList m_SBInfos = new ArrayList();
        protected override ArrayList SBInfos { get { return m_SBInfos; } }
        [Constructable]
        public DrowAnimalTrainer() : base()
        {
        }

        public override void InitSBInfo()
        {
            m_SBInfos.Add(new SBAnimalTrainer(true));
        }

        public DrowAnimalTrainer(Serial serial) : base(serial)
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