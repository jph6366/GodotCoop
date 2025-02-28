namespace GodotCoop;

using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

public partial class GameLogic
{
    public partial record State
    {
        [Meta]
        public partial record MenuBackdrop : State,
        IGet<Input.Start>, IGet<Input.Initialize>
        {
            public MenuBackdrop()
            {
                OnAttach(() => Get<IAppRepo>().GameEntered += OnGameEntered);
                OnDetach(() => Get<IAppRepo>().GameEntered -= OnGameEntered);
            }

            public void OnGameEntered() => Input(new Input.Start());

            public Transition On(in Input.Start input) => To<Playing>();

            public Transition On(in Input.Initialize input)
            {
                return ToSelf();
            }
        }
    }
}