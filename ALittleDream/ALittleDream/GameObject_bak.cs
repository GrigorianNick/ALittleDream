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
    abstract class GameObject_bak : Sprite
    {
        // Mechanics stuff
        protected double luminosity;
        protected double gravity;
        public static ArrayList objects = new ArrayList();

        public static void AddGameObject(GameObject_bak obj)
        {
            objects.Add(obj);
        }

        // Getters and setters
        public double GetX()
        {
            return spriteX;
        }

        public void SetX(int x)
        {
            spriteX = x;
        }

        public double GetY()
        {
            return spriteY;
        }

        public void SetY(int y)
        {
            spriteY = y;
        }

        public int GetHeight()
        {
            return spriteHeight;
        }

        public int GetWidth()
        {
            return spriteWidth;
        }
        public void render(SpriteBatch sb)
        {

            foreach (GameObject_bak light in LightSource.lights)
            {
                // Hardcoding light distance because simple prototype
                if (Math.Pow(Math.Pow(this.GetX() - light.GetX(), 2) + Math.Pow(this.GetY() - light.GetY(), 2), 0.5) < 100)
                {
                    Draw(sb);
                }
            }
        }

        // Abstract stuff
        public abstract bool Collide(GameObject_bak input);
        public abstract void Update(Controls controls, GameTime gameTime);
    }
}
