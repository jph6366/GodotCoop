namespace GodotCoop;

using Chickensoft.Introspection;

public partial class PlayerLogic
{
  public partial record State
  {
    [Meta]
    public abstract partial record Midair
          : State, IGet<Input.HitFloor>, IGet<Input.StartedFalling>
    {
      public Transition On(in Input.HitFloor input) =>
        input.IsMovingHorizontally ? To<Moving>() : To<Idle>();

      public Transition On(in Input.StartedFalling input) => To<Falling>();
    }
  }
}
