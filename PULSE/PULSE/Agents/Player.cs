using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Drawing;
using PulseGame.Helpers;
using System.Diagnostics;
using PulseGame.Attacks;
using PulseGame.Agents;

namespace PulseGame
{
    public class Player : MovingAgent
    {
        public float Speed;  // forward - backward speed
        public bool HasControl = true;
        public Attack[] skillList = new Attack[4];
        public float MaxHealth = 100.0f;
        public float MaxPower = 100.0f;
        public float Power;  // used for shooting bullets/skills

        bool berserkerSoundPlayed = false;
        int tempScore = 0;          // used to calculate difference between scores after entering berserker mode
        int berserkerLength = 200;  // the score difference needed to end berserker mode

        public Player()
        {
            Initialize();
        }

        public void Initialize()
        {
            Health = 100.0f;
            Power = 100.0f;
            Color = Color.White;
            Score = 0;
            LoadSkills();
        }

        #region Loading skills
        private void LoadSkills()
        {
            Attack defaultAttack = new Attack();
            defaultAttack.IsUnlocked = true;
            defaultAttack.Active = true;
            defaultAttack.Key = Keys.Space;
            defaultAttack.AttackType = Enums.AttackType.Bullet;
            defaultAttack.AttackSubType = Enums.AttackSubType.TriBullet;
            defaultAttack.CoolDown = 50;
            defaultAttack.Texture = PulseGame.Current.Content.Load<Texture2D>("Images\\raindrop");
            defaultAttack.Frames = 1;
            defaultAttack.MinDamage = 5;
            defaultAttack.MaxDamage = 10;
            defaultAttack.AttackCost = 0;
            skillList[(int)Enums.PlayerSkills.Default] = defaultAttack;

            Attack bulletShieldSkill = new Attack();
            bulletShieldSkill.IsUnlocked = true;
            bulletShieldSkill.Active = true;
            bulletShieldSkill.Key = Keys.D1;
            bulletShieldSkill.AttackType = Enums.AttackType.Bullet;
            bulletShieldSkill.AttackSubType = Enums.AttackSubType.BulletShield;
            bulletShieldSkill.CoolDown = 1500;
            bulletShieldSkill.Texture = PulseGame.Current.Content.Load<Texture2D>("Images\\raindrop");
            bulletShieldSkill.IconTexture = PulseGame.Current.Content.Load<Texture2D>("Images\\skill1");
            bulletShieldSkill.HasIcon = true;
            bulletShieldSkill.AttackCost = 30;
            bulletShieldSkill.MinDamage = 10;
            bulletShieldSkill.MaxDamage = 200;
            skillList[(int)Enums.PlayerSkills.BulletShield] = bulletShieldSkill;

            Attack teleportSkill = new Attack();
            teleportSkill.IsUnlocked = false;
            teleportSkill.Active = true;
            teleportSkill.Key = Keys.D2;
            teleportSkill.AttackType = Enums.AttackType.MovementEffect;
            teleportSkill.AttackSubType = Enums.AttackSubType.Teleport;
            teleportSkill.CoolDown = 15000;
            teleportSkill.Texture = PulseGame.Current.Content.Load<Texture2D>("Images\\raindrop");
            teleportSkill.IconTexture = PulseGame.Current.Content.Load<Texture2D>("Images\\teleport_image");
            teleportSkill.HasIcon = true;
            teleportSkill.AttackCost = 20;
            teleportSkill.MinDamage = 1;
            teleportSkill.MaxDamage = 5;
            skillList[(int)Enums.PlayerSkills.Teleport] = teleportSkill;

            Attack nukeSkill = new Attack();
            nukeSkill.IsUnlocked = false;
            nukeSkill.Active = true;
            nukeSkill.Key = Keys.D3;
            nukeSkill.AttackType = Enums.AttackType.Bullet;
            nukeSkill.AttackSubType = Enums.AttackSubType.Nuke;
            nukeSkill.CoolDown = 3000;
            nukeSkill.Texture = PulseGame.Current.Content.Load<Texture2D>("Images\\raindrop");
            nukeSkill.IconTexture = PulseGame.Current.Content.Load<Texture2D>("Images\\skill4");
            nukeSkill.HasIcon = true;
            nukeSkill.AttackCost = 60;
            nukeSkill.MinDamage = 50;
            nukeSkill.MaxDamage = 200;
            skillList[(int)Enums.PlayerSkills.Nuke] = nukeSkill;
        }
        #endregion

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

            foreach (Attack attack in skillList)
            {
                if (attack.IsUnlocked)
                {
                    attack.Update(gameTime, keyboardStateCurrent, mouseStateCurrent);
                }
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
            }

            // power recharge rate
            if (Power < MaxPower) Power += 0.10f;//0.08f;
            else Power = MaxPower;

            // health recharge rate
            if (Health < 0) Health = 0;
            else if (Health < MaxHealth) Health += 0.02f;
            else Health = MaxHealth;

            // go into berserker mode every 1000 points
            if (Score % 1000 == 0 && Score != 0)
            {
                skillList[(int)Enums.PlayerSkills.Default].CoolDown = 0;
                skillList[(int)Enums.PlayerSkills.Default].MinDamage = 10;
                skillList[(int)Enums.PlayerSkills.Default].MaxDamage = 20;
                Color = Color.Red;
                tempScore = Score;
                if (!berserkerSoundPlayed)
                {
                    PulseGame.Current.berserkerSound.Play();
                    berserkerSoundPlayed = true;
                }
            }
            if ((berserkerSoundPlayed) && ((Score - tempScore) >= berserkerLength))
            {
                skillList[(int)Enums.PlayerSkills.Default].CoolDown = 50;
                skillList[(int)Enums.PlayerSkills.Default].MinDamage = 5;
                skillList[(int)Enums.PlayerSkills.Default].MaxDamage = 10;
                Color = Color.White;
                berserkerSoundPlayed = false;
            }
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
        }

        public override void TakeDamage(float damage)
        {
            //Game1.Current.PlayerHitSound.Play();
            //Health -= damage;
        }

        public override void Draw(SpriteBatch sprites, SpriteFont font1, Rectangle visibleRect)
        {
            //if (PulseGame.Current.gameState != (int)Enums.GameState.Attract)
            //{
            //    Point point = new Point(450, 550);
            //    int width = 52;
            //    int height = 52;
            //    int padding = 1;
            //    Color cooldownColor = new Color(140, 0, 0, 80);

            //    foreach (Attack attack in skillList.Where(a => a.HasIcon).ToList())
            //    {
            //        // draws the cooldown rect
            //        if (attack.ActiveCoolDown > 0)
            //        {
            //            Rectangle targetRect = new Rectangle
            //            (
            //                point.X + padding,
            //                point.Y + padding + height - (height * attack.ActiveCoolDown / attack.CoolDown),
            //                width - 2 * padding,
            //                (height * attack.ActiveCoolDown / attack.CoolDown) - 2 * padding
            //            );

            //            DrawingHelper.DrawRectangle(targetRect, cooldownColor, true);
            //        }

            //        // draws the outline box
            //        DrawingHelper.DrawRectangle(new Rectangle(point.X, point.Y, width, height), Color.Black, false);

            //        // draws the attack icon texture
            //        sprites.Draw(attack.IconTexture, new Rectangle(point.X + padding, point.Y + padding, width - 2 * padding, height - 2 * padding), Color.White);

            //        point.X += width;
            //    }

                base.Draw(sprites, font1, visibleRect);
            //}
        }
    }
}
