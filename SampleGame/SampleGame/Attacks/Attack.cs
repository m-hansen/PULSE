using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using SampleGame.Effects;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;
using SampleGame.Helpers;

namespace SampleGame.Attacks
{
    public class Attack
    {
        public Enums.AttackType AttackType;
        public Enums.AttackSubType AttackSubType;
        public Keys Key;
        public int CoolDown;        // in miliseconds
        public bool Active = true;
        public Texture2D Texture;
        public Texture2D IconTexture;
        public bool HasIcon = false;
        public Rectangle? BoundingRect = null;
        public int Frames;
        public int MinDamage;
        public int MaxDamage;
        public int AttackCost;
        public Color Color = Color.White;

        public int ActiveCoolDown = 0;

        Random random = new Random();
        int bulletCount = 0;
        Color innerColor = Color.LightBlue;
        Color outerColor = Color.Blue;

        public virtual void Update(GameTime gameTime, KeyboardState keyboard, MouseState mouse)
        {
            if (Active)
            {
                if (ActiveCoolDown > 0)
                {
                    ActiveCoolDown -= gameTime.ElapsedGameTime.Milliseconds;
                }
                else if ((keyboard.IsKeyDown(Key) ||
                    (mouse.LeftButton == ButtonState.Pressed && AttackSubType == Enums.AttackSubType.TriBullet))
                    && Game1.Current.player.Power > AttackCost)
                {
                    AddAttackToActiveEffects(Enums.AgentType.Player, Game1.Current.player);
                    Game1.Current.player.Power -= AttackCost;
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
                case Enums.AttackType.Bullet:

                    switch (AttackSubType)
                    {
                        case Enums.AttackSubType.Default:       LoadDefaultBullet(castedBy, agent);    break;
                        case Enums.AttackSubType.TriBullet:     LoadTriBullet(castedBy, agent);        break;
                        case Enums.AttackSubType.SplitBullets:  LoadSplitBullets(castedBy, agent);     break;
                        case Enums.AttackSubType.Nuke:          LoadNuke(castedBy, agent);             break;
                        case Enums.AttackSubType.BulletShield:  LoadBulletShield(castedBy, agent);     break;
                    }

                    break;

                case Enums.AttackType.Explosion:

                    switch (AttackSubType)
                    {
                        case Enums.AttackSubType.Default:       LoadDefaultExplosion(castedBy, agent); break;
                    }

                    break;

                case Enums.AttackType.MovementEffect:

                    switch (AttackSubType)
                    {
                        case Enums.AttackSubType.Teleport:      UseTeleport(castedBy, agent);          break;
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
            bullet.Color = Color;
            bullet.MinDamage = MinDamage;
            bullet.MaxDamage = MaxDamage;
            bullet.EffectType = AttackType;
            bullet.CastedBy = castedBy;

            Game1.Current.EffectComponent.AddEffect(bullet);

            ActiveCoolDown = CoolDown;
        }

        private void LoadNuke(Enums.AgentType castedBy, MovingAgent agent)//, Color color)
        {
            int bulletSpeed = 8;
            float angleModifier = 0.25f; // lower numbers for straight line - higher for wide angle - NOTE: values between 0 and 1 work best

            for (int i = 0; i < 1; i++)
            {
                Random rand = new Random();
                Color color = (i % 2 == 0) ? new Color(rand.Next(180, 255), rand.Next(180, 255), rand.Next(180, 255), 255) : new Color(rand.Next(0, 100), rand.Next(0, 100), rand.Next(0, 100), 255);

                Bullet bullet = new Bullet();
                bullet.LoadEffect(Texture);
                bullet.Position = agent.Position + Utils.CalculateRotatedMovement(new Vector2(0, -(agent.FrameHeight / 2)), agent.Rotation);
                bullet.Rotation = (agent.Rotation - (0.5f * angleModifier)) + (float)random.NextDouble() * angleModifier;
                bullet.MaxSpeed = bulletSpeed;
                bullet.Color = color;
                bullet.MinDamage = MinDamage;
                bullet.MaxDamage = MaxDamage;
                bullet.CastedBy = castedBy;
                bullet.EffectSubType = Enums.AttackSubType.Nuke;

                Game1.Current.EffectComponent.AddEffect(bullet);
            }

            Game1.Current.EffectComponent.NukeSplit();

            ActiveCoolDown = CoolDown;
        }

        private void LoadBulletShield(Enums.AgentType castedBy, MovingAgent agent)
        {
            int bulletSpeed = 8;
            float angleModifier = 1.0f; // lower numbers for straight line - higher for wide angle - NOTE: values between 0 and 1 work best

            for (int i = 0; i < 100; i++)
            {
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
            }

            //Game1.Current.EffectComponent.SplitBulletsFromPlayer();

            ActiveCoolDown = CoolDown;
        }

        private List<Color> GetBulletColorListByAgentType(Enums.AgentType castedBy)
        {
            switch (castedBy)
            {
                case Enums.AgentType.Player:

                    return new List<Color>() 
                    {
                        Color.AliceBlue,
                        Color.Purple,
                        Color.LightBlue,
                        Color.Blue,
                        Color.LightGreen,
                        Color.Green,
                        Color.WhiteSmoke,
                        Color.White
                    };

                case Enums.AgentType.Enemy:

                    return new List<Color>()
                    {
                        Color.LightYellow,
                        Color.Yellow,
                        Color.LightGoldenrodYellow,
                        Color.Orange,
                        Color.Pink,
                        Color.Red
                    };

                default:

                    return new List<Color>()
                    {
                        Color.White,
                        Color.White
                    };
            }
        }

        private void LoadTriBullet(Enums.AgentType castedBy, MovingAgent agent)
        {
            List<Color> colorList = GetBulletColorListByAgentType(castedBy);

            int colorChangeInterval = 100; // 100 seems like a good interval - 20 to demo

            if (bulletCount > colorChangeInterval * colorList.Count / 2) bulletCount = 0;

            innerColor = colorList[(2 * (bulletCount / colorChangeInterval)) % colorList.Count];
            outerColor = colorList[(2 * (bulletCount / colorChangeInterval) + 1) % colorList.Count];

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
            bullet.EffectSubType = Enums.AttackSubType.TriBullet;

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
            bullet.EffectSubType = Enums.AttackSubType.TriBullet;

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
            bullet.EffectSubType = Enums.AttackSubType.TriBullet;

            Game1.Current.EffectComponent.AddEffect(bullet2);

            // change the color set of the bullets
            bulletCount++;

            ActiveCoolDown = CoolDown;
        }

        private void LoadSplitBullets(Enums.AgentType castedBy, MovingAgent agent)
        {
            Game1.Current.EffectComponent.SplitBulletsFromPlayer();

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
                explosion.AnimationInterval = new TimeSpan(1100000);
                explosion.EffectType = AttackType;
                explosion.CastedBy = castedBy;

                Game1.Current.EffectComponent.AddEffect(explosion);

                ActiveCoolDown = CoolDown;
            }
            else
            {
                Explosion explosion = new Explosion();
                explosion.LoadExplosion(Texture, BoundingRect, Frames);
                explosion.Position = Game1.Current.player.Position;
                explosion.AnimationInterval = new TimeSpan(1100000);

                Game1.Current.EffectComponent.AddEffect(explosion);

                ActiveCoolDown = CoolDown;
            }
        }

        private void UseTeleport(Enums.AgentType castedBy, MovingAgent agent)
        {
            Game1 game = Game1.Current;

            int teleportDistance = 100;

            Vector2 targetPos = agent.Position + Utils.CalculateRotatedMovement(new Vector2(0, -1), agent.Rotation) * teleportDistance;

            if (!game.levelInfo.VisibleRect.Contains(new Point((int)targetPos.X, (int)targetPos.Y)))
            {
                targetPos = FixTargetMovePositionForLevelBounds(targetPos, agent.Bounds.Width, agent.Bounds.Height, game.levelInfo.VisibleRect);
            }
        }

        private Vector2 FixTargetMovePositionForLevelBounds(Vector2 targetPos, int agentWidth, int agentHeight, Rectangle visibleRect)
        {
            Rectangle targetBounds = new Rectangle
            (
                (int)(targetPos.X - (agentWidth / 2)),
                (int)(targetPos.Y - (agentHeight / 2)),
                agentWidth,
                agentHeight
            );

            if (targetPos.X < visibleRect.Left)
            {

            }
            else if (targetPos.X > visibleRect.Right)
            {

            }

            return targetPos;
        }
    }
}
