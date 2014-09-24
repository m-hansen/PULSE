using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace PulseGame
{
    public class LevelNode
    {
        public Rectangle Bounds;
        public LevelNode Parent = null;
        public float F;
        public float G;

        public void CalculateHeuristic(Vector2 targetPos, Point startPoint, float previousNodeGValue)
        {
            G = Vector2.Distance(new Vector2(Bounds.Center.X, Bounds.Center.Y), new Vector2(startPoint.X, startPoint.Y)) + previousNodeGValue;
            float H = Vector2.Distance(new Vector2(Bounds.Center.X, Bounds.Center.Y), targetPos);

            F = G + H;
        }
    }
}
