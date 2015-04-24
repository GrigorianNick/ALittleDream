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
        private List<Texture2D> endImages; 
        private Texture2D blackScreen;
        private Texture2D theEnd;
        private Vector2 theEndPosition;
        private Vector2 theEndSize;
        private float timeToChange;
        private float timeElapsed;
        private bool onlyText;

        private int screenWidth;
        private int screenHeight;
        private int count;

        public EndScreen(int screenWidth, int screenHeight)
        {
            timeElapsed = 0.0f;
            timeToChange = 0.05f;
            theEndSize.X = 200;
            theEndSize.Y = 83;
            theEndPosition.X = (screenWidth - theEndSize.X) / 2;
            theEndPosition.Y = (screenHeight - theEndSize.Y) / 2;
            this.screenWidth = screenWidth;
            this.screenHeight = screenHeight;
            endImages = new List<Texture2D>();
            onlyText = true;
        }
        public override void LoadContent(ContentManager content)
        {
            blackScreen = content.Load<Texture2D>("blacksquare");
            theEnd = content.Load<Texture2D>("end/1");
            endImages.Add(content.Load<Texture2D>("end/1"));
            endImages.Add(content.Load<Texture2D>("end/2"));
            endImages.Add(content.Load<Texture2D>("end/3"));
            endImages.Add(content.Load<Texture2D>("end/4"));
            endImages.Add(content.Load<Texture2D>("end/5"));
            endImages.Add(content.Load<Texture2D>("end/6"));
            endImages.Add(content.Load<Texture2D>("end/7"));
            endImages.Add(content.Load<Texture2D>("end/8"));
            endImages.Add(content.Load<Texture2D>("end/9"));
            endImages.Add(content.Load<Texture2D>("end/10"));
            endImages.Add(content.Load<Texture2D>("end/11"));
            endImages.Add(content.Load<Texture2D>("end/12"));
            endImages.Add(content.Load<Texture2D>("end/13"));
            endImages.Add(content.Load<Texture2D>("end/14"));
            endImages.Add(content.Load<Texture2D>("end/15")); 
            endImages.Add(content.Load<Texture2D>("end/16"));
            endImages.Add(content.Load<Texture2D>("end/17"));
            endImages.Add(content.Load<Texture2D>("end/18"));
            endImages.Add(content.Load<Texture2D>("end/19"));
            endImages.Add(content.Load<Texture2D>("end/20"));
            endImages.Add(content.Load<Texture2D>("end/21"));
       
        }

        public override void UnloadContent(ContentManager content)
        {
            content.Unload();
        }

        public override void Update(Controls controls, GameTime gametime, AudioMixer audioMixe)
        {
            timeElapsed += (float)gametime.ElapsedGameTime.TotalSeconds;
            if (onlyText)
            {
                if (timeElapsed > 1.0f)
                {
                    onlyText = false;
                }

            }
            else
            {
                if (timeElapsed > timeToChange && count < 21)
                {
                    theEnd = endImages[count];
                    count++;
                    timeElapsed = 0.0f;
                }
            }
        }

        public override void Draw(SpriteBatch sb)
        {
            sb.Draw(blackScreen, new Rectangle(0, 0, screenWidth, screenHeight), Color.White);
            sb.Draw(theEnd, new Rectangle((int)theEndPosition.X, (int)theEndPosition.Y, (int)theEndSize.X, (int)theEndSize.Y), Color.White);
        }

        public override string identify()
        {
            return "I am a Game End Screen!";
        }
    }
}
