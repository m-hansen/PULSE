using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Timers;
using System.Diagnostics;
using SampleGame.Helpers;

namespace SampleGame
{
    public class MovingAgent : GameAgent
    {
        #region Public Constants

        //public Vector2 Velocity;                        // which direction the object is heading
        public bool Moving = true;                      // whether the object is moving
        public int MaxSpeed;
        //public List<LevelNode> PathToTarget;
        //public List<Sensor> SensorList = new List<Sensor>();
        //public List<Vector2> PathList;
        public int PathIndex = 0;
        public float TargetRotation;
        public float Health;

        public int TimeAtEachPoint;
        public int NPCRange;
        public int MaxFollowRange;
        public Vector2 PlaceBeforeFollow;
        public int MeleeDistance;

        public Vector2 TargetPosition;
        public int TargetIndex = 0;

        #endregion

        #region Protected Constants

        protected int timeAtCurrentNode = 0;

        #endregion

        public MovingAgent()
        {

        }

        #region Update Method

        public virtual void Update(GameTime gametime, Player playerObj, List<GameAgent> agentList, int nodeSize)
        {
            // if the object is active on the screen
            if (Moving)
            {
                // if the image is a sprite sheet 
                // and if enough time has passed to where we need to move to the next frame
                if (TotalFrames > 1 && (animElapsed += gametime.ElapsedGameTime) > AnimationInterval)
                {
                    if (++currentFrame == TotalFrames)
                        currentFrame = 0;               

                    // move back by the animation interval (in miliseconds)
                    animElapsed -= AnimationInterval;
                }

                // move the object by the velocity
                //Position += Velocity;

                //foreach (Sensor sensor in SensorList)
                //{
                //    sensor.Update(agentList, Position, Rotation);
                //}
            }
        }

        #endregion

        #region Unused

        //#region Running To Point State Methods

        //private void GoToPoint(List<GameAgent> wallList)
        //{
        //    if (TargetIndex >= PathToTarget.Count - 1)
        //    {
        //        if (Vector2.Distance(TargetPosition, Position) > MeleeDistance)
        //            SetRotationForPoint(TargetPosition, wallList);
        //        return;
        //    }

        //    LevelNode currentNode = PathToTarget[TargetIndex];

        //    if (Vector2.Distance(Position, new Vector2(currentNode.Bounds.Center.X, currentNode.Bounds.Center.Y)) < Bounds.Width)// / 2)
        //    {
        //        TargetIndex++;
        //    }
        //    else
        //    {
        //        SetRotationForPoint(new Vector2(currentNode.Bounds.Center.X, currentNode.Bounds.Center.Y), wallList);
        //        return;
        //    }

        //    if (TargetIndex >= PathToTarget.Count - 1)
        //    {
        //        SetRotationForPoint(TargetPosition, wallList);
        //        return;
        //    }

        //    currentNode = PathToTarget[TargetIndex];
        //    SetRotationForPoint(new Vector2(currentNode.Bounds.Center.X, currentNode.Bounds.Center.Y), wallList);
        //}

        //private void SetRotationForPoint(Vector2 targetPos, List<GameAgent> wallList)
        //{
        //    Vector2 targetPosition = targetPos - Position;

        //    Rotation = GetRotForSteeringForce(targetPosition);

        //    targetPosition.Normalize();

        //    // the amount of times the target position has been adjusted
        //    int adjustedCount = 1;

        //    // if a range finder is intersecting a wall, adjusting the target position.
        //    foreach (RangeFinder rangeFinder in SensorList)
        //    {
        //        Vector2? adjustedSteering = rangeFinder.AdjustRotationForCollision(Position);

        //        if (adjustedSteering.HasValue)
        //        {
        //            adjustedSteering.Value.Normalize();
        //            targetPosition += adjustedSteering.Value;
        //            adjustedCount++;
        //        }
        //    }

        //    targetPosition /= adjustedCount;

        //    targetPosition.Normalize();

        //    // moving the object
        //    Position += targetPosition * MaxSpeed;

