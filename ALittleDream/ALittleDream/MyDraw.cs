using System;
using System.Collections;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ALittleDream
{
    public class MyDraw : Component
    {
        public enum DrawLighting { always, ifLit, ifUnlit};

        public DrawLighting drawFlag;

        public MyDraw(DrawLighting flag)
        {
            drawFlag = flag;
        }
        
        
    }
}
