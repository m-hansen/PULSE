using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SampleGame.Agents;
using SampleGame.Attacks;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using SampleGame.Helpers;

namespace SampleGame
{
    public class LevelInfo
    {
        public int Width;
        public int Height;
        public int Level;
        public string Name;
        public List<GameAgent> AgentList = new List<GameAgent>();
        public Rectangle VisibleRect;
        public Vector2 PlayerStartPos;
        public int LevelNodeSize;
        public double TimeAllocated;
        private int songPhase = 1;
        private int nextID = 1;
        private int kamikazeWeight = 30;
        private int rangedWeight = 30;
        private int randomWeight = 30;
        private int aggressiveWeight = 10;
        private List<LevelTimeSpan> levelTimeSpanList = new List<LevelTimeSpan>();
        private Random rand = new Random();
        private long lastSpawnTime = 0;
        private long lastPowerUp = 0;
        private long teleportDropTime;
        private long nukeDropTime;
        private int powerUpInterval = 5000;
        //private int lastType;

        public LevelInfo()
        {
            levelTimeSpanList.Add(new LevelTimeSpan(0, 1000, 0));
            // swap between 0 and 1 difficulty for the song intro
            for (int i = 1000; i < 46000; i += 1999)
                levelTimeSpanList.Add(new LevelTimeSpan(i, i+1999, ((i % 2 == 0) ? 1 : 0)));
            levelTimeSpanList.Add(new LevelTimeSpan(46000, 57000, 2));
            levelTimeSpanList.Add(new LevelTimeSpan(57000, 66000, 3));
            levelTimeSpanList.Add(new LevelTimeSpan(66000, 101000, 2));
            levelTimeSpanList.Add(new LevelTimeSpan(101000, 119000, 3));
            levelTimeSpanList.Add(new LevelTimeSpan(119000, 131000, 4));
            levelTimeSpanList.Add(new LevelTimeSpan(131000, 140000, 3));
            levelTimeSpanList.Add(new LevelTimeSpan(140000, 157000, 4));
            levelTimeSpanList.Add(new LevelTimeSpan(157000, 166000, 3));
            levelTimeSpanList.Add(new LevelTimeSpan(166000, 184000, 4));
            levelTimeSpanList.Add(new LevelTimeSpan(184000, 201000, 5));
            levelTimeSpanList.Add(new LevelTimeSpan(201000, 210000, 4));
            levelTimeSpanList.Add(new LevelTimeSpan(210000, 229000, 5));
            levelTimeSpanList.Add(new LevelTimeSpan(229000, 247000, 4));
            levelTimeSpanList.Add(new LevelTimeSpan(247000, 264000, 3));
            levelTimeSpanList.Add(new LevelTimeSpan(264000, 274000, 1));
            levelTimeSpanList.Add(new LevelTimeSpan(274000, 279000, 0));

            nukeDropTime = rand.Next(30000, 45000);
            teleportDropTime = rand.Next(0, 20000);
        }

        #region Get Visible Area

        public void SetVisibleArea(Vector2 playerPos, int windowWidth, int windowHeight)
        {
            //VisibleRect = new Rectangle(
            //    GetVisibleRectX(playerPos, windowWidth, windowHeight),
            //    GetVisibleRectY(playerPos, windowWidth, windowHeight),
            //    windowWidth,
            //    windowHeight
            //);

            VisibleRect = new Rectangle(
                0,
                0,
                windowWidth,
                windowHeight
            );
        }

        private int GetVisibleRectX(Vector2 playerPos, int windowWidth, int windowHeight)
        {
            // if the player is close to the left bounds of the level
            if (playerPos.X < windowWidth / 2)
                return 0;

            // if the player is close to the right bounds of the level
            if (playerPos.X > (Width - windowWidth / 2))
                return Width - windowWidth;

            // otherwise the player is not close to any horizontal level bounds
            return (int)(playerPos.X - windowWidth / 2);
        }

        private int GetVisibleRectY(Vector2 playerPos, int windowWidth, int windowHeight)
        {
            // if the player is close to the top level bounds
            if (playerPos.Y < windowHeight / 2)
                return 0;

            // if the player is close to the bottom level bounds
            if (playerPos.Y > (Height - windowHeight / 2))
                return Height - windowHeight;

            // player is not close to any vertical level bounds
            return (int)(playerPos.Y - windowHeight / 2);
        }

        #endregion

        #region Load Enemy Type

        private void LoadEnemyKamikazeTowardsPlayer()
        {
            int minHealth = 10;
            int maxHealth = 20;

            TextureInfo ti = new TextureInfo("kamikaze1");

            Enemy pAgent1 = new Enemy();
            pAgent1.LoadContent(Game1.Current.Content, ti.TextureString, ti.TextureRect, ti.Frames);
            pAgent1.Position = GetRandomStartPos();// new Vector2(400, 800);//1150, 2650);
            pAgent1.Rotation = 0.0f;
            pAgent1.Type = (int)Enums.AgentType.Enemy;
            pAgent1.State = Enums.EnemyState.KamikazeTowardsPlayer;
            pAgent1.TargetPosition = new Vector2(400, 800);
            pAgent1.Health = rand.Next(minHealth, maxHealth);
            pAgent1.MaxSpeed = 5;
            pAgent1.ID = nextID;

            nextID++;

            AgentList.Add(pAgent1);
        }

