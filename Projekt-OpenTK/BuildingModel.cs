using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace Projekt_OpenTK;

public class BuildingModel : IDisposable
{
    private const bool UseMultipleLight = true;

    private readonly DirectionalLight _directionalLight = new()
    {
        Diffuse = new Vector3(0f, 0f, 0.5f)
    };

    private readonly ModelAssimp _model = new("assets/building/building.obj");

    private readonly Shader _shader = UseMultipleLight
                                          ? new Shader("Shaders/multiple_lights.vert", "Shaders/multiple_lights.frag")
                                          : new Shader("Shaders/shader.vert", "Shaders/shader_diffuse_only.frag");

    private readonly PointLight _pointLight = new(new Vector3(0f, -100000f, 0f));
    private          Vector3    _position   = Vector3.Zero;
    public           Matrix4    ModelMatrix = Matrix4.Identity;

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

    public float[] GetVertices()
    {
        return _model.Meshes.SelectMany(mesh => mesh.Vertices).ToArray();
    }

    private void _setupDirectionalLight(Vector3 cameraPosition)
    {
        _shader.SetUniform("viewPos", cameraPosition);
        _shader.SetUniform("dirLight.direction", _directionalLight.Direction);
        _shader.SetUniform("dirLight.ambient", _directionalLight.Ambient);
        _shader.SetUniform("dirLight.diffuse", _directionalLight.Diffuse);
        _shader.SetUniform("dirLight.specular", _directionalLight.Specular);
    }

    private void _setupPointLight(PointLight pointLight)
    {
        _shader.SetUniform("pointLight.position", pointLight.Position);
        _shader.SetUniform("pointLight.constant", pointLight.Constant);
        _shader.SetUniform("pointLight.linear", pointLight.Linear);
        _shader.SetUniform("pointLight.quadratic", pointLight.Quadratic);
        _shader.SetUniform("pointLight.ambient", pointLight.Ambient);
        _shader.SetUniform("pointLight.diffuse", pointLight.Diffuse);
        _shader.SetUniform("pointLight.specular", pointLight.Specular);
    }

    private void _setupMaterial()
    {
        _shader.SetUniform("material.diffuse", new Vector3(0f));
        _shader.SetUniform("material.specular", new Vector3(1f));
        _shader.SetUniform("material.shininess", new Vector3(1f));
    }

    public void Draw(Vector3 cameraPosition, Matrix4 view, Matrix4 projection)
    {
        _shader.Use();
        _shader.SetUniform("view", view);
        _shader.SetUniform("projection", projection);
        _shader.SetUniform("model", ModelMatrix);

        if (UseMultipleLight)
        {
            _setupMaterial();
            _setupDirectionalLight(cameraPosition);
            _setupPointLight(_pointLight);
        }

        foreach (var mesh in _model.Meshes)
        {
            mesh.Bind();
            GL.DrawArrays(PrimitiveType.Triangles, 0, mesh.Vertices.Length);
        }
    }
}