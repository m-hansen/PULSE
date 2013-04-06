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
        MouseState mouseStateCurrent;
        MouseState mouseStatePrevious;
        Texture2D crosshairTexture;
         
        Vector2? startPos = null;
        Vector2? endPos = null;
        List<LevelNode> pathToTarget = new List<LevelNode>();
        Texture2D debugNodeTexture;

        public EffectComponent EffectComponent;

        int debugIndex = -1;
        
        int windowWidth = 0;
        int windowHeight = 0;

        public static Game1 Current { get; private set; }
        public Player player;
        public LevelInfo levelInfo = new LevelInfo();
        public SpriteFont font1;

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

            player = new Player();
            player.AnimationInterval = TimeSpan.FromMilliseconds(100);          // next frame every 100 miliseconds
            player.RotationSpeed = 6.0f;                                        // rotate somewhat quick
            player.Speed = 4.0f;                                                // setting forward - backward speed
            player.InitializeSensors();                                         // initializes all sensors for the player object
            player.Health = 1000;

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

            // loading the player's image
            player.LoadContent(this.Content, "Images\\ship1", new Rectangle(0, 0, 38, 41), 8);

            // loading the font to display text on the screen
            font1 = Content.Load<SpriteFont>("fonts/Font1");

            // load the custom crosshair
            crosshairTexture = Content.Load<Texture2D>("Images\\crosshair");

            Attack attack1 = new Attack();
            attack1.Active = true;
            attack1.Key = Keys.Space;
            attack1.AttackType = (int)Enums.AttackType.Bullet;
            attack1.AttackSubType = (int)Enums.AttackSubType.TriBullet;
            attack1.CoolDown = 50;
            attack1.Texture = Content.Load<Texture2D>("Images\\raindrop");
            attack1.Frames = 1;
            attack1.MinDamage = 10;
            attack1.MaxDamage = 15;
            player.attackList.Add(attack1); 

            Attack attack2 = new Attack();
            attack2.Active = true;
            attack2.Key = Keys.D2;
            attack2.AttackType = (int)Enums.AttackType.Explosion;
            attack2.AttackSubType = (int)Enums.AttackSubType.Default;
            attack2.CoolDown = 1000;
            attack2.Texture = Content.Load<Texture2D>("Images\\explosion1");
            attack2.Frames = 6;
            attack2.BoundingRect = new Rectangle(0, 0, 139, 107);
            player.attackList.Add(attack2);

            levelInfo.LoadLevel(0, this.Content, windowWidth, windowHeight);

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

            debugNodeTexture = this.Content.Load<Texture2D>("Images\\10_by_10");

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

            player.Update(gameTime, keyboardStateCurrent, keyboardStatePrevious, mouseStateCurrent, mouseStatePrevious,
                          levelInfo.AgentList, levelInfo.Width, levelInfo.Height);

            // getting all non moving objects
            List<GameAgent> walls = levelInfo.AgentList.Where(a => a.Type == (int)Enums.AgentType.Wall).ToList();

            // updating each moving object
            foreach (GameAgent agent in levelInfo.AgentList.Where(a => a.Type != (int)Enums.AgentType.Wall).ToList())
                    ((MovingAgent)agent).Update(gameTime, player, walls, levelInfo.LevelNodeSize);

            // getting the updated visible rect
            levelInfo.SetVisibleArea(player.Position, windowWidth, windowHeight);

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

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);       

            spriteBatch.Begin();

            //ShowLevelGrid();

            // draw the custom crosshair
            spriteBatch.Draw(crosshairTexture, new Vector2(mouseStateCurrent.X, mouseStateCurrent.Y), 
                null, Color.White, 0.0f, new Vector2(crosshairTexture.Width / 2, crosshairTexture.Height / 2), .5f, SpriteEffects.None, 1.0f);

            foreach (GameAgent agent in levelInfo.AgentList)
                agent.Draw(this.spriteBatch, font1, levelInfo.VisibleRect);

            player.Draw(this.spriteBatch, font1, levelInfo.VisibleRect);

            DrawDebuggingInformation();

            spriteBatch.End();                          

            base.Draw(gameTime);
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
            spriteBatch.DrawString(font1, "Player Heading: " + Math.Round(playerHeading.X, 4) + ", " + Math.Round(playerHeading.Y, 4), new Vector2(20, 40), Color.DarkKhaki, 0.0f, Vector2.Zero, 0.75f, SpriteEffects.None, 0);
            spriteBatch.DrawString(font1, "Player Health: " + player.Health , new Vector2(20, 60), Color.DarkKhaki, 0.0f, Vector2.Zero, 0.75f, SpriteEffects.None, 0);
            
            //spriteBatch.DrawString(font1, "Press H to show navigation nodes.", new Vector2(20, 60), Color.DarkKhaki, 0.0f, Vector2.Zero, 0.75f, SpriteEffects.None, 0);
            //spriteBatch.DrawString(font1, "Left: " + levelInfo.VisibleRect.Left + ", Top: " + levelInfo.VisibleRect.Top + ", Right: " + levelInfo.VisibleRect.Right + ", Bottom: " + levelInfo.VisibleRect.Bottom, new Vector2(20, 60), Color.DarkKhaki, 0.0f, Vector2.Zero, 0.75f, SpriteEffects.None, 0);
            
            if (debugIndex > -1)
            {
                GameAgent agent = levelInfo.AgentList[debugIndex];

                spriteBatch.DrawString(font1, agent.GetAgentInfo(), new Vector2(20, 100), Color.DarkKhaki, 0.0f, Vector2.Zero, 0.75f, SpriteEffects.None, 0);
            }

            if (pathToTarget.Count == 1)
            {
                Vector2 startVec = new Vector2(startPos.Value.X - levelInfo.VisibleRect.Left, startPos.Value.Y - levelInfo.VisibleRect.Top);
                Vector2 endVec = new Vector2(endPos.Value.X - levelInfo.VisibleRect.Left, endPos.Value.Y - levelInfo.VisibleRect.Top);

                DrawingHelper.DrawFastLine
                (
                    startVec,
                    endVec,
                    Color.White
                );
            }
            else if (pathToTarget.Count > 0)
            {
                for (int i = 0; i < pathToTarget.Count + 1; i++)
                {
                    Vector2 startVec = i == 0 ? new Vector2(startPos.Value.X - levelInfo.VisibleRect.Left, startPos.Value.Y - levelInfo.VisibleRect.Top) : new Vector2(pathToTarget[i - 1].Bounds.Center.X - levelInfo.VisibleRect.Left, pathToTarget[i - 1].Bounds.Center.Y - levelInfo.VisibleRect.Top);
                    Vector2 endVec = i == pathToTarget.Count ? new Vector2(endPos.Value.X - levelInfo.VisibleRect.Left, endPos.Value.Y - levelInfo.VisibleRect.Top) : new Vector2(pathToTarget[i].Bounds.Center.X - levelInfo.VisibleRect.Left, pathToTarget[i].Bounds.Center.Y - levelInfo.VisibleRect.Top);

                    DrawingHelper.DrawFastLine
                    (
                        startVec,
                        endVec,
                        Color.White
                    );

                    if (i < pathToTarget.Count)
                        spriteBatch.Draw(debugNodeTexture, new Vector2(pathToTarget[i].Bounds.Center.X - levelInfo.VisibleRect.Left - 5, pathToTarget[i].Bounds.Center.Y - levelInfo.VisibleRect.Top - 5), Color.White);
                }
            }
            else
            {
                foreach (GameAgent agent in levelInfo.AgentList)
                {
                    if (agent.GetType() == typeof(MovingAgent))
                    {
                        MovingAgent movingAgent = (MovingAgent)agent;

                        if (movingAgent.State == (int)Enums.MovingAgentState.RunningToPoint)
                        {
                            List<LevelNode> levelNodeList = movingAgent.PathToTarget;
                            int index = movingAgent.TargetIndex;

                            for (int i = index; i < levelNodeList.Count + 1; i++)
                            {
                                Vector2 startVec = i == 0 ? new Vector2(movingAgent.PlaceBeforeFollow.X - levelInfo.VisibleRect.Left, movingAgent.PlaceBeforeFollow.Y - levelInfo.VisibleRect.Top) : new Vector2(levelNodeList[i - 1].Bounds.Center.X - levelInfo.VisibleRect.Left, levelNodeList[i - 1].Bounds.Center.Y - levelInfo.VisibleRect.Top);
                                Vector2 endVec = i == levelNodeList.Count ? new Vector2(movingAgent.TargetPosition.X - levelInfo.VisibleRect.Left, movingAgent.TargetPosition.Y - levelInfo.VisibleRect.Top) : new Vector2(levelNodeList[i].Bounds.Center.X - levelInfo.VisibleRect.Left, levelNodeList[i].Bounds.Center.Y - levelInfo.VisibleRect.Top);

                                DrawingHelper.DrawFastLine
                                (
                                    startVec,
                                    endVec,
                                    Color.White
                                );

                                if (i < levelNodeList.Count)
                                    spriteBatch.Draw(debugNodeTexture, new Vector2(levelNodeList[i].Bounds.Center.X - levelInfo.VisibleRect.Left - 5, levelNodeList[i].Bounds.Center.Y - levelInfo.VisibleRect.Top - 5), Color.White);
                            }
                        }
                    }
                }
            }
        }
    }
}