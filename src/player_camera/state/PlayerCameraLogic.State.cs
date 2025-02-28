namespace GodotCoop;

using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using Godot;

public partial class PlayerCameraLogic {
  /// <summary>
  ///   Overall player camera state. This would be abstract, but it's helpful to
  ///   be able to instantiate it by itself for easier testing.
  /// </summary>
  [Meta]
  public abstract partial record State : StateLogic<State>,
    IGet<Input.PhysicsTicked>,
    IGet<Input.TargetPositionChanged>,
    IGet<Input.TargetOffsetChanged> {
    public State() {
      OnAttach(
        () => {
          var gameRepo = Get<IGameRepo>();
          gameRepo.PlayerGlobalPosition.Sync += OnPlayerGlobalPositionChanged;
        }
      );

      OnDetach(
        () => {
          var gameRepo = Get<IGameRepo>();
          gameRepo.PlayerGlobalPosition.Sync -= OnPlayerGlobalPositionChanged;
        }
      );
    }

    internal void OnPlayerGlobalPositionChanged(Vector3 position) =>
      Input(new Input.TargetPositionChanged(position));

    internal void OnCameraTargetOffsetChanged(Vector3 targetOffset) =>
      Input(new Input.TargetOffsetChanged(targetOffset));

    public Transition On(in Input.PhysicsTicked input) {
      var camera = Get<IPlayerCamera2D>();
      var gameRepo = Get<IGameRepo>();
      var settings = Get<PlayerCameraSettings>();
      var data = Get<Data>();

      // Lerp to the desired horizontal position.
      var cameraPosition = camera.CameraTransform;
      cameraPosition.X = Mathf.Lerp(
        cameraPosition.X,
        data.TargetPosition.X,
        (float)input.Delta * settings.HorizontalMoveAcceleration
      );

      // Lerp to the desired vertical position.
      cameraPosition.Y = Mathf.Lerp(
        cameraPosition.Y,
        data.TargetPosition.Y,
        (float)input.Delta * settings.VerticalMoveAcceleration
      );


      // Update the camera's position.
      Output(new Output.GlobalTransformChanged(cameraPosition));

      // Lerp the camera offset.
      var offset = camera.Offset.Lerp(
        data.TargetOffset, (float)input.Delta * settings.OffsetAdjSpeed
      );

      Output(new Output.CameraOffsetChanged(offset));

      return ToSelf();
    }

    public Transition On(in Input.TargetPositionChanged input) {
      var data = Get<Data>();
      data.TargetPosition = input.TargetPosition;
      return ToSelf();
    }

    public Transition On(in Input.TargetOffsetChanged input) {
      var data = Get<Data>();
      data.TargetOffset = input.TargetOffset;
      return ToSelf();
    }
  }
}