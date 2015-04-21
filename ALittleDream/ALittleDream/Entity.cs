﻿using System;
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
        public ArrayList collisionZoneList = new ArrayList();

        public class collisionZone
        {
            public double x, y, width, height;
            public collisionZone(double x_in, double y_in, double width_in, double height_in)
            {
                x = x_in;
                y = y_in;
                width = width_in;
                height = height_in;
            }
        }

        //TO BE TWEAKED
        public static int grabDistance = 50;
        public static int toggleDistance = 50;
        public static int lightToggleRate = 12; //number of pixels per frame lighting changes by when toggled
        public int maxMomentum = 4;
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
                spriteAnimations = new List<Texture2D>();
            }
            else if (l == lightShape.none && i == interaction.toggle)
            {
                animations.Add("lights/switchOff");
                animations.Add("lights/switchOn");
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
                buildCollisionZones();
                if (this.c != collision.none) this.runCollisions(gameTime);
                this.updateMomentum();
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
                if (isIllum(this))
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
                        Entity l = new Entity(ref lanternX, ref lanternY, ref lanternHeight, ref lanternWidth, "lights/lantern.png", Entity.collision.square, Entity.lightShape.circle, Entity.movement.physics, Entity.drawIf.lit, Entity.interaction.grab, ref this.collisionObjects, ref this.lightingObjects);
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
                if (controls.onPress(Keys.Q, Buttons.LeftShoulder))
                {
                    foreach (Entity e in Entity.entityList)
                    {
                        if (e.i == interaction.toggle && e.isInToggleRange(this) && e.l == lightShape.none)
                        {
                            if (e.image == e.spriteAnimations[0])
                                e.image = e.spriteAnimations[1];
                            else
                                e.image = e.spriteAnimations[0];
                            foreach (Entity e2 in lightingObjects)
                            {
                                if (e2.toggleSet == e.toggleSet)
                                {
                                    e2.isLit = !e2.isLit;
                                }
                            }
                        }
                    }
                }
            }
        }

        private void updateMomentum()
        {
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
                    //momentumY = 1;
                    if (this.m == movement.walking) image = spriteAnimations[1];
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
            }
        }

        private bool onGround()
        {
            if (momentumY != 0) return false; //if vertical momentum, current jumping/falling\
            int bottomSide = spriteY + spriteHeight;
            if (this.m == movement.walking) image = spriteAnimations[0];
            foreach (collisionZone c in collisionZoneList)
            {
                if (spriteX == c.x && spriteY == c.y) continue; //checking against itself, so skip
                if (Math.Abs(bottomSide - c.y) < 1) //bottom of this aligns with top of a collision entity
                {
                    if (spriteX + spriteWidth > c.x && spriteX < c.x + c.width) return true;
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
                                //Console.WriteLine(leftBound + " " + rightBound);
                                return (spriteY > e.spriteY && spriteX > leftBound && spriteX < rightBound);
                            }
                        }
                    }
                }               
            }
            return false;
        }

        private bool isIllum(Entity ent, Entity e) // Does e illuminate ent?
        {
            //if (e.l == Entity.lightShape.cone) Console.WriteLine("Cone! {0}", e.maxLightRange);
            /*else if (e.l == Entity.lightShape.circle) Console.WriteLine("Circle!");
            else Console.WriteLine("None!");*/
            double delta_x = (ent.spriteX + (ent.spriteWidth / 2)) - (e.spriteX + (e.spriteWidth / 2));
            double delta_y = (ent.spriteY + (ent.spriteHeight / 2)) - (e.spriteY + (e.spriteHeight / 2));
            if (delta_x == 0) // We're on the same column
            {
                if (e.lightRange + (e.spriteHeight / 2) > delta_y) return true;
                else return false;
            }
            else if (delta_y == 0) // We're on the same height
            { 
                if (e.lightRange + (e.spriteWidth / 2) > delta_x) return true;
                else return false;
            }
            // Find offsets to calculate square's radius
            double x_offset, y_offset;
            if (Math.Abs(delta_y) > Math.Abs(delta_x))
            {
                y_offset = ent.spriteHeight / 2;
                x_offset = (y_offset / delta_y) * delta_x;
            }
            else
            {
                x_offset = ent.spriteWidth / 2;
                y_offset = (x_offset / delta_x) * delta_y;
            }
            double radius = Math.Sqrt((x_offset * x_offset) + (y_offset * y_offset));
            double distance = Math.Sqrt((delta_x * delta_x) + (delta_y * delta_y));
            if (e.l == Entity.lightShape.circle) return distance < radius + e.lightRange;
            //else if (e.l == Entity.lightShape.cone) return (distance < radius + e.lightRange) && 
            return distance < radius + e.lightRange;
        }

        private bool isIllum(Entity ent) // Only works if the light source's center isn't inside ent
        {
            foreach (Entity e in lightingObjects)
            {
                double delta_x = (ent.spriteX + (ent.spriteWidth / 2)) - (e.spriteX + (e.spriteWidth / 2));
                double delta_y = (ent.spriteY + (ent.spriteHeight / 2)) - (e.spriteY + (e.spriteHeight / 2));
                if (delta_x == 0) // We're on the same height
                {
                    if (e.lightRange + (e.spriteWidth / 2) > delta_y) return true;
                    else continue;
                }
                else if (delta_y == 0) { // We're above/below each other
                    if (e.lightRange + (e.spriteHeight / 2) > delta_x) return true;
                    else continue;
                }
                // Find offsets to calculate square's radius
                double x_offset, y_offset;
                if (Math.Abs(delta_y) > Math.Abs(delta_x))
                {
                    y_offset = ent.spriteHeight / 2;
                    x_offset = (y_offset / delta_y) * delta_x;
                }
                else
                {
                    x_offset = ent.spriteWidth / 2;
                    y_offset = (x_offset / delta_x) * delta_y;
                }
                double radius = Math.Sqrt((x_offset * x_offset) + (y_offset * y_offset));
                double distance = Math.Sqrt((delta_x * delta_x) + (delta_y * delta_y));
                if (distance < radius + e.lightRange) return true;
            }
            return false;
        }

        private double min(double one, double two)
        {
            if (one < two) return one;
            else return two;
        }

        private double max(double one, double two)
        {
            if (one > two) return one;
            else return two;
        }

        private void buildCollisionZones()
        {
            collisionZoneList.Clear(); // Clear out previous list.
            foreach (Entity e in collisionObjects)
            {
                //if (isIllum(e)) // We're illuminated
                //{
                if (e.c == collision.none || e.m == movement.walking || e.spriteName == "beta_player.png" || e.spriteName == "familiar.png") continue;
                    foreach (Entity l in lightingObjects)
                    {
                        if (!isIllum(e, l)) continue; // e isn't illuminated by l
                        if (l.spriteX == e.spriteX && l.spriteY == e.spriteY) continue; // Skipping checking against ourselves
                        double e_x = e.spriteX + (e.spriteWidth / 2);
                        double e_y = e.spriteY + (e.spriteHeight / 2);
                        double l_x = l.spriteX + (l.spriteWidth / 2);
                        double l_y = l.spriteY + (l.spriteHeight / 2);
                        double l_r = l.lightRange;
                        // Check top line intersection
                        double top_min, top_max,
                            bot_min, bot_max,
                            left_min, left_max,
                            right_min, right_max;
                        double discrim = (Math.Pow(l_r, 2) - Math.Pow(e.spriteY - l_y, 2));
                        if (discrim >= 0) // We actually intersect
                        {
                            top_min = -Math.Sqrt(discrim) + l_x;
                            top_max = Math.Sqrt(discrim) + l_x;
                        }
                        else
                        {
                            top_min = -9001;
                            top_max = -9001;
                        }
                        // Check bottom line intersection
                        discrim = (Math.Pow(l_r, 2) - Math.Pow((e.spriteY + e.spriteHeight) - l_y, 2));
                        if (discrim >= 0)
                        {
                            bot_min = -Math.Sqrt(discrim) + l_x;
                            bot_max = Math.Sqrt(discrim) + l_x;
                        }
                        else
                        {
                            bot_min = -9001;
                            bot_max = -9001;
                        }
                        // Check left line intersection
                        discrim = (Math.Pow(l_r, 2) - Math.Pow(e.spriteX - l_x, 2));
                        if (discrim >= 0)
                        {
                            left_min = -Math.Sqrt(discrim) + l_y;
                            left_max = Math.Sqrt(discrim) + l_y;
                        }
                        else
                        {
                            left_min = -9001;
                            left_max = -9001;
                        }
                        // Check right line intersection
                        discrim = (Math.Pow(l_r, 2) - Math.Pow((e.spriteX + e.spriteWidth) - l_x, 2));
                        if (discrim >= 0)
                        {
                            right_min = -Math.Sqrt(discrim) + l_y;
                            right_max = Math.Sqrt(discrim) + l_y;
                        }
                        else
                        {
                            right_min = -9001;
                            right_max = -9001;
                        }
                        double c_x,
                            c_y,
                            c_width,
                            c_height;
                        bool left = left_max > e.spriteY && left_min < e.spriteY + e.spriteHeight,
                            right = right_max > e.spriteY && right_min < e.spriteY + e.spriteHeight,
                            top = top_max > e.spriteX && top_min < e.spriteX + e.spriteWidth,
                            bot = bot_max > e.spriteX && bot_min < e.spriteX + e.spriteWidth;
                        if (!left && !right && !top && !bot) continue; // We aren't intersecting. Somehow.
                        // Find c_x
                        if (left) // Circle intersects our left
                        {
                            c_x = e.spriteX;
                        }
                        else
                        {
                            // Downwards is incorrect, use the above formula to check for bounded intersection
                            if (top && bot) // Circle spans entire height
                            {
                                c_x = min(top_min, bot_min);
                            }
                            else if (top) // Only top intersects
                            {
                                c_x = top_min;
                            }
                            else if (bot) // Only bot intersects
                            {
                                c_x = bot_min;
                            }
                            else // Neither top nor bot intersects
                            {
                                c_x = l_x - l_r;
                            }
                        }

                        if (right) // Right intersects
                        {
                            c_width = e.spriteX + e.spriteWidth - c_x;
                        }
                        else // We stop short of the right side
                        {
                            if (top && bot) // Spans block height
                            {
                                c_width = max(top_max, bot_max) - c_x;
                            }
                            else if (top)
                            {
                                c_width = top_max - c_x;
                            }
                            else if (bot)
                            {
                                c_width = bot_max - c_x;
                            }
                            else // Light smaller than block and off to the side
                            {
                                c_width = l_x + l_r;
                            }
                        }


                        // Find c_x
                        if (top) // Circle intersects our left
                        {
                            c_y = e.spriteY;
                        }
                        else
                        {
                            // Downwards is incorrect, use the above formula to check for bounded intersection
                            if (left && right) // Circle spans entire height
                            {
                                c_y = min(left_min, right_min);
                            }
                            else if (left) // Only top intersects
                            {
                                c_y = left_min;
                            }
                            else if (right) // Only bot intersects
                            {
                                c_y = right_min;
                            }
                            else // Neither top nor bot intersects
                            {
                                c_y = l_y - l_r;
                            }
                        }


                        if (bot) // Right intersects
                        {
                            c_height = e.spriteY + e.spriteHeight - c_y;
                        }
                        else // We stop short of the right side
                        {
                            if (left && right) // Spans block height
                            {
                                c_height = max(left_max, right_max) - c_y;
                            }
                            else if (left)
                            {
                                c_height = left_max - c_y;
                            }
                            else if (right)
                            {
                                c_height = right_max - c_y;
                            }
                            else // Light smaller than block and off to the side
                            {
                                c_height = l_y + l_r;
                            }
                        }

                        // Finally have our collision zone
                        collisionZoneList.Add(new collisionZone(c_x, c_y, c_width, c_height));
                    }
                //}
            }
        }

        private void runCollisions(GameTime gameTime)
        {
            foreach (collisionZone c in collisionZoneList)
            {
                int leftSide = spriteX,
                    rightSide = spriteX + spriteWidth,
                    topSide = spriteY,
                    bottomSide = spriteY + spriteHeight;

                int cLeftSide = (int)c.x,
                    cRightSide = (int)(c.x + c.width),
                    cTopSide = (int)c.y,
                    cBotSide = (int)(c.y + c.height);

                int bottomDist = cTopSide - bottomSide,
                    topDist = topSide - cBotSide,
                    leftDist = leftSide - cRightSide,
                    rightDist = cLeftSide - rightSide;
                if (bottomDist >= 0 || topDist >= 0 || leftDist >= 0 || rightDist >= 0) continue; //if any distances are greater than zero, there is no collision
                if (momentumX == 0) // Moving straight up/down
                {
                    if (topDist < bottomDist) // Moving down
                    {
                        spriteY += bottomDist;
                        momentumY = 0;
                    }
                    else // Moving up
                    {
                        spriteY -= topDist;
                        momentumY = 0;
                    }
                }
                else if (momentumY == 0)
                {
                    if (leftDist < rightDist)
                    {
                        spriteX += rightDist;
                        momentumX = 0;
                    }
                    else
                    {
                        spriteX -= leftDist;
                    }
                }
                else
                {
                    int[] dists = new int[] { topDist, bottomDist, leftDist, rightDist };
                    if (isLargest(dists, topDist) || isLargest(dists, bottomDist))
                    {

                    }
                    if (momentumX < momentumY)
                    {
                        if (topDist > bottomDist) //isLargest(dists, topDist)) //collision is with entity above
                        {
                            momentumY = 0;
                            spriteY -= topDist;
                            //spriteY -= momentumY;
                            //momentumY = 0;
                        }
                        else //collision is with entity below
                        {
                            momentumY = 0;
                            //if (this.m == movement.walking && bottomDist <= 0 - spriteHeight / 2) spriteY = (int)c.y + spriteHeight;
                            spriteY += bottomDist;
                        }
                    }
                    else {
                        if (leftDist > rightDist) //isLargest(dists, leftDist)) //collision is with entity to left
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
                }
            }
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
