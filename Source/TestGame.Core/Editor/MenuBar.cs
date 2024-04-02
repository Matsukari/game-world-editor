
using ImGuiNET;
using Nez;
using Microsoft.Xna.Framework;
using System.Reflection;

namespace Raven
{
  public class Menubar : EditorComponent, IImGuiRenderable
  {
    (string, Action)[][] _buttons; 

    void ProjectOptions()
    {
      if (ImGui.MenuItem("Save")) Editor.Component<Serializer>().SaveContent();
      if (ImGui.MenuItem("Project Settings")) Editor.Component<Settings>().Enabled = true;
    }
    void WorldOptions()
    {
      if (ImGui.MenuItem("New World")) Editor.AddTab(new World());
      if (ImGui.BeginMenu("Worlds"))
      {
        ImGui.EndMenu();
      }
    }
    void SheetOptions()
    {
      if (ImGui.MenuItem("New Sheet")) Editor.FilePicker.Open((filename)=> Editor.AddTab(new Sheet(filename))); 
      if (ImGui.BeginMenu("Sheets"))
      {
        ImGui.EndMenu();
      }
    }
    void ViewOptions() 
    {
    }
    public override void OnAddedToEntity()
    {
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
            (IconFonts.FontAwesome5.MousePointer, ()=>{}),
            (IconFonts.FontAwesome5.ArrowsAlt, ()=>{}),
            (IconFonts.FontAwesome5.HandSpock, ()=>{}),
            (IconFonts.FontAwesome5.Expand, ()=>{}),
            (IconFonts.FontAwesome5.SyncAlt, ()=>{}),
          },
          // 2, view options
          new (string, Action)[]
          {
            (IconFonts.FontAwesome5.BorderAll, ToogleGrid),
            (IconFonts.FontAwesome5.EllipsisV, ()=>_isDrawSnappingPopup = !_isDrawSnappingPopup),
          },
          // 3, select type options
          new (string, Action)[]
          {
            (IconFonts.FontAwesome5.User, ()=>{}),
            (IconFonts.FontAwesome5.Shapes, ()=>{})
          },
          // 4, paint type options
          new (string, Action)[]
          {
            (IconFonts.FontAwesome5.PaintBrush, ()=>Editor.GetEditorComponent<WorldEditor>().PaintMode = PaintMode.Pen),
            (IconFonts.FontAwesome5.Eraser, ()=>Editor.GetEditorComponent<WorldEditor>().PaintMode = PaintMode.Eraser),
            ("/",                           ()=>Editor.GetEditorComponent<WorldEditor>().PaintType = PaintType.Line),
            (IconFonts.FontAwesome5.Square, ()=>Editor.GetEditorComponent<WorldEditor>().PaintType = PaintType.Rectangle),
            (IconFonts.FontAwesome5.Fill,   ()=>Editor.GetEditorComponent<WorldEditor>().PaintType = PaintType.Fill),
            (IconFonts.FontAwesome5.Dice,   ()=>Editor.GetEditorComponent<WorldEditor>().IsRandomPaint = !Editor.GetEditorComponent<WorldEditor>().IsRandomPaint),            
          }
      };

    }
    bool _isDrawSnappingPopup = false;
    void ToogleGrid()
    {
      if (Content is Sheet sheet) Editor.GetEditorComponent<SheetView>().IsDrawGrid = !Editor.GetEditorComponent<SheetView>().IsDrawGrid;
      else if (Editor.GetEditorComponent<WorldEditor>().Enabled) 
        Editor.GetEditorComponent<WorldEditor>().IsDrawTileLayerGrid = !Editor.GetEditorComponent<WorldEditor>().IsDrawTileLayerGrid;
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
    public void Render(Editor editor)
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
      for (int i = 0; i < Editor._tabs.Count(); i++)
      {
        var open = true;
        if (ImGui.BeginTabItem(Editor._tabs[i].Content.Name, ref open))
        {
          if (Editor.GetContent().Content.Name != Editor._tabs[i].Content.Name) Editor.Switch(i);
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
      foreach (var shapeType in typeof(Shape).GetNestedTypes())
      {
        ImGui.SameLine();
        var shapeInstance = System.Convert.ChangeType(System.Activator.CreateInstance(shapeType), shapeType) as Shape;
        var icon = shapeType.GetField("Icon", BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly).GetValue(shapeInstance);
        string label = (icon == null) ? shapeType.Name : icon as string;

        // pressed; begin annotation
        if (ImGui.Button(label)) Editor.GetEditorComponent<ShapeAnnotator>().Annotate(shapeInstance);
      }

      if (Content is World)
      {
        // Paint options
        ButtonSetFlat(4, 20);
      }

      DrawSnappingPopup();

      ImGui.End();
    }
  }
}
