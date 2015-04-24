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
        public ArrayList collisionZoneList = new ArrayList();
        public static ArrayList quantumList = new ArrayList();

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
        public float maxAngle = (float) Math.PI / 4; //for rotating lights
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

        public void Update(Controls controls, GameTime gameTime, AudioMixer audioMixer)
        {
            //handle movement
            if (this.m != movement.stationary)
            {
                if (this.m == movement.walking || this.m == movement.flying)
                {
                    this.getInput(controls, gameTime, audioMixer);
                }
                this.move(controls, gameTime);
                buildCollisionZones();
                if (this.c != collision.none && this.m != movement.flying) this.runCollisions(gameTime);
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

        private void getInput(Controls controls, GameTime gameTime, AudioMixer audioMixer)
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
                if (controls.isPressed(Keys.A, Buttons.LeftThumbstickLeft))
                {
                    momentumX -= 2;
                    if (maxMomentum + momentumX < 0) momentumX = 0 - maxMomentum;
                    facingRight = false;
                }
                if (controls.isPressed(Keys.D, Buttons.LeftThumbstickRight))
                {
                    momentumX += 2;
                    if (momentumX > maxMomentum) momentumX = maxMomentum;
                    facingRight = true;
                }
                if (controls.onPress(Keys.W, Buttons.A) || controls.onPress(Keys.Space, Buttons.A)) //TODO: implement differen jump arc based on holding jump button?
                {
                    if (onTheGround)
                    {
                        momentumY = -14;
                        audioMixer.playEffect("Jump");
                    }
                }
                if (controls.onPress(Keys.T, Buttons.DPadDown))
                {
                    Console.WriteLine("Player Pos: (" + spriteX + "," + spriteY + ")");
                }

                if (controls.onPress(Keys.E, Buttons.LeftShoulder)) //grab button
                {
                    if (!isHoldingLantern)
                    { //if not holding lantern, see if one nearby
                        foreach (Entity e in Entity.entityList)
                        {
                            if (e.i == interaction.grab && e.isInGrabRange(this)) //found a lantern e in range
                            {

                                if (e.spriteName == "lights/QuantumLit.png")
                                {
                                    foreach (Entity f in Entity.quantumList)
                                    {
                                        f.spriteName = "lights/QuantumUnlit.png";
                                        f.needsNewSprite = true;
                                        f.isLit = false;
                                    }
                                    this.isHoldingLantern = true;

                                    //change lighting
                                    this.l = lightShape.circle;
                                    lightingObjects.Add(this);
                                    this.setMaxLightRange(e.maxLightRange);
                                    this.lightRange = this.maxLightRange; //no incremental light change, just take light from lantern
                                    this.animations[0] = "charHoldingLatern.png";
                                    this.animations[1] = "jump/jumpwOrb.png";
                                    //TODO: other sprites
                                    this.needsNewSprite = true; //loads new content in GameLoop.Update()

                                }
                                else if (e.spriteName != "lights/QuantumUnlit.png")
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
                                }

                                //play audio
                                audioMixer.playEffect("Interact");

                                //stop trying to find a lantern
                                break;
                            }
                        }
                    }
                    else //else player has a lantern, so drop it (if room)
                    {
                        this.isHoldingLantern = false; //no longer holding a lantern

                        bool drop = true;
                        foreach (Entity e in Entity.quantumList)
                        {
                            if (e.i == interaction.grab && e.isInGrabRange(this) && e.spriteName == "lights/QuantumUnlit.png")
                            {
                                drop = false;
                                foreach (Entity f in Entity.quantumList)
                                {
                                    f.spriteName = "lights/QuantumLit.png";
                                    f.needsNewSprite = true;
                                    f.isLit = true;
                                }
                                e.lightRange = e.maxLightRange;
                                break;
                            }
                        }
                        if (drop)
                        {
                            //make new lantern object
                            int lanternX = this.spriteX - 31; //default to left of player
                            int lanternY = this.spriteY + 10;
                            int lanternHeight = 21, lanternWidth = 22;
                            if (this.facingRight) lanternX += 31 + this.spriteWidth; //move to right of player if player is facing right
                            Entity l = new Entity(ref lanternX, ref lanternY, ref lanternHeight, ref lanternWidth, "lights/lantern.png", Entity.collision.none, Entity.lightShape.circle, Entity.movement.physics, Entity.drawIf.lit, Entity.interaction.grab, ref this.collisionObjects, ref this.lightingObjects);
                            Entity.entityList.Add(l);
                            l.momentumX = this.momentumX; // Don't inherit momentumY because it feels weird
                            l.setMaxLightRange(this.maxLightRange);
                            l.lightRange = maxLightRange; //no incremental light change, just instantly max
                            l.needsNewSprite = true; //loads new content in GameLoop.Update()
                        }
                        //change lighting
                        this.l = lightShape.none;
                        this.lightRange = 0;
                        this.maxLightRange = 0;
                        lightingObjects.Remove(this);
                        this.animations[0] = "charSprite.png";
                        this.animations[1] = "jump/jump3.png";
                        //TODO: other sprites

                        audioMixer.playEffect("Interact");

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
                        if (e.i == interaction.toggle && e.isInToggleRange(this) && e.l == lightShape.none && controls.onPress(Keys.Q, Buttons.RightShoulder))
                        {
                            audioMixer.playEffect("Interact");
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
                    if (angle < maxAngle - Math.PI / 2)
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

        private bool isIllum(Entity ent, Entity e) // Does e illuminate ent?
        {
            //if (e.l == Entity.lightShape.cone) Console.WriteLine("Cone! {0}", e.maxLightRange);
            /*else if (e.l == Entity.lightShape.circle) Console.WriteLine("Circle!");
            else Console.WriteLine("None!");*/

            if (e.l == Entity.lightShape.circle)
            {
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

            return distance < radius + e.lightRange;
        }
            else if (e.l == Entity.lightShape.cone)
            {
                double pointX = ent.spriteX + (ent.spriteWidth / 2);
                double pointY = ent.spriteY + (ent.spriteHeight / 2);

                return insideCone(e, pointX, pointY);
            }
            else { return false; }
        }

        private bool isIllum(Entity ent) // Only works if the light source's center isn't inside ent
        {
            foreach (Entity e in lightingObjects)
            {

                if (e.l == Entity.lightShape.circle)
                {
                double delta_x = (ent.spriteX + (ent.spriteWidth / 2)) - (e.spriteX + (e.spriteWidth / 2));
                double delta_y = (ent.spriteY + (ent.spriteHeight / 2)) - (e.spriteY + (e.spriteHeight / 2));
                    
                if (delta_x == 0) // We're on the same height
                {
                    if (e.lightRange + (e.spriteWidth / 2) > delta_y) return true;
                    else continue;
                }
                    else if (delta_y == 0)
                    { // We're above/below each other
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
                else if (e.l == Entity.lightShape.cone)
                {
                    double pointX = ent.spriteX + (ent.spriteWidth / 2);
                    double pointY = ent.spriteY + (ent.spriteHeight / 2);

                    if (insideCone(e, pointX, pointY))
                        return true;
                }
            }
            return false;
        }

        private bool insideCone(Entity l, double pointX, double pointY) // Checks to see whether a point is in lighting cone l
        {
            double delta_x = pointX - (l.spriteX + (l.spriteWidth / 2));
            double delta_y = pointY - (l.spriteY + (l.spriteHeight / 2));

            double dist = Math.Sqrt((delta_x * delta_x) + (delta_y * delta_y)); // distance from cone origin to point

            double other_dx = pointX - (l.spriteX + (l.spriteWidth / 2));
            double other_dy = pointY - (l.spriteY + (l.spriteHeight / 2) + l.lightRange);
            double opposite = Math.Sqrt((other_dx * other_dx) + (other_dy * other_dy)); // distance from (cone_x, cone_y + cone_range) to point

            // Use law of cos to calculate angle from y axis (down) at light origin to entity
            float pointAngle = (float)Math.Acos((dist * dist + l.lightRange * l.lightRange - opposite * opposite) / (2 * dist * l.lightRange));

            if (delta_x > 0)
                pointAngle *= -1;

            // true if in range of light and angle from center of cone to point < 30
            return (dist <= l.lightRange && Math.Abs(pointAngle - (l.angle)) < .52);
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
                if (e.c == collision.none || e.m == movement.walking || e.spriteName == "beta_player.png" || e.spriteName == "familiar.png") continue;
                    foreach (Entity l in lightingObjects)
                    {
                    if (l.l == Entity.lightShape.cone && isIllum(e)) {
                        for (int i = 0; i < 5; i++)
                        {
                            for (int j = 0; j < 5; j++)
                            {
                                if (insideCone(l, e.spriteX + (e.spriteWidth * i) / 5 + e.spriteWidth / 10, e.spriteY + (e.spriteHeight * j) / 5 + e.spriteHeight / 10))
                                    collisionZoneList.Add(new collisionZone(e.spriteX + (e.spriteWidth * i) / 5, e.spriteY + (e.spriteHeight * j) / 5, e.spriteWidth / 5, e.spriteHeight / 5));
                            }
                        }
                        continue;
                    }
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
                /*
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
                        momentumX = 0;
                    }
                }
                 */
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
                        momentumX = 0;
                    }
                }
                else
                {
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
                        //if (this.m == movement.walking && bottomDist <= 0 - spriteHeight / 2) spriteY = e.spriteY + spriteHeight;
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
                    /*double deltX, deltY;
                    if (leftDist > rightDist)
                    {
                        deltX = leftDist;
                    }
                    else
                    {
                        deltX = rightDist;
                    }
                    if (topDist > bottomDist)
                    {
                        deltY = topDist;
                    }
                    else
                    {
                        deltY = bottomDist;
                    }
                    deltY = Math.Abs(deltY / momentumY);
                    deltX = Math.Abs(deltX / momentumX);
                    /*for (int i = 0; i <= (int)Math.Abs(deltX); i++)
                    {
                        if (momentumX > 0) spriteX--;
                        else spriteX++;
                        spriteY -= momentumY / Math.Abs(momentumX);
                        if (spriteY > c.y + c.height || spriteY + spriteHeight < c.y)
                        {
                            Console.WriteLine("Break!");
                            break;
                        }
                    }
                    if (spriteY > c.y + c.height || spriteY + spriteHeight < c.y) momentumX = 0;
                    else momentumY = 0;*/
                    /*spriteX -= (int)((double)momentumX * min(deltY, deltX));
                    spriteY -= (int)((double)momentumY * min(deltX, deltY));
                    if (momentumX > 0) spriteX--;
                    else spriteX++;
                    if (momentumY > 0) spriteY--;
                    else spriteY++;
                    if (deltY < deltX)
                    {
                        momentumX = 0;
                    }
                    else
                    {
                        momentumY = 0;
                    }*/
                }
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
