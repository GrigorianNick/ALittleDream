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
using System.Linq;
using System.Xml.Linq;

namespace ALittleDream
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public unsafe class GameLoop : Game
    {
        public static bool debug = false;
        public static GameTime gameTime;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Controls controls;
        int windowWidth, windowHeight;
        Entity player, familiar, lantern;
        RenderTarget2D entities;
        RenderTarget2D lights;
        Texture2D blackSquare;
        Texture2D lightmask;
        ScreenManager screenManager;
        public static int LIGHTOFFSET = 115;

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
            if (debug)
            {

            }
            else
            {
                System.IO.Stream stream = TitleContainer.OpenStream("Content/levels/1.tmx");
                XDocument doc = XDocument.Load(stream);

                windowWidth = Convert.ToInt32(doc.Root.Attribute("width").Value) * Convert.ToInt32(doc.Root.Attribute("tilewidth").Value);
                windowHeight = Convert.ToInt32(doc.Root.Attribute("height").Value) * Convert.ToInt32(doc.Root.Attribute("tileheight").Value);

                graphics.PreferredBackBufferWidth = windowWidth;
                graphics.PreferredBackBufferHeight = windowHeight;
                graphics.ApplyChanges();
                graphics.ToggleFullScreen();

                SplashScreen splashScreen = new SplashScreen(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
                MenuScreen menuScreen = new MenuScreen(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight, this);
                GameScreen gameScreen1 = new GameScreen(1, 200, 0, -9001, -9001, GraphicsDevice);
                GameScreen gameScreen2 = new GameScreen(2, 350, 150, -9001, -9001, GraphicsDevice);
                GameScreen gameScreen3 = new GameScreen(3, 675, 0, -9001, -9001, GraphicsDevice);
                GameScreen gameScreen4 = new GameScreen(4, 225, 0, 100, 20, GraphicsDevice);
                GameScreen gameScreen5 = new GameScreen(5, 350, 150, 100, 20, GraphicsDevice);
                GameScreen gameScreen6 = new GameScreen(6, 400, 100, -9001, -9001, GraphicsDevice);
                GameScreen gameScreen7 = new GameScreen(7, 500, 50, 480, 20, GraphicsDevice);
                GameScreen gameScreen8 = new GameScreen(8, 80, 200, -9001, -9001, GraphicsDevice);
                GameScreen gameScreen9 = new GameScreen(9, 450, 0, -9001, -9001, GraphicsDevice);
                GameScreen gameScreen10 = new GameScreen(10, 470, 280, -9001, -9001, GraphicsDevice);
                GameScreen gameScreen11 = new GameScreen(11, 880, 80, 860, 20, GraphicsDevice);
                GameScreen gameScreen12 = new GameScreen(12, 40, 280, -9001, -9001, GraphicsDevice);
                //GameScreen gameScreen8 = new GameScreen(8, 450, 0, 100, 20, GraphicsDevice);
                EndScreen end = new EndScreen(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);



                Stack<Screen> screens = new Stack<Screen>();
                //screens.Push(gameScreen8);
                screens.Push(end);
                //screens.Push(gameScreen5);
                screens.Push(gameScreen7); // Quantom & Switch puzzle
                screens.Push(gameScreen11); // Quantom & Switch puzzle
                screens.Push(gameScreen4); // Switch tutorial
                screens.Push(gameScreen10); // Quantom tutorial
                screens.Push(gameScreen2); // Lantern Puzzle
                screens.Push(gameScreen6); // Light jumping puzzle
                screens.Push(gameScreen9); // Lantern Puzzle
                screens.Push(gameScreen12); // Lantern tutorial
                screens.Push(gameScreen3); // Lighting tutorial
                screens.Push(gameScreen8); // Movement puzzle
                screens.Push(gameScreen1); // Movement tutorial
                screens.Push(menuScreen);
                screens.Push(splashScreen);
                screenManager = new ScreenManager(screens, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
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

            if (!debug)
            {
                screenManager.LoadContent(this.Content);
            }
            else
            {
                foreach (string s in player.animations)
                    player.AnimatedLoadContent(this.Content, s);
                player.image = player.spriteAnimations[0];
                //player.LoadContent(this.Content);
                foreach (string s in familiar.animations)
                    familiar.AnimatedLoadContent(this.Content, s);
                familiar.image = familiar.spriteAnimations[0];
                Console.WriteLine((player.spriteX));
                // Load all GameObject content

                foreach (Entity e in Entity.entityList)
                {
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
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            if (!debug)
            {
                screenManager.UnloadContent(this.Content);
            }
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
            /*if (controls.isPressed(Keys.NumPad6, Buttons.A))
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
                windowHeight = Math.Max(0, windowHeight - 10);
                graphics.PreferredBackBufferHeight = windowHeight;
                graphics.ApplyChanges();
            }*/
            if (controls.onPress(Keys.R, Buttons.B))
            {
                resetLevel();
            }
            /*else if (controls.onPress(Keys.Space, Buttons.Back))///TO DELETE
            {
                screenManager.skipScreen();
            }*/

            foreach (Entity e in Entity.entityList)
            {
                if (controls.gamepadConnected()) // Gamepad plugged in, time to swap out sprites
                {
                    // Begin the fat if else block
                    if (e.spriteName == "controls/down.png")
                    {
                        e.spriteName = "controls/downDpad.png";
                        e.LoadContent(Content);
                    }
                    else if (e.spriteName == "controls/up.png")
                    {
                        e.spriteName = "controls/upDpad.png";
                        e.LoadContent(Content);
                    }
                    else if (e.spriteName == "controls/left.png")
                    {
                        e.spriteName = "controls/leftDpad.png";
                        e.LoadContent(Content);
                    }
                    else if (e.spriteName == "controls/right.png")
                    {
                        e.spriteName = "controls/rightDpad.png";
                        e.LoadContent(Content);
                    }
                    else if (e.spriteName == "controls/q.png")
                    {
                        e.spriteName = "controls/rb.png";
                        e.LoadContent(Content);
                    }
                    else if (e.spriteName == "controls/e.png")
                    {
                        e.spriteName = "controls/lb.png";
                        e.LoadContent(Content);
                    }
                    else if (e.spriteName == "controls/w.png")
                    {
                        e.spriteName = "controls/aGP.png";
                        e.LoadContent(Content);
                    }
                    else if (e.spriteName == "controls/a.png")
                    {
                        e.spriteName = "controls/leftLeftStick.png";
                        e.LoadContent(Content);
                    }
                    else if (e.spriteName == "controls/d.png")
                    {
                        e.spriteName = "controls/rightLeftStick.png";
                        e.LoadContent(Content);
                    }
                }
                else
                {
                    if (e.spriteName == "controls/downDpad.png")
                    {
                        e.spriteName = "controls/down.png";
                        e.LoadContent(Content);
                    }
                    else if (e.spriteName == "controls/upDpad.png")
                    {
                        e.spriteName = "controls/up.png";
                        e.LoadContent(Content);
                    }
                    else if (e.spriteName == "controls/leftDpad.png")
                    {
                        e.spriteName = "controls/left.png";
                        e.LoadContent(Content);
                    }
                    else if (e.spriteName == "controls/rightDpad.png")
                    {
                        e.spriteName = "controls/right.png";
                        e.LoadContent(Content);
                    }
                    else if (e.spriteName == "controls/lb.png")
                    {
                        e.spriteName = "controls/q.png";
                        e.LoadContent(Content);
                    }
                    else if (e.spriteName == "controls/lb.png")
                    {
                        e.spriteName = "controls/e.png";
                        e.LoadContent(Content);
                    }
                    else if (e.spriteName == "controls/aGP.png")
                    {
                        e.spriteName = "controls/w.png";
                        e.LoadContent(Content);
                    }
                    else if (e.spriteName == "controls/leftLeftStick.png")
                    {
                        e.spriteName = "controls/a.png";
                        e.LoadContent(Content);
                    }
                    else if (e.spriteName == "controls/rightLeftStick.png")
                    {
                        e.spriteName = "controls/d.png";
                        e.LoadContent(Content);
                    }
                }
            }

            if (!debug)
            {
                screenManager.Update(controls, gameTime);
            }
            else
            {
                if (player.needsNewSprite)
                {
                    Console.WriteLine("player sprite=" + player.spriteName);
                    player.spriteAnimations[0] = this.Content.Load<Texture2D>(player.animations[0]);
                    player.spriteAnimations[1] = this.Content.Load<Texture2D>(player.animations[1]);
                    //TODO: other sprites
                    player.needsNewSprite = false;
                }
                player.Update(controls, gameTime, null);
                familiar.Update(controls, gameTime, null);
                foreach (Entity e in Entity.entityList)
                {
                    if (e.needsNewSprite)
                    {
                        e.LoadContent(this.Content);
                        e.needsNewSprite = false;
                    }
                    e.Update(controls, gameTime, null);
                }
            }
            //if (Entity.debugLighting)
            base.Update(gameTime);
        }

        public void resetLevel()
        {
            screenManager.restartLevel();
            
            /*player.spriteX = 10;
            player.spriteY = 10;
            player.momentumX = 0;
            player.momentumY = 0;
            familiar.spriteX = 100;
            familiar.spriteY = 20;
            familiar.momentumX = 0;
            familiar.momentumY = 0;
            lantern.spriteX = 300;
            lantern.spriteY = 50;
            lantern.momentumX = 0;
            lantern.momentumY = 0;*/
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            if (debug)
            {
            }
            else
            {
                spriteBatch.Begin(SpriteSortMode.Immediate, null);
                if (!screenManager.play)
                    screenManager.Draw(spriteBatch);
                spriteBatch.End();

            }
            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }

        public void Quit()
        {
            this.Exit();
        }
    }
}
