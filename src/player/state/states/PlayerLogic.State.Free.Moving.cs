namespace GodotCoop;

using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

public partial class PlayerLogic
{
    public abstract partial record State
    {
        [Meta, Id("player_logic_state_free_moving")]
        public partial record Moving : Free,
        IGet<Input.StoppedMovingHorizontally>
        {
            public Moving()
            {
                this.OnEnter(() => Output(new Output.Animations.Move()));
            }

            public Transition On(in Input.StoppedMovingHorizontally input) =>
              To<Idle>();
        }
    }
}