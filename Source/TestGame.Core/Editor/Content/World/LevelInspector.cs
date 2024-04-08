using ImGuiNET;
using Icon = IconFonts.FontAwesome5;

namespace Raven
{
  public class LevelInspector : Widget.PropertiedWindow
  {
    public override string Name { get => Level.Name; set => Level.Name = value;}
    public override PropertyList Properties { get => Level.Properties; set => Level.Properties = value; }
    public readonly Level Level;
    public bool Selected = false;

    int _currentLayer = 0;
    List<bool> _layerSelected = new List<bool>();
    public Layer CurrentLayer { get => Level.Layers.GetAtOrNull(_currentLayer); }

    public LevelInspector(Level level) => Level = level;

    public override string GetIcon() => Icon.ObjectGroup;
    public override string GetName() => "Level";   

    public override void Render(ImGuiWinManager imgui)
    {
      base.Render(imgui);
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
        if (ImGui.BeginMenu(Icon.Plus + "  Create Layer"))
        {
          if (ImGui.MenuItem("Tiled")) 
          {
            var layer = new TileLayer(Level, 16, 16);
            layer.Name = $"TiledLayer {Level.Layers.Count()+1}";
            Level.Layers.Add(layer);
          }
          if (ImGui.MenuItem("Freeform"))
          {  
            var layer = new FreeformLayer(Level);
            layer.Name = $"FreeLayer {Level.Layers.Count()+1}";
            Level.Layers.Add(layer);
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
        var lockState = (!_layerOnOptions.IsLocked) ? Icon.LockOpen + "  Unlock" : Icon.Lock + "  Lock";
        if (ImGui.MenuItem(lockState))
        {
          _layerOnOptions.IsLocked = !_layerOnOptions.IsLocked;
        }
        var visib = (_layerOnOptions.IsVisible) ? Icon.EyeSlash + "  Hide" : Icon.Eye + "  Show";
        if (ImGui.MenuItem(visib))
        {
          _layerOnOptions.IsVisible = !_layerOnOptions.IsVisible;
        }
        if (ImGui.MenuItem(Icon.Trash + "  Delete"))
        {
          Level.Layers.Remove(_layerOnOptions);  
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
      var visibState = (!layer.IsVisible) ? Icon.EyeSlash : Icon.Eye;
      if (ImGui.SmallButton(visibState))
      {
        layer.IsVisible = !layer.IsVisible;
      }
      ImGui.SameLine();
      var lockState = (layer.IsLocked) ? Icon.LockOpen: Icon.Lock;
      if (ImGui.SmallButton(lockState))
      {
        layer.IsLocked = !layer.IsLocked;
      }
      ImGui.SameLine();
      if (ImGui.SmallButton(Icon.Times))
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
      ImGui.LabelText("Width", $"{Level.ContentSize.X} px");
      ImGui.LabelText("Height", $"{Level.ContentSize.Y} px");
      var levelOffset = Level.LocalOffset.ToNumerics();
      if (ImGui.InputFloat2("Position", ref levelOffset)) Level.LocalOffset = levelOffset;
    }
    void SyncLayersGui()
    {
      if (_layerSelected.Count() != Level.Layers.Count())
      {
        _layerSelected.FalseRange(Level.Layers.Count());
        try 
        {
          _layerSelected[_currentLayer] = true;
        }
        catch (Exception) 
        {
          if (_layerSelected.Count() == 0) return;
          _currentLayer = 0;
          _layerSelected[_currentLayer] = true;
        }
      }
    }
    Layer _layerOnOptions = null;
    bool _isOpenLayerHeaderPopup = false;
    bool _isOpenLayerOptionPopup = false;
    protected override void OnRenderAfterName()
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
        for (int i = 0; i < Level.Layers.Count(); i++)
        {
          var layer = Level.Layers[i];
          var flags = ImGuiTreeNodeFlags.AllowItemOverlap 
            | ImGuiTreeNodeFlags.OpenOnArrow | ImGuiTreeNodeFlags.OpenOnDoubleClick; 

          if (_layerSelected[i]) flags |= ImGuiTreeNodeFlags.Selected;

          var layerNode = (ImGui.TreeNodeEx(layer.Name, flags));

          // Multiple select
          if (ImGui.IsItemClicked() && !ImGui.IsItemToggledOpen())
          {
            // Reset selection
            if (!ImGui.GetIO().KeyCtrl)
            {
              for (int j = 0; j < _layerSelected.Count(); j++) _layerSelected[j] = false;
            }
            _layerSelected[i] = true;
            _currentLayer = i;
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
          Level.Layers.Remove(removeLayer);
        }
        ImGui.EndChild();
      }
    }
  }
}
