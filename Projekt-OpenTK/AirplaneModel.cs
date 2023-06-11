using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace Projekt_OpenTK;

public class AirplaneModel : IDisposable
{
    private readonly ModelAssimp _model = new("assets/Jet/Jet_adjusted.obj");

    // private readonly Shader _shader = new("Shaders/blinn-phong.vert", "Shaders/blinn-phong.frag");
    private readonly Shader     _shader     = new("Shaders/shader.vert", "Shaders/shader_diffuse_only.frag");
    public readonly  float      Speed       = 13.1f;
    private          Vector3    _position   = Vector3.One;
    public           Matrix4    ModelMatrix = Matrix4.Identity;
    private          Quaternion _rotation   = Quaternion.Identity;

    public Vector3 Position
    {
        get => _position;
        set
        {
            _position = value;
            ModelMatrix = Matrix4.Identity *
                          Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(50)) * 
                          Matrix4.CreateFromQuaternion(_rotation) *
                          Matrix4.CreateTranslation(_position);
        }
    }

    public void UpdateRotation(float yaw, float pitch)
    {
        _rotation = new Quaternion(pitch/100, 0, yaw/100);
    }

    public void Dispose()
    {
        _shader.Dispose();
    }

    public void Draw(Matrix4 view, Matrix4 projection)
    {
        _shader.Use();
        _shader.SetUniform("view", view);
        _shader.SetUniform("projection", projection);
        _shader.SetUniform("model", ModelMatrix);

        foreach (var mesh in _model.Meshes)
        {
            mesh.Bind();
            GL.DrawArrays(PrimitiveType.Triangles, 0, mesh.Vertices.Length);
        }
    }
}