        private void LoadEnemyKamikazeTowardsTarget()
        {
            Enemy pAgent1 = new Enemy();
            pAgent1.LoadContent(Game1.Current.Content, "Images\\enemy_agent1", new Rectangle(0, 0, 26, 29), 4);
            pAgent1.Position = new Vector2(400, 800);//1150, 2650);
            pAgent1.Rotation = 0.0f;
            pAgent1.Type = (int)Enums.AgentType.Enemy;
            pAgent1.State = Enums.EnemyState.KamikazeAcrossScreen;
            pAgent1.TargetPosition = new Vector2(400, -100);
            pAgent1.Health = 40;
            pAgent1.MaxSpeed = 5;

            AgentList.Add(pAgent1);
        }

        private void LoadEnemyAggressiveCloseToPlayer()
        {
            Enemy pAgent1 = new Enemy();
            pAgent1.LoadContent(Game1.Current.Content, "Images\\enemy_agent1", new Rectangle(0, 0, 26, 29), 4);
            pAgent1.Position = new Vector2(400, 800);//1150, 2650);
            pAgent1.Rotation = 0.0f;
            pAgent1.Type = (int)Enums.AgentType.Enemy;
            pAgent1.State = Enums.EnemyState.AggressiveCloseToPlayer;
            pAgent1.MeleeDistance = 200;
            pAgent1.Health = 15;
            pAgent1.MaxSpeed = 3;

            Attack attack1 = new Attack();
            attack1.Active = true;
            attack1.AttackType = (int)Enums.AttackType.Bullet;
            attack1.AttackSubType = (int)Enums.AttackSubType.Default;
            attack1.CoolDown = 1000;
            attack1.Texture = Game1.Current.Content.Load<Texture2D>("Images\\raindrop");
            attack1.Frames = 1;
            attack1.MinDamage = 10;
            attack1.MaxDamage = 15;
            pAgent1.attackList.Add(attack1);

            AgentList.Add(pAgent1);
        }

        private void LoadEnemyRoamRandomNoAttack()
        {
            Enemy pAgent1 = new Enemy();
            pAgent1.LoadContent(Game1.Current.Content, "Images\\enemy_agent1", new Rectangle(0, 0, 26, 29), 4);
            pAgent1.Position = new Vector2(400, 800);//1150, 2650);
            pAgent1.TargetPosition = new Vector2(400, 600);
            pAgent1.Rotation = 0.0f;
            pAgent1.Type = (int)Enums.AgentType.Enemy;
            pAgent1.State = Enums.EnemyState.RoamRandomNoAttack;
            pAgent1.MeleeDistance = 40;
            pAgent1.Health = 15;
            pAgent1.MaxSpeed = 3;

            AgentList.Add(pAgent1);
        }

        private void LoadEnemyRanged()
        {
            TextureInfo ti = new TextureInfo("enemy_agent1");

            Enemy pAgent1 = new Enemy();
            pAgent1.LoadContent(Game1.Current.Content, ti.TextureString, ti.TextureRect, ti.Frames);
            pAgent1.Position = GetRandomStartPos();//1150, 2650);
            pAgent1.TargetPosition = GetRandomEndPos(pAgent1.Position);//1150, 2650);
            pAgent1.Rotation = 0.0f;
            pAgent1.Type = (int)Enums.AgentType.Enemy;
            pAgent1.State = Enums.EnemyState.Ranged;
            pAgent1.MeleeDistance = 2;
            pAgent1.Health = 15;
            pAgent1.MaxSpeed = 2;
            pAgent1.IsUsingAttacks = true;
            pAgent1.ID = nextID;

            nextID++;

            Attack attackObj = new Attack();
            attackObj.Active = true;
            attackObj.AttackType = (int)Enums.AttackType.Bullet;
            attackObj.AttackSubType = (int)Enums.AttackSubType.Default;
            attackObj.CoolDown = 3000;
            attackObj.ActiveCoolDown = 1000;
            attackObj.Texture = Game1.Current.Content.Load<Texture2D>("Images\\raindrop");
            attackObj.Frames = 1;
            attackObj.MinDamage = 10;
            attackObj.MaxDamage = 15;
            attackObj.Color = Color.Red;
            pAgent1.attackList.Add(attackObj);

            AgentList.Add(pAgent1);
        }

        private void LoadEnemyRanged(Vector2 startPos, Vector2 targetPos, float targetRotation)
        {
            Enemy pAgent1 = new Enemy();
            pAgent1.LoadContent(Game1.Current.Content, "Images\\enemy_agent1", new Rectangle(0, 0, 26, 29), 4);
            pAgent1.Position = startPos;//1150, 2650);
            pAgent1.TargetPosition = targetPos;//1150, 2650);
            pAgent1.Rotation = targetRotation;
            pAgent1.Type = (int)Enums.AgentType.Enemy;
            pAgent1.State = Enums.EnemyState.RangedNoAttack;
            pAgent1.MeleeDistance = 2;
            pAgent1.Health = 15;
            pAgent1.MaxSpeed = 2;
            pAgent1.TargetRotation = targetRotation;
            pAgent1.IsUsingAttacks = false;

            //Attack attack1 = new Attack();
            //attack1.Active = true;
            //attack1.AttackType = (int)Enums.AttackType.Bullet;
            //attack1.AttackSubType = (int)Enums.AttackSubType.Default;
            //attack1.CoolDown = 1000;
            //attack1.Texture = Game1.Current.Content.Load<Texture2D>("Images\\raindrop");
            //attack1.Frames = 1;
            //attack1.MinDamage = 10;
            //attack1.MaxDamage = 15;
            //pAgent1.attackList.Add(attack1);

            AgentList.Add(pAgent1);
        }

