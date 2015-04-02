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
    class SplashScreen: Screen
    {
        Texture2D image1;
        Texture2D image2;
        Texture2D background;
        Vector2 titlePosition;
        Vector2 titleSize;
        double timeToChange;
        List<double> times; 
        double totalTimeElapsed;
        double timeElapsed;
        bool flip;
        Texture2D blackScreen;
        int screenWidth;
        int screenHeight;
        int i; 

        public SplashScreen(int screenWidth, int screenHeight)
        {
            timeToChange = 0.1;
            times = new List<double>();
            times.Add(0.1);//with
            times.Add(0.23);//wo
            times.Add(0.42);//with
            times.Add(0.50);//wo
            times.Add(0.80);//with
            times.Add(0.90);//wo
            times.Add(2.80);//with
            times.Add(2.90);//wo
            times.Add(5.80);//with
            totalTimeElapsed = 0;
            timeElapsed = 0;
            flip = true;
            changed = false; 
            i = 0; 
            titleSize.X = 275;
            titleSize.Y = 191;
            titlePosition.X = (screenWidth - titleSize.X) / 2;
            titlePosition.Y = (screenHeight - titleSize.Y) / 2;
            this.screenWidth = screenWidth;
            this.screenHeight = screenHeight;
        }
        public override void LoadContent(ContentManager content)
        {
            blackScreen = content.Load<Texture2D>("blacksquare");
            image1 = content.Load<Texture2D>("menu/title");
            image2 = content.Load<Texture2D>("menu/titlewLights");
            background = image1;
        }
        public override void UnloadContent(ContentManager content)
        {
            content.Unload();
        }
        public override void Update(Controls controls, GameTime gametime)
        {
            timeElapsed += gametime.ElapsedGameTime.TotalSeconds;
            totalTimeElapsed += gametime.ElapsedGameTime.TotalSeconds;
            if (timeElapsed > times[i])
            {
                i++;
                if (i == times.Count)
                {
                    timeElapsed = 0;
                    i = 0;
                }
                if (flip)
                {
                    background = image1;
                    flip = false; 
                }
                else
                {
                    background = image2;
                    flip = true; 
                }
            }

            if (totalTimeElapsed > 4.5 && !changed)
            {
                changed = true; 
                changeScreen = true;
            }

        }
        public override void Draw(SpriteBatch sb)
        {
            sb.Draw(blackScreen, new Rectangle(0, 0, screenWidth, screenHeight), Color.White);
            sb.Draw(background, new Rectangle((int)titlePosition.X, (int)titlePosition.Y, (int)titleSize.X, (int)titleSize.Y), Color.White);
        }
    }
}
