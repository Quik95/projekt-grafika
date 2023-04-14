using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using StbImageSharp;

namespace Projekt_OpenTK;

public class Skybox : IDisposable
{
    private static readonly float[] _skyboxVertices =
    {
        //   Coordinates
        -1.0f, -1.0f, 1.0f,  //        7--------6
        1.0f, -1.0f, 1.0f,   //       /|       /|
        1.0f, -1.0f, -1.0f,  //      4--------5 |
        -1.0f, -1.0f, -1.0f, //      | |      | |
        -1.0f, 1.0f, 1.0f,   //      | 3------|-2
        1.0f, 1.0f, 1.0f,    //      |/       |/
        1.0f, 1.0f, -1.0f,   //      0--------1
        -1.0f, 1.0f, -1.0f
    };

    private static readonly uint[] _skyboxIndices =
    {
        1, 2, 6,
        6, 5, 1,
        0, 4, 7,
        7, 3, 0,
        4, 5, 6,
        6, 7, 4,
        0, 3, 2,
        2, 1, 0,
        0, 1, 5,
        5, 4, 0,
        3, 7, 6,
        6, 2, 3
    };

    private static readonly string[] _skyboxFaces =
    {
        "assets/skybox/bluecloud_rt.jpg",
        "assets/skybox/bluecloud_lf.jpg",
        "assets/skybox/bluecloud_up.jpg",
        "assets/skybox/bluecloud_dn.jpg",
        "assets/skybox/bluecloud_ft.jpg",
        "assets/skybox/bluecloud_bk.jpg"
    };

    private readonly Shader             _shader;
    private readonly BufferObject<uint> EBO;

    private readonly VertexArrayObject<float, uint> VAO;
    private readonly BufferObject<float>            VBO;

    private int _textureHandle;

    public Skybox()
    {
        VBO = new BufferObject<float>(_skyboxVertices, BufferTarget.ArrayBuffer);
        EBO = new BufferObject<uint>(_skyboxIndices, BufferTarget.ElementArrayBuffer);
        VAO = new VertexArrayObject<float, uint>(VBO, EBO);
        VAO.VertexAttributePointer(0, 3, VertexAttribPointerType.Float, 3, 0);
        _shader = new Shader("Shaders/skybox.vert", "Shaders/skybox.frag");
        LoadTextures();
    }

    public void Dispose()
    {
        _shader.Dispose();
        EBO.Dispose();
        VAO.Dispose();
        VBO.Dispose();
    }

    private void LoadTextures()
    {
        _textureHandle = GL.GenTexture();
        GL.BindTexture(TextureTarget.TextureCubeMap, _textureHandle);
        GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMagFilter,
                        (int) TextureMagFilter.Linear);
        GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMinFilter,
                        (int) TextureMagFilter.Linear);
        GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapS,
                        (int) TextureParameterName.ClampToEdge);
        GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapT,
                        (int) TextureParameterName.ClampToEdge);
        GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapR,
                        (int) TextureParameterName.ClampToEdge);

        foreach (var (face, i) in _skyboxFaces.Select((i, f) => (i, f)))
        {
            StbImage.stbi_set_flip_vertically_on_load(face is "assets/skybox/bluecloud_rt.jpg" ? 1 : 0);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapT,
                            (int) TextureParameterName.ClampToEdge);
            var img = ImageResult.FromStream(File.OpenRead(face), ColorComponents.RedGreenBlueAlpha);
            GL.TexImage2D(TextureTarget.TextureCubeMapPositiveX + i, 0, PixelInternalFormat.Rgba, img.Width, img.Height,
                          0, PixelFormat.Rgba, PixelType.UnsignedByte, img.Data);
        }
    }

    public void Draw(Matrix4 view, Matrix4 projection)
    {
        GL.DepthFunc(DepthFunction.Lequal);
        GL.Disable(EnableCap.CullFace);

        VAO.Bind();

        _shader.Use();
        _shader.SetUniform("view", new Matrix4(new Matrix3(view)));
        _shader.SetUniform("projection", projection);

        GL.ActiveTexture(TextureUnit.Texture0);
        GL.BindTexture(TextureTarget.TextureCubeMap, _textureHandle);

        GL.DrawElements(PrimitiveType.Triangles, _skyboxIndices.Length, DrawElementsType.UnsignedInt, 0);

        _shader.Unbind();
        GL.DepthFunc(DepthFunction.Less);
        GL.Enable(EnableCap.CullFace);
    }
}