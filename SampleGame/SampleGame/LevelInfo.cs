using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

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
       // public List<LevelNode> WalkableNodes = new List<LevelNode>();

        public LevelInfo()
        {

        }

        #region Get Visible Area

        public void SetVisibleArea(Vector2 playerPos, int windowWidth, int windowHeight)
        {
            VisibleRect = new Rectangle(
                GetVisibleRectX(playerPos, windowWidth, windowHeight),
                GetVisibleRectY(playerPos, windowWidth, windowHeight),
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

            //PlayerStartPos = new Vector2(1850, 3000);
            PlayerStartPos = new Vector2(400, 300);

            //MovingAgent pAgent1 = new MovingAgent();
            //pAgent1.LoadContent(content, "Images\\explosion1", new Rectangle(0, 0, 139, 107), 6);
            //pAgent1.AnimationInterval = new TimeSpan(800000);
            //pAgent1.Position = new Vector2(200, 200);//1150, 2650);
            //pAgent1.Rotation = 0.0f;
            //pAgent1.Type = (int)Enums.AgentType.NPC;
            //pAgent1.State = 200;//(int)Enums.MovingAgentState.Patrolling;
            ////pAgent1.PathList = new List<Vector2>();
            ////pAgent1.PathList.Add(new Vector2(50, 50));
            ////pAgent1.PathList.Add(new Vector2(100, 100));
            //pAgent1.MaxSpeed = 1;
            //pAgent1.TimeAtEachPoint = 50;
            //pAgent1.NPCRange = 100;
            //pAgent1.MaxFollowRange = 9000;
            //pAgent1.MeleeDistance = 1;
            //AgentList.Add(pAgent1);

            //pAgent1.SensorList.Add(new RangeFinder()
            //{
            //    Type = (int)Enums.SensorType.RangeFinder,
            //    Rotation = (float)Math.PI / 4,
            //    Key = null,
            //    MaxDistance = 30,//150,
            //    Active = true,
            //    Index = 0,
            //    DirectionText = "Right"
            //});

            //pAgent1.SensorList.Add(new RangeFinder()
            //{
            //    Type = (int)Enums.SensorType.RangeFinder,
            //    Rotation = (float)Math.PI / 2,
            //    Key = null,
            //    Active = true,
            //    MaxDistance = 25,//150,
            //    Index = 1,
            //    DirectionText = ""
            //});

            //pAgent1.SensorList.Add(new RangeFinder()
            //{
            //    Type = (int)Enums.SensorType.RangeFinder,
            //    Rotation = -1 * (float)Math.PI / 4,
            //    Key = null,
            //    Active = true,
            //    MaxDistance = 30,//150,
            //    Index = 2,
            //    DirectionText = "Left"
            //});

            //pAgent1.SensorList.Add(new RangeFinder()
            //{
            //    Type = (int)Enums.SensorType.RangeFinder,
            //    Rotation = -1 * (float)Math.PI / 2,
            //    Key = null,
            //    Active = true,
            //    MaxDistance = 25,//150,
            //    Index = 3,
            //    DirectionText = ""
            //});
            /*
            GameAgent agent1 = new GameAgent();
            agent1.LoadContent(content, "Images\\600_by_200");
            agent1.Position = new Vector2(300, 300);
            agent1.Rotation = 0.0f;
            agent1.ID = 1;
            agent1.Type = (int)Enums.AgentType.Wall;
            AgentList.Add(agent1);

            GameAgent agent2 = new GameAgent();
            agent2.LoadContent(content, "Images\\200_by_500");
            agent2.Position = new Vector2(500, 650);
            agent2.Rotation = 0.0f;
            agent2.ID = 2;
            agent2.Type = (int)Enums.AgentType.Wall;
            AgentList.Add(agent2);

            GameAgent agent3 = new GameAgent();
            agent3.LoadContent(content, "Images\\200_by_200");
            agent3.Position = new Vector2(300, 800);
            agent3.Rotation = 0.0f;
            agent3.ID = 3;
            agent3.Type = (int)Enums.AgentType.Wall;
            AgentList.Add(agent3);

            GameAgent agent4 = new GameAgent();
            agent4.LoadContent(content, "Images\\200_by_900");
            agent4.Position = new Vector2(1000, 450);
            agent4.Rotation = 0.0f;
            agent4.ID = 4;
            agent4.Type = (int)Enums.AgentType.Wall;
            AgentList.Add(agent4);

            GameAgent agent5 = new GameAgent();
            agent5.LoadContent(content, "Images\\200_by_500");
            agent5.Position = new Vector2(1400, 250);
            agent5.ID = 5;
            agent5.Rotation = 0.0f;
            agent5.Type = (int)Enums.AgentType.Wall;
            AgentList.Add(agent5);

            GameAgent agent6 = new GameAgent();
            agent6.LoadContent(content, "Images\\300_by_200");
            agent6.Position = new Vector2(1850, 400);
            agent6.Rotation = 0.0f;
            agent6.ID = 6;
            agent6.Type = (int)Enums.AgentType.Wall;
            AgentList.Add(agent6);

            GameAgent agent7 = new GameAgent();
            agent7.LoadContent(content, "Images\\600_by_200");
            agent7.Position = new Vector2(1400, 800);
            agent7.Rotation = 0.0f;
            agent7.ID = 7;
            agent7.Type = (int)Enums.AgentType.Wall;
            AgentList.Add(agent7);

            GameAgent agent8 = new GameAgent();
            agent8.LoadContent(content, "Images\\200_by_200");
            agent8.Position = new Vector2(1900, 800);
            agent8.Rotation = 0.0f;
            agent8.ID = 8;
            agent8.Type = (int)Enums.AgentType.Wall;
            AgentList.Add(agent8);

            GameAgent agent9 = new GameAgent();
            agent9.LoadContent(content, "Images\\1600_by_200");
            agent9.Position = new Vector2(800, 1200);
            agent9.Rotation = 0.0f;
            agent9.ID = 9;
            agent9.Type = (int)Enums.AgentType.Wall;
            AgentList.Add(agent9);

            GameAgent agent10 = new GameAgent();
            agent10.LoadContent(content, "Images\\200_by_200");
            agent10.Position = new Vector2(1900, 800);
            agent10.Rotation = 0.0f;
            agent10.ID = 10;
            agent10.Type = (int)Enums.AgentType.Wall;
            AgentList.Add(agent10);

            GameAgent agent11 = new GameAgent();
            agent11.LoadContent(content, "Images\\200_by_600");
            agent11.Position = new Vector2(300, 1600);
            agent11.Rotation = 0.0f;
            agent11.ID = 11;
            agent11.Type = (int)Enums.AgentType.Wall;
            AgentList.Add(agent11);

            GameAgent agent12 = new GameAgent();
            agent12.LoadContent(content, "Images\\300_by_200");
            agent12.Position = new Vector2(550, 1600);
            agent12.Rotation = 0.0f;
            agent12.ID = 12;
            agent12.Type = (int)Enums.AgentType.Wall;
            AgentList.Add(agent12);

            GameAgent agent13 = new GameAgent();
            agent13.LoadContent(content, "Images\\200_by_200");
            agent13.Position = new Vector2(600, 1800);
            agent13.Rotation = 0.0f;
            agent13.ID = 13;
            agent13.Type = (int)Enums.AgentType.Wall;
            AgentList.Add(agent13);

            GameAgent agent14 = new GameAgent();
            agent14.LoadContent(content, "Images\\200_by_600");
            agent14.Position = new Vector2(1000, 1600);
            agent14.Rotation = 0.0f;
            agent14.ID = 14;
            agent14.Type = (int)Enums.AgentType.Wall;
            AgentList.Add(agent14);

            GameAgent agent15 = new GameAgent();
            agent15.LoadContent(content, "Images\\600_by_200");
            agent15.Position = new Vector2(1400, 1600);
            agent15.Rotation = 0.0f;
            agent15.ID = 15;
            agent15.Type = (int)Enums.AgentType.Wall;
            AgentList.Add(agent15);

            GameAgent agent16 = new GameAgent();
            agent16.LoadContent(content, "Images\\200_by_800");
            agent16.Position = new Vector2(1600, 2100);
            agent16.Rotation = 0.0f;
            agent16.ID = 16;
            agent16.Type = (int)Enums.AgentType.Wall;
            AgentList.Add(agent16);

            GameAgent agent17 = new GameAgent();
            agent17.LoadContent(content, "Images\\200_by_1100");
            agent17.Position = new Vector2(300, 2450);
            agent17.Rotation = 0.0f;
            agent17.ID = 17;
            agent17.Type = (int)Enums.AgentType.Wall;
            AgentList.Add(agent17);

            GameAgent agent18 = new GameAgent();
            agent18.LoadContent(content, "Images\\200_by_200");
            agent18.Position = new Vector2(900, 2200);
            agent18.Rotation = 0.0f;
            agent18.ID = 18;
            agent18.Type = (int)Enums.AgentType.Wall;
            AgentList.Add(agent18);

            GameAgent agent19 = new GameAgent();
            agent19.LoadContent(content, "Images\\1000_by_200");
            agent19.Position = new Vector2(1000, 2400);
            agent19.Rotation = 0.0f;
            agent19.ID = 19;
            agent19.Type = (int)Enums.AgentType.Wall;
            AgentList.Add(agent19);

            GameAgent agent20 = new GameAgent();
            agent20.LoadContent(content, "Images\\600_by_200");
            agent20.Position = new Vector2(700, 2800);
            agent20.Rotation = 0.0f;
            agent20.ID = 20;
            agent20.Type = (int)Enums.AgentType.Wall;
            AgentList.Add(agent20);

            GameAgent agent21 = new GameAgent();
            agent21.LoadContent(content, "Images\\600_by_200");
            agent21.Position = new Vector2(1500, 2800);
            agent21.Rotation = 0.0f;
            agent21.ID = 21;
            agent21.Type = (int)Enums.AgentType.Wall;
            AgentList.Add(agent21);

            GameAgent agent22 = new GameAgent();
            agent22.LoadContent(content, "Images\\200_by_200");
            agent22.Position = new Vector2(1300, 3100);
            agent22.Rotation = 0.0f;
            agent22.ID = 22;
            agent22.Type = (int)Enums.AgentType.Wall;
            AgentList.Add(agent22);

            GameAgent agent23 = new GameAgent();
            agent23.LoadContent(content, "Images\\200_by_200");
            agent23.Position = new Vector2(1900, 1200);
            agent23.Rotation = 0.0f;
            agent23.ID = 23;
            agent23.Type = (int)Enums.AgentType.Wall;
            AgentList.Add(agent23);

            GameAgent agent24 = new GameAgent();
            agent24.LoadContent(content, "Images\\100_by_100");
            agent24.Position = new Vector2(1750, 3050);
            agent24.Rotation = 0.0f;
            agent24.ID = 24;
            agent24.Type = (int)Enums.AgentType.Wall;
            AgentList.Add(agent24);

            // playerPos, windowWidth, windowHeight
            SetVisibleArea(PlayerStartPos, windowWidth, windowHeight);

            //LevelNode l1 = new LevelNode();
            //l1.ID = 1;
            //l1.Bounds = new Rectangle(0, 0, 600, 200);
            //l1.NeighborNodeList = new List<LevelNode>();

            //LevelNode l2 = new LevelNode();
            //l2.ID = 2;
            //l2.Bounds = new Rectangle(600, 0, 300, 900);
            //l2.NeighborNodeList = new List<LevelNode>();

            //LevelNode l3 = new LevelNode();
            //l3.ID = 3;
            //l3.Bounds = new Rectangle(0, 900, 2000, 200);
            //l3.NeighborNodeList = new List<LevelNode>();

            //LevelNode l4 = new LevelNode();
            //l4.ID = 4;
            //l4.Bounds = new Rectangle(0, 700, 200, 200);
            //l4.NeighborNodeList = new List<LevelNode>();

            //LevelNode l5 = new LevelNode();
            //l5.ID = 5;
            //l5.Bounds = new Rectangle(0, 400, 400, 300);
            //l5.NeighborNodeList = new List<LevelNode>();

            //LevelNode l6 = new LevelNode();
            //l6.ID = 6;
            //l6.Bounds = new Rectangle(1700, 700, 100, 200);
            //l6.NeighborNodeList = new List<LevelNode>();

            //LevelNode l7 = new LevelNode();
            //l7.ID = 7;
            //l7.Bounds = new Rectangle(1600, 1100, 200, 200);
            //l7.NeighborNodeList = new List<LevelNode>();

            //LevelNode l8 = new LevelNode();
            //l8.ID = 8;
            //l8.Bounds = new Rectangle(1100, 500, 900, 200);
            //l8.NeighborNodeList = new List<LevelNode>();

            //LevelNode l9 = new LevelNode();
            //l9.ID = 9;
            //l9.Bounds = new Rectangle(1500, 300, 200, 200);
            //l9.NeighborNodeList = new List<LevelNode>();

            //LevelNode l10 = new LevelNode();
            //l10.ID = 10;
            //l10.Bounds = new Rectangle(1500, 0, 500, 300);
            //l10.NeighborNodeList = new List<LevelNode>();

            //LevelNode l11 = new LevelNode();
            //l11.ID = 11;
            //l11.Bounds = new Rectangle(1100, 0, 200, 500);
            //l11.NeighborNodeList = new List<LevelNode>();

            //LevelNode l12 = new LevelNode();
            //l12.ID = 12;
            //l12.Bounds = new Rectangle(1100, 1300, 900, 200);
            //l12.NeighborNodeList = new List<LevelNode>();

            //LevelNode l13 = new LevelNode();
            //l13.ID = 13;
            //l13.Bounds = new Rectangle(1700, 1500, 300, 1000);
            //l13.NeighborNodeList = new List<LevelNode>();

            //LevelNode l14 = new LevelNode();
            //l14.ID = 14;
            //l14.Bounds = new Rectangle(400, 2500, 1600, 200);
            //l14.NeighborNodeList = new List<LevelNode>();

            //LevelNode l15 = new LevelNode();
            //l15.ID = 15;
            //l15.Bounds = new Rectangle(1800, 2700, 200, 500);
            //l15.NeighborNodeList = new List<LevelNode>();

            //LevelNode l16 = new LevelNode();
            //l16.ID = 16;
            //l16.Bounds = new Rectangle(1400, 2900, 400, 300);
            //l16.NeighborNodeList = new List<LevelNode>();

            //LevelNode l17 = new LevelNode();
            //l17.ID = 17;
            //l17.Bounds = new Rectangle(1000, 2700, 200, 500);
            //l17.NeighborNodeList = new List<LevelNode>();

            //LevelNode l18 = new LevelNode();
            //l18.ID = 18;
            //l18.Bounds = new Rectangle(1200, 2900, 200, 100);
            //l18.NeighborNodeList = new List<LevelNode>();

            //LevelNode l19 = new LevelNode();
            //l19.ID = 19;
            //l19.Bounds = new Rectangle(400, 2900, 600, 300);
            //l19.NeighborNodeList = new List<LevelNode>();

            //LevelNode l20 = new LevelNode();
            //l20.ID = 20;
            //l20.Bounds = new Rectangle(200, 3000, 200, 200);
            //l20.NeighborNodeList = new List<LevelNode>();

            //LevelNode l21 = new LevelNode();
            //l21.ID = 21;
            //l21.Bounds = new Rectangle(400, 2300, 100, 200);
            //l21.NeighborNodeList = new List<LevelNode>();

            //LevelNode l22 = new LevelNode();
            //l22.ID = 22;
            //l22.Bounds = new Rectangle(400, 2100, 400, 200);
            //l22.NeighborNodeList = new List<LevelNode>();

            //LevelNode l23 = new LevelNode();
            //l23.ID = 23;
            //l23.Bounds = new Rectangle(400, 1900, 1100, 200);
            //l23.NeighborNodeList = new List<LevelNode>();

            //LevelNode l24 = new LevelNode();
            //l24.ID = 24;
            //l24.Bounds = new Rectangle(400, 1700, 100, 200);
            //l24.NeighborNodeList = new List<LevelNode>();

            //LevelNode l25 = new LevelNode();
            //l25.ID = 25;
            //l25.Bounds = new Rectangle(1000, 2100, 500, 200);
            //l25.NeighborNodeList = new List<LevelNode>();

            //LevelNode l26 = new LevelNode();
            //l26.ID = 26;
            //l26.Bounds = new Rectangle(1100, 1700, 400, 200);
            //l26.NeighborNodeList = new List<LevelNode>();

            //LevelNode l27 = new LevelNode();
            //l27.ID = 27;
            //l27.Bounds = new Rectangle(700, 1500, 200, 400);
            //l27.NeighborNodeList = new List<LevelNode>();

            //LevelNode l28 = new LevelNode();
            //l28.ID = 28;
            //l28.Bounds = new Rectangle(400, 1300, 500, 200);
            //l28.NeighborNodeList = new List<LevelNode>();

            //LevelNode l29 = new LevelNode();
            //l29.ID = 29;
            //l29.Bounds = new Rectangle(0, 1300, 200, 1900);
            //l29.NeighborNodeList = new List<LevelNode>();

            //l1.NeighborNodeList.Add(l2);

            //l2.NeighborNodeList.Add(l1);
            //l2.NeighborNodeList.Add(l3);

            //l3.NeighborNodeList.Add(l2);
            //l3.NeighborNodeList.Add(l4);
            //l3.NeighborNodeList.Add(l6);
            //l3.NeighborNodeList.Add(l7);

            //l4.NeighborNodeList.Add(l3);
            //l4.NeighborNodeList.Add(l5);

            //l5.NeighborNodeList.Add(l4);

            //l6.NeighborNodeList.Add(l3);
            //l6.NeighborNodeList.Add(l8);

            //l7.NeighborNodeList.Add(l3);
            //l7.NeighborNodeList.Add(l12);

            //l8.NeighborNodeList.Add(l6);
            //l8.NeighborNodeList.Add(l9);
            //l8.NeighborNodeList.Add(l11);

            //l9.NeighborNodeList.Add(l8);
            //l9.NeighborNodeList.Add(l10);

            //l10.NeighborNodeList.Add(l9);

            //l11.NeighborNodeList.Add(l8);

            //l12.NeighborNodeList.Add(l7);
            //l12.NeighborNodeList.Add(l13);

            //l13.NeighborNodeList.Add(l12);
            //l13.NeighborNodeList.Add(l14);

            //l14.NeighborNodeList.Add(l13);
            //l14.NeighborNodeList.Add(l15);
            //l14.NeighborNodeList.Add(l17);
            //l14.NeighborNodeList.Add(l21);

            //l15.NeighborNodeList.Add(l14);
            //l15.NeighborNodeList.Add(l16);

            //l16.NeighborNodeList.Add(l15);
            //l16.NeighborNodeList.Add(l18);

            //l17.NeighborNodeList.Add(l14);
            //l17.NeighborNodeList.Add(l18);
            //l17.NeighborNodeList.Add(l19);

            //l18.NeighborNodeList.Add(l16);
            //l18.NeighborNodeList.Add(l17);

            //l19.NeighborNodeList.Add(l17);
            //l19.NeighborNodeList.Add(l20);

            //l20.NeighborNodeList.Add(l19);
            //l20.NeighborNodeList.Add(l29);

            //l21.NeighborNodeList.Add(l14);
            //l21.NeighborNodeList.Add(l22);

            //l22.NeighborNodeList.Add(l21);
            //l22.NeighborNodeList.Add(l23);

            //l23.NeighborNodeList.Add(l22);
            //l23.NeighborNodeList.Add(l24);
            //l23.NeighborNodeList.Add(l25);
            //l23.NeighborNodeList.Add(l26);

            //l24.NeighborNodeList.Add(l23);

            //l25.NeighborNodeList.Add(l23);

            //l26.NeighborNodeList.Add(l23);

            //l27.NeighborNodeList.Add(l23);
            //l27.NeighborNodeList.Add(l28);

            //l28.NeighborNodeList.Add(l27);

            //l29.NeighborNodeList.Add(l20);

            

            //WalkableNodes.Add(l1);
            //WalkableNodes.Add(l2);
            //WalkableNodes.Add(l3);
            //WalkableNodes.Add(l4);
            //WalkableNodes.Add(l5);
            //WalkableNodes.Add(l6);
            //WalkableNodes.Add(l7);
            //WalkableNodes.Add(l8);
            //WalkableNodes.Add(l9);
            //WalkableNodes.Add(l10);
            //WalkableNodes.Add(l11);
            //WalkableNodes.Add(l12);
            //WalkableNodes.Add(l13);
            //WalkableNodes.Add(l14);
            //WalkableNodes.Add(l15);
            //WalkableNodes.Add(l16);
            //WalkableNodes.Add(l17);
            //WalkableNodes.Add(l18);
            //WalkableNodes.Add(l19);
            //WalkableNodes.Add(l20);
            //WalkableNodes.Add(l21);
            //WalkableNodes.Add(l22);
            //WalkableNodes.Add(l23);
            //WalkableNodes.Add(l24);
            //WalkableNodes.Add(l25);
            //WalkableNodes.Add(l26);
            //WalkableNodes.Add(l27);
            //WalkableNodes.Add(l28);
            //WalkableNodes.Add(l29);
             */
        }

        #endregion
    }
}
