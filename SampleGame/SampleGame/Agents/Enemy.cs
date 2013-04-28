using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SampleGame.Attacks;
using Microsoft.Xna.Framework;
using SampleGame.Helpers;
using SampleGame.Effects;
using Microsoft.Xna.Framework.Graphics;

namespace SampleGame.Agents
{
    public class Enemy : MovingAgent
    {
        #region Constants

        public List<Attack> attackList = new List<Attack>();
        public Enums.EnemyState State;
        public bool IsUsingAttacks;
        public int Wave;
        public int Spacing; // padding outside of the bounds so that agent's aren't grouped too closely
        public int DropType;
        private int WaitTime = 0;

        #endregion

        #region Update Methods

        public override void Update(GameTime gametime, Player playerObj, List<GameAgent> agentList, int nodeSize)
        {
            switch (State)
            {
                case Enums.EnemyState.KamikazeAcrossScreen:    KamikazeAcrossScreenUpdate(gametime);    break;
                case Enums.EnemyState.KamikazeTowardsPlayer:   KamikazeTowardsPlayerUpdate(gametime);   break;
                case Enums.EnemyState.AggressiveCloseToPlayer: AggressiveCloseToPlayerUpdate(gametime); break;
                case Enums.EnemyState.AggressiveCirclePlayer:  AggressiveCirclePlayerUpdate(gametime);  break;
                case Enums.EnemyState.RoamRandom:              RoamRandomUpdate(gametime);              break;
                case Enums.EnemyState.Ranged:                  RangedUpdate(gametime);                  break;  
                case Enums.EnemyState.RangedNoAttack:          RangedNoAttackUpdate(gametime);          break;
                case Enums.EnemyState.Boss:                    BossUpdate(gametime);                    break;
            }

 	        base.Update(gametime, playerObj, agentList, nodeSize);
        }

        #endregion

        #region AI State: Boss Update Method

        private void BossUpdate(GameTime gametime)
        {
            if (WaitTime > 0)
            {
                WaitTime--;
                UseAttack(gametime);
                return;
            }

            Rectangle visibleRect = Game1.Current.levelInfo.VisibleRect;

            if (Vector2.Distance(Position, TargetPosition) < MeleeDistance)
            {
                Random rand = new Random();
                TargetPosition = new Vector2(Position.X, rand.Next(Bounds.Width / 2, visibleRect.Height - (Bounds.Width / 2)));
                WaitTime = rand.Next(0, 20);
            }

            float rot = Utils.GetRotationToTarget(TargetPosition, Position);
            Position += Utils.CalculateRotatedMovement(new Vector2(0, -1), rot) * MaxSpeed;

            UseAttack(gametime);
        }

        #endregion

        #region AI State: Kamikaze Across Screen Methods

        private void KamikazeAcrossScreenUpdate(GameTime gametime)
        {
            // updating rotation & position for moving towards target position
            Rotation = Utils.GetRotationToTarget(TargetPosition, Position);
            Position = GetNextPosition(Utils.CalculateRotatedMovement(new Vector2(0, -1), Rotation) * MaxSpeed + Position);

            // getting access to our global variables
            Game1 game = Game1.Current;

            // if this agent intersects the player
            if (Bounds.Intersects(game.player.Bounds))
            {
                // the player should take damage equal to the health of the enemy
                // and the enemy should explode
                game.player.TakeDamage((int)Health);
                TakeDamage(Health);
            }
            // if the agent is outside of the bounds of the level (with 200px adjusted padding)
            // remove the agent from our enemy list
            else if (!Bounds.Intersects(new Rectangle(-200, -200, game.levelInfo.Width + 400, game.levelInfo.Height + 400)))
            {
                // destroying the enemy
                game.levelInfo.AgentList.Remove(this);
            }
        }

        #endregion

        #region AI State: Kamikaze Towards Player Methods

        private void KamikazeTowardsPlayerUpdate(GameTime gametime)
        {
            Player playerObj = Game1.Current.player;

            // updating rotation & position for moving towards target position
            Rotation = Utils.GetRotationToTarget(playerObj.Position, Position);
            Position = GetNextPosition(Utils.CalculateRotatedMovement(new Vector2(0, -1), Rotation) * MaxSpeed + Position);

            // if this agent intersects the player
            if (Bounds.Intersects(playerObj.Bounds))
            {
                // the player should take damage equal to the health of the enemy
                // and the enemy should explode
                playerObj.TakeDamage((int)Health);
                TakeDamage(Health);
            }
        }

        #endregion

        #region AI State: Aggressive Moving Towards Player Methods

