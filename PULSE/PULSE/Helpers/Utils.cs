using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PulseGame.Helpers
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

        // Convert a texture to a color array
        public static Color[] TextureToArray(Texture2D texture)
        {
            Color[] colorArray = new Color[texture.Width * texture.Height];
            texture.GetData(colorArray);

            return colorArray;
        }

        // Per-pixel collision
        public static bool IntersectPixels(Rectangle rectangleA, Color[] dataA,
                                    Rectangle rectangleB, Color[] dataB)
        {
            // Find the bounds of the rectangle intersection
            int top = Math.Max(rectangleA.Top, rectangleB.Top);
            int bottom = Math.Min(rectangleA.Bottom, rectangleB.Bottom);
            int left = Math.Max(rectangleA.Left, rectangleB.Left);
            int right = Math.Min(rectangleA.Right, rectangleB.Right);

            // Check every point within the intersection bounds
            for (int y = top; y < bottom; y++)
            {
                for (int x = left; x < right; x++)
                {
                    // Get the color of both pixels at this point
                    Color colorA = dataA[(x - rectangleA.Left) +
                                         (y - rectangleA.Top) * rectangleA.Width];
                    Color colorB = dataB[(x - rectangleB.Left) +
                                         (y - rectangleB.Top) * rectangleB.Width];

                    // If both pixels are not completely transparent,
                    if (colorA.A != 0 && colorB.A != 0)
                    {
                        // then an intersection has been found
                        return true;
                    }
                }
            }

            // No intersection found
            return false;
        }

    }
}
