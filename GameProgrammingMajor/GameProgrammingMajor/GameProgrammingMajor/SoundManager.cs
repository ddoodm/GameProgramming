using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

namespace GameProgrammingMajor
{
    /// <summary>
    /// Loads and plays sounds for the game
    /// </summary>
    public class SoundManager
    {
        public class SoundNames
        {
            public static string
                PROJECTILE_FIRE = "Sound\\smg1_fire1",
                METAL_IMPACT = "Sound\\metal_barrel_impact_hard2";
        }

        /// <summary>
        /// The list of key-value pairs that store the sound effects
        /// </summary>
        private List<KeyValuePair<string, SoundEffect>> soundEffects;

        public SoundManager(Game game)
        {

        }

        public void load(ContentManager content)
        {
            soundEffects = new List<KeyValuePair<string, SoundEffect>>();

            // Build a list containting all sound effects pre-loaded.
            // This is about as hacky as they come.
            SoundNames dummyNames = new SoundNames();
            foreach (FieldInfo soundNameField in typeof(SoundNames).GetFields())
            {
                string soundName = (string)soundNameField.GetValue(dummyNames);

                soundEffects.Add(
                    new KeyValuePair<string, SoundEffect>(
                        soundName, content.Load<SoundEffect>(soundName)));
            }
        }

        public SoundEffect findSound(string soundKey)
        {
            // Linear search for the key
            foreach (KeyValuePair<string, SoundEffect> keyPair in soundEffects)
                if (keyPair.Key == soundKey)
                    return keyPair.Value;

            throw new Exception("There is no sound with the key provided.");
        }

        /// <summary>
        /// Play a sound
        /// </summary>
        /// <param name="soundKey">The key of the sound to play. Obtain from SoundNames class.</param>
        public void play(string soundKey)
        {
            SoundEffect sound = findSound(soundKey);
            sound.Play();
        }
    }
}
