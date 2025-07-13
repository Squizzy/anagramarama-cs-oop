using Microsoft.Extensions.Logging;
using SDL2;

namespace AgOop
{

    /// <summary> defines the SDL Audio configuration </summary>
    internal static class AudioConfig
    {
        internal static int AudioRate {get; set;} = SDL_mixer.MIX_DEFAULT_FREQUENCY;
        internal static ushort AudioFormat {get; set;} = SDL.AUDIO_S16;
        internal static int AudioChannel {get; set;} = 1;
        internal static int AudioBuffers {get; set;} = 256;    
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
    internal static class SoundsFiles
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
        // private static readonly AgOopLogger logger = new AgOopLogger("SoundManager");
        // private static readonly AgOopLogger logger;
        private readonly ILogger<SoundManager> _logger;
        // internal GameManager? _gameManager { get; set; }
        // internal LocaleManager _localeManager;

        // private readonly LocaleManager _localeManager;
        private readonly LocaleSettings _localeSettings;

        /// <summary>Constructor - initialises the sound setup </summary>
        public SoundManager(ILogger<SoundManager> logger, LocaleSettings localeSettings)
        // public SoundManager(ILogger<SoundManager> logger, LocaleManager localeManager)
        {
            _logger = logger;
            _localeSettings = localeSettings;
            // _localeManager = localeManager;
            InitialiseSoundManager();
        }

        /// <summary>The object holding the SDL audio configuration </summary>
        // private AudioConfig? _audioConfig;
        // TODO: This was changed to nullable and away from readonly - can it be changed back?

        /// <summary>The flag representing if the audio is enabled</summary>
        private bool _audioEnabled;

        /// <summary>The flag indicating if the resource has been disposed by the GC</summary>
        private bool _disposed;

        /// <summary>The cache with all the sounds</summary>
        private Sound? _soundCache;

        // #region Singleton Implementation
        
        // /// <summary>Singleton instance of the SoundManager</summary>
        // private static SoundManager? _instance;
        
        // /// <summary>Lock object for thread safety</summary>
        // private static readonly object _lock = new object();
        
        // /// <summary>
        // /// Gets the singleton instance of the SoundManager
        // /// </summary>
        // public static SoundManager Instance
        // {
        //     get
        //     {
        //         lock (_lock)
        //         {
        //             if (_instance == null)
        //             {
        //                 _instance = new SoundManager();
        //             }
        //             return _instance;
        //         }
        //     }
        // }
        
        // #endregion

        /// <summary> sound queue of the sounds to be played </summary>
        internal Queue<string> _soundQueue = new Queue<string>();

