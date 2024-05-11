using Nez;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ImGuiNET;

namespace Raven
{
  public class DashboardScene : Scene
  {
    EditorSettings _settings;
    ImGuiWinManager _imgui = new ImGuiWinManager();
    Widget.PopupDelegate _dialog = new Widget.PopupDelegate("dialog");
    Texture2D _appLogo;
    public DashboardScene()
    {
      Content.RootDirectory = "Assets";
    }
    public override void OnStart()
    {
      var imgui = Core.GetGlobalManager<Nez.ImGuiTools.ImGuiManager>();
      imgui.RegisterDrawCommand(RenderImGui);
      if (!Path.Exists(Serializer.ApplicationSavePath))
        _settings = new EditorSettings();
      else
        _settings = Serializer.LoadSettings();

      ClearColor = _settings.Colors.WorldBackground.ToColor();
      _appLogo = Content.LoadTexture(WorldEditor.Assets.Images.Icon);

      _settings.ApplyImGui();
    }   
    void TryLoad(ContentView view, string path)
    {
      var content = Serializer.LoadContent<Sheet>(path);
      if (content == null)
      {
        _dialog.Open(imgui => ImGuiUtils.TextMiddle("Oops! Cannot open file. An error occured.")); 
        return;
      }
      Core.StartSceneTransition(new FadeTransition(()=>new EditorScene(new Editor(view, content)))); 
    }
    void RenderMenubar()
    {
      if (ImGui.BeginMainMenuBar())
      {
        if (ImGui.BeginMenu("File"))
        {
          if (ImGui.BeginMenu("Open Content"))
          {
            if (ImGui.MenuItem("Open Sheet"))
            { 
              _imgui.FilePicker.Open(path=>TryLoad(new SheetView(), path), "Open Sheet"); 
            }
            if (ImGui.MenuItem("Open World"))
            { 
              _imgui.FilePicker.Open(path=>TryLoad(new WorldView(), path), "Open World"); 
            }
            ImGui.EndMenu();
          }
          if (ImGui.BeginMenu("New Content"))
          {
            if (ImGui.MenuItem("New Sheet"))
            {
              _imgui.FilePicker.Open(path => 
                  Core.StartSceneTransition(new FadeTransition(()=>new EditorScene(new Editor(new SheetView(), new Sheet(path))))), "New Sheet");
            }
            if (ImGui.MenuItem("New World"))
            { 
              Core.StartSceneTransition(new FadeTransition(()=>new EditorScene(new Editor(new WorldView(), new World()))));
            }
            ImGui.EndMenu();
          }
          if (ImGui.MenuItem("Close"))
          {
            Core.Exit(); 
          }
          ImGui.EndMenu();
        }
        ImGui.EndMainMenuBar();
      }
    }
    void RenderFiles()
    {
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
      
          var content = Serializer.TryLoadContent(recent);
          if (content.Item1 == null || content.Item2 == null)
          {
            invalidFiles.Add(i);
            _dialog.Open(imgui => ImGuiUtils.TextMiddle("Oops! Cannot open file. An error occured."));
            continue;
          }
          // does not exist in the last file
          if (_settings.LastFiles.Find(item => item.Filename == recent.Filename) == null)
          {
            Core.StartSceneTransition(new FadeTransition(()=>new EditorScene(new Editor(content.Item1, content.Item2))));
          }
          else _settings.LastFile = i;
          
          new Serializers.SettingsSerializer().Save(Serializer.ApplicationSavePath, _settings);

          var editor = new EditorScene();
          Core.StartSceneTransition(new FadeTransition(()=>editor));
        }
        i++;
      }
      for (i = invalidFiles.Count()-1; i >= 0; i--) 
      {
        _settings.FileHistory.RemoveAt(invalidFiles[i]); 
      }
      new Serializers.SettingsSerializer().Save(Serializer.ApplicationSavePath, _settings);

    }
    void RenderImGui()
    {
      _dialog.Render(_imgui);
      RenderMenubar();

      ImGui.Begin("File Manager", ImGuiWindowFlags.NoTitleBar);


      RenderIcon();

      RenderFiles();
     
      ImGui.End();

      _imgui.Render();
    }
    void RenderIcon()
    {
      var texture = Core.GetGlobalManager<Nez.ImGuiTools.ImGuiManager>().BindTexture(_appLogo);
      var size = ImGui.GetContentRegionAvail();
      size.Y *= 0.2f;
      var imageSize = ImGuiUtils.ContainSize(_appLogo.GetSize().ToNumerics(), size);
      ImGui.Image(texture, imageSize, new System.Numerics.Vector2(0, 0), new System.Numerics.Vector2(1, 1), 
          tint_col: new System.Numerics.Vector4(0.7f, 0.7f, 0.7f, 0.7f));
    }

    public override void Update()
    {
      base.Update();
    }
      
  }
}
