using BitSkull.Core;
using BitSkull.Numerics;
using System.Numerics;

namespace BitSkull.Graphics
{
    public enum CameraType { Orthographic, Perspective }

    public sealed class Camera
    {
        public CameraType Type { get; set; }
        public Transform3D Transform { get; private set; } = new Transform3D();
        public Color4 ClearColor { get; set; } = new Color4(0.15f, 0.15f, 0.15f);

        public float Near { get; set; } = 0.1f;
        public float Far { get; set; } = 100f;

        public float _fov = 60f;
        public float FOV //for 3D
        {
            get => _fov;
            set => _fov = Maths.Clamp(value, 0.1f, 179.9f);
        }

        public float Zoom { get; set; } = 3f; //how many world units feet vertically; for 2D

        public Camera(CameraType type)
        {
            Type = type;
        }

        public Matrix4x4 GetProjectionMatrix()
        {
            var wndSize = Application.GetAppInstance().GetWindowSize();
            float wndAspect = (float)wndSize.width / (float)wndSize.height;

            if (Type == CameraType.Orthographic)
            {
                float halfHeight = Zoom / 2f;
                float halfWidth = halfHeight * wndAspect;
                return Matrix4x4.CreateOrthographicOffCenter(-halfWidth, halfWidth, -halfHeight, halfHeight, Near, Far);
            }
            else //perspective
                return Matrix4x4.CreatePerspectiveFieldOfView(Maths.DegToRad(FOV), wndAspect, Near, Far);
        }

        public Matrix4x4 GetViewMatrix()
        {
            if (Type == CameraType.Orthographic)
            {
                Matrix4x4 rot = Matrix4x4.CreateFromYawPitchRoll(Transform.Rotation.Y, Transform.Rotation.X, Transform.Rotation.Z);
                Matrix4x4 trans = Matrix4x4.CreateTranslation(-Transform.Position.X, -Transform.Position.Y, -Transform.Position.Z);
                return rot * trans;
            }
            else //perspective
            {
                Matrix4x4 rot = Matrix4x4.CreateRotationZ(-Transform.Rotation.Z);
                Matrix4x4 trans = Matrix4x4.CreateTranslation(-Transform.Position.X, -Transform.Position.Y, 0f);
                Matrix4x4 scale = Matrix4x4.CreateScale(Transform.Scale.X, Transform.Scale.Y, 1f);
                return scale * rot * trans;
            }
        }
    }
}
