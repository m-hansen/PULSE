using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SampleGame
{
    public class LevelTimeSpan
    {
        public long StartTime;
        public long EndTime;
        public int Difficulty; // max amount of enemies spawning per tick 

        public LevelTimeSpan(long startTime, long endTime, int difficulty)
        {
            StartTime = startTime;
            EndTime = endTime;
            Difficulty = difficulty;
        }
    }
}
