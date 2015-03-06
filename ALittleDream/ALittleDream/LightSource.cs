using System;
using System.Collections;
using System.Linq;
using System.Text;

namespace ALittleDream
{
    class LightSource : Sprite
    {
        public static ArrayList lights = new ArrayList();

        public static void AddLightSource(GameObject_bak light)
        {
            lights.Add(light);
        }
    }
}
