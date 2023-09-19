using System;
using Server.Items;
using Server.Mobiles;

namespace Server.Items
{
    public class SoulRipper : Katana
    {
        [Constructable]
        public SoulRipper()
        {
            Hue = 1152;
            Name = "Wysysacz Dusz";
            Weight = 5.0;
            WeaponAttributes.HitLeechStam = 20;
            WeaponAttributes.HitLeechMana = 20;
            Attributes.WeaponSpeed = 10;
            Attributes.WeaponDamage = 60;
            AosElementDamages.Energy = 100;
        }
        
        public override void AddNameProperties(ObjectPropertyList list)
        {
            base.AddNameProperties(list);
            list.Add(1049644, "Z kazdym Twym uderzeniem, Twa dusza slabnie");
        }

        public SoulRipper(Serial serial) : base(serial)
        {
        }

        public override int InitMinHits { get { return 50; } }
        public override int InitMaxHits { get { return 50; } }


        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }

        public override void OnHit(Mobile attacker, Mobile defender, double damageBonus)
        {
            base.OnHit(attacker, defender, damageBonus);

            
            if (attacker is PlayerMobile)
            {
                PlayerMobile player = (PlayerMobile)attacker;
                player.Damage(8);
                attacker.FixedParticles(0x3709, 10, 30, 5052, 0x480, 0, EffectLayer.LeftFoot);
                attacker.PlaySound(0x208);
                player.SendMessage("Twoja dusza cierpi!"); 
            }
        }

    }
}
