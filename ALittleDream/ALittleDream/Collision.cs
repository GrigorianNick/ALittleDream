using System;
using System.Collections;
using System.Linq;
using System.Text;

namespace ALittleDream
{
    abstract class Collision : Component
    {
        public static ArrayList collisionList = new ArrayList();
        public static void AddCollisionObject(Entity ent);
        public abstract bool collide(Entity ent);
    }
}
