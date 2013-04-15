using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Drawing;
using SampleGame.Helpers;
using System.Diagnostics;
using SampleGame.Attacks;
using SampleGame.Agents;

namespace SampleGame
{
    public class Player : MovingAgent
    {
        public float Speed;  // forward - backward speed
        public bool HasControl = true;
        public List<Attack> attackList = new List<Attack>();
        public float MaxHealth = 100.0f;
        public float MaxPower = 100.0f;
        public float Power;  // used for shooting bullets/skills

        public Player()
        {
            Health = 50.0f;
            Power = 100.0f;
        }

        #region Initialize Sensors Method (UNUSED)

        //public void InitializeSensors()
        //{
        //    SensorList.Add(new RangeFinder()
        //    {
        //        Type = (int)Enums.SensorType.RangeFinder,
        //        Rotation = (float)Math.PI / 6,
        //        Key = Keys.P,
        //        MaxDistance = 50,//150,
        //        Index = 0,
        //        DirectionText = "Right"
        //    });

        //    SensorList.Add(new RangeFinder()
        //    {
        //        Type = (int)Enums.SensorType.RangeFinder,
        //        Rotation = 0,
        //        Key = Keys.P,
        //        MaxDistance = 60,//150,
        //        Index = 1,
        //        DirectionText = "Middle"
        //    });

        //    SensorList.Add(new RangeFinder()
        //    {
        //        Type = (int)Enums.SensorType.RangeFinder,
        //        Rotation = -1 * (float)Math.PI / 6,
        //        Key = Keys.P,
        //        MaxDistance = 50,//150,
        //        Index = 2,
        //        DirectionText = "Left"
        //    });

        //    SensorList.Add(new AdjacentAgentSensor()
        //    {
        //        Type = (int)Enums.SensorType.AgentSensor,
        //        Radius = 150,
        //        Key = Keys.O,
        //    });

        //    SensorList.Add(new PieSliceSensor() // - 60 to 60 degrees
        //    {
        //        Type = (int)Enums.SensorType.PieSliceSensor,
        //        Key = Keys.I,
        //        Rotation1 = -1 * (float)Math.PI / 3,
        //        Rotation2 = (float)Math.PI / 3,
        //        MaxDistance = 150,
        //        DisplayText = "(1,0) - Straight Ahead",
        //        Index = 0
        //    });

        //    SensorList.Add(new PieSliceSensor() // 60 to 120 degrees
        //    {
        //        Type = (int)Enums.SensorType.PieSliceSensor,
        //        Key = Keys.I,
        //        Rotation1 = (float)Math.PI / 3,
        //        Rotation2 = 2 * (float)Math.PI / 3,
        //        MaxDistance = 150,
        //        DisplayText = "(0,1) - Right",
        //        Index = 1
        //    });

        //    SensorList.Add(new PieSliceSensor() // 120 to 240 degrees
        //    {
        //        Type = (int)Enums.SensorType.PieSliceSensor,
        //        Key = Keys.I,
        //        Rotation1 = 2 * (float)Math.PI / 3,
        //        Rotation2 = 4 * (float)Math.PI / 3,
        //        MaxDistance = 150,
        //        DisplayText = "(-1,0) - Backwards",
        //        Index = 2
        //    });

        //    SensorList.Add(new PieSliceSensor() // 240 to 300 degrees
        //    {
        //        Type = (int)Enums.SensorType.PieSliceSensor,
        //        Key = Keys.I,
        //        Rotation1 = 4 * (float)Math.PI / 3,
        //        Rotation2 = 5 * (float)Math.PI / 3,
        //        MaxDistance = 150,
        //        DisplayText = "(0,-1) - Left",
        //        Index = 3
        //    });
        //}

        #endregion

