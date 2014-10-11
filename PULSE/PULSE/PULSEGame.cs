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
using PulseGame.Helpers;
using PulseGame.Effects;
using PulseGame.Attacks;  // DrawingHelper namespace

namespace PulseGame
{
    public class PulseGame : Microsoft.Xna.Framework.Game
    {
        public int gameState;
        public MouseState mouseStateCurrent;
        public EffectComponent effectComponent;
        public static PulseGame Current { get; private set; }
        public Player player;
        public LevelInfo levelInfo = new LevelInfo();
        public SpriteFont font1;
        public SoundEffect playerHitSound;
        public SoundEffect berserkerSound;
        public string levelEndText;

        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        private Stopwatch sw;
        private Stopwatch stopwatch;    // for time elapsed

        private KeyboardState keyboardStateCurrent;
        private KeyboardState keyboardStatePrevious;
        private MouseState mouseStatePrevious;

        private Texture2D crosshairTexture;
        private Texture2D titleTexture;
        private Texture2D subtitleTexture;
        private Texture2D skillLockTexture;

        private Song titleMusic;
        private Song backgroundMusic;
        private SoundEffect countdownSound;

        private bool songStart = false;
        private bool titleExpanding = false;

        private float scaleSize = 1.0f;

        private double countdown = 4;
        private double timer;           // for time elapsed
        private double deltaT;          // for time elapsed

        private bool isDebugging = false;

        private HighScoreTable highScores;

        private List<LevelNode> pathToTarget = new List<LevelNode>();

        private int windowWidth = 0;
        private int windowHeight = 0;

        public PulseGame()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferHeight = 700;    // height of screen
            graphics.PreferredBackBufferWidth = 1200;    // width of screen
            graphics.IsFullScreen = false;
            graphics.ApplyChanges();
            Content.RootDirectory = "Content";

            windowWidth = Window.ClientBounds.Width;
            windowHeight = Window.ClientBounds.Height;

            Components.Add(effectComponent = new EffectComponent(this));

            Current = this;
        }

        protected override void Initialize()
        {
            // Initialize the input devices
            this.IsMouseVisible = false;
            keyboardStateCurrent = new KeyboardState();
            mouseStateCurrent = new MouseState();

            gameState = (int)Enums.GameState.Attract;

            // Initialize DrawingHelper
            DrawingHelper.Initialize(GraphicsDevice);

            // Initialize stopwatch for consistent times across machines
            sw = new Stopwatch();
            stopwatch = new Stopwatch();

            player = new Player();
            player.AnimationInterval = TimeSpan.FromMilliseconds(100);          // next frame every 100 miliseconds
            player.RotationSpeed = 6.0f;                                        // rotate somewhat quick
            player.Speed = 4.0f;                                                // setting forward - backward speed
            player.Health = 100;
            player.HasControl = true;

            highScores = new HighScoreTable();
            levelEndText = "";

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // load in the title music
            titleMusic = Content.Load<Song>("Audio\\voxis_shattered");
            MediaPlayer.IsRepeating = false;    // everything is set up for sound to loop - just change this value

            // load in the background music
            backgroundMusic = Content.Load<Song>("Audio\\voxis_pour_elle");
            MediaPlayer.IsRepeating = false;    // everything is set up for sound to loop - just change this value

            // loading the player's image
            player.LoadContent(this.Content, "Images\\ship1", new Rectangle(0, 0, 38, 41), 8);

            // loading the font to display text on the screen
            font1 = Content.Load<SpriteFont>("fonts\\Font1");

            // load sounds
            playerHitSound = Content.Load<SoundEffect>("Audio\\player_hit");
            countdownSound = Content.Load<SoundEffect>("Audio\\beep");
            berserkerSound = Content.Load<SoundEffect>("Audio\\whistle-flute-1");

            // load the custom crosshair
            crosshairTexture = Content.Load<Texture2D>("Images\\crosshair");

            // load the title texture
            titleTexture = Content.Load<Texture2D>("Images\\PULSE");
            subtitleTexture = Content.Load<Texture2D>("Images\\enter_to_play");

            // load the skill lock texture
            skillLockTexture = Content.Load<Texture2D>("Images\\skill_lock");

            levelInfo.LoadLevel(0, this.Content, windowWidth, windowHeight);

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
            {
                this.Exit();
            }

            // Display debugging info
            if (keyboardStateCurrent.IsKeyDown(Keys.Tab) && keyboardStatePrevious.IsKeyUp(Keys.Tab))
            {
                isDebugging = !isDebugging;
            }

            // Update keyboard state
            keyboardStatePrevious = keyboardStateCurrent;
            keyboardStateCurrent = Keyboard.GetState();

            // Update mouse state
            mouseStatePrevious = mouseStateCurrent;
            mouseStateCurrent = Mouse.GetState();

            switch (gameState)
            {
                case (int)Enums.GameState.Attract:
                    // Play the title music
                    if (!songStart)
                    {
                        MediaPlayer.Play(titleMusic);
                        songStart = true;
                    }
                    if ((keyboardStateCurrent.IsKeyDown(Keys.Enter) && keyboardStatePrevious.IsKeyUp(Keys.Enter)) ||
                        (mouseStateCurrent.LeftButton == ButtonState.Pressed && mouseStatePrevious.LeftButton == ButtonState.Released))
                    {
                        MediaPlayer.Stop();
                        songStart = false;
                        gameState = (int)Enums.GameState.Countdown;
                        sw.Start();
                    }
                    break;

                case (int)Enums.GameState.Countdown:
                    if ((countdown > 2.99 && countdown < 3.01) ||
                        (countdown > 1.99 && countdown < 2.01) ||
                        (countdown > 0.99 && countdown < 1.01))
                    {
                        countdownSound.Play();
                    }

                    deltaT = sw.Elapsed.TotalSeconds - timer;
                    timer += deltaT;
                    countdown -= deltaT;

                    if (countdown <= 0)
                    {
                        gameState = (int)Enums.GameState.Gameplay;
                        timer = 0;
                        deltaT = 0;
                        sw.Stop();
                        // begin playing background music
                        if (!songStart)
                        {
                            MediaPlayer.Play(backgroundMusic);
                            songStart = true;
                        }
                    }
                    break;

                case (int)Enums.GameState.Gameplay:
                    // update player
                    if (player.Health <= 0)
                    {
                        GameOver("Game Over!");
                        break;
                    }

                    player.Update(gameTime, keyboardStateCurrent, keyboardStatePrevious, mouseStateCurrent, mouseStatePrevious,
                                  levelInfo.AgentList, levelInfo.Width, levelInfo.Height);

                    // update timer
                    if (!stopwatch.IsRunning)
                    {
                        stopwatch.Start();
                    }
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
                            GameOver("Congratulations!");
                        }
                    }

