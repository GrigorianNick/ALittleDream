using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Audio;

namespace ALittleDream
{
    abstract class GameObject
    {
        // Mechanics stuff
        protected double luminosity;
        protected double gravity;

        // Rendering stuff
        protected int spriteX, spriteY;
        protected int spriteWidth, spriteHeight;
        protected Texture2D image;

        // Getters and setters
        public int GetX()
        {
            return spriteX;
        }

        public void SetX(int x)
        {
            spriteX = x;
        }

        public int GetY()
        {
            return spriteY;
        }

        public void SetY(int y)
        {
            spriteY = y;
        }

        public void Draw(SpriteBatch sb)
        {
            sb.Draw(image, new Rectangle(spriteX, spriteY, spriteWidth, spriteHeight), Color.White);
        }

        // Abstract stuff
        public abstract bool Collide(GameObject input);
        public abstract void LoadContent(ContentManager content);
        public abstract void Update(Controls controls, GameTime gameTime);
    }
}
