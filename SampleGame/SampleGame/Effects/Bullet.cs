using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using SampleGame.Helpers;

namespace SampleGame.Effects
{
    public class Bullet : Effect
    {
        public int MaxSpeed;

        public override void Update(GameTime gameTime, int levelWidth, int levelHeight)
        {
            Position += Utils.CalculateRotatedMovement(new Vector2(0, -1), Rotation) * MaxSpeed;

            Active = IsInLevelBounds(levelWidth, levelHeight);
        }
    }
}
