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
            player = new Player(50, 50, 50, 50, "beta_player.png");
            lantern = new Lantern(180, 50, 20, 20, "beta_lantern.png");
            GameObject.AddGameObject(new Crate(200, 0, 50, 50, "beta_crate.png"));
            //GameObject.AddGameObject(new Crate(0, 200, 1000, 10, "beta_crate.png"));
            Platform.AddPlatform(new brick(200, 50, 50, 50, "beta_brick.png"));
            Platform.AddPlatform(new brick(200, 100, 50, 50, "beta_brick.png"));
            Platform.AddPlatform(new brick(200, 150, 50, 50, "beta_brick.png"));
            Platform.AddPlatform(new brick(200, 200, 50, 50, "beta_brick.png"));
            Platform.AddPlatform(new brick(200, 250, 50, 50, "beta_brick.png"));
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
            graphics.PreferredBackBufferWidth = 1024;
            graphics.PreferredBackBufferHeight = 576;
            graphics.ApplyChanges(); // Really important

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
                obj.Draw(spriteBatch);
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
