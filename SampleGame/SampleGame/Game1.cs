using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Drawing;
using System.Diagnostics;
using SampleGame.Helpers;
using SampleGame.Effects;
using SampleGame.Attacks;  // DrawingHelper namespace

namespace SampleGame
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        KeyboardState keyboardStateCurrent;
        KeyboardState keyboardStatePrevious;
        public MouseState mouseStateCurrent;
        MouseState mouseStatePrevious;
        Texture2D crosshairTexture;
        Texture2D titleTexture;
        Song backgroundMusic;
        bool songStart = false;
        bool isCountdown = false;
        double countdown = 4;
        Stopwatch sw = new Stopwatch();
        SoundEffect countdownSound;
        int titleScreen = 0;
        float scaleSize = 1.0f;
        bool titleExpanding = false;
        
        double timer;           // for time elapsed
        double deltaT;          // for time elapsed
        Stopwatch stopwatch;    // for time elapsed
         
        //Vector2? startPos = null;
        //Vector2? endPos = null;
        List<LevelNode> pathToTarget = new List<LevelNode>();
        //Texture2D debugNodeTexture;

        public EffectComponent EffectComponent;

        //int debugIndex = -1;
        
        int windowWidth = 0;
        int windowHeight = 0;

        public static Game1 Current { get; private set; }
        public Player player;
        public LevelInfo levelInfo = new LevelInfo();
        public SpriteFont font1;
        public SoundEffect PlayerHitSound;
        public SoundEffect BerserkerSound;
        public bool IsTitleScreen = true;
        public string LevelEndText = "Congratulations!";

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferHeight = 700;    // height of screen
            graphics.PreferredBackBufferWidth = 1200;    // width of screen
            graphics.IsFullScreen = false;
            graphics.ApplyChanges();
            Content.RootDirectory = "Content";

            windowWidth = Window.ClientBounds.Width;
            windowHeight = Window.ClientBounds.Height;

            Components.Add(EffectComponent = new EffectComponent(this));

            Current = this;
        }

        protected override void Initialize()
        {
            // Initialize the input devices
            this.IsMouseVisible = false;
            keyboardStateCurrent = new KeyboardState();
            mouseStateCurrent = new MouseState();

            // Initialize DrawingHelper
            DrawingHelper.Initialize(GraphicsDevice);

            // Initialize stopwatch for consistent times across machines
            stopwatch = new Stopwatch();

            player = new Player();
            player.AnimationInterval = TimeSpan.FromMilliseconds(100);          // next frame every 100 miliseconds
            player.RotationSpeed = 6.0f;                                        // rotate somewhat quick
            player.Speed = 4.0f;                                                // setting forward - backward speed
            //player.InitializeSensors();                                         // initializes all sensors for the player object
            player.Health = 100;
            player.HasControl = true;

            // ************ CREATING THE WALLS FOR THE ASSIGNMENT ********* //
            //int defaultWalls = 2;

            //for (int i = 0; i < defaultWalls; i++)
            //{
            //    agentAIList.Add(new GameAgent()
            //    {
            //        Type = (int)Enums.AgentType.Wall
            //    });
            //}

            // ********** END CREATING THE WALLS FOR THE ASSIGNMENT ******* //

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // load in the background music
            backgroundMusic = Content.Load<Song>("Audio\\voxis_pour_elle");
            MediaPlayer.IsRepeating = false;    // everything is set up for sound to loop - just change this value

            // loading the player's image
            player.LoadContent(this.Content, "Images\\ship1", new Rectangle(0, 0, 38, 41), 8);

            // loading the font to display text on the screen
            font1 = Content.Load<SpriteFont>("fonts/Font1");

            // load sounds
            PlayerHitSound = Content.Load<SoundEffect>("Audio\\player_hit");
            countdownSound = Content.Load<SoundEffect>("Audio\\beep");
            BerserkerSound = Content.Load<SoundEffect>("Audio\\whistle-flute-1");

            // load the custom crosshair
            crosshairTexture = Content.Load<Texture2D>("Images\\crosshair");

            // load the title texture
            titleTexture = Content.Load<Texture2D>("Images\\PULSE");

            Attack attack1 = new Attack();
            attack1.Active = true;
            attack1.Key = Keys.Space;
            attack1.AttackType = Enums.AttackType.Bullet;
            attack1.AttackSubType = Enums.AttackSubType.TriBullet;
            attack1.CoolDown = 50;
            attack1.Texture = Content.Load<Texture2D>("Images\\raindrop");
            attack1.Frames = 1;
            attack1.MinDamage = 5;
            attack1.MaxDamage = 10;
            attack1.AttackCost = 0;
            player.attackList.Add(attack1);

            //Attack attack2 = new Attack();
            //attack2.Active = true;
            //attack2.Key = Keys.D2;
            //attack2.AttackType = Enums.AttackType.Explosion;
            //attack2.AttackSubType = Enums.AttackSubType.Default;
            //attack2.CoolDown = 1000;
            //attack2.Texture = Content.Load<Texture2D>("Images\\explosion1");
            //attack2.IconTexture = Content.Load<Texture2D>("Images\\skill2");
            //attack2.HasIcon = true;
            //attack2.Frames = 6;
            //attack2.BoundingRect = new Rectangle(0, 0, 139, 107);
            //attack2.AttackCost = 10;
            //player.attackList.Add(attack2);

            Attack attack3 = new Attack();
            attack3.Active = true;
            attack3.Key = Keys.D1;
            attack3.AttackType = Enums.AttackType.Bullet;
            attack3.AttackSubType = Enums.AttackSubType.BulletShield;
            attack3.CoolDown = 1500;
            attack3.Texture = Content.Load<Texture2D>("Images\\raindrop");
            attack3.IconTexture = Content.Load<Texture2D>("Images\\skill1");
            attack3.HasIcon = true;
            attack3.AttackCost = 30;
            attack3.MinDamage = 10;
            attack3.MaxDamage = 200;
            player.attackList.Add(attack3);

            // ***** BULLET SPLITTING ***** //
            //Attack attack4 = new Attack();
            //attack4.Active = true;
            //attack4.Key = Keys.D3;
            //attack4.AttackType = Enums.AttackType.Bullet;
            //attack4.AttackSubType = Enums.AttackSubType.SplitBullets;
            //attack4.CoolDown = 1500;
            ////attack3.Texture = Content.Load<Texture2D>("Images\\explosion1");
            ////attack3.Frames = 6;
            ////attack3.BoundingRect = new Rectangle(0, 0, 139, 107);
            //attack4.AttackCost = 0;
            //player.attackList.Add(attack4);

            //Attack attack5 = new Attack();
            //attack5.Active = true;
            //attack5.Key = Keys.D4;
            //attack5.AttackType = Enums.AttackType.Bullet;
            //attack5.AttackSubType = Enums.AttackSubType.Nuke;
            //attack5.CoolDown = 3000;
            //attack5.Texture = Content.Load<Texture2D>("Images\\raindrop");
            //attack5.IconTexture = Content.Load<Texture2D>("Images\\skill4");
            //attack5.AttackCost = 40;
            //attack5.MinDamage = 50;
            //attack5.MaxDamage = 100;
            //player.attackList.Add(attack5);

            levelInfo.LoadLevel(0, this.Content, windowWidth, windowHeight);

            #region load walls for previous assignments
            // ************ LOADING THE WALLS FOR THE ASSIGNMENT ********* //

            //Random rnd = new Random();

            //for (int i = 0; i < agentAIList.Count; i++)
            //{
            //    int randNumb = rnd.Next(1);
            //    agentAIList[i].LoadContent(this.Content, randNumb > 0 ? "Images\\wall" : "Images\\wall1", null, 1, randNumb > 0);
            //    agentAIList[i].Scale = (float)rnd.Next(100) / 50 + 1;
            //    agentAIList[i].Position = new Vector2(rnd.Next(windowWidth), rnd.Next(windowHeight));

            //    int targetIndex = -1;

            //    // making sure the walls aren't out of the zone, intersecting the player, or intersecting other walls
            //    while (targetIndex < i)
            //    {
            //        Rectangle r = agentAIList[i].Bounds;
            //        if (targetIndex < 0 && (r.Left < 0 || r.Top < 0 || r.Left + r.Width > windowWidth || r.Top + r.Height > windowHeight || r.Intersects(player.Bounds)))
            //        {
            //            agentAIList[i].Position = new Vector2(rnd.Next(windowWidth), rnd.Next(windowHeight));
            //        }
            //        else if (targetIndex >= 0 && agentAIList[i].Bounds.Intersects(agentAIList[targetIndex].Bounds))
            //        {
            //            agentAIList[i].Position = new Vector2(rnd.Next(windowWidth), rnd.Next(windowHeight));

            //            targetIndex = 0;
            //        }
            //        else
            //        {
            //            targetIndex++;
            //        }
            //    }
            //}

            // ********* END LOADING THE WALLS FOR THE ASSIGNMENT ******** //
            #endregion

            //debugNodeTexture = this.Content.Load<Texture2D>("Images\\10_by_10");

            player.Position = levelInfo.PlayerStartPos;
            Mouse.SetPosition(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);
        }

        protected override void UnloadContent()
        {
            
        }

        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if ((keyboardStateCurrent.IsKeyUp(Keys.Escape) && keyboardStatePrevious.IsKeyDown(Keys.Escape)) || GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // Update keyboard state
            keyboardStatePrevious = keyboardStateCurrent;
            keyboardStateCurrent = Keyboard.GetState();

            // Update mouse state
            mouseStatePrevious = mouseStateCurrent;
            mouseStateCurrent = Mouse.GetState();
            
            if (IsTitleScreen && titleScreen == 0)
            {
                if (keyboardStateCurrent.IsKeyDown(Keys.Enter) || mouseStateCurrent.LeftButton == ButtonState.Pressed)
                {
                    IsTitleScreen = false;
                    isCountdown = true;
                    sw.Start();
                }
            }
            else if (isCountdown)
            {
                if ((countdown > 2.99 && countdown < 3.01) ||
                        (countdown > 1.99 && countdown < 2.01) ||
                        (countdown > 0.99 && countdown < 1.01))
                    countdownSound.Play();

                deltaT = sw.Elapsed.TotalSeconds - timer;
                timer += deltaT;
                countdown -= deltaT;
                
                if (countdown <= 0)
                {
                    isCountdown = false;
                    timer = 0;
                    deltaT = 0;
                    sw.Stop();
                }
            }
            else if (!IsTitleScreen)
            {
                // update player
                player.Update(gameTime, keyboardStateCurrent, keyboardStatePrevious, mouseStateCurrent, mouseStatePrevious,
                              levelInfo.AgentList, levelInfo.Width, levelInfo.Height);

                // begin playing background music
                if (!songStart)
                {
                    MediaPlayer.Play(backgroundMusic);
                    songStart = true;
                }

                // update timer
                if (!stopwatch.IsRunning)
                    stopwatch.Start();
                else
                {
                    double timeRemaining = levelInfo.TimeAllocated - timer;

                    if (timeRemaining > 0)
                    {
                        deltaT = stopwatch.Elapsed.TotalSeconds - timer;
                        timer += deltaT;
                        levelInfo.CheckSongStage(stopwatch);
                    }
                    else
                    {
                        IsTitleScreen = true;
                        titleScreen = 1;
                    }
                }

                // getting all non moving objects
                List<GameAgent> walls = levelInfo.AgentList.Where(a => a.Type == (int)Enums.AgentType.Wall).ToList();

                // updating each moving object
                foreach (GameAgent agent in levelInfo.AgentList.Where(a => a.Type == (int)Enums.AgentType.Enemy).OrderBy(ga => Vector2.Distance(player.Position, ga.Position)).ToList())
                    ((MovingAgent)agent).Update(gameTime, player, walls, levelInfo.LevelNodeSize);

                // getting the updated visible rect
                levelInfo.SetVisibleArea(player.Position, windowWidth, windowHeight);

                #region Code from prev assignments
                // DEBUGGING
                //if (mouseStateCurrent.LeftButton == ButtonState.Pressed && mouseStatePrevious.LeftButton != ButtonState.Pressed)
                //{
                //    if (mouseStateCurrent.X > 0 && mouseStateCurrent.X < windowWidth
                //        && mouseStateCurrent.Y > 0 && mouseStateCurrent.Y < windowHeight)
                //    {
                //        GameAgent wallAgent = levelInfo.AgentList.Where(ga => ga.Bounds.Contains(new Point(mouseStateCurrent.X + levelInfo.VisibleRect.Left, mouseStateCurrent.Y + levelInfo.VisibleRect.Top))).FirstOrDefault();

                //        if (wallAgent != null)
                //        {
                //            debugIndex = levelInfo.AgentList.IndexOf(wallAgent);
                //        }
                //        else
                //        {
                //            debugIndex = -1;
                //        }
                //        //foreach (GameAgent agent in levelInfo.AgentList)
                //        //{
                //        //    if (agent.Bounds.Contains(new Point(mouseStateCurrent.X + levelInfo.VisibleRect.Left, mouseStateCurrent.Y + levelInfo.VisibleRect.Top)))
                //        //    {
                //        //        debugIndex = levelInfo.AgentList.IndexOf(agent);
                //        //        break;
                //        //    }
                //        //}


                //        // UNCOMMENT FOR PART 2) (and make sure to comment PART 3 below)
                //        //if (debugIndex == -1)
                //        //{
                //        //    if (startPos == null)
                //        //    {
                //        //        startPos = new Vector2(mouseStateCurrent.X + levelInfo.VisibleRect.Left, mouseStateCurrent.Y + levelInfo.VisibleRect.Top);
                //        //        Debug.WriteLine("Start Pos: " + startPos);
                //        //        //startPos = new Vector2(436, 2036);
                //        //    }
                //        //    else if (endPos == null)
                //        //    {
                //        //        endPos = new Vector2(mouseStateCurrent.X + levelInfo.VisibleRect.Left, mouseStateCurrent.Y + levelInfo.VisibleRect.Top);
                //        //        //endPos = new Vector2(137, 2013);
                //        //        Vector2 playerPos = player.Position;
                //        //        Debug.WriteLine("End Pos: " + endPos);

                //        //        player.Position = startPos.Value;
                //        //        pathToTarget = player.FindPathToTarget(endPos.Value, levelInfo.AgentList.Where(ga => ga.Type == (int)Enums.AgentType.Wall).ToList(), levelInfo.LevelNodeSize);
                //        //        player.Position = playerPos;
                //        //    }
                //        //}

                //        // UNCOMMENT FOR PART 3) (and make sure to comment PART 2 above)
                //        //if (debugIndex == -1)
                //        //{
                //        //    Vector2 targetPos = new Vector2(mouseStateCurrent.X + levelInfo.VisibleRect.Left, mouseStateCurrent.Y + levelInfo.VisibleRect.Top);

                //        //    foreach (GameAgent agent in levelInfo.AgentList)
                //        //    {
                //        //        if (agent.GetType() == typeof(MovingAgent))
                //        //        {
                //        //            MovingAgent patrol = (MovingAgent)agent;
                //        //            patrol.SetTargetPoint(targetPos, levelInfo.AgentList.Where(ga => ga.Type == (int)Enums.AgentType.Wall).ToList(), levelInfo.LevelNodeSize);
                //        //        }
                //        //    }
                //        //}
                //    }
                //}
                //else if (mouseStateCurrent.LeftButton != ButtonState.Pressed)
                //{
                //    debugIndex = -1;
                //}

                //if (mouseStateCurrent.RightButton == ButtonState.Pressed && mouseStatePrevious.RightButton != ButtonState.Pressed)
                //{
                //    startPos = null;
                //    endPos = null;
                //    pathToTarget = new List<LevelNode>();
                //}
                //// END DEBUGGING
                #endregion
            }
            base.Update(gameTime);
        }

        public void GameOver()
        {
            LevelEndText = "Game Over!";
            IsTitleScreen = true;
            titleScreen = 1;
            //MediaPlayer.Stop();//Play(backgroundMusic);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);       

            spriteBatch.Begin();

            if (IsTitleScreen)
            {
                DrawTitleScreen(gameTime);
            }

            if (isCountdown)
            {
                int ct = (int)countdown;
                spriteBatch.DrawString(font1, "" + ct, new Vector2(windowWidth / 2 - 13, windowHeight / 2 - 150), Color.Green, 0.0f, Vector2.Zero, 2.00f, SpriteEffects.None, 0);
                DrawingHelper.DrawCircle(new Vector2(windowWidth / 2 - 3, windowHeight / 2 - 128), 30, Color.Green, false);
                DrawingHelper.DrawCircle(new Vector2(windowWidth / 2 - 3, windowHeight / 2 - 128), 35, Color.Green, false);
            }

            // draw the custom crosshair
            spriteBatch.Draw(crosshairTexture, new Vector2(mouseStateCurrent.X, mouseStateCurrent.Y),
                null, Color.White, 0.0f, new Vector2(crosshairTexture.Width / 2, crosshairTexture.Height / 2), .5f, SpriteEffects.None, 1.0f);

            foreach (GameAgent agent in levelInfo.AgentList)
                agent.Draw(this.spriteBatch, font1, levelInfo.VisibleRect);

            player.Draw(this.spriteBatch, font1, levelInfo.VisibleRect);

            //DrawDebuggingInformation();

            // draw skill sprites
            //spriteBatch.Draw(skill1Texture, new Rectangle(450, 550, 50, 50), Color.White);
            //spriteBatch.Draw(skill2Texture, new Rectangle(500, 550, 50, 50), Color.White);
            //spriteBatch.Draw(skill4Texture, new Rectangle(600, 550, 50, 50), Color.White);
            

            spriteBatch.End();

            // display the UI
            if (!IsTitleScreen)
            {
                spriteBatch.Begin();
                DrawUI();
                spriteBatch.End();
            }                    

            base.Draw(gameTime);
        }

        private void DrawTitleScreen(GameTime gameTime)
        {
            switch (titleScreen)
            {
                case 0:
                    if (scaleSize < 1 && titleExpanding)
                        scaleSize += (float)gameTime.ElapsedGameTime.TotalSeconds / 50;
                    else
                    {
                        titleExpanding = false;
                        if (scaleSize < 0.95) titleExpanding = true;
                        scaleSize -= (float)gameTime.ElapsedGameTime.TotalSeconds / 50;
                    }
                    spriteBatch.Draw(titleTexture, new Vector2(windowWidth / 2, windowHeight / 2 - 50), null, Color.White, 0.0f, new Vector2(titleTexture.Width / 2, titleTexture.Height/2), scaleSize, SpriteEffects.None, 0.0f);
                    //spriteBatch.DrawString(font1, "Sample Game", new Vector2(windowWidth / 2 - 115, windowHeight / 2 - 100), Color.White, 0.0f, Vector2.Zero, 2.00f, SpriteEffects.None, 0);
                    spriteBatch.DrawString(font1, "Press ENTER to play!", new Vector2(windowWidth / 2 - 85, windowHeight / 2 + 300), Color.White, 0.0f, Vector2.Zero, 1.00f, SpriteEffects.None, 0);
                    break;
                case 1:
                    spriteBatch.DrawString(font1, LevelEndText, new Vector2(windowWidth / 2 - 115, windowHeight / 2 - 100), Color.White, 0.0f, Vector2.Zero, 2.00f, SpriteEffects.None, 0);
                    spriteBatch.DrawString(font1, "You scored: " + player.Score, new Vector2(windowWidth / 2 - 85, windowHeight / 2 + 300), Color.White, 0.0f, Vector2.Zero, 1.00f, SpriteEffects.None, 0);
                    break;
            }
        }

        private void DrawUI()
        {
            spriteBatch.DrawString(font1, "Score: " + player.Score, new Vector2(20, 20), Color.White, 0.0f, Vector2.Zero, 1.00f, SpriteEffects.None, 0);

            // timer
            double timeRemaining = levelInfo.TimeAllocated - timer;
            int seconds = remainingSeconds(timeRemaining);
            spriteBatch.DrawString(font1, "Time Remaining", new Vector2(windowWidth / 2 - 70, 5), Color.White);
            if (isCountdown)
                spriteBatch.DrawString(font1, "4:40", new Vector2(windowWidth / 2 - 17, 30), Color.White);
            else if (timeRemaining <= 0)
                spriteBatch.DrawString(font1, "0:00", new Vector2(windowWidth / 2 - 17, 30), Color.White);
            else if (seconds < 10)
                spriteBatch.DrawString(font1, timeInMinutes(timeRemaining) + ":" + "0" + seconds, new Vector2(windowWidth / 2 - 17, 30), Color.White);
            else
                spriteBatch.DrawString(font1, timeInMinutes(timeRemaining) + ":" + seconds, new Vector2(windowWidth / 2 - 17, 30), Color.White);
           
            // the elapsed timer for debugging purposes
            seconds = remainingSeconds(timer);
            if (seconds < 10)
                spriteBatch.DrawString(font1, "Time Elapsed (for debugging): " + timeInMinutes(timer) + ":" + "0" + seconds, new Vector2(850, 5), Color.White);
            else
                spriteBatch.DrawString(font1, "Time Elapsed (for debugging): " + timeInMinutes(timer) + ":" + seconds, new Vector2(850, 5), Color.White);
            //spriteBatch.DrawString(font1, "Potential Transition Times (approx.)", new Vector2(850, 100), Color.Orange);
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

            // health bar
            Color healthBarColor;
            if (player.Health >= 80)
                healthBarColor = Color.Green;
            else if (player.Health >= 60)
                healthBarColor = Color.YellowGreen;
            else if (player.Health >= 40)
                healthBarColor = Color.Yellow;
            else if (player.Health >= 20)
                healthBarColor = Color.Orange;
            else
                healthBarColor = Color.Red;

            spriteBatch.DrawString(font1, "Health", new Vector2(163, 648), Color.White);
            DrawingHelper.DrawRectangle(new Rectangle(225, 650, (int)(player.MaxHealth * 2), 20), healthBarColor, false);
            DrawingHelper.DrawRectangle(new Rectangle(225, 650, (int)(player.Health * 2), 20), healthBarColor, true);

            // power bar
            spriteBatch.DrawString(font1, "Power", new Vector2(688, 648), Color.White);
            DrawingHelper.DrawRectangle(new Rectangle(750, 650, (int)(player.MaxPower * 2), 20), (player.Power < 0.5) ? Color.MediumPurple : Color.Purple, false);
            DrawingHelper.DrawRectangle(new Rectangle(750, 650, (int)(player.Power * 2), 20), (player.Power < 0.5) ? Color.MediumPurple : Color.Purple, true);

            //Color cooldownColor = new Color(140, 0, 0, 80);

            // skills bar
            //DrawingHelper.DrawRectangle(new Rectangle(450, 600 - (int)player.attackList[2].ActiveCoolDown / 30, 50, (player.attackList[2].ActiveCoolDown > 0) ? (int)player.attackList[2].ActiveCoolDown / 30 : 0), cooldownColor, true);
            //DrawingHelper.DrawRectangle(new Rectangle(450, 550, 49, 50), Color.White, false);

            //DrawingHelper.DrawRectangle(new Rectangle(500, 600 - (int)player.attackList[1].ActiveCoolDown / 20, 50, (player.attackList[1].ActiveCoolDown > 0) ? (int)player.attackList[1].ActiveCoolDown / 20 : 0), cooldownColor, true);
            //DrawingHelper.DrawRectangle(new Rectangle(500, 550, 49, 50), Color.White, false);

            //DrawingHelper.DrawRectangle(new Rectangle(550, 550, 50, 50), (player.Power < 0.5) ? Color.MediumPurple : Color.Purple, false);
            //DrawingHelper.DrawRectangle(new Rectangle(550, 550, 49, 50), Color.White, false);

            //DrawingHelper.DrawRectangle(new Rectangle(600, 600 - (int)player.attackList[3].ActiveCoolDown / 60, 50, (player.attackList[3].ActiveCoolDown > 0) ? (int)player.attackList[3].ActiveCoolDown / 60 : 0), cooldownColor, true);
            //DrawingHelper.DrawRectangle(new Rectangle(600, 550, 49, 50), Color.White, false);
        }

        private int timeInMinutes(double timeRemaining)
        {
            double temp = timeRemaining;
            int minutes = 0;

            if (temp > 60)
            {
                minutes = (int)(temp / 60);
            }
            return minutes;
        }

        private int remainingSeconds(double timeRemaining)
        {
            double temp = timeRemaining;
            return ((int)(temp % 60));
        }

        private void ShowLevelGrid()
        {
            for (int j = 0; j < (levelInfo.Height / levelInfo.LevelNodeSize); j++)
                for (int i = 0; i < (levelInfo.Width / levelInfo.LevelNodeSize); i++)
                {
                    Rectangle rect = new Rectangle(i * levelInfo.LevelNodeSize, j * levelInfo.LevelNodeSize, levelInfo.LevelNodeSize, levelInfo.LevelNodeSize);

                    if (levelInfo.AgentList.Where(ga => ga.Type == (int)Enums.AgentType.Wall && ga.Bounds.Intersects(rect)).Count() < 1
                        && levelInfo.VisibleRect.Intersects(rect))
                    {
                        DrawingHelper.DrawFastLine
                        (
                            new Vector2(rect.Left - levelInfo.VisibleRect.Left, rect.Top - levelInfo.VisibleRect.Top),
                            new Vector2(rect.Left - levelInfo.VisibleRect.Left, rect.Top - levelInfo.VisibleRect.Top + levelInfo.LevelNodeSize),
                            Color.Brown
                        );

                        DrawingHelper.DrawFastLine
                        (
                            new Vector2(rect.Left - levelInfo.VisibleRect.Left, rect.Top - levelInfo.VisibleRect.Top),
                            new Vector2(rect.Left - levelInfo.VisibleRect.Left + levelInfo.LevelNodeSize, rect.Top - levelInfo.VisibleRect.Top),
                            Color.Brown
                        );

                        spriteBatch.DrawString(font1, j + "" + i, new Vector2(rect.Center.X - levelInfo.VisibleRect.Left - 15, rect.Center.Y - levelInfo.VisibleRect.Top), Color.Brown, 0.0f, Vector2.Zero, 0.75f, SpriteEffects.None, 0);
                    }
                }
        }

        private void DrawDebuggingInformation()
        {
            Vector2 playerHeading = Utils.CalculateRotatedMovement(new Vector2(1, 0), player.Rotation);

            spriteBatch.DrawString(font1, "Player Pos: " + Math.Round(player.Position.X, 4) + ", " + Math.Round(player.Position.Y, 4), new Vector2(20, 20), Color.LightGreen, 0.0f, Vector2.Zero, 0.75f, SpriteEffects.None, 0);
            spriteBatch.DrawString(font1, "Player Heading: " + Math.Round(playerHeading.X, 4) + ", " + Math.Round(playerHeading.Y, 4) + ", " + Math.Round(player.Rotation, 4), new Vector2(20, 40), Color.DarkKhaki, 0.0f, Vector2.Zero, 0.75f, SpriteEffects.None, 0);
            spriteBatch.DrawString(font1, "Player Health: " + player.Health , new Vector2(20, 60), Color.DarkKhaki, 0.0f, Vector2.Zero, 0.75f, SpriteEffects.None, 0);
            
            //spriteBatch.DrawString(font1, "Press H to show navigation nodes.", new Vector2(20, 60), Color.DarkKhaki, 0.0f, Vector2.Zero, 0.75f, SpriteEffects.None, 0);
            //spriteBatch.DrawString(font1, "Left: " + levelInfo.VisibleRect.Left + ", Top: " + levelInfo.VisibleRect.Top + ", Right: " + levelInfo.VisibleRect.Right + ", Bottom: " + levelInfo.VisibleRect.Bottom, new Vector2(20, 60), Color.DarkKhaki, 0.0f, Vector2.Zero, 0.75f, SpriteEffects.None, 0);
            
            //if (debugIndex > -1)
            //{
            //    GameAgent agent = levelInfo.AgentList[debugIndex];

            //    spriteBatch.DrawString(font1, agent.GetAgentInfo(), new Vector2(20, 100), Color.DarkKhaki, 0.0f, Vector2.Zero, 0.75f, SpriteEffects.None, 0);
            //}

            //if (pathToTarget.Count == 1)
            //{
            //    Vector2 startVec = new Vector2(startPos.Value.X - levelInfo.VisibleRect.Left, startPos.Value.Y - levelInfo.VisibleRect.Top);
            //    Vector2 endVec = new Vector2(endPos.Value.X - levelInfo.VisibleRect.Left, endPos.Value.Y - levelInfo.VisibleRect.Top);

            //    DrawingHelper.DrawFastLine
            //    (
            //        startVec,
            //        endVec,
            //        Color.White
            //    );
            //}
            //else if (pathToTarget.Count > 0)
            //{
            //    for (int i = 0; i < pathToTarget.Count + 1; i++)
            //    {
            //        Vector2 startVec = i == 0 ? new Vector2(startPos.Value.X - levelInfo.VisibleRect.Left, startPos.Value.Y - levelInfo.VisibleRect.Top) : new Vector2(pathToTarget[i - 1].Bounds.Center.X - levelInfo.VisibleRect.Left, pathToTarget[i - 1].Bounds.Center.Y - levelInfo.VisibleRect.Top);
            //        Vector2 endVec = i == pathToTarget.Count ? new Vector2(endPos.Value.X - levelInfo.VisibleRect.Left, endPos.Value.Y - levelInfo.VisibleRect.Top) : new Vector2(pathToTarget[i].Bounds.Center.X - levelInfo.VisibleRect.Left, pathToTarget[i].Bounds.Center.Y - levelInfo.VisibleRect.Top);

            //        DrawingHelper.DrawFastLine
            //        (
            //            startVec,
            //            endVec,
            //            Color.White
            //        );

            //        if (i < pathToTarget.Count)
            //            spriteBatch.Draw(debugNodeTexture, new Vector2(pathToTarget[i].Bounds.Center.X - levelInfo.VisibleRect.Left - 5, pathToTarget[i].Bounds.Center.Y - levelInfo.VisibleRect.Top - 5), Color.White);
            //    }
            //}
            //else
            //{
            //    foreach (GameAgent agent in levelInfo.AgentList)
            //    {
            //        if (agent.GetType() == typeof(MovingAgent))
            //        {
            //            MovingAgent movingAgent = (MovingAgent)agent;

            //            if (movingAgent.State == Enums.EnemyState.RunningToPoint)
            //            {
            //                List<LevelNode> levelNodeList = movingAgent.PathToTarget;
            //                int index = movingAgent.TargetIndex;

            //                for (int i = index; i < levelNodeList.Count + 1; i++)
            //                {
            //                    Vector2 startVec = i == 0 ? new Vector2(movingAgent.PlaceBeforeFollow.X - levelInfo.VisibleRect.Left, movingAgent.PlaceBeforeFollow.Y - levelInfo.VisibleRect.Top) : new Vector2(levelNodeList[i - 1].Bounds.Center.X - levelInfo.VisibleRect.Left, levelNodeList[i - 1].Bounds.Center.Y - levelInfo.VisibleRect.Top);
            //                    Vector2 endVec = i == levelNodeList.Count ? new Vector2(movingAgent.TargetPosition.X - levelInfo.VisibleRect.Left, movingAgent.TargetPosition.Y - levelInfo.VisibleRect.Top) : new Vector2(levelNodeList[i].Bounds.Center.X - levelInfo.VisibleRect.Left, levelNodeList[i].Bounds.Center.Y - levelInfo.VisibleRect.Top);

            //                    DrawingHelper.DrawFastLine
            //                    (
            //                        startVec,
            //                        endVec,
            //                        Color.White
            //                    );

            //                    if (i < levelNodeList.Count)
            //                        spriteBatch.Draw(debugNodeTexture, new Vector2(levelNodeList[i].Bounds.Center.X - levelInfo.VisibleRect.Left - 5, levelNodeList[i].Bounds.Center.Y - levelInfo.VisibleRect.Top - 5), Color.White);
            //                }
            //            }
            //        }
            //    }
            //}
        }
    }
}