        private void LoadEnemyRangedNoAttack(TextureInfo textureInfo,
                                             Vector2 startPos, 
                                             Vector2 targetPos, 
                                             float targetRotation,
                                             int health,
                                             int maxSpeed,
                                             int wave)
                                             //List<Attack> attackList)
        {
            Enemy pAgent1 = new Enemy();
            pAgent1.LoadContent(Game1.Current.Content, textureInfo.TextureString, textureInfo.TextureRect, textureInfo.Frames);
            pAgent1.Position = startPos;//1150, 2650);
            pAgent1.TargetPosition = targetPos;//1150, 2650);
            pAgent1.Rotation = targetRotation;
            pAgent1.Type = (int)Enums.AgentType.Enemy;
            pAgent1.State = Enums.EnemyState.RangedNoAttack;
            pAgent1.MeleeDistance = 2;
            pAgent1.Health = health;
            pAgent1.MaxSpeed = maxSpeed;
            pAgent1.TargetRotation = targetRotation;
            pAgent1.IsUsingAttacks = false; // attackList != null && attackList.Count > 0;
            pAgent1.Wave = wave;
            pAgent1.ID = nextID;

            nextID++;

            //Attack attack1 = new Attack();
            //attack1.Active = true;
            //attack1.AttackType = (int)Enums.AttackType.Bullet;
            //attack1.AttackSubType = (int)Enums.AttackSubType.Default;
            //attack1.CoolDown = 1000;
            //attack1.Texture = Game1.Current.Content.Load<Texture2D>("Images\\raindrop");
            //attack1.Frames = 1;
            //attack1.MinDamage = 10;
            //attack1.MaxDamage = 15;
            //pAgent1.attackList.Add(attack1);

            AgentList.Add(pAgent1);
        }

        private void LoadEnemyRoamRandom()
        {
            TextureInfo ti = new TextureInfo("follower1");

            int maxHealth = 15;
            int minHealth = 5;

            Enemy pAgent1 = new Enemy();
            pAgent1.LoadContent(Game1.Current.Content, ti.TextureString, ti.TextureRect, ti.Frames);
            pAgent1.Position = GetRandomStartPos();//1150, 2650);
            pAgent1.TargetPosition = GetRandomEndPos(pAgent1.Position);//1150, 2650);
            pAgent1.Rotation = 0.0f;
            pAgent1.Type = (int)Enums.AgentType.Enemy;
            pAgent1.State = Enums.EnemyState.RoamRandom;
            pAgent1.MeleeDistance = 2;
            pAgent1.Health = rand.Next(minHealth, maxHealth);
            pAgent1.MaxSpeed = 3;
            pAgent1.IsUsingAttacks = true; // attackList != null && attackList.Count > 0;
            pAgent1.ID = nextID;

            nextID++;

            Attack attackObj = new Attack();
            attackObj.Active = true;
            attackObj.AttackType = (int)Enums.AttackType.Bullet;
            attackObj.AttackSubType = (int)Enums.AttackSubType.Default;
            attackObj.CoolDown = 3000;
            attackObj.ActiveCoolDown = 1000;
            attackObj.Texture = Game1.Current.Content.Load<Texture2D>("Images\\raindrop");
            attackObj.Frames = 1;
            attackObj.MinDamage = 10;
            attackObj.MaxDamage = 15;
            attackObj.Color = Color.Red;
            pAgent1.attackList.Add(attackObj);

            AgentList.Add(pAgent1);
        }

        #endregion

        #region Load Attack

        #endregion

        #region Load Level

        public void LoadLevel(int level, ContentManager content, int windowWidth, int windowHeight)
        {
            switch (level)
            {
                case 0: LoadLevel0(content, windowWidth, windowHeight); break;
            }
        }

