namespace GodotCoop;

using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.Introspection;
using Godot;


[Meta(typeof(IAutoNode))]
public partial class InGameUI : Control, IControl, IDependent
{
    public override void _Notification(int what) => this.Notify(what);

    #region Dependencies

    [Dependency] public IAppRepo AppRepo => this.DependOn<IAppRepo>();
    [Dependency] public IGameRepo GameRepo => this.DependOn<IGameRepo>();

    #endregion Dependencies

    #region Nodes


    public MixinBlackboard MixinState => throw new System.NotImplementedException();

    public IMetatype Metatype => throw new System.NotImplementedException();

    #endregion Nodes


    #region State

    public IInGameUILogic InGameUILogic { get; set; } = default!;

    public InGameUILogic.IBinding InGameUIBinding { get; set; } = default!;

    #endregion State

    public void Setup()
    {
        InGameUILogic = new InGameUILogic();
    }

    public void OnResolved()
    {
        InGameUILogic.Set(this);
        InGameUILogic.Set(AppRepo);
        InGameUILogic.Set(GameRepo);

        InGameUIBinding = InGameUILogic.Bind();


        InGameUILogic.Start();
    }
    public void OnExitTree()
    {
        InGameUILogic.Stop();
        InGameUIBinding.Dispose();
    }
}