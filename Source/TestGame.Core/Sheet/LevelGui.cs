
using Raven.Sheet.Sprites;
using Microsoft.Xna.Framework;
using Nez;
using ImGuiNET;

namespace Raven.Sheet
{
  public class LevelGui : Propertied
  {
    public override string Name { get => _level.Name; set => _level.Name = value;}
    internal Level _level;
    public bool Selected = false;
    public Color TileActiveGridColor = new Color(0.1f, 0.1f, 0.1f, 0.4f);
    public Color TileInactiveGridColor = new Color(0.1f, 0.1f, 0.1f, 0.1f);
    public LevelGui(Level level)
    {
      _level = level;
    }
    public override string GetIcon()
    {
      return IconFonts.FontAwesome5.LayerGroup;
    }
    public void Render(Batcher batcher, Camera camera)
    {
      foreach (var layer in _level.Layers)
      {
        if (layer is TileLayer tileLayer) 
        {
          var color = (_level.CurrentLayer.Name == tileLayer.Name) ? TileActiveGridColor : TileInactiveGridColor;
          Guidelines.GridLines.RenderGridLines(batcher, camera, layer.Bounds.Location, color, tileLayer.TilesQuantity, tileLayer.TileSize.ToVector2());
        }
      }
    }
    public override void RenderImGui(PropertiesRenderer renderer)
    {
      base.RenderImGui(renderer);
      if (ImGui.BeginPopupContextItem("layer-options-popup") && _layerOnOptions != null)
      {
        var lockState = (_layerOnOptions.IsLocked) ? IconFonts.FontAwesome5.LockOpen + "  Unlock" : IconFonts.FontAwesome5.Lock + "  Lock";
        if (ImGui.MenuItem(lockState))
        {
          _layerOnOptions.IsLocked = !_layerOnOptions.IsLocked;
        }
        var visib = (_layerOnOptions.IsVisible) ? IconFonts.FontAwesome5.EyeSlash + "  Hide" : IconFonts.FontAwesome5.Eye + "  Show";
        if (ImGui.MenuItem(visib))
        {
          _layerOnOptions.IsVisible = !_layerOnOptions.IsVisible;
        }
        if (ImGui.MenuItem(IconFonts.FontAwesome5.Trash + "  Delete"))
        {
          _level.Layers.Remove(_layerOnOptions);  
        }
        ImGui.EndPopup();
      }
    } 
    Layer _layerOnOptions = null;
    protected override void OnRenderAfterName(PropertiesRenderer renderer)
    {
      ImGui.LabelText("Width", $"{_level.ContentSize.X} px");
      ImGui.LabelText("Height", $"{_level.ContentSize.Y} px");
      var levelOffset = _level.LocalOffset.ToNumerics();
      if (ImGui.InputFloat2("Position", ref levelOffset)) _level.LocalOffset = levelOffset;

      if (ImGui.CollapsingHeader("Layers", ImGuiTreeNodeFlags.DefaultOpen))
      {
        Layer removeLayer = null;
        foreach (var layer in _level.Layers)
        {
          if (ImGui.TreeNodeEx(layer.Name, ImGuiTreeNodeFlags.DefaultOpen | ImGuiTreeNodeFlags.AllowItemOverlap))
          {
            // Options next to name
            ImGui.SameLine();
            ImGui.Dummy(new System.Numerics.Vector2(ImGui.GetWindowSize().X - ImGui.CalcTextSize(layer.Name).X - 200, 0f));
            ImGui.SameLine();
            ImGui.PushID($"level-{layer.Name}-id");
            var visibState = (!layer.IsVisible) ? IconFonts.FontAwesome5.EyeSlash : IconFonts.FontAwesome5.Eye;
            if (ImGui.SmallButton(visibState))
            {
              layer.IsVisible = !layer.IsVisible;
            }
            ImGui.SameLine();
            var lockState = (!layer.IsLocked) ? IconFonts.FontAwesome5.LockOpen: IconFonts.FontAwesome5.Lock;
            if (ImGui.SmallButton(lockState))
            {
              layer.IsLocked = !layer.IsLocked;
            }
            ImGui.SameLine();
            if (ImGui.SmallButton(IconFonts.FontAwesome5.Times))
            {
              removeLayer = layer;
            }
            ImGui.PopID();

            if (ImGui.IsMouseClicked(ImGuiMouseButton.Right)) 
            {
              _layerOnOptions = layer;
              ImGui.OpenPopup("layer-options-popup");
            }
            if (!layer.IsVisible || layer.IsLocked) ImGui.BeginDisabled();
            var name = layer.Name;
            var offset = layer.Offset.ToNumerics();
            if (ImGui.InputText("Name", ref name, 20, ImGuiInputTextFlags.EnterReturnsTrue)) layer.Name = name;
            if (layer.IsVisible && !layer.IsLocked) ImGui.BeginDisabled();
            ImGui.LabelText("Type", layer.GetType().Name);
            ImGui.LabelText("Level", layer.Level.Name);
            if (layer.IsVisible && !layer.IsLocked) ImGui.EndDisabled();
            ImGui.InputFloat("Opacity", ref layer.Opacity);
            if (ImGui.InputFloat2("Offset", ref offset)) layer.Offset = offset.ToVector2();
            if (!layer.IsVisible || layer.IsLocked) ImGui.EndDisabled();
            // ImGui.GetWindowDrawList().AddRect(ImGui.GetItemRectMin(), ImGui.GetItemRectMax(), GuiColors.Get(ImGuiCol.Border));
            ImGui.TreePop();
          }
        }
        if (removeLayer != null)
        {
          _level.Layers.Remove(removeLayer);
          _level.CurrentLayer = null;
        }
      }
    }
  }
}
