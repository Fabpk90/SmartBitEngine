using DumBitEngine.Core.Util;
using OpenTK.Input;
using SmartBitEngine.Core.Components;
using SmartBitEngine.Core.Components.UI;

namespace SmartBitEngine.Script
{
    public class EditorQuit : Component
    {
        public override string GetName()
        {
            return "EditorQuit";
        }

        public override void Start()
        {
            GetComponent<UIButton>().OnClick += (sender, arg) => Game.Quit();
        }

        public override void Update()
        {
            
        }

        public override void Dispose()
        {
            
        }
    }
}