        private void LoadLevel0(ContentManager content, int windowWidth, int windowHeight)
        {
            Level = 0;
            Width = 1200;//2000;
            Height = 700;//3200;
            Name = "Level 1";
            LevelNodeSize = 50;
            TimeAllocated = 280; // in seconds (4:40)

            //SetVisibleArea(new Vector2(), Width, Height);

            //PlayerStartPos = new Vector2(1850, 3000);
            PlayerStartPos = new Vector2(600, 350);

            //// inside left
            //LoadEnemyRanged(new Vector2(350, 150), new Vector2(350, 150), (float)(Math.PI / 2));
            //LoadEnemyRanged(new Vector2(350, 200), new Vector2(350, 200), (float)(Math.PI / 2));
            //LoadEnemyRanged(new Vector2(350, 250), new Vector2(350, 250), (float)(Math.PI / 2));
            //LoadEnemyRanged(new Vector2(350, 300), new Vector2(350, 300), (float)(Math.PI / 2));
            //LoadEnemyRanged(new Vector2(350, 350), new Vector2(350, 350), (float)(Math.PI / 2));
            //LoadEnemyRanged(new Vector2(350, 400), new Vector2(350, 400), (float)(Math.PI / 2));
            //LoadEnemyRanged(new Vector2(350, 450), new Vector2(350, 450), (float)(Math.PI / 2));
            //LoadEnemyRanged(new Vector2(350, 500), new Vector2(350, 500), (float)(Math.PI / 2));
            //LoadEnemyRanged(new Vector2(350, 550), new Vector2(350, 550), (float)(Math.PI / 2));

            //// mid left
            //LoadEnemyRanged(new Vector2(300, 175), new Vector2(300, 175), (float)(Math.PI / 2));
            //LoadEnemyRanged(new Vector2(300, 225), new Vector2(300, 225), (float)(Math.PI / 2));
            //LoadEnemyRanged(new Vector2(300, 275), new Vector2(300, 275), (float)(Math.PI / 2));
            //LoadEnemyRanged(new Vector2(300, 325), new Vector2(300, 325), (float)(Math.PI / 2));
            //LoadEnemyRanged(new Vector2(300, 375), new Vector2(300, 375), (float)(Math.PI / 2));
            //LoadEnemyRanged(new Vector2(300, 425), new Vector2(300, 425), (float)(Math.PI / 2));
            //LoadEnemyRanged(new Vector2(300, 475), new Vector2(300, 475), (float)(Math.PI / 2));
            //LoadEnemyRanged(new Vector2(300, 525), new Vector2(300, 525), (float)(Math.PI / 2));

            //// outside left
            //LoadEnemyRanged(new Vector2(250, 150), new Vector2(250, 150), (float)(Math.PI / 2));
            //LoadEnemyRanged(new Vector2(250, 200), new Vector2(250, 200), (float)(Math.PI / 2));
            //LoadEnemyRanged(new Vector2(250, 250), new Vector2(250, 250), (float)(Math.PI / 2));
            //LoadEnemyRanged(new Vector2(250, 300), new Vector2(250, 300), (float)(Math.PI / 2));
            //LoadEnemyRanged(new Vector2(250, 350), new Vector2(250, 350), (float)(Math.PI / 2));
            //LoadEnemyRanged(new Vector2(250, 400), new Vector2(250, 400), (float)(Math.PI / 2));
            //LoadEnemyRanged(new Vector2(250, 450), new Vector2(250, 450), (float)(Math.PI / 2));
            //LoadEnemyRanged(new Vector2(250, 500), new Vector2(250, 500), (float)(Math.PI / 2));
            //LoadEnemyRanged(new Vector2(250, 550), new Vector2(250, 550), (float)(Math.PI / 2));

            //// inside right
            //LoadEnemyRanged(new Vector2(750, 150), new Vector2(750, 150), (float)(3 * Math.PI / 2));
            //LoadEnemyRanged(new Vector2(750, 200), new Vector2(750, 200), (float)(3 * Math.PI / 2));
            //LoadEnemyRanged(new Vector2(750, 250), new Vector2(750, 250), (float)(3 * Math.PI / 2));
            //LoadEnemyRanged(new Vector2(750, 300), new Vector2(750, 300), (float)(3 * Math.PI / 2));
            //LoadEnemyRanged(new Vector2(750, 350), new Vector2(750, 350), (float)(3 * Math.PI / 2));
            //LoadEnemyRanged(new Vector2(750, 400), new Vector2(750, 400), (float)(3 * Math.PI / 2));
            //LoadEnemyRanged(new Vector2(750, 450), new Vector2(750, 450), (float)(3 * Math.PI / 2));
            //LoadEnemyRanged(new Vector2(750, 500), new Vector2(750, 500), (float)(3 * Math.PI / 2));
            //LoadEnemyRanged(new Vector2(750, 550), new Vector2(750, 550), (float)(3 * Math.PI / 2));

            //// mid right
            //LoadEnemyRanged(new Vector2(800, 175), new Vector2(800, 175), (float)(3 * Math.PI / 2));
            //LoadEnemyRanged(new Vector2(800, 225), new Vector2(800, 225), (float)(3 * Math.PI / 2));
            //LoadEnemyRanged(new Vector2(800, 275), new Vector2(800, 275), (float)(3 * Math.PI / 2));
            //LoadEnemyRanged(new Vector2(800, 325), new Vector2(800, 325), (float)(3 * Math.PI / 2));
            //LoadEnemyRanged(new Vector2(800, 375), new Vector2(800, 375), (float)(3 * Math.PI / 2));
            //LoadEnemyRanged(new Vector2(800, 425), new Vector2(800, 425), (float)(3 * Math.PI / 2));
            //LoadEnemyRanged(new Vector2(800, 475), new Vector2(800, 475), (float)(3 * Math.PI / 2));
            //LoadEnemyRanged(new Vector2(800, 525), new Vector2(800, 525), (float)(3 * Math.PI / 2));

            //// outside right
            //LoadEnemyRanged(new Vector2(850, 150), new Vector2(850, 150), (float)(3 * Math.PI / 2));
            //LoadEnemyRanged(new Vector2(850, 200), new Vector2(850, 200), (float)(3 * Math.PI / 2));
            //LoadEnemyRanged(new Vector2(850, 250), new Vector2(850, 250), (float)(3 * Math.PI / 2));
            //LoadEnemyRanged(new Vector2(850, 300), new Vector2(850, 300), (float)(3 * Math.PI / 2));
            //LoadEnemyRanged(new Vector2(850, 350), new Vector2(850, 350), (float)(3 * Math.PI / 2));
            //LoadEnemyRanged(new Vector2(850, 400), new Vector2(850, 400), (float)(3 * Math.PI / 2));
            //LoadEnemyRanged(new Vector2(850, 450), new Vector2(850, 450), (float)(3 * Math.PI / 2));
            //LoadEnemyRanged(new Vector2(850, 500), new Vector2(850, 500), (float)(3 * Math.PI / 2));
            //LoadEnemyRanged(new Vector2(850, 550), new Vector2(850, 550), (float)(3 * Math.PI / 2));

        }

