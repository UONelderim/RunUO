using System;
using Server;
using Server.Items;
using System.Collections.Generic;
using Server.Spells.First;
using Server.Spells.Eighth;
using Server.Spells;
using Server.Network;
using Server.Mobiles;

namespace Server.Items
{
    public class KopmendiumWiedzyDrowow : Spellbook
    {
        private static List<SlayerName> SlayerTypes = new List<SlayerName>
        {
            SlayerName.Silver,
            SlayerName.Repond,
            SlayerName.ReptilianDeath,
            SlayerName.Exorcism,
            SlayerName.ArachnidDoom,
            SlayerName.ElementalBan,
            SlayerName.Fey
        };

        private bool IsEquipped;

        [Constructable]
        public KopmendiumWiedzyDrowow() : base()
        {
            Hue = 2571;
            Name = "Kopmendium Wiedzy Drowow";

            Slayer = SlayerTypes[Utility.Random(SlayerTypes.Count)];

            Attributes.SpellDamage = 5;
            Attributes.CastSpeed = -1;
            Attributes.CastRecovery = 3;
			Attributes.RegenHits = 3;
        }

        public KopmendiumWiedzyDrowow(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
            writer.Write((bool)IsEquipped); // Serialize whether the item is equipped
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            IsEquipped = reader.ReadBool(); // Deserialize whether the item is equipped
        }

        public override bool OnEquip(Mobile from)
        {
            bool baseResult = base.OnEquip(from);

            if (from is PlayerMobile)
            {
                PlayerMobile player = (PlayerMobile)from;
                IsEquipped = true;

                player.SendMessage("Starozytna magia Drowow wysysa Twoja Mane.");

                DrainManaTimer timer = new DrainManaTimer(player, this);
                timer.Start();
            }

            return baseResult;
        }

        public override void OnRemoved(object parent)
        {
            base.OnRemoved(parent);

            if (parent is Mobile)
            {
                PlayerMobile player = (PlayerMobile)parent;
                IsEquipped = false;

                player.SendMessage("Starozytna Magia Drowow przestala dzialac.");
            }
        }

        private class DrainManaTimer : Timer
        {
            private PlayerMobile Player;
            private KopmendiumWiedzyDrowow Item;

            public DrainManaTimer(PlayerMobile player, KopmendiumWiedzyDrowow item) : base(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1))
            {
                Player = player;
                Item = item;
                Priority = TimerPriority.FiftyMS;
            }

            protected override void OnTick()
            {
                if (Player == null || Player.Deleted || !Player.Alive)
                {
                    Stop();
                    return;
                }

                if (Player.Backpack != null && Player.Backpack.FindItemByType(Item.GetType()) == null)
                {
                    Stop();
                    return;
                }

                if (Player.Mana > 0)
                {
                    Player.Mana--;
                }
                else
                {
                    Stop();
                }
            }
        }
    }
}
