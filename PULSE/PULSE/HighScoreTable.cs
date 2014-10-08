using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PulseGame
{
    class HighScoreTable
    {
        // Holds the player's name and their score
        public struct TableEntry
        {
            public int score;
            public string playerName;
        }

        private const int MAX_TABLE_SIZE = 10;
        private const string PATH = "scores.txt";
        private TableEntry[] highScores;

        public HighScoreTable()
        {
            highScores = new TableEntry[MAX_TABLE_SIZE];
        }

        public void LoadTable()
        {
            // Create the file if it doesn't exist
            if (!File.Exists(PATH))
            {
                File.Create(PATH);
            }

            // Read the file
            int i = 0;
            foreach (var line in File.ReadLines(PATH))
            {
                var tokens = line.Split(',');

                TableEntry entry = new TableEntry();
                entry.score = int.Parse(tokens[0]);
                entry.playerName = tokens[1];

                // Verify we don't exceed bounds
                // this should only be an issue if the user modifies the scores file
                // We will discard all values after the cap has been reached (even if they are higher values)
                if (i < MAX_TABLE_SIZE)
                    highScores[i] = entry;
                else
                    break;

                i++;
            }
            Array.Sort(highScores, (s1, s2) => s2.score.CompareTo(s1.score));
        }

        public void AddScoreToTable(int playerScore)
        {
            // Check for a new high score
            if (playerScore > highScores[MAX_TABLE_SIZE - 1].score)
            {
                highScores[MAX_TABLE_SIZE - 1].playerName = "Player1"; // TODO update this to allow for user input
                highScores[MAX_TABLE_SIZE - 1].score = playerScore;
                Array.Sort(highScores, (s1, s2) => s2.score.CompareTo(s1.score));
            }
        }

        public TableEntry[] GetHighScores()
        {
            return highScores;
        }
    }
}
