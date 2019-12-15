
using System;
using OpenTK.Audio.OpenAL;

namespace DumBitEngine.Core.Sound
{
    public class SoundClip : IDisposable
    {
        public string path;
        public int buffer;

        //TODO: make this an interface of some sort
        public SoundFormat format;
        
        public byte[] data;
        
        public SoundClip(string path, SoundFormat format, byte[] data)
        {
            buffer = AL.GenBuffer();
            this.data = data;
            this.path = path;
            this.format = format;
        }

        public void Dispose()
        {
            AL.DeleteBuffer(buffer);
        }
    }
}