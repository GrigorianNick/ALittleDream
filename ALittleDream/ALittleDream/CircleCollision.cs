using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ALittleDream
{
    class CircleCollision : Collision
    {
        public override bool collide(Entity ent)
        {
            return false;
        }
    }
}
