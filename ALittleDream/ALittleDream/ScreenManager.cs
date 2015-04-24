using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ALittleDream
{
    class ScreenManager
    {
        public Stack<Screen> screens = new Stack<Screen>();
        public Screen currentScreen;
        Screen oldCurrentScreen;
        ContentManager content;
        Texture2D blackScreen;
        bool fadeToBlack;
        public static bool appear;
        public static bool playerFellOffScreen;
        float alpha;
        public static int screenWidth;
        public static int screenHeight;
        float fadeSpeed;
        public bool play;
        public static bool playFade;
        AudioMixer audioMixer;

        //music sound files (all play together, can be adjusted mid-play
        Dictionary<string, string> musicFiles = new Dictionary<string, string>
        {
            {"Bass and Piano", "draft1_bassAndPiano"},
            {"Cello", "draft1_cello"},
            {"Dreamy Synth", "draft1_dreamsynth"},
            {"Strings", "draft1_strings"},
            {"Splash Screen", "LittleDream_Intro"},
            {"Title Screen", "LittleDream_Title"},
            {"Loop1", "LittleDream_Loop1"},
            {"Loop2", "LittleDream_Loop2"},
            {"Loop3", "LittleDream_Loop3"},
            {"Loop4", "LittleDream_Loop4"}
            //add more here
        };

        //effects sound files (played on command, just play once and done
        Dictionary<string, string> effectsFiles = new Dictionary<string, string>
        {
            {"Menu Switch", "LittleDream_MenuSwitch"},
            {"Menu Confirm", "LittleDream_MenuConfirm"},
            {"Interact", "LittleDream_Interact"},
            {"Jump", "LittleDream_Jump"},
            {"Toggle On", "LittleDream_ToggleOn"},
            {"Toggle Off", "LittleDream_ToggleOff"},
            {"Door", "LittleDream_Door"}
            //add more here
        };


        public ScreenManager(Stack<Screen> screens, int sw, int sh)
        {
            screenWidth = sw;
            screenHeight = sh;
            this.screens = screens;
            currentScreen = this.screens.Pop();
            fadeToBlack = false;
            appear = true; 
            alpha = 1.0f;
            fadeSpeed = 0.65f;
            play = false;
            playFade = false;
            audioMixer = new AudioMixer(musicFiles, effectsFiles);
        }

        public void LoadContent(ContentManager content)
        {
            blackScreen = content.Load<Texture2D>("blacksquare");
            currentScreen.LoadContent(content);
            audioMixer.LoadContent(content);
            this.content = content;
        }

        public void UnloadContent(ContentManager content)
        {
            currentScreen.UnloadContent(content);
        }

        public void Update(Controls controls, GameTime gametime)
        {
            if (playerFellOffScreen)
            {
                //Console.WriteLine("player fell off screen!");
                restartLevel();
                playerFellOffScreen = false;
            }

            if (controls.onPress(Keys.I, Buttons.BigButton)) {
                Console.WriteLine(currentScreen.identify());
            }

            if (currentScreen.changeScreen && screens.Count == 0)//
            {
                playFade = true;
                fadeToBlack = true;
                alpha = 0.0f;
                currentScreen.changeScreen = false;
            }
            else if (currentScreen.changeScreen && screens.Count > 0)
            {
                fadeToBlack = true;
                alpha = 0.0f;
                currentScreen.changeScreen = false;
            }
            else if(fadeToBlack)
            {
                alpha += fadeSpeed * (float)gametime.ElapsedGameTime.TotalSeconds;
                if (alpha > 1.0f)
                {
                    fadeToBlack = false;
                    appear = true; 
                    alpha = 1.0f;
                    if(playFade)//
                        play = true; 
                    if (!playFade)//
                    {
                        oldCurrentScreen = currentScreen;
                        currentScreen = this.screens.Pop();
                        currentScreen.LoadContent(content);
                        //oldCurrentScreen.UnloadContent(content);
                    }
                }
            }
            else if(appear)
            {
                alpha -= fadeSpeed *  (float)gametime.ElapsedGameTime.TotalSeconds;
                if (alpha < 0.0f)
                {
                    appear = false;
                    alpha = 0.0f;
                    if (playFade)//
                    {
                        playFade = false;
                    }
                }
            }
            currentScreen.Update(controls, gametime, audioMixer);
            audioMixer.Update(currentScreen);
        }

        public void Draw(SpriteBatch sb){
            currentScreen.Draw(sb);
            sb.Draw(blackScreen, new Rectangle(0, 0, screenWidth, screenHeight), Color.White*alpha);
        }

        public void restartLevel()
        {
            if (currentScreen is GameScreen)
            {
                GameScreen gameScreen = (GameScreen)currentScreen;
                GameScreen restart = new GameScreen(gameScreen.level, gameScreen.initialPlayerX, gameScreen.initialPlayerY, gameScreen.initialFamiliarX, gameScreen.initialFamiliarY, gameScreen.graphicsDevice);
                screens.Push(restart);
                currentScreen.changeScreen = true;
            }
        }

        public void skipScreen(){
            if (screens.Count > 0)
            {
                currentScreen = screens.Pop();
                currentScreen.LoadContent(content);
            }
        }
    }
}
