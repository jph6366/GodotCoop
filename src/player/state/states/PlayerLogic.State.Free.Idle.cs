namespace GodotCoop;

using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

public partial class PlayerLogic
{
    public abstract partial record State
    {
        [Meta, Id("player_logic_state_free_idle")]
        public partial record Idle : Free,
        IGet<Input.StartedMovingHorizontally>
        {
            public Idle()
            {
                this.OnEnter(() => Output(new Output.Animations.Idle()));
            }

            public Transition On(in Input.StartedMovingHorizontally input) =>
              To<Moving>();
        }
    }
}