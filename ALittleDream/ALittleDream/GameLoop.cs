#region Using Statements
using System;
using System.Collections;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
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

namespace ALittleDream
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class GameLoop : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Player player;
        Lantern lantern;
        Controls controls;
        int windowWidth = 500;
        int windowHeight = 500;

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
            // TODO: Add your initialization logic here
            player = new Player(50, 50, 40, 40, "beta_player.png");
            lantern = new Lantern(180, 50, 20, 20, "beta_lantern.png");
            GameObject.AddGameObject(new Lightbulb(180, 320, 20, 20, "beta_lightbulb.png"));
            GameObject.AddGameObject(new Lightbulb(50, 350, 20, 20, "beta_lightbulb.png"));
            GameObject.AddGameObject(new Crate(200, 0, 50, 50, "beta_crate.png"));
            //GameObject.AddGameObject(new Crate(0, 200, 1000, 10, "beta_crate.png"));
            Platform.AddPlatform(new brick(200, 50, 50, 50, "beta_brick.png"));
            Platform.AddPlatform(new brick(200, 100, 50, 50, "beta_brick.png"));
            Platform.AddPlatform(new brick(200, 150, 50, 50, "beta_brick.png"));
            Platform.AddPlatform(new brick(200, 200, 50, 50, "beta_brick.png"));
            Platform.AddPlatform(new brick(200, 300, 50, 50, "beta_brick.png"));
            Platform.AddPlatform(new brick(0, 400, 50, 50, "beta_brick.png"));
            Platform.AddPlatform(new brick(50, 400, 50, 50, "beta_brick.png"));
            Platform.AddPlatform(new brick(100, 400, 50, 50, "beta_brick.png"));
            Platform.AddPlatform(new brick(150, 400, 50, 50, "beta_brick.png"));
            Platform.AddPlatform(new brick(200, 400, 50, 50, "beta_brick.png"));
            Platform.AddPlatform(new brick(250, 400, 50, 50, "beta_brick.png"));
            Platform.AddPlatform(new brick(300, 400, 50, 50, "beta_brick.png"));
            Platform.AddPlatform(new brick(350, 400, 50, 50, "beta_brick.png"));
            Platform.AddPlatform(new brick(400, 400, 50, 50, "beta_brick.png"));
            Platform.AddPlatform(new brick(450, 400, 50, 50, "beta_brick.png"));

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
            lantern.LoadContent(this.Content);
            // Load all GameObject content
            foreach (GameObject obj in GameObject.objects)
            {
                obj.LoadContent(this.Content);
            }
            foreach (Platform plat in Platform.platforms)
            {
                plat.LoadContent(this.Content);
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
            lantern.Update(controls, gameTime);
            foreach (GameObject obj in GameObject.objects)
            {
                obj.Update(controls, gameTime);
            }

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
            foreach (GameObject obj in GameObject.objects)
            {
                //obj.Draw(spriteBatch);
                obj.render(spriteBatch);
            }
            foreach (Platform plat in Platform.platforms)
            {
                plat.render(spriteBatch);
            }
            lantern.Draw(spriteBatch);
            spriteBatch.End();
            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
