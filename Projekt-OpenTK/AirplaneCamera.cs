using OpenTK.Mathematics;

namespace FlightSim;

public class AirplaneCamera
{
    private readonly Vector3 _offset;
    private          float   _fov   = MathHelper.DegreesToRadians(60);
    private          Vector3 _front = -Vector3.UnitZ;
    private          float   _pitch;
    private          float   _yaw = -MathHelper.PiOver2;

    public AirplaneCamera(Vector3 offset, float aspectRatio)
    {
        _offset     = offset;
        AspectRatio = aspectRatio;
    }

    public float   AspectRatio { private get; set; }
    public Vector3 Front       => _front;
    public Vector3 Up          { get; private set; } = Vector3.UnitY;

    public Vector3 Right { get; private set; } = Vector3.UnitX;

    public float Pitch
    {
        get => MathHelper.RadiansToDegrees(_pitch);
        set
        {
            var angle = MathHelper.Clamp(value, -89f, 89f);
            _pitch = MathHelper.DegreesToRadians(angle);
            UpdateVectors();
        }
    }

    public float Yaw
    {
        get => MathHelper.RadiansToDegrees(_yaw);
        set
        {
            var angle = MathHelper.Clamp(value, -80 - 80, 80 - 80);
            _yaw = MathHelper.DegreesToRadians(angle);
            UpdateVectors();
        }
    }

    public float Fov
    {
        get => MathHelper.RadiansToDegrees(_fov);
        set
        {
            var angle = MathHelper.Clamp(value, 1f, 90f);
            _fov = MathHelper.DegreesToRadians(angle);
        }
    }

    public Matrix4 GetViewMatrix(Vector3 target)
    {
        return Matrix4.LookAt(target + _offset, target, Vector3.UnitY);
    }

    public Matrix4 GetProjectionMatrix()
    {
        return Matrix4.CreatePerspectiveFieldOfView(_fov, AspectRatio, 1f, 100000f);
    }

    private void UpdateVectors()
    {
        _front.X = -MathF.Cos(_pitch) * MathF.Cos(_yaw);
        _front.Y = MathF.Sin(_pitch);
        // _front.Z = MathF.Cos(_pitch) * MathF.Sin(_yaw);
        _front.Z = -1;

        Right = Vector3.Normalize(Vector3.Cross(_front, Vector3.UnitY));
        Up    = Vector3.Normalize(Vector3.Cross(Right, _front));
    }
}