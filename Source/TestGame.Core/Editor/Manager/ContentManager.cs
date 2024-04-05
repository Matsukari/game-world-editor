using Nez;

namespace Raven
{
	  public class ContentManager 
  {
    public bool HasContent { get => _tabs.Count() > 0; }
    public ContentView View { get => _views[_currentTab]; }
    public IPropertied Content { get => _tabs[_currentTab].Content; }
    public EditorContentData ContentData { get => _tabs[_currentTab].Data; }
    public int CurrentIndex { get => _currentTab; }

    public EditorSettings Settings;
    public event Action OnBeforeSwitch;
    public event Action OnAfterSwitch;
    public event Action OnAddContent;

    SheetView sheetView;
    WorldView worldView;

    // Either world or sheet; these are the objects that can be switch in and out
    internal List<EditorContent> _tabs = new List<EditorContent>();
    internal List<ContentView> _views = new List<ContentView>();
    int _currentTab = 0;

    public void Render(Batcher batcher, Camera camera) => View.Render(batcher, camera, Settings);
    public void Update()
    {
      if (HasContent)
      {
        Settings.LastFiles[_currentTab].Filename = Content.Name;
        ContentData.Filename = Content.Name;
      }
    }
    public void Switch(int index) 
    {
      Console.WriteLine($"Switched to {index}");

      OnBeforeSwitch();
      if (HasContent) 
        View.OnContentClose();
      _currentTab = Math.Clamp(index, 0, _tabs.Count()-1);
      View.OnContentOpen(Content);
      OnAfterSwitch();

      Settings.LastFile = index;
    }
    public EditorContent GetContent() => _tabs[_currentTab];

    public void AddTab(ContentView contentView, IPropertied content, bool isSwitch=true)
    {
      Console.WriteLine("Adding content on tabs");

      var contentData = new EditorContentData(content.Name, content.GetType().Name);
      // This file already exist within the tab files
      if (Settings.LastFiles.Find((file)=>file.Filename == content.Name) != null)
      {
        Console.WriteLine("Cannot add Content. Already exist.");
        return;
      }
      if (!contentView.CanDealWithType(content))
        throw new Exception();

      Settings.LastFiles.Add(contentData);

      _tabs.Add(new EditorContent(content, contentData));
      _views.Add(contentView);

      View.OnInitialize(Settings);

      OnAddContent();

      // First tab in the list yet
      if (_tabs.Count() == 1 || isSwitch) Switch(_tabs.Count()-1);
    }
  }

}