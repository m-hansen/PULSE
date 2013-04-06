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

        public void InitializeSensors()
        {
            SensorList.Add(new RangeFinder()
            {
                Type = (int)Enums.SensorType.RangeFinder,
                Rotation = (float)Math.PI / 6,
                Key = Keys.P,
                MaxDistance = 50,//150,
                Index = 0,
                DirectionText = "Right"
            });

            SensorList.Add(new RangeFinder()
            {
                Type = (int)Enums.SensorType.RangeFinder,
                Rotation = 0,
                Key = Keys.P,
                MaxDistance = 60,//150,
                Index = 1,
                DirectionText = "Middle"
            });

            SensorList.Add(new RangeFinder()
            {
                Type = (int)Enums.SensorType.RangeFinder,
                Rotation = -1 * (float)Math.PI / 6,
                Key = Keys.P,
                MaxDistance = 50,//150,
                Index = 2,
                DirectionText = "Left"
            });

            SensorList.Add(new AdjacentAgentSensor()
            {
                Type = (int)Enums.SensorType.AgentSensor,
                Radius = 150,
                Key = Keys.O,
            });

            SensorList.Add(new PieSliceSensor() // - 60 to 60 degrees
            {
                Type = (int)Enums.SensorType.PieSliceSensor,
                Key = Keys.I,
                Rotation1 = -1 * (float)Math.PI / 3,
                Rotation2 = (float)Math.PI / 3,
                MaxDistance = 150,
                DisplayText = "(1,0) - Straight Ahead",
                Index = 0
            });

            SensorList.Add(new PieSliceSensor() // 60 to 120 degrees
            {
                Type = (int)Enums.SensorType.PieSliceSensor,
                Key = Keys.I,
                Rotation1 = (float)Math.PI / 3,
                Rotation2 = 2 * (float)Math.PI / 3,
                MaxDistance = 150,
                DisplayText = "(0,1) - Right",
                Index = 1
            });

            SensorList.Add(new PieSliceSensor() // 120 to 240 degrees
            {
                Type = (int)Enums.SensorType.PieSliceSensor,
                Key = Keys.I,
                Rotation1 = 2 * (float)Math.PI / 3,
                Rotation2 = 4 * (float)Math.PI / 3,
                MaxDistance = 150,
                DisplayText = "(-1,0) - Backwards",
                Index = 2
            });

            SensorList.Add(new PieSliceSensor() // 240 to 300 degrees
            {
                Type = (int)Enums.SensorType.PieSliceSensor,
                Key = Keys.I,
                Rotation1 = 4 * (float)Math.PI / 3,
                Rotation2 = 5 * (float)Math.PI / 3,
                MaxDistance = 150,
                DisplayText = "(0,-1) - Left",
                Index = 3
            });
        }

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
                Vector2 nextPos = new Vector2(0, -1) * Speed + Position; //Utils.CalculateRotatedMovement(new Vector2(0, -1), Rotation) * Speed + Position;

                if (IsValidMove(nextPos, agentAIList, levelWidth, levelHeight))
                    Position = nextPos;
            }
            if (keyboardStateCurrent.IsKeyDown(Keys.Down) || keyboardStateCurrent.IsKeyDown(Keys.S))
            {
                Vector2 nextPos = new Vector2(0, 1) * Speed + Position;//Utils.CalculateRotatedMovement(new Vector2(0, 1), Rotation) * Speed + Position;

                if (IsValidMove(nextPos, agentAIList, levelWidth, levelHeight))
                    Position = nextPos;
            }
            if (keyboardStateCurrent.IsKeyDown(Keys.A) || keyboardStateCurrent.IsKeyDown(Keys.Left))
            {
                Vector2 nextPos = new Vector2(-1, 0) * Speed + Position; //Utils.CalculateRotatedMovement(new Vector2(-1, 0), Rotation) * Speed + Position;

                if (IsValidMove(nextPos, agentAIList, levelWidth, levelHeight))
                    Position = nextPos;
            }
            if (keyboardStateCurrent.IsKeyDown(Keys.D) || keyboardStateCurrent.IsKeyDown(Keys.Right))
            {
                Vector2 nextPos = new Vector2(1, 0) * Speed + Position; //Utils.CalculateRotatedMovement(new Vector2(1, 0), Rotation) * Speed + Position;

                if (IsValidMove(nextPos, agentAIList, levelWidth, levelHeight))
                    Position = nextPos;
            }

            foreach (Attack attack in attackList)
            {
                attack.Update(gameTime, keyboardStateCurrent, mouseStateCurrent);
            }

            foreach (Sensor sensor in SensorList)
            {
                sensor.Update(keyboardStateCurrent, agentAIList, this.Position, this.Rotation);
            }

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

                // move the object by the velocity
                Position += Velocity;
            }

            // power recharge rate
            if (Power < MaxPower) Power += 0.05f;
            else Power = MaxPower;

            // health recharge rate
            if (Health < MaxHealth) Health += 0.01f;
            else Health = MaxHealth;
        }

        private bool IsValidMove(Vector2 nextPos, List<GameAgent> agentAIList, int levelWidth, int levelHeight)
        {
            Rectangle rect = new Rectangle
            (
                (int)(nextPos.X - Origin.X * Scale),
                (int)(nextPos.Y - Origin.Y * Scale),
                FrameWidth,
                FrameHeight
            );

            bool collision = false;

            foreach (GameAgent agent in agentAIList)
            {
                if (collision = agent.Bounds.Intersects(rect))
                    break;
            }

            return (!collision && rect.Left > 0 && rect.Left + rect.Width < levelWidth && rect.Top > 0 && rect.Top + rect.Height < levelHeight);
        }

        public override void Draw(SpriteBatch sprites, SpriteFont font1, Rectangle visibleRect)
        {
            foreach (Sensor sensor in SensorList)
            {
                sensor.Draw(sprites, this.Position, visibleRect, font1);
            }

            // health bar
            sprites.DrawString(font1, "Health", new Vector2(163, 648), Color.White);
            DrawingHelper.DrawRectangle(new Rectangle(225, 650, (int)(MaxHealth * 2), 20), Color.Green, false);
            DrawingHelper.DrawRectangle(new Rectangle(225, 650, (int)(Health * 2), 20), Color.Green, true);

            // power bar
            sprites.DrawString(font1, "Power", new Vector2(688, 648), Color.White);
            DrawingHelper.DrawRectangle(new Rectangle(750, 650, (int)(MaxPower * 2), 20), Color.Purple, false);
            DrawingHelper.DrawRectangle(new Rectangle(750, 650, (int)(Power * 2), 20), Color.Purple, true);

            base.Draw(sprites, font1, visibleRect);
        }
    }
}
