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
    class Entity : GameObject
    {
        public Collision collision;
        public Lighting lighting;
        public Movement movement;
        public Draw draw;
        public Interaction interaction;
        int x, y;

        public Entity(Collision col, Lighting lit, Movement mov, Draw drw, Interaction inter)
        {
            Lighting.AddLightingObject(this);
            Collision.AddCollisionObject(this);
            Draw.AddDrawObject(this);
            collision = col;
            lighting = lit;
            movement = mov;
            draw = drw;
            interaction = inter;
        }
    }
}
