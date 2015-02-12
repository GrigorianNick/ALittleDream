using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ALittleDream
{
    class Crate : GameObject
    {
        private void Move() {
            //Console.WriteLine("CrateY: {0}", spriteY);
            if (spriteY < 400)
            {
                spriteY *= gravity;
            }
        }
        public Crate(int x, int y, int width, int height, string spriteName)
        {
            this.spriteName = spriteName;
            this.spriteX = x;
            this.spriteY = y;
            this.spriteWidth = width;
            this.spriteHeight = height;
            this.gravity = 1.1;
        }

        public override bool Collide(GameObject input)
        {
            return false;
        }

        public override void Update(Controls controls, Microsoft.Xna.Framework.GameTime gameTime)
        {
            this.Move();
        }
    }
}
