using FlightSim;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Projekt_OpenTK;

namespace LearnOpenTK;

public class Window : GameWindow
{
    private const string      FragmentShaderAdvanced    = "Shaders/shader_advanced.frag";
    private const string      FragmentShaderDiffuseOnly = "Shaders/shader_diffuse_only.frag";
    private const string      VertexShaderBlinnPhong    = "Shaders/blinn-phong.vert";
    private const string      FragmentShaderBlinnPhong  = "Shaders/blinn-phong.frag";
    private       Camera      _camera;
    private       int         _elementBufferObject;
    private       bool        _firstMove = true;
    private       Vector2     _lastPos;
    private       string[]    _models;
    private       ModelAssimp _selectedModel;
    private       int         _selectedModelIndex;
    private       Shader      _shader;
    private       Skybox      _skybox;
    public        double      _time;

    public Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
        : base(gameWindowSettings, nativeWindowSettings)
    {
        UpdateFrequency = 120;
    }

    protected override void OnLoad()
    {
        base.OnLoad();

        GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

        GL.Enable(EnableCap.DepthTest);
        GL.Enable(EnableCap.CullFace);

        _shader = new Shader("Shaders/shader.vert", FragmentShaderDiffuseOnly);
        // _shader = new Shader(VertexShaderBlinnPhong, FragmentShaderBlinnPhong);
        _shader.Use();

        _models = new[]
        {
            // "assets/dragon/dragon.obj",
            // "assets/buddha/buddha.obj",
            "assets/Jet/11805_airplane_v2_L2.obj",
            "assets/backpack/backpack.obj",
            "assets/DC10/DC-10-30.obj",
            "assets/test_cube.obj",
            "assets/lancia.obj",
            "assets/city.obj",
            "assets/MD-11/MD-11.obj"
        };
        _selectedModel = new ModelAssimp(_models[_selectedModelIndex]);

        _camera = new Camera(Vector3.UnitZ * 3, Size.X / (float) Size.Y);
        _skybox = new Skybox();

        CursorState = CursorState.Grabbed;
#if DEBUG
        CursorState = CursorState.Normal;
#endif
    }

    protected override void OnRenderFrame(FrameEventArgs e)
    {
        base.OnRenderFrame(e);
        // Title = $"FPS: {1f / e.Time:0} - {e.Time/1000}ms - {Size.X}x{Size.Y}";

        _time += 4.0 * e.Time;

        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        var model = Matrix4.Identity;
        model *= Matrix4.CreateScale(1.5f);
        if (new[] {3, 5, 6}.Contains(_selectedModelIndex))
            model *= Matrix4.CreateScale(0.05f);
        if (_selectedModelIndex == 0) {
            model *= Matrix4.CreateScale(0.01f);
            model *= Matrix4.CreateRotationX(MathHelper.DegreesToRadians(-90f));
        }

        _shader.Use();
        _shader.SetUniform("model", model);
        _shader.SetUniform("view", _camera.GetViewMatrix());
        _shader.SetUniform("projection", _camera.GetProjectionMatrix());
        // _shader.SetUniform("viewPos", _camera.Position);
        // _shader.SetUniform("lightPos", new Vector3(0.0f, 0.0f, 0.0f));

        foreach (var mesh in _selectedModel.Meshes)
        {
            mesh.Bind();
            GL.DrawArrays(PrimitiveType.Triangles, 0, mesh.Vertices.Length);
        }

        _skybox.Draw(_camera.GetViewMatrix(), _camera.GetProjectionMatrix());
        SwapBuffers();
    }

    protected override void OnUpdateFrame(FrameEventArgs e)
    {
        base.OnUpdateFrame(e);

        if (!IsFocused) // Check to see if the window is focused
            return;

        var input = KeyboardState;

        if (input.IsKeyDown(Keys.Escape)) Close();

        const float cameraSpeed = 5.5f;
        const float sensitivity = 0.2f;

        if (input.IsKeyDown(Keys.W)) _camera.Position += _camera.Front * cameraSpeed * (float) e.Time; // Forward

        if (input.IsKeyDown(Keys.S)) _camera.Position -= _camera.Front * cameraSpeed * (float) e.Time; // Backwards
        if (input.IsKeyDown(Keys.A)) _camera.Position -= _camera.Right * cameraSpeed * (float) e.Time; // Left
        if (input.IsKeyDown(Keys.D)) _camera.Position += _camera.Right * cameraSpeed * (float) e.Time; // Right
        if (input.IsKeyDown(Keys.Space)) _camera.Position += _camera.Up * cameraSpeed * (float) e.Time; // Up
        if (input.IsKeyDown(Keys.LeftShift)) _camera.Position -= _camera.Up * cameraSpeed * (float) e.Time; // Down
        if (input.IsKeyPressed(Keys.F))
        {
            WindowState = WindowState == WindowState.Normal ? WindowState.Fullscreen : WindowState.Normal;
#if DEBUG
            CursorState = WindowState == WindowState.Normal ? CursorState.Normal : CursorState.Grabbed;
#endif
        }

        if (input.IsKeyPressed(Keys.Q)) Environment.Exit(0);

        if (input.IsKeyDown(Keys.LeftControl))
            CursorState = CursorState.Normal;
        if (input.IsKeyReleased(Keys.LeftControl))
            CursorState = CursorState.Grabbed;

        foreach (var key in Enumerable.Range(49, 9))
        {
            if (!input.IsKeyPressed((Keys) key))
                continue;
            _selectedModel.Dispose();
            _selectedModelIndex = key - 49;
            _selectedModel      = new ModelAssimp(_models[_selectedModelIndex]);
        }

        // Get the mouse state
        var mouse = MouseState;

        if (_firstMove) // This bool variable is initially set to true.
        {
            _lastPos   = new Vector2(mouse.X, mouse.Y);
            _firstMove = false;
        }
        else
        {
            // Calculate the offset of the mouse position
            var deltaX = mouse.X - _lastPos.X;
            var deltaY = mouse.Y - _lastPos.Y;
            _lastPos = new Vector2(mouse.X, mouse.Y);

            // Apply the camera pitch and yaw (we clamp the pitch in the camera class)
            _camera.Yaw   += deltaX * sensitivity;
            _camera.Pitch -= deltaY * sensitivity; // Reversed since y-coordinates range from bottom to top
        }
    }

    // In the mouse wheel function, we manage all the zooming of the camera.
    // This is simply done by changing the FOV of the camera.
    protected override void OnMouseWheel(MouseWheelEventArgs e)
    {
        base.OnMouseWheel(e);

        _camera.Fov -= e.OffsetY;
    }

    protected override void OnResize(ResizeEventArgs e)
    {
        base.OnResize(e);

        GL.Viewport(0, 0, Size.X, Size.Y);
        // We need to update the aspect ratio once the window has been resized.
        _camera.AspectRatio = Size.X / (float) Size.Y;
    }
}