using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ALittleDream
{
    public abstract class Interaction : Component
    {
        public abstract void interact(Entity ent);
    }
}
