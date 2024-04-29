using Nez;
using Nez.ImGuiTools;
using ImGuiNET;

namespace TestGame;

public class TestGameProcess : Core
{
    protected override void Initialize()
    {
        base.Initialize();
        RegisterGlobalManager(new Raven.InputManager());
        RegisterGlobalManager(new Raven.CommandManager());

        Window.IsBorderless = true;
        Window.Title = "World Editor";
        Window.AllowUserResizing = true;
        ExitOnEscapeKeypress = false;

        Window.IsBorderless = true; 
        IsFixedTimeStep = true;
        var imGuiOptions = new ImGuiOptions();
        imGuiOptions.AddFont("Assets/Raw/" + Assets.Unprocessed.Fonts.RobotoCondensedRegular, 16);
        imGuiOptions.AddFont("Assets/Raw/" + Assets.Unprocessed.Fonts.FontAwesome6FreeSolid900, 18);
        imGuiOptions.IncludeDefaultFont(false);
        
        var imGuiManager = new ImGuiManager(imGuiOptions);
        imGuiManager.ShowSeperateGameWindow = false;
        imGuiManager.ShowCoreWindow = false;
        imGuiManager.ShowSceneGraphWindow = false;
        imGuiManager.ShowMenuBar = false;
        Core.RegisterGlobalManager( imGuiManager );
        NezImGuiThemes.HighContrast();
        Raven.GuiStyles.StyleViolet();
        ImGui.GetIO().ConfigFlags |= ImGuiNET.ImGuiConfigFlags.DockingEnable;        

        Scene = new Raven.DashboardScene();
        
    }
    [Nez.Console.Command( "show", "Shows something which would be otherwise hidden." )]
    static void ShowCommand( string which = "imdemo" )
    {
      var imgui = Core.GetGlobalManager<ImGuiManager>();
      if (which == "imdemo") imgui.ShowDemoWindow = !imgui.ShowDemoWindow;
      else if (which == "scene") imgui.ShowSceneGraphWindow = !imgui.ShowSceneGraphWindow;
      else if (which == "core") imgui.ShowCoreWindow = !imgui.ShowCoreWindow;
      else if (which == "style") imgui.ShowStyleEditor = !imgui.ShowStyleEditor;
      else if (which == "separate") imgui.ShowSeperateGameWindow = !imgui.ShowSeperateGameWindow;
    }
}
