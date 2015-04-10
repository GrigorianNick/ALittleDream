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
    class MenuItem
    {
        private Texture2D image;
        private string path;
        private Vector2 position;
        private Vector2 size;
        private float alpha;
        public bool selected;
        private bool increase;

        public MenuItem(string path, Vector2 position, Vector2 size)
        {
            this.path = path;
            this.position = position;
            this.size = size;
            selected = false;

        }

        public void LoadContent(ContentManager content)
        {
            image = content.Load<Texture2D>(path);
        }

        public void UnloadContent(ContentManager content)
        {
            content.Unload();
        }

        public void Update(GameTime gametime)
        {
            if (selected)
            {
                if (!increase)
                    alpha -= (float)gametime.ElapsedGameTime.TotalSeconds;
                else
                    alpha += (float)gametime.ElapsedGameTime.TotalSeconds;

                if (alpha < 0.0f)
                {
                    increase = true;
                    alpha = 0.0f;
                }
                else if (alpha > 1.0f)
                {
                    increase = false;
                    alpha = 1.0f;
                }

            }
            else
            {
                alpha = 1.0f;
            }
        } 

        public void Draw(SpriteBatch sb)
        {
            sb.Draw(image, new Rectangle((int)position.X, (int)position.Y, (int)size.X, (int)size.Y), Color.White * alpha);

        }

    }
}
