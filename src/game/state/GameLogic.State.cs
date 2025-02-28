namespace GodotCoop;

using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

public partial class GameLogic
{
    [Meta]
    public abstract partial record State : StateLogic<State>
    {
        protected State()
        {
            OnAttach(() =>
            {
                var gameRepo = Get<IGameRepo>();
                gameRepo.IsPaused.Sync += OnIsPaused;
            });

            OnDetach(() =>
            {
                var gameRepo = Get<IGameRepo>();
                gameRepo.IsPaused.Sync -= OnIsPaused;
            });
        }
        public void OnIsPaused(bool isPaused) =>
          Output(new Output.SetPauseMode(isPaused));
    }
}