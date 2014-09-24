using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace PulseGame.Helpers
{
    public class TargetVectorHelper
    {
        public float TargetDistance;
        public Vector2 TargetPosition;
        public bool IsIntersecting;

        //Enums.Direction.Up, Position, Origin, Scale, MaxSpeed, targetPos, Bounds.Width, Bounds.Height),
        public TargetVectorHelper(Enums.Direction direction, Vector2 currentPos, 
                                  Vector2 agentOrigin, float agentScale, int agentSpacing, 
                                  int agentWidth, int agentHeight, int maxSpeed, 
                                  Vector2 targetPos, List<GameAgent> agentList)
        {
            Vector2 leftTargetPos = new Vector2(currentPos.X - maxSpeed, currentPos.Y);
            Vector2 rightTargetPos = new Vector2(currentPos.X + maxSpeed, currentPos.Y);
            Vector2 upTargetPos = new Vector2(currentPos.X, currentPos.Y - maxSpeed);
            Vector2 downTargetPos = new Vector2(currentPos.X, currentPos.Y + maxSpeed);

            float leftTargetDist = Vector2.Distance(leftTargetPos, targetPos);
            float rightTargetDist = Vector2.Distance(rightTargetPos, targetPos);
            float upTargetDist = Vector2.Distance(upTargetPos, targetPos);
            float downTargetDist = Vector2.Distance(downTargetPos, targetPos);

            switch (direction)
            {
                case Enums.Direction.Left:  TargetPosition = new Vector2(currentPos.X - maxSpeed, currentPos.Y); break;
                case Enums.Direction.Right: TargetPosition = new Vector2(currentPos.X + maxSpeed, currentPos.Y); break;
                case Enums.Direction.Up:    TargetPosition = new Vector2(currentPos.X, currentPos.Y - maxSpeed); break;
                case Enums.Direction.Down:  TargetPosition = new Vector2(currentPos.X, currentPos.Y + maxSpeed); break;
            }

            TargetDistance = Vector2.Distance(TargetPosition, targetPos);

            Rectangle targetBounds = new Rectangle
            (
                (int)(TargetPosition.X - agentOrigin.X * agentScale - agentSpacing),
                (int)(TargetPosition.Y - agentOrigin.Y * agentScale - agentSpacing),
                agentWidth + 2 * agentSpacing,
                agentHeight + 2 * agentSpacing
            );

            // count must be greater than one since the targetBounds is always going to 
            // intersect the current agent that is trying to move.
            IsIntersecting = agentList.Where(ga => ga.Bounds.Intersects(targetBounds)).Count() > 1;
        }
    }
}
