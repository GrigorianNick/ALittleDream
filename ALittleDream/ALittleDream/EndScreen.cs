using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ALittleDream
{
    class EndScreen : Screen
    {
        private Texture2D blackScreen;
        private Texture2D theEnd;
        private Vector2 theEndPosition;
        private Vector2 theEndSize;
        private double timeToChange;
        private double timeElapsed;

        private int screenWidth;
        private int screenHeight;

        public EndScreen(int screenWidth, int screenHeight)
        {
            timeElapsed = 0;
            timeToChange = 100;
            theEndSize.X = 275;
            theEndSize.Y = 60;
            theEndPosition.X = (screenWidth - theEndSize.X) / 2;
            theEndPosition.Y = (screenHeight - theEndSize.Y) / 2;
            this.screenWidth = screenWidth;
            this.screenHeight = screenHeight;
        }
        public override void LoadContent(ContentManager content)
        {
            blackScreen = content.Load<Texture2D>("blacksquare");
            theEnd = content.Load<Texture2D>("menu/theEnd");

        }

        public override void UnloadContent(ContentManager content)
        {
            content.Unload();
        }

        public override void Update(Controls controls, GameTime gametime)
        {
            timeElapsed += gametime.ElapsedGameTime.TotalSeconds;
            if (timeElapsed > timeToChange)
            {
                changeScreen = true;
            }

        }

        public override void Draw(SpriteBatch sb)
        {
            sb.Draw(blackScreen, new Rectangle(0, 0, screenWidth, screenHeight), Color.White);
            sb.Draw(theEnd, new Rectangle((int)theEndPosition.X, (int)theEndPosition.Y, (int)theEndSize.X, (int)theEndSize.Y), Color.White);
        }
    }
}
