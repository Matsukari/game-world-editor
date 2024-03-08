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
        var imGuiManager = new ImGuiManager();
        Core.RegisterGlobalManager( imGuiManager );
        NezImGuiThemes.HighContrast();
        Raven.Sheet.GuiStyles.StyleViridescent();
        ImGui.GetIO().ConfigFlags |= ImGuiNET.ImGuiConfigFlags.DockingEnable;
        imGuiManager.ShowMenuBar = false;
        Scene = new SpriteSheetEditorScene();

    }

}
