using System;

namespace Server.Items
{
    public class SwordOfShatteredHopes : Broadsword
    {
        [Constructable]
        public SwordOfShatteredHopes()
            : base()
        {
            this.Name = ("Miecz Strzaskanych Nadziei");
		
            this.Hue = 91;	
			
            this.WeaponAttributes.HitDispel = 25;
            this.WeaponAttributes.HitLeechMana = 35;
            this.Attributes.WeaponSpeed = 30;	
            this.Attributes.WeaponDamage = 50;			
            this.WeaponAttributes.ResistFireBonus = 15;
            WeaponAttributes.UseBestSkill = 1;
        }

        public SwordOfShatteredHopes(Serial serial)
            : base(serial)
        {
        }

        public override int InitMinHits
        {
            get
            {
                return 255;
            }
        }
        public override int InitMaxHits
        {
            get
            {
                return 255;
            }
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