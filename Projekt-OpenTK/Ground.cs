using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Projekt_OpenTK;

public class Ground : IDisposable
{
    private const string GroundTextureSource = "assets/grass.png";

    private const int VertexCount = 128;
    private const int Size        = 8000;

    private readonly int     _groundIndicesLength;
    private readonly Texture _groundTexture;

    private readonly Shader             _shader;
    private readonly BufferObject<uint> EBO;

    private readonly VertexArrayObject<float, uint> VAO;
    private readonly BufferObject<float>            VBO;

    public Ground()
    {
        var (groundVertices, groundIndices) = GenerateTerrain();
        _groundIndicesLength                = groundIndices.Length;
        VBO                                 = new BufferObject<float>(groundVertices, BufferTarget.ArrayBuffer);
        EBO                                 = new BufferObject<uint>(groundIndices, BufferTarget.ElementArrayBuffer);
        VAO                                 = new VertexArrayObject<float, uint>(VBO, EBO);

        VAO.VertexAttributePointer(0, 3, VertexAttribPointerType.Float, 8, 0);
        VAO.VertexAttributePointer(1, 3, VertexAttribPointerType.Float, 8, 3);
        VAO.VertexAttributePointer(2, 2, VertexAttribPointerType.Float, 8, 6);

        _shader        = new Shader("Shaders/ground.vert", "Shaders/shader_diffuse_only.frag");
        _groundTexture = new Texture(GroundTextureSource);
    }

    public void Dispose()
    {
        _shader.Dispose();
        EBO.Dispose();
        VAO.Dispose();
        VBO.Dispose();
    }

    private (float[], uint[]) GenerateTerrain()
    {
        const int count = VertexCount * VertexCount;
        const int bufferStride = 3 + 3 + 2;
        var vertices = new float[count * 3 + count * 3 + count * 2];
        var indices = new uint[6 * (VertexCount - 1) * (VertexCount - 1)];
        var vertexPointer = 0;

        for (var i = 0; i < VertexCount; i++)
        for (var j = 0; j < VertexCount; j++)
        {
            var offset = vertexPointer * bufferStride;
            vertices[offset + 0] = j / ((float) VertexCount - 1) * Size;
            vertices[offset + 1] = 0;
            vertices[offset + 2] = i / ((float) VertexCount - 1) * Size;

            vertices[offset + 3] = 0;
            vertices[offset + 4] = 1;
            vertices[offset + 5] = 0;

            vertices[offset + 6] = j / ((float) VertexCount - 1);
            vertices[offset + 7] = i / ((float) VertexCount - 1);
            vertexPointer++;
        }

        var indicesPointer = 0;
        for (uint gz = 0; gz < VertexCount - 1; gz++)
        for (uint gx = 0; gx < VertexCount - 1; gx++)
        {
            var topLeft = gz * VertexCount + gx;
            var topRight = topLeft + 1;
            var bottomLeft = (gz + 1) * VertexCount + gx;
            var bottomRight = bottomLeft + 1;

            indices[indicesPointer++] = topLeft;
            indices[indicesPointer++] = bottomLeft;
            indices[indicesPointer++] = topRight;
            indices[indicesPointer++] = topRight;
            indices[indicesPointer++] = bottomLeft;
            indices[indicesPointer++] = bottomRight;
        }

        return (vertices, indices);
    }

    public void Draw(Matrix4 view, Matrix4 projection)
    {
        VAO.Bind();
        EBO.Bind();
        VBO.Bind();

        _shader.Use();
        _shader.SetUniform("view", view);
        _shader.SetUniform("projection", projection);
        _shader.SetUniform(
            "model", Matrix4.Identity * Matrix4.CreateTranslation(-(float) Size / 2, -10, -(float) Size / 2));

        _groundTexture.Bind();
        GL.DrawElements(PrimitiveType.Triangles, _groundIndicesLength, DrawElementsType.UnsignedInt, 0);

        _shader.Unbind();
    }
}