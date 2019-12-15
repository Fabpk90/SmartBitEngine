using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Assimp;
using Assimp.Unmanaged;
using DumBitEngine.Core.Util;
using DumBitEngine.Util;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using PrimitiveType = OpenTK.Graphics.OpenGL.PrimitiveType;


namespace DumBitEngine.Core.Shapes
{
    public class Mesh : IDisposable
    {   
        private int vao;
        private int vbo;
        private int ebo;
        
        public List<Vertex> vertices;
        public List<uint> indices;
        public List<Texture> textures;

        public Mesh(List<Vertex> vertexes, List<uint> indices, List<Texture> textures)
        { 
            vertices = vertexes;
            this.indices = indices;
            this.textures = textures;   
            

            SetupBuffers();
        }

        private void SetupBuffers()
        {
            vao = GL.GenVertexArray();
            vbo = GL.GenBuffer();
            ebo = GL.GenBuffer();
            
            GL.BindVertexArray(vao);
            
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Count * Marshal.SizeOf<Vertex>(), vertices.ToArray(),
                BufferUsageHint.StaticDraw);
            
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Count * sizeof(uint), indices.ToArray(),
                BufferUsageHint.StaticDraw);
            
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false
                , Marshal.SizeOf<Vertex>(), 0);
            
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false,
                Marshal.SizeOf<Vertex>(), Marshal.OffsetOf<Vertex>("normal"));
            
            GL.EnableVertexAttribArray(2);
            GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false,
                Marshal.SizeOf<Vertex>(), Marshal.OffsetOf<Vertex>("texCoord"));
        }

        public void Draw(ref Shader shader)
        {
            GL.BindVertexArray(vao);

            // bind appropriate textures
            uint diffuseNr  = 1;
            uint specularNr = 1;
            uint normalNr   = 1;
            uint heightNr   = 1;
            for(int i = 0; i < textures.Count; i++)
            {
                GL.ActiveTexture(TextureUnit.Texture0 + i); // active proper texture unit before binding
                // retrieve texture number (the N in diffuse_textureN)
                string number = "";
                string name = textures[i].type;
                if(name == "texture_diffuse")
                    number = ""+diffuseNr++;
                else if(name == "texture_specular")
                    number =""+specularNr++; // transfer unsigned int to stream
                else if(name == "texture_normal")
                    number = ""+normalNr++; // transfer unsigned int to stream
                else if(name == "texture_height")
                    number = ""+heightNr++; // transfer unsigned int to stream

                shader.SetInt("material."+name+number, i);

                //finally bind the texture
                GL.BindTexture(TextureTarget.Texture2D, textures[i].id);
            }
            
            GL.DrawElements(PrimitiveType.Triangles, indices.Count, DrawElementsType.UnsignedInt, 0);

        }

        public void Dispose()
        {
            foreach (var texture in textures)
            {
                texture.Dispose();
            }
        }
    }
}