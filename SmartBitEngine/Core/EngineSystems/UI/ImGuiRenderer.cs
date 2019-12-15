using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using ImGuiNET;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;
using SmartBitEngine;
using Vector2 = System.Numerics.Vector2;
using Vector4 = System.Numerics.Vector4;

namespace DumBitEngine.Core.Util
{
    public static class ImGuiRenderer
    {
        private static Shader shader;

        private static int vao;
        
        private static int vertexBuffer;
        private static int sizeVertex;
        
        private static int indexBuffer;
        private static int sizeIndex;
        
        private static int textureID;

        private static int height;
        private static int width;
        
        private static ImGuiIOPtr io;
        private static Matrix4x4 projectionMatrix;

        public static void Init(string shaderPath, int _width, int _height)
        {
            shader = new Shader(shaderPath);

            height = _height;
            width = _width;
            
            ImGui.SetCurrentContext(ImGui.CreateContext());
            io = ImGui.GetIO();
            io.DisplaySize = new Vector2(
                width,
                height);
            io.DisplayFramebufferScale = Vector2.One;
            io.DeltaTime = 1f / 60.0f; // DeltaTime is in seconds.
            io.Fonts.AddFontDefault();

            unsafe
            {
                byte* pixelData;
                int texWidth, texHeight, bytesPerPixel;
                
                io.Fonts.GetTexDataAsRGBA32(out pixelData, out texWidth, out texHeight, out bytesPerPixel);

                textureID = GL.GenTexture();
                GL.ActiveTexture(TextureUnit.Texture0);
                GL.BindTexture(TextureTarget.Texture2D, textureID);
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba
                    , texWidth, texHeight, 0,
                    PixelFormat.Rgba, PixelType.UnsignedByte,  new IntPtr(pixelData));
                
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int) TextureWrapMode.Repeat);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int) TextureWrapMode.Repeat);
            
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int) TextureMinFilter.NearestMipmapLinear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int) TextureMinFilter.Linear);
            }


            io.Fonts.SetTexID(new IntPtr(textureID));
            io.Fonts.ClearTexData();
            
            GenerateBuffers();
            SetKeyMappings();
            
            ImGui.NewFrame();
        }

        private static void GenerateBuffers()
        {
            vao = GL.GenVertexArray();
            vertexBuffer = GL.GenBuffer();
            indexBuffer = GL.GenBuffer();
            
            GL.BindVertexArray(vao);
            
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBuffer);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer ,indexBuffer);

            sizeVertex = 2000;
            sizeIndex = 2000;
            
            GL.BufferData(BufferTarget.ArrayBuffer, sizeVertex, IntPtr.Zero, BufferUsageHint.StaticDraw);
            GL.BufferData(BufferTarget.ElementArrayBuffer, sizeIndex, IntPtr.Zero, BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, Marshal.SizeOf<ImDrawVert>(),
                Marshal.OffsetOf<ImDrawVert>("pos"));
            GL.EnableVertexAttribArray(0);

            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, Marshal.SizeOf<ImDrawVert>(),
                Marshal.OffsetOf<ImDrawVert>("uv"));
            GL.EnableVertexAttribArray(1);

            GL.VertexAttribPointer(2, 4, VertexAttribPointerType.UnsignedByte, true, Marshal.SizeOf<ImDrawVert>(),
                Marshal.OffsetOf<ImDrawVert>("col"));
            GL.EnableVertexAttribArray(2);
            
            shader.Use();
            projectionMatrix = Matrix4x4.CreateOrthographicOffCenter(0, width , height, 0, -1f, 1f);
            //Matrix4.CreateOrthographic(width, height, -1.0f, 1.0f, out projectionMatrix);
            
            shader.SetMatrix4("projection_matrix", ref projectionMatrix);
            shader.SetInt("FontTexture", 0);
        }

        public static void ResizeScreen(int width, int height)
        {
            io.DisplaySize.X = width;
            io.DisplaySize.Y = height;
        }

        public static void DrawData()
        {
            ImGui.Render();
            if(ImGui.GetDrawData().TotalVtxCount  == 0)
                return;
            
            GL.Enable(EnableCap.Blend);
            GL.Enable(EnableCap.ScissorTest);
            GL.BlendEquation(BlendEquationMode.FuncAdd);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.Disable(EnableCap.CullFace);
            GL.Disable(EnableCap.DepthTest);

            shader.Use();

            var data = ImGui.GetDrawData();
            if (data.Valid && !Game.isQuitting)
            { 
                UpdateBuffers(data);
                int idx_offset = 0;
                int vtxOffset = 0;
               
                //Console.WriteLine(data.CmdListsCount);
                for (int i = 0; i < data.CmdListsCount; i++)
                {
                    ImDrawListPtr cmd_list = data.CmdListsRange[i];
                    
                    for (int j = 0; j < cmd_list.CmdBuffer.Size; j++)
                    {
                        var pcmd = cmd_list.CmdBuffer[j];
                        
                        GL.Scissor((int)pcmd.ClipRect.X,
                            (int)(height - pcmd.ClipRect.W),
                            (int)(pcmd.ClipRect.Z - pcmd.ClipRect.X),
                            (int)(pcmd.ClipRect.W - pcmd.ClipRect.Y));

                        if (pcmd.TextureId != IntPtr.Zero)
                        {
                            //GL.ActiveTexture(TextureUnit.Texture0);
                            GL.BindTexture(TextureTarget.Texture2D , pcmd.TextureId.ToInt32());
                        }

                        
                        int baseVertex = vtxOffset;
                        int minVertexIndex = 0;
                        int numVertices = cmd_list.VtxBuffer.Size;
                        int startIndex = idx_offset;
                    
                        GL.DrawRangeElementsBaseVertex(PrimitiveType.Triangles,
                            minVertexIndex,
                            minVertexIndex + numVertices - 1,
                            (int)pcmd.ElemCount,
                            DrawElementsType.UnsignedShort,
                            (IntPtr)(startIndex * sizeof(ushort)),
                            baseVertex);

                        idx_offset += (int)pcmd.ElemCount;
                        
                    }

                    vtxOffset += cmd_list.VtxBuffer.Size;
                }
            }

           
            
            GL.Enable(EnableCap.DepthTest);
           // GL.Enable(EnableCap.CullFace);
            GL.Disable(EnableCap.ScissorTest);
            GL.Disable(EnableCap.Blend);
            
        }

        private static void UpdateBuffers(ImDrawDataPtr data)
        {
            GL.BindVertexArray(vao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBuffer);	
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBuffer);
            
            int totalVBSize = (data.TotalVtxCount * Marshal.SizeOf<ImDrawVert>());
            if (totalVBSize > sizeVertex)
            {
                sizeVertex = (int) (totalVBSize * 1.5f);

                GL.BufferData(BufferTarget.ArrayBuffer, sizeVertex, IntPtr.Zero, BufferUsageHint.DynamicDraw);
            }
            
            uint totalIBSize = (uint)(data.TotalIdxCount * sizeof(ushort));
            if (totalIBSize > sizeIndex)
            {
                sizeIndex = (int) (totalIBSize * 1.5f);

                GL.BufferData(BufferTarget.ElementArrayBuffer, sizeIndex, IntPtr.Zero, BufferUsageHint.DynamicDraw);
            }

            int vertexOffsetInVertices = 0;
            int indexOffsetInElements = 0;
            
            for (int i = 0; i < data.CmdListsCount; i++)
            {
                ImDrawListPtr cmd_list = data.CmdListsRange[i];

                GL.BufferSubData(BufferTarget.ArrayBuffer, (IntPtr) (vertexOffsetInVertices * Marshal.SizeOf<ImDrawVert>()),
                    cmd_list.VtxBuffer.Size * Marshal.SizeOf<ImDrawVert>(), cmd_list.VtxBuffer.Data);
                
                GL.BufferSubData(BufferTarget.ElementArrayBuffer, (IntPtr) (indexOffsetInElements * sizeof(ushort)),
                    cmd_list.IdxBuffer.Size * sizeof(ushort), cmd_list.IdxBuffer.Data);

                vertexOffsetInVertices += cmd_list.VtxBuffer.Size;
                indexOffsetInElements += cmd_list.IdxBuffer.Size;
            }
        }

        private static void SetKeyMappings()
        {
            ImGuiIOPtr io = ImGui.GetIO();
            io.KeyMap[(int)ImGuiKey.Tab] = (int)Key.Tab;
            io.KeyMap[(int)ImGuiKey.LeftArrow] = (int)Key.Left;
            io.KeyMap[(int)ImGuiKey.RightArrow] = (int)Key.Right;
            io.KeyMap[(int)ImGuiKey.UpArrow] = (int)Key.Up;
            io.KeyMap[(int)ImGuiKey.DownArrow] = (int)Key.Down;
            io.KeyMap[(int)ImGuiKey.PageUp] = (int)Key.PageUp;
            io.KeyMap[(int)ImGuiKey.PageDown] = (int)Key.PageDown;
            io.KeyMap[(int)ImGuiKey.Home] = (int)Key.Home;
            io.KeyMap[(int)ImGuiKey.End] = (int)Key.End;
            io.KeyMap[(int)ImGuiKey.Delete] = (int)Key.Delete;
            io.KeyMap[(int)ImGuiKey.Backspace] = (int)Key.BackSpace;
            io.KeyMap[(int)ImGuiKey.Enter] = (int)Key.Enter;
            io.KeyMap[(int)ImGuiKey.Escape] = (int)Key.Escape;
            io.KeyMap[(int)ImGuiKey.A] = (int)Key.A;
            io.KeyMap[(int)ImGuiKey.C] = (int)Key.C;
            io.KeyMap[(int)ImGuiKey.V] = (int)Key.V;
            io.KeyMap[(int)ImGuiKey.X] = (int)Key.X;
            io.KeyMap[(int)ImGuiKey.Y] = (int)Key.Y;
            io.KeyMap[(int)ImGuiKey.Z] = (int)Key.Z;
        }

        public static void Dispose()
        {
            shader.Dispose();
            GL.DeleteBuffer(vao);
            GL.DeleteBuffer(vertexBuffer);
            GL.DeleteBuffer(indexBuffer);
            GL.DeleteTexture(textureID);
        }

       
    }
}