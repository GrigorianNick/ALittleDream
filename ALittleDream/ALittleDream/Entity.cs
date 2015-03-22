using System;
using System.Collections;
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
        public static ArrayList collisionObjects = new ArrayList();
        public static ArrayList lightingObjects = new ArrayList();
        public collision c;
        public lightShape l;
        public movement m;
        public drawIf d;
        public interaction i;
        public int momentumX, momentumY;
        public int maxMomentum = 7;
        public bool debugDistances = false;

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
            walking, flying, stationary, physics
        }

        public enum drawIf
        {
            always, lit, notLit
        }

        public enum interaction
        {
            grab, none
        }
        

        public Entity(ref int x_in, ref int y_in, ref int height, ref int width, string spriteFile, collision col, lightShape ls, movement mov, drawIf drw, interaction inter)
        {
            //assign inherited sprite values
            spriteX = x_in;
            spriteY = y_in;
            spriteHeight = height;
            spriteWidth = width;
            spriteName = spriteFile;

            //assign entity components
            if (!(col == collision.none))
            {
                collisionObjects.Add(this);
            }
            if (!(ls == lightShape.none))
            {
                lightingObjects.Add(this);
            }
            c = col;
            l = ls;
            m = mov;
            d = drw;
            i = inter;
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
        }

        public static void AddEntityObject(Entity ent)
        {
            entityList.Add(ent);
        }

        public void Update(Controls controls, GameTime gameTime)
        {
            if (this.m != movement.stationary)
            {
                this.getInput(controls, gameTime);
                this.move(controls, gameTime);
                this.checkCollisions(gameTime);
            }
        }

        private void getInput(Controls controls, GameTime gameTime)
        {
            if (controls.onPress(Keys.F1, Buttons.BigButton))
            {
                debugDistances = true;
            }
            if (m == movement.walking) {
                bool onTheGround = onGround();
                if (controls.isPressed(Keys.A, Buttons.DPadLeft))
                {
                    momentumX -= 2;
                    if (maxMomentum + momentumX < 0) momentumX = 0 - maxMomentum;
                }
                if (controls.isPressed(Keys.D, Buttons.DPadRight))
                {
                    momentumX += 2;
                    if (momentumX > maxMomentum) momentumX = maxMomentum;
                }
                if (controls.onPress(Keys.W, Buttons.DPadUp) || controls.onPress(Keys.Space, Buttons.DPadUp)) //TODO: implement differen jump arc based on holding jump button?
                {
                    if (onTheGround) momentumY = -12;
                }
                //if (controls.isPressed(Keys.S, Buttons.DPadDown))
                //{
                //    spriteY++;
                //}
            } else if (m == movement.flying) {
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
                if (controls.isPressed(Keys.Up, Buttons.DPadUp) )
                {
                    momentumY -= 2;
                    if (maxMomentum + momentumY < 0) momentumY = 0 - maxMomentum;
                }
                if (controls.isPressed(Keys.Down, Buttons.DPadDown))
                {
                    momentumY += 2;
                    if (momentumY > maxMomentum) momentumY = maxMomentum;
                }
            }
        }

        private void move(Controls controls, GameTime gameTime)
        {
            spriteX += momentumX;
            if (momentumX > 0) momentumX--;
            if (momentumX < 0) momentumX++;
            spriteY += momentumY;
            if (m == movement.flying)
            {
                if (momentumY > 0) momentumY--;
                if (momentumY < 0) momentumY++;
            }
            else
            {
                if (!onGround()) momentumY++;
            }
        }

        private bool onGround()
        {
            if (momentumY != 0) return false; //if vertical momentum, current jumping/falling\
            int bottomSide = spriteY + spriteHeight;
            foreach (Entity e in collisionObjects)
            {
                if (spriteName == e.spriteName && spriteX == e.spriteX && spriteY == e.spriteY) continue; //checking against itself, so skip
                if (Math.Abs(bottomSide - e.spriteY) < 1) //bottom of this aligns with top of a collision entity
                {
                    if (spriteX + spriteWidth > e.spriteX && spriteX < e.spriteX + e.spriteWidth) return true;
                }                
            }
            return false;
        }

        private void checkCollisions(GameTime gameTime)
        {
            int leftSide = spriteX, rightSide = spriteX + spriteWidth, topSide = spriteY, bottomSide = spriteY + spriteHeight; //calculate edges of this hitbox

            foreach (Entity e in collisionObjects) //for each entity with collision
            {
                if (spriteName == e.spriteName && spriteX == e.spriteX && spriteY == e.spriteY) continue; //checking against itself, so skip
                int eLeftSide = e.spriteX, eRightSide = e.spriteX + e.spriteWidth, eTopSide = e.spriteY, eBottomSide = e.spriteY + spriteHeight; //calculate edges of entity's hitbox
                int bottomDist = eTopSide - bottomSide, topDist = topSide - eBottomSide, leftDist = leftSide - eRightSide, rightDist = eLeftSide - rightSide; //calculate distances from each side of entity to this
                if (debugDistances)
                {
                    Console.WriteLine("distances from " + this.spriteName + "(" + spriteX + "," + spriteY + ") to " + e.spriteName + "(" + e.spriteX + "," + e.spriteY + "):");
                    Console.WriteLine("top=" + topDist + ", bottom=" + bottomDist + ", left=" + leftDist + ", right=" + rightDist);
                }
                
                if (bottomDist > 0 || topDist > 0 || leftDist > 0 || rightDist > 0) continue; //if any distances are greater than zero, there is no collision
                if (spriteName == "beta_player.png" && e.spriteName == "beta_lantern.png" || spriteName == "beta_lantern.png" && e.spriteName == "beta_player.png") continue; //ignore collisions between familiar and player
                //else there was a collision, determine which side and kill momentum
                int[] dists = new int[] { topDist, bottomDist, leftDist, rightDist }; //for comparing: largest is side of collision (other sides will be more negative)
                if (isLargest(dists, topDist)) //collision is with entity above
                {
                    momentumY = 0;
                    spriteY -= topDist;
                }
                else if (isLargest(dists, bottomDist)) //collision is with entity below
                {
                    momentumY = 0;
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
            if (debugDistances) //turn off flag so debug only prints one frame of distances
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
