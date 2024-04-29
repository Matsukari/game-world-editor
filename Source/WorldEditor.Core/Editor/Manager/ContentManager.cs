using Nez;

namespace Raven
{
  public class ContentRenderer : RenderableComponent 
  {
    readonly ContentManager _contentManager;
    public ContentRenderer(ContentManager contentManager) => _contentManager = contentManager;
    public override bool IsVisibleFromCamera(Camera camera) => true;
    public override void Render(Batcher batcher, Camera camera)
    {
      if (_contentManager.View != null)
        _contentManager.View.Render(batcher, camera, _contentManager.Settings);
    }
  }
  public class ContentManager 
  {
    public bool HasContent { get => _tabs.Count() > 0; }
    public ContentView View { get => _views.GetAtOrNull(_currentTab); }
    public IPropertied Content { get => _tabs.GetAtOrNull(_currentTab).Content; }
    public EditorContentData ContentData { get => _tabs.GetAtOrNull(_currentTab).Data; }
    public int CurrentIndex { get => _currentTab; }

    public EditorSettings Settings;
    public event Action<EditorContent, ContentView> OnCloseContent;
    public event Action<EditorContent, ContentView> OnOpenContent;
    public event Action<EditorContent, ContentView> OnAddContent;

    // Either world or sheet; these are the objects that can be switch in and out
    internal List<EditorContent> _tabs = new List<EditorContent>();
    internal List<ContentView> _views = new List<ContentView>();
    int _currentTab = 0;

    public ContentManager(EditorSettings settings) => Settings = settings;

    public void Update()
    {
      if (HasContent)
      {
        Settings.LastFiles[_currentTab].Filename = Content.Name;
        ContentData.Filename = Content.Name;
      }
    }
    public void Switch(int index, bool force=true) 
    {
      // Console.WriteLine($"Switched to {index}");

      OnCloseContent(_tabs[_currentTab], _views[_currentTab]);
      View.OnContentClose();
      _currentTab = Math.Clamp(index, 0, _tabs.Count()-1);
      View.OnContentOpen(Content);
      OnOpenContent(_tabs[_currentTab], _views[_currentTab]);

    }
    public void OpenTab(int index)
    {
      if (index < 0 || index >= _tabs.Count()) return;
      // Console.WriteLine("Opened " + index);
      _views[index].OnContentOpen(_views[index].Content);
      OnOpenContent(_tabs[index], _views[index]);
      Settings.LastFile = index;
      _currentTab = index;
    }
    public void CloseTab(int index)
    {
      if (index < 0 || index >= _tabs.Count()) return;
      // Console.WriteLine("Closed " + index);
      OnCloseContent(_tabs[index], _views[index]);
      _views[index].OnContentClose();
    }
    public EditorContent GetContent() => _tabs[_currentTab];

    public void RemoveTab(int index)
    {

      CloseTab(index);

      // Console.WriteLine("Removing " + index);
      _tabs.RemoveAt(index);
      _views.RemoveAt(index);
      Settings.LastFiles.RemoveAt(index);

      var newTab = _currentTab;
      if (_currentTab >= index) newTab--;
      if (newTab < 0) newTab = 0;

      OpenTab(newTab);

    }
    public void AddTab(ContentView contentView, IPropertied content, bool isSwitch=false, bool forceAdd=true)
    {
      if (!contentView.CanDealWithType(content))
        throw new Exception();

      var contentData = new EditorContentData(content.Name, content.GetType().Name);

      // This file already exist within the tab files
      if (Settings.LastFiles.Find((file)=>file.Filename == content.Name) != null)
      {
        if (forceAdd)
        {
          content.Name = content.Name.EnsureNoRepeat(); 
          AddTab(contentView, content, isSwitch, forceAdd);
          return;
        }
        else 
        {
          Console.WriteLine("Cannot add Content. Already exist.");
          return;
        }
      }

      Console.WriteLine("Adding content on tabs");

      Settings.LastFiles.Add(contentData);

      contentData.PropertiedContext = content;

      var tab = new EditorContent(content, contentData);
      _tabs.Add(tab);
      _views.Add(contentView);

      if (OnAddContent != null) OnAddContent(tab, contentView);

      View.OnInitialize(Settings);

    }
  }

}
