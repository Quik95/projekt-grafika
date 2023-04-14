using OpenTK.Graphics.OpenGL4;

namespace Projekt_OpenTK;

public class BufferObject<TDataType> : IDisposable
    where TDataType : unmanaged
{
    private readonly BufferTarget _bufferType;
    private readonly int          _handle;

    public unsafe BufferObject(Span<TDataType> data, BufferTarget bufferType)
    {
        _bufferType = bufferType;

        _handle = GL.GenBuffer();
        Bind();
        fixed (void* d = data)
        {
            GL.BufferData(bufferType, data.Length * sizeof(TDataType), (nint) d, BufferUsageHint.StaticDraw);
        }

        GL.BindBuffer(bufferType, 0);
        GL.BindVertexArray(0);
    }

    public void Dispose()
    {
        GL.DeleteBuffer(_handle);
    }

    public void Bind()
    {
        GL.BindBuffer(_bufferType, _handle);
    }
}