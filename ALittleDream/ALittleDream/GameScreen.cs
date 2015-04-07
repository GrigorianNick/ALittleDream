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
    class GameScreen: Screen
    {
        public Entity player;
        public Entity familiar;
        public Entity door;
        private ContentManager content;
        RenderTarget2D entities;
        RenderTarget2D lights;
        Texture2D blackSquare;
        Texture2D lightmask;
        public static int LIGHTOFFSET = 115;
        public int level;
        public int initialPlayerX;
        public int initialPlayerY;
        public int initialFamiliarX;
        public int initialFamiliarY;
        public int initialDoorPositionX;
        public int initialDoorPositionY;
        public List<Vector2> initialLights;
        public GraphicsDevice graphicsDevice;
        private Texture2D treesFront;
        private Texture2D treesBack;
        private ArrayList entityListLevel = new ArrayList();

        public GameScreen(int level, int playerX, int playerY, int familiarX, int familiarY, int doorPositionX, int doorPositionY, GraphicsDevice graphicsDevice)
        {
            this.level = level;
            initialPlayerX = playerX;
            initialPlayerY = playerY;
            initialFamiliarX = familiarX;
            initialFamiliarY = familiarY;
            initialDoorPositionX = doorPositionX;
            initialDoorPositionY = doorPositionY;
            this.graphicsDevice = graphicsDevice;
            int playerHeight = 40;
            int playerWidth = 25;
            int familiarHeight = 20;
            int familiarWidth = 10;
            int doorHeight = 50;
            int doorWidth = 40;
            this.player = new Entity(ref playerX, ref playerY, ref playerHeight, ref playerWidth, "beta_player", Entity.collision.square, Entity.lightShape.none, Entity.movement.walking, Entity.drawIf.always, Entity.interaction.none);
            this.familiar = new Entity(ref familiarX, ref familiarY, ref familiarHeight, ref familiarWidth, "familiar/familiar", Entity.collision.none, Entity.lightShape.circle, Entity.movement.flying, Entity.drawIf.always, Entity.interaction.none);
            this.door = new Entity(ref doorPositionX, ref doorPositionY, ref doorHeight, ref doorWidth, "door", Entity.collision.none, Entity.lightShape.none, Entity.movement.stationary, Entity.drawIf.always, Entity.interaction.none);
            changeScreen = false;
            loadXML();
        }

        private void loadXML(){
            // BEGIN TILED XML PARSING
            System.IO.Stream stream = TitleContainer.OpenStream("Content/levels/"+level+".tmx");
            XDocument doc = XDocument.Load(stream);

            List<Tile> tiles = new List<Tile>();
            foreach (XElement tileset in doc.Root.Elements("tileset"))
            {
                string light = "";
                string collision = "";
                string draw = "";
                int xOffset = 0;
                int yOffset = 0;
                if (tileset.Attribute("light") != null)
                    light = tileset.Attribute("light").Value;
                if (tileset.Attribute("collision") != null)
                    collision = tileset.Attribute("collision").Value;
                if (tileset.Attribute("draw") != null)
                    draw = tileset.Attribute("draw").Value;

                if (tileset.Element("tileoffset") != null)
                {
                    xOffset = Convert.ToInt32(tileset.Element("tileoffset").Attribute("x").Value);
                    yOffset = Convert.ToInt32(tileset.Element("tileoffset").Attribute("y").Value);
                }

                tiles.Add(new Tile(tileset.Element("image").Attribute("source").Value,
                Convert.ToInt32(tileset.Attribute("tileheight").Value),
                Convert.ToInt32(tileset.Attribute("tilewidth").Value),
                xOffset, yOffset, light, draw, collision));
            }


            XElement e = doc.Root.Element("layer").Element("data");
            int counterX = 0;
            int counterY = 0;
            foreach (XElement tile in e.Elements("tile"))
            {
                if (tile.Attribute("gid").Value != "0")
                {
                    Tile t = tiles[Convert.ToInt32(tile.Attribute("gid").Value) - 1];
                    int x = (counterX * 40);
                    int y = (counterY * 40);
                    int width = t.width;
                    int height = t.height;
                    if (t.light != "")
                    {
                            Entity.AddEntityObject(new Entity(ref x, ref y, ref width, ref height, t.image, Entity.collision.none, Entity.lightShape.circle, Entity.movement.stationary, Entity.drawIf.always, Entity.interaction.none));
                    }
                    else
                    {
                        if (t.collision == "none")
                        {
                            Entity.AddEntityObject(new Entity(ref x, ref y, ref width, ref height, t.image, Entity.collision.none, Entity.lightShape.none, Entity.movement.stationary, Entity.drawIf.always, Entity.interaction.none));

                        }
                        else
                        {
                            Console.WriteLine("yescollision");
                            Entity.AddEntityObject(new Entity(ref x, ref y, ref width, ref height, t.image, Entity.collision.square, Entity.lightShape.none, Entity.movement.stationary, Entity.drawIf.lit, Entity.interaction.none));
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


        public override void LoadContent(ContentManager content)
        {
            //Entity.entityList = entityListLevel;
            this.content = content;
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

            treesBack = content.Load<Texture2D>("treesBack");
            treesFront = content.Load<Texture2D>("treesFront");
        }

        public override void UnloadContent(ContentManager content)
        {

        }

        public override void Update(Controls controls, GameTime gameTime)
        {
            if ((player.spriteX + 15) > door.spriteX && (player.spriteX + player.spriteWidth) < (door.spriteX + door.spriteWidth + 15)
    && (player.spriteY + 15) > door.spriteY && (player.spriteY + player.spriteHeight) < (door.spriteY + door.spriteHeight + 15)
                && controls.isPressed(Keys.Up, Buttons.DPadUp))
                changeScreen = true;

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
            //spriteBatch.Draw(treesBack, new Rectangle(-500 - (player.spriteX / 24), 0, graphicsDevice.PresentationParameters.BackBufferWidth * 2, graphicsDevice.PresentationParameters.BackBufferHeight), Color.White);
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
            foreach (Entity e in Entity.entityList)
            {
                if(e.d == Entity.drawIf.always)
                    e.Draw(spriteBatch);
            }
            player.Draw(spriteBatch);
            Console.WriteLine(player.spriteX + "," + player.spriteY);
            familiar.Draw(spriteBatch);
            //spriteBatch.Draw(treesFront, new Rectangle(-500 - (player.spriteX / 36), 0, graphicsDevice.PresentationParameters.BackBufferWidth * 2, graphicsDevice.PresentationParameters.BackBufferHeight), Color.White);

        }
    }
}
