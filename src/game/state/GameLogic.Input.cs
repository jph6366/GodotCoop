namespace GodotCoop;

public partial class GameLogic {
    public static class Input {
        public readonly record struct Initialize(int placeholder);

        public readonly record struct EndGame(GameOverReason Reason);

        public readonly record struct PauseButtonPressed;

        public readonly record struct PauseMenuTransitioned;

        public readonly record struct VictoryMenuTransitioned;

        public readonly record struct DefeatMenuTransitioned;

        public readonly record struct SaveRequested;

        public readonly record struct SaveCompleted;

        public readonly record struct GoToMainMenu;

        public readonly record struct Start;
    }
}