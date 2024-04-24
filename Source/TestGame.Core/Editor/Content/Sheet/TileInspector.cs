using ImGuiNET;

namespace Raven
{
  public class SheetObjectInspector : Widget.PropertiedWindow
  {
    public override string Name { get => Inspector.Name; set => Inspector.Name = value;}
    public override PropertyList Properties { get => Inspector.Properties; set => Inspector.Properties = value; }

    public Widget.PropertiedWindow Inspector;
    public override void Render(ImGuiWinManager imgui)
    {
      if (!IsOpen || Inspector == null) return;

      ImGuiManager = imgui;

      var windowname = GetIcon() + "   " + GetName();

      if (NoClose) 
        ImGui.Begin(windowname, Flags);
      else
        ImGui.Begin(windowname, ref _isOpen, Flags);
      
      if (ImGui.IsWindowHovered()) ImGui.SetWindowFocus();
      Bounds.Location = ImGui.GetWindowPos();
      Bounds.Size = ImGui.GetWindowSize();

      if (Inspector != null)
        Inspector.RenderContent(imgui);
      else 
      {
        ImGuiUtils.TextMiddle("No object selected");
      }

      ImGui.End();
    }    
  }
  public class TileInspector : Widget.PropertiedWindow
  { 
    public override string Name { get => Tile.Name; set => Tile.Name = value;}
    public override PropertyList Properties { get => Tile.Properties; set => Tile.Properties = value; }

    public Tile Tile;
    public override void Render(ImGuiWinManager imgui)
    {
      if (Tile != null) base.Render(imgui);
    }    
    public override string GetIcon()
    {
      return IconFonts.FontAwesome5.BorderNone;
    }
    protected override void OnChangeName(string prev, string curr)
    {
      if (Tile._sheet.CreateTile(Tile)) Console.WriteLine("Created tile");
      Tile._sheet.GetCreatedTile(Tile.Id).Name = curr;
    }        
    protected override void OnChangeProperty(string name)
    {
      if (Tile._sheet.CreateTile(Tile)) Console.WriteLine("Created tile");
    }
    protected override void OnRenderAfterName()
    {
      ImGui.BeginDisabled();
      ImGui.LabelText("Id", Tile.Id.ToString());
      ImGui.LabelText("Tile", $"{Tile.Coordinates.X}x, {Tile.Coordinates.Y}y");
      ImGui.EndDisabled();
    }
  }   
}
