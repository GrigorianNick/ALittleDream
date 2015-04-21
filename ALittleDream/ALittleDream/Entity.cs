using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Audio;

namespace ALittleDream
{
    public unsafe class Entity : Sprite
    {
        public static ArrayList entityList = new ArrayList();
        public ArrayList collisionObjects = new ArrayList();
        public ArrayList lightingObjects = new ArrayList();

        //TO BE TWEAKED
        public static int grabDistance = 50;
        public static int toggleDistance = 50;
        public static int lightToggleRate = 12; //number of pixels per frame lighting changes by when toggled
        public int maxMomentum = 7;
        public static float maxAngle = (float) Math.PI / 4; //for rotating lights
        public static float rotateRate = 0.008F; //for rotating lights

        public collision c;
        public lightShape l;
        public movement m;
        public drawIf d;
        public interaction i;
        public int momentumX, momentumY;
        public int lightRange = 0; //current lighting amount
        public int maxLightRange = 0; //max light range
        public bool isLit = true; //whether current lighting amount heading towards max or towards 0
        public bool rotatingClockWise = true; //for rotating lights
        public bool debugDistances = false;
        public bool debugLighting = false;
        public List<String> animations = new List<String>();
        int frames = 0;
        int animation = 0;
        public bool isHoldingLantern = false;
        public bool needsNewSprite = false;
        public String toggleSet = "";

        private bool switchGlowing;

        public enum collision
        {
            square, none
        }

        public enum lightShape
        {
            circle, cone, none
        }

        public enum movement
        {
            walking, flying, stationary, physics, rotating
        }

        public enum drawIf
        {
            always, lit, notLit
        }

        public enum interaction
        {
            grab, toggle, none
        }


        public Entity(ref int x_in, ref int y_in, ref int height, ref int width, string spriteFile, collision col, lightShape ls, movement mov, drawIf drw, interaction inter, ref ArrayList collisionObjects, ref ArrayList lightingObjects)
        {
            //assign inherited sprite values
            spriteX = x_in;
            spriteY = y_in;
            spriteHeight = height;
            spriteWidth = width;
            spriteName = spriteFile;
            angle = 1000; //default value isn't zero because check in Sprite class would use 0 for actual angle rotating

            //assign entity components
            if (!(col == collision.none))
            {
                collisionObjects.Add(this);
            }
            if (!(ls == lightShape.none))
            {
                lightingObjects.Add(this);
            }
            this.collisionObjects = collisionObjects;
            this.lightingObjects = lightingObjects;
            c = col;
            l = ls;
            m = mov;
            if (m == movement.rotating)
            {
                angle = 0;
            }
            d = drw;
            i = inter;

            switchGlowing = false;
            //collision.x = spriteX;
            //collision.y = spriteY;
            //collision.height = height;
            //collision.width = width;

            //movement.x = spriteX;
            //movement.y = spriteY;
            //movement.height = height;
            //movement.width = width;

            //lighting.x = spriteX;
            //lighting.y = spriteY;
            facingRight = true;
            if (m == movement.walking)
            {
                animations.Add("beta_player");//0
                animations.Add("jump/jump3");//1
                spriteAnimations = new List<Texture2D>();
            }
            else if (m == movement.flying)
            {
                animations.Add("familiar/familiar");
                animations.Add("familiar/familiar1");
                animations.Add("familiar/familiar2");
                spriteAnimations = new List<Texture2D>();
            }
            else if (l == lightShape.none && i == interaction.toggle)
            {
                animations.Add("lights/switchOff");
                animations.Add("lights/switchOn");
                animations.Add("lights/switchOffGLOW");
                animations.Add("lights/switchOnGLOW");
                spriteAnimations = new List<Texture2D>();
            }
          
        }

        public static void AddEntityObject(Entity ent)
        {
            entityList.Add(ent);
        }

        public void assignToggle(String set)
        {
            this.toggleSet = set;
        }

        public void setMaxLightRange(int range)
        {
            this.maxLightRange = range;
        }

        public void Update(Controls controls, GameTime gameTime)
        {
            //handle movement
            if (this.m != movement.stationary)
            {
                if (this.m == movement.walking || this.m == movement.flying)
                {
                    this.getInput(controls, gameTime);
                }
                this.move(controls, gameTime);
                if (this.c != collision.none) this.checkCollisions(gameTime);
            }

            //handle change in light size for fixtures
            if (this.l != lightShape.none)
            {
                if (isLit && this.lightRange < this.maxLightRange)
                {
                    lightRange += Entity.lightToggleRate;
                    if (this.lightRange > this.maxLightRange) //toggle overshot max
                    {
                        this.lightRange = this.maxLightRange;
                    }
                }
                
                if (!isLit && this.lightRange > 0)
                {
                    this.lightRange -= Entity.lightToggleRate;
                    if (this.lightRange < 0)
                    {
                        this.lightRange = 0; //toggle overshot 0
                    }
                }
            }

            //handle illumination
            if (this.d != drawIf.always)
            {
                if (this.isIlluminated(gameTime))
                {
                    if (this.d == drawIf.lit) this.c = collision.square;
                    else this.c = collision.none;
                }
                else
                {
                    if (this.d == drawIf.lit) this.c = collision.none;
                    else this.c = collision.square;
                }
            }
        }

