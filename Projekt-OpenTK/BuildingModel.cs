using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace Projekt_OpenTK;

public class BuildingModel : IDisposable
{
    private readonly ModelAssimp _model = new("assets/building/building.obj");

    // private readonly Shader _shader = new("Shaders/blinn-phong.vert", "Shaders/blinn-phong.frag");
    private readonly Shader _shader = new("Shaders/shader.vert", "Shaders/shader_diffuse_only.frag");

    private Vector3 _position   = Vector3.Zero;
    public  Matrix4 ModelMatrix = Matrix4.Identity;

    public Vector3 Position
    {
        get => _position;
        set
        {
            _position = value;
            ModelMatrix = Matrix4.Identity *
                          Matrix4.CreateScale(10f) *
                          Matrix4.CreateTranslation(_position);
        }
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