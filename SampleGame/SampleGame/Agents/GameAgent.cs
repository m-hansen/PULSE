using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SampleGame
{
    public class GameAgent
    {
        #region Constants

        public Texture2D Texture { get; private set; }  // the image set for the object
        public Vector2 Origin;                          // origin of object: currently set to the middle of the image
        public Vector2 Position;                        // the current position of the object
        public float Rotation = 0.0f;                   // how far the object has rotated
        public float RotationSpeed = 1.0f;              // how fast the object should rotate
        public float Scale = 1;                         // how much to scale the image
        public float ZLayer;                            // depth of the object
        public Color Color = Color.White;               // max RGB of the image to draw
        public int Type;                                // type of agent (wall, etc)
        public bool Active = true;                      // whether the object is active on the screen
        public int ID;
        
        public int TotalFrames { get; private set; }    // the total frames in the image
        public TimeSpan AnimationInterval;              // how often the frames are changed

        protected Rectangle[] rects;                    // rectangle array of each sub image to draw within the sprite sheet
        protected int currentFrame;                     // which frame of the image we're currently on
        protected TimeSpan animElapsed;                 // how long it's been since we last moved frames

        // helper property for getting the width and height of the object.
        public int FrameWidth { get { return rects == null ? Texture.Width : rects[0].Width; } }
        public int FrameHeight { get { return rects == null ? Texture.Height : rects[0].Height; } }

        // the agent's current bounding rectangle, used for collision detection
        public Rectangle Bounds
        {
            get
            {
                return new Rectangle
                (
                    (int)(Position.X - Origin.X * Scale),
                    (int)(Position.Y - Origin.Y * Scale),
                    (int)(rects == null ? Texture.Width * Scale : rects[0].Width * Scale),
                    (int)(rects == null ? Texture.Height * Scale : rects[0].Height * Scale)
                );
            }
        }

        #endregion

        #region Debugging

        public bool DisplayInfo = false;

        public string GetAgentInfo()
        {
            return "ID: " + ID + ", Position: " + Position.X + ", " + Position.Y + ", "
                 + "Width: " + Bounds.Width + ", Height: " + Bounds.Height;
        }

        #endregion

        public bool ContainsRect(Rectangle targetRect)
        {
            return Rectangle.Intersect(Bounds, targetRect).IsEmpty;
        }

        protected bool IsInVisibleArea(Rectangle visibleRect)
        {
            return visibleRect.Intersects(Bounds);
        }

        // Render the sprite to the screen
        public virtual void Draw(SpriteBatch sprites, SpriteFont font1, Rectangle visibleRect)
        {
            // whether the object is currently being drawn on the screen
            if (Active && IsInVisibleArea(visibleRect))
            {
                sprites.Draw(Texture, new Vector2(Position.X - visibleRect.Left, Position.Y - visibleRect.Top), rects == null ? null : (Rectangle?)rects[currentFrame], Color, Rotation, Origin, Scale, SpriteEffects.None, ZLayer);
            }
        }

        // Load the texture for the agent from the content pipeline
        public virtual void LoadContent(ContentManager contentManager, string assetName, Rectangle? firstRect = null, int frames = 1, bool horizontal = true, int space = 0)
        {
            // loading the image for the object
            Texture = contentManager.Load<Texture2D>(assetName);

            // setting the total number of frames within the image
            TotalFrames = frames;

            // setting the origin to the center of the object
            Origin = new Vector2(Texture.Width / (2 * (horizontal ? frames : 1)), Texture.Height / (2 * (horizontal ? 1 : frames)));

            // if the image is a sprite sheet, set each rectangle of the object
            if (firstRect.HasValue)
            {
                rects = new Rectangle[frames];

                for (int i = 0; i < frames; i++)
                {
                    rects[i] = new Rectangle
                    (
                        firstRect.Value.Left + (horizontal ? (firstRect.Value.Width + space) * i : 0),
                        firstRect.Value.Top + (horizontal ? 0 : (firstRect.Value.Height + space) * i),
                        firstRect.Value.Width,
                        firstRect.Value.Height
                    );
                }
            }
        }
    }
}
