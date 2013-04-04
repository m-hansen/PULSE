using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SampleGame
{
    public class Enums
    {
        public enum AttackType
        {
            Bullet = 0,
            Explosion = 1
        }

        public enum AttackSubType
        {
            Default = 0
        }

        public enum SensorType
        {
            RangeFinder = 0,
            AgentSensor = 1,
            PieSliceSensor = 2
        }

        public enum AgentType
        {
            Wall = 0,
            NPC = 1,
            Patrol = 2
        }

        public enum Deceleration
        {
            Fast = 1,
            Normal = 2,
            Slow = 3,
        }

        public enum MovingAgentState
        {
            Patrolling = 1,
            Following = 2,
            Evading = 3,
            Paused = 4,
            RunningToPoint = 5,
            GoingTowardsDirection = 6
        }
    }
}
