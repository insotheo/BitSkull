using BitSkull.Core;
using BitSkull.Numerics;
using System.Numerics;

namespace BitSkull.Graphics
{
    public enum CameraType { Orthographic, Perspective }

    public sealed class Camera
    {
        public CameraType Type { get; set; }

        public Vec3D Position = Vec3D.Zero;
        public Vec3D Rotation = Vec3D.Zero;
        public Vec2D Scale = new Vec2D(1f, 1f);

        public Vec3D Up = Vec3D.UnitY;
        public Vec3D Front = new Vec3D(0f, 0f, -1f);
        public Vec3D Direction = Vec3D.Zero;

        public Color4 ClearColor = new Color4(0.15f, 0.15f, 0.15f);

        public float _fov = 60f;
        public float FOV //for 3D
        {
            get => _fov;
            set => _fov = Maths.Clamp(value, 0.1f, 179.9f);
        }
        public float Zoom
        {
            get => _fov;
            set => _fov = value;
        }

        public float Near = 0.1f;
        public float Far = 100f;

        public Camera(CameraType type)
        {
            Type = type;
            _fov = type == CameraType.Orthographic ? 2f : 60f;
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
                Matrix4x4 rot = Matrix4x4.CreateFromYawPitchRoll(Rotation.Y, Rotation.X, Rotation.Z);
                Matrix4x4 trans = Matrix4x4.CreateTranslation(-Position.X, -Position.Y, -Position.Z);
                return rot * trans;
            }
            else //perspective
            {
                return Matrix4x4.CreateLookAt(Position.ToSystem(), (Position + Front).ToSystem(), Up.ToSystem());
            }
        }
    }
}
