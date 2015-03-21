using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ALittleDream
{
    public abstract class Movement : Component
    {
        public abstract void move(Controls controls, GameTime gameTime);
    }
}
