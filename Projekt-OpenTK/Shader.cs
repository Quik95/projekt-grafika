using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace Projekt_OpenTK;

public class Shader : IDisposable
{
    private readonly int _handle;

    public Shader(string vertexPath, string fragmentPath)
    {
        var vertex = LoadShader(ShaderType.VertexShader, vertexPath);
        var fragment = LoadShader(ShaderType.FragmentShader, fragmentPath);
        _handle = GL.CreateProgram();
        GL.AttachShader(_handle, vertex);
        GL.AttachShader(_handle, fragment);
        GL.LinkProgram(_handle);
        GL.GetProgram(_handle, GetProgramParameterName.LinkStatus, out var status);
        if (status == 0) throw new Exception($"Program failed to link with error: {GL.GetProgramInfoLog(_handle)}");
        GL.DetachShader(_handle, vertex);
        GL.DetachShader(_handle, fragment);
        GL.DeleteShader(vertex);
        GL.DeleteShader(fragment);
    }

    public void Dispose()
    {
        GL.DeleteProgram(_handle);
    }

    public void Use()
    {
        GL.UseProgram(_handle);
    }

    public void Unbind()
    {
        GL.UseProgram(0);
    }

    public void SetUniform(string name, int value)
    {
        var location = GL.GetUniformLocation(_handle, name);
        if (location == -1) throw new Exception($"{name} uniform not found on shader.");
        GL.Uniform1(location, value);
    }

    public void SetUniform(string name, Matrix4 value)
    {
        //A new overload has been created for setting a uniform so we can use the transform in our shader.
        var location = GL.GetUniformLocation(_handle, name);
        if (location == -1) throw new Exception($"{name} uniform not found on shader.");
        GL.UniformMatrix4(location, true, ref value);
    }

    public void SetUniform(string name, Vector4 value)
    {
        var location = GL.GetUniformLocation(_handle, name);
        if (location == -1)
            throw new ArgumentException($"{name} uniform not found on shader.");
        GL.Uniform4(location, ref value);
    }

    public void SetUniform(string name, Vector3 value)
    {
        var location = GL.GetUniformLocation(_handle, name);
        if (location == -1)
            throw new ArgumentException($"{name} uniform not found on shader.");
        GL.Uniform3(location, ref value);
    }

    public void SetUniform(string name, float value)
    {
        var location = GL.GetUniformLocation(_handle, name);
        if (location == -1) throw new Exception($"{name} uniform not found on shader.");
        GL.Uniform1(location, value);
    }

    private int LoadShader(ShaderType type, string path)
    {
        var src = File.ReadAllText(path);
        var handle = GL.CreateShader(type);
        GL.ShaderSource(handle, src);
        GL.CompileShader(handle);
        var infoLog = GL.GetShaderInfoLog(handle);
        if (!string.IsNullOrWhiteSpace(infoLog))
            throw new Exception($"Error compiling shader of type {type}, failed with error {infoLog}");

        return handle;
    }
}