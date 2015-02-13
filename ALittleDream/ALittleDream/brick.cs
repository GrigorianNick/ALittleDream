// Tossaway class until I can figure out better nomenclature for platforms

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ALittleDream
{
    class brick : Platform
    {
        public brick(int x, int y, int width, int height, string spriteName)
        {
            this.spriteName = spriteName;
            this.spriteX = x;
            this.spriteY = y;
            this.spriteWidth = width;
            this.spriteHeight = height;
        }
        public override bool Collide(GameObject input)
        {
            if ((this.GetX() + this.GetWidth()) >= input.GetX() &&
                this.GetX() <= (input.GetX() + input.GetWidth()) &&
                (this.GetY() + this.GetHeight()) >= input.GetY() &&
                this.GetY() <= (input.GetY() + input.GetHeight()))
            {
                foreach (GameObject light in LightSource.lights)
                {
                    //Console.WriteLine("Distance: {0}", Math.Pow(this.GetX() - light.GetX(), 2) + Math.Pow(this.GetY() - light.GetY(), 2));
                    // Hardcoding light distance because simple prototype
                    if (Math.Pow(Math.Pow(this.GetX() - light.GetX(), 2) + Math.Pow(this.GetY() - light.GetY(), 2), 0.5) < 100)
                    {
                        return true;
                    }
                    //return true;
                }
            }
            return false;
        }
    }

    


}
