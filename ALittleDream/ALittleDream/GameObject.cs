﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ALittleDream
{
    abstract class GameObject
    {
        public abstract void Update();
        public abstract GameObject getInstance();
    }
}
