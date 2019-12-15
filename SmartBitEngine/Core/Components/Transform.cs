using System.Numerics;
using SmartBitEngine.Core.Components;

namespace SmartBitEngine
{
    public class Transform : Component
    {
        private Matrix4x4 matrix;

        public Transform()
        {
            matrix = Matrix4x4.Identity;
        }

        public override string GetName()
        {
            return "Transform";
        }

        public ref Matrix4x4 GetMatrix()
        {
            return ref matrix;
        }
        

        public override void Start()
        {
            
        }

        public override void Update()
        {
            
        }

        public override void Dispose()
        {
            
        }
    }
}