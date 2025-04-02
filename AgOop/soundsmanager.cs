using SDL2;

namespace AgOop
{

    /// <summary> defines the SDL Audio configuration </summary>
    internal class AudioConfig
    {
        internal int AudioRate {get; set;} = SDL_mixer.MIX_DEFAULT_FREQUENCY;
        internal ushort AudioFormat {get; set;} = SDL.AUDIO_S16;
        internal int AudioChannel {get; set;} = 1;
        internal int AudioBuffers {get; set;} = 256;    
    }


    /// <summary> defines the Sound class </summary>
    internal class Sound
    {
        /// <summary> Name of the sound </summary>
        internal string Name {get; }

        /// <summary> Actual audio data chunk </summary>
        internal IntPtr AudioChunk {get; }

        /// <summary> Next sound in the cache </summary>
        internal Sound? Next {get; set;}

        /// <summary> Sound constructor </summary>
        /// <param name="name">The name of the sound</param>
        /// <param name="audioChunk">The audio data</param>
        internal Sound(string name, IntPtr audioChunk)
        {
            Name = name;
            AudioChunk = audioChunk;
        }
    }


    /// <summary> The list of the sound files used in the game </summary>
    internal class SoundsFiles
    {
        // TODO: This list could be loaded from the/a config file
        /// <summary> Dictionary of the sound name and their associated filename </summary>
        internal static readonly IReadOnlyDictionary<string, string> GameSounds = new Dictionary<string, string>()
        {
            ["click-answer"] = "click-answer.wav",
            ["click-shuffle"] = "click-shuffle.wav",
            ["foundbig"] = "foundbig.wav",
            ["found"] = "found.wav",
            ["clear"] = "clearword.wav",
            ["duplicate"] = "duplicate.wav",
            ["badword"] = "badword.wav",
            ["shuffle"] = "shuffle.wav",
            ["clock-tick"] = "clock-tick.wav"
        };
    }


    // Kept for future reference
    // /// <summary> Interface for the sounds manager </summary>
    //     internal interface ISoundManager: IDisposable
    //     {
    //         void PlaySound(string name);
    //     }


    internal class SoundManager : IDisposable
    {
        /// <summary>The object holding the SDL audio configuration </summary>
        private readonly AudioConfig _audioConfig;

        /// <summary>The flag representing if the audio is enabled</summary>
        private bool _audioEnabled;

        /// <summary>The flag indicating if the resource has been disposed by the GC</summary>
        private bool _disposed;

        /// <summary>The cache with all the sounds</summary>
        private Sound? _soundCache;


        /// <summary>Constructor - initialises the sound setup </summary>
        internal SoundManager()
        {
            Console.WriteLine("SoundManager Constructor");
            
            _audioConfig = new AudioConfig();
            
            try
            {
                int result = SDL_mixer.Mix_OpenAudio(_audioConfig.AudioRate,
                                                     _audioConfig.AudioFormat,
                                                     _audioConfig.AudioChannel,
                                                     _audioConfig.AudioBuffers);
                if (result == -1)
                {
                    throw new InvalidOperationException($"Problem with audio initialisation {SDL.SDL_GetError()}");
                }
                BufferSounds();
                _audioEnabled = true;
            }
            catch (Exception Ex)
            {
                Console.WriteLine($"SoundManager Constructor: {Ex.Message}");
                _audioEnabled = false;
            }
        }


        /// <summary>Finalizer - clean up the sound setup </summary>
        ~SoundManager()
        {
            Dispose(false);
        }


        /// <summary> iDisposable required method to dispose  </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


        /// <summary> The actual cleanup activity when disposing </summary>
        /// <param name="disposing">true if from Dispose, false if from the Finalizer</param>
        protected virtual void Dispose(bool disposing)
        {
            Console.WriteLine($"SoundManager Dispose requested  by: " + (disposing ? "Dispose" : "~SoundManager"));

            if (_disposed)
            {
                return;
            }

            if (_audioEnabled)
            {
                SDL_mixer.Mix_CloseAudio();
                ClearSoundBuffer();
                _audioEnabled = false;
            }

            _disposed = true;
        }