        public void Update(GameTime gameTime, KeyboardState keyboardStateCurrent, KeyboardState keyboardStatePrevious,
                           MouseState mouseStateCurrent, MouseState mouseStatePrevious, List<GameAgent> agentAIList,
                           int levelWidth, int levelHeight)
        {
            if (!HasControl) return;

            float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            Vector2 mouseStateCurrentVec = new Vector2(mouseStateCurrent.X, mouseStateCurrent.Y);

            Rotation = Utils.GetRotationToTarget(mouseStateCurrentVec, Position);

            // rotation
            //if (keyboardStateCurrent.IsKeyDown(Keys.Left) || keyboardStateCurrent.IsKeyDown(Keys.A))
            //    Rotation -= (elapsedTime * RotationSpeed) % MathHelper.TwoPi;
            //if (keyboardStateCurrent.IsKeyDown(Keys.Right) || keyboardStateCurrent.IsKeyDown(Keys.D))
            //    Rotation += (elapsedTime * RotationSpeed) % MathHelper.TwoPi;

            // movement
            if (keyboardStateCurrent.IsKeyDown(Keys.Up) || keyboardStateCurrent.IsKeyDown(Keys.W))
            {
                //Position += new Vector2(0, -1) * Speed;
                Vector2 nextPos = new Vector2(0, -1) * Speed + Position; //Utils.CalculateRotatedMovement(new Vector2(0, -1), Rotation) * Speed + Position;

                if (IsValidMove(nextPos, agentAIList, levelWidth, levelHeight))
                    Position = nextPos;
            }
            if (keyboardStateCurrent.IsKeyDown(Keys.Down) || keyboardStateCurrent.IsKeyDown(Keys.S))
            {
                //Position += new Vector2(0, 1) * Speed;
                Vector2 nextPos = new Vector2(0, 1) * Speed + Position;//Utils.CalculateRotatedMovement(new Vector2(0, 1), Rotation) * Speed + Position;

                if (IsValidMove(nextPos, agentAIList, levelWidth, levelHeight))
                    Position = nextPos;
            }
            if (keyboardStateCurrent.IsKeyDown(Keys.A) || keyboardStateCurrent.IsKeyDown(Keys.Left))
            {
                //Position += new Vector2(-1, 0) * Speed;
                Vector2 nextPos = new Vector2(-1, 0) * Speed + Position; //Utils.CalculateRotatedMovement(new Vector2(-1, 0), Rotation) * Speed + Position;

                if (IsValidMove(nextPos, agentAIList, levelWidth, levelHeight))
                    Position = nextPos;
            }
            if (keyboardStateCurrent.IsKeyDown(Keys.D) || keyboardStateCurrent.IsKeyDown(Keys.Right))
            {
                //Position += new Vector2(1, 0) * Speed;
                Vector2 nextPos = new Vector2(1, 0) * Speed + Position; //Utils.CalculateRotatedMovement(new Vector2(1, 0), Rotation) * Speed + Position;
                
                if (IsValidMove(nextPos, agentAIList, levelWidth, levelHeight))
                    Position = nextPos;
            }

            // the player moved over a npc or wall, the player will take damage equal to that agent
            foreach (GameAgent intersectingAgent in agentAIList.Where(ga => ga.Bounds.Intersects(Bounds)).ToList())
            {
                // if the player ran over an enemy, the enemy will take damage equal to that enemy
                if (intersectingAgent.Type == (int)Enums.AgentType.Enemy)
                {
                    MovingAgent movingAgent = (MovingAgent)intersectingAgent;

                    if (MaxHealth > movingAgent.Health)
                    {
                        TakeDamage(movingAgent.Health);
                        movingAgent.TakeDamage(movingAgent.Health);
                    }
                    else
                    {
                        TakeDamage(movingAgent.Health);
                    }
                }
                else if (intersectingAgent.Type == (int)Enums.AgentType.Wall)
                {
                    TakeDamage(Health);
                }
                else if (intersectingAgent.Type == (int)Enums.AgentType.Item)
                {
                    Item item = (Item)intersectingAgent;
                    item.GetItem();
                    agentAIList.Remove(item);
                }
            }

            foreach (Attack attack in attackList)
            {
                attack.Update(gameTime, keyboardStateCurrent, mouseStateCurrent);
            }

            //foreach (Sensor sensor in SensorList)
            //{
            //    sensor.Update(keyboardStateCurrent, agentAIList, this.Position, this.Rotation);
            //}

            // if the object is active on the screen
            if (Moving)
            {
                // if the image is a sprite sheet 
                // and if enough time has passed to where we need to move to the next frame
                if (TotalFrames > 1 && (animElapsed += gameTime.ElapsedGameTime) > AnimationInterval)
                {
                    if (++currentFrame == TotalFrames)
                        currentFrame = 0;

                    // move back by the animation interval (in miliseconds)
                    animElapsed -= AnimationInterval;
                }
            }

            // power recharge rate
            //Power = MaxPower;
            if (Power < MaxPower) Power += 0.10f;//0.08f;
            else Power = MaxPower;

            // health recharge rate
            if (Health < MaxHealth) Health += 0.02f;
            else Health = MaxHealth;
        }

        private bool IsValidMove(Vector2 nextPos, List<GameAgent> agentAIList, int levelWidth, int levelHeight)
        {
            Rectangle rect = new Rectangle
            (
                (int)(nextPos.X - Origin.X * Scale),
                (int)(nextPos.Y - Origin.Y * Scale),
                (int)(rects == null ? Texture.Width * Scale : rects[0].Width * Scale),
                (int)(rects == null ? Texture.Height * Scale : rects[0].Height * Scale)
            );

            return rect.Top > 0 && rect.Left > 0 && rect.Right < levelWidth && rect.Bottom < levelHeight;

            // COMMENTED OUT: Player can now run into enemies & walls.

            //bool collision = false;

            //foreach (GameAgent agent in agentAIList)
            //{
            //    if (collision = agent.Bounds.Intersects(rect))
            //        break;
            //}

            //return (!collision && rect.Left > 0 && rect.Left + rect.Width < levelWidth && rect.Top > 0 && rect.Top + rect.Height < levelHeight);
        }

        public override void TakeDamage(float damage)
        {
            Game1.Current.PlayerHitSound.Play();
            Health -= damage;

            if (Health <= 0)
            {
                // GAME OVER:
            }
        }

        public override void Draw(SpriteBatch sprites, SpriteFont font1, Rectangle visibleRect)
        {
            Point point = new Point(450, 550);
            int width = 52;
            int height = 52;
            int padding = 1;
            Color cooldownColor = new Color(140, 0, 0, 80);

            foreach (Attack attack in attackList.Where(a => a.HasIcon).ToList())
            {
                // draws the cooldown rect
                if (attack.ActiveCoolDown > 0)
                {
                    Rectangle targetRect = new Rectangle
                    (
                        point.X + padding,
                        point.Y + padding + height - (height * attack.ActiveCoolDown / attack.CoolDown),
                        width - 2 * padding,
                        (height * attack.ActiveCoolDown / attack.CoolDown) - 2 * padding
                    );

                    DrawingHelper.DrawRectangle(targetRect, cooldownColor, true);
                }

                // draws the outline box
                DrawingHelper.DrawRectangle(new Rectangle(point.X, point.Y, width, height), Color.White, false);

                // draws the attack icon texture
                sprites.Draw(attack.IconTexture, new Rectangle(point.X + padding, point.Y + padding, width - 2 * padding, height - 2 * padding), Color.White);

                point.X += width;
            }

            base.Draw(sprites, font1, visibleRect);
        }
    }
}
