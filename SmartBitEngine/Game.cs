using System;
using DumBitEngine.Core.Sound;
using DumBitEngine.Core.Util;
using ImGuiNET;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;
using SmartBitEngine.Core.Components.UI;
using SmartBitEngine.Core.EngineSystems;
using SmartBitEngine.Script;
using SmartBitEngine.Util;

namespace SmartBitEngine
{
    public class Game : GameWindow
    {
        public static Game instance;
        public static bool isQuitting = false;
        
        private SceneGraph sceneGraph;

        public Game() : base(1280, 720, GraphicsMode.Default, "SmartBitEngine", GameWindowFlags.Default,
            DisplayDevice.Default, 3, 3, GraphicsContextFlags.ForwardCompatible)
        {
            instance = this;
            
            EngineSystems.Init();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            sceneGraph = new SceneGraph();

            MouseDown += (sender, args) => MasterInput.MouseEvent(args);
            MouseUp += (sender, args) => MasterInput.MouseEvent(args);
            MouseMove += (sender, args) => MasterInput.MouseEvent(args);
            MouseWheel += (sender, args) => MasterInput.MouseEvent(args);

            GameObject g = GameObject.CreateObjectWith<UIButton>("Test");
            g.AddComponent<EditorQuit>();

            SceneGraph.AddGameObject(g);
            
            //SceneGraph.Save();
        }

        protected override void OnResize(EventArgs e)
        {
            ImGuiRenderer.ResizeScreen(Width, Height);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            Time.deltaTime = (float) e.Time;
            
            GL.Clear(ClearBufferMask.ColorBufferBit);
            
            MasterInput.Update();
            AudioMaster.Update();
            
            ImGui.NewFrame();
            ImGuiInput.Update();
            
            sceneGraph.Draw();
            ImGui.EndFrame();
            ImGuiRenderer.DrawData();

            SwapBuffers();
        }

        public static void Quit()
        {
            instance.sceneGraph.Dispose();

            isQuitting = true;
            EngineSystems.Dispose();
            
            instance.Exit();
        }
    }
}