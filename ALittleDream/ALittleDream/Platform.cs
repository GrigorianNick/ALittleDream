using System;
using System.Collections;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Audio;

namespace ALittleDream
{
    abstract class Platform : Sprite
    {
        public static ArrayList platforms = new ArrayList();

        public static void AddPlatform(Platform plat)
        {
            platforms.Add(plat);
        }
        public double GetX()
        {
            return spriteX;
        }

        public double GetY()
        {
            return spriteY;
        }

        public int GetHeight()
        {
            return spriteHeight;
        }

        public int GetWidth()
        {
            return spriteWidth;
        }

        public abstract bool Collide(GameObject input);

        public void render(SpriteBatch sb){
        
            foreach (GameObject light in LightSource.lights)
            {
                // Hardcoding light distance because simple prototype
                if (Math.Pow(this.GetX() - light.GetX(), 2) + Math.Pow(this.GetY() - light.GetY(), 2) < 10000) {
                    Draw(sb);
                }
            }
        }
    }
}
