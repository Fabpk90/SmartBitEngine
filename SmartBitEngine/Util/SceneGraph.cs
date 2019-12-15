using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using DumBitEngine.Core.Util;
using ImGuiNET;
using Newtonsoft.Json;

namespace SmartBitEngine.Util
{
    public class SceneGraph : IDisposable
    {
        public static SceneGraph instance;
        private List<GameObject> gameObjects;
        

        public SceneGraph()
        {
            if (instance == null)
            {
                instance = this;
                gameObjects = new List<GameObject>();
            }
        }

        public void Dispose()
        {
            foreach (var gameObject in gameObjects)
            {
                gameObject.Dispose();
            }
        }

        public void Draw()
        {
            foreach (var gameObject in gameObjects)
            {
                gameObject.Update();
            }
        }

        public static void Save()
        {
            StreamWriter wr = new StreamWriter(new BufferedStream(new FileStream("map.txt", FileMode.OpenOrCreate)));
            JsonSerializer serializer = new JsonSerializer();
            JsonWriter writer = new JsonTextWriter(wr);

            foreach (GameObject gameObject in instance.gameObjects)
            {
                serializer.Serialize(writer, gameObject);
            }
            
            wr.Close();
        }

        public static void AddGameObject(GameObject g)
        {
            instance.gameObjects.Add(g);
            g.Start();
        }
    }
}