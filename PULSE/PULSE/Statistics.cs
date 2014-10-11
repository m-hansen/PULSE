using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PulseGame
{
    public enum StatType
    {
        SHOT,
        ENEMY_KILL,
        MOUSE_CLICK,
        SCORE,
        NUM_STATS
    };

    class Statistics
    {

        private static int[] stats = new int[(int)StatType.NUM_STATS];

        public Statistics()
        {
            //stats = new int[(int)StatType.NUM_STATS];
        }
        
        public static void Increment(int index)
        {
            if (index >= (int)StatType.NUM_STATS)
                return;
            stats[index]++;
        }

        public static int GetStat(int index)
        {
            return stats[index];
        }

        public static void Clear()
        {
            for (int i = 0; i < stats.Length; i++)
            {
                stats[i] = 0;
            }
        }

    }
}
