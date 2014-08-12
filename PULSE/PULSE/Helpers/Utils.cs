using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SampleGame.Helpers
{
    public static class Utils
    {
        // rotation float to vector2 movement
        public static Vector2 CalculateRotatedMovement(Vector2 point, float rotation)
        {
            return Vector2.Transform(point, Matrix.CreateRotationZ(rotation));
        }

        // calculates the rotation to a target vector
        public static float GetRotationToTarget(Vector2 targetPos, Vector2 agentPos)
        {
            Vector2 dist = targetPos - agentPos;

            float rotation = (float)Math.Atan2(dist.X, -dist.Y);
            while (rotation < 0) rotation += MathHelper.TwoPi;
            return rotation;
        }

        // float to degrees
        public static float GetRotationInDegrees(float Rotation)
        {
            return (float)Math.Round(Rotation * 180 / MathHelper.Pi, 2);
        }
    }
}
