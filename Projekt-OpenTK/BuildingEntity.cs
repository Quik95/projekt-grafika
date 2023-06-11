using OpenTK.Mathematics;

namespace Projekt_OpenTK;

public class BuildingEntity
{
    private readonly static (int X, int Z) BuildingPositionBounds = (-1000, 1000);
    private readonly        BuildingModel  _model;
    private Vector3 _position;

    public BuildingEntity(BuildingModel model)
    {
        _model    = model;
        var x = Random.Shared.Next(BuildingPositionBounds.X, BuildingPositionBounds.Z);
        var z = Random.Shared.Next(BuildingPositionBounds.X, BuildingPositionBounds.Z);
        _position = new Vector3(x, -10f, z);
    }

    public void Draw(Matrix4 viewMatrix, Matrix4 projectionMatrix)
    {
        _model.Position = _position;
        _model.Draw(viewMatrix, projectionMatrix);
    }
}