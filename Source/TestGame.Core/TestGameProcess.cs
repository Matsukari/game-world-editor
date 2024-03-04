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
        ExitOnEscapeKeypress = false;
        var imGuiManager = new ImGuiManager();
        Core.RegisterGlobalManager( imGuiManager );
        NezImGuiThemes.HighContrast();
        ImGuiNET.ImGui.GetIO().ConfigFlags |= ImGuiNET.ImGuiConfigFlags.DockingEnable;
        Scene = new SpriteSheetEditorScene();

    }

}