        private void getInput(Controls controls, GameTime gameTime)
        {
            if (controls.onPress(Keys.F1, Buttons.BigButton))
            {
                debugDistances = true;
            }
            if (controls.onPress(Keys.F2, Buttons.BigButton))
            {
                debugLighting = true;
            }
            if (m == movement.walking)
            {
                bool onTheGround = onGround();
                if (controls.isPressed(Keys.A, Buttons.DPadLeft))
                {
                    momentumX -= 2;
                    if (maxMomentum + momentumX < 0) momentumX = 0 - maxMomentum;
                    facingRight = false;
                }
                if (controls.isPressed(Keys.D, Buttons.DPadRight))
                {
                    momentumX += 2;
                    if (momentumX > maxMomentum) momentumX = maxMomentum;
                    facingRight = true;
                }
                if (controls.onPress(Keys.W, Buttons.DPadUp) || controls.onPress(Keys.Space, Buttons.DPadUp)) //TODO: implement differen jump arc based on holding jump button?
                {
                    if (onTheGround) momentumY = -14;
                }
                if (controls.onPress(Keys.T, Buttons.DPadDown))
                {
                    Console.WriteLine("Player Pos: (" + spriteX + "," + spriteY + ")");
                }

                if (controls.onPress(Keys.E, Buttons.RightShoulder)) //grab button
                {
                    if (!isHoldingLantern) { //if not holding lantern, see if one nearby
                        foreach (Entity e in Entity.entityList)
                        {
                            if (e.i == interaction.grab && e.isInGrabRange(this)) //found a lantern e in range
                            {
                                this.isHoldingLantern = true; //now holding a lantern

                                //remove lantern from all relavent lists
                                Entity.entityList.Remove(e);
                                collisionObjects.Remove(e);
                                lightingObjects.Remove(e);

                                //change lighting
                                this.l = lightShape.circle;
                                lightingObjects.Add(this);
                                this.setMaxLightRange(e.maxLightRange);
                                this.lightRange = this.maxLightRange; //no incremental light change, just take light from lantern
                                this.animations[0] = "charHoldingLatern.png";
                                this.animations[1] = "jump/jumpwOrb.png";
                                //TODO: other sprites
                                this.needsNewSprite = true; //loads new content in GameLoop.Update()

                                //stop trying to find a lantern
                                break;
                            }
                        }
                    }
                    else //else player has a lantern, so drop it (if room)
                    {
                        this.isHoldingLantern = false; //no longer holding a lantern

                        //make new lantern object
                        int lanternX = this.spriteX - 31; //default to left of player
                        int lanternY = this.spriteY + 10;
                        int lanternHeight = 21, lanternWidth = 22;
                        if (this.facingRight) lanternX += 31 + this.spriteWidth; //move to right of player if player is facing right
                        Entity l = new Entity(ref lanternX, ref lanternY, ref lanternHeight, ref lanternWidth, "lights/lantern.png", Entity.collision.none, Entity.lightShape.circle, Entity.movement.physics, Entity.drawIf.lit, Entity.interaction.grab, ref this.collisionObjects, ref this.lightingObjects);
                        Entity.entityList.Add(l);
                        l.setMaxLightRange(this.maxLightRange);
                        l.lightRange = maxLightRange; //no incremental light change, just instantly max
                        l.needsNewSprite = true; //loads new content in GameLoop.Update()

                        //change lighting
                        this.l = lightShape.none;
                        this.lightRange = 0;
                        this.maxLightRange = 0;
                        lightingObjects.Remove(this);
                        this.animations[0] = "charSprite.png";
                        this.animations[1] = "jump/jump3.png";
                        //TODO: other sprites
                        this.needsNewSprite = true; //loads new content in GameLoop.Update()
                    }
                }
            }
            else if (m == movement.flying)
            {
                if (frames == 10)
                {
                    image = spriteAnimations[animation];
                    if (animation == 2)
                        animation = 0;
                    else
                        animation++;
                    frames = 0;
                }
                frames++;
                if (controls.isPressed(Keys.Left, Buttons.DPadLeft))
                {
                    momentumX -= 2;
                    if (maxMomentum + momentumX < 0) momentumX = 0 - maxMomentum;
                }
                if (controls.isPressed(Keys.Right, Buttons.DPadRight))
                {
                    momentumX += 2;
                    if (momentumX > maxMomentum) momentumX = maxMomentum;
                }
                if (controls.isPressed(Keys.Up, Buttons.DPadUp))
                {
                    momentumY -= 2;
                    if (maxMomentum + momentumY < 0) momentumY = 0 - maxMomentum;
                }
                if (controls.isPressed(Keys.Down, Buttons.DPadDown))
                {
                    momentumY += 2;
                    if (momentumY > maxMomentum) momentumY = maxMomentum;
                }

                //handle switches
                if (this.m == movement.flying)
                {
                    foreach (Entity e in Entity.entityList)
                    {
                        if (e.i == interaction.toggle && e.isInToggleRange(this) && e.l == lightShape.none && controls.onPress(Keys.Q, Buttons.LeftShoulder))
                        {
                            if (e.image == e.spriteAnimations[2])
                                e.image = e.spriteAnimations[3];
                            else
                                e.image = e.spriteAnimations[2];
                            foreach (Entity e2 in lightingObjects)
                            {
                                if (e2.toggleSet == e.toggleSet)
                                {
                                    e2.isLit = !e2.isLit;
                                }
                            }
                        }
                        else if (e.i == interaction.toggle && e.isInToggleRange(this) && e.l == lightShape.none)
                        {
                            if (!e.switchGlowing)
                            {
                                e.switchGlowing = true;
                                if (e.image == e.spriteAnimations[0])
                                    e.image = e.spriteAnimations[2];
                                else
                                    e.image = e.spriteAnimations[3];
                            }
                        }
                        else if (e.i == interaction.toggle && e.l == lightShape.none)
                        {
                            if (e.switchGlowing)
                            {
                                e.switchGlowing = false;
                                if (e.image == e.spriteAnimations[2])
                                    e.image = e.spriteAnimations[0];
                                else
                                    e.image = e.spriteAnimations[1];
                            }
                        }
                    }
                }
            }
        }

