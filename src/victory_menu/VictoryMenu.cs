namespace GodotCoop;

using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.Introspection;
using Godot;

public interface IVictoryMenu : IControl
{
    event VictoryMenu.MainMenuEventHandler MainMenu;

    event VictoryMenu.TransitionCompletedEventHandler TransitionCompleted;

    void FadeIn();
    void FadeOut();
}

[Meta(typeof(IAutoNode))]
public partial class VictoryMenu : Control, IVictoryMenu
{
    public override void _Notification(int what) => this.Notify(what);

    #region Nodes

    [Node] public IButton MainMenuButton { get; set; } = default!;
    [Node] public IAnimationPlayer AnimationPlayer { get; set; } = default!;

    #endregion Nodes

    #region Signals

    [Signal]
    public delegate void MainMenuEventHandler();

    [Signal]
    public delegate void TransitionCompletedEventHandler();

    #endregion Signals

    public void OnReady() => MainMenuButton.Pressed += OnMainMenuPressed;

    public void OnExitTree() => MainMenuButton.Pressed -= OnMainMenuPressed;

    public void OnMainMenuPressed() => EmitSignal(SignalName.MainMenu);

    public void FadeIn() => AnimationPlayer.Play("fade_in");

    public void OnAnimationFinished(StringName name)
      => EmitSignal(SignalName.TransitionCompleted);

    public void FadeOut() => AnimationPlayer.Play("fade_out");
}