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
        public Player(int x, int y, int width, int height)
        {
            this.spriteX = x;
            this.spriteY = y;
            this.spriteWidth = width;
            this.spriteHeight = height;
        }

        // Private stuff
        public void Move(Controls controls)
        {
            //Console.WriteLine("Moved!");
            if (controls.onPress(Keys.Right, Buttons.DPadRight) || controls.isHeld(Keys.Right, Buttons.DPadRight))
            {
                spriteX++;
            }
            else if (controls.onPress(Keys.Left, Buttons.DPadRight) || controls.isHeld(Keys.Left, Buttons.DPadLeft))
            {
                spriteX--;
            }
            else if (controls.onPress(Keys.Up, Buttons.DPadRight) || controls.isHeld(Keys.Up, Buttons.DPadUp))
            {
                spriteY--;
            }
            else if (controls.onPress(Keys.Down, Buttons.DPadRight) || controls.isHeld(Keys.Down, Buttons.DPadDown))
            {
                spriteY++;
            }
        }

        // Public abstract stuff
        public override bool Collide(GameObject input)
        {
            return true;
        }

        public override void LoadContent(ContentManager content)
        {
            image = content.Load<Texture2D>("beta_player.png");
        }

        public override void Update(Controls controls, GameTime gameTime)
        {
            //Console.WriteLine("Player update!");
            this.Move(controls);
            //SetX(GetX() + 1);
            //spriteX += 1;
        }
    }
}
