using System;
using System.Collections;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ALittleDream
{
    abstract unsafe class Draw : Component
    {
        public static SpriteBatch sb;
        public static ArrayList drawList = new ArrayList();
        public Sprite sprite;
        public override void Update()
        {
            Draw.sb.Begin();
            foreach (Entity ent in Draw.drawList)
            {
                ent.draw.draw();
            }
            Draw.sb.End();
            //Somehow need to call Game.draw();
        }
        public Draw(int* x, int* y, int height, int width, string spriteName)
        {
            sprite.spriteX = x;
            sprite.spriteY = y;
            sprite.spriteHeight = height;
            sprite.spriteWidth = width;
            sprite.spriteName = spriteName;
        }
        public abstract void draw();
    }
}
