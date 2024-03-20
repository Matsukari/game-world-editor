using Nez;
using Nez.ImGuiTools;
using ImGuiNET;

namespace TestGame;

public class TestGameProcess : Core
{
    protected override void Initialize()
    {
        base.Initialize();
        RegisterGlobalManager(new Raven.Input.InputManager());
        ExitOnEscapeKeypress = false;
        Window.IsBorderless = true; 
        IsFixedTimeStep = true;
        var imGuiOptions = new ImGuiOptions();
        imGuiOptions.AddFont("Assets/Raw/" + Assets.Unprocessed.Fonts.RobotoCondensedRegular, 13);
        imGuiOptions.AddFont("Assets/Raw/" + Assets.Unprocessed.Fonts.FontAwesome6FreeSolid900, 13);
        imGuiOptions.IncludeDefaultFont(false);
        
        var imGuiManager = new ImGuiManager(imGuiOptions);
        Core.RegisterGlobalManager( imGuiManager );
        NezImGuiThemes.HighContrast();
        Raven.Sheet.GuiStyles.StyleViolet();
        ImGui.GetIO().ConfigFlags |= ImGuiNET.ImGuiConfigFlags.DockingEnable;
        imGuiManager.ShowMenuBar = false;
        Scene = new SpriteSheetEditorScene();


    }
    [Nez.Console.Command( "show", "Shows something which would be otherwise hidden." )]
    static void ShowCommand( string which = "imdemo" )
    {
      var imgui = Core.GetGlobalManager<ImGuiManager>();
      if (which == "imdemo") imgui.ShowDemoWindow = !imgui.ShowDemoWindow;
      else if (which == "scene") imgui.ShowSceneGraphWindow = !imgui.ShowSceneGraphWindow;
      else if (which == "core") imgui.ShowCoreWindow = !imgui.ShowCoreWindow;

    }


}
