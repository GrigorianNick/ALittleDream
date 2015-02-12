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
    class Sprite
    {
        protected string spriteName;
        protected double spriteX, spriteY;
        protected int spriteWidth, spriteHeight;
        protected Texture2D image;

        public void Draw(SpriteBatch sb)
        {
            sb.Draw(image, new Rectangle((int)spriteX, (int)spriteY, spriteWidth, spriteHeight), Color.White);
        }
        public void LoadContent(ContentManager content)
        {
            image = content.Load<Texture2D>(this.spriteName);
        }
    }
}
