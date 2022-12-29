using System;
using Server;
using Server.Mobiles;
using System.Collections.Generic;
using Server.Commands;
using Server.Targeting;

namespace Nelderim.Engines.ChaosChest
{
    public class ChaosChestQuest : Item
    {
        private static readonly double DEFAULT_DROP_CHANCE = 0.01;
        private static double DROP_CHANCE_OVERRIDE;
        private static ChaosSigilType CURRENT_STAGE_OVERRIDE;
        private static ChaosChestQuest instance;

        private static Dictionary<string, ChaosSigilType> REGION_MAP = new Dictionary<string, ChaosSigilType>
        {
            {"TylReviaren", ChaosSigilType.Natury},
            {"Melisande_VeryEasy", ChaosSigilType.Natury},
            {"Melisande_Easy", ChaosSigilType.Natury},
            {"Melisande_Medium", ChaosSigilType.Natury},
            {"Melisande_Difficult", ChaosSigilType.Natury},
            {"Melisande_VeryDifficult", ChaosSigilType.Natury},
            {"Piramida", ChaosSigilType.Natury},
            {"Elbrind", ChaosSigilType.Morlokow},
            {"CzerwoneOrki", ChaosSigilType.Morlokow},
            {"Hurengrav_LVL1", ChaosSigilType.Smierci},
            {"WielkaPokracznaBestia_LVL2_VeryEasy", ChaosSigilType.Smierci},
            {"WielkaPokracznaBestia_LVL2_Easy", ChaosSigilType.Smierci},
            {"WielkaPokracznaBestia_LVL2_Medium", ChaosSigilType.Smierci},
            {"WielkaPokracznaBestia_LVL2_Difficult", ChaosSigilType.Smierci},
            {"WielkaPokracznaBestia_LVL2_VeryDifficult", ChaosSigilType.Smierci},
            {"WielkaPokracznaBestia_LVL1", ChaosSigilType.Smierci},
            {"KryptyNaurow", ChaosSigilType.Smierci},
            {"KrysztalowaJaskinia", ChaosSigilType.Krysztalow},
            {"KrysztaloweSmoki", ChaosSigilType.Krysztalow},
            {"Wulkan_LVL3_Difficult", ChaosSigilType.Ognia},
            {"Wulkan_LVL3_VeryDifficult", ChaosSigilType.Ognia},
            {"Wulkan_LVL4_VeryDifficult", ChaosSigilType.Ognia},
            {"LezeOgnistychSmokow_LVL2", ChaosSigilType.Ognia},
            {"LezeOgnistychSmokow_LVL3", ChaosSigilType.Ognia},
            {"Demonowo", ChaosSigilType.Ognia},
            {"Shimmering_LV1_VeryEasy", ChaosSigilType.Swiatla},
            {"Shimmering_LV1_Easy", ChaosSigilType.Swiatla},
            {"Shimmering_LV1_Medium", ChaosSigilType.Swiatla},
            {"Shimmering_LV1_Difficult", ChaosSigilType.Swiatla},
            {"Shimmering_LV1_VeryDifficult", ChaosSigilType.Swiatla},
            {"Shimmering_LV2_VeryEasy", ChaosSigilType.Swiatla},
            {"Shimmering_LV2_Easy", ChaosSigilType.Swiatla},
            {"Shimmering_LV2_Medium", ChaosSigilType.Swiatla},
            {"Shimmering_LV2_Difficult", ChaosSigilType.Swiatla},
            {"Shimmering_LV2_VeryDifficult", ChaosSigilType.Swiatla},
            {"BialyWilk_VeryEasy", ChaosSigilType.Swiatla},
            {"BialyWilk_Easy", ChaosSigilType.Swiatla},
            {"BialyWilk_Medium", ChaosSigilType.Swiatla},
            {"BialyWilk_Difficult", ChaosSigilType.Swiatla},
            {"BialyWilk_VeryDifficult", ChaosSigilType.Swiatla},
            {"UlnhyrOrben", ChaosSigilType.Swiatla},
            {"HallTorech_LVL1", ChaosSigilType.Licza},
            {"HallTorech_LVL2", ChaosSigilType.Licza},
            {"HallTorech_LVL3", ChaosSigilType.Licza},
            {"Garth_LVL1", ChaosSigilType.Licza},
            {"Garth_LVL2", ChaosSigilType.Licza}
        };

        public static void Initialize()
        {
            CommandSystem.Register("CreateChaosChestQuest", AccessLevel.Counselor, CreateChaosChestQuest);
        }
        
        private static void CreateChaosChestQuest(CommandEventArgs e)
        {
            if(instance == null)
                e.Mobile.Target = new CreateTarget();
            else
            {
                e.Mobile.Location = instance.Location;
                e.Mobile.Map = instance.Map;
            }
        }

        internal static ChaosSigilType CURRENT_STAGE
        {
            get
            {
                return CURRENT_STAGE_OVERRIDE != ChaosSigilType.NONE
                    ? CURRENT_STAGE_OVERRIDE
                    : (ChaosSigilType) Math.Pow(2, (int) DateTime.Now.DayOfWeek);
            }
        }
        
        private static double DROP_CHANCE
        {
            get
            {
                return DROP_CHANCE_OVERRIDE > 0.0d
                    ? DROP_CHANCE_OVERRIDE
                    : DEFAULT_DROP_CHANCE;
            }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public double DropChanceOverride
        {
            get { return DROP_CHANCE_OVERRIDE; }
            set
            {
                DROP_CHANCE_OVERRIDE = value;
            }
        }
        
        [CommandProperty( AccessLevel.GameMaster )]
        public double DropChance
        {
            get { return DROP_CHANCE; }
        }
        
        [CommandProperty( AccessLevel.GameMaster )]
        public ChaosSigilType CurrentStageOverride
        {
            get { return CURRENT_STAGE_OVERRIDE; }
            set
            {
                CURRENT_STAGE_OVERRIDE = value;
            }
        }
        
        [CommandProperty( AccessLevel.GameMaster )]
        public ChaosSigilType CurrentStage
        {
            get { return CURRENT_STAGE; }
        }
        
        private ChaosChestQuest() : base(0x1BC3)
        {
            Movable = false;
        }

        public ChaosChestQuest(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int) 0); // version
            writer.Write((int)CurrentStageOverride);
            writer.Write(DROP_CHANCE_OVERRIDE);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            CurrentStageOverride = (ChaosSigilType)reader.ReadInt();
            DropChanceOverride = reader.ReadDouble();
        }

        public static void OnBeforeDeath(BaseCreature baseCreature)
        {
            if (baseCreature != null && baseCreature.Region != null &&
                baseCreature.Region.Name != null &&
                REGION_MAP.ContainsKey(baseCreature.Region.Name) && 
                REGION_MAP[baseCreature.Region.Name].Equals(CURRENT_STAGE))
            {
                if (Utility.RandomDouble() < DROP_CHANCE)
                {
                    baseCreature.AddToBackpack(new ChaosKey(CURRENT_STAGE));
                }
            }
        }
        
        private class CreateTarget : Target
		{
			public CreateTarget( ) : base( -1, true, TargetFlags.None )
			{
			}

 			protected override void OnTarget( Mobile from, object targeted )
			{
                IPoint3D location = targeted as IPoint3D;

                if ( location != null )
                {
                    ChaosChestQuest quest = new ChaosChestQuest();

                    quest.MoveToWorld( new Point3D( location ), from.Map );
                    instance = quest;
                }
                else
                {
                    from.SendMessage( "Invalid location" );
                }
			}
		}
    }
}