        internal bool _isPlaying;



        
        internal void InitialiseSoundManager()
        {
            // Console.WriteLine("SoundManager Constructor"); 
            _logger.LogInformation("SoundManager Constructor");

            // _audioConfig = new AudioConfig();

            try
            {
                int result = SDL_mixer.Mix_OpenAudio(AudioConfig.AudioRate,
                                                     AudioConfig.AudioFormat,
                                                     AudioConfig.AudioChannel,
                                                     AudioConfig.AudioBuffers);
                // int result = SDL_mixer.Mix_OpenAudio(_audioConfig.AudioRate,
                //                                      _audioConfig.AudioFormat,
                //                                      _audioConfig.AudioChannel,
                //                                      _audioConfig.AudioBuffers);
                if (result == -1)
                {
                    throw new InvalidOperationException($"Problem with audio initialisation {SDL.SDL_GetError()}");
                }
                BufferSounds();
                _audioEnabled = true;
            }
            catch (Exception Ex)
            {
                // Console.WriteLine($"SoundManager Constructor: {Ex.Message}");
                _logger.LogError($"SoundManager Constructor: {Ex.Message}");
                _audioEnabled = false;
            }

            _logger.LogInformation("SoundManager Constructor DONE");
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
            // Console.WriteLine($"SoundManager Dispose requested  by: " + (disposing ? "Dispose" : "~SoundManager"));
            _logger.LogInformation($"SoundManager Dispose requested  by: {(disposing ? "Dispose" : "~SoundManager")}");

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
        /// <param name="name">unique id string for the sound</param>
        /// <param name="filename">the filename of the WAV file</param>
        /// <returns>Nothing</returns>
        internal void PushSound(string name, string filename)
        {
            try
            {
                // Determine the path to the sound file
                string soundPath = _localeSettings.audioPath;
                // string? soundPath = _localeManager.audioSubPath;
                if (string.IsNullOrEmpty(soundPath))
                {
                    throw new InvalidOperationException("Path to audio files empty. LocaleManager may not be properly initialized.");
                }
                // soundPath += !soundPath.EndsWith(_localeManager.DIR_SEP) ? _localeManager.DIR_SEP : "";
                // soundPath += !soundPath.EndsWith(_localeManager.DIR_SEP) ? _localeManager.DIR_SEP : "";
                string soundFilename = Path.Join(soundPath, filename);
                // string soundFilename = soundPath + filename;

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
                // Console.WriteLine($"SoundManager PushSound error: {Ex.Message}");
                _logger.LogError($"SoundManager PushSound error: {Ex.Message}");
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
                // Console.Write($"SoundManager GetSound error: {Ex.Message}");
                _logger.LogError($"SoundManager GetSound error: {Ex.Message}");
                return IntPtr.Zero;
            }
        }

        /// <summary> Enqueue the name of the sounds to be played </summary>
        /// <param name="name">name of the sound</param>
        internal void EnqueueSound(string name)
        {
            try
            {
                ValidateSoundName(name);
                _soundQueue.Enqueue(name);

                if (!_isPlaying)
                {
                    ProcessSoundQueue();
                }
            }
            catch (Exception Ex)
            {
                // Console.WriteLine($"QueueSound error: {Ex.Message}");
                _logger.LogError($"QueueSound error: {Ex.Message}");
            }
        }

        internal void QueueSound(string name)
        {
            EnqueueSound(name);
            // Instance.EnqueueSound(name);
        }

        internal void ProcessSoundQueue()
        {
            // Instance.ProcessCurrentSoundQueue();
            ProcessCurrentSoundQueue();
        }

        internal void ProcessCurrentSoundQueue()
        {
            if (_soundQueue.Count == 0)
            {
                _isPlaying = false;
                return;
            }

            if (!_isPlaying)
            {
                string name = _soundQueue.Dequeue();
                _isPlaying = true;
                PlaySound(name);
            }
        }

        internal void PlaySoundWithCallback(string name)
        {
            try
            {
                ValidateSoundName(name);
                SDL_mixer.Mix_ChannelFinished(MixChannelFinishedCallaback);
                PlaySound(name);
            }
            catch (Exception Ex)
            {
                // Console.WriteLine($"SoundManager PlaySoundWithCallback error: {Ex.Message}");
                _logger.LogError($"SoundManager PlaySoundWithCallback error: {Ex.Message}");
            }
        }

        internal void MixChannelFinishedCallaback(int Channel)
        {
            _isPlaying = false;
        }

        /// <summary> Attempts to play the requested sound </summary>
        /// <param name="name">The sound to be played</param>
        /// <exception cref="InvalidOperationException">Error related to acquiring, or playing the sound</exception>
        internal void PlaySound(string name)
        {
            _logger.LogDebug("SoundManager PlaySound called for sound: {name}", name);
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
                // Console.WriteLine($"SoundManager PlaySound error: {Ex.Message}");
                _logger.LogError($"SoundManager PlaySound error: {Ex.Message}");
            }
        }


        /// <summary> Throws an error if the filename is not in the file list or is blank </summary>
        /// <param name="name">the sound to be played</param>
        /// <exception cref="InvalidOperationException">no sound specified</exception>
        /// <exception cref="ArgumentException">Invalid sound specified</exception>
        internal void ValidateSoundName(string? name)
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