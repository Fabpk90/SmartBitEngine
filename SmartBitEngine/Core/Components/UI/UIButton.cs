using System;
using ImGuiNET;

namespace SmartBitEngine.Core.Components.UI
{
    public class UIButton : Component
    {
        public string Name { get; set; }
        public event EventHandler<OnClickArg> OnClick;


        public UIButton()
        {
            Name = "Test";
        }
        

        public override string GetName()
        {
            return "Button";
        }

        public override void Start()
        {
            
        }

        public override void Update()
        {
            if (ImGui.Button(Name))
            {
                OnClick?.Invoke(this, new OnClickArg());
            }
        }

        public override void Dispose()
        {
            
        }
    }

    public struct OnClickArg
    {
    }
}