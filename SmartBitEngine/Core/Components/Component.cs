using System;

namespace SmartBitEngine.Core.Components
{
    public abstract class Component : IRenderable
    {
        public GameObject gameobject;

        public abstract string GetName();
        public abstract void Start();
        public abstract void Update();
        public abstract void Dispose();

        public T GetComponent<T>() where T : Component
        {
            return gameobject.GetComponent<T>();
        }
    }
}