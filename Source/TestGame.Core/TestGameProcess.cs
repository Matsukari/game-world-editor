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
        Window.IsBorderless = true; 
        IsFixedTimeStep = true;
        ExitOnEscapeKeypress = false;
        var imGuiManager = new ImGuiManager();
        Core.RegisterGlobalManager( imGuiManager );
        NezImGuiThemes.HighContrast();
        ImGui.GetIO().ConfigFlags |= ImGuiNET.ImGuiConfigFlags.DockingEnable;
        Scene = new SpriteSheetEditorScene();

    }

}
