using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ALittleDream
{
    public class AudioMixer
    {

        class MusicTrack {
            private SoundEffectInstance instance;
            private float currentVolume, goalVolume,
                fadeRate = 0.015f; //to be tweaked
            private bool loop;

            public MusicTrack(SoundEffectInstance i, bool l) {
                instance = i;
                currentVolume = 0.0f;
                goalVolume = 0.0f;
                loop = l;
            }

            public void Play() {
                instance.IsLooped = loop;
                instance.Play();
            }

            public void Pause() {
                instance.Pause();
            }

            public void setVolumeFade(float v) {
                goalVolume = v;
            }

            public void setVolumeHard(float v) {
                goalVolume = v;
                currentVolume = goalVolume;
            }

            public bool isLooped()
            {
                return loop;
            }

            public void setLoop(bool l)
            {
                loop = l;
            }

            public void Update() {
                if (goalVolume > currentVolume) {
                    currentVolume += fadeRate;
                    if (currentVolume > goalVolume) currentVolume = goalVolume; //if overshoots
                }
                if (goalVolume < currentVolume) {
                    currentVolume -= fadeRate;
                    if (currentVolume < goalVolume) currentVolume = goalVolume; //if overshoots
                }
                instance.Volume = (float) Math.Pow((double) currentVolume, 2.0); //squared scaling makes fading sound less jarring
            }
        }
        
        //filled with SoundEffect objects named by string keys
        private Dictionary<string, SoundEffect> music; 
        private Dictionary<string, SoundEffect> effects;
        private Dictionary<string, MusicTrack> loadedMusicTracks;

        //holds paths from Content/audio/music and name of the tracks initialized in constructor
        private readonly Dictionary<string, string> musicPaths;

        //holds paths from Content/audio/soundEffects and name of the effects intialized in constructor
        private readonly Dictionary<string, string> effectsPaths;

        //splash and title tracks only started once
        private bool splashStarted;
        private bool menuStarted;
        private bool levelMusicStarted;

        public AudioMixer(Dictionary<string, string> musicFiles, Dictionary<string, string> effectsFiles)
        {
            music = new Dictionary<string, SoundEffect>();
            effects = new Dictionary<string, SoundEffect>();
            loadedMusicTracks = new Dictionary<string, MusicTrack>();

            musicPaths = musicFiles;
            effectsPaths = effectsFiles;

            splashStarted = false;
            menuStarted = false;
            levelMusicStarted = false;
        }

        public void LoadContent(ContentManager content)
        {
            //initialize music
            foreach (String s in musicPaths.Keys)
            {
                music.Add(s, content.Load<SoundEffect>("audio/music/" + musicPaths[s]));
            }

            //initialize effects
            foreach (String s in effectsPaths.Keys)
            {
                effects.Add(s, content.Load<SoundEffect>("audio/soundEffects/" + effectsPaths[s]));
            }
            LoadMusicTracks();
        }

        public void LoadMusicTracks()
        {
            foreach (String s in music.Keys)
            {
                loadedMusicTracks.Add(s, new MusicTrack(music[s].CreateInstance(), true));
            }

            //all else will loop
            loadedMusicTracks["Splash Screen"].setLoop(false);
            loadedMusicTracks["Title Screen"].setLoop(false);
        }

        public void PlayLoopedTracks() {
            foreach (String s in loadedMusicTracks.Keys) {
                if (loadedMusicTracks[s].isLooped()) loadedMusicTracks[s].Play();
            }
        }

        public void PauseMusic()
        {
            foreach (String s in loadedMusicTracks.Keys)
            {
                loadedMusicTracks[s].Pause();
            }
        }

        public void FadeTrackIn(String track)
        {
            if (!loadedMusicTracks.Keys.Contains(track)) return;
            loadedMusicTracks[track].setVolumeFade(0.9f);
        }

        public void FadeTrackOut(String track)
        {
            if (!loadedMusicTracks.Keys.Contains(track)) return;
            loadedMusicTracks[track].setVolumeFade(0.0f);
        }

        public void PlayTracksFadedAdjust(String[] tracks)
        {
            foreach (String s in loadedMusicTracks.Keys) FadeTrackOut(s);
            foreach (String s in tracks) FadeTrackIn(s);
        }

        public void SetTrackVolume(String track, float v)
        {
            if (!loadedMusicTracks.Keys.Contains(track)) return;
            loadedMusicTracks[track].setVolumeHard(v);
        }

        public void playEffect(String name) {
            effects[name].Play();
        }

        public void Update(Screen screen)
        {
            int numberOfLights = 0;

            if (!(screen is GameScreen))
            {
                if (screen is SplashScreen && !splashStarted)
                {
                    loadedMusicTracks["Splash Screen"].setVolumeHard(0.8f);
                    loadedMusicTracks["Splash Screen"].Play();
                    splashStarted = true;
                }
                if (screen is MenuScreen && !menuStarted)
                {
                    loadedMusicTracks["Title Screen"].setVolumeHard(0.8f);
                    loadedMusicTracks["Title Screen"].Play();
                    menuStarted = true;
                }
                return;
            }
            else
            {
                if (!levelMusicStarted)
                {
                    FadeTrackOut("Title Screen");
                    PlayLoopedTracks();
                    FadeTrackIn("Loop1");
                    levelMusicStarted = true;
                    Console.WriteLine("level music started");
                }

                ArrayList lights = ((GameScreen)screen).player.lightingObjects;
                foreach (Entity e in lights)
                {
                    if (e.isLit) numberOfLights++;
                }
            }
            
            if (numberOfLights < 3)
            {
                FadeTrackIn("Loop1");
                FadeTrackOut("Loop2");
                FadeTrackOut("Loop3");
                FadeTrackOut("Loop4");                
            }
            if (numberOfLights == 3)
            {
                FadeTrackIn("Loop1");
                FadeTrackIn("Loop2");
                FadeTrackOut("Loop3");
                FadeTrackOut("Loop4");
            }
            if (numberOfLights == 4)
            {
                FadeTrackIn("Loop1");
                FadeTrackIn("Loop2");
                FadeTrackIn("Loop3");
                FadeTrackOut("Loop4");
            }
            if (numberOfLights > 5)
            {
                FadeTrackIn("Loop1");
                FadeTrackIn("Loop2");
                FadeTrackIn("Loop3");
                FadeTrackIn("Loop4");
            }

            foreach (string s in loadedMusicTracks.Keys)
            {
                loadedMusicTracks[s].Update();
            }
        }
    }
}
