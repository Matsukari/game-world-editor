using ImGuiNET;

namespace Raven
{
  public class SpriteInspector: Widget.PropertiedWindow
  {
    public override string Name { get => Sprite.Name; set => Sprite.Name = value;}
    public override PropertyList Properties { get => Sprite.Properties; set => Sprite.Properties = value; }
    public override bool CanOpen => Sprite != null;
     
    public Sprite Sprite;
    Sheet _sheet { get => Sprite._sheet; }

    public override void Render(ImGuiWinManager imgui)
    {
      if (Sprite != null) base.Render(imgui);
    }    
    public override string GetIcon()
    {
      return IconFonts.FontAwesome5.GripHorizontal;
    } 
    protected override void OnChangeProperty(string name)
    {
      foreach (var tile in Sprite._tiles)
      {
        Console.WriteLine("Changing properties of sprite");
         var existingTile = Sprite._sheet.GetTileData(tile);
        // If not created yet
        Sprite._sheet.CreateTile(existingTile);
        existingTile.Properties.OverrideOrAddAll(Properties);
      }
    }
    protected override void OnChangeName(string prev, string curr)
    {
      foreach (var tile in Sprite._tiles)
      {
        // If not created yet
        var existingTile = Sprite._sheet.GetTileData(tile);
        Sprite._sheet.CreateTile(existingTile);
        existingTile.Name = curr;
      }
    } 
    public override void OnRender(ImGuiWinManager imgui)
    {
      if (ImGui.CollapsingHeader("Sprite", ImGuiTreeNodeFlags.DefaultOpen)) 
      {
        NameInput();
        ImGui.BeginDisabled();
        ImGui.LabelText("Tiles", Sprite._tiles.Count.ToString());
        ImGui.LabelText("Region", Sprite.Region.RenderStringFormat());
        ImGui.EndDisabled();
      }

      if (ImGui.CollapsingHeader("Tiles", ImGuiTreeNodeFlags.DefaultOpen))
      {
        var properties = new Dictionary<string, List<(object, Tile)>>();
        var namedTiles = new List<Tile>();
        var tiles = _sheet.GetTiles(Sprite.Region.ToRectangleF());
        foreach (var tile in tiles)
        {
          var tileCoord = _sheet.GetTileCoord(tile); 
          var tileData = _sheet.CustomTileExists(tileCoord);
          if (tileData != null) 
          {
            foreach (var prop in tileData.Properties)
            {
              if (!properties.ContainsKey(prop.Name))
                properties[prop.Name] = new List<(object, Tile)>();
              properties[prop.Name].Add((prop.Value, tileData));
            }
            if (tileData.Properties.Data.Count == 0)
            {
              namedTiles.Add(tileData);
            }
          }
        }

        int i = 0;
        foreach (var prop in properties)
        {
          ImGui.PushID(i);
          var propNode = ImGui.TreeNodeEx(prop.Key, ImGuiTreeNodeFlags.AllowItemOverlap);
          ImGui.SameLine();
          ImGui.TextDisabled($" ({prop.Value.Count()} matches)");

          if (propNode)
          {
            int j = 0;
            foreach (var item in prop.Value)  
            {
              ImGui.PushID(j);
              var mainName = $"Tile {item.Item2.Id}";
              var subName = (item.Item2.Name != null) ? item.Item2.Name : "No Name";
              var itemNode = ImGui.TreeNodeEx(mainName, ImGuiTreeNodeFlags.AllowItemOverlap);
              ImGui.SameLine();
              ImGui.TextDisabled($" ({subName})");

              if (itemNode)
              {
                ImGui.BeginDisabled();
                ImGuiUtils.DrawImage(item.Item2.Sprite, new System.Numerics.Vector2(ImGui.GetContentRegionAvail().X));
                PropertiesRenderer.RenderBestMatch(item.Item1);
                ImGui.EndDisabled();
                ImGui.TreePop();
              }
              ImGui.PopID();
            }
            ImGui.TreePop();
          }
          ImGui.PopID();
        }
      }
      PropertiesRenderer.Render(imgui, this, OnChangeProperty);
    }
  }
}
