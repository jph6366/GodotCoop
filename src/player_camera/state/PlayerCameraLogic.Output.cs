namespace GodotCoop;
using Godot;

public partial class PlayerCameraLogic {
  public static class Output {
    public readonly record struct GlobalTransformChanged(
      Transform2D GlobalTransform
    );
    public readonly record struct CameraLocalPositionChanged(
      Vector2 CameraLocalPosition
    ); public readonly record struct CameraOffsetChanged(Vector2 Offset);
  }
}
