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
        List<Texture2D> menuImages; 
        List<MenuItem> menuItems;
        int itemNum;
        Texture2D menuBackground;
        int screenWidth;
        int screenHeight;
        int i;
        float timeToChange;
        float timeElapsed;
        GameLoop game;


        public MenuScreen(int screenWidth, int screenHeight, GameLoop game)
        {
            this.game = game;
            this.screenWidth = screenWidth;
            this.screenHeight = screenHeight;
            i = 0; 
            timeToChange = 0.11f;
            timeElapsed = 0.0f;
            changeScreen = false;
            menuImages = new List<Texture2D>();
            menuItems = new List<MenuItem>();
            menuItems.Add(new MenuItem("menu/enter", new Vector2((screenWidth - 150) / 2, (screenHeight - 40) / 2), new Vector2(150, 45)));
            menuItems.Add(new MenuItem("menu/perish", new Vector2((screenWidth - 150) / 2, ((screenHeight - 40) / 2)+50), new Vector2(150, 45)));
            itemNum = 0;
            menuItems[0].selected = true;
        }

        public override void LoadContent(ContentManager content)
        {
            foreach(MenuItem i in menuItems){
                i.LoadContent(content);
            }
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
            foreach (MenuItem i in menuItems)
            {
                i.Update(gametime);
            }

            if (controls.onPress(Keys.Enter, Buttons.A) && !changed)
            {
                if (menuItems[0].selected == true)
                {
                    changed = true;
                    changeScreen = true;
                }
                else
                {
                    game.Quit();
                }
            }
            else if (controls.onPress(Keys.Up, Buttons.DPadUp) && itemNum > 0)
            {
                foreach (MenuItem i in menuItems)
                {
                    i.selected = false;
                }
                itemNum--;
                menuItems[itemNum].selected = true;
                
            }
            else if (controls.onPress(Keys.Down, Buttons.DPadDown) && itemNum < menuItems.Count-1)
            {
                foreach (MenuItem i in menuItems)
                {
                    i.selected = false;
                }
                itemNum++;
                menuItems[itemNum].selected = true;
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
            foreach (MenuItem i in menuItems)
            {
                i.Draw(sb);
            }
        }

    }
}
