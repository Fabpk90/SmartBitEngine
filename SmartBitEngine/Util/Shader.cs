using System;
using System.Collections;
using System.IO;
using System.Numerics;
using DumBitEngine.Util;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Vector3 = OpenTK.Vector3;

namespace DumBitEngine.Core.Util
{
    public class Shader : IDisposable
    {
        public int ProgramId { get; }

        private string path;

        public Shader(string path)
        {
            var asset = AssetLoader.UseElement(path);
            
            if (asset != null)
            {
                Shader shader = (Shader)asset;
                ProgramId = shader.ProgramId;
                this.path = path;
            }
            else
            {
                this.path = path;
                
                string fragmentShader, vertexShader;

                ProgramId = GL.CreateProgram();
            
                ParseShader(path, out vertexShader, out fragmentShader);

                int fs = CompileShader(fragmentShader, ShaderType.FragmentShader);
                int vs = CompileShader(vertexShader, ShaderType.VertexShader);
            
                GL.AttachShader(ProgramId, fs);
                GL.AttachShader(ProgramId, vs);

            
                GL.LinkProgram(ProgramId);
                GL.ValidateProgram(ProgramId);

                //clean intermediates
                GL.DeleteShader(vs);
                GL.DeleteShader(fs);
                
                AssetLoader.AddElement(path, this);
            }
            
           
        }
        private void ParseShader(string path, out string shaderVertex, out string shaderFragment)
        {
            StreamReader r = new StreamReader(path);

            string[] buffers = new string[2];
            int typeParsing = -1; //0 == vertex 1 == fragment

            while (!r.EndOfStream)
            {
                var line = r.ReadLine();
                if (line.Contains("#shader"))
                {
                    if (line.Contains("vertex"))
                    {
                        typeParsing = 0;
                    }
                    else if (line.Contains("fragment"))
                    {
                        typeParsing = 1;
                    }
                }
                else
                {
                    buffers[typeParsing] += line + "\n";
                }
            }

            shaderVertex = buffers[0];
            shaderFragment = buffers[1];
        }
        
        private int CompileShader(string source, ShaderType type)
        {
            int id = GL.CreateShader(type);

            GL.ShaderSource(id, source);
            GL.CompileShader(id);

            string log = "";
            GL.GetShaderInfoLog(id, out log);

            if (log != "")
            {
                Console.WriteLine(log);
                GL.DeleteShader(id);
            }

            return id;
        }

        public void Use()
        {
            GL.UseProgram(ProgramId);
        }

        public void SetInt(string name, int value)
        {   
            GL.Uniform1(GL.GetUniformLocation(ProgramId, name), value);
        }

        public void SetMatrix4(string name, ref Matrix4 matrix)
        {
            GL.UniformMatrix4(GL.GetUniformLocation(ProgramId, name),false, ref matrix);
        }
        
        public void SetMatrix4(string name, Matrix4 matrix)
        {
            GL.UniformMatrix4(GL.GetUniformLocation(ProgramId, name),false, ref matrix);
        }
        
        public void SetMatrix4(string name, ref Matrix4x4 matrix)
        {
            GL.UniformMatrix4(GL.GetUniformLocation(ProgramId, name), 1, false, ref matrix.M11);
        }

        public void SetVector3(string name, ref Vector3 vector)
        {
            GL.Uniform3(GL.GetUniformLocation(ProgramId, name), ref vector);
        }
        
        public void SetVector3(string name, Vector3 vector)
        {
            GL.Uniform3(GL.GetUniformLocation(ProgramId, name), vector);
        }

        public void Dispose()
        {  
            if (AssetLoader.RemoveElement(path))
            {
                GL.DeleteProgram(ProgramId);

                Console.WriteLine("Unloading shader at: " + path);
            }       
        }

        public void SetFloat(string name, float f)
        {
           GL.Uniform1(GL.GetUniformLocation(ProgramId, name), f);
        }

        public void SetVector3(string name, ref System.Numerics.Vector3 vector)
        {
            GL.Uniform3(GL.GetUniformLocation(ProgramId, name), vector.X, vector.Y, vector.Z);
        }
        
        public void SetVector3(string name, System.Numerics.Vector3 vector)
        {
            GL.Uniform3(GL.GetUniformLocation(ProgramId, name), vector.X, vector.Y, vector.Z);
        }
    }
}