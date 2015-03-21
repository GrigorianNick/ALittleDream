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
    unsafe class Sprite
    {
        public string spriteName;
        public int* spriteX = null;
        public int* spriteY = null;
        public int spriteWidth, spriteHeight;
        public Texture2D image;

        public void Draw(SpriteBatch sb)
        {
            if (spriteX == null || spriteY == null)
            {
                throw new NullReferenceException("spriteX/Y is null!");
            }
            else
            {
                sb.Draw(image, new Rectangle((int)spriteX, (int)spriteY, spriteWidth, spriteHeight), Color.White);
            }
        }
        public void LoadContent(ContentManager content)
        {
            image = content.Load<Texture2D>(this.spriteName);
        }
    }
}
