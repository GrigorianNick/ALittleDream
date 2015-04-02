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
    class MenuScreen : Screen
    {
        Vector2 startButtonPosition;
        Vector2 quitButtonPosition;
        Vector2 startButtonSize;
        Vector2 quitButtonSize;
        Texture2D startButton;
        Texture2D quitButton;
        List<Texture2D> menuImages; 
        Texture2D menuBackground;
        float startAlpha;
        float quitAlpha;
        int screenWidth;
        int screenHeight;
        int i;
        float timeToChange;
        float timeElapsed;
        bool increase;
        bool startSelected;
        GameLoop game;


        public MenuScreen(int screenWidth, int screenHeight, GameLoop game)
        {
            this.game = game;
            startButtonPosition = new Vector2();
            startButtonSize.X = 130;
            startButtonSize.Y = 40;
            startButtonPosition.X = (screenWidth - startButtonSize.X)/ 2;
            startButtonPosition.Y = (screenHeight - startButtonSize.Y)/ 2;
            quitButtonPosition = new Vector2();
            quitButtonSize.X = 130;
            quitButtonSize.Y = 40;
            quitButtonPosition.X = (screenWidth - quitButtonSize.X) / 2;
            quitButtonPosition.Y = ((screenHeight - quitButtonSize.Y) / 2) + 50;
            this.screenWidth = screenWidth;
            this.screenHeight = screenHeight;
            startAlpha = 0.0f;
            quitAlpha = 1.0f;
            i = 0; 
            timeToChange = 0.11f;
            timeElapsed = 0.0f;
            increase = true;
            changeScreen = false;
            menuImages = new List<Texture2D>();
            startSelected = true;
        }

        public override void LoadContent(ContentManager content)
        {
            startButton = content.Load<Texture2D>("menu/enter.png");
            quitButton = content.Load<Texture2D>("menu/perish.png");
            menuBackground = content.Load<Texture2D>("menu/menuBackground.png");
            menuImages.Add(content.Load<Texture2D>("menu/menuBackground.png"));
            menuImages.Add(content.Load<Texture2D>("menu/menuBackground1.png"));
            menuImages.Add(content.Load<Texture2D>("menu/menuBackground2.png"));
            menuImages.Add(content.Load<Texture2D>("menu/menuBackground3.png"));
            menuImages.Add(content.Load<Texture2D>("menu/menuBackground4.png"));
            menuImages.Add(content.Load<Texture2D>("menu/menuBackground5.png"));

        }

        public override void UnloadContent(ContentManager content)
        {
            content.Unload();
        }

        public override void Update(Controls controls, GameTime gametime)
        {
            if (startSelected)
            {
                if (!increase)
                    startAlpha -= (float)gametime.ElapsedGameTime.TotalSeconds;
                else
                    startAlpha += (float)gametime.ElapsedGameTime.TotalSeconds;

                if (startAlpha < 0.0f)
                {
                    increase = true;
                    startAlpha = 0.0f;
                }
                else if (startAlpha > 1.0f)
                {
                    increase = false;
                    startAlpha = 1.0f;
                }
            }
            else
            {
                if (!increase)
                    quitAlpha -= (float)gametime.ElapsedGameTime.TotalSeconds;
                else
                    quitAlpha += (float)gametime.ElapsedGameTime.TotalSeconds;

                if (quitAlpha < 0.0f)
                {
                    increase = true;
                    quitAlpha = 0.0f;
                }
                else if (quitAlpha > 1.0f)
                {
                    increase = false;
                    quitAlpha = 1.0f;
                }
            }

            if (controls.onPress(Keys.Enter, Buttons.A) && !changed)
            {
                if (startSelected)
                {
                    changed = true;
                    changeScreen = true;
                }
                else
                {
                    game.Quit();
                }
            }
            else if (controls.onPress(Keys.Up, Buttons.DPadUp) && !startSelected)
            {
                startSelected = true;
                quitAlpha = 1.0f;
            }
            else if (controls.onPress(Keys.Down, Buttons.DPadDown) && startSelected)
            {
                startSelected = false;
                startAlpha = 1.0f;
            }

            timeElapsed += (float)gametime.ElapsedGameTime.TotalSeconds;
            if (timeToChange < timeElapsed)
            {
                int count = 0;
                foreach (Texture2D t in menuImages)
                {
                    if (i == count)
                        menuBackground = t;
                    count++;
                }
                i++;
                if (i == menuImages.Count)
                    i = 0;
                Console.WriteLine(i);
                Console.WriteLine(menuImages.Count);
                //menuBackground = menuImages[i];
                timeElapsed = 0.0f;
            }
  
        }

        public override void Draw(SpriteBatch sb)
        {
            sb.Draw(menuBackground, new Rectangle(0, 0, screenWidth, screenHeight), Color.White);
            sb.Draw(startButton, new Rectangle((int)startButtonPosition.X, (int)startButtonPosition.Y, (int)startButtonSize.X, (int)startButtonSize.Y), Color.White * startAlpha);
            sb.Draw(quitButton, new Rectangle((int)quitButtonPosition.X, (int)quitButtonPosition.Y, (int)quitButtonSize.X, (int)quitButtonSize.Y), Color.White * quitAlpha);

        }

    }
}
