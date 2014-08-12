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
            Explosion = 1,
            MovementEffect = 2
        }

        public enum AttackSubType
        {
            Default = 0,
            TriBullet = 1,
            SplitBullets = 2,
            Nuke = 3,
            NukeSpawn = 4,
            BulletShield = 5,
            Teleport = 6,
            DoubleTriBullet = 7,
            ReflectingStar = 8
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
            Enemy = 1,
            Player = 2,
            Item = 3
        }

        public enum Deceleration
        {
            Fast = 1,
            Normal = 2,
            Slow = 3,
        }

        public enum EnemyState
        {
            Patrolling = 1,
            Following = 2,
            Evading = 3,
            Paused = 4,
            RunningToPoint = 5,
            GoingTowardsDirection = 6,
            KamikazeTowardsPlayer = 7,
            KamikazeAcrossScreen = 8,
            AggressiveCloseToPlayer = 9,
            AggressiveCirclePlayer = 10,
            RoamPathFollowing = 11,
            RoamRandom = 12,
            RoamRandomNoAttack = 13,
            Ranged = 14,
            RangedNoAttack = 15,
            Boss = 16
        }

        public enum Direction
        {
            Up = 0,
            Left = 1,
            Down = 2,
            Right = 3
        }

        public enum ItemType
        {
            NukeAttack = 1,
            HealthPowerUp = 2,
            EnergyPowerUp = 3,
            TeleportSpell = 4
        }
    }
}
