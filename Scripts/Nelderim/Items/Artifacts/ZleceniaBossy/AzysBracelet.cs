using System;
using Server.Engines.Craft;

namespace Server.Items
{
    public class AzysBracelet : BaseBracelet
    {
        private bool isSecondClick = false; // Used to track the second double-click

        public override int InitMinHits { get { return 50; } }
        public override int InitMaxHits { get { return 50; } }

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
            this.SkillBonuses.Skill_2_Value = 5;
            Label1 = "*branzoleta ma miejsce na 3 palce - czy odwazysz sie jej dotknac?*";
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

        public override void OnDoubleClick(Mobile from)
        {
            if (!isSecondClick)
            {
                // On the first double-click, modify attributes and weight
                this.Attributes.BonusDex = 10;
                this.Attributes.BonusInt = -30;
                this.Weight = 20.0;

                isSecondClick = true;
            }
            else
            {
                // On the second double-click, revert attributes and weight to original values
                this.Attributes.BonusDex = 0; // Reset BonusDex
                this.Attributes.BonusInt = 15; // Reset BonusInt to the original value
                this.Weight = 0.1; // Reset weight to the original value

                isSecondClick = false;
            }

            base.OnDoubleClick(from);
        }
    }
}
