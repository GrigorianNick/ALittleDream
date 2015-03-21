using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ALittleDream
{
    public abstract class GameObject : Sprite
    {
        public static GameTime gameTime;
        public abstract void Update(Controls controls, GameTime gameTime);
        /*public abstract GameObject getInstance();*/
    }
}
