using System;
using System.Collections.Generic;
using System.Collections;
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
        public List<Texture2D> spriteAnimations;
        public bool facingRight;

        public void Draw(SpriteBatch sb)
        {
            if (facingRight)
                sb.Draw(image, new Rectangle(spriteX, spriteY, spriteWidth, spriteHeight), Color.White);
            else
                sb.Draw(image, new Rectangle(spriteX, spriteY, spriteWidth, spriteHeight), null, Color.White, 0f, Vector2.Zero, SpriteEffects.FlipHorizontally, 1f);
        }
        public void LoadContent(ContentManager content)
        {
            image = content.Load<Texture2D>(this.spriteName);
        }
        public void AnimatedLoadContent(ContentManager content, string path)
        {
            spriteAnimations.Add(content.Load<Texture2D>(path));
        }
    }
}
