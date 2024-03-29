using System;
using Server.Engines.Craft;

namespace Server.Items
{

    public class AzysBracelet : BaseBracelet
    {
        
        public override int InitMinHits{ get{ return 50; } }
        public override int InitMaxHits{ get{ return 50; } }
        
        [Constructable]
        public AzysBracelet()
            : base(0x1F06)
        {
            this.Weight = 0.1;
            this.Name = "Branzoleta Zaklinania Maga Azy";
            this.Hue = 0x78A;
            this.Attributes.Luck = -100;
            this.Attributes.BonusInt = 15;
            this.SkillBonuses.Skill_2_Name = SkillName.Alchemy;
            this.SkillBonuses.Skill_2_Value = 20;

        }

        public AzysBracelet(Serial serial)
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