using OpenTK.Graphics.OpenGL4;

namespace Projekt_OpenTK;

public class VertexArrayObject<TVertexType, TIndexType> : IDisposable
    where TVertexType : unmanaged
    where TIndexType : unmanaged
{
    private readonly int _handle;

    public VertexArrayObject(BufferObject<TVertexType> vbo, BufferObject<TIndexType> ebo)
    {
        _handle = GL.GenVertexArray();
        Bind();
        vbo.Bind();
        ebo.Bind();
    }

    public void Dispose()
    {
        GL.DeleteVertexArray(_handle);
    }

    public unsafe void VertexAttributePointer(int index, int count, VertexAttribPointerType type, int vertexSize,
                                              int offSet)
    {
        GL.VertexAttribPointer(index, count, type, false, vertexSize * sizeof(TVertexType),
                               (nint) (offSet * sizeof(TVertexType)));
        GL.EnableVertexAttribArray(index);
    }

    public void Bind()
    {
        GL.BindVertexArray(_handle);
    }
}