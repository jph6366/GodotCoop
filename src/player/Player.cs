namespace GodotCoop;

using Chickensoft.AutoInject;
using Chickensoft.Collections;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.Introspection;
using Godot;
using Compiler = System.Runtime.CompilerServices;

public interface IPlayer : ICharacterBody3D, IProvide<IPlayerLogic> {
  public IPlayerLogic PlayerLogic { get; }

  public bool IsMovingHorizontally();

  /// <summary>
  ///   Gets the input vector constrained to the XZ plane.
  /// </summary>
  /// <param name="cameraBasis">Camera's global transform basis.</param>
  public Vector3 GetGlobalInputVector(Basis cameraBasis);

  /// <summary>
  ///   Gets the next Y-axis rotation basis for smooth rotation.
  /// </summary>
  /// <param name="direction">Normalized global direction.</param>
  /// <param name="delta">Delta time.</param>
  /// <param name="rotationSpeed">Rotation speed (quaternions?/sec).</param>
  Basis GetNextRotationBasis(
    Vector3 direction,
    double delta,
    float rotationSpeed
  );
}

[Meta(typeof(IAutoNode))]
public partial class Player : CharacterBody3D, IPlayer, IDependent {
  public override void _Notification(int what) => this.Notify(what);

  #region Provisions
  IPlayerLogic IProvide<IPlayerLogic>.Value() => PlayerLogic;
  #endregion Provisions

  #region Dependencies
  [Dependency]
  public IGameRepo GameRepo => this.DependOn<IGameRepo>();

  [Dependency]
  public IAppRepo AppRepo => this.DependOn<IAppRepo>();
  #endregion Dependencies

  #region Exports

  [Export(PropertyHint.Range, "0, 100, 0.1")]
  public float RotationSpeed { get; set; } = 12.0f;

  [Export(PropertyHint.Range, "0, 100, 0.1")]
  public float StoppingSpeed { get; set; } = 1.0f;

  [Export(PropertyHint.Range, "-100, 0, 0.1")]
  public float Gravity { get; set; } = -30.0f;

  [Export(PropertyHint.Range, "0, 100, 0.1")]
  public float MoveSpeed { get; set; } = 8f;

  [Export(PropertyHint.Range, "0, 100, 0.1")]
  public float Acceleration { get; set; } = 4f;

  [Export(PropertyHint.Range, "0, 100, 0.1")]
  public float JumpImpulseForce { get; set; } = 12f;

  [Export(PropertyHint.Range, "0, 100, 0.1")]
  public float JumpForce { get; set; } = 4.5f;

  #endregion Exports

  #region State

  public IPlayerLogic PlayerLogic { get; set; } = default!;
  public PlayerLogic.Settings Settings { get; set; } = default!;

  public PlayerLogic.IBinding PlayerBinding { get; set; } = default!;

  public MixinBlackboard MixinState => throw new System.NotImplementedException();

  public IMetatype Metatype => throw new System.NotImplementedException();

  #endregion State

  public void Setup() {
    Settings = new PlayerLogic.Settings(
      RotationSpeed,
      StoppingSpeed,
      Gravity,
      MoveSpeed,
      Acceleration,
      JumpImpulseForce,
      JumpForce
    );

    PlayerLogic = new PlayerLogic();
    PlayerLogic.Set(this as IPlayer);
    PlayerLogic.Set(Settings);
    PlayerLogic.Set(AppRepo);
    PlayerLogic.Set(GameRepo);
  }

  public void OnReady() => SetPhysicsProcess(true);

  public void OnExitTree() {
    PlayerLogic.Stop();
    PlayerBinding.Dispose();
  }

  public void OnResolved() {
    PlayerBinding = PlayerLogic.Bind();
    GameRepo.SetPlayerGlobalPosition(GlobalPosition);

    PlayerBinding
      .Handle((in PlayerLogic.Output.MovementComputed output) => {
        Transform = Transform with { Basis = output.Rotation };
        Velocity = output.Velocity;
      })
      .Handle((in PlayerLogic.Output.VelocityChanged output) =>
        Velocity = output.Velocity
      );

    // Allow the player model to lookup our state machine and bind to it.
    this.Provide();
    // Start the player state machine last.
    PlayerLogic.Start();
  }

  public void OnPhysicsProcess(double delta) {
    PlayerLogic.Input(new PlayerLogic.Input.PhysicsTick(delta));

    var jumpPressed = Input.IsActionPressed(GameInputs.Jump);
    var jumpJustPressed = Input.IsActionJustPressed(GameInputs.Jump);

    if (ShouldJump(jumpPressed, jumpJustPressed)) {
      PlayerLogic.Input(new PlayerLogic.Input.Jump(delta));
    }

    MoveAndSlide();

    PlayerLogic.Input(new PlayerLogic.Input.Moved(new Vector2(GlobalPosition.X, GlobalPosition.Y)));
  }

  public static bool ShouldJump(bool jumpPressed, bool jumpJustPressed) =>
    jumpPressed || jumpJustPressed;

  #region IPlayer

  public Vector3 GetGlobalInputVector(Basis cameraBasis) {
    var rawInput = Input.GetVector(
      GameInputs.MoveLeft, GameInputs.MoveRight, GameInputs.MoveUp,
      GameInputs.MoveDown
    );

    // 2.5D Movement: Keep movement on the XZ plane only.
    var input = new Vector3 {
      X = rawInput.X,
      Z = rawInput.Y,
      Y = 0 // Ensure no Y movement from input
    };

    return cameraBasis * input;
  }

  public Basis GetNextRotationBasis(
    Vector3 direction,
    double delta,
    float rotationSpeed
  ) {
    // Lock rotation to Y-axis only
    var targetRotation = new Basis(Quaternion.FromEuler(new Vector3(0, Mathf.Atan2(direction.X, direction.Z), 0)));

    var scale = Transform.Basis.Scale;
    return new Basis(
      Transform
        .Basis
        .GetRotationQuaternion()
        .Slerp(targetRotation.GetRotationQuaternion(), (float)delta * rotationSpeed)
    ).Scaled(scale);
  }

  [Compiler.MethodImpl(Compiler.MethodImplOptions.AggressiveInlining)]
  public bool IsMovingHorizontally() =>
    (Velocity with { Y = 0f }).Length() > Settings.StoppingSpeed;

  #endregion IPlayer
}
