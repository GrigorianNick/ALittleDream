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
    class Lightbulb : GameObject_bak
    {
        public Lightbulb(int x, int y, int width, int height, string spriteName)
        {
            this.spriteName = spriteName;
            this.spriteX = x;
            this.spriteY = y;
            this.spriteWidth = width;
            this.spriteHeight = height;
            LightSource.AddLightSource(this);
        }

        public void render(SpriteBatch sb)
        {
            Draw(sb);
        }

        public override bool Collide(GameObject_bak input)
        {
            return false;
        }

        public override void Update(Controls controls, GameTime gameTime) { }
    }
}