        //    // this ensures that the object does not intersect with any walls.
        //    foreach (GameAgent wall in wallList.Where(ga => ga.Bounds.Intersects(Bounds)))
        //    {
        //        if (Bounds.Bottom > wall.Bounds.Top && Bounds.Top < wall.Bounds.Top)
        //        {
        //            Position.Y -= Position.Y + ((float)Bounds.Height / 2) - wall.Bounds.Top;
        //        }

        //        if (Bounds.Top < wall.Bounds.Bottom && Bounds.Bottom > wall.Bounds.Bottom)
        //        {
        //            Position.Y += (float)wall.Bounds.Bottom - (Position.Y - ((float)Bounds.Height / 2));
        //        }

        //        if (Bounds.Left < wall.Bounds.Right && Bounds.Right > wall.Bounds.Right)
        //        {
        //            Position.X += (float)wall.Bounds.Right - (Position.X - ((float)Bounds.Width / 2));
        //        }

        //        if (Bounds.Right > wall.Bounds.Left && Bounds.Left < wall.Bounds.Left)
        //        {
        //            Position.X -= Position.X + ((float)Bounds.Width / 2) - wall.Bounds.Left;
        //        }
        //    }
        //}

        //public void SetTargetPoint(Vector2 targetPos, List<GameAgent> wallList, int nodeSize)
        //{
        //    PathToTarget = FindPathToTarget(targetPos, wallList, nodeSize);

        //    if (PathToTarget != null)
        //    {
        //        TargetPosition = targetPos;
        //        PlaceBeforeFollow = Position;
        //        TargetIndex = 0;
        //        ////State = Enums.EnemyState.RunningToPoint;
        //    }
        //}

        //#endregion

        //#region Evade State Methods

        //private void EvadeToPath()
        //{
        //    // Change to A*


        //    //if (Vector2.Distance(PlaceBeforeFollow, Position) < 0.5)
        //    //{
        //    //    // pause at the node for a second
        //    //    timeAtCurrentNode = TimeAtEachPoint;
        //    //    //State = Enums.EnemyState.Paused;
        //    //    return;
        //    //}

        //    //Rotation = GetRotationToTarget(PlaceBeforeFollow);
        //    //Position = CalculateRotatedMovement(new Vector2(0, -1), Rotation) * MaxSpeed + Position;
        //}

        //#endregion

        //#region Following State Methods

        //private void FollowPlayer(Player playerObj, List<GameAgent> wallList, int nodeSize)
        //{
        //    if (Vector2.Distance(Position, PlaceBeforeFollow) > MaxFollowRange)
        //    {
        //        ////State = Enums.EnemyState.Evading;
        //        return;
        //    }

        //    PathToTarget = FindPathToTarget(playerObj.Position, wallList, nodeSize);

        //    if (PathToTarget == null || PathToTarget.Count < 1)
        //    {
        //        ////State = Enums.EnemyState.Evading;
        //        return;
        //    }

        //    if (PathToTarget[0].Bounds.Contains(Bounds))
        //    {
        //        PathToTarget.Remove(PathToTarget[0]);
        //    }

        //    Vector2 targetPos = FindBestPathToTargetFromPathList(playerObj.Position);

        //    if (Vector2.Distance(Position, playerObj.Position) > MeleeDistance)
        //    {
        //        Rotation = GetRotationToTarget(targetPos);
        //        Position = Utils.CalculateRotatedMovement(new Vector2(0, -1), Rotation) * MaxSpeed + Position;
        //        return;
        //    }
        //}

        //#endregion

        //#region Paused State Methods

        //private void CheckTimeToNextMove()
        //{
        //    timeAtCurrentNode--;

        //    if (timeAtCurrentNode < 1)
        //    {
        //        // moving to the next path
        //        //State = Enums.EnemyState.Patrolling;
        //    }
        //}

        //#endregion

        //#region Patrolling State Methods

        //private void CheckPathState()
        //{
        //    Vector2 targetPos = PathList[PathIndex];

        //    // we've arrived at the node position
        //    if (Vector2.Distance(targetPos, Position) < 0.5)
        //    {
        //        // pause at the node for a second
        //        timeAtCurrentNode = TimeAtEachPoint;
        //        //State = Enums.EnemyState.Paused;
        //        PathIndex = (PathIndex + 1) % PathList.Count();
        //    }
        //}

