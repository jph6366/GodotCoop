namespace GodotCoop;

using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.Introspection;
using Godot;

/// <summary>
///   Interface for the player camera in a 2.5D environment.
///   Exposes relevant properties to the logic layer, enabling easy unit testing.
/// </summary>
public interface IPlayerCamera2D : INode2D {
    IPlayerCameraLogic CameraLogic { get; }
    Vector2 Offset { get; }
    Vector2 CameraLocalPosition { get; }
    Transform2D CameraTransform { get; }
    void UsePlayerCamera();
}

[Meta(typeof(IAutoNode))]
public partial class PlayerCamera : Node2D, IPlayerCamera2D, IDependent {
    public override void _Notification(int what) => this.Notify(what);

    #region State
    [Dependency] public IGameRepo GameRepo => this.DependOn<IGameRepo>();
    public IPlayerCameraLogic CameraLogic { get; private set; } = default!;
    private PlayerCameraLogic.IBinding CameraBinding { get; set; } = default!;
    #endregion

    #region Exports
    [Export] public Vector2 Offset { get; private set; } = Vector2.Zero;

    [Export(PropertyHint.ResourceType, "PlayerCameraSettings")]
    public PlayerCameraSettings Settings { get; private set; } = new();
    #endregion

    #region Nodes
    [Node("%Camera2D")] public ICamera2D CameraNode { get; private set; } = default!;
    #endregion

    #region Computed
    public Vector2 CameraLocalPosition => CameraNode.Position;
    public Transform2D CameraTransform => CameraNode.GlobalTransform;

    public MixinBlackboard MixinState => throw new System.NotImplementedException();

    public IMetatype Metatype => throw new System.NotImplementedException();
    #endregion

    /// <summary>
    /// Initializes camera logic and binds it to the game repository.
    /// </summary>
    public void Setup() {
        CameraLogic = new PlayerCameraLogic();
        CameraLogic.Set(this);
        CameraLogic.Set(Settings);
        CameraLogic.Set(GameRepo);
        SetPhysicsProcess(true);
    }

    /// <summary>
    /// Handles dependency resolution and binds camera logic events.
    /// </summary>
    public void OnResolved() {
        CameraBinding = CameraLogic.Bind();
        CameraBinding
            .Handle((in PlayerCameraLogic.Output.GlobalTransformChanged output) =>
                GlobalPosition = output.GlobalTransform.Origin)
            .Handle((in PlayerCameraLogic.Output.CameraLocalPositionChanged output) =>
                CameraNode.Position = output.CameraLocalPosition);
        CameraLogic.Start();
    }

    /// <summary>
    /// Processes joystick input for camera movement in a 2.5D context.
    /// </summary>
    public void OnPhysicsProcess(double delta) {
        HandleCameraInput();
        CameraLogic.Input(new PlayerCameraLogic.Input.PhysicsTicked(delta));
    }

    /// <summary>
    /// Captures joystick input for smooth camera movement.
    /// </summary>
    private void HandleCameraInput() {
        var xMotion = InputUtilities.GetJoyPadActionPressedMotion("camera_left", "camera_right", JoyAxis.RightX);
        if (xMotion is not null)
            CameraLogic.Input(new PlayerCameraLogic.Input.JoyPadInputOccurred(xMotion));

        var yMotion = InputUtilities.GetJoyPadActionPressedMotion("camera_up", "camera_down", JoyAxis.RightY);
        if (yMotion is not null)
            CameraLogic.Input(new PlayerCameraLogic.Input.JoyPadInputOccurred(yMotion));
    }

    /// <summary>
    /// Activates this camera as the primary player camera.
    /// </summary>
    public void UsePlayerCamera() => CameraNode.MakeCurrent();

    /// <summary>
    /// Ensures proper cleanup when the camera is removed.
    /// </summary>
    public void OnExitTree() {
        CameraLogic.Stop();
        CameraBinding.Dispose();
    }
}
