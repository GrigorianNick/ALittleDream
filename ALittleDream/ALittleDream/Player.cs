using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;

namespace ALittleDream
{
    class Player : GameObject
    {

        // Constructor
        public Player(int x, int y, int width, int height, string spriteName)
        {
            this.spriteName = spriteName;
            this.spriteX = x;
            this.spriteY = y;
            this.spriteWidth = width;
            this.spriteHeight = height;
            this.gravity = 1;
        }

        // Private stuff
        private void Move(Controls controls)
        {
            // WASD movement
            if (controls.isHeld(Keys.D, Buttons.DPadRight))
            {
                spriteX++;
                foreach (GameObject obj in GameObject.objects) {
                    if (this.Collide(obj))
                    {
                        obj.SetX((int)obj.GetX() + 1);
                        break;
                    }
                }
            }
            else if (controls.isHeld(Keys.A, Buttons.DPadRight))
            {
                spriteX--;
                foreach (GameObject obj in GameObject.objects)
                {
                    if (this.Collide(obj))
                    {
                        obj.SetX((int)obj.GetX() - 1);
                        break;
                    }
                }
            }
            if (controls.isHeld(Keys.W, Buttons.DPadRight))
            {
                spriteY--;
                foreach (GameObject obj in GameObject.objects)
                {
                    if (this.Collide(obj))
                    {
                        spriteY++;
                        break;
                    }
                }
            }
            else if (controls.isHeld(Keys.S, Buttons.DPadRight))
            {
                spriteY++;
                foreach (GameObject obj in GameObject.objects)
                {
                    if (obj.Collide(this))
                    {
                        spriteY--;
                        break;
                    }
                }
            }
        }

        // Public abstract stuff
        // We're just gonna pretend everything's a square.
        public override bool Collide(GameObject input)
        {
            if ((this.GetX() + this.GetWidth()) >= input.GetX() &&
                this.GetX() <= (input.GetX() + input.GetWidth()) &&
                (this.GetY() + this.GetHeight()) >= input.GetY() &&
                this.GetY() <= (input.GetY() + input.GetHeight()) ) {
                    return true;
                }
            return false;
        }

        public override void Update(Controls controls, GameTime gameTime)
        {
            this.Move(controls);
        }
    }
}
