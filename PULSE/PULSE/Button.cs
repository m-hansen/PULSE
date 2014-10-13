using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Drawing;

namespace PulseGame
{
    class Button
    {
        Texture2D texture;
        SpriteBatch spriteBatch;
        SpriteFont font;
        Rectangle location;
        Vector2 origin;
        Vector2 textLoc;
        MouseState currentMouseState;
        MouseState previousMouseState;
        //float scale = 0.5f;
        string text;
        bool containsMouse = false;
        bool clicked = false;
        static SoundEffect buttonHoverSound;

        public Button(Texture2D texture, SoundEffect btnSfx, SpriteFont font, SpriteBatch spriteBatch)
        {
            buttonHoverSound = btnSfx;
            this.texture = texture;
            this.font = font;
            this.spriteBatch = spriteBatch;
            this.location = new Rectangle(0, 0, texture.Width, texture.Height);
            origin = new Vector2(texture.Width / 2, texture.Height / 2);
        }

        public bool ContainsMouse
        {
            get { return containsMouse; }
            set { containsMouse = value; }
        }

        public string Text
        {
            get { return text; }
            set 
            {
                text = value;
                // TODO adjust loc based on text size
                textLoc = new Vector2(location.X, location.Y);
            }
        }

        public void Location(int x, int y)
        {
            location.X = (int) (x - origin.X);
            location.Y = (int) (y - origin.Y);
        }

        public void Update()
        {
            currentMouseState = Mouse.GetState();
            Point mousePos = new Point(currentMouseState.X, currentMouseState.Y);

            if (location.Contains(mousePos))
            {
                // Play sound only once when mouse enters
                if (!containsMouse && buttonHoverSound != null)
                {
                    buttonHoverSound.Play();
                }
                containsMouse = true;
            }
            else
            {
                containsMouse = false;
            }

            // Check if the button was clicked
            if ((currentMouseState.LeftButton == ButtonState.Released) && 
                (previousMouseState.LeftButton == ButtonState.Pressed) && 
                (containsMouse))
            {
                clicked = true;
            }

            previousMouseState = currentMouseState;
        }

        public void Draw(SpriteBatch sb)
        {
            Color color = Color.White;

            if (containsMouse)
            {
                color = Color.BlueViolet;
            }

            //DrawingHelper.DrawRectangle(location, color, false);
            sb.Draw(texture, new Vector2(location.X, location.Y), null, color, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.0f);
        }
    }
}
