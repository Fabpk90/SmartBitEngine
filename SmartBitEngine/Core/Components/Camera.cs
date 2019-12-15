using OpenTK;
using SmartBitEngine;
using SmartBitEngine.Core.Components;

namespace DumBitEngine.Core.Util
{
    public class Camera : Component
    {
        public static Camera main;
        
        public Matrix4 view;
        public Matrix4 projection;

        public Vector3 cameraPos;
        private Vector3 cameraUp;
        private Vector3 cameraFront;
        private Vector3 cameraRight;

        private float movementSpeed;
        private float mouseSensitivity;

        private float yaw;
        private float pitch;

        private float fov;

        public Vector3 CameraFront => cameraFront;

        
        private float aspectRatio;

        public Camera()
        {
            main = this;
        }

        public override string GetName()
        {
            return "Camera";
        }

        public override void Start()
        {
            cameraPos = new Vector3(0, 0, 10f);
            cameraFront = new Vector3(0, 0, -1f);
            cameraUp = new Vector3(0, 1, 0);

            //TODO: update this
            aspectRatio = 1;

            projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(55.0f), aspectRatio,
                0.1f, 100f);
            view = Matrix4.LookAt(cameraPos, cameraFront + cameraPos, cameraUp);

            movementSpeed = 1.5f;

            yaw = pitch = 0.0f;
            mouseSensitivity = .25f;
            fov = 45f;
            
            if (main == null)
            {
                main = this;
            }
        }
        public override void Update()
        {
            view = Matrix4.LookAt(cameraPos, cameraFront + cameraPos, cameraUp);
        }

        public override void Dispose()
        {
            if (main == this)
            {
                main = null;
            }
        }
    }
}