        /// <summary> push all the game sounds onto the soundCache linked list. </summary>
        /// <param name="soundCache">the sound cache</param>
        /// <returns>Nothing</returns>
        internal void BufferSounds()
        {
            // // gameSounds.Key is the name of the sound, gameSound.Value is the filename for it
            // Dictionary<string, string> gameSounds = new Dictionary<string, string>()
            // {
            //     ["click-answer"] = "click-answer.wav",
            //     ["click-shuffle"] = "click-shuffle.wav",
            //     ["foundbig"] = "foundbig.wav",
            //     ["found"] = "found.wav",
            //     ["clear"] = "clearword.wav",
            //     ["duplicate"] = "duplicate.wav",
            //     ["badword"] = "badword.wav",
            //     ["shuffle"] = "shuffle.wav",
            //     ["clock-tick"] = "clock-tick.wav"
            // };

            foreach ( KeyValuePair<string, string> thisSound in SoundsFiles.GameSounds )
            {
                PushSound(thisSound.Key, thisSound.Value);
            }
        }


        /// <summary> push a sound onto the soundCache </summary>
//        /// <param name="soundCache">pointer to the head of the soundCache</param>
        /// <param name="name">unique id string for the sound</param>
        /// <param name="filename">the filename of the WAV file</param>
        /// <returns>Nothing</returns>
        internal void PushSound(string name, string filename)
        {
            try
            {
                // Determine the path to the sound file
                string? soundPath = LocaleManager.audioSubPath;
                if (string.IsNullOrEmpty(soundPath))
                {
                    throw new InvalidOperationException("Path to audio files empty. LocaleManager may not be properly initialized.");
                }
                soundPath += !soundPath.EndsWith(LocaleManager.DIR_SEP) ? LocaleManager.DIR_SEP : "";
                string soundFilename = soundPath + filename;

                // Load the sound audio data
                IntPtr audioChunk = SDL_mixer.Mix_LoadWAV(soundFilename);
                if (audioChunk == IntPtr.Zero)
                {
                    throw new IOException($"Problem with SDL loading sound {name} (file name: {filename}): {SDL.SDL_GetError()}");
                }

                // Create the sound node
                Sound newSound = new Sound(name: name, audioChunk: audioChunk);

                // Add the sound node to the soundCache
                newSound.Next = _soundCache;
                _soundCache = newSound;
            }
            catch (Exception Ex)
            {
                Console.WriteLine($"SoundManager PushSound error: {Ex.Message}");
                // load an empty sound placeholder to so the game can 
                // gracefully continue without this individual sound
                Sound newSound = new Sound(name: name, audioChunk: IntPtr.Zero);
                newSound.Next = _soundCache;
                _soundCache = newSound;
            }

            // string soundFilename = new string(LocaleManager.basePath) + LocaleManager.audioSubPath;
            
            // if ((soundFilename[0] != 0) && (soundFilename[soundFilename.Length - 1] != LocaleManager.DIR_SEP))
            // {
            //     soundFilename += LocaleManager.DIR_SEP;
            // }

        }


        // internal static void SoundManagerExit()
        // {
        //     Console.WriteLine("SoundManager Destructor");
            
        //     if (_audio_enabled)
        //     {
        //         SDL_mixer.Mix_CloseAudio();
        //         ClearSoundBuffer();
        //     }
        // }


        // internal static bool audio_enabled = true;

        // /// <summary>audio_len</summary>
        // internal static uint audio_len;

        // /// <summary>audio_pos</summary>
        // internal static IntPtr audio_pos;

        /// <summary> defines the Sound class </summary>
        /// <remarks> Constructor</remarks>
        /// <param name="name">Name of the sound</param>
        // /// <param name="audioChunk">pointer to the audio chunk</param>
        // internal class Sound
        // {
        //     /// <value> Property <c>Name</c> name of the sound </value>
        //     internal string Name = "";
        //     /// <value> Property <c>audio_chunk</c> audio chunk </value>
        //     internal IntPtr Audio_chunk = IntPtr.Zero;
        //     /// <value> Property <c>Next</c> next sound </value>
        //     internal Sound? Next;

