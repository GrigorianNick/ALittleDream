using System;
using System.Collections;
using System.Linq;
using System.Text;

namespace ALittleDream
{
    public abstract class Collision : Component
    {
        public static ArrayList collisionList = new ArrayList();
        public static void AddCollisionObject(Entity ent)
        {
            collisionList.Add(ent);
        }
        public abstract bool collide(Entity ent);
    }
}
