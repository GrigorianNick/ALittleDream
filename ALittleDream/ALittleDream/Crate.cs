using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ALittleDream
{
    class Crate : GameObject
    {
        private void Move() {
            bool rewind = false;
            spriteY+=1;
            foreach (GameObject obj in GameObject.objects)
            {
                if (this.Collide(obj))
                {
                    rewind = true;
                    break;
                }
            }
            foreach (Platform plat in Platform.platforms)
            {
                if (plat.Collide(this))
                {
                    rewind = true;
                    break;
                }
            }
            if (rewind)
            {
                spriteY--;
            }
        }
        public Crate(int x, int y, int width, int height, string spriteName)
        {
            this.spriteName = spriteName;
            this.spriteX = x;
            this.spriteY = y;
            this.spriteWidth = width;
            this.spriteHeight = height;
        }

        public override bool Collide(GameObject input)
        {
            if (input == this)
            {
                return false;
            }
            if ((this.GetX() + this.GetWidth()) >= input.GetX() &&
                this.GetX() <= (input.GetX() + input.GetWidth()) &&
                (this.GetY() + this.GetHeight()) >= input.GetY() &&
                this.GetY() <= (input.GetY() + input.GetHeight()))
            {
                return true;
            }
            return false;
        }

        public override void Update(Controls controls, Microsoft.Xna.Framework.GameTime gameTime)
        {
            this.Move();
        }
    }
}
