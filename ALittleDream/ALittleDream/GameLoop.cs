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
            {/**
                // BEGIN TILED XML PARSING
                System.IO.Stream stream = TitleContainer.OpenStream("Content/levels/test.tmx");
                XDocument doc = XDocument.Load(stream);

                windowWidth = Convert.ToInt32(doc.Root.Attribute("width").Value) * Convert.ToInt32(doc.Root.Attribute("tilewidth").Value);
                windowHeight = Convert.ToInt32(doc.Root.Attribute("height").Value) * Convert.ToInt32(doc.Root.Attribute("tileheight").Value);

                graphics.PreferredBackBufferWidth = windowWidth;
                graphics.PreferredBackBufferHeight = windowHeight;
                graphics.ApplyChanges();

                //initialize player
                int playerX = 10, playerY = 10, playerHeight = 40, playerWidth = 25;
                player = new Entity(ref playerX, ref playerY, ref playerHeight, ref playerWidth, "beta_player.png", Entity.collision.square, Entity.lightShape.none, Entity.movement.walking, Entity.drawIf.always, Entity.interaction.none);

                //initialize familiar
                int familiarX = 100, familiarY = 20, familiarHeight = 20, familiarWidth = 10;
                familiar = new Entity(ref familiarX, ref familiarY, ref familiarHeight, ref familiarWidth, "familiar/familiar.png", Entity.collision.none, Entity.lightShape.circle, Entity.movement.flying, Entity.drawIf.always, Entity.interaction.none);
                familiar.setMaxLightRange(115);

                //initialize blocks
                int[] blockX = new int[]{
                10, 200, 250, 300, 350, 400, 350, 350, -40, 440, 540
            };
                int[] blockY = new int[]{
                100, 200, 200, 200, 200, 200, 150, 100, 50, 320, 320
            };
                int[] blockHeight = new int[]{
                50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50
            };
                int[] blockWidth = new int[]{
                50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50
            };
                for (int i = 0; i < blockHeight.Length; i++)
                {
                    Entity.AddEntityObject(new Entity(ref blockX[i], ref blockY[i], ref blockHeight[i], ref blockWidth[i], "beta_brick.png", Entity.collision.square, Entity.lightShape.none, Entity.movement.stationary, Entity.drawIf.lit, Entity.interaction.none));
                }

                //initialize lantern
                int lanternX = 300,
                    lanternY = 50,
                    lanternHeight = 30,
                    lanternWidth = 30;
                lantern = new Entity(ref lanternX, ref lanternY, ref lanternHeight, ref lanternWidth, "lights/lantern.png", Entity.collision.square, Entity.lightShape.circle, Entity.movement.physics, Entity.drawIf.lit, Entity.interaction.grab);
                Entity.AddEntityObject(lantern);
                lantern.setMaxLightRange(115);

                //initialize lightbulbs (beta lampposts?)
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
                for (int i = 0; i < lightX.Length; i++)
                {
                    Entity.AddEntityObject(new Entity(ref lightX[i], ref lightY[i], ref lightHeight[i], ref lightWidth[i], "beta_lightbulb.png", Entity.collision.none, Entity.lightShape.circle, Entity.movement.stationary, Entity.drawIf.always, Entity.interaction.none));
                }

                //initialize static light sizes
                ((Entity)Entity.lightingObjects[2]).setMaxLightRange(115);
                ((Entity)Entity.lightingObjects[3]).setMaxLightRange(115);

                int lightSwitchX = 300;
                int lightSwitchY = 300;
                int lightSwitchHeight = 40;
                int lightSwitchWidth = 40;
                Entity lightSwitch = new Entity(ref lightSwitchX, ref lightSwitchY, ref lightSwitchHeight, ref lightSwitchWidth, "beta_brick_old.png", Entity.collision.none, Entity.lightShape.none, Entity.movement.stationary, Entity.drawIf.lit, Entity.interaction.toggle);

                int switchableX = 500;
                int switchableY = 300;
                int switchableHeight = 80;
                int switchableWidth = 20;
                Entity switchable = new Entity(ref switchableX, ref switchableY, ref switchableHeight, ref switchableWidth, "lights/lamppostShort.png", Entity.collision.none, Entity.lightShape.circle, Entity.movement.stationary, Entity.drawIf.lit, Entity.interaction.toggle);
                switchable.isLit = false;
                lightSwitch.setMaxLightRange(80);
                switchable.setMaxLightRange(80);

                Entity.AddEntityObject(lightSwitch);
                Entity.AddEntityObject(switchable);
                lightSwitch.assignToggle("toggle1");
                switchable.assignToggle("toggle1");**/

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

                SplashScreen splashScreen = new SplashScreen(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
                MenuScreen menuScreen = new MenuScreen(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight, this);
                GameScreen gameScreen1 = new GameScreen(1, 150, 0, 100, 20, GraphicsDevice);
                GameScreen gameScreen2 = new GameScreen(2, 250, 150, 100, 20, GraphicsDevice);
                GameScreen gameScreen3 = new GameScreen(3, 600, 0, 100, 20, GraphicsDevice);
                GameScreen gameScreen4 = new GameScreen(4, 100, 0, 100, 20, GraphicsDevice);

                Stack<Screen> screens = new Stack<Screen>();
                screens.Push(gameScreen4);
                screens.Push(gameScreen3);
                screens.Push(gameScreen2);
                screens.Push(gameScreen1);
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
            else if (controls.onPress(Keys.Space, Buttons.A))
            {
                screenManager.skipScreen();
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
                player.Update(controls, gameTime);
                familiar.Update(controls, gameTime);
                foreach (Entity e in Entity.entityList)
                {
                    if (e.needsNewSprite)
                    {
                        e.LoadContent(this.Content);
                        e.needsNewSprite = false;
                    }
                    e.Update(controls, gameTime);
                }
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
            lantern.spriteX = 300;
            lantern.spriteY = 50;
            lantern.momentumX = 0;
            lantern.momentumY = 0;
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            if (debug)
            {/**
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
                    var new_rect = new Rectangle(l.spriteX - (l.lightRange - l.spriteWidth), l.spriteY - (l.lightRange - l.spriteHeight), l.lightRange * 2, l.lightRange * 2);
                    spriteBatch.Draw(lightmask, new_rect, Color.White);
                    spriteBatch.Draw(lightmask, new_rect, Color.White);
                }

                spriteBatch.Draw(lightmask, new Rectangle(familiar.spriteX - (lantern.lightRange - familiar.spriteWidth), familiar.spriteY - (lantern.lightRange - familiar.spriteHeight), lantern.lightRange * 2, lantern.lightRange * 2)
    , Color.White);
                spriteBatch.Draw(lightmask, new Rectangle(familiar.spriteX - (lantern.lightRange - familiar.spriteWidth), familiar.spriteY - (lantern.lightRange - familiar.spriteHeight), lantern.lightRange * 2, lantern.lightRange * 2)
    , Color.White);
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
                spriteBatch.End();**/
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
