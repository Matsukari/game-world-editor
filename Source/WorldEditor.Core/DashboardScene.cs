using Nez;
using Microsoft.Xna.Framework;
using ImGuiNET;

namespace Raven
{
  public class DashboardScene : Scene
  {
    public DashboardScene()
    {
      ClearColor = Color.Black;
    }
    public override void OnStart()
    {
      var imgui = Core.GetGlobalManager<Nez.ImGuiTools.ImGuiManager>();
      imgui.RegisterDrawCommand(RenderImGui);
    }    
    void RenderImGui()
    {
      Console.WriteLine("rendering");
      RenderDockSpace();

      ImGui.Begin("Main");

      if (ImGui.Button("Start")) Core.Scene = new EditorScene();
      ImGui.End();
    }
    void RenderDockSpace()
    {
      ImGuiDockNodeFlags dockspace_flags = ImGuiDockNodeFlags.None;

      ImGuiWindowFlags window_flags = ImGuiWindowFlags.MenuBar | ImGuiWindowFlags.NoDocking;

      var viewport = ImGui.GetMainViewport();
      ImGui.SetNextWindowPos(viewport.WorkPos);
      ImGui.SetNextWindowSize(viewport.WorkSize);
      ImGui.SetNextWindowViewport(viewport.ID);
      ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 0.0f);
      ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0.0f);
      window_flags |= ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoMove;
      window_flags |= ImGuiWindowFlags.NoBringToFrontOnFocus | ImGuiWindowFlags.NoNavFocus;

      ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, 0);
      ImGui.Begin("DockSpace", window_flags);
      uint dockspace_id = ImGui.GetID("MyDockSpace");
      ImGui.DockSpace(dockspace_id, new System.Numerics.Vector2(), dockspace_flags);

      ImGui.PopStyleVar();
      ImGui.PopStyleVar();
      ImGui.PopStyleVar();
      ImGui.End();
    }
    public override void Update()
    {
      base.Update();
    }
      
  }
}
