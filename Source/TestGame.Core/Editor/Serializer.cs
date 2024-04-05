using Raven.Serializers;

namespace Raven
{
  public class Serializer 
  {
    public static string[] SheetStdExtensions = new [] {".rvsheet", ".rv"};
    public static string[] WorldStdExtensions = new [] {".rvworld", ".rvw"};
    public string ApplicationSaveFolder;
    public string ApplicationSaveFilename;
    public string ApplicationSavePath { get => ApplicationSaveFolder + "/" + ApplicationSaveFilename; }
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

      foreach (var file in loadedSettings.LastFiles)
      {
             if (file.Type == "Sheet") _contentManager.AddTab(new SheetView(), LoadContent<Sheet>(file.Filename));
        else if (file.Type == "World") _contentManager.AddTab(new WorldView(), LoadContent<World>(file.Filename));
        else throw new Exception($"Error in file metadata. Cannot load {file.Type} content");
      }
      _contentManager.Settings.Colors = loadedSettings.Colors;
    }
    public EditorSettings LoadSettings()
    {
      Console.WriteLine($"Loading {ApplicationSavePath}");
      return new SettingsSerializer().Load(ApplicationSavePath);
    }
    public void SaveSettings()
    {
      Console.WriteLine($"Saving at {ApplicationSavePath}");
      new SettingsSerializer().Save(ApplicationSavePath, _contentManager.Settings);
    }
    public T LoadContent<T>(string file) where T: class
    {
      if (typeof(T) == typeof(Sheet)) return new SheetSerializer().Load(file) as T;
      else 
      {
        throw new Exception($"Cannot load type {typeof(T).Name}");
      }
    }
    public void SaveContent()
    {
      var content = _contentManager.GetContent().Content;
      var filepath = _contentManager.GetContent().Data.Filename;
      if (filepath == null) filepath = "Sample.content";
      if (content is Sheet sheet) new SheetSerializer().Save(filepath, sheet);
      else 
      {
        throw new Exception($"Cannot load type {typeof(Sheet).Name}");
      }
    }
  }
}