        #endregion

        #region Song Info

        public void CheckSongStage(Stopwatch stopwatch)
        {
            //switch (songType)
            //CheckVoxisPourElleSongStage(stopwatch);

            GetCurrentDifficulty(stopwatch);
        }

        private void GetCurrentDifficulty(Stopwatch stopwatch)
        {
            if (stopwatch.ElapsedMilliseconds - lastSpawnTime < 1000) return;

            LevelTimeSpan levelTimeSpanObj = levelTimeSpanList.Where(lts => lts.StartTime < stopwatch.ElapsedMilliseconds && stopwatch.ElapsedMilliseconds < lts.EndTime).FirstOrDefault();

            if (levelTimeSpanObj == null) return;

            int numEnemiesToSpawn = rand.Next(levelTimeSpanObj.Difficulty + 1);

            for (int i = 0; i < numEnemiesToSpawn; i++)
            {
                SpawnNextEnemy(rand.Next(100));
            }

            CheckDrops(stopwatch, levelTimeSpanObj.Difficulty);

            lastSpawnTime = stopwatch.ElapsedMilliseconds;
        }

        private void CheckDrops(Stopwatch stopwatch, int difficulty)
        {
            if (AgentList.Where(ga => ga.Type == (int)Enums.AgentType.Enemy).Count() > 0)
            {
                GameAgent agent = AgentList.Where(ga => ga.Type == (int)Enums.AgentType.Enemy && ((Enemy)ga).DropType == 0).OrderByDescending(ga => ga.ID).FirstOrDefault();

                if (agent == null) return;

                if (nukeDropTime > 0 && stopwatch.ElapsedMilliseconds > nukeDropTime)
                {
                    ((Enemy)agent).DropType = (int)Enums.ItemType.NukeAttack;
                    nukeDropTime = 0;
                }
                //else if (teleportDropTime > 0 && stopwatch.ElapsedMilliseconds > teleportDropTime)
                //{
                //    ((Enemy)agent).DropType = (int)Enums.ItemType.TeleportSpell;
                //    teleportDropTime = 0;
                //}
                else if ( (difficulty != 0) && (stopwatch.ElapsedMilliseconds - lastPowerUp > powerUpInterval / difficulty) )
                {
                    Player playerObj = Game1.Current.player;

                    if (playerObj.Health < playerObj.MaxHealth * 0.25 && GetCurrentHealthPowerUpCount() < 3)
                    {
                        ((Enemy)agent).DropType = (int)Enums.ItemType.HealthPowerUp;
                        lastPowerUp = stopwatch.ElapsedMilliseconds;
                    }
                    else if (playerObj.Power < playerObj.MaxPower * 0.25 && GetCurrentEnergyPowerUpCount() < 3)
                    {
                        ((Enemy)agent).DropType = (int)Enums.ItemType.EnergyPowerUp;
                        lastPowerUp = stopwatch.ElapsedMilliseconds;
                    }
                }
            }
        }

        private void SpawnNextEnemy(int targetWeight)
        {
            if (targetWeight < kamikazeWeight)
            {
                LoadEnemyKamikazeTowardsPlayer();
            }

            targetWeight -= kamikazeWeight;

            if (targetWeight < rangedWeight)
            {
                LoadEnemyRanged();
            }

            targetWeight -= rangedWeight;

            if (targetWeight < randomWeight)
            {
                LoadEnemyRoamRandom();
            }

            targetWeight -= randomWeight;

            // aggressive not updated yet
            //if (targetWeight < aggressiveWeight)
            //{
            //    LoadEnemyAggressiveCloseToPlayer();
            //}
        }

        public void SpawnEnemies(Stopwatch stopwatch)
        {

            //spriteBatch.DrawString(font1, "0:49", new Vector2(900, 120), Color.Orange);
            //spriteBatch.DrawString(font1, "1:06", new Vector2(900, 140), Color.Orange);
            //spriteBatch.DrawString(font1, "1:24", new Vector2(900, 160), Color.Orange);
            //spriteBatch.DrawString(font1, "1:41", new Vector2(900, 180), Color.Orange);
            //spriteBatch.DrawString(font1, "1:57", new Vector2(900, 200), Color.Orange);
            //spriteBatch.DrawString(font1, "2:10", new Vector2(900, 220), Color.Orange);
            //spriteBatch.DrawString(font1, "2:18", new Vector2(900, 240), Color.Orange);
            //spriteBatch.DrawString(font1, "2:35", new Vector2(900, 260), Color.Orange);
            //spriteBatch.DrawString(font1, "2:43", new Vector2(900, 280), Color.Orange);
            //spriteBatch.DrawString(font1, "2:59", new Vector2(900, 300), Color.Orange);
            //spriteBatch.DrawString(font1, "3:16", new Vector2(900, 320), Color.Orange);
            //spriteBatch.DrawString(font1, "3:24", new Vector2(900, 340), Color.Orange);
            //spriteBatch.DrawString(font1, "3:42", new Vector2(900, 360), Color.Orange);
            //spriteBatch.DrawString(font1, "4:17", new Vector2(900, 380), Color.Orange);
        }

