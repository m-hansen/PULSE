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
        public int MaxDamage;
        public int MinDamage;
        public int TimeElapsed;
        public int SplitsRemaining = 2;

        public override Effect CloneToDirection(float offset)
        {
            Bullet bullet = new Bullet();
            bullet.LoadEffect(Texture);
            bullet.Position = Position;
            bullet.Rotation = (Rotation + offset) % (float)(2 * Math.PI);
            bullet.MaxSpeed = MaxSpeed;
            bullet.Color = Color;
            bullet.MinDamage = MinDamage;
            bullet.MaxDamage = MaxDamage;
            bullet.CastedBy = CastedBy;

            return bullet;
        }

        public override void Update(GameTime gameTime, LevelInfo levelInfo)
        {
            if (!Active) return;

            TimeElapsed += gameTime.ElapsedGameTime.Milliseconds;
            if (SplitsRemaining > 0 && EffectSubType == Enums.AttackSubType.Nuke && TimeElapsed > 200)
            {
                Game1.Current.EffectComponent.SplitBulletsFromPlayer();
                TimeElapsed = 0;
                SplitsRemaining--;
            }

            Position += Utils.CalculateRotatedMovement(new Vector2(0, -1), Rotation) * MaxSpeed;

            Active = IsInLevelBounds(levelInfo.Width, levelInfo.Height);

            if (CastedBy == Enums.AgentType.Player)
            {
                List<GameAgent> intersectingAgentList = levelInfo.AgentList.Where(ga => Bounds.Intersects(ga.Bounds)).ToList();

                if (intersectingAgentList.Count > 0)
                {
                    Random rand = new Random();

                    foreach (GameAgent agent in intersectingAgentList)
                    {
                        if (agent.Type != (int)Enums.AgentType.Wall)
                        {
                            ((MovingAgent)agent).TakeDamage(MinDamage + rand.Next(MaxDamage - MinDamage));
                            Active = false;
                        }
                    }
                }
            }
            else
            {
                Player playerObj = Game1.Current.player;

                if (Bounds.Intersects(playerObj.Bounds))
                {
                    playerObj.TakeDamage(MinDamage + (new Random()).Next(MaxDamage - MinDamage));
                    Active = false;
                }
            }
        }
    }
}
