using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SampleGame.Helpers
{
    public class TextureInfo
    {
        public string TextureString;
        public int Frames;
        public Rectangle TextureRect;
        public float Scale = 1.0f;

        public TextureInfo(string textureString)
        {
            switch (textureString)
            {
                case "enemy_agent1":

                    TextureString = "Images\\enemy_agent1";
                    TextureRect = new Rectangle(0, 0, 26, 29);
                    Frames = 4;

                    break;

                case "follower1":

                    TextureString = "Images\\follower1";
                    TextureRect = new Rectangle(0, 0, 46, 42);
                    Frames = 4;

                    break;

                case "kamikaze1":

                    TextureString = "Images\\kamikaze1";
                    TextureRect = new Rectangle(0, 0, 34, 43);
                    Frames = 4;

                    break;

                case "big_ship1":

                    TextureString = "Images\\big_ship1";
                    TextureRect = new Rectangle(0, 0, 120, 85);
                    Frames = 4;

                    break;

                case "boss1":

                    TextureString = "Images\\boss1";
                    TextureRect = new Rectangle(0, 0, 528, 358);
                    Frames = 4;

                    break;

                case "star_effect2":

                    TextureString = "Images\\star_effect2";
                    TextureRect = new Rectangle(0, 0, 100, 100);
                    Frames = 16;

                    break;
            }
        }
    }
}
