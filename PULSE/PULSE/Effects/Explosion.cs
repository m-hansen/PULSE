using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PulseGame.Helpers;

namespace PulseGame.Effects
{
    public class Explosion : Effect
    {
        private Rectangle[] rects;                    // rectangle array of each sub image to draw within the sprite sheet
        public int TotalFrames;

        public override int FrameWidth { get { return rects == null ? Texture.Width : rects[0].Width; } }
        public override int FrameHeight { get { return rects == null ? Texture.Height : rects[0].Height; } }

        public override Rectangle Bounds
        {
            get
            {
                return new Rectangle
                (
                    (int)(Position.X - Origin.X),
                    (int)(Position.Y - Origin.Y),
                    (int)(rects == null ? Texture.Width : rects[0].Width),
                    (int)(rects == null ? Texture.Height : rects[0].Height)
                );
            }
        }

        public override void Update(GameTime gameTime, LevelInfo levelInfo)
        {
            // if the image is a sprite sheet 
            // and if enough time has passed to where we need to move to the next frame
            if (TotalFrames > 1 && (animElapsed += gameTime.ElapsedGameTime) > AnimationInterval)
            {
                if (++currentFrame == TotalFrames)
                {
                    if (EffectSubType != Enums.AttackSubType.ReflectingStar) Active = false;
                    currentFrame = 0;
                }

                // move back by the animation interval (in miliseconds)
                animElapsed -= AnimationInterval;

                if (EffectSubType == Enums.AttackSubType.ReflectingStar)
                {
                    EffectComponent ef = PulseGame.Current.effectComponent;
                    Vector2 playerPos = PulseGame.Current.player.Position;

                    foreach (Effect effect in ef.effectList.Where(e => e.GetType() == typeof(Bullet) && e.CastedBy == Enums.AgentType.Enemy && e.Bounds.Intersects(Bounds)).ToList())
                    {
                        Bullet bullet = (Bullet)effect;

                        bullet.Rotation = Utils.GetRotationToTarget(playerPos, bullet.Position);
                        bullet.Position += Utils.CalculateRotatedMovement(new Vector2(0, -2), bullet.Rotation);
                    }

                    Player playerObj = PulseGame.Current.player;

                    if (playerObj.Bounds.Intersects(Bounds))
                    {
                        playerObj.TakeDamage(playerObj.Health);
                    }
                }
            }

            base.Update(gameTime, levelInfo);
        }

        public void LoadExplosion(Texture2D texture, Rectangle? firstRect = null, int frames = 1, bool horizontal = true, int space = 0)
        {
            Texture = texture;

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

        public override void Draw(SpriteBatch sprites, Rectangle visibleRect)
        {
            // whether the object is currently being drawn on the screen
            if (Active && IsInVisibleArea(visibleRect))
            {
                sprites.Draw(Texture, new Vector2(Position.X - visibleRect.Left, Position.Y - visibleRect.Top), rects == null ? null : (Rectangle?)rects[currentFrame], Color, Rotation, Origin, 1, SpriteEffects.None, 0);
            }
        }
    }
}
