
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
    internal WorldGui _worldGui;
    internal List<bool> _levelSelected = new List<bool>();
    public bool Selected = false;
    public Color TileActiveGridColor = new Color(0.2f, 0.2f, 0.2f, 0.4f);
    public Color TileInactiveGridColor = new Color(0.1f, 0.1f, 0.1f, 0.4f);
    
    public LevelGui(Level level, WorldGui gui)
    {
      _level = level;
      _worldGui = gui;
    }
    public override string GetIcon()
    {
      return IconFonts.FontAwesome5.ObjectGroup;
    }
    public override string GetName()
    {
      return "Level";
    }
        
    public void Render(Batcher batcher, Camera camera)
    {
      foreach (var layer in _level.Layers)
      {
        if (layer is TileLayer tileLayer && _worldGui.IsDrawTileLayerGrid) 
        {
          var color = (_level.CurrentLayer != null && _level.CurrentLayer.Name == tileLayer.Name) ? TileActiveGridColor : TileInactiveGridColor;
          Guidelines.GridLines.RenderGridLines(batcher, camera, layer.Bounds.Location, color, tileLayer.TilesQuantity, tileLayer.TileSize.ToVector2());
        }
      }
    }
    public override void RenderImGui(PropertiesRenderer renderer)
    {
      base.RenderImGui(renderer);
      DrawLayerOptionsPopup();
      DrawLayerHeaderPopup();
    }
    void DrawLayerHeaderPopup()
    {
      if (_isOpenLayerHeaderPopup)
      {
        _isOpenLayerHeaderPopup = false;
        ImGui.OpenPopup("layer-header-options-popup");
      }
      if (ImGui.BeginPopupContextItem("layer-header-options-popup"))
      {
        if (ImGui.BeginMenu(IconFonts.FontAwesome5.Plus + "  Create Layer"))
        {
          if (ImGui.MenuItem("Tiled")) 
          {
            var layer = new TileLayer(_level, 16, 16);
            layer.Name = $"Layer {_level.Layers.Count()+1}";
            _level.Layers.Add(layer);
          }
          if (ImGui.MenuItem("Freeform"))
          { 
          }
          ImGui.EndMenu();
        }
        ImGui.EndPopup();
      }
    }
    void DrawLayerOptionsPopup()
    {
      if (_isOpenLayerOptionPopup)
      {
        _isOpenLayerOptionPopup = false;
        ImGui.OpenPopup("layer-options-popup");
      }
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
    void DrawLayerOptions(Layer layer, ref Layer removeLayer)
    {
      // Options next to name
      ImGui.SameLine();
      ImGui.Dummy(new System.Numerics.Vector2(ImGui.GetWindowSize().X - ImGui.CalcTextSize(layer.Name).X - 140, 0f));
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

    }
    void DrawLayerContent(Layer layer)
    {
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
    }
    void DrawLevelContent()
    {
      ImGui.LabelText("Width", $"{_level.ContentSize.X} px");
      ImGui.LabelText("Height", $"{_level.ContentSize.Y} px");
      var levelOffset = _level.LocalOffset.ToNumerics();
      if (ImGui.InputFloat2("Position", ref levelOffset)) _level.LocalOffset = levelOffset;
    }
    void SyncLayersGui()
    {
      if (_levelSelected.Count() != _level.Layers.Count())
      {
        _levelSelected.Clear();
        foreach (var layer in _level.Layers)
        {
          _levelSelected.Add(false);
          if (layer.IsCurrentLayerInLevel) _levelSelected[_levelSelected.Count()-1] = true;
        }
      }
    }
    Layer _layerOnOptions = null;
    bool _isOpenLayerHeaderPopup = false;
    bool _isOpenLayerOptionPopup = false;
    protected override void OnRenderAfterName(PropertiesRenderer renderer)
    {
      SyncLayersGui();
      DrawLevelContent();
      var header = ImGui.CollapsingHeader("Layers", ImGuiTreeNodeFlags.DefaultOpen);

      // The hader
      if (ImGui.IsItemClicked(ImGuiMouseButton.Right))
      {
        _isOpenLayerHeaderPopup = true;
      }
      if (header)
      {
        ImGui.BeginChild($"level-layers-content-child", new System.Numerics.Vector2(ImGui.GetWindowWidth(), 200), false, ImGuiWindowFlags.AlwaysVerticalScrollbar);
        // Draw layers
        Layer removeLayer = null;
        for (int i = 0; i < _level.Layers.Count(); i++)
        {
          var layer = _level.Layers[i];
          var flags = ImGuiTreeNodeFlags.AllowItemOverlap 
            | ImGuiTreeNodeFlags.OpenOnArrow | ImGuiTreeNodeFlags.OpenOnDoubleClick; 

          if (_levelSelected[i]) flags |= ImGuiTreeNodeFlags.Selected;

          var layerNode = (ImGui.TreeNodeEx(layer.Name, flags));

          // Multiple select
          if (ImGui.IsItemClicked() && !ImGui.IsItemToggledOpen())
          {
            // Reset selection
            if (!ImGui.GetIO().KeyCtrl)
            {
              for (int j = 0; j < _levelSelected.Count(); j++) _levelSelected[j] = false;
            }
            _levelSelected[i] = true;
            _level.CurrentLayer = layer;
          }

          // Layer options; select
          if (ImGui.IsItemClicked(ImGuiMouseButton.Right)) 
          {
            _layerOnOptions = layer;
            _isOpenLayerOptionPopup = true;
          }
          DrawLayerOptions(layer, ref removeLayer);
         
          // Layer content
          if (layerNode)
          { 
            ImGui.PushID($"layer-{layer.Name}-content");
            DrawLayerContent(layer);
            ImGui.PopID();
            ImGui.TreePop();
          }
        }
        if (removeLayer != null)
        {
          _level.Layers.Remove(removeLayer);
          _level.CurrentLayer = null;
        }
        ImGui.EndChild();
      }
    }
  }
}