        // NOTE: Random movement needs to be updated
        private void AggressiveCloseToPlayerUpdate(GameTime gametime)
        {
            // getting the player info
            Player playerObj = Game1.Current.player;

            // finding the distance between agent and player
            float distance = Vector2.Distance(playerObj.Position, Position);

            // if the enemy is still too far from the player
            if (distance > MeleeDistance)
            {
                // moving closer to the player
                Rotation = Utils.GetRotationToTarget(playerObj.Position, Position);
                Position += Utils.CalculateRotatedMovement(new Vector2(0, -1), Rotation) * MaxSpeed;
            }
            else
            {
                Rotation = Utils.GetRotationToTarget(playerObj.Position, Position);
                // random movement?
            }

            // if this agent intersects the player
            if (Bounds.Intersects(playerObj.Bounds))
            {
                // the player should take damage equal to the health of the enemy
                // and the enemy should explode
                playerObj.TakeDamage((int)Health);
                TakeDamage(Health);
            }

            if (IsUsingAttacks) UseAttack(gametime);
        }

        #endregion

        #region AI State: Aggressive Circle Player Methods

        // NOTE: this method is currently the same as AggressiveCloseToPlayer();
        private void AggressiveCirclePlayerUpdate(GameTime gametime)
        {
            Player playerObj = Game1.Current.player;

            float distance = Vector2.Distance(playerObj.Position, Position);

            if (distance > MeleeDistance)
            {
                Rotation = Utils.GetRotationToTarget(playerObj.Position, Position);
                Position += Utils.CalculateRotatedMovement(new Vector2(0, -1), Rotation) * MaxSpeed;
            }
            else
            {
                Rotation = Utils.GetRotationToTarget(playerObj.Position, Position);
                // random movement?
            }

            // if this agent intersects the player
            if (Bounds.Intersects(playerObj.Bounds))
            {
                // the player should take damage equal to the health of the enemy
                // and the enemy should explode
                playerObj.TakeDamage((int)Health);
                TakeDamage(Health);
            }

            if (IsUsingAttacks) UseAttack(gametime);
        }

        #endregion

        #region AI State: Roam Random Methods

        private void RoamRandomUpdate(GameTime gametime)
        {
            // NOTE: Could implement a target rotation and rotation speed to make this not
            // turn immediately

            Player playerObj = Game1.Current.player;

            // if the agent is still not close to it's target
            if (Vector2.Distance(TargetPosition, Position) > MeleeDistance)
            {
                float rotToTarget = Utils.GetRotationToTarget(TargetPosition, Position);
                Position += Utils.CalculateRotatedMovement(new Vector2(0, -1), rotToTarget) * MaxSpeed;
                Rotation = Utils.GetRotationToTarget(playerObj.Position, Position);
            }
            else if (timeAtCurrentNode > 0)
            {
                Rotation = Utils.GetRotationToTarget(playerObj.Position, Position);
                timeAtCurrentNode--;
            }
            else
            {
                Rectangle visibleRect = Game1.Current.levelInfo.VisibleRect;

                Random rand = new Random();

                TargetPosition = new Vector2
                (
                       rand.Next(visibleRect.Top + FrameWidth, visibleRect.Right - FrameWidth),
                       rand.Next(visibleRect.Left + FrameHeight, visibleRect.Bottom - FrameHeight)
                );

                float rotToTarget = Utils.GetRotationToTarget(TargetPosition, Position);
                Position += Utils.CalculateRotatedMovement(new Vector2(0, -1), rotToTarget) * MaxSpeed;
                Rotation = Utils.GetRotationToTarget(playerObj.Position, Position);

                timeAtCurrentNode = rand.Next(TimeAtEachPoint);
            }

            // if this agent intersects the player
            if (Bounds.Intersects(playerObj.Bounds))
            {
                // the player should take damage equal to the health of the enemy
                // and the enemy should explode
                playerObj.TakeDamage((int)Health);
                TakeDamage(Health);
            }

            if (IsUsingAttacks) UseAttack(gametime);
        }

        #endregion

        #region AI State: Ranged Methods

        private void RangedUpdate(GameTime gametime)
        {
            if (Vector2.Distance(TargetPosition, Position) > MeleeDistance)
            {
                Rotation = Utils.GetRotationToTarget(TargetPosition, Position);
                Position += Utils.CalculateRotatedMovement(new Vector2(0, -1), Rotation) * MaxSpeed;
            }
            else
            {
                Rotation = Utils.GetRotationToTarget(Game1.Current.player.Position, Position);
            }

            Player playerObj = Game1.Current.player;

            // if this agent intersects the player
            if (Bounds.Intersects(playerObj.Bounds))
            {
                // the player should take damage equal to the health of the enemy
                // and the enemy should explode
                playerObj.TakeDamage((int)Health);
                TakeDamage(Health);
            }

            if (IsUsingAttacks) UseAttack(gametime);
        }

