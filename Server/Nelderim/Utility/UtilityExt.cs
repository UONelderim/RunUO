
using System.Collections.Generic;
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
        
        public static T RandomWeigthed<T>(Dictionary<T, int> weightedItems)
        {
            if (weightedItems != null)
            {
                var sum = weightedItems.Values.Sum();
                var rnd = Random(sum);
                foreach (var keyValue in weightedItems)
                {
                    if (rnd < keyValue.Value)
                        return keyValue.Key;
                    rnd -= keyValue.Value;
                }
            }
            return default;
        }
        
        public static T RandomWeigthed<T>(Dictionary<T, double> weightedItems)
        {
            const double resolution = 1000;
            if (weightedItems != null)
            {
                var sum = weightedItems.Values.Sum();
                var rnd = Random((int)(sum * resolution)) / resolution;
                foreach (var keyValue in weightedItems)
                {
                    if (rnd < keyValue.Value)
                        return keyValue.Key;
                    rnd -= keyValue.Value;
                }
            }
            return default;
        }
    }
}