        private void move(Controls controls, GameTime gameTime)
        {
            //rotating lights
            if (this.m == movement.rotating)
            {
                if (rotatingClockWise)
                {
                    angle += rotateRate;
                    if (angle > maxAngle)
                    {
                        rotatingClockWise = false;
                    }
                }
                else
                {
                    angle -= rotateRate;
                    if (angle < 0 - maxAngle)
                    {
                        rotatingClockWise = true;
                    }
                }
            }

            //all other movement
            else
            {
                spriteX += momentumX;
                spriteY += momentumY;
                if (m == movement.flying)
                {
                    if (momentumY > 0) momentumY--;
                    if (momentumY < 0) momentumY++;
                    if (momentumX > 0) momentumX--;
                    if (momentumX < 0) momentumX++;
                }
                else
                {
                    if (onGround())
                    {
                        if (momentumX > 0) momentumX--;
                        if (momentumX < 0) momentumX++;
                    }
                    else
                    {
                        momentumY++;
                        if (this.m == movement.walking) image = spriteAnimations[1];
                    }
                }
            }
        }

        private bool onGround()
        {
            if (momentumY != 0) return false; //if vertical momentum, current jumping/falling\
            int bottomSide = spriteY + spriteHeight;
            if (this.m == movement.walking) image = spriteAnimations[0];
            foreach (Entity e in collisionObjects)
            {
                if (e.c == collision.none) continue;
                if (spriteName == e.spriteName && spriteX == e.spriteX && spriteY == e.spriteY) continue; //checking against itself, so skip
                if (Math.Abs(bottomSide - e.spriteY) < 1) //bottom of this aligns with top of a collision entity
                {
                    if (spriteX + spriteWidth > e.spriteX && spriteX < e.spriteX + e.spriteWidth) return true;
                }
            }
            return false;
        }

        private bool isInGrabRange(Entity e)
        {
            return Math.Pow(spriteX - e.spriteX, 2) + Math.Pow(spriteY - e.spriteY, 2) < Math.Pow(Entity.grabDistance, 2);

        }

        private bool isInToggleRange(Entity e)
        {
            return Math.Pow(spriteX - e.spriteX, 2) + Math.Pow(spriteY - e.spriteY, 2) < Math.Pow(Entity.toggleDistance, 2);
        }


