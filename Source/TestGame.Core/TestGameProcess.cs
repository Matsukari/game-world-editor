using Nez;
using Nez.ImGuiTools;

namespace TestGame;

public class TestGameProcess : Core
{
    protected override void Initialize()
    {
        base.Initialize();
        // TODO: Add your initialization logic here
        Window.IsBorderless = true; 
        IsFixedTimeStep = true;
        var imGuiManager = new ImGuiManager();
        Core.RegisterGlobalManager( imGuiManager );
        NezImGuiThemes.HighContrast();
        ImGuiNET.ImGui.GetIO().ConfigFlags |= ImGuiNET.ImGuiConfigFlags.DockingEnable;
        Scene = new Tools.SpriteSheetEditorScene();

    }

}
