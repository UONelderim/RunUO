using System;
using Server.Items;
using Server.Targeting;
using System.Collections.Generic;
using Server.ContextMenus;
using Server.Gumps;
using System.Collections;

// szczaw :: 2013.01.13 :: identyfikowanie przedmiotow
// szczaw :: 2013.01.14 :: przeniesienie tekstow do cliloca
namespace Server.Mobiles
{
    class IdentyfikatorVendore : BaseVendor
    {
        const int cliloc = 1070000;

        // o zgrozo...
        private ArrayList _sbInfos = new ArrayList();
        protected override ArrayList SBInfos { get { return _sbInfos; } }

        public override bool IsActiveVendor { get { return false; } }

        public override void InitSBInfo()
        {
        }

        [Constructable]
        public IdentyfikatorVendore() : base( "- identyfikator" )
		{
			SetSkill( SkillName.ItemID, 75.0, 100.0 );
			SetSkill( SkillName.DetectHidden, 75.0, 100.0 );
			SetSkill( SkillName.Tracking, 75.0, 100.0 );            
        }

        public IdentyfikatorVendore( Serial serial )
            : base(serial)
        {
        }

        static IdentyfikatorVendore()
        {
            Price = 500;
            MaxLevel = 5;
        }
      
        #region Identyfikowanie