        //     internal Sound(string name, IntPtr audioChunk)
        //     {
        //         Name = name;
        //         Audio_chunk = audioChunk;
        //     }
        // }


        // public static Sound? soundCache = new(null, IntPtr.Zero);


        // SKIPPED the Error and Debug functions of the original C as this is handled differently in C#


        /// <summary> Return the audio chunk (data) for the sound requested
        /// Search through the list of sound names and return the corresponding audio chunk
        /// walk the module level soundCache until the required name is found.  
        /// when found, return the audio data
        /// if name is not found, return NULL instead.
        /// </summary>
        /// <param name="name">sound for which the audio chunk is requested</param>
        /// <returns>the audio chunk, or NULL if not found</returns>
        internal IntPtr GetSound(string name)
        {
            try
            {
                // // Check that the SoundManager is instanced and not disposed of.
                ObjectDisposedException.ThrowIf(_disposed, this);

                // // Check if the name is defined at all
                // if (string.IsNullOrEmpty(name))
                // {
                //     throw new InvalidOperationException("No sound specified");
                // }

                // // Check if the sound requested is a defined sound
                // if (!SoundsFiles.GameSounds.ContainsKey(name))
                // {
                //     throw new ArgumentException("Sound requested is not in the list of sounds available");
                // }
                ValidateSoundName(name);


                // point to the head node of the sound cache
                Sound? currentSound = _soundCache;

                // go through all the sounds until...
                while (currentSound != null)
                {
                    // the desired sound is found
                    if (currentSound.Name == name)
                    {
                        // then return it
                        return currentSound.AudioChunk;
                    }
                    currentSound = currentSound.Next;
                }

                // if the sound was not found in the sound cache, return IntPtr.Zero
                return IntPtr.Zero;
            }
            catch (Exception Ex)
            {
                Console.Write($"SoundManager GetSound error: {Ex.Message}");
                return IntPtr.Zero;
            }
        }


        /// <summary> Attempts to play the requested sound </summary>
        /// <param name="name">The sound to be played</param>
        /// <exception cref="InvalidOperationException">Error related to acquiring, or playing the sound</exception>
        internal void PlaySound(string name)
        {
            if (!_audioEnabled)
            {
                return;
            }
    
            try
            {
                ObjectDisposedException.ThrowIf(_disposed, this);

                IntPtr soundChunk = GetSound(name);
                if (soundChunk == IntPtr.Zero)
                {
                    throw new InvalidOperationException($"Failed getting the sound audio data for sound: {name}");
                }

                int result = SDL_mixer.Mix_PlayChannel(-1, soundChunk, 0);
                if (result == -1)
                {
                    throw new InvalidOperationException($"Failed to play sound {name}: {SDL.SDL_GetError()}");
                }
            }
            catch (Exception Ex)
            {
                Console.WriteLine($"SoundManager PlaySound error: {Ex.Message}");
            }
        }


        /// <summary> Throws an error if the filename is not in the file list or is blank </summary>
        /// <param name="name">the sound to be played</param>
        /// <exception cref="InvalidOperationException">no sound specified</exception>
        /// <exception cref="ArgumentException">Invalid sound specified</exception>
        internal static void ValidateSoundName(string? name)
        {
            // Check if the name is defined at all
            if (string.IsNullOrEmpty(name))
            {
                throw new InvalidOperationException("No sound name specified");
            }

            // Check if the sound requested is a defined sound
            if (!SoundsFiles.GameSounds.ContainsKey(name))
            {
                throw new ArgumentException($"Sound {name} requested is not in the list of sounds available");
            }
        }


        /// <summary>Free the memory of the sound chunks</summary>
        /// <returns>Nothing</returns>
        internal void ClearSoundBuffer()
        {
            Sound? current = _soundCache;

            while (current != null)
            {
                if (current.AudioChunk != IntPtr.Zero)
                {
                    SDL_mixer.Mix_FreeChunk(current.AudioChunk);
                }
                current = current.Next;
            }

            _soundCache = null;
        }

    }
}