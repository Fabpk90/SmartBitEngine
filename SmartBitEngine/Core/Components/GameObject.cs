using System.Collections.Generic;
using System.Text;
using SmartBitEngine.Core.Components;

namespace SmartBitEngine
{
    public class GameObject : Component
    {
        private List<Component> components;

        public int id;
        public static int globalID = 0;

        public string name;
        public bool destroyOnLoad = true;

        public Transform transform => (Transform) components[0];

        public GameObject(string name)
        {
            this.name = name;
            components = new List<Component> { new Transform()};

            id = globalID++;
        }

        public static GameObject CreateObjectWith<T>(string name) where T : Component, new()
        {
            GameObject g = new GameObject(name);
            g.AddComponent<T>();

            return g;
        }

        public void AddComponent<T>() where T : Component, new()
        {
            T t = new T();
            components.Add(t);
            t.gameobject = this;
            t.Start();
        }
        

        public void RemoveComponent<T>() where T : Component
        {
            for (int i = 0; i < components.Count; i++)
            {
                if (components[i].GetType() == typeof(T))
                {
                    components[i].Dispose();
                    components.RemoveAt(i);
                }
            }
        }

        public new T GetComponent<T>() where T : class
        {
            foreach (Component component in components)
            {
                if (component is T)
                {
                    return component as T;
                }
            }

            return null;
        }

        public override string GetName()
        {
            return name;
        }

        public override void Start()
        {
            foreach (var component in components)
            {
                component.Start();
            }
        }

        public override void Update()
        {
            foreach (var component in components)
            {
                component.Update();
            }
        }

        public override void Dispose()
        {
            foreach (var component in components)
            {
                component.Dispose();
            }
        }

        public override string ToString()
        {
            StringBuilder cmp = new StringBuilder();

            foreach (Component component in components)
            {
                cmp.Append(component.GetName());
            }

            return cmp.ToString();
        }
    }
}