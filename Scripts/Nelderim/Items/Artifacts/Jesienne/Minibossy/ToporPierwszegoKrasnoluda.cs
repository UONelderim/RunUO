using System;
using Server;

namespace Server.Items
{
    public class ToporPierwszegoKrasnoluda : WarAxe
    {
        
        public override int InitMinHits { get { return 60; } }
        public override int InitMaxHits { get { return 60; } }

        [Constructable]
        public ToporPierwszegoKrasnoluda()
        {
            Hue = 2671;
            Name = "Topor Pierwszego Krasnoluda";
            Attributes.WeaponDamage = 55;
            WeaponAttributes.HitLeechHits = 25;
            Attributes.WeaponSpeed = 30;
            Attributes.AttackChance = 10;
            SkillBonuses.SetValues(0, SkillName.Macing, 5.0);
            
            
        }

        public ToporPierwszegoKrasnoluda(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

        }
    }
}