                    // getting all non moving objects
                    List<GameAgent> walls = levelInfo.AgentList.Where(a => a.Type == (int)Enums.AgentType.Wall).ToList();

                    // updating each moving object
                    foreach (GameAgent agent in levelInfo.AgentList.Where(a => a.Type == (int)Enums.AgentType.Enemy).OrderBy(ga => Vector2.Distance(player.Position, ga.Position)).ToList())
                    {
                        ((MovingAgent)agent).Update(gameTime, player, walls, levelInfo.LevelNodeSize);
                    }

                    // getting the updated visible rect
                    levelInfo.SetVisibleArea(player.Position, windowWidth, windowHeight);
                    break;

                case (int)Enums.GameState.GameOver:
                    if ((keyboardStateCurrent.IsKeyDown(Keys.Enter) && keyboardStatePrevious.IsKeyUp(Keys.Enter)) || 
                        (mouseStateCurrent.LeftButton == ButtonState.Pressed && mouseStatePrevious.LeftButton == ButtonState.Released))
                    {
                        RestartGame();
                    }
                    break;
            }

            base.Update(gameTime);
        }

        public void GameOver(string endText)
        {
            gameState = (int)Enums.GameState.GameOver;
            levelEndText = endText;
            highScores.LoadTable();
            highScores.AddScoreToTable(player.Score);
        }

        public void RestartGame()
        {
            MediaPlayer.Stop();

            // Init vars
            player.Initialize();

            Statistics.Clear();

            timer = 0;
            deltaT = 0;
            countdown = 4;
            sw = new Stopwatch();
            stopwatch = new Stopwatch();

            effectComponent.effectList.Clear();

            levelInfo.Initialize();
            levelInfo.LoadLevel(0, this.Content, windowWidth, windowHeight);

            player.Position = levelInfo.PlayerStartPos;
            Mouse.SetPosition(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);

            songStart = false;

            gameState = (int)Enums.GameState.Attract;
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();

            if (gameState == (int)Enums.GameState.Attract)
            {
                DrawTitleScreen(gameTime);
            }

            if (gameState == (int)Enums.GameState.GameOver)
            {
                DrawGameOverScreeen(gameTime);
            }

            if (gameState == (int)Enums.GameState.Countdown)
            {
                int ct = (int)countdown;
                spriteBatch.DrawString(font1, "" + ct, new Vector2(windowWidth / 2 - 13, windowHeight / 2 - 150), Color.Green, 0.0f, Vector2.Zero, 2.00f, SpriteEffects.None, 0);
                DrawingHelper.DrawCircle(new Vector2(windowWidth / 2 - 3, windowHeight / 2 - 128), 30, Color.Green, false);
                DrawingHelper.DrawCircle(new Vector2(windowWidth / 2 - 3, windowHeight / 2 - 128), 35, Color.Green, false);
            }

            // draw the custom crosshair
            spriteBatch.Draw(crosshairTexture, new Vector2(mouseStateCurrent.X, mouseStateCurrent.Y),
                null, Color.White, 0.0f, new Vector2(crosshairTexture.Width / 2, crosshairTexture.Height / 2), .5f, SpriteEffects.None, 1.0f);

            if (gameState != (int)Enums.GameState.Attract)
            {
                // draw each agent
                foreach (GameAgent agent in levelInfo.AgentList)
                {
                    agent.Draw(this.spriteBatch, font1, levelInfo.VisibleRect);
                }

                // draw the player
                player.Draw(this.spriteBatch, font1, levelInfo.VisibleRect);
            }

            if (isDebugging)
                DrawDebuggingInformation();

            spriteBatch.End();


            if (gameState != (int)Enums.GameState.Attract)
            {
                // display the UI
                spriteBatch.Begin();
                DrawUI();
                spriteBatch.End();
            }

            base.Draw(gameTime);
        }

        private void DrawTitleScreen(GameTime gameTime)
        {
            if (scaleSize < 1 && titleExpanding)
            {
                scaleSize += (float)gameTime.ElapsedGameTime.TotalSeconds / 50;
            }
            else
            {
                titleExpanding = false;
                if (scaleSize < 0.95) titleExpanding = true;
                scaleSize -= (float)gameTime.ElapsedGameTime.TotalSeconds / 50;
            }

            spriteBatch.Draw(titleTexture, new Vector2(windowWidth / 2, windowHeight / 2 - 50), null, Color.White, 0.0f, new Vector2(titleTexture.Width / 2, titleTexture.Height / 2), scaleSize, SpriteEffects.None, 0.0f);
            spriteBatch.Draw(subtitleTexture, new Vector2(windowWidth / 2, windowHeight - 50), null, Color.White, 0.0f, new Vector2(subtitleTexture.Width / 2, subtitleTexture.Height / 2), 0.75f, SpriteEffects.None, 0.0f);
        }

        private void DrawGameOverScreeen(GameTime gameTime)
        {
            spriteBatch.DrawString(font1, levelEndText, new Vector2(windowWidth / 2 - 115, windowHeight / 2 - 100), Color.White, 0.0f, Vector2.Zero, 2.00f, SpriteEffects.None, 0);
            spriteBatch.DrawString(font1, "You scored: " + player.Score, new Vector2(windowWidth / 2 - 85, windowHeight / 2 + 300), Color.White, 0.0f, Vector2.Zero, 1.00f, SpriteEffects.None, 0);

            // Draw high scores table
            var scores = highScores.GetHighScores();
            spriteBatch.DrawString(font1, "High Scores", new Vector2(windowWidth / 2, windowHeight / 2 - 30), Color.White, 0.0f, Vector2.Zero, 1.25f, SpriteEffects.None, 0);
            for (int i = 0; i < scores.Length; i++)
            {
                spriteBatch.DrawString(font1, (i+1).ToString() +  "    " + scores[i].playerName + "    " + scores[i].score, 
                    new Vector2(windowWidth / 2, windowHeight / 2 + (i * 20)), Color.White, 0.0f, Vector2.Zero, 1.00f, SpriteEffects.None, 0);
            }
        }

        private void DrawUI()
        {
            // lower GUI background color
            DrawingHelper.DrawRectangle(new Rectangle(0, windowHeight - (windowHeight / 10), windowWidth, windowHeight / 10), new Color(20, 20, 20, 255), true);

            // timer box
            DrawingHelper.DrawRectangle(new Rectangle((windowWidth / 2) - (windowWidth / 10), 0, windowWidth / 5, windowHeight / 10), new Color(20, 20, 20, 20), true);

            spriteBatch.DrawString(font1, "Score: " + player.Score, new Vector2(20, 20), Color.White, 0.0f, Vector2.Zero, 1.00f, SpriteEffects.None, 0);

            // timer
            double timeRemaining = levelInfo.TimeAllocated - timer;
            int seconds = remainingSeconds(timeRemaining);
            spriteBatch.DrawString(font1, "Time Remaining", new Vector2(windowWidth / 2 - 70, 5), Color.White);
            if (gameState == (int)Enums.GameState.Countdown)
                spriteBatch.DrawString(font1, "4:40", new Vector2(windowWidth / 2 - 17, 30), Color.White);
            else if (timeRemaining <= 0)
                spriteBatch.DrawString(font1, "0:00", new Vector2(windowWidth / 2 - 17, 30), Color.White);
            else if (seconds < 10)
                spriteBatch.DrawString(font1, timeInMinutes(timeRemaining) + ":" + "0" + seconds, new Vector2(windowWidth / 2 - 17, 30), Color.White);
            else
                spriteBatch.DrawString(font1, timeInMinutes(timeRemaining) + ":" + seconds, new Vector2(windowWidth / 2 - 17, 30), Color.White);

            // health bar
            Color healthBarColor;
            if (player.Health > 50)
                healthBarColor = Color.Green;
            else if (player.Health > 20)
                healthBarColor = Color.Yellow;
            else
                healthBarColor = Color.Red;
            /*if (player.Health >= 80)
                healthBarColor = Color.Green;
            else if (player.Health >= 60)
                healthBarColor = Color.YellowGreen;
            else if (player.Health >= 40)
                healthBarColor = Color.Yellow;
            else if (player.Health >= 20)
                healthBarColor = Color.Orange;
            else
                healthBarColor = Color.Red;*/
            spriteBatch.DrawString(font1, "Health", new Vector2(163, 648), Color.White);
            DrawingHelper.DrawRectangle(new Rectangle(225, 650, (int)(player.Health * 2), 20), healthBarColor, true);
            DrawingHelper.DrawRectangle(new Rectangle(225, 650, (int)(player.MaxHealth * 2), 20), Color.Black, false);

            // power bar
            spriteBatch.DrawString(font1, "Power", new Vector2(688, 648), Color.White);
            DrawingHelper.DrawRectangle(new Rectangle(750, 650, (int)(player.Power * 2), 20), (player.Power < 0.5) ? Color.MediumPurple : Color.Purple, true);
            DrawingHelper.DrawRectangle(new Rectangle(750, 650, (int)(player.MaxPower * 2), 20), Color.Black, false);

            // Skill bar
            int uiHeight = windowHeight / 10; // 10% of window height
            int numSkills = player.skillList.Length;
            int skillBoxWidth = 52;
            int skillBoxHeight = 52;
            int padding = 1;
            Point skillBarPos = new Point((windowWidth / 2) - (numSkills * (padding + skillBoxWidth) / 2), windowHeight - uiHeight + ((uiHeight - (padding + skillBoxHeight)) / 2));
            Color cooldownColor = new Color(140, 0, 0, 80);
            int skillNumber = 0;
            foreach (Attack attack in player.skillList.Where(a => a.HasIcon).ToList())
            {
                skillNumber++;

                // draws the attack icon texture
                spriteBatch.Draw(attack.IsUnlocked ? attack.IconTexture : skillLockTexture, new Rectangle(skillBarPos.X + padding, skillBarPos.Y + padding, skillBoxWidth - (2 * padding), skillBoxHeight - (2 * padding)), Color.White);
                spriteBatch.DrawString(font1, skillNumber.ToString(), new Vector2(skillBarPos.X + padding + 3, skillBarPos.Y + padding), Color.LemonChiffon);

                // draws the cooldown rect
                if (attack.ActiveCoolDown > 0)
                {
                    Rectangle targetRect = new Rectangle
                    (
                        skillBarPos.X + padding,
                        skillBarPos.Y + padding + skillBoxHeight - (skillBoxHeight * attack.ActiveCoolDown / attack.CoolDown),
                        skillBoxWidth - 2 * padding,
                        (skillBoxHeight * attack.ActiveCoolDown / attack.CoolDown) - 2 * padding
                    );

                    DrawingHelper.DrawRectangle(targetRect, cooldownColor, true);
                }

                // draws the outline box
                DrawingHelper.DrawRectangle(new Rectangle(skillBarPos.X, skillBarPos.Y, skillBoxWidth, skillBoxHeight), Color.Black, false);

                skillBarPos.X += skillBoxWidth;
            }
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
            spriteBatch.DrawString(font1, "Player Health: " + player.Health, new Vector2(20, 60), Color.DarkKhaki, 0.0f, Vector2.Zero, 0.75f, SpriteEffects.None, 0);

            // stats
            spriteBatch.DrawString(font1, "Shots Fired: " + Statistics.GetStat((int)StatType.SHOT), new Vector2(20, 100), Color.DarkKhaki, 0.0f, Vector2.Zero, 0.75f, SpriteEffects.None, 0);
            spriteBatch.DrawString(font1, "Enemies Destroyed: " + Statistics.GetStat((int)StatType.ENEMY_KILL), new Vector2(20, 120), Color.DarkKhaki, 0.0f, Vector2.Zero, 0.75f, SpriteEffects.None, 0);
        }
    }
}