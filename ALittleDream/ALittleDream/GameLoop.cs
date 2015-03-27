﻿#region Using Statements
using System;
using System.Collections;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
using System.Collections.Generic;
#endregion

/* 
 * What this prototype does:
 *      Sets up user and lantern control
 *      Object collision
 *      Illumination-controlled collision
 * What this prototype does not do:
 *      Physics
 *      Good object collision (it doesn't do not-squares)
 *      Variable illumination
 * 
 * Controls:
 *      Arrow keys for lantern
 *      WASD for player
 *      Numpad4 = reduce window width
 *      Numpad6 = increase window width
 *      Numpad8 = reduce window height
 *      Numpad2 = incrase window height
 *      ESC = Exit game
 * 
 * Code layout:
 * 
 *      Base classes: GameObject and Platform.
 *          GameObject is meant for things that move around and collide, like crates or players.
 *          Platforms are meant for things that collide but don't budge.
 *      
 *      Derived classes: brick, crate, lightbulb
 *          Bricks are placeholder platforms.
 *          Crates are crates, and will probably make it into the final game.
 *              You can push these around.
 *              They fall at a constant rate.
 *          Lightbulbs are intangable objects that provide light.
 *              There's a bug where you can push them to the right.
 * 
 */

using ALittleDream;

namespace ALittleDream
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public unsafe class GameLoop : Game
    {
        public static GameTime gameTime;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Controls controls;
        int windowWidth = 500;
        int windowHeight = 500;
        Entity player, familiar;
        RenderTarget2D entities;
        RenderTarget2D lights;
        Texture2D blackSquare;
        Texture2D lightmask;
        public const int LIGHTOFFSET = 115;

        public GameLoop()
            : base()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            System.IO.StreamReader file = new System.IO.StreamReader("Content/levels/test.txt");
            string line;
            int counter = 0;
            while ((line = file.ReadLine()) != null)
            {
                Console.WriteLine(line);
                counter++;
            }
            Console.WriteLine(counter);
            file.Close();
            // TODO: Add your initialization logic here

            int playerX = 10, playerY = 10, playerHeight = 40, playerWidth = 30;
            int familiarX = 100, familiarY = 20, familiarHeight = 15, familiarWidth = 10;
            player = new Entity(ref playerX, ref playerY, ref playerHeight, ref playerWidth, "beta_player.png", Entity.collision.square, Entity.lightShape.none, Entity.movement.walking, Entity.drawIf.always, Entity.interaction.none);
            familiar = new Entity(ref familiarX, ref familiarY, ref familiarHeight, ref familiarWidth, "beta_lantern.png", Entity.collision.none, Entity.lightShape.circle, Entity.movement.flying, Entity.drawIf.always, Entity.interaction.none);

            int[] blockX = new int[]{
                10, 200, 250, 300, 350, 400, 350, 350, -40
            };
            int[] blockY = new int[]{
                100, 200, 200, 200, 200, 200, 150, 100, 50
            };
            int[] blockHeight = new int[]{
                50, 50, 50, 50, 50, 50, 50, 50, 50
            };
            int[] blockWidth = new int[]{
                50, 50, 50, 50, 50, 50, 50, 50, 50
            };

            for (int i = 0; i < blockHeight.Length; i++)
            {
                Entity.AddEntityObject(new Entity(ref blockX[i], ref blockY[i], ref blockHeight[i], ref blockWidth[i], "beta_brick.png", Entity.collision.square, Entity.lightShape.none, Entity.movement.stationary, Entity.drawIf.lit, Entity.interaction.none));
            }

            int[] lightX = new int[]{
                60, 200
            };
            int[] lightY = new int[]{
                100, 150
            };
            int[] lightHeight = new int[]{
                20, 20
            };
            int[] lightWidth = new int[]{
                20, 20
            };


            for (int i = 0; i < 2; i++)
            {
                Entity.AddEntityObject(new Entity(ref lightX[i], ref lightY[i], ref lightHeight[i], ref lightWidth[i], "beta_lightbulb.png", Entity.collision.none, Entity.lightShape.circle, Entity.movement.stationary, Entity.drawIf.always, Entity.interaction.none));
            }

            //GameObject.objects = new ArrayList();

            base.Initialize();

            //Joystick.Init();
            controls = new Controls();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            player.LoadContent(this.Content);
            familiar.LoadContent(this.Content);
            Console.WriteLine((player.spriteX));
            // Load all GameObject content
            
            foreach (Entity e in Entity.entityList) {
            
                Console.WriteLine((e.spriteX));
                e.LoadContent(this.Content);
            }

            //loading render targets for Light Mask
            var pp = GraphicsDevice.PresentationParameters;
            entities = new RenderTarget2D(GraphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight);
            lights = new RenderTarget2D(GraphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight);
            blackSquare = Content.Load<Texture2D>("blacksquare");
            lightmask = Content.Load<Texture2D>("lightmask");
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            controls.Update();
            // Window sizing with numpad
            if (controls.isPressed(Keys.NumPad6, Buttons.A))
            {
                windowWidth += 10;
                graphics.PreferredBackBufferWidth = windowWidth;
                graphics.ApplyChanges();
            }
            else if (controls.isPressed(Keys.NumPad4, Buttons.A))
            {
                windowWidth = Math.Max(0, windowWidth - 10);
                graphics.PreferredBackBufferWidth = windowWidth;
                graphics.ApplyChanges();
            }
            if (controls.isPressed(Keys.NumPad2, Buttons.A))
            {
                windowHeight += 10;
                graphics.PreferredBackBufferHeight = windowHeight;
                graphics.ApplyChanges();
            }
            else if (controls.isPressed(Keys.NumPad8, Buttons.A))
            {
                windowHeight = Math.Max(0,windowHeight - 10);
                graphics.PreferredBackBufferHeight = windowHeight;
                graphics.ApplyChanges();
            }
            else if (controls.onPress(Keys.R, Buttons.A))
            {
                resetLevel();
            }
            player.Update(controls, gameTime);
            familiar.Update(controls, gameTime);
            foreach (Entity e in Entity.entityList)
            {
                e.Update(controls, gameTime);
            }
            //if (Entity.debugLighting)
            base.Update(gameTime);
        }

        public void resetLevel()
        {
            player.spriteX = 10;
            player.spriteY = 10;
            player.momentumX = 0;
            player.momentumY = 0;
            familiar.spriteX = 100;
            familiar.spriteY = 20;
            familiar.momentumX = 0;
            familiar.momentumY = 0;
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            ///**
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            player.Draw(spriteBatch);
            familiar.Draw(spriteBatch);
            foreach (Entity e in Entity.entityList)
            {
                e.Draw(spriteBatch);
            }
            spriteBatch.End();
            //**/
            //Light Shader code: uncomment code below and comment code above to use
            //Set render target to entities then draw all entities
            GraphicsDevice.SetRenderTarget(entities);
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();
            foreach (Entity e in Entity.entityList)
            {                
                e.Draw(spriteBatch);
            }
            spriteBatch.End();
            GraphicsDevice.SetRenderTarget(null);

            //Set render target to lightMask then draw lightmask.png every instance where there is a light
            GraphicsDevice.SetRenderTarget(lights);
            GraphicsDevice.Clear(Color.Black);

            // Create a Black Background
            spriteBatch.Begin();
            spriteBatch.Draw(blackSquare, new Vector2(0, 0), new Rectangle(0, 0, 800, 800), Color.White);
            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive);

            // Draw out lightmasks based on torch positions.
            foreach (Entity l in Entity.lightingObjects)
            {
                var new_pos = new Vector2((float)l.spriteX - LIGHTOFFSET, (float)l.spriteY - LIGHTOFFSET);
                spriteBatch.Draw(lightmask, new_pos, Color.White);
            }
            spriteBatch.Draw(lightmask, new Vector2((float)familiar.spriteX - LIGHTOFFSET, (float)familiar.spriteY - LIGHTOFFSET), Color.White);
            spriteBatch.End();

            GraphicsDevice.SetRenderTarget(null);

            //Draw everything to screen w/ blendstate
            GraphicsDevice.Clear(Color.Black);
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
            spriteBatch.End();
            
            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
