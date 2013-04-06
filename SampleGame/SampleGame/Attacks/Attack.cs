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

        public int ActiveCoolDown = 0;

        Random random = new Random();
        int bulletCount = 0;
        int colorChangeInterval = 20; // 100 seems like a good interval - 20 to demo
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
                else if (keyboard.IsKeyDown(Key) || 
                    (mouse.LeftButton == ButtonState.Pressed && AttackType == (int)Enums.AttackType.Bullet) ||
                    (mouse.RightButton == ButtonState.Pressed && AttackType == (int)Enums.AttackType.Explosion))
                {
                    AddAttackToActiveEffects();
                }
            }
        }

        private void AddAttackToActiveEffects()
        {
            switch (AttackType)
            {
                case (int)Enums.AttackType.Bullet:

                    Player playerObj = Game1.Current.player;
                    int bulletSpeed = 8;
                    float angleModifier = 0.25f; // lower numbers for straight line - higher for wide angle - NOTE: values between 0 and 1 work best

                    if (playerObj.Power > 1)
                    {
                        Bullet bullet = new Bullet();
                        bullet.LoadEffect(Texture);
                        bullet.Position = playerObj.Position + Utils.CalculateRotatedMovement(new Vector2(0, -(playerObj.FrameHeight / 2)), playerObj.Rotation);
                        bullet.Rotation = (playerObj.Rotation - (0.5f * angleModifier)) + (float)random.NextDouble() * angleModifier;
                        bullet.MaxSpeed = bulletSpeed;
                        bullet.Color = innerColor;

                        Game1.Current.EffectComponent.AddEffect(bullet);

                        Bullet bullet1 = new Bullet();
                        bullet1.LoadEffect(Texture);
                        bullet1.Position = playerObj.Position + Utils.CalculateRotatedMovement(new Vector2(0, -(playerObj.FrameHeight / 2)), playerObj.Rotation);
                        bullet1.Rotation = playerObj.Rotation - ((float)random.NextDouble() * angleModifier);//0.02f;
                        bullet1.MaxSpeed = bulletSpeed;
                        bullet1.Color = outerColor;

                        Game1.Current.EffectComponent.AddEffect(bullet1);

                        Bullet bullet2 = new Bullet();
                        bullet2.LoadEffect(Texture);
                        bullet2.Position = playerObj.Position + Utils.CalculateRotatedMovement(new Vector2(0, -(playerObj.FrameHeight / 2)), playerObj.Rotation);
                        bullet2.Rotation = playerObj.Rotation + ((float)random.NextDouble() * angleModifier);
                        bullet2.MaxSpeed = bulletSpeed;
                        bullet2.Color = outerColor;

                        Game1.Current.EffectComponent.AddEffect(bullet2);

                        // change the color set of the bullets
                        bulletCount++;
                        if (bulletCount == colorChangeInterval)
                        {
                            innerColor = Color.Pink;
                            outerColor = Color.Purple;
                        }
                        else if (bulletCount == 2 * colorChangeInterval)
                        {
                            innerColor = Color.Pink;
                            outerColor = Color.Red;
                        }
                        else if (bulletCount == 3 * colorChangeInterval)
                        {
                            innerColor = Color.LightGoldenrodYellow;
                            outerColor = Color.Orange;
                        }
                        else if (bulletCount == 4 * colorChangeInterval)
                        {
                            innerColor = Color.LightYellow;
                            outerColor = Color.Yellow;
                        }
                        else if (bulletCount == 5 * colorChangeInterval)
                        {
                            innerColor = Color.LightGreen;
                            outerColor = Color.Green;
                        }
                        else if (bulletCount == 6 * colorChangeInterval)
                        {
                            innerColor = Color.LightBlue;
                            outerColor = Color.Blue;
                            bulletCount = 0;
                        }
                        playerObj.Power--;
                        ActiveCoolDown = CoolDown;
                    }
                    break;

                case (int)Enums.AttackType.Explosion:

                    switch (AttackSubType)
                    {
                        case (int)Enums.AttackSubType.Default:

                            LoadDefaultExplosion();

                            break;


                    }
                    

                    break;
            }
        }

        private void LoadDefaultExplosion()
        {
            Player playerObj = Game1.Current.player;

            MouseState mouseStateCurrent = Mouse.GetState();

            Explosion explosion = new Explosion();
            explosion.LoadExplosion(Texture, BoundingRect, Frames);
            explosion.Position = new Vector2(mouseStateCurrent.X, mouseStateCurrent.Y);
            explosion.AnimationInterval = new TimeSpan(2000000);

            Game1.Current.EffectComponent.AddEffect(explosion);

            ActiveCoolDown = CoolDown;
        }
    }
}
