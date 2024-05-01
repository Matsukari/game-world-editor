
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

    void SaveFile()
    {
      _editor.WindowManager.FilePicker.Open(file => 
      {
        _editor.Serializer.SaveContent(file);
      }, "Save file");
    }
    void ProjectOptions()
    {
      var type = (_editor.ContentManager.View is WorldView) ? "World" : "Sheet";
      if (ImGui.MenuItem("Project Settings")) _editor.WindowManager.GetRenderable<Settings>().IsOpen = true;
      if (ImGui.MenuItem("Project Dashboard")) Core.StartSceneTransition(new FadeTransition(()=>new DashboardScene()));
      if (ImGui.MenuItem("Save")) SaveFile();
      if (ImGui.MenuItem("Save As...")) SaveFile();
      if (ImGui.MenuItem("Close File")) _editor.ContentManager.RemoveTab(_editor.ContentManager.CurrentIndex);
      if (ImGui.MenuItem("Quit")) Core.Exit();
    }
    void WorldOptions()
    {
      if (ImGui.MenuItem("New World")) _editor.ContentManager.AddTab(new WorldView(), new World());
      if (ImGui.MenuItem("Open World")) 
        _editor.WindowManager.FilePicker.Open(path=>_editor.ContentManager.AddTab(new WorldView(), Serializer.LoadContent<World>(path)), "Open Sheet"); 
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
      if (ImGui.MenuItem("New Sheet")) 
        _editor.WindowManager.FilePicker.Open((filename)=> _editor.ContentManager.AddTab(new SheetView(), new Sheet(filename)), "Open Sheet"); 

      if (ImGui.MenuItem("Open World")) 
        _editor.WindowManager.FilePicker.Open(path=>_editor.ContentManager.AddTab(new SheetView(), Serializer.LoadContent<Sheet>(path)), "Open Sheet"); 

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

        ImGui.SeparatorText("Snap to custom size");
        {
          ImGui.InputInt("Width", ref _snapWidth);
          ImGui.InputInt("Height", ref _snapHeight); 
          if (ImGui.Button("Ok")) 
          {
            InputManager.MouseSnapSize.X = _snapWidth;
            InputManager.MouseSnapSize.Y = _snapHeight;
          }
        }
        ImGui.EndPopup();
      }
    }
    int _snapWidth = 1;
    int _snapHeight = 1;
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
        ImGuiUtils.SpanX(40);
        if (ImGui.Button(Icon.Undo) || _editor.Settings.Hotkeys.Undo.IsPressed())
        {
          Core.GetGlobalManager<CommandManagerHead>().Current.Undo();
        }
        if (ImGui.Button(Icon.Redo) || _editor.Settings.Hotkeys.Redo.IsPressed())
        {
          Core.GetGlobalManager<CommandManagerHead>().Current.Redo();
        }

        ImGui.EndMainMenuBar();
      }


      // // Start of file tabs
      BeginStackBar("files-bar", 27);

      ImGui.Dummy(new System.Numerics.Vector2(250f, 0));
      ImGui.SameLine();
      ImGui.BeginTabBar("files-tabs");
      var tabs = _editor.ContentManager._tabs;
      var remove = -1;
      for (int i = 0; i < tabs.Count(); i++)
      {
        var open = true;

        ImGui.PushStyleColor(ImGuiCol.Text, (i == _editor.ContentManager.CurrentIndex) ? EditorColors.Get(ImGuiCol.Text) : EditorColors.Get(ImGuiCol.TextDisabled));
        if (i == _editor.Settings.LastFile && _once)
        {
          Console.WriteLine("Open at: " + i.ToString());
          ImGui.SetNextItemOpen(true);
          for (int j = 0; j < tabs.Count(); j++)
          {
            if (j != i)
              ImGui.SetTabItemClosed(tabs[j].Content.Name.BestWrap());
          }
          _once = false;
        }
        if (ImGui.BeginTabItem(tabs[i].Content.Name.BestWrap(), ref open))
        {
          var bottomLeft = new System.Numerics.Vector2();
          bottomLeft.X = ImGui.GetItemRectMin().X;
          bottomLeft.Y = ImGui.GetItemRectMax().Y;
          ImGui.GetWindowDrawList().AddRect(bottomLeft, 
              bottomLeft + new System.Numerics.Vector2(ImGui.GetItemRectSize().X, 2), _editor.Settings.Colors.Accent.ToImColor());
          if (_editor.ContentManager.Content.Name != tabs[i].Content.Name) _editor.ContentManager.Switch(i);
          ImGui.EndTabItem();
        }
        ImGui.PopStyleColor();
        if (!open)
        {
          remove = i;
        }
      }
      if (remove != -1)
        _editor.ContentManager.RemoveTab(remove);
      ImGui.EndTabBar();

      // Start of tools bar
      ImGui.PushStyleColor(ImGuiCol.WindowBg, _editor.Settings.Colors.ToolbarBg.ToImColor());
      BeginStackBar("tools-bar", 37);
      ImGui.PopStyleColor();

      ImGuiUtils.SpanX(270);
      Widget.ImGuiWidget.ToggleButtonGroup(
          ids: new []{Icon.MousePointer, Icon.ArrowsAlt, Icon.HandSpock, Icon.Expand, Icon.SyncAlt},
          toggles: ref _editorOpToggled,
          actions: new Action[]{
          ()=>_editor.Operator = EditorOperator.Select,
          ()=>_editor.Operator = EditorOperator.MoveOnly, 
          ()=>_editor.Operator = EditorOperator.HandPanner, 
          ()=>_editor.Operator = EditorOperator.Scaler, 
          ()=>_editor.Operator = EditorOperator.Rotator,           
          },
          fallback: null,
          color: EditorColors.Get(ImGuiCol.ButtonActive));

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
          color: EditorColors.Get(ImGuiCol.ButtonActive));

      if (_editor.ContentManager.View is WorldView|| (_editor.ContentManager.View is SheetView sheetView && sheetView.SceneView.IsEditing))
      {
        // Geometry opeionts
        ImGuiUtils.SpanX(20);
        foreach (var shapeModel in _shapeModels)
        {
          ImGui.SameLine();
          var shapeInstance = shapeModel;
          var icon = shapeModel.Icon;
          IPropertied context = (_editor.ContentManager.View is WorldView w) ? w.World : (_editor.ContentManager.View as SheetView).SceneView.SpriteScene;
          if (_editor.Selection.Capture is IPropertied prop) context = prop;

          // pressed; begin annotation
          if (ImGui.Button(icon)) _editor.ShapeAnnotator.Annotate(context, shapeInstance);
        }
      }

      if (_editor.ContentManager.View is WorldView worldView)
      {
        ImGuiUtils.SpanX(20);
        Widget.ImGuiWidget.ToggleButtonGroup(
            ids: new []{Icon.PaintBrush, Icon.Eraser},
            toggles: ref _paintModeToggled,
            actions: new []{
              ()=>{worldView.PaintMode = PaintMode.Pen; }, 
              ()=>{worldView.PaintMode = PaintMode.Eraser; }, 
            },
            fallback: null,
            color: EditorColors.Get(ImGuiCol.ButtonActive));


        Widget.ImGuiWidget.ToggleButtonGroup(
            ids: new []{"/", Icon.Square, Icon.Fill},
            toggles: ref _paintTypeToggled,
            actions: new []{
              ()=>{worldView.PaintType = PaintType.Line; }, 
              ()=>{worldView.PaintType = PaintType.Rectangle; }, 
              ()=>{worldView.PaintType = PaintType.Fill; }, 
            },
            fallback: ()=>{worldView.PaintType = PaintType.Single;},
            color: EditorColors.Get(ImGuiCol.ButtonActive));

        ImGui.SameLine();

        if (Widget.ImGuiWidget.ToggleButton(Icon.Dice, ref _isRandomPaint))
        {
          worldView.IsRandomPaint = !worldView.IsRandomPaint;
        }

        ImGuiUtils.SpanX(20);
        if (ImGui.Button(Icon.Play))
        {
          var scene = new WorldScene(Core.Scene, worldView.World, _editor.Settings.Colors.WorldBackground.ToColor());
          Core.StartSceneTransition(new CrossFadeTransition(()=>scene));
        }
      }


      DrawSnappingPopup();


      ImGui.End();
    }
    bool _once = true;
    ShapeModel[] _shapeModels = new ShapeModel[]{new RectangleModel(), new EllipseModel(), new PointModel(), new PolygonModel()};
    bool[] _editorOpToggled = new []{true, false, false, false, false};
    bool[] _selTypeToggled = new []{false, false};
    bool[] _paintModeToggled = new []{true, false};
    bool[] _paintTypeToggled = new []{false, false, false};
    bool _isRandomPaint = false;
  }
}
