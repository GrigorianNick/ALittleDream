using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ALittleDream
{
    public class CircleLighting : Lighting
    {
        public override bool light(Entity ent)
        {
            return false;
        }
    }
}
