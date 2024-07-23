using Server.Mobiles;
using System;
using System.Collections.Generic;

namespace Server.Engines.MiniChamps
{
    public enum MiniChampType
    {
        //We just cut out unused ones until servuo
        MeraktusTheTormented = 12,
        Pyre,
        Morena,
        OrcCommander
    }

    public class MiniChampTypeInfo
    {
        public int Required { get; set; }
        public Type SpawnType { get; set; }

        public MiniChampTypeInfo(int required, Type spawnType)
        {
            Required = required;
            SpawnType = spawnType;
        }
    }

    public class MiniChampLevelInfo
    {
        public MiniChampTypeInfo[] Types { get; set; }

        public MiniChampLevelInfo(params MiniChampTypeInfo[] types)
        {
            Types = types;
        }
    }

    public class MiniChampInfo
    {
        public MiniChampLevelInfo[] Levels { get; set; }
        public Type EssenceType { get; set; }

        public int MaxLevel => Levels == null ? 0 : Levels.Length;

        public MiniChampInfo(Type essenceType, params MiniChampLevelInfo[] levels)
        {
            Levels = levels;
            EssenceType = essenceType;
        }

        public MiniChampLevelInfo GetLevelInfo(int level)
        {
            level--;

            if (level >= 0 && level < Levels.Length)
                return Levels[level];

            return null;
        }

        public static Dictionary<MiniChampType, MiniChampInfo> Table => m_Table;

        private static readonly Dictionary<MiniChampType, MiniChampInfo> m_Table = new()
        {
            {
                MiniChampType.MeraktusTheTormented,
                new MiniChampInfo(
                    null,
                    new MiniChampLevelInfo
                    (
                        new MiniChampTypeInfo(20, typeof(Minotaur))
                    ),
                    new MiniChampLevelInfo
                    (
                        new MiniChampTypeInfo(20, typeof(MinotaurScout))
                    ),
                    new MiniChampLevelInfo
                    (
                        new MiniChampTypeInfo(15, typeof(MinotaurCaptain))
                    ),
                    new MiniChampLevelInfo
                    (
                        new MiniChampTypeInfo(15, typeof(MinotaurCaptain))
                    ),
                    new MiniChampLevelInfo
                    (
                        new MiniChampTypeInfo(1, typeof(Meraktus))
                    )
                )
            },
            {
                MiniChampType.Pyre,
                new MiniChampInfo(
                    null,
                    new MiniChampLevelInfo(
                        new MiniChampTypeInfo(20, typeof(FireElemental)),
                        new MiniChampTypeInfo(20, typeof(OgnistyWojownik)),
                        new MiniChampTypeInfo(20, typeof(OgnistyNiewolnik))
                    ),
                    new MiniChampLevelInfo(
                        new MiniChampTypeInfo(15, typeof(DullCopperElemental)),
                        new MiniChampTypeInfo(15, typeof(FireGargoyle)),
                        new MiniChampTypeInfo(15, typeof(GargoyleEnforcer))
                    ),
                    new MiniChampLevelInfo(
                        new MiniChampTypeInfo(10, typeof(EnslavedGargoyle)),
                        new MiniChampTypeInfo(10, typeof(OgnistySmok)),
                        new MiniChampTypeInfo(10, typeof(FireBeetle))
                    ),
                    new MiniChampLevelInfo(
                        new MiniChampTypeInfo(5, typeof(FireSteed)),
                        new MiniChampTypeInfo(5, typeof(PrastaryOgnistySmok)),
                        new MiniChampTypeInfo(5, typeof(feniks))
                    ),
                    new MiniChampLevelInfo
                    (
                        new MiniChampTypeInfo(1, typeof(Pyre))
                    )
                )
            },
            {
                MiniChampType.Morena,
                new MiniChampInfo(
                    null,
                    new MiniChampLevelInfo(
                        new MiniChampTypeInfo(25, typeof(Ghoul)),
                        new MiniChampTypeInfo(25, typeof(Skeleton)),
                        new MiniChampTypeInfo(25, typeof(PatchworkSkeleton))
                    ),
                    new MiniChampLevelInfo(
                        new MiniChampTypeInfo(20, typeof(WailingBanshee)),
                        new MiniChampTypeInfo(20, typeof(BoneMagi)),
                        new MiniChampTypeInfo(20, typeof(BoneKnight))
                    ),
                    new MiniChampLevelInfo(
                        new MiniChampTypeInfo(15, typeof(LichLord)),
                        new MiniChampTypeInfo(15, typeof(FleshGolem)),
                        new MiniChampTypeInfo(15, typeof(Mummy2))
                    ),
                    new MiniChampLevelInfo(
                        new MiniChampTypeInfo(10, typeof(SkeletalDragon)),
                        new MiniChampTypeInfo(10, typeof(RottingCorpse)),
                        new MiniChampTypeInfo(10, typeof(AncientLich))
                    ),
                    new MiniChampLevelInfo
                    (
                        new MiniChampTypeInfo(1, typeof(MorenaAwatar))
                    )
                )
            },
            {
                MiniChampType.OrcCommander,
                new MiniChampInfo(
                    null,
                    new MiniChampLevelInfo(
                        new MiniChampTypeInfo(25, typeof(Orc)),
                        new MiniChampTypeInfo(25, typeof(Ratman)),
                        new MiniChampTypeInfo(25, typeof(Goblin))
                    ),
                    new MiniChampLevelInfo(
                        new MiniChampTypeInfo(20, typeof(OrcishMage)),
                        new MiniChampTypeInfo(20, typeof(LesserGoblinSapper)),
                        new MiniChampTypeInfo(20, typeof(Troll))
                    ),
                    new MiniChampLevelInfo(
                        new MiniChampTypeInfo(15, typeof(JukaWarrior)),
                        new MiniChampTypeInfo(15, typeof(OrcCaptain)),
                        new MiniChampTypeInfo(15, typeof(TrollLord))
                    ),
                    new MiniChampLevelInfo(
                        new MiniChampTypeInfo(15, typeof(JukaMage)),
                        new MiniChampTypeInfo(15, typeof(OrcBomber)),
                        new MiniChampTypeInfo(15, typeof(OgreLord))
                    ),
                    new MiniChampLevelInfo
                    (
                        new MiniChampTypeInfo(1, typeof(KapitanIIILegionuOrkow))
                    )
                )
            }
        };

        public static MiniChampInfo GetInfo(MiniChampType type)
        {
            if (Table.TryGetValue(type, out var info))
            {
                return info;
            }

            Console.WriteLine($"Unable to get ChampionSpawnInfo for {type}");
            return Table[MiniChampType.MeraktusTheTormented];
        }
    }
}