        public void CheckVoxisPourElleSongStage(Stopwatch stopwatch)
        {
            switch (songPhase)
            {
                case 1: if (stopwatch.ElapsedMilliseconds > 5000)  LoadVoxisPourElleSongStage1(); break; // about 5 sec
                case 2: if (stopwatch.ElapsedMilliseconds > 10000) LoadVoxisPourElleSongStage2(); break; // about 10 sec
                case 3: if (stopwatch.ElapsedMilliseconds > 20000) LoadVoxisPourElleSongStage3(); break; // about 15 sec
                case 4: if (stopwatch.ElapsedMilliseconds > 25000) LoadVoxisPourElleSongStage4(); break; // about 30 sec
                case 5: if (stopwatch.ElapsedMilliseconds > 30000) LoadVoxisPourElleSongStage4(); break; // about 49 sec
            } 
        }

        private void LoadVoxisPourElleSongStage1()
        {

            //TextureInfo ti = new TextureInfo("enemy_agent1");

            //int wave = 1;
            //int health = 100;
            //int maxSpeed = 2;

            //songPhase++;
            //LoadEnemyRangedNoAttack(ti, new Vector2(-150, 450), new Vector2(200, 450), (float)(Math.PI / 2), health, maxSpeed, wave);
            //LoadEnemyRangedNoAttack(ti, new Vector2(-150, 550), new Vector2(200, 550), (float)(Math.PI / 2), health, maxSpeed, wave);
            //LoadEnemyRangedNoAttack(ti, new Vector2(-100, 475), new Vector2(250, 475), (float)(Math.PI / 2), health, maxSpeed, wave);
            //LoadEnemyRangedNoAttack(ti, new Vector2(-100, 525), new Vector2(250, 525), (float)(Math.PI / 2), health, maxSpeed, wave);
            //LoadEnemyRangedNoAttack(ti, new Vector2(-50, 500), new Vector2(300, 500), (float)(Math.PI / 2), health, maxSpeed, wave);

            //LoadEnemyRangedNoAttack(ti, new Vector2(-150, 150), new Vector2(200, 150), (float)(Math.PI / 2), health, maxSpeed, wave);
            //LoadEnemyRangedNoAttack(ti, new Vector2(-150, 250), new Vector2(200, 250), (float)(Math.PI / 2), health, maxSpeed, wave);
            //LoadEnemyRangedNoAttack(ti, new Vector2(-100, 175), new Vector2(250, 175), (float)(Math.PI / 2), health, maxSpeed, wave);
            //LoadEnemyRangedNoAttack(ti, new Vector2(-100, 225), new Vector2(250, 225), (float)(Math.PI / 2), health, maxSpeed, wave);
            //LoadEnemyRangedNoAttack(ti, new Vector2(-50, 200), new Vector2(300, 200), (float)(Math.PI / 2), health, maxSpeed, wave);

            //LoadEnemyRangedNoAttack(ti, new Vector2(1300, 450), new Vector2(950, 450), (float)(3 * Math.PI / 2), health, maxSpeed, wave);
            //LoadEnemyRangedNoAttack(ti, new Vector2(1300, 550), new Vector2(950, 550), (float)(3 * Math.PI / 2), health, maxSpeed, wave);
            //LoadEnemyRangedNoAttack(ti, new Vector2(1250, 475), new Vector2(900, 475), (float)(3 * Math.PI / 2), health, maxSpeed, wave);
            //LoadEnemyRangedNoAttack(ti, new Vector2(1250, 525), new Vector2(900, 525), (float)(3 * Math.PI / 2), health, maxSpeed, wave);
            //LoadEnemyRangedNoAttack(ti, new Vector2(1200, 500), new Vector2(850, 500), (float)(3 * Math.PI / 2), health, maxSpeed, wave);

            //LoadEnemyRangedNoAttack(ti, new Vector2(1300, 150), new Vector2(950, 150), (float)(3 * Math.PI / 2), health, maxSpeed, wave);
            //LoadEnemyRangedNoAttack(ti, new Vector2(1300, 250), new Vector2(950, 250), (float)(3 * Math.PI / 2), health, maxSpeed, wave);
            //LoadEnemyRangedNoAttack(ti, new Vector2(1250, 175), new Vector2(900, 175), (float)(3 * Math.PI / 2), health, maxSpeed, wave);
            //LoadEnemyRangedNoAttack(ti, new Vector2(1250, 225), new Vector2(900, 225), (float)(3 * Math.PI / 2), health, maxSpeed, wave);
            //LoadEnemyRangedNoAttack(ti, new Vector2(1200, 200), new Vector2(850, 200), (float)(3 * Math.PI / 2), health, maxSpeed, wave);
        }

