using System;
using ImGuiNET;
using OpenTK.Input;

namespace DumBitEngine.Core.Util
{
    public static class ImGuiInput
    {

        private static ImGuiIOPtr io;
        
        public static void Init()
        {
            io = ImGui.GetIO();
        }
        
        public static void Update()
        {
            io.DeltaTime = Time.deltaTime;
            
            io.MousePos = MasterInput.MousePosition;
            io.MouseDown[0] = MasterInput.IsLeftMouseButtonDown;
            io.MouseWheel = MasterInput.MouseScroll;
            
            if (MasterInput.GetKeyPressed(out Key keyPressed))
            {
                if (keyPressed == Key.BackSpace)
                {
                    io.KeysDown[(int) Key.BackSpace] = true;
                }
                else
                {
                    io.KeysDown[(int) Key.BackSpace] = false;
                    char c = (char) (keyPressed + 14);
                    io.AddInputCharacter(c);
                }
                
            }
            else
            {
                io.KeysDown[(int) Key.BackSpace] = false;
            }
        }
    }
}