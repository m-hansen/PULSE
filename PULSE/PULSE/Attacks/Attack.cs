using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using PulseGame.Effects;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;
using PulseGame.Helpers;
using PulseGame.Agents;

namespace PulseGame.Attacks
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
        public int MaxSequence;
        private int currentSequence;

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
                    && PulseGame.Current.player.Power > AttackCost)
                {
                    AddAttackToActiveEffects(Enums.AgentType.Player, PulseGame.Current.player);
                    PulseGame.Current.player.Power -= AttackCost;
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
                        case Enums.AttackSubType.Default:         LoadDefaultBullet(castedBy, agent);    break;
                        case Enums.AttackSubType.TriBullet:       LoadTriBullet(castedBy, agent);        break;
                        case Enums.AttackSubType.SplitBullets:    LoadSplitBullets(castedBy, agent);     break;
                        case Enums.AttackSubType.Nuke:            LoadNuke(castedBy, agent);             break;
                        case Enums.AttackSubType.BulletShield:    LoadBulletShield(castedBy, agent);     break;
                        case Enums.AttackSubType.DoubleTriBullet: LoadDoubleTriBullet(castedBy, agent);  break;
                    }

                    break;

                case Enums.AttackType.Explosion:

                    switch (AttackSubType)
                    {
                        case Enums.AttackSubType.Default:         LoadDefaultExplosion(castedBy, agent); break;
                        case Enums.AttackSubType.ReflectingStar:  LoadReflectingStar(castedBy, agent);   break;
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
            bullet.HitsRemaining = 1;

            PulseGame.Current.effectComponent.AddEffect(bullet);

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
                bullet.HitsRemaining = 3;
                bullet.EffectSubType = Enums.AttackSubType.Nuke;

                PulseGame.Current.effectComponent.AddEffect(bullet);
            }

            PulseGame.Current.effectComponent.NukeSplit();

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
                bullet.HitsRemaining = 1;

                PulseGame.Current.effectComponent.AddEffect(bullet);
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

        private void LoadDoubleTriBullet(Enums.AgentType castedBy, MovingAgent agent)
        {
            //innerColor = Color.Red;
            //outerColor = Color.Orange;

            int bulletSpeed = 8;
            float angleModifier = 0.25f; // lower numbers for straight line - higher for wide angle - NOTE: values between 0 and 1 work best

            Vector2 playerPos = PulseGame.Current.player.Position;

            for (int i = 0; i < 2; i++)
                for (int j = 0; j < 3; j++)
                {
                    Vector2 pos = new Vector2(agent.Position.X, agent.Position.Y + (i == 0 ? -50 : 50)) + Utils.CalculateRotatedMovement(new Vector2(0, -(agent.FrameHeight / 4)), agent.Rotation);

                    Bullet bullet = new Bullet();
                    bullet.LoadEffect(Texture);
                    bullet.Position = pos;
                    bullet.Rotation = (Utils.GetRotationToTarget(playerPos, pos) - (0.5f * angleModifier)) + (float)random.NextDouble() * angleModifier;
                    bullet.MaxSpeed = bulletSpeed;
                    bullet.Color = innerColor;
                    bullet.MinDamage = MinDamage;
                    bullet.MaxDamage = MaxDamage;
                    bullet.CastedBy = castedBy;
                    bullet.EffectSubType = Enums.AttackSubType.TriBullet;
                    bullet.EffectType = Enums.AttackType.Bullet;
                    bullet.Color = (i % 2 == 0) ? new Color(50, 100, 240) : new Color(245, 50, 55); // light blue / light red
                    bullet.HitsRemaining = 1;

                    PulseGame.Current.effectComponent.AddEffect(bullet);
                }

            currentSequence += AttackCost;

            if (currentSequence > MaxSequence)
            {
                Random rand = new Random();
                ActiveCoolDown = rand.Next(CoolDown);
                currentSequence = 0;
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
            bullet.HitsRemaining = 1;

            PulseGame.Current.effectComponent.AddEffect(bullet);

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
            bullet.HitsRemaining = 1;

            PulseGame.Current.effectComponent.AddEffect(bullet1);

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
            bullet.HitsRemaining = 1;

            PulseGame.Current.effectComponent.AddEffect(bullet2);

            // change the color set of the bullets
            bulletCount++;

            ActiveCoolDown = CoolDown;
        }

        private void LoadSplitBullets(Enums.AgentType castedBy, MovingAgent agent)
        {
            PulseGame.Current.effectComponent.SplitBulletsFromPlayer();

            ActiveCoolDown = CoolDown;
        }

        private void LoadReflectingStar(Enums.AgentType castedBy, MovingAgent agent)
        {
            Random rand  = new Random();

            int padding = 100;
            LevelInfo levelInfo = PulseGame.Current.levelInfo;

            Explosion explosion = new Explosion();
            explosion.LoadExplosion(Texture, BoundingRect, Frames);
            explosion.Position = new Vector2(rand.Next(padding, 3 * padding), rand.Next(padding, levelInfo.Height - padding));
            explosion.AnimationInterval = new TimeSpan(1100000);
            explosion.EffectSubType = Enums.AttackSubType.ReflectingStar;
            explosion.CastedBy = castedBy;

            PulseGame.Current.effectComponent.AddEffect(explosion);

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
                explosion.EffectSubType = Enums.AttackSubType.Default;

                PulseGame.Current.effectComponent.AddEffect(explosion);

                ActiveCoolDown = CoolDown;
            }
            else
            {
                Explosion explosion = new Explosion();
                explosion.LoadExplosion(Texture, BoundingRect, Frames);
                explosion.Position = PulseGame.Current.player.Position;
                explosion.AnimationInterval = new TimeSpan(1100000);
                explosion.EffectSubType = Enums.AttackSubType.Default;

                PulseGame.Current.effectComponent.AddEffect(explosion);

                ActiveCoolDown = CoolDown;
            }
        }

        private void UseTeleport(Enums.AgentType castedBy, MovingAgent agent)
        {
            PulseGame game = PulseGame.Current;

            Vector2 targetPos = new Vector2(game.mouseStateCurrent.X, game.mouseStateCurrent.Y);

            Rectangle targetBounds = new Rectangle
            (
                (int)(targetPos.X - (agent.Bounds.Width / 2)),
                (int)(targetPos.Y - (agent.Bounds.Height / 2)),
                agent.Bounds.Width,
                agent.Bounds.Height
            );

            //if (!game.levelInfo.VisibleRect.Contains(new Point((int)targetPos.X, (int)targetPos.Y)))
            //{
            //    targetPos = FixTargetMovePositionForLevelBounds(targetPos, targetBounds, game.levelInfo.VisibleRect);

            //    targetBounds = new Rectangle
            //    (
            //        (int)(targetPos.X - (agent.Bounds.Width / 2)),
            //        (int)(targetPos.Y - (agent.Bounds.Height / 2)),
            //        agent.Bounds.Width,
            //        agent.Bounds.Height
            //    );
            //}

            if (castedBy == Enums.AgentType.Player)
            {
                List<GameAgent> intersectingAgentList = game.levelInfo.AgentList.Where(ga => ga.Type == (int)Enums.AgentType.Enemy && ga.Bounds.Intersects(targetBounds) && ((Enemy)ga).Health < agent.Health).ToList();

                foreach (GameAgent intersectingAgent in intersectingAgentList)
                {
                    Enemy enemyObj = ((Enemy)intersectingAgent);

                    enemyObj.TakeDamage(enemyObj.Health);
                }
            }

            agent.Position = targetPos;
            agent.Color = Color.Yellow;
            agent.Color = Color.White;

            ActiveCoolDown = CoolDown;
        }

        private Vector2 FixTargetMovePositionForLevelBounds(Vector2 targetPos, Rectangle targetBounds, Rectangle visibleRect)
        {
            int xOffset = 0, yOffset = 0;
            
            if (targetBounds.Left < visibleRect.Left)
                xOffset = visibleRect.Left - targetBounds.Left;

            else if (targetBounds.Right > visibleRect.Right)
                xOffset = visibleRect.Right - targetBounds.Right;

            if (targetBounds.Top < visibleRect.Top)
                yOffset = visibleRect.Top - targetBounds.Top;

            else if (targetBounds.Bottom > visibleRect.Bottom)
                yOffset = visibleRect.Bottom - targetBounds.Bottom;

            targetBounds.Offset(xOffset, yOffset);

            return new Vector2(targetBounds.X, targetBounds.Y);
        }
    }
}
