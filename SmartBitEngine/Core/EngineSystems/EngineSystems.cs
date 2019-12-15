using System;
using DumBitEngine.Core.Physics;
using DumBitEngine.Core.Sound;
using DumBitEngine.Core.Util;

namespace SmartBitEngine.Core.EngineSystems
{
    public static class EngineSystems
    {
        public static void Init()
        {
            MasterInput.Init();
            Physics.Init();
            AudioMaster.Init();
            
            ImGuiRenderer.Init("Assets/Shader/imgui.glsl", Game.instance.Width, Game.instance.Height);
            ImGuiInput.Init();
        }

        public static void Dispose()
        {
            AudioMaster.Dispose();
            ImGuiRenderer.Dispose();
        }
    }
}