        private bool isIlluminated(GameTime gameTime)
        {
            foreach (Entity e in lightingObjects)
            {
                {
                    if (Math.Pow(spriteX - e.spriteX, 2) + Math.Pow(spriteY - e.spriteY, 2) < Math.Pow(e.lightRange, 2)) //if within light range
                    {
                        if (e.l == Entity.lightShape.circle) //if circle illumination, then illuminated
                {
                    if (debugLighting) Console.WriteLine("block at (" + spriteX + "," + spriteY + ") is lit by (" + e.spriteX + "," + e.spriteY + ")");
                    return true;
                }
                        else //else it's a cone light, must make sure rotation lines up
                        {
                            if (e.angle == 1000) //default position = facing downwards
                            {
                                //illuminated if below light fixture and x position is within cone (this is a little hacky)
                                return (spriteY > e.spriteY && spriteX > e.spriteX - 1 / Math.Sqrt(2) * e.lightRange && spriteX < e.spriteX + 1 / Math.Sqrt(2) * e.lightRange);
                            }
                            else
                            {
                                //magic number 0.52 in the Sin calculation accounts for about the angle of the light fixture's cone relative to fixture's position
                                int leftBound = (int) (e.spriteX - Math.Sin(0.52 + e.angle) * e.lightRange / Math.Sqrt(2) - 80);
                                int rightBound = (int) (e.spriteX + Math.Sin(0.52 - e.angle) * e.lightRange / Math.Sqrt(2));
                                Console.WriteLine(leftBound + " " + rightBound);
                                return (spriteY > e.spriteY && spriteX > leftBound && spriteX < rightBound);
                            }
                        }
                    }
                }               
            }
            return false;
        }

        private void checkCollisions(GameTime gameTime)
        {

            foreach (Entity e in collisionObjects) //for each entity with collision
            {
            int leftSide = spriteX,
                rightSide = spriteX + spriteWidth,
                topSide = spriteY,
                bottomSide = spriteY + spriteHeight; //calculate edges of this hitbox

                if (spriteName == e.spriteName && spriteX == e.spriteX && spriteY == e.spriteY) continue; //checking against itself, so skip
                if (e.c == collision.none) continue; //collision not currently active for other entity
                int eLeftSide = e.spriteX,
                    eRightSide = e.spriteX + e.spriteWidth,
                    eTopSide = e.spriteY,
                    eBottomSide = e.spriteY + spriteHeight; //calculate edges of entity's hitbox
                int bottomDist = eTopSide - bottomSide,
                    topDist = topSide - eBottomSide,
                    leftDist = leftSide - eRightSide,
                    rightDist = eLeftSide - rightSide; //calculate distances from each side of entity to this
                if (debugDistances && spriteName == "beta_player.png")
                {
                    Console.WriteLine("distances from " + this.spriteName + "(" + spriteX + "," + spriteY + ") to " + e.spriteName + "(" + e.spriteX + "," + e.spriteY + "):");
                    Console.WriteLine("top=" + topDist + ", bottom=" + bottomDist + ", left=" + leftDist + ", right=" + rightDist);
                }

                if (bottomDist >= 0 || topDist >= 0 || leftDist >= 0 || rightDist >= 0) continue; //if any distances are greater than zero, there is no collision
                if (spriteName == "beta_player.png" && e.spriteName == "beta_lantern.png" || spriteName == "beta_lantern.png" && e.spriteName == "beta_player.png") continue; //ignore collisions between familiar and player
                //TODO: make this hard coding less hacky
                //else there was a collision, determine which side and kill momentum
                int[] dists = new int[] { topDist, bottomDist, leftDist, rightDist }; //for comparing: largest is side of collision (other sides will be more negative)
                if (isLargest(dists, topDist)) //collision is with entity above
                {
                    momentumY = 0;
                    spriteY -= topDist;
                    //spriteY -= momentumY;
                    //momentumY = 0;
                }
                else if (isLargest(dists, bottomDist)) //collision is with entity below
                {
                    momentumY = 0;
                    if (this.m == movement.walking && bottomDist <= 0 - spriteHeight / 2) spriteY = e.spriteY + spriteHeight;
                    spriteY += bottomDist;
                }
                else if (isLargest(dists, leftDist)) //collision is with entity to left
                {
                    momentumX = 0;
                    spriteX -= leftDist;
                }
                else //collision is with entity to the right
                {
                    momentumX = 0;
                    spriteX += rightDist;
                }
            }
            if (debugDistances && spriteName == "beta_player.png") //turn off flag so debug only prints one frame of distances
            {
                Console.WriteLine("");
                debugDistances = false;
            }
        }

        private bool isLargest(int[] nums, int n)
        {
            foreach (int i in nums)
            {
                if (i > n) return false;
            }
            return true;
        }
    }
}
