using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using PulseGame.Helpers;
using PulseGame.Agents;

namespace PulseGame.Effects
{
    public class Bullet : Effect
    {
        public int MaxSpeed;
        public int MaxDamage;
        public int MinDamage;
        public int TimeElapsed;
        public int SplitsRemaining = 3;
        public int HitsRemaining;

        public override Effect CloneToDirection(float offset, Enums.AttackType type, Enums.AttackSubType subType)
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
            bullet.EffectType = type;
            bullet.EffectSubType = subType;

            return bullet;
        }

        public override void Update(GameTime gameTime, LevelInfo levelInfo)
        {
            if (!Active) return;

            TimeElapsed += gameTime.ElapsedGameTime.Milliseconds;
            if (SplitsRemaining > 0 && EffectSubType == Enums.AttackSubType.Nuke && TimeElapsed > 200)
            {
                PulseGame.Current.effectComponent.NukeSplit();//SplitBulletsFromPlayer();
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
                        if (agent.Type == (int)Enums.AgentType.Enemy)
                        {
                            if (((Enemy)agent).State == Enums.EnemyState.Boss)
                            {
                                if (!Utils.IntersectPixels(agent.Bounds, Utils.TextureToArray(agent.Texture), Bounds, Utils.TextureToArray(Texture)))
                                {
                                    // per pixel collision for boss only
                                    continue;
                                }
                            }

                            ((MovingAgent)agent).TakeDamage(MinDamage + rand.Next(MaxDamage - MinDamage));
                            //if (EffectSubType != Enums.AttackSubType.Nuke && EffectSubType != Enums.AttackSubType.NukeSpawn) Active = false;
                            HitsRemaining--;

                            if (HitsRemaining < 1) Active = false;
                        }
                    }
                }
            }
            else
            {
                Player playerObj = PulseGame.Current.player;

                if (Bounds.Intersects(playerObj.Bounds))
                {
                    playerObj.TakeDamage(MinDamage + (new Random()).Next(MaxDamage - MinDamage));
                    Active = false;
                }
            }
        }
    }
}
