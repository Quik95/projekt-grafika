using OpenTK.Mathematics;

namespace Projekt_OpenTK;

public struct DirectionalLight
{
    public Vector3 Direction { get; set; }
    public Vector3 Ambient   { get; set; }
    public Vector3 Diffuse   { get; set; }
    public Vector3 Specular  { get; set; }

    public DirectionalLight()
    {
        Direction = new Vector3(0f, 1000f, 1000f);
        Ambient   = new Vector3(244f / 255f, 233 / 255f, 155 / 255f) * 1.0f; //sunlight
        Diffuse   = new Vector3(0f, 0f, 0f);
        Specular  = new Vector3(244f / 255f, 233 / 255f, 155 / 255f) * 0.1f; //sunlight
    }
}

public struct PointLight
{
    public Vector3 Position  { get; set; }
    public float   Constant  { get; set; }
    public float   Linear    { get; set; }
    public float   Quadratic { get; set; }
    public Vector3 Ambient   { get; set; }
    public Vector3 Diffuse   { get; set; }
    public Vector3 Specular  { get; set; }

    public PointLight(Vector3 position)
    {
        Position  = position;
        Constant  = 1.0f * 1_000_000;
        Linear    = 0.7f * 1_000_000;
        Quadratic = 1.8f * 1_000_000;
        Ambient   = new Vector3(238f / 255f, 75f / 255f, 43f / 255f);
        Diffuse   = new Vector3(238f / 255f, 75f / 255f, 43f / 255f);
        Specular  = new Vector3(238f / 255f, 75f / 255f, 43f / 255f);
    }
}