using System;
using System.Collections;
using Server.Engines.Quests;
using Server.Items;
using Server.Mobiles;
using Server.Network;

namespace Server.Engines.Quests.CraftingExperiments
{
    public class BlacksmithExperimentator : CraftsmanExperimentator
    {
        protected override Type QuestType { get { return typeof(BlacksmithyExperiment); } }
        protected override CraftingExperiment CreateQuestSystem(PlayerMobile player)
        {
            return new BlacksmithyExperiment(player);
        }

        [Constructable]
        public BlacksmithExperimentator() : base(" - szalony kowal")
        {
        }

        public BlacksmithExperimentator(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class BowyerExperimentator : CraftsmanExperimentator
    {
        protected override Type QuestType { get { return typeof(BowFletchingExperiment); } }
        protected override CraftingExperiment CreateQuestSystem(PlayerMobile player)
        {
            return new BowFletchingExperiment(player);
        }

        [Constructable]
        public BowyerExperimentator() : base(" - szalony ³ukmistrz")
        {
        }

        public BowyerExperimentator(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public abstract class CraftsmanExperimentator : BaseQuester
    {
        protected abstract Type QuestType { get; }
        protected abstract CraftingExperiment CreateQuestSystem(PlayerMobile player);

        public CraftsmanExperimentator(string title) : base(title)
        {
        }

        public CraftsmanExperimentator(Serial serial) : base(serial)
        {
        }

        public override void OnTalk(PlayerMobile player, bool contextMenu)
        {
            this.Direction = GetDirectionTo(player);

            CraftingExperiment qs = player.Quest as CraftingExperiment;

            if (qs != null)
            {
                if (qs.IsObjectiveInProgress(typeof(BringRareResourceObjective)))
                {
                    qs.AddConversation(new DuringBringRareResourceConversation());
                }
                else if (qs.IsObjectiveInProgress(typeof(BringProductsObjective)))
                {
                    qs.AddConversation(new DuringBringProductsConversation());
                }
                //else if (qs.IsObjectiveInProgress(typeof(GetReward)))
                //{
                //    GetReward obj = (GetReward) qs.FindObjective(typeof(GetReward));
                //    if (obj != null)
                //    {
                //        obj.Complete();

                //        qs.GiveRewardTo(player);
                //    }
                //}
                else
                {
                    SayTo(player, "...co to ja mialem...");
                }
            }
            else
            {
                QuestSystem newQuest = CreateQuestSystem(player);

                if (player.Quest == null && QuestSystem.CanOfferQuest(player, QuestType))
                {
                    newQuest.SendOffer();
                }
                else
                {
                    newQuest.AddConversation(new DontOfferConversation());
                }
            }
        }

        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            this.Direction = GetDirectionTo(from);

            PlayerMobile player = from as PlayerMobile;

            if (player != null)
            {
                CraftingExperiment qs = player.Quest as CraftingExperiment;
                if (qs != null)
                {
                    QuestObjective obj = qs.FindObjective(typeof(BringRareResourceObjective));

                    if (obj != null && !obj.Completed)
                    {
                        BringRareResourceObjective resObj = obj as BringRareResourceObjective;
                        if (resObj != null && resObj.IsProperProduct(dropped))
                        {
                            int required = obj.MaxProgress - obj.CurProgress;

                            if (dropped.Amount <= required)
                            {
                                obj.CurProgress += dropped.Amount;

                                dropped.Delete();

                                if (dropped.Amount < required)
                                    qs.ShowQuestLogUpdated();
                                SayTo(player, "Dziekuje!");

                                return true;
                            }
                            else
                            {
                                dropped.Amount -= required;

                                obj.Complete();

                                return false;
                            }
                        }
                    }

                    obj = qs.FindObjective(typeof(BringProductsObjective));

                    if (obj != null && !obj.Completed)
                    {
                        // TODO: dac mozliwosc oddawania itemow w pojemniku.

                        BringProductsObjective prodObj = obj as BringProductsObjective;
                        if (prodObj != null && prodObj.IsProperProduct(dropped))
                        {
                            int required = obj.MaxProgress - obj.CurProgress;
                            if (dropped.Amount <= required)
                            {
                                obj.CurProgress += dropped.Amount;

                                dropped.Delete();

                                if (dropped.Amount < required)
                                    qs.ShowQuestLogUpdated();

                                SayTo(player, "Dziekuje!");

                                return true;
                            }
                            else
                            {
                                dropped.Amount -= required;

                                obj.Complete();

                                return false;
                            }
                        }
                    }
                }
            }

            SayTo(player, "To mi sie nie przyda...");

            return base.OnDragDrop(from, dropped);
        }

        public override void InitSBInfo()
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}