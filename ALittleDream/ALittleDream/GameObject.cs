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
    abstract class GameObject : Sprite
    {
        // Mechanics stuff
        protected double luminosity;
        protected double gravity;
        public static ArrayList objects = new ArrayList();

        public static void AddGameObject(GameObject obj)
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

        // Abstract stuff
        public abstract bool Collide(GameObject input);
        public abstract void Update(Controls controls, GameTime gameTime);
    }
}