        private void LoadVoxisPourElleSongStage2()
        {
            TextureInfo ti = new TextureInfo("follower1");
            
            songPhase++;
            int wave = 2;
            int health = 100;
            int maxSpeed = 2;

            LoadEnemyRangedNoAttack(ti, new Vector2(-50, 50), new Vector2(200, 200), (float)(Math.PI / 2), health, maxSpeed, wave); // middle left
            LoadEnemyRangedNoAttack(ti, new Vector2(600, -150), new Vector2(300, 150), (float)(Math.PI / 2), health, maxSpeed, wave);// top 
            LoadEnemyRangedNoAttack(ti, new Vector2(1000, -300), new Vector2(300, 250), (float)(Math.PI / 2), health, maxSpeed, wave);// bottom

            LoadEnemyRangedNoAttack(ti, new Vector2(-250, 300), new Vector2(200, 500), (float)(Math.PI / 2), health, maxSpeed, wave);  // middle left
            LoadEnemyRangedNoAttack(ti, new Vector2(600, 900), new Vector2(300, 550), (float)(Math.PI / 2), health, maxSpeed, wave);// top 
            LoadEnemyRangedNoAttack(ti, new Vector2(800, 900), new Vector2(300, 450), (float)(Math.PI / 2), health, maxSpeed, wave);// bottom

            LoadEnemyRangedNoAttack(ti, new Vector2(1450, 50), new Vector2(950, 200), (float)(3 * Math.PI / 2), health, maxSpeed, wave);  // middle left
            LoadEnemyRangedNoAttack(ti, new Vector2(500, -100), new Vector2(850, 150), (float)(3 * Math.PI / 2), health, maxSpeed, wave);// top 
            LoadEnemyRangedNoAttack(ti, new Vector2(300, -50), new Vector2(850, 250), (float)(3 * Math.PI / 2), health, maxSpeed, wave);// bottom

            LoadEnemyRangedNoAttack(ti, new Vector2(1250, 500), new Vector2(950, 500), (float)(3 * Math.PI / 2), health, maxSpeed, wave); // middle left
            LoadEnemyRangedNoAttack(ti, new Vector2(400, 1050), new Vector2(850, 550), (float)(3 * Math.PI / 2), health, maxSpeed, wave);// top 
            LoadEnemyRangedNoAttack(ti, new Vector2(400, 900), new Vector2(850, 450), (float)(3 * Math.PI / 2), health, maxSpeed, wave);// bottom
        }

        private void LoadVoxisPourElleSongStage3()
        {
            TextureInfo ti = new TextureInfo("kamikaze1");

            int health = 100;
            int maxSpeed = 2;
            int wave1 = 3;
            int wave2 = 2;

            songPhase++;
            LoadEnemyRangedNoAttack(ti, new Vector2(-150, 350), new Vector2(200, 350), (float)(Math.PI / 2), health, maxSpeed, wave1);
            LoadEnemyRangedNoAttack(ti, new Vector2(-100, 325), new Vector2(250, 325), (float)(Math.PI / 2), health, maxSpeed, wave1);
            LoadEnemyRangedNoAttack(ti, new Vector2(-100, 375), new Vector2(250, 375), (float)(Math.PI / 2), health, maxSpeed, wave1);
            LoadEnemyRangedNoAttack(ti, new Vector2(-50, 300), new Vector2(300, 300), (float)(Math.PI / 2), health, maxSpeed, wave1);
            LoadEnemyRangedNoAttack(ti, new Vector2(-50, 350), new Vector2(300, 350), (float)(Math.PI / 2), health, maxSpeed, wave1);
            LoadEnemyRangedNoAttack(ti, new Vector2(-50, 400), new Vector2(300, 400), (float)(Math.PI / 2), health, maxSpeed, wave1);

            LoadEnemyRangedNoAttack(ti, new Vector2(1300, 350), new Vector2(950, 350), (float)(3 * Math.PI / 2), health, maxSpeed, wave1);
            LoadEnemyRangedNoAttack(ti, new Vector2(1250, 325), new Vector2(900, 325), (float)(3 * Math.PI / 2), health, maxSpeed, wave1);
            LoadEnemyRangedNoAttack(ti, new Vector2(1250, 375), new Vector2(900, 375), (float)(3 * Math.PI / 2), health, maxSpeed, wave1);
            LoadEnemyRangedNoAttack(ti, new Vector2(1200, 300), new Vector2(850, 300), (float)(3 * Math.PI / 2), health, maxSpeed, wave1);
            LoadEnemyRangedNoAttack(ti, new Vector2(1200, 350), new Vector2(850, 350), (float)(3 * Math.PI / 2), health, maxSpeed, wave1);
            LoadEnemyRangedNoAttack(ti, new Vector2(1200, 400), new Vector2(850, 400), (float)(3 * Math.PI / 2), health, maxSpeed, wave1);

            TextureInfo ti2 = new TextureInfo("follower1");
            LoadEnemyRangedNoAttack(ti2, new Vector2(-150, 300), new Vector2(200, 300), (float)(Math.PI / 2), health, maxSpeed, wave2);
            LoadEnemyRangedNoAttack(ti2, new Vector2(-150, 400), new Vector2(200, 400), (float)(Math.PI / 2), health, maxSpeed, wave2);
            LoadEnemyRangedNoAttack(ti2, new Vector2(1300, 300), new Vector2(950, 300), (float)(3 * Math.PI / 2), health, maxSpeed, wave2);
            LoadEnemyRangedNoAttack(ti2, new Vector2(1300, 400), new Vector2(950, 400), (float)(3 * Math.PI / 2), health, maxSpeed, wave2);
        }

        private void LoadVoxisPourElleSongStage4()
        {
            songPhase++;

            int health = 100;
            int maxSpeed = 1;
            int wave = 4;

            TextureInfo ti = new TextureInfo("big_ship1");
            LoadEnemyRangedNoAttack(ti, new Vector2(520, -50), new Vector2(520, 50), (float)Math.PI, health, maxSpeed, wave);
            LoadEnemyRangedNoAttack(ti, new Vector2(680, -50), new Vector2(680, 50), (float)Math.PI, health, maxSpeed, wave);

            LoadEnemyRangedNoAttack(ti, new Vector2(520, 750), new Vector2(520, 650), 0, health, maxSpeed, wave);
            LoadEnemyRangedNoAttack(ti, new Vector2(680, 750), new Vector2(680, 650), 0, health, maxSpeed, wave);
        }

