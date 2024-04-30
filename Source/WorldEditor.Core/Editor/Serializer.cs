using ImGuiNET;
using Raven.Serializers;

namespace Raven
{
  public class Serializer 
  {
    public static string[] SheetStdExtensions = new [] {".rvsheet", ".rv"};
    public static string[] WorldStdExtensions = new [] {".rvworld", ".rvw"};
    public static string ApplicationSaveFolder = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
    public static string ApplicationSaveFilename = "moneditor.app-settings";
    public static string ApplicationSavePath { get => ApplicationSaveFolder + "/" + ApplicationSaveFilename; }
    readonly ContentManager _contentManager;

    public Serializer(ContentManager contentManager)
    {
      _contentManager = contentManager;
      ApplicationSaveFolder = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
      ApplicationSaveFilename = "moneditor.app-settings";
    }

    public void LoadStartup()
    {
      if (!File.Exists(ApplicationSavePath))
      {
        return;
      }
      var loadedSettings = LoadSettings();

      JsonCache.Data.Clear();

      List<int> invalidFiles = new List<int>();
      for (int i = 0; i < loadedSettings.LastFiles.Count(); i++)
      {
        var file = loadedSettings.LastFiles[i];
        try 
        {
          if (file.Type == "Sheet") _contentManager.AddTab(new SheetView(), LoadContent<Sheet>(file.Filename));
          else if (file.Type == "World") _contentManager.AddTab(new WorldView(), LoadContent<World>(file.Filename));
          else throw new Exception($"Error in file metadata. Cannot load {file.Type} content");
        } 
        catch (FileNotFoundException) 
        { 
          invalidFiles.Add(i);
          Console.WriteLine("Err; does not exist; ignoring, " + i);
          continue; 
        }
        catch (NotSupportedException) 
        { 
          invalidFiles.Add(i);
          Console.WriteLine("Err; does not support type, " + i);
          continue; 
        }
      }
      for (int i = invalidFiles.Count()-1; i >= 0; i--) 
      {
        loadedSettings.LastFiles.RemoveAt(invalidFiles[i]); 
      }


      new SettingsSerializer().Save(ApplicationSavePath, loadedSettings);

      _contentManager.Settings.Colors = loadedSettings.Colors;
      _contentManager.Settings.Graphics = loadedSettings.Graphics;
      _contentManager.Settings.Hotkeys = loadedSettings.Hotkeys;
      _contentManager.Settings.ImGuiColors = loadedSettings.ImGuiColors;
      _contentManager.Settings.LastFile = loadedSettings.LastFile;

      // _contentManager.Settings = loadedSettings;
      if (_contentManager.Settings.ImGuiColors.Count != ImGui.GetStyle().Colors.Count)
      {
        Console.WriteLine("First time; default theme used");
        for (int i = 0; i < ImGui.GetStyle().Colors.Count; i++)
          _contentManager.Settings.ImGuiColors.Add(ImGui.GetStyle().Colors[i]);
      }

      _contentManager.Settings.ApplyImGui();
      
    }
    public static EditorSettings LoadSettings()
    {
      Console.WriteLine($"Loading {ApplicationSavePath}");
      var settings = new SettingsSerializer().Load(ApplicationSavePath);

      List<int> invalidFiles = new List<int>();
      for (int i = 0; i < settings.LastFiles.Count(); i++)
      {
        var file = settings.LastFiles[i];
        if (Path.Exists(file.Filename) || (file.Type != "Sheet" || file.Type != "World"))
        { 
          invalidFiles.Add(i);
          Console.WriteLine("Err; does not exist; ignoring, " + i);
        }
      }
      for (int i = invalidFiles.Count()-1; i >= 0; i--) 
      {
        settings.LastFiles.RemoveAt(invalidFiles[i]); 
      }
      new SettingsSerializer().Save(ApplicationSavePath, settings);
      return settings;
    }
    public void SaveSettings()
    {
      Console.WriteLine($"Saving at {ApplicationSavePath}");
      _contentManager.Settings.LastFile = _contentManager.CurrentIndex;

      foreach (var file in _contentManager._tabs)
      {
        if (_contentManager.Settings.FileHistory.Find(item => item.Filename == file.Content.Name) == null)
          _contentManager.Settings.FileHistory.Add(file.Data.Copy());
      }
      new SettingsSerializer().Save(ApplicationSavePath, _contentManager.Settings);
    }
    public static T LoadContent<T>(string file) where T: class
    {
      Console.WriteLine($"Loading content {file}");
      if (!Path.Exists(file)) 
      {
        throw new FileNotFoundException();
      }
      if (typeof(T) == typeof(Sheet)) return new SheetSerializer().Load(file) as T;
      else if (typeof(T) == typeof(World)) return new WorldSerializer().Load(file) as T;
      else 
      {
        throw new NotSupportedException($"Cannot load type {typeof(T).Name}");
      }

    }
    public void SaveContent(string filepath)
    {
      var content = _contentManager.GetContent().Content;
      if (filepath == null) filepath = "Sample.content";
      if (content is Sheet sheet) new SheetSerializer().Save(filepath, sheet);
      else if (content is World world) new WorldSerializer().Save(filepath, world);
      else 
      {
        throw new NotSupportedException($"Cannot load type {typeof(Sheet).Name}");
      }
    }
    public void SaveContent() => SaveContent(_contentManager.GetContent().Data.Filename);
  }
}
