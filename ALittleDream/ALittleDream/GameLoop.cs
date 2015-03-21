#region Using Statements
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

            int playerX = 10, playerY = 10, playerHeight = 40, playerWidth = 40;
            int familiarX = 20, familiarY = 20, familiarHeight = 10, familiarWidth = 10;
            player = new Entity(ref playerX, ref playerY, ref playerHeight, ref playerWidth, "beta_player.png", new SquareCollision(), new NoLighting(), new Walking(), new MyDraw(MyDraw.DrawLighting.always), new NoInteraction());
            familiar = new Entity(ref familiarX, ref familiarY, ref familiarHeight, ref familiarWidth, "beta_lantern.png", new SquareCollision(), new CircleLighting(), new Flying(), new MyDraw(MyDraw.DrawLighting.always), new NoInteraction());


            int[] blockX = new int[]{
                100, 200
            };
            int[] blockY = new int[]{
                100, 200
            };
            int[] blockHeight = new int[]{
                50, 50
            };
            int[] blockWidth = new int[]{
                50, 50
            };

            for (int i = 0; i < 2; i++)
            {
                Entity.AddEntityObject(new Entity(ref blockX[i], ref blockY[i], ref blockHeight[i], ref blockWidth[i], "beta_brick.png", new SquareCollision(), new NoLighting(), new NoMovement(), new MyDraw(MyDraw.DrawLighting.ifLit), new NoInteraction()));
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
            player.Update(controls, gameTime);
            familiar.Update(controls, gameTime);
            
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            player.Draw(spriteBatch);
            familiar.Draw(spriteBatch);
            foreach (Entity e in Entity.entityList)
            {                
                e.Draw(spriteBatch);
            }
            spriteBatch.End();
            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
