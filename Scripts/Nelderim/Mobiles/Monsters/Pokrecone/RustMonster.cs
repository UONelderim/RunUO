using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("resztki zardzewialego konstruktu")]
    public class RustMonster : BaseCreature
    {
        [Constructable]
        public RustMonster() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "Zardzewialy Konstrukt";
            Body = 305;
            BaseSoundID = 752;

            SetStr(200, 300);
            SetDex(100, 160);
            SetInt(50, 80);

            SetHits(230, 290);

            SetDamage(3, 6);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 55, 65);
            SetResistance(ResistanceType.Fire, 45, 55);
            SetResistance(ResistanceType.Cold, 45, 55);
            SetResistance(ResistanceType.Poison, 45, 55);
            SetResistance(ResistanceType.Energy, 45, 55);

            SetSkill(SkillName.MagicResist, 50.3, 80.0);
            SetSkill(SkillName.Tactics, 120.1, 160.0);
            SetSkill(SkillName.Wrestling, 125.1, 160.0);

            Fame = 2500;
            Karma = 0;

            VirtualArmor = 50;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Gems, (Utility.RandomMinMax(2, 3)));
        }

        public override int Meat
        {
            get { return 1; }
        }

        public override int Hides
        {
            get { return 8; }
        }

        public RustMonster(Serial serial) : base(serial)
        {
        }

        public void DoSpecialAbility(Mobile target)
        {
            if (target is PlayerMobile)
            {
                int resnum = 0;
                PlayerMobile owner = target as PlayerMobile;
                BaseArmor helm1 = new PlateHelm();
                BaseArmor gorget1 = new PlateGorget();
                BaseArmor tunic1 = new PlateChest();
                BaseArmor arms1 = new PlateArms();
                BaseArmor gloves1 = new PlateGloves();
                BaseArmor legs1 = new PlateLegs();
                BaseShield shield1 = new HeaterShield();
                BaseWeapon wep1 = new Kryss();
                Item helm = owner.FindItemOnLayer(Layer.Helm);
                if (helm != null) helm1 = helm as BaseArmor;
                Item gorget = owner.FindItemOnLayer(Layer.Neck);
                if (gorget != null) gorget1 = gorget as BaseArmor;
                Item tunic = owner.FindItemOnLayer(Layer.InnerTorso);
                if (tunic != null) tunic1 = tunic as BaseArmor;
                Item arms = owner.FindItemOnLayer(Layer.Arms);
                if (arms != null) arms1 = arms as BaseArmor;
                Item gloves = owner.FindItemOnLayer(Layer.Gloves);
                if (gloves != null) gloves1 = gloves as BaseArmor;
                Item legs = owner.FindItemOnLayer(Layer.Pants);
                if (legs != null) legs1 = legs as BaseArmor;
                Item shield = owner.FindItemOnLayer(Layer.TwoHanded);
                if (shield != null) shield1 = shield as BaseShield;
                Item wep = owner.FindItemOnLayer(Layer.FirstValid);
                if (wep != null) wep1 = wep as BaseWeapon;

                if (shield1 != null)
                {
                    resnum = (int)(shield1.Resource);
                    if (resnum >= 1 && resnum <= 99 && shield1.HitPoints >= 21)
                    {
                        shield1.HitPoints -= 20;
                        target.SendMessage("one of your items has been affected by rust");
                        return;
                    }
                }

                if (tunic1 != null)
                {
                    resnum = (int)(tunic1.Resource);
                    if (resnum >= 1 && resnum <= 99 && tunic1.HitPoints >= 21)
                    {
                        tunic1.HitPoints -= 20;
                        target.SendMessage("one of your items has been affected by rust");
                        return;
                    }
                }

                if (legs1 != null)
                {
                    resnum = (int)(legs1.Resource);
                    if (resnum >= 1 && resnum <= 99 && legs1.HitPoints >= 21)
                    {
                        legs1.HitPoints -= 20;
                        target.SendMessage("one of your items has been affected by rust");
                        return;
                    }
                }

                if (arms1 != null)
                {
                    resnum = (int)(arms1.Resource);
                    if (resnum >= 1 && resnum <= 99 && arms1.HitPoints >= 21)
                    {
                        arms1.HitPoints -= 20;
                        target.SendMessage("one of your items has been affected by rust");
                        return;
                    }
                }

                if (helm1 != null)
                {
                    resnum = (int)(helm1.Resource);
                    if (resnum >= 1 && resnum <= 99 && helm1.HitPoints >= 21)
                    {
                        helm1.HitPoints -= 20;
                        target.SendMessage("one of your items has been affected by rust");
                        return;
                    }
                }

                if (gloves1 != null)
                {
                    resnum = (int)(gloves1.Resource);
                    if (resnum >= 1 && resnum <= 99 && gloves1.HitPoints >= 21)
                    {
                        gloves1.HitPoints -= 20;
                        target.SendMessage("one of your items has been affected by rust");
                        return;
                    }
                }

                if (gorget1 != null)
                {
                    resnum = (int)(gorget1.Resource);
                    if (resnum >= 1 && resnum <= 99 && gorget1.HitPoints >= 21)
                    {
                        gorget1.HitPoints -= 20;
                        target.SendMessage("one of your items has been affected by rust");
                        return;
                    }
                }

                if (wep1 != null)
                {
                    resnum = (int)(wep1.Resource);
                    if (resnum >= 1 && resnum <= 99 && wep1.HitPoints >= 21)
                    {
                        wep1.HitPoints -= 20;
                        target.SendMessage("one of your items has been affected by rust");
                        return;
                    }
                }

                if (shield1 != null)
                {
                    resnum = (int)(shield1.Resource);
                    if (resnum >= 1 && resnum <= 99 && shield1.HitPoints >= 11)
                    {
                        shield1.HitPoints -= 10;
                        target.SendMessage("one of your items has been affected by rust");
                        return;
                    }
                }

                if (tunic1 != null)
                {
                    resnum = (int)(tunic1.Resource);
                    if (resnum >= 1 && resnum <= 99 && tunic1.HitPoints >= 11)
                    {
                        tunic1.HitPoints -= 10;
                        target.SendMessage("one of your items has been affected by rust");
                        return;
                    }
                }

                if (legs1 != null)
                {
                    resnum = (int)(legs1.Resource);
                    if (resnum >= 1 && resnum <= 99 && legs1.HitPoints >= 11)
                    {
                        legs1.HitPoints -= 10;
                        target.SendMessage("one of your items has been affected by rust");
                        return;
                    }
                }

                if (arms1 != null)
                {
                    resnum = (int)(arms1.Resource);
                    if (resnum >= 1 && resnum <= 99 && arms1.HitPoints >= 11)
                    {
                        arms1.HitPoints -= 10;
                        target.SendMessage("one of your items has been affected by rust");
                        return;
                    }
                }

                if (helm1 != null)
                {
                    resnum = (int)(helm1.Resource);
                    if (resnum >= 1 && resnum <= 99 && helm1.HitPoints >= 11)
                    {
                        helm1.HitPoints -= 10;
                        target.SendMessage("one of your items has been affected by rust");
                        return;
                    }
                }

                if (gloves1 != null)
                {
                    resnum = (int)(gloves1.Resource);
                    if (resnum >= 1 && resnum <= 99 && gloves1.HitPoints >= 11)
                    {
                        gloves1.HitPoints -= 10;
                        target.SendMessage("one of your items has been affected by rust");
                        return;
                    }
                }

                if (gorget1 != null)
                {
                    resnum = (int)(gorget1.Resource);
                    if (resnum >= 1 && resnum <= 99 && gorget1.HitPoints >= 11)
                    {
                        gorget1.HitPoints -= 10;
                        target.SendMessage("one of your items has been affected by rust");
                        return;
                    }
                }

                if (wep1 != null)
                {
                    resnum = (int)(wep1.Resource);
                    if (resnum >= 1 && resnum <= 99 && wep1.HitPoints >= 11)
                    {
                        wep1.HitPoints -= 10;
                        target.SendMessage("one of your items has been affected by rust");
                        return;
                    }
                }
            }

            return;
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            DoSpecialAbility(defender);
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