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
    public unsafe class Sprite
    {
        public string spriteName;
        public int spriteX, spriteY, spriteHeight, spriteWidth;
        public Texture2D image;

        public void Draw(SpriteBatch sb)
        {
            sb.Draw(image, new Rectangle(spriteX, spriteY, spriteWidth, spriteHeight), Color.White);            
        }
        public void LoadContent(ContentManager content)
        {
            image = content.Load<Texture2D>(this.spriteName);
        }
    }
}
