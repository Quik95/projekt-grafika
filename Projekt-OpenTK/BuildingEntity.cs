using OpenTK.Mathematics;

namespace Projekt_OpenTK;

public sealed class BuildingEntity : Collidable
{
    private static readonly (int X, int Z) BuildingPositionBounds = (-1000, 1000);
    // private static readonly (int X, int Z) BuildingPositionBounds = (-20, -10);
    private readonly        BuildingModel  _model;
    private readonly        Vector3        _position;

    public BuildingEntity(BuildingModel model)
    {
        _model = model;
        var x = Random.Shared.Next(BuildingPositionBounds.X, BuildingPositionBounds.Z);
        var z = Random.Shared.Next(BuildingPositionBounds.X, BuildingPositionBounds.Z);
        _position = new Vector3(x, -10f, z);
    }

    private static readonly Vector3 BuildingSize = new(23.1f, 15.4f, 16.2f);
    public override Collider ModelCollider =>
        new(
            _position + new Vector3(-5f, 0f, 0f) + BuildingSize / 2.0f,
            BuildingSize
        );

    public void Draw(Vector3 cameraPosition, Matrix4 viewMatrix, Matrix4 projectionMatrix)
    {
        _model.Position = _position;
        _model.Draw(cameraPosition, viewMatrix, projectionMatrix);
    }
}