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
    class GameScreen: Screen
    {
        private string path;
        public Entity player;
        public Entity familiar;
        public Entity door;
        private ContentManager content;
        private GraphicsDevice graphicsDevice;
        RenderTarget2D entities;
        RenderTarget2D lights;
        Texture2D blackSquare;
        Texture2D lightmask;
        public static int LIGHTOFFSET = 115;
        private int level;

        public GameScreen(int level, int playerX, int playerY, int familiarX, int familiarY, int doorPositionX, int doorPositionY, List<Vector2> lights, GraphicsDevice graphicsDevice)
        {
            this.graphicsDevice = graphicsDevice;
            int playerHeight = 40;
            int playerWidth = 25;
            int familiarHeight = 20;
            int familiarWidth = 10;
            int doorHeight = 50;
            int doorWidth = 40;
            this.level = level;
            this.player = new Entity(ref playerX, ref playerY, ref playerHeight, ref playerWidth, "beta_player", Entity.collision.square, Entity.lightShape.none, Entity.movement.walking, Entity.drawIf.always, Entity.interaction.none);
            this.familiar = new Entity(ref familiarX, ref familiarY, ref familiarHeight, ref familiarWidth, "familiar/familiar", Entity.collision.none, Entity.lightShape.circle, Entity.movement.flying, Entity.drawIf.always, Entity.interaction.none);
            this.door = new Entity(ref doorPositionX, ref doorPositionY, ref doorHeight, ref doorWidth, "door", Entity.collision.none, Entity.lightShape.none, Entity.movement.stationary, Entity.drawIf.always, Entity.interaction.none);
            changeScreen = false;
            loadXML();
        }

        private void loadXML(){

        }


        public override void LoadContent(ContentManager content)
        {

            if ((player.spriteX + 15) > door.spriteX && (player.spriteX + player.spriteWidth) < (door.spriteX + door.spriteWidth + 15)
                && (player.spriteY + 15) > door.spriteY && (player.spriteY + player.spriteHeight) < (door.spriteY + door.spriteHeight + 15))
                changeScreen = true;
            foreach (string s in player.animations)
                player.AnimatedLoadContent(content, s);
            player.image = player.spriteAnimations[0];
            foreach (string s in familiar.animations)
                familiar.AnimatedLoadContent(content, s);
            familiar.image = familiar.spriteAnimations[0];
            door.image = content.Load<Texture2D>("door");

            foreach (Entity e in Entity.entityList)
            {
                e.LoadContent(content);
            }

            var pp = graphicsDevice.PresentationParameters;
            entities = new RenderTarget2D(graphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight);
            lights = new RenderTarget2D(graphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight);
            blackSquare = content.Load<Texture2D>("blacksquare");
            lightmask = content.Load<Texture2D>("lightmask");
        }

        public override void UnloadContent(ContentManager content)
        {

        }

        public override void Update(Controls controls, GameTime gameTime)
        {
            if (player.needsNewSprite)
            {
                Console.WriteLine("player sprite=" + player.spriteName);
                player.spriteAnimations[0] = content.Load<Texture2D>(player.animations[0]);
                player.spriteAnimations[1] = content.Load<Texture2D>(player.animations[1]);
                //TODO: other sprites
                player.needsNewSprite = false;
            }
            player.Update(controls, gameTime);
            familiar.Update(controls, gameTime);
            foreach (Entity e in Entity.entityList)
            {
                if (e.needsNewSprite)
                {
                    e.LoadContent(content);
                    e.needsNewSprite = false;
                }
                e.Update(controls, gameTime);
            }
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.End();
            graphicsDevice.SetRenderTarget(entities);
            graphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();
            foreach (Entity e in Entity.entityList)
            {
                e.Draw(spriteBatch);
            }
            door.Draw(spriteBatch);
            spriteBatch.End();
            graphicsDevice.SetRenderTarget(null);

            //Set render target to lightMask then draw lightmask.png every instance where there is a light
            graphicsDevice.SetRenderTarget(lights);
            graphicsDevice.Clear(Color.Black);

            // Create a Black Background
            spriteBatch.Begin();
            spriteBatch.Draw(blackSquare, new Vector2(0, 0), new Rectangle(0, 0, 800, 800), Color.White);
            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive);

            // Draw out lightmasks based on torch positions.
            foreach (Entity l in Entity.lightingObjects)
            {
                var new_rect = new Rectangle(l.spriteX - (LIGHTOFFSET - l.spriteWidth), l.spriteY - (LIGHTOFFSET - l.spriteHeight), LIGHTOFFSET * 2, LIGHTOFFSET * 2);
                spriteBatch.Draw(lightmask, new_rect, Color.White);
                spriteBatch.Draw(lightmask, new_rect, Color.White);
            }

            spriteBatch.Draw(lightmask, new Rectangle(familiar.spriteX - (LIGHTOFFSET - familiar.spriteWidth), familiar.spriteY - (LIGHTOFFSET - familiar.spriteHeight), LIGHTOFFSET * 2, LIGHTOFFSET * 2)
, Color.White);
            spriteBatch.Draw(lightmask, new Rectangle(familiar.spriteX - (LIGHTOFFSET - familiar.spriteWidth), familiar.spriteY - (LIGHTOFFSET - familiar.spriteHeight), LIGHTOFFSET * 2, LIGHTOFFSET * 2)
, Color.White);
            spriteBatch.End();

            graphicsDevice.SetRenderTarget(null);

            //Draw everything to screen w/ blendstate
            graphicsDevice.Clear(Color.Black);
            spriteBatch.Begin(SpriteSortMode.Immediate, null);
            spriteBatch.Draw(entities, new Vector2(0, 0), Color.White);
            spriteBatch.End();

            BlendState blend = new BlendState();
            blend.ColorBlendFunction = BlendFunction.Add;
            blend.ColorSourceBlend = Blend.DestinationColor;
            blend.ColorDestinationBlend = Blend.Zero;

            spriteBatch.Begin(SpriteSortMode.Immediate, blend);
            spriteBatch.Draw(lights, new Vector2(0, 0), Color.White);
            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.Immediate, null);
            player.Draw(spriteBatch);
            familiar.Draw(spriteBatch);
        }
    }
}
