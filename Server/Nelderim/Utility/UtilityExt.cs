
using System.Linq;

namespace Server
{
    public static partial class Utility
    {
        public static int RandomIndex(int[] chances)
        {
            int chancesSum = chances.Sum();
            int rand = Random(chancesSum);
            
            for (int i = 0; i < chances.Length; i++)
            {
                int chance = chances[i];

                if (rand < chance)
                    return i;
                rand -= chance;
            }

            return chances.Length - 1;
        }
    }
}