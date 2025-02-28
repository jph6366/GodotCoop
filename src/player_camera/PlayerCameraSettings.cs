namespace GodotCoop;

using Godot;

[GlobalClass]
public partial class PlayerCameraSettings : Resource {
    [Export(PropertyHint.Range, "0, 10, 0.01")]
    public float JoypadSensitivity { get; set; } = 5;

    /// <summary>
    /// Maximum vertical offset constraint (in pixels).
    /// </summary>
    [Export(PropertyHint.Range, "0, 500, 1")]
    public float VerticalMax { get; set; } = 45;

    /// <summary>
    /// Minimum vertical offset constraint (in pixels).
    /// </summary>
    [Export(PropertyHint.Range, "-500, 0, 1")]
    public float VerticalMin { get; set; } = -200;

    /// <summary>
    /// How fast the camera follows the target (pixels per second).
    /// </summary>
    [Export(PropertyHint.Range, "0, 500, 1")]
    public float FollowSpeed = 150f;

    /// <summary>
    /// How fast the camera follows the target offset (pixels per second).
    /// </summary>
    [Export(PropertyHint.Range, "0, 500, 1")]
    public float OffsetAdjSpeed = 20f;

    /// <summary>
    /// Acceleration for horizontal movement.
    /// </summary>
    [Export(PropertyHint.Range, "0, 500, 1")]
    public float HorizontalMoveAcceleration = 60f;

    /// <summary>
    /// Acceleration for vertical movement.
    /// </summary>
    [Export(PropertyHint.Range, "0, 500, 1")]
    public float VerticalMoveAcceleration = 60f;

    /// <summary>
    /// Minimum camera rotation angle (degrees).
    /// </summary>
    [Export(PropertyHint.Range, "-90, 0, 1")]
    public float AngleMin { get; set; } = -45f;

    /// <summary>
    /// Maximum camera rotation angle (degrees).
    /// </summary>
    [Export(PropertyHint.Range, "0, 90, 1")]
    public float AngleMax { get; set; } = 45f;
}