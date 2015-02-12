using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;

namespace ALittleDream
{
    public class Controls
    {
        public KeyboardState kb;
        public KeyboardState kbo;
        public GamePadState gp;
        public GamePadState gpo;

        public Controls()
        {
            this.kb = Keyboard.GetState();
            this.kbo = Keyboard.GetState();
            this.gp = GamePad.GetState(PlayerIndex.One);
            this.gpo = GamePad.GetState(PlayerIndex.One);

        }

        public void Update()
        {
            kbo = kb;
            gpo = gp;
            kb = Keyboard.GetState();
            this.gp = GamePad.GetState(PlayerIndex.One);
        }

        public bool isPressed(Keys key, Buttons button)
        {
            //Console.WriteLine (button);
            return kb.IsKeyDown(key) || gp.IsButtonDown(button);
        }

        public bool onPress(Keys key, Buttons button)
        {
            if ((gp.IsButtonDown(button) && gpo.IsButtonUp(button)))
            {
                Console.WriteLine(button);
            }
            return (kb.IsKeyDown(key) && kbo.IsKeyUp(key)) ||
                (gp.IsButtonDown(button) && gpo.IsButtonUp(button));
        }

        public bool onRelease(Keys key, Buttons button)
        {
            //Console.WriteLine (button);
            return (kb.IsKeyUp(key) && kbo.IsKeyDown(key)) ||
                (gp.IsButtonUp(button) && gpo.IsButtonDown(button));
        }

        public bool isHeld(Keys key, Buttons button)
        {
            //Console.WriteLine (button);
            return (kb.IsKeyDown(key) && kbo.IsKeyDown(key)) ||
                (gp.IsButtonDown(button) && gpo.IsButtonDown(button));
        }

    }
}

