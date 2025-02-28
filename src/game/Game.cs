namespace GodotCoop;

using System;
using Chickensoft.AutoInject;
using Chickensoft.Collections;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.Introspection;
using Godot;

public interface IGame : INode3D,
IProvide<IGameRepo> {
  void LoadExistingGame();

}

[Meta(typeof(IAutoNode))]
public partial class Game : Node3D, IGame, IDependent {
  public override void _Notification(int what) => this.Notify(what);


  #region State

  public IGameRepo GameRepo { get; set; } = default!;
  public IGameLogic GameLogic { get; set; } = default!;

  public GameLogic.IBinding GameBinding { get; set; } = default!;

  #endregion State

  #region Nodes

  [Node] public IPlayerCamera2D PlayerCamera { get; set; } = default!;

  [Node] public IPlayer Player { get; set; } = default!;

  // [Node] public IMap Map { get; set; } = default!;
  [Node] public IControl InGameUI { get; set; } = default!;
  [Node] public IDefeatMenu DefeatMenu { get; set; } = default!;
  [Node] public IVictoryMenu VictoryMenu { get; set; } = default!;
  [Node] public IPauseMenu PauseMenu { get; set; } = default!;

  #endregion Nodes

  #region Provisions

  IGameRepo IProvide<IGameRepo>.Value() => GameRepo;

  #endregion Provisions

  #region Dependencies

  [Dependency] public IAppRepo AppRepo => this.DependOn<IAppRepo>();

  public MixinBlackboard MixinState => throw new NotImplementedException();

  public IMetatype Metatype => throw new NotImplementedException();

  #endregion Dependencies

  public void Setup() {
    // FileSystem = new FileSystem();

    // SaveFilePath = FileSystem.Path.Join(OS.GetUserDataDir(), SAVE_FILE_NAME);

    GameRepo = new GameRepo();
    GameLogic = new GameLogic();
    GameLogic.Set(GameRepo);
    GameLogic.Set(AppRepo);

    // This is how to create JsonSerializerOptions for use with LogicBlocks
    // and the Chickensoft serialization utilities.
    // var resolver = new SerializableTypeResolver();
    // // Tell our type type resolver about the Godot-specific converters.
    // GodotSerialization.Setup();

    var upgradeDependencies = new Blackboard();

    DefeatMenu.TryAgain += OnStart;
    DefeatMenu.MainMenu += OnMainMenu;
    DefeatMenu.TransitionCompleted += OnDefeatMenuTransitioned;

    VictoryMenu.MainMenu += OnMainMenu;
    VictoryMenu.TransitionCompleted += OnVictoryMenuTransitioned;

    PauseMenu.MainMenu += OnMainMenu;
    PauseMenu.Resume += OnResume;
    PauseMenu.TransitionCompleted += OnPauseMenuTransitioned;
    PauseMenu.Save += OnPauseMenuSaveRequested;

    // GameChunk = new SaveChunk<GameData>(
    //   (chunk) =>
    //   {
    //       var gameData = new GameData()
    //       {
    //           MapData = chunk.GetChunkSaveData<MapData>(),
    //           PlayerData = chunk.GetChunkSaveData<PlayerData>(),
    //           PlayerCameraData = chunk.GetChunkSaveData<PlayerCameraData>()
    //       };

    //       return gameData;
    //   },
    //     onLoad: (chunk, data) =>
    //     {
    //         chunk.LoadChunkSaveData(data.MapData);
    //         chunk.LoadChunkSaveData(data.PlayerData);
    //         chunk.LoadChunkSaveData(data.PlayerCameraData);
    //     }
    //   );

    // Calling Provide() triggers the Setup/OnResolved on dependent
    // nodes who depend on the values we provide. This means that
    // all nodes registering save managers will have already registered
    // their relevant save managers by now. This is useful when restoring state
    // while loading an existing save file.
  }

  public void OnResolved() {

    GameBinding = GameLogic.Bind();
    GameBinding
      .Handle(
        (in GameLogic.Output.StartGame _) => {
          PlayerCamera.UsePlayerCamera();
          InGameUI.Show();
        })
      .Handle(
        (in GameLogic.Output.SetPauseMode output) =>
          CallDeferred(nameof(SetPauseMode), output.IsPaused)
      )
      .Handle((in GameLogic.Output.ShowLostScreen _) => {
        DefeatMenu.Show();
        DefeatMenu.FadeIn();
        DefeatMenu.Animate();
      })
      .Handle((in GameLogic.Output.ExitLostScreen _) => DefeatMenu.FadeOut())
      .Handle((in GameLogic.Output.ShowPauseMenu _) => {
        PauseMenu.Show();
        PauseMenu.FadeIn();
      })
      .Handle((in GameLogic.Output.ShowWonScreen _) => {
        VictoryMenu.Show();
        VictoryMenu.FadeIn();
      })
      .Handle((in GameLogic.Output.ExitWonScreen _) => VictoryMenu.FadeOut())
      .Handle((in GameLogic.Output.ExitPauseMenu _) => PauseMenu.FadeOut())
      .Handle((in GameLogic.Output.HidePauseMenu _) => PauseMenu.Hide())
      .Handle((in GameLogic.Output.ShowPauseSaveOverlay _) =>
        PauseMenu.OnSaveStarted()
      )
      .Handle((in GameLogic.Output.HidePauseSaveOverlay _) =>
        PauseMenu.OnSaveCompleted()
      );

    // Trigger the first state's OnEnter callbacks so our bindings run.
    // Keeps everything in sync from the moment we start!
    GameLogic.Start();

    GameLogic.Input(
      new GameLogic.Input.Initialize(placeholder: 0)
    );

    this.Provide();
  }

  public override void _Input(InputEvent @event) {
    if (Input.IsActionJustPressed("ui_cancel")) {
      GameLogic.Input(new GameLogic.Input.PauseButtonPressed());
    }
  }

  public void OnMainMenu() =>
    GameLogic.Input(new GameLogic.Input.GoToMainMenu());

  public void OnResume() =>
    GameLogic.Input(new GameLogic.Input.PauseButtonPressed());

  public void OnStart() =>
    GameLogic.Input(new GameLogic.Input.Start());

  public void OnVictoryMenuTransitioned() =>
    GameLogic.Input(new GameLogic.Input.VictoryMenuTransitioned());

  public void OnPauseMenuTransitioned() =>
    GameLogic.Input(new GameLogic.Input.PauseMenuTransitioned());

  public void OnPauseMenuSaveRequested() =>
    GameLogic.Input(new GameLogic.Input.SaveRequested());

  public void OnDefeatMenuTransitioned() =>
    GameLogic.Input(new GameLogic.Input.DefeatMenuTransitioned());

  public void OnExitTree() {
    DefeatMenu.TryAgain -= OnStart;
    DefeatMenu.MainMenu -= OnMainMenu;
    DefeatMenu.TransitionCompleted -= OnDefeatMenuTransitioned;
    VictoryMenu.MainMenu -= OnMainMenu;
    PauseMenu.MainMenu -= OnMainMenu;
    PauseMenu.Resume -= OnResume;
    PauseMenu.TransitionCompleted -= OnPauseMenuTransitioned;

    GameLogic.Stop();
    GameBinding.Dispose();
    GameRepo.Dispose();
  }

  public void LoadExistingGame() => GD.Print("Hello World");
  private void SetPauseMode(bool isPaused) => GetTree().Paused = isPaused;
}