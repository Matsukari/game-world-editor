
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
      if (ImGui.MenuItem("Save")) _editor.Serializer.SaveContent();
      if (ImGui.MenuItem("Project Settings")) _editor.WindowManager.GetRenderable<Settings>().IsOpen = true;
    }
    void WorldOptions()
    {
      if (ImGui.MenuItem("New World")) _editor.ContentManager.AddTab(new WorldView(), new World());
      if (ImGui.BeginMenu("Worlds"))
      {
        ImGui.EndMenu();
      }
    }
    void SheetOptions()
    {
      if (ImGui.MenuItem("New Sheet")) _editor.WindowManager.FilePicker.Open((filename)=> _editor.ContentManager.AddTab(new SheetView(), new Sheet(filename))); 
      if (ImGui.BeginMenu("Sheets"))
      {
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
          // 1, operationButton
          new (string, Action)[]
          {
            (Icon.MousePointer, ()=>{}),
            (Icon.ArrowsAlt, ()=>{}),
            (Icon.HandSpock, ()=>{}),
            (Icon.Expand, ()=>{}),
            (Icon.SyncAlt, ()=>{}),
          },
          // 2, view options
          new (string, Action)[]
          {
            (Icon.BorderAll, ToogleGrid),
            (Icon.EllipsisV, ()=>_isDrawSnappingPopup = !_isDrawSnappingPopup),
          },
          // 3, select type options
          new (string, Action)[]
          {
            (Icon.User, ()=>{}),
            (Icon.Shapes, ()=>{})
          },
      };

    }
    bool _isDrawSnappingPopup = false;
    void ToogleGrid()
    {
      if (_editor.ContentManager.View is SheetView sheet) sheet.IsDrawGrid = !sheet.IsDrawGrid;
      else if (_editor.ContentManager.View is WorldView world) world.IsDrawTileLayerGrid = !world.IsDrawTileLayerGrid;
    }
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
      void ButtonSetFlat(int set, float span = 10)
      {
        ImGui.SameLine();
        ImGui.Dummy(new System.Numerics.Vector2(span, 0f)); 
        foreach (var button in _buttons[set]) 
        {
          ImGui.SameLine();
          
          if (ImGui.Button(button.Item1)) button.Item2.Invoke();
        }
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

      // Operation buttosn
      ButtonSetFlat(1, 270);

      // View options
      ButtonSetFlat(2, 10);

      // Select type options
      ButtonSetFlat(3, 20);
    
      // Geometry opeionts
      ImGui.SameLine();
      ImGui.Dummy(new System.Numerics.Vector2(20, 0)); 

      // Draw shape annotators
      foreach (var shapeModel in _shapeModels)
      {
        ImGui.SameLine();
        var shapeInstance = shapeModel;
        var icon = shapeModel.Icon;

        // pressed; begin annotation
        if (ImGui.Button(icon)) _editor.ShapeAnnotator.Annotate(_editor.ContentManager.ContentData.PropertiedContext, shapeInstance);
      }


      Widget.ImGuiWidget.SpanX(20);
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
    bool[] _paintModeToggled = new []{false, false};
    bool[] _paintTypeToggled = new []{false, false, false};
    bool _isRandomPaint = false;
  }
}
