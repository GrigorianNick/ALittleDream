using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ALittleDream
{
    public unsafe class Walking : Movement
    {
        public int horizMomentum = 0;
        public int vertMomentum = 0;

        public override void move(Controls controls, GameTime gameTime)
        {
            if (controls.onPress(Keys.A, Buttons.DPadLeft)) {
                (x)--;
                Console.WriteLine(x);
            }
            if (controls.onPress(Keys.D, Buttons.DPadRight)) {
                (x)++;
            }
            if (controls.onPress(Keys.W, Buttons.DPadUp) || controls.onPress(Keys.Space, Buttons.DPadUp)) {
                (y)--;
            }
            if (controls.onPress(Keys.S, Buttons.DPadDown)) {
                (y)++;
            }
        }
    }
}
