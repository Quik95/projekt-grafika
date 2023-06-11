using FlightSim;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Projekt_OpenTK;

namespace LearnOpenTK;

internal enum CameraType
{
    Airplane,
    FPS
}

public class Window : GameWindow
{
    private AirplaneModel  _airplane;
    private AirplaneCamera _airplaneCamera;
    private Camera         _camera;
    private CameraType     _cameraType = CameraType.FPS;
    private bool           _firstMove  = true;
    private Ground         _ground;
    private Vector2        _lastPos;
    private Skybox         _skybox;

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

        var aspectRatio = Size.X / (float) Size.Y;
        _camera = new Camera(Vector3.UnitZ * 3, aspectRatio);

        _airplaneCamera = new AirplaneCamera(new Vector3(0.0f, 6.5f, 10.0f), aspectRatio,
                                             Matrix4.Identity * Matrix4.CreateScale(0.01f) *
                                             Matrix4.CreateRotationX(MathHelper.DegreesToRadians(-90f)));
        _airplane = new AirplaneModel();
        _skybox   = new Skybox();
        _ground   = new Ground();

        CursorState = CursorState.Grabbed;
#if DEBUG
        CursorState = CursorState.Normal;
#endif
    }

    protected override void OnRenderFrame(FrameEventArgs e)
    {
        base.OnRenderFrame(e);

        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        var viewMatrix = _cameraType switch
                         {
                             CameraType.Airplane => _airplaneCamera.GetViewMatrix(),
                             CameraType.FPS      => _camera.GetViewMatrix(),
                             _                   => throw new ArgumentOutOfRangeException()
                         };
        var projectionMatrix = _cameraType switch
                               {
                                   CameraType.Airplane => _airplaneCamera.GetProjectionMatrix(),
                                   CameraType.FPS      => _camera.GetProjectionMatrix(),
                                   _                   => throw new ArgumentOutOfRangeException()
                               };

        _airplane.Draw(viewMatrix, projectionMatrix);
        _ground.Draw(viewMatrix, projectionMatrix);
        _skybox.Draw(viewMatrix, projectionMatrix);
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

        if (input.IsKeyPressed(Keys.C))
            _cameraType = _cameraType == CameraType.FPS ? CameraType.Airplane : CameraType.FPS;

        if (input.IsKeyDown(Keys.LeftControl))
            CursorState = CursorState.Normal;
        if (input.IsKeyReleased(Keys.LeftControl))
            CursorState = CursorState.Grabbed;

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

            if (_cameraType is CameraType.FPS)
            {
                // Apply the camera pitch and yaw (we clamp the pitch in the camera class)
                _camera.Yaw   += deltaX * sensitivity;
                _camera.Pitch -= deltaY * sensitivity; // Reversed since y-coordinates range from bottom to top
            }
            else
            {
                _airplaneCamera.Yaw   -= deltaX * sensitivity;
                _airplaneCamera.Pitch += deltaY * sensitivity;
            }
        }


        _airplaneCamera.Position += _airplaneCamera.Front * _airplane.Speed * (float) e.Time;
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
        _camera.AspectRatio         = Size.X / (float) Size.Y;
        _airplaneCamera.AspectRatio = Size.X / (float) Size.Y;
    }
}