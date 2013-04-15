using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SampleGame.Effects
{
    public class Effect
    {
        public Texture2D Texture;
        public Vector2 Origin;
        public Vector2 Position;
        public float Rotation = 0.0f;
        public bool Active = true;
        public TimeSpan AnimationInterval;
        public Color Color = Color.White;
        public Enums.AgentType CastedBy;
        public Enums.AttackType EffectType;
        public Enums.AttackSubType EffectSubType;

        protected int currentFrame;
        protected TimeSpan animElapsed;

        public virtual int FrameWidth { get { return Texture.Width; } }
        public virtual int FrameHeight { get { return Texture.Height; } }

        public virtual Rectangle Bounds
        {
            get
            {
                return new Rectangle
                (
                    (int)(Position.X - Origin.X),
                    (int)(Position.Y - Origin.Y),
                    (Texture.Width),
                    (Texture.Height)
                );
            }
        }

        public virtual Effect CloneToDirection(float offset, Enums.AttackType type, Enums.AttackSubType subType)
        {
            return new Effect() { Active = false };
        }

        public bool ContainsRect(Rectangle targetRect)
        {
            return Rectangle.Intersect(Bounds, targetRect).IsEmpty;
        }

        protected bool IsInLevelBounds(int levelWidth, int levelHeight)
        {
            Rectangle levelRect = new Rectangle(0, 0, levelWidth, levelHeight);

            return levelRect.Intersects(Bounds);
        }

        public void LoadEffect(Texture2D texture)
        {
            Texture = texture;

            // setting the origin to the center of the object
            Origin = new Vector2(Texture.Width / 2, Texture.Height / 2);
        }

        public virtual void Update(GameTime gameTime, LevelInfo levelInfo)
        {
            
        }

        protected bool IsInVisibleArea(Rectangle visibleRect)
        {
            return visibleRect.Intersects(Bounds);
        }

        public virtual void Draw(SpriteBatch batch, Rectangle visibleRect)
        {
            if (Active && IsInVisibleArea(visibleRect))
            {
                batch.Draw(Texture, new Vector2(Position.X - visibleRect.Left, Position.Y - visibleRect.Top), null, Color, Rotation, Origin, 1, SpriteEffects.None, 0);
            }
        }
    }
}