        //private void MoveAlongPath()
        //{
        //    Rotation = GetRotationToTarget(PathList[PathIndex]);
        //    Position = Utils.CalculateRotatedMovement(new Vector2(0, -1), Rotation) * MaxSpeed + Position;
        //}

        //#endregion

        //#region A* Pathfinding

        //public List<LevelNode> FindPathToTarget(Vector2 targetPos, List<GameAgent> wallList, int nodeSize)
        //{
        //    // finding which graph node this agent is currently in
        //    int row = (int)Position.Y / nodeSize;
        //    int col = (int)Position.X / nodeSize;

        //    // initializing a* lists
        //    List<LevelNode> closedNodes = new List<LevelNode>();
        //    List<LevelNode> openNodes = new List<LevelNode>();

        //    // creating the graph node the current is currently in
        //    LevelNode _currentNode = new LevelNode()
        //    {
        //        Bounds = new Rectangle(col * nodeSize, row * nodeSize, nodeSize, nodeSize)
        //    };

        //    // initializing the path to the target
        //    LevelNode targetPath;

        //    // creating a point of the target's vector position.
        //    // this is used to see if the target's position is inside a node's rectangle bounds.
        //    Point targetPoint = new Point((int)targetPos.X, (int)targetPos.Y);

        //    // if the current node also contains the target's position
        //    if (_currentNode.Bounds.Contains(targetPoint))
        //    {
        //        targetPath = _currentNode;
        //    }
        //    else 
        //    {
        //        // adding the current node to our open list
        //        openNodes.Add(_currentNode);

        //        // running the A* path finding
        //        targetPath = GetPathToTarget(openNodes, closedNodes, targetPos, targetPoint, nodeSize, wallList);
        //    }

        //    // reinitializing the lists for reuse
        //    // NOTE: Reinitializing instead of clearing since these
        //    // lists have a good chance to be huge.
        //    openNodes = new List<LevelNode>();
        //    closedNodes = new List<LevelNode>();
            
        //    // if we could find a target path
        //    if (targetPath != null)
        //    {
        //        // while we're still not at our first node
        //        while (targetPath.Parent != null)
        //        {
        //            // adding the targetNode to our closed list 
        //            // (which is the path backwards).
        //            closedNodes.Add(targetPath);

        //            // traversing through our linked list
        //            targetPath = targetPath.Parent;
        //        }

        //        // reversing our path node list
        //        for (int i = closedNodes.Count - 1; i > -1; i--)
        //            openNodes.Add(closedNodes[i]);

        //        // clearing the nodes from the list
        //        closedNodes = null;
        //    }

        //    return openNodes;
        //}

        //private LevelNode GetPathToTarget(List<LevelNode> openNodes, List<LevelNode> closedNodes,
        //                                  Vector2 targetPos, Point targetPoint, int nodeSize, 
        //                                  List<GameAgent> wallList)
        //{
        //    LevelNode duplicateNode;
        //    LevelNode currentNode;

        //    while (openNodes.Count > 0)
        //    {
        //        currentNode = openNodes[0];

        //        List<LevelNode> possibleNodes = GetPossibleNodes(currentNode, nodeSize, targetPos, wallList);

        //        foreach(LevelNode node in possibleNodes)
        //        {
        //            if (node.Bounds.Contains(targetPoint))
        //            {
                        
        //                return node;
        //            }

        //            duplicateNode = openNodes.Where(ln => ln.Bounds == node.Bounds).FirstOrDefault();

        //            if (duplicateNode == null)
        //            {
        //                openNodes.Add(node);
        //            }
        //            else if (duplicateNode.F > node.F)
        //            {
        //                duplicateNode.Parent = currentNode;
        //                duplicateNode.F = node.F;
        //            }
        //        }

        //        closedNodes.Add(currentNode);
        //        openNodes.Remove(currentNode);
        //        openNodes = openNodes.OrderBy(ln => ln.F).ToList();
        //    }

        //    return null;
        //}

        //private List<LevelNode> GetPossibleNodes(LevelNode currentNode, int nodeSize, Vector2 targetPos, List<GameAgent> wallList)
        //{
        //    List<LevelNode> possibleNodes = new List<LevelNode>();

