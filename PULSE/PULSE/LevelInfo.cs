using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using PulseGame.Agents;
using PulseGame.Attacks;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using PulseGame.Helpers;

namespace PulseGame
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
        //private int aggressiveWeight = 10;
        private List<LevelTimeSpan> levelTimeSpanList = new List<LevelTimeSpan>();
        private Random rand = new Random();
        private long bossSpawnTime;
        private long lastSpawnTime = 0;
        private long lastPowerUp = 0;
        private long teleportDropTime;
        private long nukeDropTime;
        private int powerUpInterval = 5000;
        //private int lastType;

        public LevelInfo()
        {
            Initialize();
        }

        public void Initialize()
        {
            AgentList.Clear();
            levelTimeSpanList.Clear();

            levelTimeSpanList.Add(new LevelTimeSpan(0, 1000, 0));
            // swap between 0 and 1 difficulty for the song intro
            for (int i = 1000; i < 46000; i += 1999)
            {
                levelTimeSpanList.Add(new LevelTimeSpan(i, i + 1999, ((i % 2 == 0) ? 1 : 0)));
            }
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
            teleportDropTime = rand.Next(1000, 5000);
            bossSpawnTime = 60000;
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
                windowHeight - (windowHeight / 10)
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

        private void SpawnBoss()
        {
            int health = 50000;

            TextureInfo ti = new TextureInfo("boss1");
            ti.Scale = 0.75f;

            Enemy boss = new Enemy();
            boss.LoadContent(PulseGame.Current.Content, ti.TextureString, ti.TextureRect, ti.Frames);
            boss.Position = new Vector2(VisibleRect.Right + 50, VisibleRect.Center.Y);
            boss.TargetPosition = new Vector2(VisibleRect.Right - 150, VisibleRect.Center.Y);
            boss.Rotation = (float)(7 * Math.PI / 2);
            boss.Type = (int)Enums.AgentType.Enemy;
            boss.State = Enums.EnemyState.Boss;
            boss.Health = health;
            boss.Scale = ti.Scale;
            boss.MaxSpeed = 2;
            boss.MeleeDistance = 5;
            boss.Score = 10000;
            boss.ID = nextID;

            Attack attackObj = new Attack();
            attackObj.Active = true;
            attackObj.AttackType = Enums.AttackType.Bullet;
            attackObj.AttackSubType = Enums.AttackSubType.DoubleTriBullet;
            attackObj.CoolDown = 15000;
            attackObj.ActiveCoolDown = 1000;
            attackObj.AttackCost = 100;
            attackObj.Texture = PulseGame.Current.Content.Load<Texture2D>("Images\\raindrop");
            attackObj.Frames = 1;
            attackObj.MinDamage = 1;
            attackObj.MaxDamage = 1;
            attackObj.Color = Color.Red;
            attackObj.MaxSequence = 10000;
            boss.attackList.Add(attackObj);

            TextureInfo ti1 = new TextureInfo("star_effect2");

            Attack attackObj1 = new Attack();
            attackObj1.Active = true;
            attackObj1.AttackType = Enums.AttackType.Explosion;
            attackObj1.AttackSubType = Enums.AttackSubType.ReflectingStar;
            attackObj1.CoolDown = 30000;
            attackObj1.ActiveCoolDown = 1000;
            attackObj1.Texture = PulseGame.Current.Content.Load<Texture2D>(ti1.TextureString);
            attackObj1.BoundingRect = ti1.TextureRect;
            attackObj1.Frames = ti1.Frames;
            attackObj1.MinDamage = 100;
            attackObj1.MaxDamage = 200;
            attackObj1.Color = Color.White;
            boss.attackList.Add(attackObj1);

            AgentList.Add(boss);
        }

        private void LoadEnemyKamikazeTowardsPlayer()
        {
            int minHealth = 10;
            int maxHealth = 20;

            TextureInfo ti = new TextureInfo("kamikaze1");

            Enemy pAgent1 = new Enemy();
            pAgent1.LoadContent(PulseGame.Current.Content, ti.TextureString, ti.TextureRect, ti.Frames);
            pAgent1.Position = GetRandomStartPos();// new Vector2(400, 800);//1150, 2650);
            pAgent1.Rotation = 0.0f;
            pAgent1.Type = (int)Enums.AgentType.Enemy;
            pAgent1.State = Enums.EnemyState.KamikazeTowardsPlayer;
            pAgent1.TargetPosition = new Vector2(400, 800);
            pAgent1.Health = rand.Next(minHealth, maxHealth);
            pAgent1.MaxSpeed = 5;
            pAgent1.Score = 10;
            pAgent1.ID = nextID;

            nextID++;

            AgentList.Add(pAgent1);
        }

        private void LoadEnemyKamikazeTowardsTarget()
        {
            Enemy pAgent1 = new Enemy();
            pAgent1.LoadContent(PulseGame.Current.Content, "Images\\enemy_agent1", new Rectangle(0, 0, 26, 29), 4);
            pAgent1.Position = new Vector2(400, 800);//1150, 2650);
            pAgent1.Rotation = 0.0f;
            pAgent1.Type = (int)Enums.AgentType.Enemy;
            pAgent1.State = Enums.EnemyState.KamikazeAcrossScreen;
            pAgent1.TargetPosition = new Vector2(400, -100);
            pAgent1.Health = 40;
            pAgent1.Score = 10;
            pAgent1.MaxSpeed = 5;

            AgentList.Add(pAgent1);
        }

        private void LoadEnemyAggressiveCloseToPlayer()
        {
            Enemy pAgent1 = new Enemy();
            pAgent1.LoadContent(PulseGame.Current.Content, "Images\\enemy_agent1", new Rectangle(0, 0, 26, 29), 4);
            pAgent1.Position = new Vector2(400, 800);//1150, 2650);
            pAgent1.Rotation = 0.0f;
            pAgent1.Type = (int)Enums.AgentType.Enemy;
            pAgent1.State = Enums.EnemyState.AggressiveCloseToPlayer;
            pAgent1.MeleeDistance = 200;
            pAgent1.Health = 15;
            pAgent1.Score = 10;
            pAgent1.MaxSpeed = 3;

            Attack attack1 = new Attack();
            attack1.Active = true;
            attack1.AttackType = (int)Enums.AttackType.Bullet;
            attack1.AttackSubType = (int)Enums.AttackSubType.Default;
            attack1.CoolDown = 1000;
            attack1.Texture = PulseGame.Current.Content.Load<Texture2D>("Images\\raindrop");
            attack1.Frames = 1;
            attack1.MinDamage = 10;
            attack1.MaxDamage = 15;
            pAgent1.attackList.Add(attack1);

            AgentList.Add(pAgent1);
        }

        private void LoadEnemyRoamRandomNoAttack()
        {
            Enemy pAgent1 = new Enemy();
            pAgent1.LoadContent(PulseGame.Current.Content, "Images\\enemy_agent1", new Rectangle(0, 0, 26, 29), 4);
            pAgent1.Position = new Vector2(400, 800);//1150, 2650);
            pAgent1.TargetPosition = new Vector2(400, 600);
            pAgent1.Rotation = 0.0f;
            pAgent1.Type = (int)Enums.AgentType.Enemy;
            pAgent1.State = Enums.EnemyState.RoamRandomNoAttack;
            pAgent1.MeleeDistance = 40;
            pAgent1.Health = 15;
            pAgent1.Score = 10;
            pAgent1.MaxSpeed = 3;

            AgentList.Add(pAgent1);
        }

        private void LoadEnemyRanged()
        {
            TextureInfo ti = new TextureInfo("enemy_agent1");

            Enemy pAgent1 = new Enemy();
            pAgent1.LoadContent(PulseGame.Current.Content, ti.TextureString, ti.TextureRect, ti.Frames);
            pAgent1.Position = GetRandomStartPos();//1150, 2650);
            pAgent1.TargetPosition = GetRandomEndPos(pAgent1.Position);//1150, 2650);
            pAgent1.Rotation = 0.0f;
            pAgent1.Type = (int)Enums.AgentType.Enemy;
            pAgent1.State = Enums.EnemyState.Ranged;
            pAgent1.MeleeDistance = 2;
            pAgent1.Health = 15;
            pAgent1.MaxSpeed = 2;
            pAgent1.Score = 10;
            pAgent1.IsUsingAttacks = true;
            pAgent1.ID = nextID;

            nextID++;

            Attack attackObj = new Attack();
            attackObj.Active = true;
            attackObj.AttackType = (int)Enums.AttackType.Bullet;
            attackObj.AttackSubType = (int)Enums.AttackSubType.Default;
            attackObj.CoolDown = 3000;
            attackObj.ActiveCoolDown = 1000;
            attackObj.Texture = PulseGame.Current.Content.Load<Texture2D>("Images\\raindrop");
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
            pAgent1.LoadContent(PulseGame.Current.Content, "Images\\enemy_agent1", new Rectangle(0, 0, 26, 29), 4);
            pAgent1.Position = startPos;//1150, 2650);
            pAgent1.TargetPosition = targetPos;//1150, 2650);
            pAgent1.Rotation = targetRotation;
            pAgent1.Type = (int)Enums.AgentType.Enemy;
            pAgent1.State = Enums.EnemyState.RangedNoAttack;
            pAgent1.MeleeDistance = 2;
            pAgent1.Health = 15;
            pAgent1.MaxSpeed = 2;
            pAgent1.Score = 10;
            pAgent1.TargetRotation = targetRotation;
            pAgent1.IsUsingAttacks = false;

            AgentList.Add(pAgent1);
        }

        private void LoadEnemyRangedNoAttack(TextureInfo textureInfo,
                                             Vector2 startPos, 
                                             Vector2 targetPos, 
                                             float targetRotation,
                                             int health,
                                             int maxSpeed,
                                             int wave)
        {
            Enemy pAgent1 = new Enemy();
            pAgent1.LoadContent(PulseGame.Current.Content, textureInfo.TextureString, textureInfo.TextureRect, textureInfo.Frames);
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
            pAgent1.Score = 10;

            nextID++;

            AgentList.Add(pAgent1);
        }

        private void LoadEnemyRoamRandom()
        {
            TextureInfo ti = new TextureInfo("follower1");

            int maxHealth = 15;
            int minHealth = 5;

            Enemy pAgent1 = new Enemy();
            pAgent1.LoadContent(PulseGame.Current.Content, ti.TextureString, ti.TextureRect, ti.Frames);
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
            pAgent1.Score = 10;

            nextID++;

            Attack attackObj = new Attack();
            attackObj.Active = true;
            attackObj.AttackType = (int)Enums.AttackType.Bullet;
            attackObj.AttackSubType = (int)Enums.AttackSubType.Default;
            attackObj.CoolDown = 3000;
            attackObj.ActiveCoolDown = 1000;
            attackObj.Texture = PulseGame.Current.Content.Load<Texture2D>("Images\\raindrop");
            attackObj.Frames = 1;
            attackObj.MinDamage = 10;
            attackObj.MaxDamage = 15;
            attackObj.Color = Color.Red;
            pAgent1.attackList.Add(attackObj);

            AgentList.Add(pAgent1);
        }

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
            TimeAllocated = 10;//280; // in seconds (4:40)

            SetVisibleArea(new Vector2(), Width, Height);

            //PlayerStartPos = new Vector2(1850, 3000);
            PlayerStartPos = new Vector2(600, 350);
        }

        #endregion

        #region Song Info

        public void CheckSongStage(Stopwatch stopwatch)
        {
            GetCurrentDifficulty(stopwatch);
        }

        private void GetCurrentDifficulty(Stopwatch stopwatch)
        {
            if (stopwatch.ElapsedMilliseconds > bossSpawnTime && bossSpawnTime > 0)
            {
                SpawnBoss();
                bossSpawnTime = 0;
            }

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
                else if (teleportDropTime > 0 && stopwatch.ElapsedMilliseconds > teleportDropTime)
                {
                    ((Enemy)agent).DropType = (int)Enums.ItemType.TeleportSpell;
                    teleportDropTime = 0;
                }
                else if ( (difficulty != 0) && (stopwatch.ElapsedMilliseconds - lastPowerUp > powerUpInterval / difficulty) )
                {
                    Player playerObj = PulseGame.Current.player;

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
