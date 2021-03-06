﻿using System;
using System.Collections;
using System.Linq;
using System.Text;

namespace ALittleDream
{
    public abstract class Lighting : Component
    {
        public static ArrayList lightingList = new ArrayList();
        public static void AddLightingObject(Entity ent)
        {
            lightingList.Add(ent);
        }
        // ent calls Lighting.light. Lighting.light returns whether or not ent is lit.
        public abstract bool light(Entity ent);
    }
}