        //    for (int i = -1; i < 2; i++)
        //        for (int j = -1; j < 2; j++)
        //            if (!(i == 0 && j == 0))
        //            {
        //                Rectangle bounds = new Rectangle
        //                (
        //                    currentNode.Bounds.Left + (nodeSize * i),
        //                    currentNode.Bounds.Top + (nodeSize * j),
        //                    nodeSize,
        //                    nodeSize
        //                );

        //                if (wallList.Where(ga => ga.Bounds.Intersects(bounds)).Count() <= 0)
        //                {
        //                    possibleNodes.Add(new LevelNode()
        //                    {
        //                        Bounds = bounds,
        //                        Parent = currentNode
        //                    });
        //                }
        //            }

        //    foreach (LevelNode node in possibleNodes)
        //    {
        //        node.CalculateHeuristic(targetPos, currentNode.Bounds.Center, currentNode.G);
        //    }

        //    return possibleNodes;
        //}

        //protected Vector2 FindBestPathToTargetFromPathList(Vector2 targetPos)
        //{
        //    if (PathToTarget.Count < 1)
        //    {
        //        return targetPos;
        //    }

        //    Vector2 nextPos;

        //    if (PathToTarget.Count > 1)
        //    {
        //        Rectangle bounds = PathToTarget[1].Bounds;

        //        if (bounds.Center.Y == Bounds.Center.Y)
        //        {
        //            // next node is to the left
        //            if (bounds.Center.X < Bounds.Center.X)
        //            {
        //                targetPos = new Vector2(bounds.Right, bounds.Center.Y);
        //            }
        //            else // next node is to the right
        //            {
        //                targetPos = new Vector2(bounds.Left, bounds.Center.Y);
        //            }
        //        }
        //        else
        //        {
        //            // next node is above
        //            if (bounds.Center.Y < Bounds.Center.Y)
        //            {
        //                targetPos = new Vector2(bounds.Center.X, bounds.Bottom);
        //            }
        //            else // next node is below
        //            {
        //                targetPos = new Vector2(bounds.Center.X, bounds.Top);
        //            }
        //        }
        //    }
        //    else
        //    {
        //        nextPos = targetPos;
        //    }

        //    Rectangle targetBounds = PathToTarget[0].Bounds;
        //    float halfWidth = (float)Bounds.Width / 2;

        //    if (targetPos.Y == Position.Y)
        //    {
        //        if (targetPos.X < targetBounds.Left + halfWidth)
        //            return new Vector2(targetBounds.Left + halfWidth, targetPos.Y);

        //        if (targetPos.X > targetBounds.Right - halfWidth)
        //            return new Vector2(targetBounds.Right - halfWidth, targetPos.Y);

        //        return targetPos;
        //    }

        //    float slope = (Position.Y - targetPos.Y) / (Position.X - targetPos.X);
        //    float offset = Position.Y - (slope * Position.X);

        //    // next box is below the current position
        //    if (Position.Y < targetBounds.Top)
        //    {
        //        float targetX = (targetBounds.Top - offset) / slope;

        //        if (targetX < targetBounds.Left + halfWidth)
        //            return new Vector2(targetBounds.Left + halfWidth, targetBounds.Top + halfWidth);

        //        if (targetX > targetBounds.Right - halfWidth)
        //            return new Vector2(targetBounds.Right - halfWidth, targetBounds.Top + halfWidth);

        //        return new Vector2(targetX, targetBounds.Top + halfWidth);
        //    }

        //    // next box is above the current position
        //    if (Position.Y > targetBounds.Bottom)
        //    {
        //        float targetX = (targetBounds.Bottom - offset) / slope;

        //        if (targetX < targetBounds.Left + halfWidth)
        //            return new Vector2(targetBounds.Left + halfWidth, targetBounds.Bottom - halfWidth);

        //        if (targetX > targetBounds.Right - halfWidth)
        //            return new Vector2(targetBounds.Right - halfWidth, targetBounds.Bottom - halfWidth);

        //        return new Vector2(targetX, targetBounds.Bottom - halfWidth);
        //    }

        //    // next box is to the right of the current position
        //    if (Position.X < targetBounds.Left)
        //    {
        //        float targetY = slope * targetBounds.Left + offset;

