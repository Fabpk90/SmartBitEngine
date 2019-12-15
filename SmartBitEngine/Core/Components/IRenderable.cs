using System;

namespace SmartBitEngine.Core.Components
{
    public interface IRenderable : IDisposable
    {
        string GetName();
        void Start();
        void Update();
    }
}