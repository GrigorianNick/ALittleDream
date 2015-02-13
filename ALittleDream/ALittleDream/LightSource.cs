using System;
using System.Collections;
using System.Linq;
using System.Text;

namespace ALittleDream
{
    class LightSource
    {
        public static ArrayList lights = new ArrayList();

        public static void AddLightSource(GameObject light)
        {
            lights.Add(light);
        }
    }
}
