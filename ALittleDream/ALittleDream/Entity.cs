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
    class Entity
    {
        public Collision collision;
        public Lighting lighting;
        public Movement movement;
        public Draw draw;
        public Interaction interaction;

        //public Entity();
    }
}
