﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ALittleDream
{
    public class NoLighting : Lighting
    {
        public override bool light(Entity ent)
        {
            return false;
        }
    }
}
