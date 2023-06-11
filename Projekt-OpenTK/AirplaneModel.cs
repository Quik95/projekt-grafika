using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace Projekt_OpenTK;

public class AirplaneModel : IDisposable
{
    private readonly ModelAssimp _model = new("assets/Jet/11805_airplane_v2_L2.obj");
    // private readonly Shader _shader = new("Shaders/blinn-phong.vert", "Shaders/blinn-phong.frag");
    private readonly Shader _shader = new("Shaders/shader.vert", "Shaders/shader_diffuse_only.frag");
    public readonly float Speed = 13.1f;
    public Vector3 Position = Vector3.One;
    public Matrix4 ModelMatrix = Matrix4.Identity * Matrix4.CreateRotationX(-90.0f) * Matrix4.CreateScale(0.005f);

    public void Dispose()
    {
        _shader.Dispose();
    }

    public void SetPosition(Vector3 position)
    {
        
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