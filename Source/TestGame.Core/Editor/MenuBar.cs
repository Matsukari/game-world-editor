
using ImGuiNET;
using Nez;
using Microsoft.Xna.Framework;
using Icon = IconFonts.FontAwesome5;

namespace Raven
{
  public class Menubar : IImGuiRenderable
  {
    (string, Action)[][] _buttons; 
    readonly Editor _editor;

    void ProjectOptions()
    {
      var type = (_editor.ContentManager.View is WorldView) ? "World" : "Sheet";
      if (ImGui.MenuItem("Project Settings")) _editor.WindowManager.GetRenderable<Settings>().IsOpen = true;
      if (ImGui.MenuItem("Project Dashboard")) {}
      ImGui.Separator();
      if (ImGui.MenuItem($"Save ({type})")) _editor.Serializer.SaveContent();
      if (ImGui.MenuItem("Save as")) _editor.Serializer.SaveContent();
      if (ImGui.MenuItem("Close")) Core.Exit();
    }
    void WorldOptions()
    {
      if (ImGui.MenuItem("New World")) _editor.ContentManager.AddTab(new WorldView(), new World());
      if (ImGui.BeginMenu("Worlds"))
      {
        foreach (var content in _editor.ContentManager._tabs)
        {
          if (content.Content is World world && ImGui.MenuItem(world.Name.BestWrap()))
          {
            _editor.ContentManager.Switch(_editor.ContentManager._tabs.FindIndex(item => item.Content.Name == world.Name));
          }
        }
        ImGui.EndMenu();
      }
    }
    void SheetOptions()
    {
      if (ImGui.MenuItem("New Sheet")) _editor.WindowManager.FilePicker.Open((filename)=> _editor.ContentManager.AddTab(new SheetView(), new Sheet(filename))); 
      if (ImGui.BeginMenu("Sheets"))
      {
        foreach (var content in _editor.ContentManager._tabs)
        {
          if (content.Content is Sheet sheet && ImGui.MenuItem(sheet.Name.BestWrap()))
          {
            _editor.ContentManager.Switch(_editor.ContentManager._tabs.FindIndex(item => item.Content.Name == sheet.Name));
          }
        }
        ImGui.EndMenu();
      }
    }
    void ViewOptions() 
    {
    }
    public Menubar(Editor editor)
    {
      _editor = editor;
      _buttons = new (string, Action)[][]{
        // 0, main menu buttons
        new (string, Action)[]
        {
          ("Project", ProjectOptions),
            ("World", WorldOptions),
            ("Sheet", SheetOptions),
            ("View", ViewOptions),
        },
      };

    }
    bool _isDrawSnappingPopup = false;
    void DrawSnappingPopup()
    {
      if (_isDrawSnappingPopup)
      {
        _isDrawSnappingPopup = false;
        ImGui.OpenPopup("snap-options-popup");
      }
      if (ImGui.BeginPopupContextItem("snap-options-popup"))
      {
        if (ImGui.MenuItem("Snap to grid"))
        {
        }
        if (ImGui.MenuItem("Snap to custom size"))
        {
        }
        ImGui.EndPopup();
      }
    }
    void IImGuiRenderable.Render(Raven.ImGuiWinManager imgui)
    {
      Vector2 stackSize = new Vector2();
      void BeginStackBar(string name, float height)
      {
        ImGui.Begin(name, ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoBringToFrontOnFocus | ImGuiWindowFlags.NoDocking);
        var position = new Vector2(0f, stackSize.Y-1);
        var size = new Vector2(Screen.Width, height);
        ImGui.SetWindowPos(position.ToNumerics());
        ImGui.SetWindowSize(size.ToNumerics());
        stackSize.Y += size.Y;
      }

      // Main menubar; topmost horizontal bar
      if (ImGui.BeginMainMenuBar())
      {
        stackSize = ImGui.GetWindowSize();
        foreach (var menubarButton in _buttons[0]) 
        {
          if (ImGui.BeginMenu(menubarButton.Item1)) 
          {
            menubarButton.Item2.Invoke();
            ImGui.EndMenu();
          }
        }
        ImGui.EndMainMenuBar();
      }


      // // Start of file tabs
      BeginStackBar("files-bar", 27);

      ImGui.Dummy(new System.Numerics.Vector2(250f, 0));
      ImGui.SameLine();
      ImGui.BeginTabBar("files-tabs");
      var tabs = _editor.ContentManager._tabs;
      for (int i = 0; i < tabs.Count(); i++)
      {
        var open = true;
        if (ImGui.BeginTabItem(tabs[i].Content.Name, ref open))
        {
          if (_editor.ContentManager.Content.Name != tabs[i].Content.Name) _editor.ContentManager.Switch(i);
          ImGui.EndTabItem();
        }
        if (!open)
        {

        }
      }
      ImGui.EndTabBar();

      // Start of tools bar
      BeginStackBar("tools-bar", 37);

      ImGuiUtils.SpanX(270);
      Widget.ImGuiWidget.ToggleButtonGroup(
          ids: new []{Icon.MousePointer, Icon.ArrowsAlt, Icon.HandSpock, Icon.Expand, Icon.SyncAlt},
          toggles: ref _editorOpToggled,
          actions: new []{
          ()=>{}, 
          ()=>{}, 
          ()=>{}, 
          ()=>{}, 
          ()=>{},           
          },
          fallback: null,
          color: EditorColors.Get(ImGuiCol.ButtonHovered));

      ImGuiUtils.SpanX(10);
      var gridToggle = (_editor.ContentManager.View is WorldView world) ? _editor.Settings.Graphics.DrawLayerGrid : _editor.Settings.Graphics.DrawSheetGrid; 
      if (Widget.ImGuiWidget.ToggleButton(Icon.BorderAll, ref gridToggle))
      {
        if (_editor.ContentManager.View is WorldView) _editor.Settings.Graphics.DrawLayerGrid = !_editor.Settings.Graphics.DrawLayerGrid;
        else _editor.Settings.Graphics.DrawSheetGrid = !_editor.Settings.Graphics.DrawSheetGrid;
      }
      ImGui.SameLine();
      if (ImGui.Button(Icon.EllipsisV)) _isDrawSnappingPopup = !_isDrawSnappingPopup;

      ImGuiUtils.SpanX(20);
      Widget.ImGuiWidget.ToggleButtonGroup(
          ids: new []{Icon.User, Icon.Shapes},
          toggles: ref _selTypeToggled,
          actions: new []{
          ()=>{}, 
          ()=>{}, 
          },
          fallback: null,
          color: EditorColors.Get(ImGuiCol.ButtonHovered));

      // Geometry opeionts
      ImGuiUtils.SpanX(20);
      foreach (var shapeModel in _shapeModels)
      {
        ImGui.SameLine();
        var shapeInstance = shapeModel;
        var icon = shapeModel.Icon;

        // pressed; begin annotation
        if (ImGui.Button(icon)) _editor.ShapeAnnotator.Annotate(_editor.ContentManager.ContentData.PropertiedContext, shapeInstance);
      }


      ImGuiUtils.SpanX(20);
      if (_editor.ContentManager.View is WorldView worldView)
      {
        Widget.ImGuiWidget.ToggleButtonGroup(
            ids: new []{Icon.PaintBrush, Icon.Eraser},
            toggles: ref _paintModeToggled,
            actions: new []{
              ()=>{worldView.PaintMode = PaintMode.Pen; }, 
              ()=>{worldView.PaintMode = PaintMode.Eraser; }, 
            },
            fallback: null,
            color: EditorColors.Get(ImGuiCol.ButtonHovered));


        Widget.ImGuiWidget.ToggleButtonGroup(
            ids: new []{"/", Icon.Square, Icon.Fill},
            toggles: ref _paintTypeToggled,
            actions: new []{
              ()=>{worldView.PaintType = PaintType.Line; }, 
              ()=>{worldView.PaintType = PaintType.Rectangle; }, 
              ()=>{worldView.PaintType = PaintType.Fill; }, 
            },
            fallback: ()=>{worldView.PaintType = PaintType.Single;},
            color: EditorColors.Get(ImGuiCol.ButtonHovered));

        ImGui.SameLine();

        if (Widget.ImGuiWidget.ToggleButton(Icon.Dice, ref _isRandomPaint))
        {
          worldView.IsRandomPaint = !worldView.IsRandomPaint;
        }
      }

      DrawSnappingPopup();

      ImGui.End();
    }
    ShapeModel[] _shapeModels = new ShapeModel[]{new RectangleModel(), new EllipseModel(), new PointModel(), new PolygonModel()};
    bool[] _editorOpToggled = new []{false, true, false, false, false};
    bool[] _selTypeToggled = new []{false, false};
    bool[] _paintModeToggled = new []{true, false};
    bool[] _paintTypeToggled = new []{false, false, false};
    bool _isRandomPaint = false;
  }
}
