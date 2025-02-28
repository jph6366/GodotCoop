namespace GodotCoop;

using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

public partial class PlayerLogic
{
    public partial record State
    {
        [Meta, Id("player_logic_state_midair_falling")]
        public partial record Falling : Midair
        {
            public Falling()
            {
                this.OnEnter(() => Output(new Output.Animations.Fall()));
            }
        }
    }
}