
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
    public override void OnAddedToScene()
    {
      Core.GetGlobalManager<ImGuiManager>().RegisterDrawCommand(RenderImGui);
      _buttons = new (string, Action)[][]{
        // 0, main menu buttons
        new (string, Action)[]
        {
          ("Project", ()=>
           {

           }),
            ("World", ()=>
             {

             }),
            ("Sheet", ()=> 
             {
             if (ImGui.MenuItem("New Sheet"))
             {
              Editor.OpenFilePicker((filename)=>{ Editor.AddTab(new Sheet(filename)); });
             }
             }),
            ("View", ()=>
             {

             }),
        },
          // 1, operationButton
          new (string, Action)[]
          {
            (IconFonts.FontAwesome5.MousePointer, ()=>
             {

             }),
            (IconFonts.FontAwesome5.ArrowsAlt, ()=>
             {

             }),
            (IconFonts.FontAwesome5.HandSpock, ()=>
             {

             }),
            (IconFonts.FontAwesome5.Expand, ()=>
             {

             }),
            (IconFonts.FontAwesome5.SyncAlt, ()=>
             {

             }),
          },
          // 2, view options
          new (string, Action)[]
          {
            (IconFonts.FontAwesome5.Th, ()=>
             {

             }),
            (IconFonts.FontAwesome5.EllipsisV, ()=>
             {

             }),
          },
          // 3, select type options
          new (string, Action)[]
          {
            (IconFonts.FontAwesome5.MousePointer, ()=>
             {

             }),
            (IconFonts.FontAwesome5.Shapes, ()=>
             {

             })
          }
      };

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
          if (Editor.GetCurrent().Name != Editor._tabs[i].Name) Editor.Switch(i);
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
      ButtonSetFlat(2, 10);

      // Select type options
      ButtonSetFlat(3, 20);
    
      // Geometry opeionts
      ImGui.PushStyleColor(ImGuiCol.Text, Editor.ColorSet.ShapeButtonTextColor.ToImColor());
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





      ImGui.End();
    }
  }
}
