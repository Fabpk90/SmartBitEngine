using System;
using System.Numerics;
using OpenTK.Audio.OpenAL;

namespace DumBitEngine.Core.Sound
{
    public class Source : IDisposable
    {
        private SoundClip clip;
        public int sourceID;
        private bool isPlaying;

        private float volume;

        public float Volume
        {
            get { return volume; }
            set
            {
                volume = value;
                AL.Source(sourceID, ALSourcef.Gain, value);
            }
        }

        public Source()
        {
            sourceID = AL.GenSource();
            isPlaying = false;

            Volume = 1.0f;
            
            AL.Source(sourceID, ALSourcef.Pitch, 1f);
            AL.Source(sourceID, ALSource3f.Position, 0, 0, 2);
        }

        

        public bool IsPlaying()
        {
            return isPlaying;
        }

        public SoundClip GetClip()
        {
            return clip;
        }

        public void SetClip(SoundClip clip)
        {
            this.clip = clip;
            AL.Source(sourceID, ALSourcei.Buffer, clip.buffer);
        }

        public void Play()
        {
            AL.SourcePlay(sourceID);

            isPlaying = true;
        }

        public void Stop()
        {
            isPlaying = false;
            AL.SourceStop(sourceID);
        }

        public void Dispose()
        {
            AL.DeleteSource(sourceID);
        }

        public void SetPosition(ref Vector3 position)
        {
            AL.Source(sourceID, ALSource3f.Position, position.X, position.Y, position.Z);
        }
        
        public void SetPosition(Vector3 position)
        {
            AL.Source(sourceID, ALSource3f.Position, position.X, position.Y, position.Z);
        }
    }
}