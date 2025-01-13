using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Arya.Chess;
using Server.ACC.CSS.Systems.Avatar;
using Server.ACC.CSS.Systems.Chivalry;
using Server.ACC.CSS.Systems.Mage;
using Server.ACC.CSS.Systems.Necromancy;
using Server.Engines.BulkOrders;
using Server.Engines.Quests.CraftingExperiments;
using Server.Engines.Quests.Haven;
using Server.Engines.Quests.Necro;
using Server.Engines.Quests.Samurai;
using Server.Ethics;
using Server.Items;
using Server.Mobiles;

namespace Server.Commands
{
    public static class ServUoSave
    {
        private static readonly List<Type> itemsToDelete = new List<Type>
        {
            typeof(DesCityWallEast), typeof(DesCityWallSouth), typeof(PowerHourScroll), typeof(TrashBarrel), 
            typeof(FishingPole), typeof(RewardScroll), typeof(SheafOfHay), typeof(ArcaneFocus), typeof(SmallHunterBOD), 
            typeof(LargeHunterBOD), typeof(Aquarium), typeof(HuntingBulkOrderBook), typeof(ZakrwawioneBandaze), 
            typeof(GoldenWool), typeof(SiegeComponent), typeof(FieryCannonball), typeof(SiegeCannon), 
            typeof(MageSpellbook), typeof(NecroSpellbook), typeof(ChivalrySpellbook), typeof(MaabusCoffinComponent), 
            typeof(IronSiegeLog), typeof(ExplodingCannonball), typeof(SiegeRam), typeof(HeavySiegeLog), 
            typeof(IronCannonball), typeof(SiegeCatapult), typeof(FairyTree_Addon), typeof(MaabusCoffin),
            typeof(AvatarEnemyOfOneScroll), typeof(EnchantedSextant), typeof(MieczDalekiegoZasiegu), 
            typeof(EthicsPersistance), typeof(ChessControl), typeof(WinnerPaper), typeof(DragonDust),
            typeof(DaemonBloodChest), 
        };

        private static readonly List<Type> mobilesToDelete = new List<Type>
        {
            typeof(IDamageableItem), typeof(Ninja), typeof(MasterMikael), typeof(Putrefier),
            typeof(Malefic), typeof(TownCrier), typeof(WarHorseA), typeof(WarHorseB), typeof(WarHorseC),
            typeof(WarHorseD), typeof(WarHorseE), typeof(BowyerExperimentator), typeof(BlacksmithExperimentator), 
            typeof(TailorExperimentator), typeof(SeekerOfAdventure), typeof(BaseEscortable), typeof(XmlQuestNPCNude), 
            typeof(XmlQuestNPCNude2), typeof(XmlQuestNPCMieszczanin), typeof(XmlQuestNPCGornik), typeof(XmlQuestNPCMurzyn), 
            typeof(ChaosGuard), typeof(XmlQuestNPCChlop), typeof(DeadlyImp), typeof(XmlQuestNPCFuglus), 
            typeof(XmlQuestNPCPijak), typeof(EscortableMage), typeof(Noble), typeof(DrowAnimalTrainer),
            typeof(EvolutionDragon), typeof(EvolutionDragon2), typeof(EvolutionDragon3), typeof(EvolutionDragon4),
            typeof(EvolutionDragon5), typeof(FerelTreefellow), typeof(Swoop), typeof(NBurugh)
        };

        public static void Initialize()
        {
            CommandSystem.Register("ServUoSave", AccessLevel.GameMaster, new CommandEventHandler(ServUoSave_Command));
        }

        private static void ServUoSave_Command(CommandEventArgs e)
        {
            if (Config.Shard_Local)
            {
                List<XmlSpawner> toReset = new List<XmlSpawner>();
                foreach (var item in World.Items.Values.Where(x => x is XmlSpawner).ToList())
                {
                    var spawner = (XmlSpawner)item;
                    if (spawner.WayPoint != null)
                    {
                        toReset.Add(spawner);
                    }
                }

                foreach (var xmlSpawner in toReset)
                {
                    xmlSpawner.Reset();
                }

                List<IEntity> toDelete = new List<IEntity>();
                foreach (var item in World.Items.Values.Where(x => itemsToDelete.Contains(x.GetType())))
                {
                    toDelete.Add(item);
                }

                foreach (var mobile in World.Mobiles.Values.Where(x => mobilesToDelete.Contains(x.GetType())))
                {
                    toDelete.Add(mobile);
                }

                foreach (var entity in toDelete)
                {
                    entity.Delete();
                }

                // foreach (var type in mobilesToDelete)
                // {
                // World.m_MobileTypes.Remove(type);
                // }

                // foreach (var type in itemsToDelete)
                // {
                // World.m_ItemTypes.Remove(type);
                // }

                World.ServUOSave = true;
                try
                {
                    Misc.AutoSave.Save();
                    File.Delete("Servuo/Saves/Nelderim/Gains.sav");
                    if(!File.Exists("Servuo/Saves/Nelderim/Languages.sav"))
                        File.Move("Servuo/Saves/Nelderim/Speech.sav", "Servuo/Saves/Nelderim/Languages.sav");
                }
                finally
                {
                    World.ServUOSave = false;
                }
            }
            else
            {
                e.Mobile.SendMessage("Command disabled!");
            }
        }
    }
}