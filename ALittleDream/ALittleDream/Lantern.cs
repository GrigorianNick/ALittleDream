﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;

namespace ALittleDream
{
    class Lantern : GameObject_bak
    {

        public Lantern(int x, int y, int width, int height, string spriteName)
        {
            LightSource.AddLightSource(this);
            this.spriteName = spriteName;
            this.spriteX = x;
            this.spriteY = y;
            this.spriteWidth = width;
            this.spriteHeight = height;
        }

        // Private stuff
        private void Move(Controls controls)
        {
            // WASD movement
            if (controls.isHeld(Keys.Right, Buttons.DPadRight))
            {
                spriteX++;
            }
            else if (controls.isHeld(Keys.Left, Buttons.DPadRight))
            {
                spriteX--;
            }
            if (controls.isHeld(Keys.Up, Buttons.DPadRight))
            {
                spriteY--;
            }
            else if (controls.isHeld(Keys.Down, Buttons.DPadRight))
            {
                spriteY++;
            }
        }

        public override bool Collide(GameObject_bak input)
        {
            return false;
        }

        public override void Update(Controls controls, GameTime gameTime)
        {
            this.Move(controls);
        }

    }
}
