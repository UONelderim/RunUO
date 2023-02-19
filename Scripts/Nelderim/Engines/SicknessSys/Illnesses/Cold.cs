using Nelderim.Scripts.Nelderim.Engines.SicknessSys.Illnesses;

namespace Server.SicknessSys.Illnesses
{
    public static class Cold
    {
        private static int illnessType = 1;
        private static string name;
        private static int statDrain;
        private static int baseDamage;
        private static int powerDegenRate;

        public static int IllnessType
        {
            get
            {
                return illnessType;
            }
        }

        public static string Name
        {
            get
            {
                if (name == null)
                {
                    name = BaseVirus.GetRandomName(IllnessType);
                }
                return name;
            }
        }

        public static int StatDrain
        {
            get
            {
                if (statDrain == 0)
                {
                    statDrain = BaseVirus.GetRandomDamage(IllnessType);
                }
                return statDrain;
            }
        }

        public static int BaseDamage
        {
            get
            {
                if (baseDamage == 0)
                {
                    baseDamage = BaseVirus.GetRandomDamage(IllnessType);
                }
                return baseDamage;
            }
        }

        public static int PowerDegenRate
        {
            get
            {
                if (powerDegenRate == 0)
                {
                    powerDegenRate = BaseVirus.GetRandomDegen(IllnessType);
                }
                return powerDegenRate;
            }
        }

        public static void UpdateBody(VirusCell cell)
        {
            IllnessMutationLists.SetMutation(cell);
        }
    }
}