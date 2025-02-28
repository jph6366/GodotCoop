namespace GodotCoop;

using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

public partial class InGameUILogic
{
    [Meta]
    public partial record State : StateLogic<State>
    {
        public State()
        {
            OnAttach(() =>
            {
                var gameRepo = Get<IGameRepo>();
            });

            OnDetach(() =>
            {
                var gameRepo = Get<IGameRepo>();
            });
        }
    }
}