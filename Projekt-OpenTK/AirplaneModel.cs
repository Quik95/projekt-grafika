using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace Projekt_OpenTK;

public class AirplaneModel : Collidable, IDisposable
{
    private const    bool             UseMultipleLight  = false;
    private readonly DirectionalLight _directionalLight = new();
    private readonly ModelAssimp      _model            = new("assets/Jet/Jet_adjusted.obj");

    private readonly Shader _shader = UseMultipleLight
                                          ? new Shader("Shaders/multiple_lights.vert", "Shaders/multiple_lights.frag")
                                          : new Shader("Shaders/blinn-phong.vert", "Shaders/blinn-phong.frag");

    public readonly float Speed = 13.1f;

    private Vector3    _position   = Vector3.One;
    private Quaternion _rotation   = Quaternion.Identity;
    public  Matrix4    ModelMatrix = Matrix4.Identity;

    private Matrix4 RotationMatrix => Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(50)) *
                                      Matrix4.CreateFromQuaternion(_rotation);

    private Matrix4 TranslationMatrix => Matrix4.CreateTranslation(_position);

    public Vector3 Position
    {
        get => _position;
        set
        {
            _position = value;
            ModelMatrix = Matrix4.Identity *
                          RotationMatrix *
                          TranslationMatrix;
        }
    }

    private static readonly Vector3 AirplaneSize = new(10.4f, 4.2f, 10f);
    public sealed override Collider ModelCollider =>
        new(
            Position + AirplaneSize / 2.0f,
            AirplaneSize
        );

    public void Dispose()
    {
        _shader.Dispose();
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
        _shader.SetUniform("material.shininess", new Vector3(0.8f));
    }

    public void UpdateRotation(float yaw, float pitch)
    {
        _rotation = new Quaternion(pitch / 100, 0, yaw / 100);
    }

    public void Draw(Vector3 cameraPosition, Matrix4 view, Matrix4 projection)
    {
        _shader.Use();
        _shader.SetUniform("view", view);
        _shader.SetUniform("projection", projection);
        _shader.SetUniform("model", ModelMatrix);

        if (UseMultipleLight)
        {
            var pointLight = new PointLight(new Vector3(0f, -1f, 0f));

            _setupDirectionalLight(cameraPosition);
            _setupPointLight(pointLight);
            _setupMaterial();
        }
        else
        {
            _shader.SetUniform("ambient_color", new Vector3(244.0f/255.0f, 233.0f/255.0f, 155.0f/255.0f));
            _shader.SetUniform("diffuse_color", new Vector3(0.1f, 0.1f, 0.1f));
            _shader.SetUniform("specular_color", new Vector3(1f));
        }

        foreach (var mesh in _model.Meshes)
        {
            mesh.Bind();
            GL.DrawArrays(PrimitiveType.Triangles, 0, mesh.Vertices.Length);
        }
    }
}