using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ALittleDream
{
    abstract unsafe class Component : GameObject
    {
        public int* x;
        public int* y;
    }
}
