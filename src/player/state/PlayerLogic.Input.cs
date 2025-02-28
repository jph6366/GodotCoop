namespace GodotCoop;

using Godot;

public partial class PlayerLogic
{
    public static class Input
    {
        public readonly record struct Enable;
        public readonly record struct PhysicsTick(double Delta);
        public readonly record struct Jump(double Delta);
        public readonly record struct Moved(Vector2 GlobalPosition);
        public readonly record struct HitFloor(bool IsMovingHorizontally);
        public readonly record struct LeftFloor(bool IsFalling);
        public readonly record struct StartedMovingHorizontally;
        public readonly record struct StoppedMovingHorizontally;
        public readonly record struct StartedFalling;
    }
}