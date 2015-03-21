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
    public unsafe class Entity : GameObject
    {
        public static ArrayList entityList = new ArrayList();
        public Collision collision;
        public Lighting lighting;
        public Movement movement;
        public MyDraw draw;
        public Interaction interaction;

        public Entity(ref int x_in, ref int y_in, ref int height, ref int width, string spriteFile, Collision col, Lighting lit, Movement mov, MyDraw drw, Interaction inter)
        {
            //assign inherited sprite values
            spriteX = x_in;
            spriteY = y_in;
            spriteHeight = height;
            spriteWidth = width;
            spriteName = spriteFile;

            //assign entity components
            if (!(col is NoCollision))
            {
                Collision.AddCollisionObject(this);
            }
            if (!(lit is NoLighting))
            {
                Lighting.AddLightingObject(this);
            }            
            collision = col;
            lighting = lit;
            movement = mov;
            draw = drw;
            interaction = inter;
            collision.x = spriteX;
            collision.y = spriteY;
            collision.height = height;
            collision.width = width;

            movement.x = spriteX;
            movement.y = spriteY;
            movement.height = height;
            movement.width = width;

            lighting.x = spriteX;
            lighting.y = spriteY;
        }

        public static void AddEntityObject(Entity ent)
        {
            entityList.Add(ent);
        }

        public override void Update(Controls controls, GameTime gameTime)
        {
            movement.move(controls, gameTime);
        }
    }
}