        private void LoadVoxisPourElleSongStage5()
        {
            songPhase++;

            Player playerObj = Game1.Current.player;

            playerObj.HasControl = true;

            Attack attack1 = new Attack();
            attack1.Active = true;
            attack1.AttackType = (int)Enums.AttackType.Bullet;
            attack1.AttackSubType = (int)Enums.AttackSubType.Default;
            attack1.CoolDown = 3000;
            attack1.ActiveCoolDown = 1000;
            attack1.Texture = Game1.Current.Content.Load<Texture2D>("Images\\raindrop");
            attack1.Frames = 1;
            attack1.MinDamage = 10;
            attack1.MaxDamage = 15;
            attack1.Color = Color.Red;

            int count = 0;

            foreach (Enemy enemyObj in AgentList.Where(p => p.Type == (int)Enums.AgentType.Enemy && ((Enemy)p).Wave == 1).ToList())
            {
                enemyObj.attackList.Add(attack1);
                enemyObj.State = Enums.EnemyState.Ranged;
                enemyObj.IsUsingAttacks = true;
            }

            foreach (Enemy enemyObj in AgentList.Where(p => p.Type == (int)Enums.AgentType.Enemy && ((Enemy)p).Wave == 2).ToList())
            {
                enemyObj.attackList.Add(attack1);
                enemyObj.State = Enums.EnemyState.RoamRandom;
                enemyObj.TimeAtEachPoint = 100;
                enemyObj.Spacing = 10;
                enemyObj.MaxSpeed = 2;
                enemyObj.IsUsingAttacks = true;
            }

            foreach (Enemy enemyObj in AgentList.Where(p => p.Type == (int)Enums.AgentType.Enemy && ((Enemy)p).Wave == 3).OrderBy(p => Vector2.Distance(playerObj.Position, p.Position)).ToList())
            {
                enemyObj.State = Enums.EnemyState.KamikazeTowardsPlayer;
                enemyObj.MaxSpeed = count < 10 ? 3 : 5; // furthest 2 kamikaze's fly faster towards player
                count++;
            }

            Random rand = new Random();
            int index = rand.Next(AgentList.Count);
            ((Enemy)AgentList[index]).DropType = (int)Enums.ItemType.NukeAttack;
        }

        #endregion

        #region Helpers

        private int GetCurrentEnergyPowerUpCount()
        {
            return AgentList.Where(ga => (ga.GetType() == typeof(Item) && ((Item)ga).ItemType == (int)Enums.ItemType.EnergyPowerUp)
                                      || (ga.GetType() == typeof(Enemy) && ((Enemy)ga).DropType == (int)Enums.ItemType.EnergyPowerUp)).Count();
        }

        private int GetCurrentHealthPowerUpCount()
        {
            return AgentList.Where(ga => (ga.GetType() == typeof(Item) && ((Item)ga).ItemType == (int)Enums.ItemType.HealthPowerUp)
                                      || (ga.GetType() == typeof(Enemy) && ((Enemy)ga).DropType == (int)Enums.ItemType.HealthPowerUp)).Count();
        }

        #endregion

        private Vector2 GetRandomEndPos(Vector2 startPos)
        {
            Vector2 endPos = new Vector2();
            
            int padding = 50;

            if (startPos.X < 0)
            {
                endPos.X = rand.Next(padding, 3 * padding);
                endPos.Y = rand.Next((int)startPos.Y - padding > padding ? padding : (int)startPos.Y - padding, (int)startPos.Y + padding > Height - padding ? Height - padding : (int)startPos.Y + padding);
                return endPos;
            }

            if (startPos.X > Width)
            {
                endPos.X = rand.Next(Width - (3 * padding), Width - padding);
                endPos.Y = rand.Next((int)startPos.Y - padding > padding ? padding : (int)startPos.Y - padding, (int)startPos.Y + padding > Height - padding ? Height - padding : (int)startPos.Y + padding);
                return endPos;
            }

            if (startPos.Y < 0)
            {
                endPos.X = rand.Next((int)startPos.X - padding > padding ? padding : (int)startPos.X - padding, (int)startPos.X + padding > Width - padding ? Width - padding : (int)startPos.X + padding);
                endPos.Y = rand.Next(padding, 3 * padding); 
                return endPos;
            }

            if (startPos.Y > Height)
            {
                endPos.X = rand.Next((int)startPos.X - padding > padding ? padding : (int)startPos.X - padding, (int)startPos.X + padding > Width - padding ? Width - padding : (int)startPos.X + padding);
                endPos.Y = rand.Next(Height - (3 * padding), Height - padding);
                return endPos;
            }

            return new Vector2(Width / 2, Height / 2);
        }

        private Vector2 GetRandomStartPos()
        {
            Vector2 position = new Vector2();

            int randDirection = rand.Next(4);
            int padding = 40;

            switch (randDirection)
            {
                case (int)Enums.Direction.Up:

                    position.X = rand.Next(padding, Width - padding);
                    position.Y = -50;

                    break;

                case (int)Enums.Direction.Down:

                    position.X = rand.Next(padding, Width - padding);
                    position.Y = Height + 50;

                    break;

                case (int)Enums.Direction.Left:

                    position.X = -50;
                    position.Y = rand.Next(padding, Height - padding);

                    break;

                case (int)Enums.Direction.Right:

                    position.X = Width + 50;
                    position.Y = rand.Next(padding, Height - padding);

                    break;
            }

            return position;
        }
    }
}
