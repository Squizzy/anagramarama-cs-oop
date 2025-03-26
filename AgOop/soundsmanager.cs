using SDL2;

namespace AgOop
{

    internal class SoundManagerVariables
    {
        internal static int audio_rate = SDL_mixer.MIX_DEFAULT_FREQUENCY;
        internal static ushort audio_format = SDL.AUDIO_S16;
        internal static int audio_channels = 1;
        internal static int audio_buffers = 256;    
    }

    internal class SoundManager
    {

        /// <summary>Constructor - initialises the sound setup </summary>
        internal SoundManager()
        {
            Console.WriteLine("SoundManager Constructor");
            
            //TODO: Initialisation of the sounds
            if (SDL_mixer.Mix_OpenAudio(SoundManagerVariables.audio_rate,
                                        SoundManagerVariables.audio_format,
                                        SoundManagerVariables.audio_channels,
                                        SoundManagerVariables.audio_buffers) == -1)
            {
                Console.WriteLine("Unable to set audio: %s", SDL.SDL_GetError());
                audio_enabled = false;
            }
            else
            {
                BufferSounds(ref soundCache);
            }
        }

        /// <summary>Finalizer - clean up the sound setup </summary>
        internal static void SoundManagerExit()
        {
            Console.WriteLine("SoundManager Destructor");
            
            if (audio_enabled)
            {
                SDL_mixer.Mix_CloseAudio();
                ClearSoundBuffer();
            }
        }


        // audio vars
        /// <summary>The flag representing if the audio is enabled</summary>
        internal static bool audio_enabled = true;

        // /// <summary>audio_len</summary>
        // internal static uint audio_len;

        // /// <summary>audio_pos</summary>
        // internal static IntPtr audio_pos;

        /// <summary> defines the Sound class </summary>
        /// <remarks> Constructor</remarks>
        /// <param name="name">Name of the sound</param>
        /// <param name="audioChunk">pointer to the audio chunk</param>
        internal class Sound
        {
            /// <value> Property <c>Name</c> name of the sound </value>
            internal string Name = "";
            /// <value> Property <c>audio_chunk</c> audio chunk </value>
            internal IntPtr Audio_chunk = IntPtr.Zero;
            /// <value> Property <c>Next</c> next sound </value>
            internal Sound? Next;

            internal Sound(string name, IntPtr audioChunk)
            {
                Name = name;
                Audio_chunk = audioChunk;
            }
        }

        /// <summary>soundCache</summary>
        internal static Sound? soundCache;
        // public static Sound? soundCache = new(null, IntPtr.Zero);


        // SKIPPED the Error and Debug functions of the original C as this is handled differently in C#


        /// <summary> Search through the list of sound names and return the corresponding audio chunk
        /// walk the module level soundCache until the requiredname is found.  
        /// when found, return the audio data
        /// if name is not found, return NULL instead.
        /// </summary>
        /// <param name="name">name - the unique id string of the required sound</param>
        /// <returns>a chunk of audio or NULL if not found</returns>
        internal static IntPtr GetSound(string name)
        {
            Sound? currentSound = soundCache;

            while (currentSound != null)
            {
                if (currentSound.Name == name)
                {
                    return currentSound.Audio_chunk;
                }
                currentSound = currentSound.Next;
            }
            return IntPtr.Zero;
        }


        /// <summary> push a sound onto the soundCache </summary>
        /// <param name="soundCache">pointer to the head of the soundCache</param>
        /// <param name="name">unique id string for the sound</param>
        /// <param name="filename">the filename of the WAV file</param>
        /// <returns>Nothing</returns>
        internal static void PushSound(ref Sound? soundCache, string name, string filename)
        {

            string soundFilename = new string(LocaleManager.basePath) + LocaleManager.audioSubPath;

            if ((soundFilename[0] != 0) && (soundFilename[soundFilename.Length - 1] != LocaleManager.DIR_SEP))
            {
                soundFilename += LocaleManager.DIR_SEP;
            }
            soundFilename += filename;

            IntPtr Audio_chunk = SDL_mixer.Mix_LoadWAV(soundFilename);

            Sound thisSound = new Sound(name: name, audioChunk: Audio_chunk);

            thisSound.Next = soundCache;

            soundCache = thisSound;
        }


        /// <summary> push all the game sounds onto the soundCache linked list.  
        /// Note that soundCache is passed into pushSound by reference, 
        /// so that the head pointer can be updated </summary>
        /// <param name="soundCache">the sound cache</param>
        /// <returns>Nothing</returns>
        internal static void BufferSounds(ref Sound? soundCache)
        {
            PushSound(ref soundCache, "click-answer", "click-answer.wav");
            PushSound(ref soundCache, "click-shuffle", "click-shuffle.wav");
            PushSound(ref soundCache, "foundbig", "foundbig.wav");
            PushSound(ref soundCache, "found", "found.wav");
            PushSound(ref soundCache, "clear", "clearword.wav");
            PushSound(ref soundCache, "duplicate", "duplicate.wav");
            PushSound(ref soundCache, "badword", "badword.wav");
            PushSound(ref soundCache, "shuffle", "shuffle.wav");
            PushSound(ref soundCache, "clock-tick", "clock-tick.wav");
        }


        /// <summary> Free the memory of the sound chunks
        /// No longer needed in c# but kept for the sake of keeting
        /// </summary>
        /// <returns>Nothing</returns>
        internal static void ClearSoundBuffer()
        {
            soundCache = null;
        }


        internal static void PlaySound(string SoundName)
        {
            if (audio_enabled)
            {
                SDL_mixer.Mix_PlayChannel(-1, GetSound(SoundName), 0);
            }
        }
    }
}