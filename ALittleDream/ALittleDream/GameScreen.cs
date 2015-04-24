using System;
using System.Collections;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace ALittleDream
{
    class GameScreen : Screen
    {
        public Entity player;
        public Entity familiar;
        public Entity door;
        private ContentManager content;
        RenderTarget2D entities;
        RenderTarget2D lights;
        Texture2D blackSquare;
        Texture2D lightmask;
        Texture2D triangleLightMask;
        public int level;
        public int initialPlayerX;
        public int initialPlayerY;
        public int initialFamiliarX;
        public int initialFamiliarY;
        public GraphicsDevice graphicsDevice;
        private Texture2D treesFront;
        private Texture2D treesBack;
        private ArrayList entityListLevel = new ArrayList();
        public ArrayList collisionObjects = new ArrayList();
        public ArrayList lightingObjects = new ArrayList();
        private bool changed;
        bool playerDied = false;
        private List<Texture2D> end;
        private int count = 0;
        private int frames = 0;


        public GameScreen(int level, int playerX, int playerY, int familiarX, int familiarY, GraphicsDevice graphicsDevice)
        {
            this.level = level;
            initialPlayerX = playerX;
            initialPlayerY = playerY;
            initialFamiliarX = familiarX;
            initialFamiliarY = familiarY;
            this.graphicsDevice = graphicsDevice;
            int playerHeight = 40;
            int playerWidth = 25;
            int familiarHeight = 20;
            int familiarWidth = 10;
            this.player = new Entity(ref playerX, ref playerY, ref playerHeight, ref playerWidth, "beta_player", Entity.collision.square, Entity.lightShape.none, Entity.movement.walking, Entity.drawIf.always, Entity.interaction.none, ref collisionObjects, ref lightingObjects);
            this.familiar = new Entity(ref familiarX, ref familiarY, ref familiarHeight, ref familiarWidth, "familiar/familiar", Entity.collision.none, Entity.lightShape.circle, Entity.movement.flying, Entity.drawIf.always, Entity.interaction.none, ref collisionObjects, ref lightingObjects);
            changeScreen = false;
            this.changed = false;
        }

        private void loadXML()
        {
            // BEGIN TILED XML PARSING
            System.IO.Stream stream = TitleContainer.OpenStream("Content/levels/" + level + ".tmx");
            XDocument doc = XDocument.Load(stream);

            List<Tile> tiles = new List<Tile>();
            foreach (XElement tileset in doc.Root.Elements("tileset"))
            {
                string light = "";
                string collision = "";
                string draw = "";
                int xOffset = 0;
                int yOffset = 0;
                double angle = 0;
                bool door = false;
                string interact = "";
                string toggle = "";
                string movement = "";
                int maxLightRange = 0;
                string image = "";
                if (tileset.Attribute("light") != null)
                {
                    light = tileset.Attribute("light").Value;
                    maxLightRange = Convert.ToInt32(tileset.Attribute("maxLightRange").Value);
                }
                if (tileset.Attribute("angle") != null)
                {
                    angle = Convert.ToDouble(tileset.Attribute("angle").Value);
                }
                if (tileset.Attribute("collision") != null)
                    collision = tileset.Attribute("collision").Value;
                if (tileset.Attribute("draw") != null)
                    draw = tileset.Attribute("draw").Value;

                if (tileset.Element("tileoffset") != null)
                {
                    xOffset = Convert.ToInt32(tileset.Element("tileoffset").Attribute("x").Value);
                    yOffset = Convert.ToInt32(tileset.Element("tileoffset").Attribute("y").Value);
                }
                if (tileset.Attribute("door") != null)
                    door = true;
                if (tileset.Attribute("interact") != null)
                    interact = tileset.Attribute("interact").Value;
                if (tileset.Attribute("toggle") != null)
                    toggle = tileset.Attribute("toggle").Value;
                if (tileset.Attribute("movement") != null)
                    movement = tileset.Attribute("movement").Value;
                if (tileset.Attribute("control") != null)
                {
                    if (Screen.input == 0)//keyboard
                        image = tileset.Element("image").Attribute("source1").Value;
                    else
                    {//controller
                        image = tileset.Element("image").Attribute("source2").Value;
                    }

                }
                else
                {
                    image = tileset.Element("image").Attribute("source").Value;
                }

                tiles.Add(new Tile(image,
                Convert.ToInt32(tileset.Attribute("tileheight").Value),
                Convert.ToInt32(tileset.Attribute("tilewidth").Value),
                xOffset, yOffset, (float)angle, light, maxLightRange, draw, collision, door, interact, toggle, movement));
            }


            XElement e = doc.Root.Element("layer").Element("data");
            int counterX = 0;
            int counterY = 0;
            foreach (XElement tile in e.Elements("tile"))
            {
                if (tile.Attribute("gid").Value != "0")
                {
                    Tile t = tiles[Convert.ToInt32(tile.Attribute("gid").Value) - 1];
                    int x = (counterX * 40) + t.offsetX;
                    int y = (counterY * 40) + t.offsetY;
                    int width = t.width;
                    int height = t.height;

                    if (t.light != "")
                    {
                        if (t.interact == "grab")
                        {
                            Entity ent = new Entity(ref x, ref y, ref width, ref height, t.image, Entity.collision.none, Entity.lightShape.circle, Entity.movement.stationary, Entity.drawIf.always, Entity.interaction.grab, ref collisionObjects, ref lightingObjects);
                            ent.maxLightRange = t.maxLightRange;
                            entityListLevel.Add(ent);
                            Console.WriteLine(t.image);
                            if (t.image == "lights/QuantumUnlit.png")
                            {
                                Entity.quantumList.Add(ent);
                                ent.isLit = false;
                                ent.d = Entity.drawIf.lit;
                            }
                        }
                        else if (t.interact == "toggle")
                        {
                            Entity ent = new Entity(ref x, ref y, ref width, ref height, t.image, Entity.collision.none, Entity.lightShape.circle, Entity.movement.stationary, Entity.drawIf.lit, Entity.interaction.toggle, ref collisionObjects, ref lightingObjects);
                            ent.maxLightRange = t.maxLightRange;
                            ent.assignToggle(t.toggle);
                            ent.isLit = false;
                            entityListLevel.Add(ent);
                        }
                        else
                        {

                            Entity.lightShape ls;
                            Entity.movement m;
                            if (t.light == "cone")
                            {
                                if (t.movement == "stationary") m = Entity.movement.stationary;
                                else m = Entity.movement.rotating;
                                ls = Entity.lightShape.cone;
                            }
                            else
                            {
                                ls = Entity.lightShape.circle;
                                m = Entity.movement.stationary;
                            }

                            // Hack to make spotlight angles consistent, will change later
                            if (t.image == "lights/movingSpotlight.png")
                                t.image = "lights/movingSpotlight2.png";

                            Entity ent = new Entity(ref x, ref y, ref width, ref height, t.image, Entity.collision.none, ls, m, Entity.drawIf.always, Entity.interaction.none, ref collisionObjects, ref lightingObjects);
                            
                            // Sets maxLightRange to the length of cone lightmask if light is a cone
                            if(t.light == "cone")
                                ent.maxLightRange = 280;
                            else
                                ent.maxLightRange = t.maxLightRange;

                            ent.angle += t.angle;
                            ent.maxAngle += t.angle;

                            entityListLevel.Add(ent);
                        }
                    }
                    else
                    {
                        if (t.interact == "toggle")
                        {
                            Entity ent = new Entity(ref x, ref y, ref width, ref height, t.image, Entity.collision.none, Entity.lightShape.none, Entity.movement.stationary, Entity.drawIf.lit, Entity.interaction.toggle, ref collisionObjects, ref lightingObjects);
                            ent.assignToggle(t.toggle);
                            entityListLevel.Add(ent);
                        }
                        else if (t.door)
                        {
                            this.door = new Entity(ref x, ref y, ref width, ref height, t.image, Entity.collision.none, Entity.lightShape.none, Entity.movement.stationary, Entity.drawIf.lit, Entity.interaction.none, ref collisionObjects, ref lightingObjects);
                        }
                        else
                        {

                            if (t.collision == "none")
                            {
                                if (t.draw == "always")
                                    entityListLevel.Add(new Entity(ref x, ref y, ref width, ref height, t.image, Entity.collision.none, Entity.lightShape.none, Entity.movement.stationary, Entity.drawIf.always, Entity.interaction.none, ref collisionObjects, ref lightingObjects));
                                else
                                    entityListLevel.Add(new Entity(ref x, ref y, ref width, ref height, t.image, Entity.collision.none, Entity.lightShape.none, Entity.movement.stationary, Entity.drawIf.lit, Entity.interaction.none, ref collisionObjects, ref lightingObjects));
                            }
                            else
                            {
                                entityListLevel.Add(new Entity(ref x, ref y, ref width, ref height, t.image, Entity.collision.square, Entity.lightShape.none, Entity.movement.stationary, Entity.drawIf.lit, Entity.interaction.none, ref collisionObjects, ref lightingObjects));
                            }
                        }
                    }
                }
                counterX++;
                if (counterX == Convert.ToInt32(doc.Root.Attribute("width").Value))
                {
                    counterX = 0;
                    counterY++;
                }
            }

            // TODO: parse rest of XML, specifically the tile specification
            // END TILED XML PARSING
        }

        public override string identify()
        {
            return "I am a Game Screen! level " + this.level;
        }

        public override void LoadContent(ContentManager content)
        {
            loadXML();
            Entity.entityList.Clear();
            Entity.entityList = entityListLevel;

            this.content = content;
            foreach (string s in player.animations)
                player.AnimatedLoadContent(content, s);
            player.image = player.spriteAnimations[0];
            foreach (string s in familiar.animations)
                familiar.AnimatedLoadContent(content, s);
            familiar.image = familiar.spriteAnimations[0];
            door.LoadContent(content);

            foreach (Entity e in Entity.entityList)
            {
                if (e.l == Entity.lightShape.none && e.i == Entity.interaction.toggle)
                {
                    foreach (string s in e.animations)
                    {
                        e.AnimatedLoadContent(content, s);

                    } e.image = e.spriteAnimations[0];
                }
                else
                    e.LoadContent(content);
            }

            var pp = graphicsDevice.PresentationParameters;
            entities = new RenderTarget2D(graphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight);
            lights = new RenderTarget2D(graphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight);
            blackSquare = content.Load<Texture2D>("blacksquare");
            lightmask = content.Load<Texture2D>("lightmask");
            triangleLightMask = content.Load<Texture2D>("triangleLightMask2");

            treesBack = content.Load<Texture2D>("treesBack");
            treesFront = content.Load<Texture2D>("treesFront");

            /**if (level == 8)
            {
                end = new List<Texture2D>();
                end.Add(content.Load<Texture2D>("end1"));
                end.Add(content.Load<Texture2D>("end2"));
                end.Add(content.Load<Texture2D>("end3"));
                end.Add(content.Load<Texture2D>("end4"));
            }**/
        }

        public override void UnloadContent(ContentManager content)
        {

        }



        public override void Update(Controls controls, GameTime gameTime, AudioMixer audioMixer)
        {
            //handle player reaching door
            if ((player.spriteX + 15) > door.spriteX && (player.spriteX + player.spriteWidth) < (door.spriteX + door.spriteWidth + 15)
    && (player.spriteY + 15) > door.spriteY && (player.spriteY + player.spriteHeight) < (door.spriteY + door.spriteHeight + 15)
                && !changed && controls.isPressed(Keys.E, Buttons.LeftShoulder))
            {
                audioMixer.playEffect("Door");
                changeScreen = true;
                changed = true;
            }


            //if player falls below game area, reset level
            if (player.spriteY > ScreenManager.screenHeight && !playerDied)
            {
                playerDied = true;
                ScreenManager.playerFellOffScreen = true;
            }


            if (player.needsNewSprite)
            {
                //Console.WriteLine("player sprite=" + player.spriteName);
                player.spriteAnimations[0] = content.Load<Texture2D>(player.animations[0]);
                player.spriteAnimations[1] = content.Load<Texture2D>(player.animations[1]);
                //TODO: other sprites
                player.needsNewSprite = false;
            }
            player.Update(controls, gameTime, audioMixer);
            familiar.Update(controls, gameTime, audioMixer);
            foreach (Entity e in Entity.entityList)
            {
                if (e.needsNewSprite)
                {
                    e.LoadContent(content);
                    e.needsNewSprite = false;
                }
                e.Update(controls, gameTime, audioMixer);
            }



        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.End();
            graphicsDevice.SetRenderTarget(entities);
            graphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();

            //if (level != 8)
            //{
            spriteBatch.Draw(treesBack, new Rectangle(-500 - (player.spriteX / 16), 0, graphicsDevice.PresentationParameters.BackBufferWidth * 2, graphicsDevice.PresentationParameters.BackBufferHeight), Color.White);
            //}
            /**else
            {
                spriteBatch.Draw(end[count], new Rectangle(0 - (player.spriteX / 16), 0, graphicsDevice.PresentationParameters.BackBufferWidth, graphicsDevice.PresentationParameters.BackBufferHeight), Color.White);
                spriteBatch.Draw(treesBack, new Rectangle(-500 - (player.spriteX / 16), 0, graphicsDevice.PresentationParameters.BackBufferWidth * 2, graphicsDevice.PresentationParameters.BackBufferHeight), Color.White);
                frames++;
                if (frames % 50 == 0 && (count == 1 || count == 3))
                    count++;
                else if (frames % 25 == 0 && (count == 0 || count == 2))
                    count++;
                if (count > 3)
                    count = 0;
            }**/

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
            foreach (Entity e in lightingObjects)
            {
                if (e.l == Entity.lightShape.circle)
                {
                    //Console.WriteLine(l.isLit);
                    var new_rect = new Rectangle(e.spriteX - (e.lightRange - e.spriteWidth), e.spriteY - (e.lightRange - e.spriteHeight), e.lightRange * 2, e.lightRange * 2);
                    spriteBatch.Draw(lightmask, new_rect, Color.White);
                    spriteBatch.Draw(lightmask, new_rect, Color.White);
                }
                else if (e.l == Entity.lightShape.cone)
                {
                    // adds 45 degrees to angle to account for rotation of lightmask image
                    spriteBatch.Draw(triangleLightMask, new Vector2(e.spriteX, e.spriteY), null, Color.White, (float)(e.angle+.79), new Vector2(0, 0), 1, SpriteEffects.None, 0);
                    //Console.WriteLine("Angle:" + e.angle+.785);
                }
            }

            spriteBatch.Draw(lightmask, new Rectangle(familiar.spriteX - (familiar.lightRange - familiar.spriteWidth), familiar.spriteY - (familiar.lightRange - familiar.spriteHeight), familiar.lightRange * 2, familiar.lightRange * 2)
, Color.White);
            spriteBatch.Draw(lightmask, new Rectangle(familiar.spriteX - (familiar.lightRange - familiar.spriteWidth), familiar.spriteY - (familiar.lightRange - familiar.spriteHeight), familiar.lightRange * 2, familiar.lightRange * 2)
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
            foreach (Entity e in Entity.entityList)
            {
                if (e.d == Entity.drawIf.always)
                    e.Draw(spriteBatch);
            }
            player.Draw(spriteBatch);
            //Console.WriteLine(player.spriteX + "," + player.spriteY);
            familiar.Draw(spriteBatch);
            //spriteBatch.Draw(treesFront, new Rectangle( (player.spriteX / 36), 0, graphicsDevice.PresentationParameters.BackBufferWidth , graphicsDevice.PresentationParameters.BackBufferHeight*2), Color.White);

        }
    }
}
