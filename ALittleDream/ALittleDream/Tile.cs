using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ALittleDream
{
    class Tile
    {
        public string image;
        public int width;
        public int height;
        public int offsetX;
        public int offsetY;
        public string light;
        public string draw;
        public string collision;

        public Tile(string image, int width, int height, int offsetX, int offsetY, string light, string draw, string collision)
        {
            this.image = image;
            this.width = width;
            this.height = height;
            this.offsetX = offsetX;
            this.offsetY = offsetY;
            this.light = light;
            this.draw = draw;
            this.collision = collision;
        }

        public Tile(string image, int width, int height)
        {
            this.image = image;
            this.width = width;
            this.height = height;
            offsetX = 0;
            offsetY = 0;
            light = null;
        }


    }
}
