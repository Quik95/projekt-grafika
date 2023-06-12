using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace Projekt_OpenTK;

public abstract class Collidable
{
    public abstract Collider ModelCollider { get; }

    public bool CheckCollision(Collidable other)
    {
        var (selfMin, selfMax)   = (ModelCollider.Min, ModelCollider.Max);
        var (otherMin, otherMax) = (other.ModelCollider.Min, other.ModelCollider.Max);
        
        return selfMin.X <= otherMax.X &&
               selfMax.X >= otherMin.X &&
               selfMin.Y <= otherMax.Y &&
               selfMax.Y >= otherMin.Y &&
               selfMin.Z <= otherMax.Z &&
               selfMax.Z >= otherMin.Z;

    }
}

public record Collider(Vector3 Center, Vector3 Size)
{
    public Vector3 Min => Center - Size / 2.0f;
    public Vector3 Max => Center + Size / 2.0f;
}