        //        if (targetY < targetBounds.Top + halfWidth)
        //            return new Vector2(targetBounds.Left + halfWidth, targetBounds.Top + halfWidth);

        //        if (targetY > targetBounds.Bottom - halfWidth)
        //            return new Vector2(targetBounds.Left + halfWidth, targetBounds.Bottom - halfWidth);

        //        return new Vector2(targetBounds.Left + halfWidth, targetY);
        //    }

        //    // next box is to the left of the current position
        //    if (Position.X > targetBounds.Right)
        //    {
        //        float targetY = slope * targetBounds.Right + offset;

        //        if (targetY < targetBounds.Top + halfWidth)
        //            return new Vector2(targetBounds.Right - halfWidth, targetBounds.Top + halfWidth);

        //        if (targetY > targetBounds.Bottom - halfWidth)
        //            return new Vector2(targetBounds.Right - halfWidth, targetBounds.Bottom - halfWidth);

        //        return new Vector2(targetBounds.Right - halfWidth, targetY);
        //    }

        //    return Position;
        //}

        //#endregion

        #region Steering Behaviors

        // NOTE: These are all currently unused.

        //public Vector2 Seek(Vector2 targetPos)
        //{
        //    Vector2 desiredVelocity = Vector2.Normalize(targetPos - Position) * MaxSpeed;

        //    return desiredVelocity - Velocity;
        //}

        //public Vector2 Flee(Vector2 targetPos)
        //{
        //    double fleeArea = 100 * 100;

        //    if (Vector2.DistanceSquared(Position, targetPos) > fleeArea)
        //    {
        //        return Vector2.Zero;
        //    }

        //    Vector2 desiredVelocity = Vector2.Normalize(Position - targetPos);

        //    return desiredVelocity - Velocity;
        //}

        //public Vector2 Arrive(Vector2 targetPos, Enums.Deceleration deceleration)
        //{
        //    Vector2 toTarget = targetPos - Position;

        //    float distanceToTarget = toTarget.Length();

        //    // if we're not yet at the target position
        //    if (distanceToTarget > 0)
        //    {
        //        // fine tweaking of the deceleration
        //        double decelerationTweaker = 0.3;

        //        // speed required to reach the target given the desired deceleration
        //        double speed = distanceToTarget / ((double)deceleration * decelerationTweaker);

        //        // make sure velocity does not exceed the max
        //        speed = Math.Min(speed, MaxSpeed);

        //        // proceed to target without normalizing
        //        Vector2 desiredVelocity = toTarget * (float)(speed / distanceToTarget);
        //    }

        //    return Vector2.Zero;
        //}

        //public Vector2 Pursuit(GameAgent evader)
        //{
        //    // if the evader is ahead and facing the agent then we can seek
        //    // for the evader's current position
        //    Vector2 toEvader = evader.Position - Position;

        //    float relativeHeading;
        //    Vector2.Dot(evader.Rotation, Rotation, out relativeHeading);


        //}

        #endregion

        #endregion

        #region Private Helper Methods

        private void CheckIfPlayerInRange(Player playerObj)
        {
            //if (State == Enums.EnemyState.Following || State == Enums.EnemyState.Evading) return;

            if (Vector2.Distance(Position, playerObj.Position) <= NPCRange)
            {
                //State = Enums.EnemyState.Following;
                PlaceBeforeFollow = Position;
            }
        }

        private float GetRotForSteeringForce(Vector2 steeringForce)
        {
            float temp = (float)Math.Atan2(steeringForce.X, -steeringForce.Y);

            while (temp < 0) temp += MathHelper.TwoPi;

            return temp;
        }

        private float GetRotationToTarget(Vector2 targetPos)
        {
            Vector2 dist = targetPos - Position;

            float temp = (float)Math.Atan2(dist.X, -dist.Y);
            while (temp < 0) temp += MathHelper.TwoPi;
            return temp;
        }

        #endregion

        #region Take Damage Methods

        public virtual void TakeDamage(float damage)
        {
            Health -= damage;

            if (Health <= 0)
            {
                Game1.Current.levelInfo.AgentList.Remove(this);
            }
        }

        #endregion
    }
}
