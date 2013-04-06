using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using SampleGame.Effects;
using Microsoft.Xna.Framework.Graphics;
using SampleGame.Helpers;

namespace SampleGame.Attacks
{
    public class Attack
    {
        public int AttackType;
        public int AttackSubType;
        public Keys Key;
        public int CoolDown;        // in miliseconds
        public bool Active = true;
        public Texture2D Texture;
        public Rectangle? BoundingRect = null;
        public int Frames;
        public int MinDamage;
        public int MaxDamage;

        public int ActiveCoolDown = 0;

        Random random = new Random();
        int bulletCount = 0;

        public virtual void Update(GameTime gameTime, KeyboardState keyboard, MouseState mouse)
        {
            if (Active)
            {
                if (ActiveCoolDown > 0)
                {
                    ActiveCoolDown -= gameTime.ElapsedGameTime.Milliseconds;
                }
                else if (keyboard.IsKeyDown(Key) || 
                    (mouse.LeftButton == ButtonState.Pressed && AttackType == (int)Enums.AttackType.Bullet) ||
                    (mouse.RightButton == ButtonState.Pressed && AttackType == (int)Enums.AttackType.Explosion))
                {
                    AddAttackToActiveEffects(Enums.AgentType.Player, Game1.Current.player);
                }
            }
        }

        public void Update(GameTime gameTime)
        {
            if (Active && ActiveCoolDown > 0)
            {
                ActiveCoolDown -= gameTime.ElapsedGameTime.Milliseconds;
            }
        }

        public void UseAttack(Enums.AgentType castedBy, MovingAgent agent)
        {
            AddAttackToActiveEffects(castedBy, agent);
        }

        private void AddAttackToActiveEffects(Enums.AgentType castedBy, MovingAgent agent)
        {
            switch (AttackType)
            {
                case (int)Enums.AttackType.Bullet:

                    switch (AttackSubType)
                    {
                        case (int)Enums.AttackSubType.Default:
                            LoadDefaultBullet(castedBy, agent);
                            break;

                        case (int)Enums.AttackSubType.TriBullet:
                            if (Game1.Current.player.Power > 0)
                            {
                                LoadTriBullet(castedBy, agent);
                                Game1.Current.player.Power--;
                            }
                            break;
                    }

                    break;

                case (int)Enums.AttackType.Explosion:

                    switch (AttackSubType)
                    {
                        case (int)Enums.AttackSubType.Default: LoadDefaultExplosion(castedBy, agent); break;
                    }

                    break;
            }
        }

        private void LoadDefaultBullet(Enums.AgentType castedBy, MovingAgent agent)
        {
            Bullet bullet = new Bullet();
            bullet.LoadEffect(Texture);
            bullet.Position = agent.Position + Utils.CalculateRotatedMovement(new Vector2(0, -(agent.FrameHeight / 2)), agent.Rotation);
            bullet.Rotation = agent.Rotation;
            bullet.MaxSpeed = castedBy == Enums.AgentType.Player ? 8 : 6;
            bullet.Color = Color.White;
            bullet.MinDamage = MinDamage;
            bullet.MaxDamage = MaxDamage;
            bullet.CastedBy = castedBy;

            Game1.Current.EffectComponent.AddEffect(bullet);

            ActiveCoolDown = CoolDown;
        }