        #endregion

        #region AI State: Ranged No Attack Methods

        private void RangedNoAttackUpdate(GameTime gametime)
        {
            if (Vector2.Distance(TargetPosition, Position) > MeleeDistance)
            {
                Rotation = Utils.GetRotationToTarget(TargetPosition, Position);
                Position += Utils.CalculateRotatedMovement(new Vector2(0, -1), Rotation) * MaxSpeed;
            }
            else
            {
                Rotation = TargetRotation;
            }

            Player playerObj = Game1.Current.player;

            // if this agent intersects the player
            if (Bounds.Intersects(playerObj.Bounds))
            {
                // the player should take damage equal to the health of the enemy
                // and the enemy should explode
                playerObj.TakeDamage((int)Health);
                TakeDamage(Health);
            }
        }

        #endregion

        #region Take Damage Methods

        public override void TakeDamage(float damage)
        {
            // taking the damage
            Health -= damage;

            // if the enemy has no more health
            if (Health <= 0)
            {
                if (DropType > 0)
                {
                    Game1.Current.levelInfo.AgentList.Add(new Item(DropType, Position));
                }

                // creating an explosion effect
                Explosion explosion = new Explosion();
                explosion.LoadExplosion(Game1.Current.Content.Load<Texture2D>("Images\\explosion7"), new Rectangle(0, 0, 40, 40), 11);
                explosion.Position = Position;
                explosion.AnimationInterval = new TimeSpan(1100000);

                // adding the effect to our list
                Game1.Current.EffectComponent.AddEffect(explosion);

                // removing / destroying the enemy
                Game1.Current.levelInfo.AgentList.Remove(this);

                // giving the player points for destroying the enemy
                Game1.Current.player.Score += Score;
            }
        }

        #endregion

        #region Steering Behaviors

        private Vector2 GetNextPosition(Vector2 targetPos)
        {
            return targetPos;

            //Rectangle targetBounds = new Rectangle
            //(
            //    (int)(targetPos.X - Origin.X * Scale - Spacing),
            //    (int)(targetPos.Y - Origin.Y * Scale - Spacing),
            //    (int)(rects == null ? Texture.Width * Scale : rects[0].Width * Scale + 2 * Spacing),
            //    (int)(rects == null ? Texture.Height * Scale : rects[0].Height * Scale + 2 * Spacing)
            //);

            //GameAgent intersectingAgent = Game1.Current.levelInfo.AgentList.Where(ga => ga.Bounds.Intersects(targetBounds) && ga != this).FirstOrDefault();

            //if (intersectingAgent == null) return targetPos;

            //List<GameAgent> agentList = Game1.Current.levelInfo.AgentList;

            //List<TargetVectorHelper> targetVectorHelperList = new List<TargetVectorHelper>()
            //{
            //    new TargetVectorHelper(Enums.Direction.Up, Position, Origin, Scale, Spacing, Bounds.Width, Bounds.Height, MaxSpeed, targetPos, agentList),
            //    new TargetVectorHelper(Enums.Direction.Left, Position, Origin, Scale, Spacing, Bounds.Width, Bounds.Height, MaxSpeed, targetPos, agentList),
            //    new TargetVectorHelper(Enums.Direction.Down, Position, Origin, Scale, Spacing, Bounds.Width, Bounds.Height, MaxSpeed, targetPos, agentList),
            //    new TargetVectorHelper(Enums.Direction.Right, Position, Origin, Scale, Spacing, Bounds.Width, Bounds.Height, MaxSpeed, targetPos, agentList)
            //};

            //targetVectorHelperList = targetVectorHelperList.Where(tvh => !tvh.IsIntersecting).OrderBy(tvh => tvh.TargetDistance).ToList();

            //return targetVectorHelperList.Count < 1 ? Position : targetVectorHelperList[0].TargetPosition;
        }

        #endregion

        #region Use Attack methods

        private void UseAttack(GameTime gametime)
        {
            bool usedAttack = false;

            foreach (Attack attack in attackList.OrderByDescending(a => a.MaxDamage).ToList())
            {
                if (!usedAttack && attack.ActiveCoolDown < 1)
                {
                    attack.UseAttack(Enums.AgentType.Enemy, this);

                   //attack.ActiveCoolDown = attack.CoolDown + (new Random()).Next((int)(attack.CoolDown * 0.2));

                    usedAttack = true;
                }
                else
                {
                    attack.Update(gametime);
                }
            }
        }

        #endregion
    }
}
