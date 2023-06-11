using OpenTK.Graphics.OpenGL4;
using Silk.NET.Assimp;
using StbImageSharp;
using File = System.IO.File;

namespace Projekt_OpenTK;

public class Texture : IDisposable
{
    private readonly int _handle;

    public Texture(string path, TextureType type = TextureType.None, TextureUnit textureSlot = TextureUnit.Texture0)
    {
        Path    = path;
        Type    = type;
        _handle = GL.GenTexture();
        Bind(textureSlot);

        try
        {
            if (!path.Contains("backpack"))
                StbImage.stbi_set_flip_vertically_on_load(1);
            var img = ImageResult.FromStream(File.OpenRead(path), ColorComponents.RedGreenBlueAlpha);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, img.Width, img.Height, 0,
                          PixelFormat.Rgba, PixelType.UnsignedByte, img.Data);
        }
        catch (FileNotFoundException e)
        {
            Console.WriteLine(e);
        }


        SetParameters();
    }

    public unsafe Texture(Span<byte> data, int width, int height)
    {
        _handle = GL.GenTexture();
        Bind();

        fixed (void* d = &data[0])
        {
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, width, height, 0, PixelFormat.Rgba,
                          PixelType.UnsignedByte, (nint) d);
            SetParameters();
        }
    }

    public string      Path { get; set; }
    public TextureType Type { get; }

    public void Dispose()
    {
        GL.DeleteTexture(_handle);
    }

    private void SetParameters()
    {
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS,
                        (int) TextureParameterName.TextureWrapS);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT,
                        (int) TextureParameterName.TextureWrapT);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
                        (int) TextureMinFilter.LinearMipmapLinear);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int) TextureMagFilter.Linear);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureBaseLevel, 0);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMaxLevel, 8);
        GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
    }

    public void Bind(TextureUnit textureSlot = TextureUnit.Texture0)
    {
        GL.ActiveTexture(textureSlot);
        GL.BindTexture(TextureTarget.Texture2D, _handle);
    }
}