        private void LoadTriBullet(Enums.AgentType castedBy, MovingAgent agent)
        {
            int colorChangeInterval = 20; // 100 seems like a good interval - 20 to demo
            Color innerColor;
            Color outerColor;

            if (bulletCount > 6 * colorChangeInterval)
            {
                innerColor = Color.LightBlue;
                outerColor = Color.Blue;
                bulletCount = 0;
            }
            else if (bulletCount > 5 * colorChangeInterval)
            {
                innerColor = Color.LightGreen;
                outerColor = Color.Green;
            }
            else if (bulletCount > 4 * colorChangeInterval)
            {
                innerColor = Color.LightYellow;
                outerColor = Color.Yellow;
            }
            else if (bulletCount > 3 * colorChangeInterval)
            {
                innerColor = Color.LightGoldenrodYellow;
                outerColor = Color.Orange;
            }
            else if (bulletCount > 2 * colorChangeInterval)
            {
                innerColor = Color.Pink;
                outerColor = Color.Red;
            }
            else if (bulletCount > colorChangeInterval)
            {
                innerColor = Color.Pink;
                outerColor = Color.Purple;
            }
            else
            {
                innerColor = Color.LightBlue;
                outerColor = Color.Blue;
            }

            int bulletSpeed = 8;
            float angleModifier = 0.25f; // lower numbers for straight line - higher for wide angle - NOTE: values between 0 and 1 work best

            Bullet bullet = new Bullet();
            bullet.LoadEffect(Texture);
            bullet.Position = agent.Position + Utils.CalculateRotatedMovement(new Vector2(0, -(agent.FrameHeight / 2)), agent.Rotation);
            bullet.Rotation = (agent.Rotation - (0.5f * angleModifier)) + (float)random.NextDouble() * angleModifier;
            bullet.MaxSpeed = bulletSpeed;
            bullet.Color = innerColor;
            bullet.MinDamage = MinDamage;
            bullet.MaxDamage = MaxDamage;
            bullet.CastedBy = castedBy;

            Game1.Current.EffectComponent.AddEffect(bullet);

            Bullet bullet1 = new Bullet();
            bullet1.LoadEffect(Texture);
            bullet1.Position = agent.Position + Utils.CalculateRotatedMovement(new Vector2(0, -(agent.FrameHeight / 2)), agent.Rotation);
            bullet1.Rotation = agent.Rotation - ((float)random.NextDouble() * angleModifier);//0.02f;
            bullet1.MaxSpeed = bulletSpeed;
            bullet1.Color = outerColor;
            bullet1.MinDamage = MinDamage;
            bullet1.MaxDamage = MaxDamage;
            bullet1.CastedBy = castedBy;

            Game1.Current.EffectComponent.AddEffect(bullet1);

            Bullet bullet2 = new Bullet();
            bullet2.LoadEffect(Texture);
            bullet2.Position = agent.Position + Utils.CalculateRotatedMovement(new Vector2(0, -(agent.FrameHeight / 2)), agent.Rotation);
            bullet2.Rotation = agent.Rotation + ((float)random.NextDouble() * angleModifier);
            bullet2.MaxSpeed = bulletSpeed;
            bullet2.Color = outerColor;
            bullet2.MinDamage = MinDamage;
            bullet2.MaxDamage = MaxDamage;
            bullet2.CastedBy = castedBy;

            Game1.Current.EffectComponent.AddEffect(bullet2);

            // change the color set of the bullets
            bulletCount++;
            


            ActiveCoolDown = CoolDown;
        }

        private void LoadDefaultExplosion(Enums.AgentType castedBy, MovingAgent agent)
        {
            if (castedBy == Enums.AgentType.Player)
            {
                MouseState mouseStateCurrent = Mouse.GetState();

                Explosion explosion = new Explosion();
                explosion.LoadExplosion(Texture, BoundingRect, Frames);
                explosion.Position = new Vector2(mouseStateCurrent.X, mouseStateCurrent.Y);
                explosion.AnimationInterval = new TimeSpan(2000000);

                Game1.Current.EffectComponent.AddEffect(explosion);

                ActiveCoolDown = CoolDown;
            }
            else
            {
                Explosion explosion = new Explosion();
                explosion.LoadExplosion(Texture, BoundingRect, Frames);
                explosion.Position = Game1.Current.player.Position;
                explosion.AnimationInterval = new TimeSpan(2000000);

                Game1.Current.EffectComponent.AddEffect(explosion);

                ActiveCoolDown = CoolDown;
            }
        }
    }
}
