
using ImGuiNET;
using Nez;
using Nez.ImGuiTools;
using Microsoft.Xna.Framework;
using System.Reflection;

namespace Raven.Sheet
{
  public class ViewMenubar : Editor.SubEntity
  {
    (string, Action)[][] _buttons; 

    void ProjectOptions()
    {
      if (ImGui.MenuItem("Save")) Editor.Save();
      if (ImGui.MenuItem("Project Settings")) Editor.OpenProjectSettings();
    }
    void WorldOptions()
    {
      if (ImGui.MenuItem("New World")) Editor.OpenNameModal((name)=>Editor.AddTab(new World()));
      if (ImGui.BeginMenu("Worlds"))
      {
        ImGui.EndMenu();
      }
    }
    void SheetOptions()
    {
      if (ImGui.MenuItem("New Sheet")) Editor.OpenFilePicker((filename)=> Editor.AddTab(new Sheet(filename))); 
      if (ImGui.BeginMenu("Sheets"))
      {
        ImGui.EndMenu();
      }
    }
    void ViewOptions() 
    {
      Editor.GetComponent<Settings>().Enabled = true;
    }
    public override void OnAddedToScene()
    {
      Core.GetGlobalManager<ImGuiManager>().RegisterDrawCommand(RenderImGui);
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
            (IconFonts.FontAwesome5.Th, ToogleGrid),
            (IconFonts.FontAwesome5.EllipsisV, ()=>_isDrawSnappingPopup = !_isDrawSnappingPopup),
          },
          // 3, select type options
          new (string, Action)[]
          {
            (IconFonts.FontAwesome5.MousePointer, ()=>{}),
            (IconFonts.FontAwesome5.Shapes, ()=>{})
          },
          // 4, paint type options
          new (string, Action)[]
          {
            (IconFonts.FontAwesome5.PaintBrush, ()=>Editor.GetCurrentGui<WorldGui>().PaintMode = PaintMode.Pen),
            (IconFonts.FontAwesome5.Eraser, ()=>Editor.GetCurrentGui<WorldGui>().PaintMode = PaintMode.Eraser),
            ("/",                           ()=>Editor.GetCurrentGui<WorldGui>().PaintType = PaintType.Line),
            (IconFonts.FontAwesome5.Square, ()=>Editor.GetCurrentGui<WorldGui>().PaintType = PaintType.Rectangle),
            (IconFonts.FontAwesome5.Fill,   ()=>Editor.GetCurrentGui<WorldGui>().PaintType = PaintType.Fill),
            (IconFonts.FontAwesome5.Dice,   ()=>Editor.GetCurrentGui<WorldGui>().IsRandomPaint = !Editor.GetCurrentGui<WorldGui>().IsRandomPaint),            
          }
      };

    }
    bool _isDrawSnappingPopup = false;
    void ToogleGrid()
    {
      if (Editor.GetCurrent() is Sheet sheet) Editor.GetSubEntity<SheetView>().IsDrawGrid = !Editor.GetSubEntity<SheetView>().IsDrawGrid;
      else if (Editor.GetCurrentGui() is WorldGui worldGui) worldGui.IsDrawTileLayerGrid = !worldGui.IsDrawTileLayerGrid;
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
    void RenderImGui()
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
        if (ImGui.BeginTabItem(Editor._tabs[i].Name))
        {
          if (Editor.GetCurrentGui().Name != Editor._tabs[i].Name) Editor.Switch(i);
          ImGui.EndTabItem();
        }
      }
      ImGui.EndTabBar();



      // Start of tools bar
      BeginStackBar("tools-bar", 37);

      // Operation buttosn
      ImGui.PushStyleColor(ImGuiCol.Text, Editor.ColorSet.ViewbarSpecialButton.ToImColor());
      ButtonSetFlat(1, 270);
      ImGui.PopStyleColor();

      // View options
      ImGui.PushStyleColor(ImGuiCol.Text, Editor.ColorSet.ViewbarViewButton.ToImColor());
      ButtonSetFlat(2, 10);
      ImGui.PopStyleColor();

      // Select type options
      ButtonSetFlat(3, 20);
    
      // Geometry opeionts
      ImGui.PushStyleColor(ImGuiCol.Text, Editor.ColorSet.ViewbarShapeButton.ToImColor());
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
        if (ImGui.Button(label)) Editor.GetSubEntity<Annotator>().Annotate(shapeInstance);
      }
      ImGui.PopStyleColor();

      if (Editor.GetCurrent() is World)
      {
        // Paint options
        ImGui.PushStyleColor(ImGuiCol.Button, ImGui.GetStyle().Colors[(int)ImGuiCol.WindowBg]);
        ButtonSetFlat(4, 20);
        ImGui.PopStyleColor();
      }

      DrawSnappingPopup();

      ImGui.End();
    }
  }
}
