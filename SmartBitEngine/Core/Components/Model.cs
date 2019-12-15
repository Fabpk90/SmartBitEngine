using System;
using System.Collections;
using System.Collections.Generic;
using Assimp;
using Assimp.Unmanaged;
using DumBitEngine.Core.Util;
using Matrix4x4 = System.Numerics.Matrix4x4;
using OpenTK;
using Camera = DumBitEngine.Core.Util.Camera;
using Scene = Assimp.Scene;
using ImGuiNET;
using DumBitEngine.Util;
using SmartBitEngine;
using SmartBitEngine.Core.Components;

namespace DumBitEngine.Core.Shapes
{
    public class Model : Component
    {
        private Shader shader;

        public List<Mesh> meshes;

        private string meshName;
        private string path;

        public bool isRotating;

        public Model()
        {
            
        }
        public Model(string path, string meshName)
        {
            var asset = AssetLoader.UseElement(path + meshName);
            
            if (asset != null)
            {
                Model model = asset as Model;

                Console.WriteLine("Loading model from table");

                if (model != null)
                {
                    meshes = model.meshes;
                    shader = model.shader;

                    this.meshName = meshName;
                    this.path = path;
                }
            }
            else
            {
                this.path = path;
                this.meshName = meshName;
                var context = new AssimpContext();

                shader = new Shader("Assets/Shaders/mesh.glsl");
                shader.Use();

                
                shader.SetMatrix4("model", Matrix4.Identity);
                shader.SetMatrix4("view", ref Camera.main.view);
                shader.SetMatrix4("projection", ref Camera.main.projection);
                
                //shader.SetVector3("lightColor",  LevelManager.activeScene.GetSceneColor());
               // shader.SetVector3("light.lightPos",  LevelManager.activeScene.lightSources[0].Parent.getMatrix4X4().Translation);

                meshes = new List<Mesh>();
                
                var scene = context.ImportFile(path + meshName,
                    PostProcessSteps.Triangulate | PostProcessSteps.FlipUVs | PostProcessSteps.GenerateNormals);

                if (scene != null)
                {
                    ProcessNode(scene.RootNode, scene);
                    scene.Clear();
                }
                
                AssetLoader.AddElement(path+meshName, this);

                Console.WriteLine(meshName +" has " +meshes.Count);
            }
        }

        private void ProcessNode(Node node, Scene scene)
        {
            for (int i = 0; i < node.MeshCount; i++)
            {
                meshes.Add(ProcessMesh(scene.Meshes[node.MeshIndices[i]], scene));
            }

            for (int i = 0; i < node.ChildCount; i++)
            {
                ProcessNode(node.Children[i], scene);
            }
        }

        private Mesh ProcessMesh(Assimp.Mesh mesh, Scene scene)
        {
            List<Vertex> vertices = new List<Vertex>();
            List<uint> indices = new List<uint>();
            List<Texture> textures = new List<Texture>();

            //loading vertex
            for (int i = 0; i < mesh.VertexCount; i++)
            {
                Vertex v;

                v.position.X = mesh.Vertices[i].X;
                v.position.Y = mesh.Vertices[i].Y;
                v.position.Z = mesh.Vertices[i].Z;
                
                v.normal.X = mesh.Normals[i].X;
                v.normal.Y = mesh.Normals[i].Y;
                v.normal.Z = mesh.Normals[i].Z;

                v.texCoord.X = mesh.TextureCoordinateChannels[0][i].X;
                v.texCoord.Y = mesh.TextureCoordinateChannels[0][i].Y;

                vertices.Add(v);
            }

            //loading indices
            for (int i = 0; i < mesh.FaceCount; i++)
            {
                var face = mesh.Faces[i];
                for (int j = 0; j < face.IndexCount; j++)
                {
                    indices.Add((uint)face.Indices[j]);
                }
            }
            
            //loading material
            if (mesh.MaterialIndex >= 0)
            {
                var mat = scene.Materials[mesh.MaterialIndex];

                List<Texture> texDiffuse = LoadMaterialTextures(mat, TextureType.Diffuse, "texture_diffuse");
                textures.AddRange(texDiffuse);

                List<Texture> texSpecular = LoadMaterialTextures(mat, TextureType.Specular, "texture_specular");

                if (texSpecular.Count == 0)
                {
                    texSpecular = LoadMaterialTextures(mat, TextureType.Height, "texture_specular");
                }
                
                textures.AddRange(texSpecular);
            }
            
            return new Mesh(vertices, indices, textures);
        }

        private List<Texture> LoadMaterialTextures(Material material, TextureType type, string typeName)
        {
            List<Texture> tex = new List<Texture>();

            for (int i = 0; i < material.GetMaterialTextureCount(type); i++)
            {
                if (material.GetMaterialTexture(type, i, out var texSlot))
                {
                    //texSlot.FilePath. fix that, relative path
                    var texFullPath = texSlot.FilePath.Split('\\');
                    string texPath = texFullPath[texFullPath.Length - 1]; 
                    
                    Texture texture = new Texture(path + texPath, typeName);
                    tex.Add(texture);
                }
                
            }
            return tex;
        }

        public override void Start()
        {
            
        }

        public override void Dispose()
        {
            if(AssetLoader.RemoveElement(path+meshName))
            {
                foreach (var mesh in meshes)
                {
                    mesh.Dispose();
                }

                shader.Dispose();
            }  
        }

        public override void Update()
        {
            shader.Use();

            shader.SetMatrix4("model", ref gameobject.transform.GetMatrix());
            shader.SetMatrix4("view", ref Camera.main.view);
            shader.SetVector3("viewPos", Camera.main.view.ExtractTranslation());
           // shader.SetVector3("light.lightColor", ref LevelManager.activeScene.lightSources[0].color);
            shader.SetMatrix4("projection", ref Camera.main.projection);
            //shader.SetVector3("light.lightPos",  LevelManager.activeScene.lightSources[0].Parent.getMatrix4X4().Translation);

            
            shader.SetFloat("material.shininess", 2);

            foreach (var mesh in meshes)
            {
                mesh.Draw(ref shader);
            }
        }

        public override string GetName()
        {
            return "Model";
        }
    }
}