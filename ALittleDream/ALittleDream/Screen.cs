using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;


namespace ALittleDream
{
    public abstract class Screen
    {
        public bool changeScreen;
        public bool changed;
        public static int input; //0 - keyboard //1 - controller

        public abstract void LoadContent(ContentManager content);

        public abstract void UnloadContent(ContentManager content);

        public abstract void Update(Controls controls, GameTime gametime, AudioMixer audioMixer);

        public abstract void Draw(SpriteBatch sb);

        public abstract string identify();
    }
}