        [CommandProperty(AccessLevel.GameMaster)]
        public static int Price { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public static int MaxLevel { get; set; }

        private enum CanBeIdentifiedResult
        {
            Item,
            Container,
            NotInBackpack,
            IncorrectItem,
            IncorrectMobile,
            AlreadyIdentified,
            TooDifficult,
        }

        private void BeginIdentifyItems(Mobile from)
        {
            if(from == null || !(from is PlayerMobile))
                return;

            from.SendLocalizedMessage(cliloc + 1); // "Wskaż przedmiot lub pojemnik z przedmiotami który chcesz zidentyfikować."
            from.Target = new GeneralTarget(2, false, TargetFlags.None, this.ContinueIdentifyItems );
        }

        private void ContinueIdentifyItems( Mobile from, object targeted )
        {
            var result = CheckItem(from, targeted);
            IList<IIdentifiable> toIdentify = null;

            switch(result)
            {
                case CanBeIdentifiedResult.Item:
                    toIdentify = new List<IIdentifiable> { (IIdentifiable) targeted };
                    break;
                case CanBeIdentifiedResult.Container:
                    toIdentify = GetItemsToIdentify(from, (Container) targeted); 
                    break;
                case CanBeIdentifiedResult.NotInBackpack:
                    from.SendLocalizedMessage(cliloc + 2); //"Przedmiot musi znajdowac sie w Twoim plecaku."
                    return;
                case CanBeIdentifiedResult.IncorrectItem:
                    from.SendLocalizedMessage(cliloc + 3); //"Nie moge tego zidentyifkowac."
                    return;
                case CanBeIdentifiedResult.AlreadyIdentified:
                    from.SendLocalizedMessage(cliloc + 4); // "Ten przedmiot juz jest zidentyfikowany."
                    return;
                case CanBeIdentifiedResult.TooDifficult:
                    from.SendLocalizedMessage(cliloc + 5); // "Ten przedmiot jest zbyt unikalny zebym dal rade odczytac jego wlasnosci."
                    return;
                case CanBeIdentifiedResult.IncorrectMobile:
                    // from == null, nawet nie ma komu o tym powiedziec :(
                    return;
            }

            PlayerMobile player = (PlayerMobile) from;

            int totalCost;
            bool canAfford;

            if(result == CanBeIdentifiedResult.Item)
            {
                totalCost = Price * (int)((IIdentifiable)toIdentify[0]).IdentifyLevel;
                canAfford = player.CanAfford(totalCost);

                if(canAfford)
                {
                    player.TryTakeGold(totalCost);
                    toIdentify[0].Identified = true;
                    from.SendLocalizedMessage(cliloc + 6); // "Zidentyfikowalem ten przedmiot dla Ciebie."
                }
                else
                    from.SendLocalizedMessage(cliloc + 7, totalCost.ToString()); // Nie stac Ciebie na identyfikacje przedmiotu! Potrzebujesz ~1_COST zlota.
            }
            else if(toIdentify.Count > 0)
            {
                totalCost = 0;
                foreach (var item in toIdentify)
                    totalCost += Price * (int)((IIdentifiable)item).IdentifyLevel;

                Action msgYouCantAfford = () => from.SendLocalizedMessage(cliloc + 8, String.Format("{0}\t{1}", toIdentify.Count, totalCost));
                canAfford = player.CanAfford(totalCost);

                if(canAfford)
                {
                    var gump = new GeneralConfirmGump(
                        ( n, ri ) =>
                        {
                            bool paid = player.TryTakeGold(totalCost);

                            if(!paid)
                            {
                                msgYouCantAfford();
                                return;
                            }

                            foreach(var item in toIdentify)
                                item.Identified = true;

                            from.SendLocalizedMessage(cliloc + 9, toIdentify.Count.ToString()); // "Zidentyfikowalem {0} przedmiotow."
                        });

                    gump.Text = String.Format("Potrafie zidentyfikowac {0} przedmiotow z tego pojemnika, zidentyfikuje je za {1} zlota.", toIdentify.Count, totalCost);

                    from.SendGump(gump);
                }
                else
                    msgYouCantAfford();
            }
            else
                from.SendLocalizedMessage(cliloc+10); // "W tym pojemniku nie ma nic godnego uwagi, co moglbym zidentyfikowac."

        }

        private IList<IIdentifiable> GetItemsToIdentify( Mobile owner, Container container )
        {
            IList<IIdentifiable> itemsToIdentify = new List<IIdentifiable>();

            CanBeIdentifiedResult result;
            foreach(var item in container.Items)
            {
                result = CheckItem(owner, item);

                if(result == CanBeIdentifiedResult.Item)
                    itemsToIdentify.Add((IIdentifiable)item);
                else if (result == CanBeIdentifiedResult.Container )         // Rekurencja
                {
                    var fromContainer = GetItemsToIdentify(owner, (Container) item);

                    foreach(var i in fromContainer)
                        itemsToIdentify.Add(i);
                }
            }

            return itemsToIdentify;
        }


        private CanBeIdentifiedResult CheckItem( Mobile m, object o )
        {
            PlayerMobile from = m as PlayerMobile;
            Item item = o as Item;

            if(from == null)
                return CanBeIdentifiedResult.IncorrectMobile;

            else if(item == null)
                return CanBeIdentifiedResult.IncorrectItem;

            else if(item.Serial != m.Backpack.Serial && !item.IsChildOf(from.Backpack))
                return CanBeIdentifiedResult.NotInBackpack;

            else if(item is IIdentifiable)
            {
                var t = ((IIdentifiable) item);

                if(t.Identified)
                    return CanBeIdentifiedResult.AlreadyIdentified;
                else if((int) t.IdentifyLevel > MaxLevel)
                    return CanBeIdentifiedResult.TooDifficult;
                else
                    return CanBeIdentifiedResult.Item;
            }

            else if(item is Container)
                return CanBeIdentifiedResult.Container;

            else
                return CanBeIdentifiedResult.IncorrectItem;
            
        }

        public override void AddCustomContextEntries( Mobile from, List<ContextMenus.ContextMenuEntry> list )
        {
            base.AddCustomContextEntries(from, list);

            if(from.Alive)
            {
                list.Add(new GeneralContextMenuEntry(6061, () => this.BeginIdentifyItems(from)));
            }
        }
        #endregion

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize(writer);

            int version = 1;
            writer.Write(version);
            writer.Write(Price);
            writer.Write(MaxLevel);
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            Price = reader.ReadInt();
            MaxLevel = reader.ReadInt();
            
        }
    }
}
