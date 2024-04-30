using Nez;
using Microsoft.Xna.Framework;
using ImGuiNET;

namespace Raven
{
  public class DashboardScene : Scene
  {
    EditorSettings _settings;
    ImGuiWinManager _imgui = new ImGuiWinManager();
    Widget.PopupDelegate _dialog = new Widget.PopupDelegate("dialog");
    public DashboardScene()
    {
      ClearColor = Color.Black;
    }
    public override void OnStart()
    {
      var imgui = Core.GetGlobalManager<Nez.ImGuiTools.ImGuiManager>();
      imgui.RegisterDrawCommand(RenderImGui);
      if (!Path.Exists(Serializer.ApplicationSavePath))
        _settings = new EditorSettings();
      else
        _settings = Serializer.LoadSettings();
    }    
    void RenderImGui()
    {
      RenderDockSpace();

      _dialog.Render(_imgui);

      ImGui.Begin("Updates");
      ImGuiUtils.TextMiddle("This is the latest version. Debug 1.");
      ImGui.End();

      ImGui.Begin("File Manager");

      var size = new System.Numerics.Vector2(ImGui.GetContentRegionAvail().X * 0.5f - ImGui.GetStyle().ItemSpacing.X, 20);
      if (ImGui.Button("New Sheet", size))
      {
        _imgui.FilePicker.Open(path => 
            Core.StartSceneTransition(new FadeTransition(()=>new EditorScene(new Editor(new SheetView(), new Sheet(path))))), "New Sheet");
      }
      ImGui.SameLine();
      if (ImGui.Button("New World", size))
      { 
        Core.StartSceneTransition(new FadeTransition(()=>new EditorScene(new Editor(new WorldView(), new World()))));
      }
      if (ImGui.Button("Open Sheet", size))
      { 
        _imgui.FilePicker.Open(path=>
            Core.StartSceneTransition(new FadeTransition(()=>new EditorScene(new Editor(new SheetView(), Serializer.LoadContent<Sheet>(path))))), "Open Sheet"); 
      }
      ImGui.SameLine();
      if (ImGui.Button("Open World", size))
      { 
        _imgui.FilePicker.Open(path=>
            Core.StartSceneTransition(new FadeTransition(()=>new EditorScene(new Editor(new WorldView(), Serializer.LoadContent<World>(path))))), "Open World"); 
      }
 
      var i = 0;
      List<int> invalidFiles = new List<int>(); 
      foreach (var recent in _settings.FileHistory)
      {
        if (ImGui.MenuItem(recent.Filename.BestWrap())) 
        {
          if (!Path.Exists(recent.Filename))
          {
            invalidFiles.Add(i);
            _dialog.Open(imgui => ImGuiUtils.TextMiddle("File no longer exist."));
            continue;
          }
      
          // does not exist in the last file
          if (_settings.LastFiles.Find(item => item.Filename == recent.Filename) == null)
          {
            _settings.LastFiles.Add(recent);
            _settings.LastFile = _settings.LastFiles.Count() - 1;
          }
          else _settings.LastFile = i;
          
          new Serializers.SettingsSerializer().Save(Serializer.ApplicationSavePath, _settings);
          Core.StartSceneTransition(new FadeTransition(()=>new EditorScene()));
        }
        i++;
      }
      for (i = invalidFiles.Count()-1; i >= 0; i--) 
      {
        _settings.FileHistory.RemoveAt(invalidFiles[i]); 
      }
      new Serializers.SettingsSerializer().Save(Serializer.ApplicationSavePath, _settings);

      ImGui.End();

      _imgui.Render();
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
