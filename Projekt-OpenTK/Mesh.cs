using OpenTK.Graphics.OpenGL4;

namespace Projekt_OpenTK;

public class Mesh : IDisposable
{
    public Mesh(float[] vertices, uint[] indices, List<Texture> textures)
    {
        Vertices = vertices;
        Indices  = indices;
        Textures = textures;
        SetupMesh();
    }

    public float[]                        Vertices { get; }
    public uint[]                         Indices  { get; }
    public IReadOnlyList<Texture>         Textures { get; private set; }
    public VertexArrayObject<float, uint> VAO      { get; set; }
    public BufferObject<float>            VBO      { get; set; }
    public BufferObject<uint>             EBO      { get; set; }

    public void Dispose()
    {
        Textures = null;
        VAO.Dispose();
        VBO.Dispose();
        EBO.Dispose();
    }

    public void SetupMesh()
    {
        EBO = new BufferObject<uint>(Indices, BufferTarget.ElementArrayBuffer);
        VBO = new BufferObject<float>(Vertices, BufferTarget.ArrayBuffer);
        VAO = new VertexArrayObject<float, uint>(VBO, EBO);
        VAO.VertexAttributePointer(0, 3, VertexAttribPointerType.Float, 8, 0);
        VAO.VertexAttributePointer(1, 3, VertexAttribPointerType.Float, 8, 3);
        VAO.VertexAttributePointer(2, 2, VertexAttribPointerType.Float, 8, 6);
    }

    public void Bind()
    {
        VAO.Bind();

        foreach (var (texture, i) in Textures.Select((t, i) => (t, i)))
        {
            texture.Bind(TextureUnit.Texture0 + i);
            GL.ActiveTexture(TextureUnit.Texture0 + i);
        }
    }
}